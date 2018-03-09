// Trinity.FFI.Native.cpp : Defines the exported functions for the DLL application.
//

#include "Trinity.FFI.Native.h"
#include <cstring>

static struct TRINITY_INTERFACES g_interfaces;
static bool g_init = false;

TRINITYFFINATIVE_API void TRINITY_FFI_SET_INTERFACES(const TRINITY_INTERFACES* interfaces)
{
    memcpy(&g_interfaces, interfaces, sizeof(TRINITY_INTERFACES));
	g_init = true;
}

TRINITYFFINATIVE_API TRINITY_INTERFACES*  TRINITY_FFI_GET_INTERFACES()
{
	if (!g_init)
	{
		// TODO hosting
	}

    return &g_interfaces;
}
