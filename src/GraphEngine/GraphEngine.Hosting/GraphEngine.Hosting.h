#pragma once

#include "TrinityCommon.h"

#if !defined(TRINITYC_EXPORTS)
DLL_IMPORT TrinityErrorCode GraphEngineInit(int argc, char** argv, OUT void** lpenv);
DLL_IMPORT TrinityErrorCode GraphEngineUninit(IN void* lpenv);
#endif