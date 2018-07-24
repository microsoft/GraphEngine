// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


/* this ALWAYS GENERATED file contains the definitions for the interfaces */


 /* File created by MIDL compiler version 8.00.0603 */
/* @@MIDL_FILE_HEADING(  ) */

#pragma warning( disable: 4049 )  /* more than 64k source lines */


/* verify that the <rpcndr.h> version is high enough to compile this file*/
#ifndef __REQUIRED_RPCNDR_H_VERSION__
#define __REQUIRED_RPCNDR_H_VERSION__ 475
#endif

#include "rpc.h"
#include "rpcndr.h"

#ifndef __RPCNDR_H_VERSION__
#error this stub requires an updated version of <rpcndr.h>
#endif // __RPCNDR_H_VERSION__

#ifndef COM_NO_WINDOWS_H
#include "windows.h"
#include "ole2.h"
#endif /*COM_NO_WINDOWS_H*/

#ifndef __mscoree_h__
#define __mscoree_h__

#if defined(_MSC_VER) && (_MSC_VER >= 1020)
#pragma once
#endif

/* Forward Declarations */ 

#ifndef __IDebuggerThreadControl_FWD_DEFINED__
#define __IDebuggerThreadControl_FWD_DEFINED__
typedef interface IDebuggerThreadControl IDebuggerThreadControl;

#endif 	/* __IDebuggerThreadControl_FWD_DEFINED__ */


#ifndef __IDebuggerInfo_FWD_DEFINED__
#define __IDebuggerInfo_FWD_DEFINED__
typedef interface IDebuggerInfo IDebuggerInfo;

#endif 	/* __IDebuggerInfo_FWD_DEFINED__ */


#ifndef __ICLRErrorReportingManager_FWD_DEFINED__
#define __ICLRErrorReportingManager_FWD_DEFINED__
typedef interface ICLRErrorReportingManager ICLRErrorReportingManager;

#endif 	/* __ICLRErrorReportingManager_FWD_DEFINED__ */


#ifndef __ICLRErrorReportingManager2_FWD_DEFINED__
#define __ICLRErrorReportingManager2_FWD_DEFINED__
typedef interface ICLRErrorReportingManager2 ICLRErrorReportingManager2;

#endif 	/* __ICLRErrorReportingManager2_FWD_DEFINED__ */


#ifndef __ICLRPolicyManager_FWD_DEFINED__
#define __ICLRPolicyManager_FWD_DEFINED__
typedef interface ICLRPolicyManager ICLRPolicyManager;

#endif 	/* __ICLRPolicyManager_FWD_DEFINED__ */


#ifndef __ICLRGCManager_FWD_DEFINED__
#define __ICLRGCManager_FWD_DEFINED__
typedef interface ICLRGCManager ICLRGCManager;

#endif 	/* __ICLRGCManager_FWD_DEFINED__ */


#ifndef __ICLRGCManager2_FWD_DEFINED__
#define __ICLRGCManager2_FWD_DEFINED__
typedef interface ICLRGCManager2 ICLRGCManager2;

#endif 	/* __ICLRGCManager2_FWD_DEFINED__ */


#ifndef __IHostControl_FWD_DEFINED__
#define __IHostControl_FWD_DEFINED__
typedef interface IHostControl IHostControl;

#endif 	/* __IHostControl_FWD_DEFINED__ */


#ifndef __ICLRControl_FWD_DEFINED__
#define __ICLRControl_FWD_DEFINED__
typedef interface ICLRControl ICLRControl;

#endif 	/* __ICLRControl_FWD_DEFINED__ */


#ifndef __ICLRRuntimeHost_FWD_DEFINED__
#define __ICLRRuntimeHost_FWD_DEFINED__
typedef interface ICLRRuntimeHost ICLRRuntimeHost;

#endif 	/* __ICLRRuntimeHost_FWD_DEFINED__ */


