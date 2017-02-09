// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <os/os.h>
#if defined (TRINITY_PLATFORM_WINDOWS)
#include <Windows.h>
#include <TlHelp32.h>
#include <corelib>

namespace Trinity
{
	namespace Debugger
	{
		void EnableUnhandledExceptionFilter();
		void TryStartDebugger(bool suspendOtherThreads);

		List<THREADENTRY32> GetThreads(DWORD pid);
		BOOL SuspendOtherThreads();

		void printError(char* msg);

		typedef bool(*DebuggerCommand)(Array<string>&);

		struct DebuggerCommandTuple
		{
			String m_string;
			DebuggerCommand m_cmd;
		};
	}
}
#endif