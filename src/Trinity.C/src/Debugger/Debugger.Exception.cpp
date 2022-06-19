// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#if defined(TRINITY_PLATFORM_WINDOWS)
#include <Windows.h>
#include <io>
#include <threading>
#include "Debugger.h"

namespace Trinity
{
	namespace Debugger
	{
		std::mutex g_ExceptionLock;

		VOID PrintExceptionRecord(PEXCEPTION_RECORD pRecord)
		{
			/*Error code and description extracted from MSDN: http://msdn.microsoft.com/en-us/library/windows/desktop/aa363082(v=vs.85).aspx */

			LPSTR pErrorStr = NULL;

			switch (pRecord->ExceptionCode){

			case EXCEPTION_ACCESS_VIOLATION:
				pErrorStr = "The thread tried to read from or write to a virtual address for which it does not have the appropriate access.";
				break;

			case EXCEPTION_ARRAY_BOUNDS_EXCEEDED:
				pErrorStr = "The thread tried to access an array element that is out of bounds and the underlying hardware supports bounds checking.";
				break;

			case EXCEPTION_BREAKPOINT:
				pErrorStr = "A breakpoint was encountered.";
				break;

			case EXCEPTION_DATATYPE_MISALIGNMENT:
				pErrorStr = "The thread tried to read or write data that is misaligned on hardware that does not provide alignment. For example, 16-bit values must be aligned on 2-byte boundaries; 32-bit values on 4-byte boundaries, and so on.";
				break;

			case EXCEPTION_FLT_DENORMAL_OPERAND:
				pErrorStr = "One of the operands in a floating-point operation is denormal. A denormal value is one that is too small to represent as a standard floating-point value.";
				break;

			case EXCEPTION_FLT_DIVIDE_BY_ZERO:
				pErrorStr = "The thread tried to divide a floating-point value by a floating-point divisor of zero.";
				break;

			case EXCEPTION_FLT_INEXACT_RESULT:
				pErrorStr = "The result of a floating-point operation cannot be represented exactly as a decimal fraction.";
				break;

			case EXCEPTION_FLT_INVALID_OPERATION:
				pErrorStr = "This exception represents any floating-point exception not included in this list.";
				break;

			case EXCEPTION_FLT_OVERFLOW:
				pErrorStr = "The exponent of a floating-point operation is greater than the magnitude allowed by the corresponding type.";
				break;

			case EXCEPTION_FLT_STACK_CHECK:
				pErrorStr = "The stack overflowed or underflowed as the result of a floating-point operation.";
				break;

			case EXCEPTION_FLT_UNDERFLOW:
				pErrorStr = "The exponent of a floating-point operation is less than the magnitude allowed by the corresponding type.";
				break;

			case EXCEPTION_ILLEGAL_INSTRUCTION:
				pErrorStr = "The thread tried to execute an invalid instruction.";
				break;

			case EXCEPTION_IN_PAGE_ERROR:
				pErrorStr = "The thread tried to access a page that was not present, and the system was unable to load the page. For example, this exception might occur if a network connection is lost while running a program over the network.";
				break;

			case EXCEPTION_INT_DIVIDE_BY_ZERO:
				pErrorStr = "The thread tried to divide an integer value by an integer divisor of zero.";
				break;

			case EXCEPTION_INT_OVERFLOW:
				pErrorStr = "The result of an integer operation caused a carry out of the most significant bit of the result.";
				break;

			case EXCEPTION_INVALID_DISPOSITION:
				pErrorStr = "An exception handler returned an invalid disposition to the exception dispatcher. Programmers using a high-level language such as C should never encounter this exception.";
				break;

			case EXCEPTION_NONCONTINUABLE_EXCEPTION:
				pErrorStr = "The thread tried to continue execution after a noncontinuable exception occurred.";
				break;

			case EXCEPTION_PRIV_INSTRUCTION:
				pErrorStr = "The thread tried to execute an instruction whose operation is not allowed in the current machine mode.";
				break;

			case EXCEPTION_SINGLE_STEP:
				pErrorStr = "A trace trap or other single-instruction mechanism signaled that one instruction has been executed.";
				break;

			case EXCEPTION_STACK_OVERFLOW:
				pErrorStr = "The thread used up its stack.";
				break;

			default:
				pErrorStr = "Unknown exception code.";
			}

			printf("%s\n", pErrorStr);

			if (pRecord->ExceptionCode == EXCEPTION_ACCESS_VIOLATION)
			{
				printf("Attempting to %s ADDRESS 0x%016llx\n",
					[](uint64_t code)->LPSTR
				{
					switch (code){
					case 0:
						return "read";
					case 1:
						return "write";
					case 8:
						return "DEP";
					default:
						return "[UNKNOWN OPERATION]";
					}
				}(pRecord->ExceptionInformation[0]),
					pRecord->ExceptionInformation[1]);
			}

			if (pRecord->ExceptionRecord)
			{
				Console::WriteLine("Nested exception:");
				PrintExceptionRecord(pRecord->ExceptionRecord);
			}
		}

		LONG WINAPI TrinityUnhandledExceptionFilter(
			_In_ LPEXCEPTION_POINTERS ExceptionInfo)
		{
			g_ExceptionLock.lock();
			Console::WriteLine("\n\n****\n\n");
			Console::WriteLine("Thread {0} triggered unhandled exception:", GetThreadId(GetCurrentThread()));

			PrintExceptionRecord(ExceptionInfo->ExceptionRecord);

			Console::WriteLine("\n\n****\n\n");

			g_ExceptionLock.unlock();

			TryStartDebugger(true);

			//Don't die
			while (true)
				Sleep(1000);

			return EXCEPTION_CONTINUE_SEARCH;
		}

		void EnableUnhandledExceptionFilter()
		{
			SetUnhandledExceptionFilter(TrinityUnhandledExceptionFilter);
		}
	}
}
#endif
