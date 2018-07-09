// GraphEngine.Hosting.cpp : Defines the entry point for the application.
//

#include "TrinityCommon.h"
#include "mscoree.h"

#if defined(TRINITY_PLATFORM_WINDOWS)
static const wchar_t* coreCLRDll = L"coreclr.dll";
#elif defined(TRINITY_PLATFORM_LINUX)
static const wchar_t* coreCLRDll = L"libcoreclr.so";
#elif defined(TRINITY_PLATFORM_DARWIN)
static const wchar_t* coreCLRDll = L"libcoreclr.dylib";
#endif

// sample host here: https://github.com/dotnet/samples/blob/master/core/hosting/host.cpp

DLL_EXPORT TrinityErrorCode GraphEngineInitializeHosting(int argc, char** argv)
{
    return TrinityErrorCode::E_SUCCESS;
}
