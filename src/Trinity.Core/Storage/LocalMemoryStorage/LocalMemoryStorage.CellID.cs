// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Net;

using Trinity;
using Trinity.Core.Lib;

namespace Trinity.Storage
{
    public partial class LocalMemoryStorage
    {
        private long sn = 0;
        XRandom rand = new XRandom(Environment.TickCount);

        /// <summary>
        /// Generates a new random 64-bit cell id.
        /// </summary>
        /// <returns>A new 64-bit cell id.</returns>
        /// <remarks>This is a thread-safe method that you can call to get a new cell id.</remarks>
        public long NewRandomCellID()
        {
            return rand.NextInt64();
        }

        /// <summary>
        /// Generates a sequentially incremented 64-bit cell id.
        /// </summary>
        /// <returns>A new 64-bit cell id.</returns>
        /// <remarks>This is a thread-safe method that you can call to get a new cell id.</remarks> 
        public long NextSequentialCellID()
        {
            return Interlocked.Increment(ref sn);
        }
    }
}
