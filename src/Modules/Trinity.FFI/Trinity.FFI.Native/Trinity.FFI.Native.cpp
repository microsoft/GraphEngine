// Trinity.FFI.Native.cpp : Defines the exported functions for the DLL application.
//

#include "Trinity.FFI.Native.h"
#include <cstring>
#include "CellAccessor.h"

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

        /*
        
        1. Find the coreclr assembly location, load it
           - Raise if hosting environment is not installed
        2. Commence coreclr initialization sequence.
           - If success, we will have ExecutionEngine (EE) provider standby.
        3. With EE provider, initialize an AppDomain (EE)
           - Let's call it "runtime".
        4. Load appropriate assemblies
           - i.   System assemblies (System.IO, etc.)
           - ii.  For each GraphEngine package, we will have a language-specific package mapped to it.
                  These language-specific packages will then proceed to handle loading logic of .NET assemblies.
           - iii. The main entry is written in a master Reisen package. Plugin-packages extend the loading routine.
           - iv.  For example: GE -> Reisen (python) -> HA (azure) + CompositeStorage
                  On the Reisen+Python side, it would be Reisen.Core, Reisen.HA, Reisen.CompositeStorage;
                  Reisen.Core might trigger loading of essential packages.
        5. Fire up init routines (set trinity ffi interfaces etc. ;)
           - At this moment, Trinity and related assemblies (HA etc.) should be initialized.
           - We have initialization routines and attributes at .NET side.
        6. Start executing user logic.
           - Defaults to an 'Application' with an 'Entry Point'.
             For Python, it's the current file, or the one marked as '__MAIN__'
           - A Reisen extension package may alter how user logic should be executed.
             It might turn the proactive execution into callbacks, event handlers, etc.
             For example, a High-Availability program will have a main event loop, a startup handler,
             a termination handler etc. The main event loop may be re-entered after the program recovers
             from a crash. How this is coordinated is defined by a Reisen package.

        */
	}

    return &g_interfaces;
}
