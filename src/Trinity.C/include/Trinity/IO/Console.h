// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <iostream>
#include <Trinity/String.h>
#include <os/os.h>
#include <fcntl.h>

namespace Trinity
{
    namespace IO
    {
        namespace Console
        {
            enum ConsoleColor : int16_t
            {
                /*
                BLACK 0
                BLUE 1 BLUE
                GREEN 2 GREEN
                CYAN 3 BLUE + GREEN
                RED 4 RED
                MAGENTA 5 BLUE + RED
                BROWN 6 GREEN + RED
                LIGHTGRAY 7 BLUE + GREEN + RED
                DARKGRAY 8 INTENSITY
                LIGHTBLUE 9 BLUE + INTENSITY
                LIGHTGREEN 10 GREEN + INTENSITY
                LIGHTCYAN 11 BLUE + GREEN + INTENSITY
                LIGHTRED 12 RED + INTENSITY
                LIGHTMAGENTA 13 BLUE + RED + INTENSITY
                YELLOW 14 GREEN + RED + INTENSITY
                WHITE 15 BLUE + GREEN + RED + INTENSITY
                */
                ForegroundDark = 0x0000,
                ForegroundDarkBlue = 0x0001,
                ForegroundDarkGreen = 0x0002,
                ForegroundDarkCyan = 0x0003,
                ForegroundDarkRed = 0x0004,
                ForegroundDarkMagenta = 0x0005,
                ForegroundDarkYellow = 0x0006,
                ForegroundGray = 0x0007,
                ForegroundDarkGray = 0x0008,
                ForegroundBlue = 0x0009,
                ForegroundGreen = 0x000a,
                ForegroundCyan = 0x000b,
                ForegroundRed = 0x000c,
                ForegroundMagenta = 0x000d,
                ForegroundYellow = 0x000e,
                ForegroundWhite = 0x000f,

                BackgroundDark = 0x0000,
                BackgroundDarkBlue = 0x0010,
                BackgroundDarkGreen = 0x0020,
                BackgroundDarkCyan = 0x0030,
                BackgroundDarkRed = 0x0040,
                BackgroundDarkMagenta = 0x0050,
                BackgroundDarkYellow = 0x0060,
                BackgroundGray = 0x0070,
                BackgroundDarkGray = 0x0080,
                BackgroundBlue = 0x0090,
                BackgroundGreen = 0x00a0,
                BackgroundCyan = 0x00b0,
                BackgroundRed = 0x00c0,
                BackgroundMagenta = 0x00d0,
                BackgroundYellow = 0x00e0,
                BackgroundWhite = 0x00f0,
            };

            class __ConsoleInitializer
            {
            public:
                __ConsoleInitializer()
                {
#ifdef TRINITY_PLATFORM_WINDOWS
                    _setmode(_fileno(stdout), _O_U8TEXT);
#endif
#ifdef __cplusplus_cli
                    System::Console::OutputEncoding = System::Text::Encoding::UTF8;
#endif
                }
            };

#ifdef TRINITY_PLATFORM_WINDOWS
            static HANDLE handle = NULL;
            static __ConsoleInitializer __initializer;
#endif
            inline void _lock_console() {}
            inline void _unlock_console() {}

            inline void Flush()
            {
                fflush(stdout);
            }

            inline void SetColor(ConsoleColor color)
            {
#ifdef TRINITY_PLATFORM_WINDOWS
                if (handle == NULL)
                    handle = GetStdHandle(STD_OUTPUT_HANDLE);
                SetConsoleTextAttribute(handle, (WORD)color);
#else
                TRINITY_COMPILER_WARNING("Console::SetColor not implemented")
#endif
            }

            inline void SetColor(int16_t color)
            {
                SetColor((ConsoleColor)color);
            }

            inline void ResetColor()
            {
#ifdef TRINITY_PLATFORM_WINDOWS
                if (handle == NULL)
                    handle = GetStdHandle(STD_OUTPUT_HANDLE);
                SetConsoleTextAttribute(handle, ForegroundGray);
#endif
            }

            inline void Write(const String& line)
            {
                _lock_console();
#ifdef TRINITY_PLATFORM_WINDOWS
                std::wcout << (u16char*)line.ToWcharArray();
#else
                std::cout << line;
#endif
                _unlock_console();
            }

            inline void WriteLine()
            {
                _lock_console();
#ifdef TRINITY_PLATFORM_WINDOWS
                std::wcout << std::endl;
#else
                std::cout << std::endl;
#endif
                _unlock_console();
            }

            template<typename ...Args>
            inline void Write(const String& format, Args... arguments)
            {
                Write(String::Format(format, arguments...));
            }

            template<typename T>
            inline void Write(T argument)
            {
                Write(String::Format("{0}", argument));
            }

            template<typename ...Args>
            inline void WriteLine(Args... arguments)
            {
                Write(arguments...);
                WriteLine();
            }

            template<typename ...Args>
            inline void Write(int16_t color, Args... arguments)
            {
                SetColor(color);
                Write(arguments...);
                ResetColor();
            }

            template<typename ...Args>
            inline void WriteLine(int16_t color, Args... arguments)
            {
                SetColor(color);
                WriteLine(arguments...);
                ResetColor();
            }

            inline String ReadLine()
            {
                std::string str;
                std::getline(std::cin, str);
                return str;
            }

            inline int32_t ReadKey()
            {
                return getchar();
            }
        }
    }
}