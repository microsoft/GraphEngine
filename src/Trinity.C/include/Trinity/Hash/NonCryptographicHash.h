// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"

namespace Trinity
{
    namespace Hash
    {
        inline void phong(uint32_t *hp, uint8_t* p, size_t len)
        {
            uint8_t* e = p + len;
            uint32_t h = *hp;
            for (; p != e; )
            {
                uint8_t c = *p++;
                if (c == 0 && p > e)
                    break;
                h = 0x63c63cd9 * h + 0x9c39c33d + c;
            }
            *hp = h;
        }
        inline void fnv_1(uint32_t *hp, uint8_t* p, size_t len)
        {
            uint8_t* e = p + len;
            uint32_t h = *hp;
            for (; p != e; ++p)
            {
                h *= 16777619;
                h ^= *p;
            }
            *hp = h;
        }
        inline void hash_64(uint64_t *hp, uint8_t* p, size_t len)
        {
            fnv_1((uint32_t*)(hp), p, len);
            phong((uint32_t*)(hp)+1, p, len);
        }
        template<typename TH, typename TD, typename TFunc>
        inline void H(TH* hp, TD* p, const TFunc& f)
        {
            f(hp, (uint8_t*)(p), sizeof(TD));
        }
    }
}