#ifndef __ICLRRuntimeHost2_FWD_DEFINED__
#define __ICLRRuntimeHost2_FWD_DEFINED__
typedef interface ICLRRuntimeHost2 ICLRRuntimeHost2;

#endif 	/* __ICLRRuntimeHost4_FWD_DEFINED__ */

#ifndef __ICLRRuntimeHost4_FWD_DEFINED__
#define __ICLRRuntimeHost4_FWD_DEFINED__
typedef interface ICLRRuntimeHost4 ICLRRuntimeHost4;

#endif  /* __ICLRRuntimeHost4_FWD_DEFINED__ */

#ifndef __ICLRExecutionManager_FWD_DEFINED__
#define __ICLRExecutionManager_FWD_DEFINED__
typedef interface ICLRExecutionManager ICLRExecutionManager;

#endif 	/* __ICLRExecutionManager_FWD_DEFINED__ */


#ifndef __IHostNetCFDebugControlManager_FWD_DEFINED__
#define __IHostNetCFDebugControlManager_FWD_DEFINED__
typedef interface IHostNetCFDebugControlManager IHostNetCFDebugControlManager;

#endif 	/* __IHostNetCFDebugControlManager_FWD_DEFINED__ */


#ifndef __ITypeName_FWD_DEFINED__
#define __ITypeName_FWD_DEFINED__
typedef interface ITypeName ITypeName;

#endif 	/* __ITypeName_FWD_DEFINED__ */


#ifndef __ITypeNameBuilder_FWD_DEFINED__
#define __ITypeNameBuilder_FWD_DEFINED__
typedef interface ITypeNameBuilder ITypeNameBuilder;

#endif 	/* __ITypeNameBuilder_FWD_DEFINED__ */


#ifndef __ITypeNameFactory_FWD_DEFINED__
#define __ITypeNameFactory_FWD_DEFINED__
typedef interface ITypeNameFactory ITypeNameFactory;

#endif 	/* __ITypeNameFactory_FWD_DEFINED__ */


#ifndef __IManagedObject_FWD_DEFINED__
#define __IManagedObject_FWD_DEFINED__
typedef interface IManagedObject IManagedObject;

#endif 	/* __IManagedObject_FWD_DEFINED__ */


#ifndef __ComCallUnmarshal_FWD_DEFINED__
#define __ComCallUnmarshal_FWD_DEFINED__

#ifdef __cplusplus
typedef class ComCallUnmarshal ComCallUnmarshal;
#else
typedef struct ComCallUnmarshal ComCallUnmarshal;
#endif /* __cplusplus */

#endif 	/* __ComCallUnmarshal_FWD_DEFINED__ */


#ifndef __ComCallUnmarshalV4_FWD_DEFINED__
#define __ComCallUnmarshalV4_FWD_DEFINED__

#ifdef __cplusplus
typedef class ComCallUnmarshalV4 ComCallUnmarshalV4;
#else
typedef struct ComCallUnmarshalV4 ComCallUnmarshalV4;
#endif /* __cplusplus */

#endif 	/* __ComCallUnmarshalV4_FWD_DEFINED__ */


#ifndef __CLRRuntimeHost_FWD_DEFINED__
#define __CLRRuntimeHost_FWD_DEFINED__

#ifdef __cplusplus
typedef class CLRRuntimeHost CLRRuntimeHost;
#else
typedef struct CLRRuntimeHost CLRRuntimeHost;
#endif /* __cplusplus */

#endif 	/* __CLRRuntimeHost_FWD_DEFINED__ */


#ifndef __TypeNameFactory_FWD_DEFINED__
#define __TypeNameFactory_FWD_DEFINED__

#ifdef __cplusplus
typedef class TypeNameFactory TypeNameFactory;
#else
typedef struct TypeNameFactory TypeNameFactory;
#endif /* __cplusplus */

#endif 	/* __TypeNameFactory_FWD_DEFINED__ */


