// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System.Threading.Tasks;

namespace Trinity.Network.Messaging
{
    /// <summary>
    /// Represents a network request object for a synchronous protocol without response.
    /// </summary>
    public unsafe class SynReqArgs
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

        internal SynReqHandler RequestHandler;

        internal SynReqArgs(byte* buffer, int offset, int size, SynReqHandler handler)
        {
            Buffer = buffer;
            Offset = offset;
            Size = size;
            RequestHandler = handler;
        }

        /// <returns>
        /// E_SUCCESS:        The message is successfully processed.
        /// E_RPC_EXCEPTION:  The message handler throws an exception.
        /// </returns>
        internal Task<TrinityErrorCode> MessageProcessAsync()
        {
            return RequestHandler(this).ContinueWith(
                t => 
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        return TrinityErrorCode.E_SUCCESS;
                    }
                    else
                    {
                        CommunicationInstance._RaiseUnhandledExceptionEvents(this, new MessagingUnhandledExceptionEventArgs(this, t.Exception));
                        return TrinityErrorCode.E_RPC_EXCEPTION;
                    }
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
