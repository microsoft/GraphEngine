// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL.Lib;

namespace Trinity.Storage
{
    /// <summary>
    /// Represents an accessor that owns a buffer.
    /// </summary>
    public interface IAccessor
    {
        /// <summary>
        /// Get the pointer to the underlying buffer.
        /// Note, the pointer always point to the beginning of the buffer,
        /// and there may be metadata instead of actual data.
        /// </summary>
        unsafe byte* GetUnderlyingBufferPointer();

        /// <summary>
        /// Get the byte array representation of the underlying buffer.
        /// </summary>
        byte[] ToByteArray();

        /// <summary>
        /// Get the length of the buffer.
        /// </summary>
        int GetBufferLength();

        /// <summary>
        /// Provides an interface for resizing a field of the accessor. Usually called by a sub-accessor.
        /// </summary>
        ResizeFunctionDelegate ResizeFunction { get; set; }
    }
}
