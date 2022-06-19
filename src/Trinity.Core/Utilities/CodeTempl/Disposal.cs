// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Trinity.Utilities.CodeTempl
{
    internal class DisposalClass: IDisposable
    {
        private volatile bool disposed = false;
        private object disposal_lock = new object();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                lock (disposal_lock)
                {
                    if (!this.disposed)
                    {
                        if (disposing)
                        {
                            //TODO: dispose children fields, if any
                        }

                        //TODO: dispose resources held by current class
                        Thread.MemoryBarrier();
                        this.disposed = true;
                    }
                }
            }
        }
        ~DisposalClass()
        {
            Dispose(false);
        }
    }
}
