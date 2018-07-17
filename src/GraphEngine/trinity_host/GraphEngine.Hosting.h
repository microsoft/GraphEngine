#pragma once

#include "TrinityCommon.h"

#if !defined(TRINITYC_EXPORTS)
DLL_IMPORT TrinityErrorCode GraphEngineInit(IN int n_apppaths, IN wchar_t** lp_apppaths, IN wchar_t* lp_entry_asm, IN wchar_t* lp_entry_class, IN wchar_t* lp_entry_method, OUT void*& lpenv);
DLL_IMPORT TrinityErrorCode GraphEngineUninit(IN void* lpenv);
#endif