#ifndef __ICLRAppDomainResourceMonitor_FWD_DEFINED__
#define __ICLRAppDomainResourceMonitor_FWD_DEFINED__
typedef interface ICLRAppDomainResourceMonitor ICLRAppDomainResourceMonitor;

#endif 	/* __ICLRAppDomainResourceMonitor_FWD_DEFINED__ */


/* header files for imported files */
#include "unknwn.h"
#include "gchost.h"

#ifdef __cplusplus
extern "C"{
#endif 


/* interface __MIDL_itf_mscoree_0000_0000 */
/* [local] */ 

#define DECLARE_DEPRECATED 
#define DEPRECATED_CLR_STDAPI STDAPI

struct IActivationFactory;

EXTERN_GUID(CLSID_TypeNameFactory, 0xB81FF171, 0x20F3, 0x11d2, 0x8d, 0xcc, 0x00, 0xa0, 0xc9, 0xb0, 0x05, 0x25);
EXTERN_GUID(CLSID_ComCallUnmarshal, 0x3F281000,0xE95A,0x11d2,0x88,0x6B,0x00,0xC0,0x4F,0x86,0x9F,0x04);
EXTERN_GUID(CLSID_ComCallUnmarshalV4, 0x45fb4600,0xe6e8,0x4928,0xb2,0x5e,0x50,0x47,0x6f,0xf7,0x94,0x25);
EXTERN_GUID(IID_IManagedObject, 0xc3fcc19e, 0xa970, 0x11d2, 0x8b, 0x5a, 0x00, 0xa0, 0xc9, 0xb7, 0xc9, 0xc4);
EXTERN_GUID(IID_ICLRAppDomainResourceMonitor, 0XC62DE18C, 0X2E23, 0X4AEA, 0X84, 0X23, 0XB4, 0X0C, 0X1F, 0XC5, 0X9E, 0XAE);
EXTERN_GUID(IID_ICLRPolicyManager, 0x7D290010, 0xD781, 0x45da, 0xA6, 0xF8, 0xAA, 0x5D, 0x71, 0x1A, 0x73, 0x0E);
EXTERN_GUID(IID_ICLRGCManager, 0x54D9007E, 0xA8E2, 0x4885, 0xB7, 0xBF, 0xF9, 0x98, 0xDE, 0xEE, 0x4F, 0x2A);
EXTERN_GUID(IID_ICLRGCManager2, 0x0603B793, 0xA97A, 0x4712, 0x9C, 0xB4, 0x0C, 0xD1, 0xC7, 0x4C, 0x0F, 0x7C);
EXTERN_GUID(IID_ICLRErrorReportingManager, 0x980d2f1a, 0xbf79, 0x4c08, 0x81, 0x2a, 0xbb, 0x97, 0x78, 0x92, 0x8f, 0x78);
EXTERN_GUID(IID_ICLRErrorReportingManager2, 0xc68f63b1, 0x4d8b, 0x4e0b, 0x95, 0x64, 0x9d, 0x2e, 0xfe, 0x2f, 0xa1, 0x8c);
EXTERN_GUID(IID_ICLRRuntimeHost, 0x90F1A06C, 0x7712, 0x4762, 0x86, 0xB5, 0x7A, 0x5E, 0xBA, 0x6B, 0xDB, 0x02);
EXTERN_GUID(IID_ICLRRuntimeHost2, 0x712AB73F, 0x2C22, 0x4807, 0xAD, 0x7E, 0xF5, 0x01, 0xD7, 0xb7, 0x2C, 0x2D);
EXTERN_GUID(IID_ICLRRuntimeHost4, 0x64F6D366, 0xD7C2, 0x4F1F, 0xB4, 0xB2, 0xE8, 0x16, 0x0C, 0xAC, 0x43, 0xAF);
EXTERN_GUID(IID_ICLRExecutionManager, 0x1000A3E7, 0xB420, 0x4620, 0xAE, 0x30, 0xFB, 0x19, 0xB5, 0x87, 0xAD, 0x1D);
EXTERN_GUID(IID_ITypeName, 0xB81FF171, 0x20F3, 0x11d2, 0x8d, 0xcc, 0x00, 0xa0, 0xc9, 0xb0, 0x05, 0x22);
EXTERN_GUID(IID_ITypeNameBuilder, 0xB81FF171, 0x20F3, 0x11d2, 0x8d, 0xcc, 0x00, 0xa0, 0xc9, 0xb0, 0x05, 0x23);
EXTERN_GUID(IID_ITypeNameFactory, 0xB81FF171, 0x20F3, 0x11d2, 0x8d, 0xcc, 0x00, 0xa0, 0xc9, 0xb0, 0x05, 0x21);
DEPRECATED_CLR_STDAPI GetCORSystemDirectory(_Out_writes_to_(cchBuffer, *dwLength) LPWSTR pbuffer, DWORD  cchBuffer, DWORD* dwLength);
DEPRECATED_CLR_STDAPI GetCORVersion(_Out_writes_to_(cchBuffer, *dwLength) LPWSTR pbBuffer, DWORD cchBuffer, DWORD* dwLength);
DEPRECATED_CLR_STDAPI GetFileVersion(LPCWSTR szFilename, _Out_writes_to_opt_(cchBuffer, *dwLength) LPWSTR szBuffer, DWORD cchBuffer, DWORD* dwLength);
DEPRECATED_CLR_STDAPI GetCORRequiredVersion(_Out_writes_to_(cchBuffer, *dwLength) LPWSTR pbuffer, DWORD cchBuffer, DWORD* dwLength);
DEPRECATED_CLR_STDAPI GetRequestedRuntimeInfo(LPCWSTR pExe, LPCWSTR pwszVersion, LPCWSTR pConfigurationFile, DWORD startupFlags, DWORD runtimeInfoFlags, _Out_writes_opt_(dwDirectory) LPWSTR pDirectory, DWORD dwDirectory, _Out_opt_ DWORD *dwDirectoryLength, _Out_writes_opt_(cchBuffer) LPWSTR pVersion, DWORD cchBuffer, _Out_opt_ DWORD* dwlength);
DEPRECATED_CLR_STDAPI GetRequestedRuntimeVersion(_In_ LPWSTR pExe, _Out_writes_to_(cchBuffer, *dwLength) LPWSTR pVersion, DWORD cchBuffer, _Out_ DWORD* dwLength);
DEPRECATED_CLR_STDAPI CorBindToRuntimeHost(LPCWSTR pwszVersion, LPCWSTR pwszBuildFlavor, LPCWSTR pwszHostConfigFile, VOID* pReserved, DWORD startupFlags, REFCLSID rclsid, REFIID riid, LPVOID FAR *ppv);
DEPRECATED_CLR_STDAPI CorBindToRuntimeEx(LPCWSTR pwszVersion, LPCWSTR pwszBuildFlavor, DWORD startupFlags, REFCLSID rclsid, REFIID riid, LPVOID FAR *ppv);
DEPRECATED_CLR_STDAPI CorBindToRuntimeByCfg(IStream* pCfgStream, DWORD reserved, DWORD startupFlags, REFCLSID rclsid,REFIID riid, LPVOID FAR* ppv);
DEPRECATED_CLR_STDAPI CorBindToRuntime(LPCWSTR pwszVersion, LPCWSTR pwszBuildFlavor, REFCLSID rclsid, REFIID riid, LPVOID FAR *ppv);
DEPRECATED_CLR_STDAPI CorBindToCurrentRuntime(LPCWSTR pwszFileName, REFCLSID rclsid, REFIID riid, LPVOID FAR *ppv);
DEPRECATED_CLR_STDAPI ClrCreateManagedInstance(LPCWSTR pTypeName, REFIID riid, void **ppObject);
DECLARE_DEPRECATED void STDMETHODCALLTYPE CorMarkThreadInThreadPool();
DEPRECATED_CLR_STDAPI RunDll32ShimW(HWND hwnd, HINSTANCE hinst, LPCWSTR lpszCmdLine, int nCmdShow);
DEPRECATED_CLR_STDAPI LoadLibraryShim(LPCWSTR szDllName, LPCWSTR szVersion, LPVOID pvReserved, HMODULE *phModDll);
DEPRECATED_CLR_STDAPI CallFunctionShim(LPCWSTR szDllName, LPCSTR szFunctionName, LPVOID lpvArgument1, LPVOID lpvArgument2, LPCWSTR szVersion, LPVOID pvReserved);
DEPRECATED_CLR_STDAPI GetRealProcAddress(LPCSTR pwszProcName, VOID** ppv);
DECLARE_DEPRECATED void STDMETHODCALLTYPE CorExitProcess(int exitCode);
DEPRECATED_CLR_STDAPI LoadStringRC(UINT iResouceID, _Out_writes_z_(iMax) LPWSTR szBuffer, int iMax, int bQuiet);
typedef HRESULT  (STDAPICALLTYPE *FnGetCLRRuntimeHost)(REFIID riid, IUnknown **pUnk);
typedef /* [public] */ 
enum __MIDL___MIDL_itf_mscoree_0000_0000_0001
    {
        HOST_TYPE_DEFAULT	= 0,
        HOST_TYPE_APPLAUNCH	= 0x1,
        HOST_TYPE_CORFLAG	= 0x2
    } 	HOST_TYPE;

STDAPI CorLaunchApplication(HOST_TYPE dwClickOnceHost, LPCWSTR pwzAppFullName, DWORD dwManifestPaths, LPCWSTR* ppwzManifestPaths, DWORD dwActivationData, LPCWSTR* ppwzActivationData, LPPROCESS_INFORMATION lpProcessInformation);
typedef HRESULT ( __stdcall *FExecuteInAppDomainCallback )( 
    void *cookie);

typedef /* [public][public] */ 
enum __MIDL___MIDL_itf_mscoree_0000_0000_0002
    {
        STARTUP_CONCURRENT_GC	= 0x1,
        STARTUP_LOADER_OPTIMIZATION_MASK	= ( 0x3 << 1 ) ,
        STARTUP_LOADER_OPTIMIZATION_SINGLE_DOMAIN	= ( 0x1 << 1 ) ,
        STARTUP_LOADER_OPTIMIZATION_MULTI_DOMAIN	= ( 0x2 << 1 ) ,
        STARTUP_LOADER_OPTIMIZATION_MULTI_DOMAIN_HOST	= ( 0x3 << 1 ) ,
        STARTUP_LOADER_SAFEMODE	= 0x10,
        STARTUP_LOADER_SETPREFERENCE	= 0x100,
        STARTUP_SERVER_GC	= 0x1000,
        STARTUP_HOARD_GC_VM	= 0x2000,
        STARTUP_SINGLE_VERSION_HOSTING_INTERFACE	= 0x4000,
        STARTUP_LEGACY_IMPERSONATION	= 0x10000,
        STARTUP_DISABLE_COMMITTHREADSTACK	= 0x20000,
        STARTUP_ALWAYSFLOW_IMPERSONATION	= 0x40000,
        STARTUP_TRIM_GC_COMMIT	= 0x80000,
        STARTUP_ETW	= 0x100000,
        STARTUP_ARM	= 0x400000,
        STARTUP_SINGLE_APPDOMAIN	= 0x800000,
        STARTUP_APPX_APP_MODEL	= 0x1000000,
        STARTUP_DISABLE_RANDOMIZED_STRING_HASHING	= 0x2000000 // not supported
    } 	STARTUP_FLAGS;

typedef /* [public] */ 
enum __MIDL___MIDL_itf_mscoree_0000_0000_0003
    {
        CLSID_RESOLUTI