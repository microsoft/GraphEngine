#include <functional>
#include <io>
#include "GraphEngine.Hosting.h"
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

#if defined(TRINITY_PLATFORM_WINDOWS)
#include "mscoree.h"
const char* ppath_sep = "\\";
const char* penv_sep  = ";";
using pclrhost_t = ICLRRuntimeHost4 * ;
using pdll_t     = HMODULE;
using error_t    = HRESULT;
using clr_char_t = wchar_t;
using domainid_t = DWORD;
#define t(x) L##x
#define str_buff(x) (x.ToWcharArray())
#else
#include "coreclrhost.h"
#include <dlfcn.h>
#include <dirent.h>
const char* ppath_sep = "/";
const char* penv_sep  = ":";
using pclrhost_t = void*;
using pdll_t     = void*;
using error_t    = int;
using INT_PTR    = void*;
using clr_char_t = char;
using domainid_t = unsigned int;
#define t(x) x
#define str_buff(x) (x.Data())

#ifndef SUCCEEDED
#define SUCCEEDED(Status) ((Status) >= 0)
#endif // !SUCCEEDED

#endif

// sample host here: https://github.com/dotnet/samples/blob/master/core/hosting/host.cpp
// actual host here: https://github.com/dotnet/coreclr/blob/release/2.0.0/src/coreclr/hosts/coreconsole/coreconsole.cpp

#define MAX_LONGPATH 1024

// Encapsulates the environment that CoreCLR will run in, including the TPALIST
class HostEnvironment
{
    String m_appPaths;

    // The list of paths to the assemblies that will be trusted by CoreCLR
    String m_tpaList;

    pclrhost_t m_CLRRuntimeHost;
    pdll_t     m_coreCLRModule;
    domainid_t m_domainId;

#if !defined(TRINITY_PLATFORM_WINDOWS)
    coreclr_initialize_ptr       m_pfinit;
    coreclr_execute_assembly_ptr m_pfexec;
    coreclr_create_delegate_ptr  m_pfgetfunc;
    coreclr_shutdown_2_ptr       m_pfshutdown;
#endif

    // The path to the directory that CoreCLR is in
    String m_coreCLRDirectoryPath;

    std::vector<String> GetCoreCLRPaths()
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
    pdll_t TryLoadCoreCLR(const String& directoryPath)
    {
        String  coreCLRPath = Path::Combine(directoryPath, coreCLRDll);
        pdll_t  result;

#if defined(TRINITY_PLATFORM_WINDOWS)
        result = ::LoadLibraryExW(coreCLRPath.ToWcharArray(), NULL, 0);

        if (!result)
        {
            return nullptr;
        }

        // Pin the module - CoreCLR.dll does not support being unloaded.
        pdll_t dummy_coreCLRModule;
        if (!::GetModuleHandleExW(GET_MODULE_HANDLE_EX_FLAG_PIN, coreCLRPath.ToWcharArray(), &dummy_coreCLRModule))
        {
            return nullptr;
        }
#else
        result = ::dlopen(coreCLRPath.Data(), RTLD_NOW | RTLD_LOCAL);
#endif

        return result;
    }

    TrinityErrorCode _Init()
    {
        for (auto& coreCLRPath : GetCoreCLRPaths())
        {
            m_coreCLRModule = TryLoadCoreCLR(coreCLRPath);
            if (m_coreCLRModule)
            {
                // Save the directory that CoreCLR was found in
                m_coreCLRDirectoryPath = coreCLRPath;
                break;
            }
        }

        if (!m_coreCLRModule)
        {
            return TrinityErrorCode::E_LOAD_FAIL;
        }

        String appPath = m_coreCLRDirectoryPath;

        //  include subdirectories of anything in m_appPaths
        //  usually the resource satellites are placed in these subdirectories.
        auto split_entries = m_appPaths.Split(penv_sep, String::StringSplitOptions::RemoveEmptyEntries);
        for (auto &p : from(split_entries).select_many(Directory::GetDirectories))
        {
            m_appPaths += penv_sep + p;
        }

        if (!m_appPaths.Empty())
        {
            appPath += penv_sep + m_appPaths;
        }

        String appNiPath = appPath;

        // Construct native search directory paths
        String nativeDllSearchDirs = appPath + penv_sep + m_coreCLRDirectoryPath;

        // Start the CoreCLR
        return StartCoreCLR(appPath, appNiPath, appNiPath);
    }

