
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
#include "Trinity.h"

typedef char* (*TRINITY_FFI_SYNC_HANDLER)(char*);
typedef void  (*TRINITY_FFI_ASYNC_HANDLER)(char*);

typedef TrinityErrorCode (*TRINITY_FFI_ACCESSOR_USE_1)(long long, void**);
typedef TrinityErrorCode (*TRINITY_FFI_ACCESSOR_USE_2)(long long, CellAccessOptions, void**);
typedef TrinityErrorCode (*TRINITY_FFI_ACCESSOR_USE_3)(long long, CellAccessOptions, void**, char*);
typedef TrinityErrorCode (*TRINITY_FFI_CELL_NEW_1)(char*, void**);
typedef TrinityErrorCode (*TRINITY_FFI_CELL_NEW_2)(long long, char*, void**);
typedef TrinityErrorCode (*TRINITY_FFI_CELL_NEW_3)(char*, char*, void**);
typedef char* (*TRINITY_FFI_CELL_TOSTRING)(void*);
typedef long long (*TRINITY_FFI_CELL_GETID)(void*);
typedef void (*TRINITY_FFI_CELL_SETID)(void*, long long);
typedef char* (*TRINITY_FFI_CELL_GET)(void*, char*);
typedef long (*TRINITY_FFI_CELL_HAS)(void*, char*);
typedef void (*TRINITY_FFI_CELL_SET)(void*, char*, char*);
typedef void (*TRINITY_FFI_CELL_APPEND)(void*, char*, char*);
typedef void (*TRINITY_FFI_CELL_DELETE)(void*, char*);
typedef TrinityErrorCode (*TRINITY_FFI_CLOUD_LOADCELL)(long long, void**);
typedef TrinityErrorCode (*TRINITY_FFI_CLOUD_SAVECELL)(long long, void*);
typedef TrinityErrorCode (*TRINITY_FFI_ENUM_NEXT)(void*);
typedef TrinityErrorCode (*TRINITY_FFI_GC_FREE)(void*);
typedef TrinityErrorCode (*TRINITY_FFI_GC_DISPOSE)(void*);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_LOADCELL)(long long, void**);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_SAVECELL_1)(long long, void*);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_SAVECELL_2)(long long, CellAccessOptions, void*);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_SAVECELL_3)(void*);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_SAVECELL_4)(CellAccessOptions, void*);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_REMOVECELL)(long long);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_SAVESTORAGE)();
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_LOADSTORAGE)();
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_RESETSTORAGE)();
typedef TrinityErrorCode (*TRINITY_FFI_SCHEMA_GET)(void**, long*);

extern "C" struct TRINITY_INTERFACES
{
    TRINITY_FFI_ACCESSOR_USE_1 accessor_use_1;
    TRINITY_FFI_ACCESSOR_USE_2 accessor_use_2;
    TRINITY_FFI_ACCESSOR_USE_3 accessor_use_3;
    TRINITY_FFI_CELL_NEW_1 cell_new_1;
    TRINITY_FFI_CELL_NEW_2 cell_new_2;
    TRINITY_FFI_CELL_NEW_3 cell_new_3;
    TRINITY_FFI_CELL_TOSTRING cell_tostring;
    TRINITY_FFI_CELL_GETID cell_getid;
    TRINITY_FFI_CELL_SETID cell_setid;
    TRINITY_FFI_CELL_GET cell_get;
    TRINITY_FFI_CELL_HAS cell_has;
    TRINITY_FFI_CELL_SET cell_set;
    TRINITY_FFI_CELL_APPEND cell_append;
    TRINITY_FFI_CELL_DELETE cell_delete;
    TRINITY_FFI_CLOUD_LOADCELL cloud_loadcell;
    TRINITY_FFI_CLOUD_SAVECELL cloud_savecell;
    TRINITY_FFI_ENUM_NEXT enum_next;
    TRINITY_FFI_GC_FREE gc_free;
    TRINITY_FFI_GC_DISPOSE gc_dispose;
    TRINITY_FFI_LOCAL_LOADCELL local_loadcell;
    TRINITY_FFI_LOCAL_SAVECELL_1 local_savecell_1;
    TRINITY_FFI_LOCAL_SAVECELL_2 local_savecell_2;
    TRINITY_FFI_LOCAL_SAVECELL_3 local_savecell_3;
    TRINITY_FFI_LOCAL_SAVECELL_4 local_savecell_4;
    TRINITY_FFI_LOCAL_REMOVECELL local_removecell;
    TRINITY_FFI_LOCAL_SAVESTORAGE local_savestorage;
    TRINITY_FFI_LOCAL_LOADSTORAGE local_loadstorage;
    TRINITY_FFI_LOCAL_RESETSTORAGE local_resetstorage;
    TRINITY_FFI_SCHEMA_GET schema_get;
};

TRINITYFFINATIVE_API TRINITY_INTERFACES* TRINITY_FFI_GET_INTERFACES();
