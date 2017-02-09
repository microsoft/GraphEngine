// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Trinity.Network.Sockets;

namespace Trinity.Network.Messaging
{
    /// <summary>
    /// Represents a registry that maintains the mapping from message types to their message handlers.
    /// </summary>
    public static class MessageRegistry
    {
        /// <summary>
        /// Registers a synchronous message handler.
        /// </summary>
        /// <param name="msgId">A 16-bit unsigned message id.</param>
        /// <param name="message_handler">A message hander for a synchronous protocol with response.</param>
        public static void RegisterMessageHandler(ushort msgId, SynReqRspHandler message_handler)
        {
            MessageHandlers.DefaultParser.RegisterMessageHandler(msgId, message_handler);
        }
        /// <summary>
        /// Registers a preserved synchronous message handler.
        /// </summary>
        /// <param name="msgId">A 16-bit unsigned message id.</param>
        /// <param name="message_handler">A message hander for a synchronous protocol with response.</param>
        public static void RegisterPreservedMessageHandler(ushort msgId, SynReqRspHandler message_handler)
        {
            MessageHandlers.DefaultParser.RegisterPreservedMessageHandler(msgId, message_handler);
        }

        /// <summary>
        /// Registers a synchronous message handler.
        /// </summary>
        /// <param name="msgId">A 16-bit unsigned message id.</param>
        /// <param name="message_handler">A message hander for a synchronous protocol without response.</param>
        public static void RegisterMessageHandler(ushort msgId, SynReqHandler message_handler)
        {
            MessageHandlers.DefaultParser.RegisterMessageHandler(msgId, message_handler);
        }

        /// <summary>
        /// Registers a preserved synchronous message handler.
        /// </summary>
        /// <param name="msgId">A 16-bit unsigned message id.</param>
        /// <param name="message_handler">A message hander for a synchronous protocol without response.</param>
        public static void RegisterPreservedMessageHandler(ushort msgId, SynReqHandler message_handler)
        {
            MessageHandlers.DefaultParser.RegisterPreservedMessageHandler(msgId, message_handler);
        }

        /// <summary>
        /// Registers an asynchronous message handler.
        /// </summary>
        /// <param name="msgId">A 16-bit unsigned message id.</param>
        /// <param name="message_handler">A message hander for an asynchronous protocol without response.</param>
        public static void RegisterMessageHandler(ushort msgId, AsyncReqHandler message_handler)
        {
            MessageHandlers.DefaultParser.RegisterMessageHandler(msgId, message_handler);
        }

        /// <summary>
        /// Registers a preserved asynchronous message handler.
        /// </summary>
        /// <param name="msgId">A 16-bit unsigned message id.</param>
        /// <param name="message_handler">A message hander for an asynchronous protocol without response.</param>
        public static void RegisterPreservedMessageHandler(ushort msgId, AsyncReqHandler message_handler)
        {
            MessageHandlers.DefaultParser.RegisterPreservedMessageHandler(msgId, message_handler);
        }
    }
}
