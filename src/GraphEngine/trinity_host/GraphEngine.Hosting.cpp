#include <functional>
#include <io>
#include "GraphEngine.Hosting.h"
#include "mscoree.h"
#include <linq.hpp>

using namespace cpplinq;
using namespace Trinity;

#if defined(TRINITY_PLATFORM_WINDOWS)
static const String coreCLRDll("coreclr.dll");
#elif defined(TRINITY_PLATFORM_LINUX)
static const String coreCLRDll("libcoreclr.so");
#elif defined(TRINITY_PLATFORM_DARWIN)
static const String coreCLRDll("libcoreclr.dylib");
#endif

// sample host here: https://github.com/dotnet/samples/blob/master/core/hosting/host.cpp
// actual host here: https://github.com/dotnet/coreclr/blob/release/2.0.0/src/coreclr/hosts/coreconsole/coreconsole.cpp

#define MAX_LONGPATH 1024

String withstr(std::function<DWORD(const Array<u16char>&)> fn)
{
    Array<u16char> charray(MAX_LONGPATH);
    auto len = fn(charray);
    return String::FromWcharArray(charray, len);
}

// Encapsulates the environment that CoreCLR will run in, including the TPALIST
class HostEnvironment
{
    // The path to this module
    String m_hostPath;

    // The path to the directory containing this module
    String m_hostDirectoryPath;

    // The name of this module, without the path
    String m_hostExeName;

    String m_appPaths;

    // The list of paths to the assemblies that will be trusted by CoreCLR
    String m_tpaList;

    ICLRRuntimeHost4* m_CLRRuntimeHost;

    HMODULE m_coreCLRModule;

    DWORD m_domainId;

    // The path to the directory that CoreCLR is in
    String m_coreCLRDirectoryPath;

    List<String> GetCoreCLRPaths()
    {
        auto base_path =
            from(Environment::Run("dotnet --info"))
            .where([](auto &p) {return p.Contains("Base Path:"); })
            .first_or_default();
        auto idx = base_path.IndexOf(':');
        if (idx == String::npos)
        {
            return {};
        }
        else
        {
            base_path = base_path.Substring(idx + 1).Trim();
            if (base_path.Empty())
            {
                return {};
            }

            if (*base_path.rbegin() == Path::DirectorySeparator)
            {
                base_path.PopBack();
            }

            base_path = Path::GetParent(Path::GetParent(base_path));
            base_path = Path::Combine(base_path, "shared", "Microsoft.NETCore.App");
            return
                from(Directory::GetDirectories(base_path))
                .select(Path::GetFileName)
                // TODO proper version validation
                .where([](auto& p) { return p.StartsWith("2."); })
                .select([&](auto& p) { return Path::Combine(base_path, p); })
                .to_vector();
            //Path::GetDirectoryName
        }
    }

    // Attempts to load CoreCLR.dll from the given directory.
    // On success pins the dll, sets m_coreCLRDirectoryPath and returns the HMODULE.
    // On failure returns nullptr.
    HMODULE TryLoadCoreCLR(const String& directoryPath)
    {
        String  coreCLRPath = Path::Combine(directoryPath, coreCLRDll);
        HMODULE result      = ::LoadLibraryExW(coreCLRPath.ToWcharArray(), NULL, 0);

        if (!result)
        {
            return nullptr;
        }

        // Pin the module - CoreCLR.dll does not support being unloaded.
        HMODULE dummy_coreCLRModule;
        if (!::GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_PIN, coreCLRPath.ToWcharArray(), &dummy_coreCLRModule))
        {
            return nullptr;
        }

