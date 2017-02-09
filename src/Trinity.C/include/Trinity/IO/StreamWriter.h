// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <fstream>
#include <os/os.h>
#include <Trinity/String.h>
#include <Trinity/IO/File.h>

namespace Trinity
{
    namespace IO
    {
        class StreamWriter
        {
        public:
            StreamWriter() = default;

            void Open(const String& filename)
            {
#if defined TRINITY_PLATFORM_WINDOWS
                stream.open(filename.ToWcharArray(), std::ios_base::out);
#else
                stream.open(filename.c_str(), std::ios_base::out);
#endif
            }

            void Close()
            {
                stream.close();
            }

            StreamWriter(const String& output_file)
            {
                Open(output_file);
            }
            StreamWriter(StreamWriter&& rval)
            {
                stream = std::move(rval.stream);
            }

            void Write(const String& text)
            {
                stream << text;
            }

            template<typename ...Args>
            void Write(const String& format, Args... args)
            {
                stream << String::Format(format, args...);
            }

            void WriteLine(const String& line)
            {
                stream << line << std::endl;
            }

            template<typename ...Args>
            void WriteLine(const String& format, Args... args)
            {
                stream << String::Format(format, args...) << std::endl;
            }

            void WriteLine()
            {
                stream << std::endl;
            }

            void Flush()
            {
                stream.flush();
            }

            bool Good()
            {
                return stream.good();
            }


        private:
            std::ofstream stream;
        };

    }
}