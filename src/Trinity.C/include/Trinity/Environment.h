// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "String.h"
#include <cstdint>
#include <vector>
#include <os/os.h>

#if !defined(TRINITY_PLATFORM_WINDOWS)
#include <thread>
#endif

namespace Trinity
{
	namespace Environment
	{
		inline String NewLine()
		{
			return "\r\n";
		}

		inline String GetCurrentDirectory()
		{
#if defined(TRINITY_PLATFORM_WINDOWS)
			DWORD size = ::GetCurrentDirectoryW(0, NULL);
			if (size == 0)
				return "";
			Array<u16char> path(size);
			if (0 == ::GetCurrentDirectoryW(size, path))
				return "";
			return String::FromWcharArray(path);
#else
            char* dir_buf = new char[1024];
            char* cwd_buf = getcwd(dir_buf, 1024);
            if(cwd_buf == NULL) { cwd_buf[0] = 0; }
            String ret(dir_buf);
            delete[] dir_buf;
            return ret;
#endif
		}
		inline bool SetCurrentDirectory(const String& workDir)
		{
#if defined(TRINITY_PLATFORM_WINDOWS)
			auto str = workDir.ToWcharArray();
			return (TRUE == ::SetCurrentDirectoryW(str));
#else
            return !chdir(workDir.c_str());
#endif
		}

		inline void Exit(int exitCode = 0)
		{
			exit(exitCode);
		}

		inline uint32_t GetProcessorCount()
		{
#if defined(TRINITY_PLATFORM_WINDOWS)
			SYSTEM_INFO systemInfo;
			ZeroMemory(&systemInfo, sizeof(systemInfo));
			GetSystemInfo(&systemInfo);
			return systemInfo.dwNumberOfProcessors;
#else
            return std::thread::hardware_concurrency();
#endif
		}

        inline std::vector<String> Run(const char* command)
        {
            std::vector<String> ret;
            Array<char>   lplinebuf(1024);
            FILE*         pspipe = _popen(command, "r");

            if (nullptr == pspipe)
            {
                return ret;
            }

            while (fgets(lplinebuf, (int)lplinebuf.Length(), pspipe))
            {
                ret.push_back(String(lplinebuf));
            }

            _pclose(pspipe);

            return ret;
        }
	}
}