    bool TPAListContainsFile(const String& fileNameWithoutExtension, const Array<String>& extensions)
    {
        return std::any_of(extensions.begin(), extensions.end(), [&, this](auto &ext) {
            auto fileName = String(ppath_sep) + fileNameWithoutExtension + ext + String(penv_sep);
            return m_tpaList.Contains(fileName);
        });
    }

    String RemoveExtensionAndNi(String fileName)
    {
        // Remove extension, if it exists
        auto idx = fileName.IndexOfLast('.');
        if (idx != String::npos)
        {
            fileName = fileName.Substring(0, idx);

            // Check for .ni
            size_t len = fileName.Length();
            if (len > 3 &&
                fileName[len - 1] == 'i' &&
                fileName[len - 2] == 'n' &&
                fileName[len - 3] == '.')
            {
                fileName = fileName.Substring(0, len - 3);
            }
        }

        return fileName;
    }

    void AddFilesFromDirectoryToTPAList(const String& targetPath, const Array<String>& extensions)
    {
        for (auto &fileName : Directory::GetFiles(targetPath, extensions))
        {
            // It seems that CoreCLR doesn't always use the first instance of an assembly on the TPA list (ni's may be preferred
            // over il, even if they appear later). So, only include the first instance of a simple assembly name to allow
            // users the opportunity to override Framework assemblies by placing dlls in %CORE_LIBRARIES%

#if defined(TRINITY_PLATFORM_WINDOWS)
            // ToLower for case-insensitive comparisons
            fileName = fileName.ToLower();
#endif
            // Remove extension
            String fileNameWithoutExtension = RemoveExtensionAndNi(fileName);

            // Add to the list if not already on it
            if (!TPAListContainsFile(fileNameWithoutExtension, extensions))
            {
                m_tpaList.Append(Path::Combine(targetPath, fileName));
                m_tpaList.Append(penv_sep);
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
#if defined(TRINITY_PLATFORM_WINDOWS)
            // Free the module. This is done for completeness, but in fact CoreCLR.dll 
            // was pinned earlier so this call won't actually free it. The pinning is
            // done because CoreCLR does not support unloading.
            ::FreeLibrary(m_coreCLRModule);
#else
            dlclose(m_coreCLRModule);
#endif
        }
    }

    // Returns the semicolon-separated list of paths to runtime dlls that are considered trusted.
    // On first call, scans the coreclr directory for dlls and adds them all to the list.
    const String GetTpaList()
    {
        if (m_tpaList.Empty())
        {
            Array<String> rgTPAExtensions ={
                        ".ni.dll",		// Probe for .ni.dll first so that it's preferred if ni and il coexist in the same dir
                        ".dll",
                        ".ni.exe",
                        ".exe",
            };

            AddFilesFromDirectoryToTPAList(m_coreCLRDirectoryPath, rgTPAExtensions);
        }

        return m_tpaList;
    }

    // Create the host handle, loading it from CoreCLR.dll if necessary, and starts CoreCLR.
    // returns E_SUCCESS if the host exists or is successfully created.
    // returns E_LOAD_FAIL if the coreclr dll is not loaded
    // returns E_FAILURE if initialization fails
    TrinityErrorCode StartCoreCLR(const String& appPath, const String& appNiPath, const String& nativeDllSearchDirs)
    {
        if (!m_CLRRuntimeHost)
        {
            if (!m_coreCLRModule)
            {
                return TrinityErrorCode::E_LOAD_FAIL;
            }

            // Initialization properties
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
            // For additional Unix properties, see:
            // https://github.com/dotnet/coreclr/blob/master/src/dlls/mscoree/unixinterface.cpp

            auto _tpalist   = str_buff(GetTpaList());
            auto _appPath   = str_buff(appPath);
            auto _appNiPath = str_buff(appNiPath);
            auto _ndsd      = str_buff(nativeDllSearchDirs);

            const clr_char_t* property_keys[] ={
                t("TRUSTED_PLATFORM_ASSEMBLIES"),
                t("APP_PATHS"),
                t("APP_NI_PATHS"),
                t("NATIVE_DLL_SEARCH_DIRECTORIES"),
#if !defined(TRINITY_PLATFORM_WINDOWS)
                "System.GC.Server",
                "System.GC.Concurrent",
#endif
            };

            const clr_char_t* property_values[] ={
                // TRUSTED_PLATFORM_ASSEMBLIES
                _tpalist,
                // APP_PATHS
                _appPath,
                // APP_NI_PATHS
                _appNiPath,
                // NATIVE_DLL_SEARCH_DIRECTORIES
                _ndsd,
#if !defined(TRINITY_PLATFORM_WINDOWS)
                "true",
                "true",
#endif
            };



#if defined(TRINITY_PLATFORM_WINDOWS)
            FnGetCLRRuntimeHost pfnGetCLRRuntimeHost =
                (FnGetCLRRuntimeHost)::GetProcAddress(m_coreCLRModule, "GetCLRRuntimeHost");
            if (!pfnGetCLRRuntimeHost)
            {
                return TrinityErrorCode::E_LOAD_FAIL;
            }

            error_t hr = pfnGetCLRRuntimeHost(IID_ICLRRuntimeHost4, (IUnknown**)&m_CLRRuntimeHost);
            if (FAILED(hr))
            {
                return TrinityErrorCode::E_LOAD_FAIL;
            }
#else
            m_pfinit     = (coreclr_initialize_ptr)dlsym(m_coreCLRModule, "coreclr_initialize");
            m_pfexec     = (coreclr_execute_assembly_ptr)dlsym(m_coreCLRModule, "coreclr_execute_assembly");
            m_pfshutdown = (coreclr_shutdown_2_ptr)dlsym(m_coreCLRModule, "coreclr_shutdown_2");
            m_pfgetfunc  = (coreclr_create_delegate_ptr)dlsym(m_coreCLRModule, "coreclr_create_delegate");

            if (!m_pfinit || !m_pfexec || !m_pfshutdown || m_pfgetfunc)
            {
                return TrinityErrorCode::E_LOAD_FAIL;
            }
#endif

#if defined(TRINITY_PLATFORM_WINDOWS)
            // Default startup flags
            hr = m_CLRRuntimeHost->SetStartupFlags((STARTUP_FLAGS)
                (STARTUP_FLAGS::STARTUP_LOADER_OPTIMIZATION_SINGLE_DOMAIN |
                 STARTUP_FLAGS::STARTUP_SINGLE_APPDOMAIN |
                 STARTUP_FLAGS::STARTUP_CONCURRENT_GC |
                 STARTUP_FLAGS::STARTUP_SERVER_GC));

            // TODO should clean up host handle when disposing the reference
            if (FAILED(hr))
            {
                m_CLRRuntimeHost = nullptr;
                return TrinityErrorCode::E_FAILURE;
            }

            hr = m_CLRRuntimeHost->Start();
            if (FAILED(hr))
            {
                m_CLRRuntimeHost = nullptr;
                return TrinityErrorCode::E_FAILURE;
            }

            //-------------------------------------------------------------

            // Create an AppDomain

            /*
                    Console::WriteLine("Creating an AppDomain");
                    Console::WriteLine("TRUSTED_PLATFORM_ASSEMBLIES={0}", GetTpaList());
                    Console::WriteLine("APP_PATHS={0}", appPath);
                    Console::WriteLine("APP_NI_PATHS={0}", appNiPath);
                    Console::WriteLine("NATIVE_DLL_SEARCH_DIRECTORIES={0}", nativeDllSearchDirs);
            */

            hr = m_CLRRuntimeHost->CreateAppDomainWithManager(
                Path::GetFileNameWithoutExtension(Path::GetProcessPath()).ToWcharArray(),   // The friendly name of the AppDomain
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
                sizeof(property_keys) / sizeof(property_keys[0]),  // The number of properties
                property_keys,
                property_values,
                &m_domainId);

            if (FAILED(hr))
            {
                m_CLRRuntimeHost = nullptr;
                return TrinityErrorCode::E_FAILURE;
            }

            return TrinityErrorCode::E_SUCCESS;
#else
            auto exe_path = Path::GetProcessPath();
            auto exe_name = Path::GetFileNameWithoutExtension(exe_path);
            error_t st = m_pfinit(
                exe_path.Data(),
                exe_name.Data(),
                sizeof(property_keys) / sizeof(property_keys[0]),
                property_keys,
                property_values,
                &m_CLRRuntimeHost,
                &m_domainId);

            if (!SUCCEEDED(st))
            {
                Console::WriteLine("GraphEngine.Host: m_pfinit failed with status {0:X}", st);
                m_CLRRuntimeHost = nullptr;
                return TrinityErrorCode::E_FAILURE;
            }
#endif
        }

        return TrinityErrorCode::E_SUCCESS;
    }

    // Returns the domain id.
    const domainid_t GetDomainId() const
    {
        return m_domainId;
    }

    TrinityErrorCode GetFunction(
        IN const String& assemblyName,
        IN const String& className,
        IN const String& methodName,
        OUT INT_PTR& init_func) const
    {
        //Console::WriteLine(
        //    "GetFunction\n"
        //    "assemblyName = {0}\n"
        //    "className    = {1}\n"
        //    "methodName   = {2}",
        //    assemblyName, className, methodName);

#if defined(TRINITY_PLATFORM_WINDOWS)
        auto hr = m_CLRRuntimeHost->CreateDelegate(
            m_domainId,
            assemblyName.ToWcharArray(),
            className.ToWcharArray(),
            methodName.ToWcharArray(),
            &init_func);

        if (FAILED(hr))
        {
            Console::WriteLine("GetFunction failed with code 0x{0:X}", hr);
            return TrinityErrorCode::E_FAILURE;
        }
#else
        auto st = m_pfgetfunc(
            m_CLRRuntimeHost,
            m_domainId,
            assemblyName.Data(),
            className.Data(),
            methodName.Data(),
            &init_func);

        if (!SUCCEEDED(st))
        {
            Console::WriteLine("GetFunction failed with code 0x{0:X}", st);
            return TrinityErrorCode::E_FAILURE;
        }
#endif

        return TrinityErrorCode::E_SUCCESS;
    }

    TrinityErrorCode Uninit() const
    {
        if (nullptr == m_CLRRuntimeHost)
        {
            return TrinityErrorCode::E_INVALID_ARGUMENTS;
        }

        int exitCode = 0;

#if defined(TRINITY_PLATFORM_WINDOWS)

        //-------------------------------------------------------------
        // Unload the AppDomain

        error_t hr = m_CLRRuntimeHost->UnloadAppDomain2(
            m_domainId,
            true,
            (int *)&exitCode);                          // Wait until done

        if (FAILED(hr))
        {
            return TrinityErrorCode::E_UNLOAD_FAIL;
        }

        //-------------------------------------------------------------
        // Stop the host

        hr = m_CLRRuntimeHost->Stop();

        if (FAILED(hr))
        {
            return TrinityErrorCode::E_UNLOAD_FAIL;
        }

        //-------------------------------------------------------------

        // Release the reference to the host

        m_CLRRuntimeHost->Release();
#else
        error_t st = m_pfshutdown(m_CLRRuntimeHost, m_domainId, &exitCode);
        if (!SUCCEEDED(st))
        {
            return TrinityErrorCode::E_UNLOAD_FAIL;
        }
#endif


        return TrinityErrorCode::E_SUCCESS;
    }
};

TrinityErrorCode GraphEngineInit_impl(
    IN int n_apppaths,
    IN char** lp_apppaths,
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
        apppaths[i] = String(lp_apppaths[i]).Trim();
    }
    lpenv         = new HostEnvironment(String::Join(penv_sep, apppaths), err);
    return err;
}


DLL_EXPORT TrinityErrorCode GraphEngineInit(
    IN int n_apppaths,
    IN char** lp_apppaths,
    OUT void*& lpenv)
{
    return GraphEngineInit_impl(n_apppaths, lp_apppaths, lpenv);
}

DLL_EXPORT TrinityErrorCode GraphEngineGetFunction(
    IN const void* _lpenv,
    IN char* lp_entry_asm,
    IN char* lp_entry_class,
    IN char* lp_entry_method,
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

    TrinityErrorCode err = lpenv->Uninit();
    delete lpenv;

    return err;
}