        return result;
    }

    TrinityErrorCode _Init()
    {
        // Discover the path to this exe's module. All other files are expected to be in the same directory.
        m_hostPath = withstr([](const Array<u16char>& charray) { return ::GetModuleFileNameW(NULL, (LPWSTR)charray.data(), MAX_LONGPATH); });
        m_hostDirectoryPath = Path::GetDirectoryName(m_hostPath);
        m_hostExeName = Path::GetFileName(m_hostPath);

        //TODO guess coreclr path
        for (auto& coreCLRPath : GetCoreCLRPaths())
        {
            m_coreCLRModule = TryLoadCoreCLR(coreCLRPath);
            if (m_coreCLRModule)
            {
                break;
            }
        }

        if (m_coreCLRModule)
        {
            // Save the directory that CoreCLR was found in
            m_coreCLRDirectoryPath = withstr([&](auto& charray) { return ::GetModuleFileNameW(m_coreCLRModule, (LPWSTR)charray.data(), MAX_LONGPATH); });
            m_coreCLRDirectoryPath = Path::GetDirectoryName(m_coreCLRDirectoryPath);
        }
        else
        {
            return TrinityErrorCode::E_LOAD_FAIL;
        }

        String appPath = m_coreCLRDirectoryPath;

        if (!m_appPaths.Empty())
        {
            appPath += ";" + m_appPaths;
        }

        String appNiPath = appPath;

        // Construct native search directory paths
        String nativeDllSearchDirs = appPath + ";" + m_coreCLRDirectoryPath;

        // Start the CoreCLR
        ICLRRuntimeHost4 *host = CreateCLRRuntimeHost();
        if (!host)
        {
            return TrinityErrorCode::E_FAILURE;
        }

        // Default startup flags
        HRESULT hr = host->SetStartupFlags((STARTUP_FLAGS)
            (STARTUP_FLAGS::STARTUP_LOADER_OPTIMIZATION_SINGLE_DOMAIN |
             STARTUP_FLAGS::STARTUP_SINGLE_APPDOMAIN |
             STARTUP_FLAGS::STARTUP_CONCURRENT_GC |
             STARTUP_FLAGS::STARTUP_SERVER_GC));
        if (FAILED(hr))
        {
            return TrinityErrorCode::E_FAILURE;
        }

        hr = host->Start();
        if (FAILED(hr))
        {
            return TrinityErrorCode::E_FAILURE;
        }

        //-------------------------------------------------------------

        // Create an AppDomain

        // Allowed property names:
        // APPBASE
        // - The base path of the application from which the exe and other assemblies will be loaded
        //
        // TRUSTED_PLATFORM_ASSEMBLIES
        // - The list of complete paths to each of the fully trusted assemblies
        //
        // APP_PATHS
        // - The list of paths which will be probed by the assembly loader
        //
        // APP_NI_PATHS
        // - The list of additional paths that the assembly loader will probe for ngen images
        //
        // NATIVE_DLL_SEARCH_DIRECTORIES
        // - The list of paths that will be probed for native DLLs called by PInvoke
        //

        auto _tpalist = GetTpaList().ToWcharArray();
        auto _appPath = appPath.ToWcharArray();
        auto _appNiPath = appNiPath.ToWcharArray();
        auto _ndsd = nativeDllSearchDirs.ToWcharArray();

        const wchar_t *property_keys[] ={
            L"TRUSTED_PLATFORM_ASSEMBLIES",
            L"APP_PATHS",
            L"APP_NI_PATHS",
            L"NATIVE_DLL_SEARCH_DIRECTORIES",
        };
        const wchar_t *property_values[] ={
            // TRUSTED_PLATFORM_ASSEMBLIES
            _tpalist,
            // APP_PATHS
            _appPath,
            // APP_NI_PATHS
            _appNiPath,
            // NATIVE_DLL_SEARCH_DIRECTORIES
            _ndsd,
        };


        //log << W("Creating an AppDomain") << Logger::endl;
        //log << W("TRUSTED_PLATFORM_ASSEMBLIES=") << property_values[0] << Logger::endl;
        //log << W("APP_PATHS=") << property_values[1] << Logger::endl;
        //log << W("APP_NI_PATHS=") << property_values[2] << Logger::endl;
        //log << W("NATIVE_DLL_SEARCH_DIRECTORIES=") << property_values[3] << Logger::endl;

        hr = host->CreateAppDomainWithManager(
            GetHostExeName().ToWcharArray(),   // The friendly name of the AppDomain
            // Flags:
            // APPDOMAIN_ENABLE_PLATFORM_SPECIFIC_APPS
            // - By default CoreCLR only allows platform neutral assembly to be run. To allow
            //   assemblies marked as platform specific, include this flag
            //
            // APPDOMAIN_ENABLE_PINVOKE_AND_CLASSIC_COMINTEROP
            // - Allows sandboxed applications to make P/Invoke calls and use COM interop
            //
            // APPDOMAIN_SECURITY_SANDBOXED
            // - Enables sandboxing. If not set, the app is considered full trust
            //
            // APPDOMAIN_IGNORE_UNHANDLED_EXCEPTION
            // - Prevents the application from being torn down if a managed exception is unhandled
            //
            APPDOMAIN_ENABLE_PLATFORM_SPECIFIC_APPS |
            APPDOMAIN_ENABLE_PINVOKE_AND_CLASSIC_COMINTEROP |
            APPDOMAIN_DISABLE_TRANSPARENCY_ENFORCEMENT,
            NULL,                                      // Name of the assembly that contains the AppDomainManager implementation
            NULL,                                      // The AppDomainManager implementation type name
            sizeof(property_keys) / sizeof(wchar_t*),  // The number of properties
            property_keys,
            property_values,
            &m_domainId);

        if (FAILED(hr))
        {
            return TrinityErrorCode::E_FAILURE;
        }

        return TrinityErrorCode::E_SUCCESS;

    }

    bool TPAListContainsFile(const String& fileNameWithoutExtension, const Array<String>& extensions)
    {
        return std::any_of(extensions.begin(), extensions.end(), [&, this](auto &ext) {
            // Note, ext starts with '*' so skip it.
            auto fileName = String("\\") + fileNameWithoutExtension + ext.Substring(1) + String(";");
            return m_tpaList.Contains(fileName);
        });
    }

    String RemoveExtensionAndNi(String fileName)
    {
        // Remove extension, if it exists
        auto idx = fileName.IndexOf('.');
        if (idx != String::npos)
        {
            fileName = fileName.Substring(0, idx);

            // Check for .ni
            size_t len = fileName.Length();
            if (len > 3 &&
                fileName[len - 1] == L'i' &&
                fileName[len - 2] == L'n' &&
                fileName[len - 3] == L'.')
            {
                fileName = fileName.Substring(0, len - 3);
            }
        }

        return fileName;
    }

    void AddFilesFromDirectoryToTPAList(const String& targetPath, const Array<String>& extensions)
    {
        for (auto& ext : extensions)
        {
            String pattern = Path::Combine(targetPath, ext);
            WIN32_FIND_DATA data;
            HANDLE findHandle = FindFirstFile(pattern.ToWcharArray(), &data);

            if (findHandle != INVALID_HANDLE_VALUE)
            {
                do
                {
                    if (!(data.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY))
                    {
                        // It seems that CoreCLR doesn't always use the first instance of an assembly on the TPA list (ni's may be preferred
                        // over il, even if they appear later). So, only include the first instance of a simple assembly name to allow
                        // users the opportunity to override Framework assemblies by placing dlls in %CORE_LIBRARIES%

                        // ToLower for case-insensitive comparisons
                        // Remove extension
                        String fileName = String::FromWcharArray(data.cFileName, wcslen(data.cFileName)).ToLower();
                        String fileNameWithoutExtension = RemoveExtensionAndNi(fileName);

                        // Add to the list if not already on it
                        if (!TPAListContainsFile(fileNameWithoutExtension, extensions))
                        {
                            m_tpaList.Append(targetPath);
                            m_tpaList.Append(fileName);
                            m_tpaList.Append(L';');
                        }
                    }
                } while (0 != FindNextFile(findHandle, &data));

                FindClose(findHandle);
            }
        }
    }


