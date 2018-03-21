// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity
{
    /// <summary>
    /// Represents the collection of Trinity error code.
    /// </summary>
    public enum TrinityErrorCode : int
    {
        /// <summary>
        /// Indicates that a response message is too long.
        /// </summary>
        E_MSG_OVERFLOW          = -15,
        /// <summary>
        /// Indicates that the network subsystem has shut down.
        /// </summary>
        E_NETWORK_SHUTDOWN      = -14,
        /// <summary>
        /// Indicates a remote message handler exception.
        /// </summary>
        E_RPC_EXCEPTION         = -13,
        /// <summary>
        /// Indicates memory allocation failure.
        /// </summary>
        E_NOMEM                 = -12,
        /// <summary>
        /// Represents network receiving failure.
        /// </summary>
        E_NETWORK_RECV_FAILURE  = -11,
        /// <summary>
        /// Indicates that write operation is done on a readonly storage.
        /// </summary>
        E_READONLY              = -10,
        /// <summary>
        /// Indicates invalid arguments when calling a method.
        /// </summary>
        E_INVALID_ARGUMENTS = -9,

        /// <summary>
        /// Indicates the cell type feature is not enabled.
        /// </summary>
        E_CELL_TYPE_NOT_ENABLED =-8,

        /// <summary>
        /// Represents an error when the expected cell type mismatches the existing one in the system.
        /// </summary>
        E_WRONG_CELL_TYPE = -7,

        /// <summary>
        /// Represents network sending failure.
        /// </summary>
        E_NETWORK_SEND_FAILURE = -6,

        /// <summary>
        /// Represents an error when no free memory slot is available when adding a new cell to the system.
        /// </summary>
        E_NO_FREE_ENTRY = -5,

        /// <summary>
        /// Represents an error occurred when a duplicate cell is found when adding a new cell to the system.
        /// </summary>
        E_DUPLICATED_CELL = -4,

        /// <summary>
        /// Represents a temporary failure of a system operation. The failed operation should be retried.
        /// </summary>
        E_RETRY = -3,

        /// <summary>
        /// Represents an error of not finding a cell.
        /// </summary>
        E_CELL_NOT_FOUND = -2,

        /// <summary>
        /// Represents a general failure.
        /// </summary>
        E_FAILURE = -1,

        /// <summary>
        /// Represents a general success status.
        /// </summary>
        E_SUCCESS = 0,

        /// <summary>
        /// Represents a status that an expected cell is found.
        /// </summary>
        E_CELL_FOUND = 1,
    }
}
