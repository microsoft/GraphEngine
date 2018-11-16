// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <TrinityCommon.h>

namespace Storage
{
    typedef struct
    {
        int64_t LowBits;
        int64_t HighBits;
    }MD5_SIGNATURE, *PMD5_SIGNATURE;

    namespace LocalMemoryStorage
    {
        enum Int32_Constants : int32_t
        {
            c_MaxTrunkCount = 256,
        };

        typedef struct
        {
            /* version (ulong) + md5 * trunkCount */
            uint64_t         version;
            MD5_SIGNATURE    trunk_signatures[c_MaxTrunkCount];
        }TRINITY_IMAGE_SIGNATURE, *PTRINITY_IMAGE_SIGNATURE;

        namespace Logging
        {

#pragma pack(push, 1)
            typedef struct
            {
                cellid_t    CELL_ID;
                int32_t     CONTENT_LEN;
                uint16_t    CELL_TYPE;
                uint8_t     CHECKSUM; // 8-bit second-order check
            }LOG_RECORD_HEADER, *PLOG_RECORD_HEADER;
#pragma pack(pop)
        }
    }
}