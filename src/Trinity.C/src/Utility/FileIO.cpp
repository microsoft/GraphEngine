// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Utility/FileIO.h"
#include <io>
#include <sys/stat.h>
#include <sys/types.h>

#define BUF_RECORD_CNT (length ? 1 : 0)

namespace Trinity
{
    namespace FileIO
    {
        void * OpenFile4Write(u16char* FileName)
        {
            FILE* fp_p = nullptr;
            if (!_wfopen_s(&fp_p, FileName, _u("wb")))
                return (void*)(fp_p);
            return nullptr;
        }

        void * OpenFile4Read(u16char* FileName)
        {
            FILE* fp_p = nullptr;
            if (!_wfopen_s(&fp_p, FileName, _u("rb")))
                return (void*)(fp_p);
            return nullptr;
        }

        int64_t GetFileSize(void* fp)
        {
#if defined(TRINITY_PLATFORM_WINDOWS)
            int fd = _fileno((FILE*)fp);
            struct __stat64 buf;
            if (_fstat64(fd, &buf)) { return -1; }
            else return buf.st_size;
#else
            int fd = fileno((FILE*)fp);
            struct stat64 buf;
            if (fstat64(fd, &buf)) { return -1; }
            else return buf.st_size;
#endif
        }

        void  CloseFileHandle(void* fp)
        {
            fclose((FILE*)fp);
        }

        int32_t  ReadInt(void* fp)
        {
            char int_bytes[4];
            fread(int_bytes, 4, 1, (FILE*)fp);
            return *(int*)int_bytes;
        }

        bool  WriteInt(void* fp, int32_t value)
        {
            char int_bytes[4];
            *(int*)int_bytes = value;
            return (1 == fwrite(int_bytes, 4, 1, (FILE*)fp));
        }

        bool  WriteBuffer(void* fp, char* buf, size_t length)
        {
            return (BUF_RECORD_CNT == fwrite(buf, length, 1, (FILE*)fp)) && !fflush((FILE*)fp);
        }

        bool  ReadBuffer(void* fp, char* buf, size_t length)
        {
            return (BUF_RECORD_CNT == fread(buf, length, 1, (FILE*)fp));
        }

        bool  WriteBufferToFile(u16char* FileName, char* buf, size_t length)
        {
            FILE* fp      = nullptr;
            bool  success = !_wfopen_s(&fp, FileName, _u("wb"));

            if (success)
            {
                success = (BUF_RECORD_CNT == fwrite(buf, length, 1, fp));
                success = success && !fflush(fp);
                success = !fclose(fp) && success;
            }

            return success;
        }

        bool  AppendBufferToFile(u16char* FileName, char* buf, size_t length)
        {
            FILE* fp      = nullptr;
            bool  success = !_wfopen_s(&fp, FileName, _u("ab"));

            if (success)
            {
                success = (BUF_RECORD_CNT == fwrite(buf, length, 1, fp));
                success = success && !fflush(fp);
                success = !fclose(fp) && success;
            }

            return success;
        }

        bool  ReadBufferFromFile(u16char* FileName, char* buf, size_t length)
        {
            FILE* fp      = nullptr;
            bool  success = !_wfopen_s(&fp, FileName, _u("ab"));

            if (success)
            {
                success = (BUF_RECORD_CNT == fread(buf, length, 1, fp));
                success = !fclose(fp) && success;
            }

            return success;
        }
    }
}
