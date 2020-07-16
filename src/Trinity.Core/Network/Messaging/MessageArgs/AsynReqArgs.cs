// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System.Threading;
using System.Threading.Tasks;
using Trinity.Core.Lib;

namespace Trinity.Network.Messaging
{
    /// <summary>
    /// Represents a network request object for an asynchronous protocol without response.
    /// </summary>
    public unsafe class AsynReqArgs
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

        internal AsyncReqHandler RequestHandler;

        internal AsynReqArgs(byte* message, int offset, int size, AsyncReqHandler handler)
        {
            Buffer = (byte*)Memory.malloc((ulong)size);
            Memory.memcpy(Buffer, message + offset, (ulong)size);
            Offset = 0;

            Size = size;
            RequestHandler = handler;
        }

        // Used by local storage multi-body message. Do not allocate but takes the buffer directly.
        internal AsynReqArgs(AsyncReqHandler handler, byte* buf, int offset, int len)
        {
            RequestHandler = handler;
            Offset         = offset;
            Size           = len;
            Buffer         = buf;
        }

        internal TrinityErrorCode AsyncProcessMessage()
        {
            Task fireAndForget = MessageProcessProcAsync();
            //  TODO in the future we might have additional steps for queueing the async msg
            //  (serialize to disk when the queue is too long, for example); that will require
            //  us to leverage TrinityErrorCode to indicate whether the async message is queued
            //  successfully or not.
            return TrinityErrorCode.E_SUCCESS;
        }

        private Task MessageProcessProcAsync()
        {
            return RequestHandler(this).ContinueWith(
                t =>
                {
                    try
                    {
                        if (t.Status != TaskStatus.RanToCompletion)
                        {
                            CommunicationInstance._RaiseUnhandledExceptionEvents(this, new MessagingUnhandledExceptionEventArgs(this, t.Exception));
                        }
                    }
                    finally
                    {
                        Memory.free(Buffer);
                    }
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
