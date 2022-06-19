// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"
#define prime_count 72

namespace HashHelper
{
    uint32_t GetMinPrime();
    uint32_t GetPrime(uint32_t min);
    bool IsPrime(uint32_t candidate);

}
