// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <corelib>
#include <os/os.h>
#include <fstream>
#include "StreamWriter.h"

#if !defined(TRINITY_PLATFORM_WINDOWS)
inline int fopen_s(FILE ** _File, const char * _Filename, const char * _Mode)
{
    *_File = fopen(_Filename, _Mode);
    if (*_File == NULL) return errno;
    else return 0;
}

inline int _wfopen_s(FILE ** _File, const u16char * _Filename, const u32char * _Mode)
{
    String fname_str = String::FromWcharArray(_Filename, -1);
    String fmode_str = String::FromWcharArray(_Mode, -1);

    return fopen_s(_File, fname_str.c_str(), fmode_str.c_str());
}

inline int _wfopen_cswrapper(FILE ** _File, const u16char * _Filename, const u16char * _Mode)
{
    String fname_str = String::FromWcharArray(_Filename, -1);
    String fmode_str = String::FromWcharArray(_Mode, -1);

    return fopen_s(_File, fname_str.c_str(), fmode_str.c_str());
}

#endif

namespace Trinity
{
    namespace IO
    {
        namespace File
        {
            inline bool Exists(const String& path)
            {
#if defined(TRINITY_PLATFORM_WINDOWS)
                DWORD attrs = GetFileAttributesW(path.ToWcharArray());
                return (attrs != INVALID_FILE_ATTRIBUTES && !(attrs & FILE_ATTRIBUTE_DIRECTORY));
#else
                return (-1 != access(path.c_str(), F_OK));
#endif
            }
            //TODO encoding
            inline String ReadAllText(const String& filename)
            {
                static const size_t buf_size = 8192;
                Array<char> buf(buf_size + 1);
                FILE* fp;
                String ret;
                if (0 == fopen_s(&fp, filename.c_str(), "r"))
                {
                    if (fp)
                    {
                        while (!feof(fp) && !ferror(fp))
                        {
                            size_t bytes_read = fread(buf, sizeof(char), buf_size, fp);
                            buf[bytes_read] = 0;
                            ret.Append(buf);
                        }
                        fclose(fp);
                    }
                }
                return ret;
            }

            inline List<String> ReadAllLines(const String& filename)
            {
                static const size_t buf_size = 8192;
                Array<char> buf(buf_size + 1);
                List<String> ret;
                std::string linebuf;
                std::ifstream stream(filename.c_str());
                while (!stream.eof())
                {
                    std::getline(stream, linebuf);
                    ret.push_back(linebuf);
                }
                return ret;
            }

            inline void WriteAllText(const String& filename, const String& text)
            {
                StreamWriter sw;
                sw.Open(filename);
                sw.Write(text);
                sw.Close();
            }

        }
    }
}