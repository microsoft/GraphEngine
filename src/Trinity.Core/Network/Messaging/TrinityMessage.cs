// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using Trinity;
using Trinity.Core.Lib;
using Trinity.Network;
using Trinity.Network.Messaging;

namespace Trinity.Network.Messaging
{
    /// <summary>
    /// Represents a binary network message.
    /// </summary>
    public unsafe sealed class TrinityMessage : IDisposable
    {
        static TrinityMessage()
        {
            TrinityC.Init();
        }
        /// <summary>
        /// A pointer pointing to the underlying buffer.
        /// </summary>
        public byte* Buffer;
        /// <summary>
        /// The size of the underlying buffer.
        /// </summary>
        public int Size;
        /// <summary>
        /// value = 8
        /// </summary>
        public const int Offset = TrinityProtocol.MsgHeader;

        internal TrinityMessage(TrinityMessageType msgType, ushort msgId, int size)
        {
            Size = TrinityProtocol.MsgHeader + size;
            Buffer = (byte*)CMemory.C_malloc((ulong)Size);
            *(int*)Buffer = TrinityProtocol.TrinityMsgHeader + size;
            *(Buffer + TrinityProtocol.MsgTypeOffset) = (byte)msgType;
            *(ushort*)(Buffer + TrinityProtocol.MsgIdOffset) = msgId;
        }

        /// <summary>
        /// Used for constructing a Response message.
        /// </summary>
        /// <param name="errorCode">A 32-bit Trinity error code.</param>
        /// <param name="size">The message body size to allocate.</param>
        internal TrinityMessage(TrinityErrorCode errorCode, int size)
        {
            Size = TrinityProtocol.MsgHeader + size;
            Buffer = (byte*)CMemory.C_malloc((ulong)Size);
            *(int*)Buffer = TrinityProtocol.TrinityMsgHeader + size;
            *(int*)(Buffer + TrinityProtocol.TrinityMsgHeader) = (int)errorCode;
        }

        /// <summary>
        /// Used for constructing a Response message.
        /// </summary>
        /// <param name="errorCode">A 32-bit Trinity error code.</param>
        internal TrinityMessage(TrinityErrorCode errorCode)
        {
            Size = TrinityProtocol.MsgHeader;
            Buffer = (byte*)CMemory.C_malloc((ulong)Size);
            *(int*)Buffer = TrinityProtocol.TrinityMsgHeader;
            *(int*)(Buffer + TrinityProtocol.TrinityMsgHeader) = (int)errorCode;
        }

        /// <summary>
        /// Note: msgType and msgId are left to zero
        /// </summary>
        internal unsafe TrinityMessage(byte* buffer, int offset, int size)
        {
            Size = TrinityProtocol.MsgHeader + size;
            Buffer = (byte*)CMemory.C_malloc((ulong)Size);
            *(int*)Buffer = TrinityProtocol.TrinityMsgHeader + size;
            Memory.Copy(buffer, offset, Buffer + TrinityProtocol.MsgHeader, 0, size);
        }

        /// <summary>
        /// Allocate a TrinityMessage whose buffer length is the specified size
        /// </summary>
        /// <param name="size">Message buffer length</param>
        internal unsafe TrinityMessage(int size)
        {
            Size = size;
            Buffer = (byte*)CMemory.C_malloc((ulong)size);
            *(int*)Buffer = size - TrinityProtocol.SocketMsgHeader;
        }

        /// <summary>
        /// Directly initialize a Trinity Message using "raw" message buffer
        /// </summary>
        public TrinityMessage(byte* buf, int size)
        {
            Buffer = buf;
            Size = size;
        }

        /// <summary>
        /// Releases the unmanaged memory buffer used by the TrinityMessage.
        /// </summary>
        public void Dispose()
        {
            CMemory.C_free(Buffer);
        }

        /// <summary>
        /// Determines whether this instance and a specified object, which must also be a <see cref="T:Trinity.Network.Messaging.TrinityMessage"/> object, have the same contents.
        /// </summary>
        /// <param name="obj">The object to compare to this instance.</param>
        /// <returns>true if <paramref name="obj"/> is a <see cref="T:Trinity.Network.Messaging.TrinityMessage"/> and its contents are the same as this instance; otherwise, false.</returns>
        public override bool Equals(object obj)
        {
            TrinityMessage tm = obj as TrinityMessage;
            if (tm == null)
                return false;

            return this == tm;
        }

        /// <summary>
        /// Determines whether the two specified TrinityMessages have the same contents.
        /// </summary>
        /// <param name="tm1">The first TrinityMessage to compare.</param>
        /// <param name="tm2">The second TrinityMessage to compare.</param>
        /// <returns>true if the two TrinityMessages have the same contents; otherwise, false.</returns>
        public static bool operator ==(TrinityMessage tm1, TrinityMessage tm2)
        {
            if (ReferenceEquals(tm1, tm2))
                return true;
            if (ReferenceEquals(tm1, null) || ReferenceEquals(tm2, null))
                return false;

            if (tm1.Size != tm2.Size)
            {
                return false;
            }

            try
            {
                return Memory.Compare(tm1.Buffer, tm2.Buffer, tm1.Size);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Determines whether the two specified TrinityMessages have different contents.
        /// </summary>
        /// <param name="tm1">The first TrinityMessage to compare.</param>
        /// <param name="tm2">The second TrinityMessage to compare.</param>
        /// <returns>true if the two TrinityMessages have different contents; otherwise, false.</returns>
        public static bool operator !=(TrinityMessage tm1, TrinityMessage tm2)
        {
            return !(tm1 == tm2);
        }

        /// <summary>
        /// Returns the hash code for this TrinityMessage.
        /// </summary>
        /// <returns>A 32-bit signed integer hash code.</returns>
        public override int GetHashCode()
        {
            return HashHelper.HashBytes(Buffer, Size);
        }

        internal static TrinityMessage TRUE
        {
            get
            {
                var msg = new TrinityMessage(8);
                *(int*)(msg.Buffer + 4) = 1;
                return msg;
            }
        }
        internal static TrinityMessage FALSE
        {
            get
            {
                var msg = new TrinityMessage(8);
                *(int*)(msg.Buffer + 4) = 0;
                return msg;
            }
        }
    }
}
