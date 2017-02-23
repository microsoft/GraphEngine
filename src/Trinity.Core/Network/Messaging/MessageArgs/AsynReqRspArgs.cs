// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Diagnostics;

namespace Trinity.Network.Messaging
{
    /// <summary>
    /// Represents a network request object for a synchronous protocol with response.
    /// </summary>
    public unsafe class AsynReqRspArgs
    {
        /// <summary>
        /// The pointer pointing to the underlying buffer.
        /// </summary>
        public byte* Buffer;
        /// <summary>
        /// Offset of the message body.
        /// </summary>
        public int Offset;
        /// <summary>
        /// Message body size.
        /// </summary>
        public int Size;

        /// <summary>
        /// The message handler for the synchronous protocol with response.
        /// </summary>
        internal AsyncReqRspHandler RequestHandler;

        internal AsynReqRspArgs(byte* message, int offset, int size, AsyncReqRspHandler handler)
        {
            Buffer = (byte*)Memory.malloc((ulong)size);
            Memory.memcpy(Buffer, message + offset, (ulong)size);
            Offset = 0;
            Size = size;
            RequestHandler = handler;
        }

        // Used by local storage multi-body message. Do not allocate but takes the buffer directly.
        internal AsynReqRspArgs(AsyncReqRspHandler handler, byte* buf, int offset, int len)
        {
            RequestHandler = handler;
            Offset         = offset;
            Size           = len;
            Buffer         = buf;
        }

        internal TrinityErrorCode AsyncProcessMessage()
        {
            //  TODO based on the perf. stats of the handler, we give hints to the scheduler.
            //  For some tasks, we should favor FAIRNESS (especially those with a deadline, or timeout).
            //  For some tasks, we should favor THROUGHPUT
            //  For some tasks, we may even favor REALTIMENESS, where several tasks can be chained in a
            //  continuation-passing-style(CPS) manner.
            Task.Factory.StartNew(MessageProcessProc, CancellationToken.None, TaskCreationOptions.PreferFairness, TaskScheduler.Current);
            //  TODO in the future we might have additional steps for queueing the async msg
            //  (serialize to disk when the queue is too long, for example); that will require
            //  us to leverage TrinityErrorCode to indicate whether the async message is queued
            //  successfully or not.
            return TrinityErrorCode.E_SUCCESS;
        }


        private void MessageProcessProc()
        {
            try
            {
                RequestHandler(this);
            }
            catch (Exception e)
            {
                CommunicationInstance._RaiseUnhandledExceptionEvents(this, new MessagingUnhandledExceptionEventArgs(this, e));
            }
            finally
            {
                Memory.free(Buffer);
            }
        }
    }
}
