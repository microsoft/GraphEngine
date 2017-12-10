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
		static bool s_DebuggerStarted = false;
		static Mutex s_DebuggerMutex;

		void printError(char* msg)
		{
			DWORD eNum;
			Array<u16char> sysMsg(256);
			u16char* p;

			eNum = GetLastError();
			FormatMessageW(FORMAT_MESSAGE_FROM_SYSTEM | FORMAT_MESSAGE_IGNORE_INSERTS,
				NULL, eNum,
				MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT), // Default language
				sysMsg, 256, NULL);

			// Trim the end of the line and terminate it with a null
			p = sysMsg;
			while ((*p > 31) || (*p == 9))
				++p;
			do { *p-- = 0; } while ((p >= sysMsg) &&
				((*p == '.') || (*p < 33)));

			// Display the message
			printf("\n  WARNING: %s failed with error %d (%s)", msg, eNum, String::FromWcharArray(sysMsg).c_str());
		}


		extern List<DebuggerCommandTuple> g_cmd_list;

		bool ExecuteDebuggerCommand(String& cmd, Array<String>& args)
		{
			for (auto &entry : g_cmd_list)
			{
				if (entry.m_string == cmd)
					return entry.m_cmd(args);
			}
			return false;
		}

		void DebuggerEntry()
		{
			DWORD threadId = GetCurrentThreadId();
			while (true)
			{
				Console::Write(Console::ForegroundRed, ">");
				auto line = Console::ReadLine();
				Console::WriteLine();

				auto args = line.Split(" ");

				if (args.Length() < 1)
					continue;

				if (!ExecuteDebuggerCommand(args[0], args))
					Console::WriteLine(Console::ForegroundRed, "Command not understood.");

				Console::WriteLine("\n");
			}
		}

		void TryStartDebugger(bool suspendOthers)
		{
			s_DebuggerMutex.lock();
			if (s_DebuggerStarted)
			{
				s_DebuggerMutex.unlock();
				return;
			}
			s_DebuggerStarted = true;
			s_DebuggerMutex.unlock();

			Console::WriteLine("Debugger started.");

			if (suspendOthers)
				SuspendOtherThreads();

			//TODO check deadlocks!
			DebuggerEntry();

		}
	}
}
#endif