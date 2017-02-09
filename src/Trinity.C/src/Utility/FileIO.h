// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"
namespace Trinity
{
    namespace FileIO
    {
        void * OpenFile4Write(u16char* FileName);

        void * OpenFile4Read(u16char* FileName);

        int64_t  GetFileSize(void* fp);

        void  CloseFileHandle(void* fp);

        int32_t  ReadInt(void* fp);

        bool  WriteInt(void* fp, int32_t value);

        bool  WriteBuffer(void* fp, char* buf, size_t length);

        bool  ReadBuffer(void* fp, char* buf, size_t length);

        bool  WriteBufferToFile(u16char* FileName, char* buf, size_t length);

        bool  AppendBufferToFile(u16char* FileName, char* buf, size_t length);

        bool  ReadBufferFromFile(u16char* FileName, char* buf, size_t length);
    }
}