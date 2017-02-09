// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <os/os.h>
#include <Trinity/String.h>
#include <Trinity/IO/File.h>
#include <cstdio>

#define FILE_BUF_SIZE 8192 * 1024

namespace Trinity
{
    namespace IO
    {
        class BinaryWriter
        {
        public:

            BinaryWriter(const String& output_file)
            {
                _wfopen_s(&fp, output_file.ToWcharArray(), L"wb");
                file_buf = new char[FILE_BUF_SIZE];
                setvbuf(fp, file_buf, _IOFBF, FILE_BUF_SIZE);
            }
            ~BinaryWriter()
            {
                fclose(fp);
                delete[] file_buf;
            }

            bool Write(bool value)
            {
                return (1 == fwrite(&value, sizeof(bool), 1, fp));
            }
            bool Write(char value)
            {
                return (1 == fwrite(&value, sizeof(char), 1, fp));
            }
            bool Write(int8_t value)
            {
                return (1 == fwrite(&value, sizeof(int8_t), 1, fp));
            }
            bool Write(uint8_t value)
            {
                return (1 == fwrite(&value, sizeof(uint8_t), 1, fp));
            }
            bool Write(int16_t value)
            {
                return (1 == fwrite(&value, sizeof(int16_t), 1, fp));
            }
            bool Write(uint16_t value)
            {
                return (1 == fwrite(&value, sizeof(uint16_t), 1, fp));
            }
            bool Write(int32_t value)
            {
                return (1 == fwrite(&value, sizeof(int32_t), 1, fp));
            }
            bool Write(uint32_t value)
            {
                return (1 == fwrite(&value, sizeof(uint32_t), 1, fp));
            }
            bool Write(int64_t value)
            {
                return (1 == fwrite(&value, sizeof(int64_t), 1, fp));
            }
            bool Write(uint64_t value)
            {
                return (1 == fwrite(&value, sizeof(uint64_t), 1, fp));
            }
            bool Write(float_t value)
            {
                return (1 == fwrite(&value, sizeof(float_t), 1, fp));
            }
            bool Write(double_t value)
            {
                return (1 == fwrite(&value, sizeof(double_t), 1, fp));
            }
            bool Write(char* buffer, int32_t start, int32_t size)
            {
                return ((size ? 1 : 0) == fwrite(buffer + start, size, 1, fp));
            }

        private:
            FILE* fp;
            char* file_buf;
        };

    }
}
