// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

#pragma once

#include <cstdint>
#include <atomic>
#include <algorithm>

#include "os/os.h"
#include "arch/cpu.h"
#include "TrinityErrorCode.h"

typedef int64_t cellid_t;

/**! Should be synchronized with Trinity.Core\TSL\Lib\Common.cs */
enum CellAccessOptions : int32_t
{
    /// <summary>
    /// No actions. This entry should not be used.
    /// </summary>
    None                         = 0x0,
    /// <summary>
    /// Throws an exception when a cell is not found.
    /// </summary>
    ThrowExceptionOnCellNotFound = 0x1,
    /// <summary>
    /// Creates a new cell when a cell is not found.
    /// </summary>
    CreateNewOnCellNotFound      = 0x2,
    /// <summary>
    /// Specifies that write-ahead-log should be performed with strong durability.
    /// </summary>
    StrongLogAhead               = 0x4,
    /// <summary>
    /// Specifies that write-ahead-log should be performed with weak durability. This option brings better performance,
    /// but the durability may be degraded when this option is used.
    /// </summary>
    WeakLogAhead                 = 0x8,
};
