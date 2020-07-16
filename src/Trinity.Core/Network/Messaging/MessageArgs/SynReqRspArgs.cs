// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System.Threading.Tasks;

namespace Trinity.Network.Messaging
{
    /// <summary>
    /// Represents a network request object for a synchronous protocol with response.
    /// </summary>
    public unsafe class SynReqRspArgs
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
        /// The response object produced by the message handler.
        /// </summary>
        public TrinityMessage Response = null;

        /// <summary>
        /// The message handler for the synchronous protocol with response.
        /// </summary>
        internal SynReqRspHandler RequestHandler;

        internal SynReqRspArgs(byte* message, int offset, int size, SynReqRspHandler handler)
        {
            Buffer = message;
            Offset = offset;
            Size = size;
            RequestHandler = handler;
        }

        /// <returns>
        /// E_SUCCESS:          The message is successfully processed. The response can be passed down to network.
        /// E_RPC_EXCEPTION:    The message handler throws an exception. The response is disposed and nulled.
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
                        if (Response != null) { Response.Dispose(); Response = null; }
                        return TrinityErrorCode.E_RPC_EXCEPTION;
                    }
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
