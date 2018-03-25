// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#include "Trinity/Collections/List.h"
#include "Trinity/Configuration/TrinityConfig.h"
#include "BackgroundThread/BackgroundThread.h"
#include "LogAutoFlushTask.h"
#include <corelib>
#include <io>
#include <atomic>
#include <thread>
#include <mutex>
#include <deque>


#if defined(TRINITY_PLATFORM_WINDOWS)
#include <DbgHelp.h>
#endif

#include "Log.h"

using namespace Trinity;
using namespace Trinity::IO;

namespace Trinity
{
    namespace Diagnostics
    {
        const size_t MAX_LOG_ENTRY_COUNT = 1 << 20;
        typedef struct
        {
            u16char* logMessage;
            int64_t  logTimestamp;
            LogLevel logLevel;
        } LOG_ENTRY, *PLOG_ENTRY;
        // static data and methods

        static std::recursive_mutex s_LogMutex;
        static std::recursive_mutex s_LogInitMutex;
        static std::deque<LOG_ENTRY>s_LogEntries;
        static LogLevel             s_CurrentLogLevel = LogLevel::Info;
        static String               s_LogLevelName[7]  { "OFF", "Fatal", "Error", "Warning", "Info", "Debug", "Verbose", };
        static String               s_LoggingPrefix[7] { "[ OFF     ] ", "[ FATAL   ] ", "[ ERROR   ] ", "[ WARNING ] ", "[ INFO    ] ", "[ DEBUG   ] ", "[ VERBOSE ] " };
        static String               s_LogFileName;
        static StreamWriter         s_LogStream;
        static bool                 s_EchoOnConsole = true;
        static bool                 s_initialized = false;
        static LogAutoFlushTask*    s_bgtask = nullptr;

        void CloseFile()
        {
            std::lock_guard<std::recursive_mutex> lock(s_LogInitMutex);
            std::lock_guard<std::recursive_mutex> lock2(s_LogMutex);

            if (s_LogStream.Good()) try { s_LogStream.Close(); } catch (...) {}
        }

        void InitializeLogger()
        {
            std::lock_guard<std::recursive_mutex> lock(s_LogInitMutex);
            if (s_initialized) return;

            s_bgtask = new LogAutoFlushTask();
            BackgroundThread::TaskScheduler::AddTask(s_bgtask);
            s_initialized = true;
        }

        void UninitializeLogger()
        {
            std::lock_guard<std::recursive_mutex> lock(s_LogInitMutex);
            std::lock_guard<std::recursive_mutex> lock2(s_LogMutex);
            if (!s_initialized) return;

            CloseFile();
            s_LogEntries.clear();
            BackgroundThread::TaskScheduler::RemoveTask(s_bgtask);
            // !s_bgtask is wrapped in ref, no need to delete
            s_bgtask = nullptr;

            s_initialized = false;
        }

        void OpenFile(String log_path)
        {
            std::lock_guard<std::recursive_mutex> lock(s_LogInitMutex);
            std::lock_guard<std::recursive_mutex> lock2(s_LogMutex);

            CloseFile();

            //If two instances are started at the same time, one of them would fail to open log file.
        DetermineLogFileName:
            if (!Path::_CompletePath(log_path, true)) 
            {
                WriteLine(LogLevel::Error, "Log: cannot create log directory '{0}', logging to file is disabled.", log_path);
                return;
            }

            s_LogFileName = Path::Combine(log_path, "trinity-[" + DateTime::Now().ToStringForFilename() + "].log");
            s_LogStream.Open(s_LogFileName);
            if (!s_LogStream.Good())
            {
                if (File::Exists(s_LogFileName))
                {
                    WriteLine(LogLevel::Warning, "Logger: log file {0} already exists, waiting for one second before generating a new log file name.", s_LogFileName);
                    std::this_thread::sleep_for(std::chrono::milliseconds(1000));
                    goto DetermineLogFileName;
                }
                else
                {
                    //  If the file is absent, it means that we cannot access the log file. abort.
                    WriteLine(LogLevel::Error, "Logger: cannot open log file {0} for write, logging to file is disabled.", s_LogFileName);
                }
            }
        }

        static String LogLevelToString(LogLevel level)
        {
            return s_LogLevelName[level];
        }

        static void _flush_if_necessary(LogLevel level)
        {
            if (level <= LogLevel::Info)
                s_LogStream.Flush();
        }

        // Caller holds s_LogMutex
        static inline void _post_log_entry(const LogLevel logLevel, const String& msg, DateTime& time)
        {
            // Drop oldest entries if the capacity limit is reached
            while (s_LogEntries.size() >= MAX_LOG_ENTRY_COUNT)
            {
                //free msg buffer
                free(s_LogEntries.front().logMessage);
                s_LogEntries.pop_front();
            }

            auto wchar_arr = msg.ToWcharArray();
            u16char *buf   = (u16char*)malloc(sizeof(u16char) * wchar_arr.Length());
            std::copy(wchar_arr.begin(), wchar_arr.end(), buf);
            
            s_LogEntries.emplace_back(LOG_ENTRY
            {
                buf,
                time.Timestamp(),
                logLevel,
            });
        }

        //////////////////////////////

        // Interface implementations

