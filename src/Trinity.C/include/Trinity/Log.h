// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "String.h"

namespace Trinity
{
    namespace Diagnostics
    {
        /**! Should be synchronized with Log.cs */
        enum LogLevel : int32_t
        {
            /// <summary>
            /// No message is logged
            /// </summary>
            Off     = 0,

            /// <summary>
            /// Only unrecoverable system errors are logged
            /// </summary>
            Fatal   = 1,

            /// <summary>
            /// Unrecoverable system errors and application logLevel errors are logged
            /// </summary>
            Error   = 2,

            /// <summary>
            /// Fatal system error, application error and application warning are logged
            /// </summary>
            Warning = 3,

            /// <summary>
            /// All errors, warnings and notable application messages are logged
            /// </summary>
            Info    = 4,

            /// <summary>
            /// All errors, warnings, application messages and debugging messages are logged
            /// </summary>
            Debug   = 5,

            /// <summary>
            /// All messages are logged
            /// </summary>
            Verbose = 6,
        };

        typedef struct
        {
            u16char* logMessage;
            int64_t  logTimestamp;
            LogLevel logLevel;
        } LOG_ENTRY, *PLOG_ENTRY;
    }
}