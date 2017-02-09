// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if defined(TRINITY_PLATFORM_WINDOWS)
#include <Windows.h>
#include <TlHelp32.h>

#include "Debugger.h"
#include <threading>
#include <collections>
#include <corelib>
#include <io>

namespace Trinity
{
	namespace Debugger
	{
		List<THREADENTRY32> GetThreads(DWORD dwOwnerPID)
		{
			List<THREADENTRY32> ret;

			HANDLE hThreadSnap = INVALID_HANDLE_VALUE;
			THREADENTRY32 te32;

			// Take a snapshot of all running threads  
			hThreadSnap = CreateToolhelp32Snapshot(TH32CS_SNAPTHREAD, 0);
			if (hThreadSnap == INVALID_HANDLE_VALUE)
				return ret;

			// Fill in the size of the structure before using it. 
			te32.dwSize = sizeof(THREADENTRY32);

			// Retrieve information about the first thread,
			// and exit if unsuccessful
			if (!Thread32First(hThreadSnap, &te32))
			{
				printError("Thread32First");  // Show cause of failure
				CloseHandle(hThreadSnap);     // Must clean up the snapshot object!
				return ret;
			}

			// Now walk the thread list of the system,
			// and display information about each thread
			// associated with the specified process
			do
			{
				if (te32.th32OwnerProcessID == dwOwnerPID)
				{
					ret.push_back(te32);
				}
			} while (Thread32Next(hThreadSnap, &te32));

			//  Don't forget to clean up the snapshot object.
			CloseHandle(hThreadSnap);
			return ret;
		}

		BOOL SuspendOtherThreads()
		{
			BOOL success = TRUE;

			auto myThreadId = GetCurrentThreadId();

			if (!myThreadId)
			{
				printError("myThreadId = GetThreadId");
				return FALSE;
			}

			auto threads = GetThreads(GetCurrentProcessId());

			if (threads.size() == 0)
				return FALSE;

			for (auto t : threads)
				if (t.th32ThreadID != myThreadId)
				{
					HANDLE hThread = OpenThread(THREAD_ALL_ACCESS, false, t.th32ThreadID);
					if (!hThread)
					{
						printError("OpenThread");
						success = FALSE;
						continue;
					}
					if (-1 == SuspendThread(hThread))
					{
						printError("SuspendThread");
						success = FALSE;
					}
					CloseHandle(hThread);
				}

			return success;
		}
	}
}
#endif