// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Debugger.h"
#if defined(TRINITY_PLATFORM_WINDOWS)

#include <corelib>

#include <Storage/LocalStorage/LocalMemoryStorage.h>

#define DEBUGGER_CMD(x) bool x(Array<String>& args)
#define PARSE_INT(idx) int32_t arg##idx; if (args.Length() <= idx || !args[idx].TryParse(arg##idx)) return false;
#define PARSE_INT64(idx) int64_t arg##idx; if (args.Length() <= idx || !args[idx].TryParse(arg##idx)) return false;

namespace Trinity{
	namespace Debugger
	{
		DEBUGGER_CMD(StackTrace)
		{
			return true;
		}
		DEBUGGER_CMD(ListModule)
		{
			return true;
		}
		DEBUGGER_CMD(ListThreads)
		{
			auto threads = GetThreads(GetCurrentProcessId());
			for (auto t : threads)
			{
				HANDLE hThread;
				if (t.th32ThreadID == GetCurrentThreadId())
					hThread = GetCurrentThread();
				else
				{
					hThread = OpenThread(THREAD_ALL_ACCESS, false, t.th32ThreadID);
					if (!hThread)
					{
						printError("OpenThread");
						continue;
					}
				}
				printf("\n     THREAD ID      = 0x%08X", t.th32ThreadID);
				printf("\n     base priority  = %d", t.tpBasePri);
				printf("\n     delta priority = %d\n\n", t.tpDeltaPri);

				if (t.th32ThreadID == GetCurrentThreadId())
					CloseHandle(hThread);
			}
			return true;
		}
		DEBUGGER_CMD(DumpLocalMemoryStorage)
		{
			Storage::LocalMemoryStorage::DebugDump();
			return true;
		}
		DEBUGGER_CMD(DumpMTHash)
		{
			PARSE_INT(1);
			Storage::LocalMemoryStorage::MTHashDebugDump(arg1);
			return true;
		}
		DEBUGGER_CMD(DumpMemoryTrunk)
		{
			PARSE_INT(1);
			Storage::LocalMemoryStorage::MemoryTrunkDebugDump(arg1);
			return true;
		}
        DEBUGGER_CMD(SearchCellEntry)
        {
            PARSE_INT64(1);
            Storage::LocalMemoryStorage::MTHashSearchCellEntry(arg1);
            return true;
        }

		std::vector<DebuggerCommandTuple> g_cmd_list =
		{
			{ "k", StackTrace },
			{ "~", ListThreads },
			{ "lm", ListModule },
			{ "storage", DumpLocalMemoryStorage },
			{ "trunk", DumpMemoryTrunk },
			{ "mthash", DumpMTHash },
			{ "search", SearchCellEntry },
		};
	}
}
#endif
