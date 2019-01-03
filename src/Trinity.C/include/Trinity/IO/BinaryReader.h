// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <os/os.h>
#include <fstream>
#include <Trinity/String.h>
#include <Trinity/IO/File.h>
#define FILE_BUF_SIZE 8192 * 1024

namespace Trinity
{
    namespace IO
    {
        class BinaryReader
        {
        public:
            BinaryReader(const String &output_file)
            {
                file_buf = new char[FILE_BUF_SIZE];
                if (!_wfopen_s(&fp, output_file.ToWcharArray(), _u("rb")))
                {
                    setvbuf(fp, file_buf, _IOFBF, FILE_BUF_SIZE);
                }
                else
                {
                    fp = NULL;
                }
            }
            ~BinaryReader()
            {
                if (fp) { fclose(fp); }
                delete[] file_buf;
            }

            bool ReadBoolean()
            {
                fread(buff, sizeof(bool), 1, fp);
                return buff[0] != 0;
            }
            char ReadChar()
            {
                fread(buff, sizeof(char), 1, fp);
                return (char) buff[0];
            }
            int8_t ReadInt8()
            {
                fread(buff, sizeof(int8_t), 1, fp);
                return *(int8_t*) buff;
            }
            uint8_t ReadUInt8()
            {
                fread(buff, sizeof(uint8_t), 1, fp);
                return *(uint8_t*) buff;
            }
            int16_t ReadInt16()
            {
                fread(buff, sizeof(int16_t), 1, fp);
                return *(int16_t*) buff;
            }
            uint16_t ReadUInt16()
            {
                fread(buff, sizeof(uint16_t), 1, fp);
                return *(uint16_t*) buff;
            }
            int32_t ReadInt32()
            {
                fread(buff, sizeof(int32_t), 1, fp);
                return *(int32_t*) buff;
            }
            uint32_t ReadUInt32()
            {
                fread(buff, sizeof(uint32_t), 1, fp);
                return *(uint32_t*) buff;
            }
            int64_t ReadInt64()
            {
                fread(buff, sizeof(int64_t), 1, fp);
                return *(int64_t*) buff;
            }
            uint64_t ReadUInt64()
            {
                fread(buff, sizeof(uint64_t), 1, fp);
                return *(uint64_t*) buff;
            }
            float_t ReadFloat()
            {
                fread(buff, sizeof(float_t), 1, fp);
                return *(float_t*) buff;
            }
            double_t ReadDouble()
            {
                fread(buff, sizeof(double_t), 1, fp);
                return *(double_t*) buff;
            }
            bool Read(char* buffer, int32_t start, int32_t count)
            {
                return (1 == fread(buffer + start, count, 1, fp));
            }
            inline bool Good()
            {
                return (fp != NULL);
            }
        private:
            FILE* fp;
            char* file_buf;
            char  buff[16];
        };
    }
}
