
/***********************************

  Auto-generated from Cpp.tt

 ***********************************/

#pragma once
// The following ifdef block is the standard way of creating macros which make exporting 
// from a DLL simpler. All files within this DLL are compiled with the TRINITYFFINATIVE_EXPORTS
// symbol defined on the command line. This symbol should not be defined on any project
// that uses this DLL. This way any other project whose source files include this file see 
// TRINITYFFINATIVE_API functions as being imported from a DLL, whereas this DLL sees symbols
// defined with this macro as being exported.
#ifdef TRINITYFFINATIVE_EXPORTS
#include <cstdint>
#include <os/os.h>
#define TRINITYFFINATIVE_API extern "C" __declspec(dllexport)
#else
#define TRINITYFFINATIVE_API extern "C" __declspec(dllimport)
#endif
#include <TrinityErrorCode.h>

enum CellAccessOptions: int32_t
{
    ThrowExceptionOnCellNotFound = 1,
    ReturnNullOnCellNotFound = 2,
    CreateNewOnCellNotFound = 4,
    StrongLogAhead = 8,
    WeakLogAhead = 16
};

typedef char* (*TRINITY_FFI_SYNC_HANDLER)(char*);
typedef void  (*TRINITY_FFI_ASYNC_HANDLER)(char*);

typedef void (*TRINITY_FFI_SYNC_REGISTRY)(long, TRINITY_FFI_SYNC_HANDLER);
typedef void (*TRINITY_FFI_ASYNC_REGISTRY)(long, TRINITY_FFI_ASYNC_HANDLER);
typedef char* (*TRINITY_FFI_SYNC_SEND)(long, long, char*);
typedef void (*TRINITY_FFI_ASYNC_SEND)(long, long, char*);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_LOADCELL)(long long, void**);
typedef TrinityErrorCode (*TRINITY_FFI_CLOUD_LOADCELL)(long long, void**);
typedef TrinityErrorCode (*TRINITY_FFI_CELL_FIELDENUM_MOVENEXT)(void*, void*);
typedef TrinityErrorCode (*TRINITY_FFI_CELL_FIELDINFO_NAME)(void*, char**);
typedef TrinityErrorCode (*TRINITY_FFI_CELL_FIELDENUM_CURRENT)(void*, void**);
typedef TrinityErrorCode (*TRINITY_FFI_CELL_FIELDENUM_DISPOSE)(void*);
typedef TrinityErrorCode (*TRINITY_FFI_CELL_FIELDENUM_GET)(void*, void**);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_SAVECELL_1)(long long, void*);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_SAVECELL_2)(long long, CellAccessOptions, void*);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_SAVECELL_3)(void*);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_SAVECELL_4)(CellAccessOptions, void*);
typedef TrinityErrorCode (*TRINITY_FFI_CLOUD_SAVECELL)(long long, void*);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_REMOVECELL)(long long);
typedef TrinityErrorCode (*TRINITY_FFI_NEWCELL_1)(char*, void**);
typedef TrinityErrorCode (*TRINITY_FFI_NEWCELL_2)(long long, char*, void**);
typedef TrinityErrorCode (*TRINITY_FFI_NEWCELL_3)(char*, char*, void**);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_USECELL_1)(long long, void**);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_USECELL_2)(long long, CellAccessOptions, void**);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_USECELL_3)(long long, CellAccessOptions, void**, char*);
typedef void (*TRINITY_FFI_CELL_DISPOSE)(void*);
typedef char* (*TRINITY_FFI_CELL_TOSTRING)(void*);
typedef long long (*TRINITY_FFI_CELL_GETID)(void*);
typedef void (*TRINITY_FFI_CELL_SETID)(void*, long long);
typedef char* (*TRINITY_FFI_CELL_GET)(void*, char*);
typedef long (*TRINITY_FFI_CELL_HAS)(void*, char*);
typedef void (*TRINITY_FFI_CELL_SET)(void*, char*, char*);
typedef void (*TRINITY_FFI_CELL_APPEND)(void*, char*, char*);
typedef void (*TRINITY_FFI_CELL_DELETE)(void*, char*);

extern "C" struct TRINITY_INTERFACES
{
    TRINITY_FFI_SYNC_REGISTRY sync_registry;
    TRINITY_FFI_ASYNC_REGISTRY async_registry;
    TRINITY_FFI_SYNC_SEND sync_send;
    TRINITY_FFI_ASYNC_SEND async_send;
    TRINITY_FFI_LOCAL_LOADCELL local_loadcell;
    TRINITY_FFI_CLOUD_LOADCELL cloud_loadcell;
    TRINITY_FFI_CELL_FIELDENUM_MOVENEXT cell_fieldenum_movenext;
    TRINITY_FFI_CELL_FIELDINFO_NAME cell_fieldinfo_name;
    TRINITY_FFI_CELL_FIELDENUM_CURRENT cell_fieldenum_current;
    TRINITY_FFI_CELL_FIELDENUM_DISPOSE cell_fieldenum_dispose;
    TRINITY_FFI_CELL_FIELDENUM_GET cell_fieldenum_get;
    TRINITY_FFI_LOCAL_SAVECELL_1 local_savecell_1;
    TRINITY_FFI_LOCAL_SAVECELL_2 local_savecell_2;
    TRINITY_FFI_LOCAL_SAVECELL_3 local_savecell_3;
    TRINITY_FFI_LOCAL_SAVECELL_4 local_savecell_4;
    TRINITY_FFI_CLOUD_SAVECELL cloud_savecell;
    TRINITY_FFI_LOCAL_REMOVECELL local_removecell;
    TRINITY_FFI_NEWCELL_1 newcell_1;
    TRINITY_FFI_NEWCELL_2 newcell_2;
    TRINITY_FFI_NEWCELL_3 newcell_3;
    TRINITY_FFI_LOCAL_USECELL_1 local_usecell_1;
    TRINITY_FFI_LOCAL_USECELL_2 local_usecell_2;
    TRINITY_FFI_LOCAL_USECELL_3 local_usecell_3;
    TRINITY_FFI_CELL_DISPOSE cell_dispose;
    TRINITY_FFI_CELL_TOSTRING cell_tostring;
    TRINITY_FFI_CELL_GETID cell_getid;
    TRINITY_FFI_CELL_SETID cell_setid;
    TRINITY_FFI_CELL_GET cell_get;
    TRINITY_FFI_CELL_HAS cell_has;
    TRINITY_FFI_CELL_SET cell_set;
    TRINITY_FFI_CELL_APPEND cell_append;
    TRINITY_FFI_CELL_DELETE cell_delete;
};

TRINITYFFINATIVE_API TRINITY_INTERFACES* TRINITY_FFI_GET_INTERFACES();
