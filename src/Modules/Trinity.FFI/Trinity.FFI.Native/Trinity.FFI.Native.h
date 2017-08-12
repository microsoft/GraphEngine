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

typedef int64_t Cell;

enum CellAccessOptions: int32_t
{
    ThrowExceptionOnCellNotFound = 1,
    ReturnNullOnCellNotFound = 2,
    CreateNewOnCellNotFound = 4,
    StrongLogAhead = 8,
    WeakLogAhead = 16
};

typedef char* (*TRINITY_FFI_SYNC_HANDLER)(int32_t, char*);
typedef void  (*TRINITY_FFI_ASYNC_HANDLER)(int32_t, char*);

typedef void (*TRINITY_FFI_SYNC_REGISTRY)(int32_t, TRINITY_FFI_SYNC_HANDLER);
typedef void  (*TRINITY_FFI_ASYNC_REGISTRY)(int32_t, TRINITY_FFI_ASYNC_HANDLER);

typedef char* (*TRINITY_FFI_SYNC_SEND)(int32_t, int32_t, char*);
typedef void  (*TRINITY_FFI_ASYNC_SEND)(int32_t, int32_t, char*);

typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_SAVESTORAGE)();
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_LOADSTORAGE)();
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_RESETSTORAGE)();

typedef TrinityErrorCode (*TRINITY_FFI_CLOUD_SAVESTORAGE)();
typedef TrinityErrorCode (*TRINITY_FFI_CLOUD_LOADSTORAGE)();
typedef TrinityErrorCode (*TRINITY_FFI_CLOUD_RESETSTORAGE)();

typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_LOADCELL)(int64_t cellId, Cell* pcell);
typedef TrinityErrorCode (*TRINITY_FFI_CLOUD_LOADCELL)(int64_t cellId, Cell* pcell);

typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_SAVECELL_1)(int64_t cellId, Cell cell);
typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_SAVECELL_2)(int64_t cellId, CellAccessOptions options, Cell cell);
typedef TrinityErrorCode (*TRINITY_FFI_CLOUD_SAVECELL)(int64_t cellId, Cell cell);

typedef TrinityErrorCode (*TRINITY_FFI_LOCAL_REMOVECELL)(int64_t cellId);
typedef TrinityErrorCode (*TRINITY_FFI_CLOUD_REMOVECELL)(int64_t cellId);

typedef TrinityErrorCode (*TRINITY_FFI_NEWCELL_1)(char* cellType, Cell* pcell);
typedef TrinityErrorCode (*TRINITY_FFI_NEWCELL_2)(int64_t cellId, char* cellType, Cell* pcell);
typedef TrinityErrorCode (*TRINITY_FFI_NEWCELL_3)(char* cellType, char* cellContent, Cell* pcell);

typedef char*   (*TRINITY_FFI_CELL_TOSTRING)(Cell cell);
typedef int64_t (*TRINITY_FFI_CELL_GETID)(Cell cell);
typedef void    (*TRINITY_FFI_CELL_SETID)(Cell cell, int64_t cellId);
typedef char*   (*TRINITY_FFI_CELL_GET)(Cell cell, char* field);
typedef int32_t (*TRINITY_FFI_CELL_HAS)(Cell cell, char* field);//non-zero if cell has field.
typedef void    (*TRINITY_FFI_CELL_SET)(Cell cell, char* field, char* content);
typedef void    (*TRINITY_FFI_CELL_APPEND)(Cell cell, char* field, char* content);
typedef void    (*TRINITY_FFI_CELL_DELETE)(Cell cell, char* field);
typedef void    (*TRINITY_FFI_CELL_DISPOSE)(Cell cell);

extern "C" struct TRINITY_INTERFACES
{
    TRINITY_FFI_SYNC_REGISTRY    sync_registry;
    TRINITY_FFI_ASYNC_REGISTRY   async_registry;

    TRINITY_FFI_SYNC_SEND        sync_send;
    TRINITY_FFI_ASYNC_SEND       async_send;

    TRINITY_FFI_LOCAL_LOADSTORAGE  local_loadstorage;
    TRINITY_FFI_LOCAL_SAVESTORAGE  local_savestorage;
    TRINITY_FFI_LOCAL_RESETSTORAGE local_resetstorage;

    TRINITY_FFI_CLOUD_LOADSTORAGE  cloud_loadstorage;
    TRINITY_FFI_CLOUD_SAVESTORAGE  cloud_savestorage;
    TRINITY_FFI_CLOUD_RESETSTORAGE cloud_resetstorage;

    //IEnumerable<ICellAccessor> EnumerateGenericCellAccessors(LocalMemoryStorage storage);
    //IEnumerable<ICell> EnumerateGenericCells(LocalMemoryStorage storage);
    TRINITY_FFI_LOCAL_LOADCELL   local_loadcell;
    TRINITY_FFI_CLOUD_LOADCELL   cloud_loadcell;

    TRINITY_FFI_LOCAL_SAVECELL_1 local_savecell_1;
    TRINITY_FFI_LOCAL_SAVECELL_2 local_savecell_2;
    TRINITY_FFI_CLOUD_SAVECELL   cloud_savecell;

    TRINITY_FFI_LOCAL_REMOVECELL local_removecell;
    TRINITY_FFI_LOCAL_REMOVECELL cloud_removecell;

    TRINITY_FFI_NEWCELL_1        newcell_1;
    TRINITY_FFI_NEWCELL_2        newcell_2;
    TRINITY_FFI_NEWCELL_3        newcell_3;

    TRINITY_FFI_CELL_TOSTRING    cell_tostring;
    TRINITY_FFI_CELL_GETID       cell_getid;
    TRINITY_FFI_CELL_SETID       cell_setid;
    TRINITY_FFI_CELL_HAS         cell_hasfield;
    TRINITY_FFI_CELL_GET         cell_getfield;
    TRINITY_FFI_CELL_SET         cell_setfield;
    TRINITY_FFI_CELL_APPEND      cell_appendfield;
    TRINITY_FFI_CELL_DELETE      cell_removefield;
    TRINITY_FFI_CELL_DISPOSE     cell_dispose;

    //ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId);
    //ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options);
    //ICellAccessor UseGenericCell(LocalMemoryStorage storage, long cellId, CellAccessOptions options, string cellType);
};
TRINITYFFINATIVE_API TRINITY_INTERFACES* TRINITY_FFI_GET_INTERFACES();
