// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Network.Messaging
{
    /// <summary>
    /// Provides data for the event that is raised when there is an exception that is not handled in a message handler.
    /// </summary>
    public unsafe class MessagingUnhandledExceptionEventArgs
    {
        /// <summary>
        /// The pointer pointing to the underlying buffer of the message.
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
        /// The exception thrown by the message handler.
        /// </summary>
        public Exception ExceptionObject;

        internal unsafe MessagingUnhandledExceptionEventArgs(AsynReqArgs reqArgs, Exception exception)
        {
            this.Buffer = reqArgs.Buffer;
            this.Offset = reqArgs.Offset;
            this.Size   = reqArgs.Size;
            this.ExceptionObject = exception;
        }

        internal unsafe MessagingUnhandledExceptionEventArgs(AsynReqRspArgs reqArgs, Exception exception)
        {
            this.Buffer = reqArgs.Buffer;
            this.Offset = reqArgs.Offset;
            this.Size   = reqArgs.Size;
            this.ExceptionObject = exception;
        }

        internal unsafe MessagingUnhandledExceptionEventArgs(SynReqRspArgs reqArgs, Exception exception)
        {
            this.Buffer = reqArgs.Buffer;
            this.Offset = reqArgs.Offset;
            this.Size   = reqArgs.Size;
            this.ExceptionObject = exception;
        }

        internal unsafe MessagingUnhandledExceptionEventArgs(SynReqArgs reqArgs, Exception exception)
        {
            this.Buffer = reqArgs.Buffer;
            this.Offset = reqArgs.Offset;
            this.Size   = reqArgs.Size;
            this.ExceptionObject = exception;
        }
    }

    /// <summary>
    /// Represents the method that will handle the event raised by an exception that is not handled by a message handler.
    /// </summary>
    /// <param name="sender">The source message event argument object that caused the unhandled exception.</param>
    /// <param name="e">A MessagingUnhandledEventArgs that contains the event data.</param>
    public delegate void MessagingUnhandledExceptionEventHandler(object sender, MessagingUnhandledExceptionEventArgs e);
}
