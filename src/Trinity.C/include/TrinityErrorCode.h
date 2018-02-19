// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
// stdafx.h : include file for standard system include files,
// or project specific include files that are used frequently, but
// are changed infrequently
//

#pragma once
#include <cstdio>

enum TrinityErrorCode : int32_t
{
    E_MSG_OVERFLOW          = -15,
    E_NETWORK_SHUTDOWN      = -14,
    E_RPC_EXCEPTION         = -13,
    E_NOMEM                 = -12,
    E_NETWORK_RECV_FAILURE  = -11,
    E_READONLY              = -10,
    E_INVALID_ARGUMENTS     = -9,
    E_CELL_TYPE_NOT_ENABLED = -8,
    E_WRONG_CELL_TYPE       = -7,
    E_NETWORK_SEND_FAILURE  = -6,
    E_NO_FREE_ENTRY         = -5, //No free entries
    E_DUPLICATED_CELL       = -4, //For AddCell
    E_RETRY                 = -3,
    E_CELL_NOT_FOUND        = -2,
    E_FAILURE               = -1,
    E_SUCCESS               = 0,
    E_CELL_FOUND            = 1,
    E_ENUMERATION_END       = 2,
};