public:

    HostEnvironment(IN const String& appPaths,
                    OUT TrinityErrorCode& eresult) :
        m_CLRRuntimeHost(nullptr),
        m_coreCLRModule(nullptr),
        m_appPaths(appPaths)
    {
        eresult = _Init();
    }

    ~HostEnvironment()
    {
        if (m_coreCLRModule)
        {
            // Free the module. This is done for completeness, but in fact CoreCLR.dll 
            // was pinned earlier so this call won't actually free it. The pinning is
            // done because CoreCLR does not support unloading.
            ::FreeLibrary(m_coreCLRModule);
        }
    }

    // Returns the semicolon-separated list of paths to runtime dlls that are considered trusted.
    // On first call, scans the coreclr directory for dlls and adds them all to the list.
    const String GetTpaList()
    {
        if (m_tpaList.Empty())
        {
            Array<String> rgTPAExtensions ={
                        "*.ni.dll",		// Probe for .ni.dll first so that it's preferred if ni and il coexist in the same dir
                        "*.dll",
                        "*.ni.exe",
                        "*.exe",
            };

            AddFilesFromDirectoryToTPAList(m_coreCLRDirectoryPath, rgTPAExtensions);
        }

        return m_tpaList;
    }

    // Returns the path to the host module
    const String GetHostPath() const
    {
        return m_hostPath;
    }

    // Returns the path to the host module
    const String GetHostExeName() const
    {
        return m_hostExeName;
    }

    // Returns the ICLRRuntimeHost4 reference AS-IS. nullptr if uninitialized.
    ICLRRuntimeHost4* GetCLRRuntimeHost() const
    {
        return m_CLRRuntimeHost;
    }

    // Returns the ICLRRuntimeHost4 instance, loading it from CoreCLR.dll if necessary, or nullptr on failure.
    ICLRRuntimeHost4* CreateCLRRuntimeHost()
    {
        if (!m_CLRRuntimeHost)
        {
            if (!m_coreCLRModule)
            {
                return nullptr;
            }

            FnGetCLRRuntimeHost pfnGetCLRRuntimeHost =
                (FnGetCLRRuntimeHost)::GetProcAddress(m_coreCLRModule, "GetCLRRuntimeHost");

            if (!pfnGetCLRRuntimeHost)
            {
                return nullptr;
            }

            HRESULT hr = pfnGetCLRRuntimeHost(IID_ICLRRuntimeHost4, (IUnknown**)&m_CLRRuntimeHost);
            if (FAILED(hr))
            {
                return nullptr;
            }
        }

        return m_CLRRuntimeHost;
    }

    // Returns the domain id.
    const int GetDomainId() const
    {
        return m_domainId;
    }

    TrinityErrorCode GetFunction(
        IN const String& assemblyName,
        IN const String& className,
        IN const String& methodName,
        OUT INT_PTR& init_func) const
    {
        auto hr = m_CLRRuntimeHost->CreateDelegate(
            m_domainId,
            Path::GetFileNameWithoutExtension(assemblyName).ToWcharArray(),
            className.ToWcharArray(),
            methodName.ToWcharArray(),
            &init_func);

        if (FAILED(hr))
        {
            return TrinityErrorCode::E_FAILURE;
        }

        return TrinityErrorCode::E_SUCCESS;
    }
};

