// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//

#pragma once
#include <cstdio>

enum TrinityErrorCode : int32_t
{
    E_NOENTRY               = -23,  // General purpose ENOENT
    E_INIT_FAIL             = -22,
    E_MANAGED_EXCEPTION     = -21,  // Exception propagated from managed runtime.
    E_UNLOAD_FAIL           = -20,  // For example, if unloading AppDomain failed.
    E_LOAD_FAIL             = -19,  // For example, if LoadLibrary() failed.
    E_MSG_OVERFLOW          = -18,
    E_NETWORK_SHUTDOWN      = -17,
    E_CELL_LOCK_OVERFLOW    = -16,
    E_TIMEOUT               = -15,
    E_DEADLOCK              = -14,  // For example, if the thread calling SaveStorage() still holds a lock.
    E_RPC_EXCEPTION         = -13,
    E_NOMEM                 = -12,
    E_NETWORK_RECV_FAILURE  = -11,
    E_READONLY              = -10,
    E_INVALID_ARGUMENTS     = -9,
    E_CELL_TYPE_NOT_ENABLED = -8,
    E_WRONG_CELL_TYPE       = -7,
    E_NETWORK_SEND_FAILURE  = -6,
    E_NO_FREE_ENTRY         = -5,   // No free entries
    E_DUPLICATED_CELL       = -4,   // For AddCell
    E_RETRY                 = -3,
    E_CELL_NOT_FOUND        = -2,
    E_FAILURE               = -1,
    E_SUCCESS               = 0,
    E_CELL_FOUND            = 1,
    E_ENUMERATION_END       = 2,
};