        void PrintCallStack(void)
        {
#if defined(TRINITY_PLATFORM_WINDOWS)
            uint32_t  i;
            void         * stack[100];
            unsigned short frames;
            SYMBOL_INFO  * symbol;
            HANDLE         process;

            process = GetCurrentProcess();
            SymInitialize(process, NULL, TRUE);

            frames = CaptureStackBackTrace(0, 100, stack, NULL);
            symbol = (SYMBOL_INFO *)calloc(sizeof(SYMBOL_INFO) + 256 * sizeof(char), 1);

            if (symbol == NULL)
            {
                Trinity::IO::Console::WriteLine("Cannot calloc symbol buffer");
                return;
            }

            symbol->MaxNameLen = 255;
            symbol->SizeOfStruct = sizeof(SYMBOL_INFO);

            {
                std::lock_guard<std::recursive_mutex> lock(s_LogMutex);
                for (i = 1 /*skip myself*/; i < frames; i++)
                {
                    SymFromAddr(process, (DWORD64)(stack[i]), 0, symbol);
                    String func_name = String(symbol->Name);
                    Trinity::IO::Console::WriteLine("{0}: {1} - {2}", frames - i - 1, func_name, (LPVOID)symbol->Address);
                }
            }

            free(symbol);

            Trinity::IO::Console::WriteLine("Press any key to continue ...");
#pragma warning(suppress: 6031)
            getchar();
#else
            Trinity::IO::Console::WriteLine("PrintStack: Windows only.");
#endif
        }

        void _writeline_impl(LogLevel logLevel, const String& msg)
        {
            if (logLevel <= s_CurrentLogLevel && logLevel != LogLevel::Off)
            {
                std::lock_guard<std::recursive_mutex> lock(s_LogMutex);

                DateTime dt     = DateTime::Now();
                String   time   = "[" + dt.ToString() + "]";
                String   header = s_LoggingPrefix[(int)logLevel];

                _post_log_entry(logLevel, msg, dt);

                if (s_EchoOnConsole)
                {
                    Console::WriteLine(header + msg);
                }

                if (s_LogStream.Good())
                {
                    s_LogStream.WriteLine(time + header + msg);
                    _flush_if_necessary(logLevel);
                }

            }
        }

        void Flush()
        {
            std::lock_guard<std::recursive_mutex> lock(s_LogMutex);

            if (s_LogStream.Good())
            {
                s_LogStream.Flush();
            }

            Console::Flush();
        }

        void _writeline_impl(LogLevel logLevel, const char* srcFile, int32_t lineNum, const String& msg)
        {
            if (logLevel <= s_CurrentLogLevel && logLevel != LogLevel::Off)
            {
                String message;

                message.Append(String::Format("::Begin LogEntry (LogLevel:{0})", LogLevelToString(logLevel)));
#if defined(TRINITY_PLATFORM_WINDOWS)
                message.Append(String::Format("    ProcessId: {0}, ThreadId: {1}, Elapsed Seconds: {2}", GetCurrentProcessId(), GetCurrentThreadId(), ((double)clock()) / CLOCKS_PER_SEC));
#else
                message.Append(String::Format("    ProcessId: {0}, ThreadId: {1}, Elapsed Seconds: {2}", getpid(), pthread_self(), ((double)clock()) / CLOCKS_PER_SEC));
#endif
                message.Append(String::Format("    File: {0} \t Line: {1}", srcFile, lineNum));
                message.Append(msg);
                message.Append("::End LogEntry");

                _writeline_impl(logLevel, message);
            }
        }

        void SetLogLevel(LogLevel level)
        {
            s_CurrentLogLevel = level;
            WriteLine(LogLevel::Info, "LogLevel set to {0}", s_LoggingPrefix[level]);
        }

        void SetEchoOnConsole(bool is_set)
        {
            s_EchoOnConsole = is_set;
            WriteLine(LogLevel::Info, "EchoOnConsole set to {0}", is_set ? "ON" : "OFF");
        }

        //////////////////////////////
    }
}

DLL_EXPORT
VOID __stdcall CLogOpenFile(const u16char* logDir)
{
    Trinity::Diagnostics::OpenFile(logDir);
}

DLL_EXPORT
VOID __stdcall CLogCloseFile()
{
    Trinity::Diagnostics::CloseFile();
}

DLL_EXPORT
VOID __stdcall CLogInitialize()
{
    Trinity::Diagnostics::InitializeLogger();
}

DLL_EXPORT
VOID __stdcall CLogUninitialize()
{
    Trinity::Diagnostics::UninitializeLogger();
}

DLL_EXPORT
VOID __stdcall CLogWriteLine(Trinity::Diagnostics::LogLevel logLevel, const u16char* str)
{
    String msg_str = String::FromWcharArray(str, -1);
    _writeline_impl(logLevel, msg_str);
}

DLL_EXPORT
VOID __stdcall CLogFlush()
{
    Trinity::Diagnostics::Flush();
}

DLL_EXPORT
VOID __stdcall CLogSetLogLevel(Trinity::Diagnostics::LogLevel level)
{
    Trinity::Diagnostics::SetLogLevel(level);
}

DLL_EXPORT
VOID __stdcall CLogSetEchoOnConsole(bool is_set)
{
    Trinity::Diagnostics::SetEchoOnConsole(is_set);
}

// E_SUCCESS if there are one or more entries collected.
// E_FAILURE if failed to retrieve, or there are no entries committed.
DLL_EXPORT
TrinityErrorCode __stdcall CLogCollectEntries(OUT size_t& arr_size, OUT PLOG_ENTRY& entries)
{
    std::lock_guard<std::recursive_mutex> lock(s_LogMutex);
    arr_size   = s_LogEntries.size();
    entries    = arr_size == 0 ? nullptr : (PLOG_ENTRY)malloc(sizeof(LOG_ENTRY) * arr_size);
    std::copy(s_LogEntries.begin(), s_LogEntries.end(), entries);
    s_LogEntries.clear();
    return arr_size == 0 ? TrinityErrorCode::E_FAILURE : TrinityErrorCode::E_SUCCESS;
}