TrinityErrorCode GraphEngineInit_impl(
    IN int n_apppaths,
    IN wchar_t** lp_apppaths,
    OUT void*& lpenv)
{
    if (n_apppaths < 0)
    {
        return TrinityErrorCode::E_INVALID_ARGUMENTS;
    }

    TrinityErrorCode err;
    Array<String> apppaths(n_apppaths);
    for (int i=0; i < n_apppaths; ++i)
    {
        apppaths[i] = lp_apppaths[i];
    }
    lpenv         = new HostEnvironment(String::Join(";", apppaths), err);
    return err;
}


DLL_EXPORT TrinityErrorCode GraphEngineInit(
    IN int n_apppaths,
    IN wchar_t** lp_apppaths,
    OUT void*& lpenv)
{
    return GraphEngineInit_impl(n_apppaths, lp_apppaths, lpenv);
}

DLL_EXPORT TrinityErrorCode GraphEngineGetFunction(
    IN const void* _lpenv,
    IN wchar_t* lp_entry_asm,
    IN wchar_t* lp_entry_class,
    IN wchar_t* lp_entry_method,
    OUT void** lp_func
)
{
    if (nullptr == _lpenv)
    {
        return TrinityErrorCode::E_INVALID_ARGUMENTS;
    }

    INT_PTR iptr;

    auto lpenv = reinterpret_cast<const HostEnvironment*>(_lpenv);
    auto result = lpenv->GetFunction(String(lp_entry_asm),
                                     String(lp_entry_class),
                                     String(lp_entry_method),
                                     iptr);

    *lp_func = (void*)iptr;
    return result;
}

DLL_EXPORT TrinityErrorCode GraphEngineUninit(
    IN const void* _lpenv)
{
    if (nullptr == _lpenv)
    {
        return TrinityErrorCode::E_INVALID_ARGUMENTS;
    }

    auto lpenv = reinterpret_cast<const HostEnvironment*>(_lpenv);

    ICLRRuntimeHost4* host = lpenv->GetCLRRuntimeHost();
    if (nullptr == host)
    {
        return TrinityErrorCode::E_INVALID_ARGUMENTS;
    }

    //-------------------------------------------------------------
    // Unload the AppDomain
    int exitCode = 0;

    HRESULT hr = host->UnloadAppDomain2(
        lpenv->GetDomainId(),
        true,
        (int *)&exitCode);                          // Wait until done

    if (FAILED(hr))
    {
        return TrinityErrorCode::E_UNLOAD_FAIL;
    }

    //-------------------------------------------------------------
    // Stop the host

    hr = host->Stop();

    if (FAILED(hr))
    {
        return TrinityErrorCode::E_UNLOAD_FAIL;
    }

    //-------------------------------------------------------------

    // Release the reference to the host

    host->Release();

    return TrinityErrorCode::E_SUCCESS;
}