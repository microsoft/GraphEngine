// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using Trinity;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Core.Lib;
using System.IO;
using Trinity.Configuration;

namespace Trinity.Storage
{
    internal unsafe partial class RemoteStorage : Storage, IDisposable
    {
        public override TrinityErrorCode LoadCell(long cell_id, out byte[] cellBuff)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.LoadCell, sizeof(long));
            *(long*)(msg.Buffer + TrinityMessage.Offset) = cell_id;

            TrinityResponse response;
            Network.Client.SynClient sc = GetClient();
            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == sc.SendMessage(msg.Buffer, msg.Size, out response));
            PutBackClient(sc);
            msg.Dispose();

            cellBuff = new byte[response.Size - response.Offset];
            var eResult = response.ErrorCode;
            Memory.Copy(response.Buffer, response.Offset, cellBuff, 0, cellBuff.Length);
            response.Dispose();
            return sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE;
        }

        public override TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.LoadCellWithType, sizeof(long));
            *(long*)(msg.Buffer + TrinityMessage.Offset) = cellId;

            TrinityResponse response;
            Network.Client.SynClient sc = GetClient();
            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == sc.SendMessage(msg.Buffer, msg.Size, out response));
            PutBackClient(sc);
            msg.Dispose();

            int payload_len = response.Size - response.Offset;
            byte* payload_ptr = response.Buffer + response.Offset;
            int cell_len = payload_len - sizeof(ushort);
            cellBuff = new byte[cell_len];
            var eResult = response.ErrorCode;
            Memory.Copy(payload_ptr, 0, cellBuff, 0, cell_len);
            cellType = *(ushort*)(payload_ptr + cell_len);
            response.Dispose();
            return sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE;
        }

        public override TrinityErrorCode SaveCell(long cellId, byte[] cellBytes)
        {
            fixed (byte* p = cellBytes)
            {
                return SaveCell(cellId, p, cellBytes.Length, StorageConfig.c_UndefinedCellType);
            }
        }
        public override TrinityErrorCode SaveCell(long cellId, byte[] cellBytes, int startIndex, int cellSize)
        {
            fixed (byte* p = cellBytes)
            {
                return SaveCell(cellId, p + startIndex, cellSize, StorageConfig.c_UndefinedCellType);
            }
        }

        public override TrinityErrorCode SaveCell(long cellId, byte[] cellBytes, int startIndex, int cellSize, ushort cellType)
        {
            fixed (byte* p = cellBytes)
            {
                return SaveCell(cellId, p + startIndex, cellSize, cellType);
            }
        }

        public override TrinityErrorCode SaveCell(long cell_id, byte* cellBytes, int cellSize, ushort cellType)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.SaveCell, cellSize + 14 /*cell_type(2)+ cell_id(8) +cell_size(4)*/);
            byte* p = msg.Buffer + TrinityMessage.Offset;
            *(ushort*)p = cellType;
            p += 2;

            *(long*)p = cell_id;
            p += 8;

            *(int*)p = cellSize;
            p += 4;

            Memory.Copy(cellBytes, p, cellSize);

            TrinityResponse response;
            Network.Client.SynClient sc = GetClient();
            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == sc.SendMessage(msg.Buffer, msg.Size, out response));
            PutBackClient(sc);
            msg.Dispose();

            var eResult = response.ErrorCode;
            response.Dispose();
            return sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE;
        }

        internal TrinityErrorCode SaveCell(long cellId, byte[] cellBytes, ushort cellType)
        {
            fixed (byte* p = cellBytes)
            {
                return SaveCell(cellId, p, cellBytes.Length, cellType);
            }
        }

        public override TrinityErrorCode SaveCell(long cellId, byte* cellPtr, int length)
        {
            return SaveCell(cellId, cellPtr, length, StorageConfig.c_UndefinedCellType);
        }

        public override TrinityErrorCode RemoveCell(long cell_id)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.RemoveCell, sizeof(long));
            *(long*)(msg.Buffer + TrinityMessage.Offset) = cell_id;

            TrinityResponse response;
            Network.Client.SynClient sc = GetClient();
            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == sc.SendMessage(msg.Buffer, msg.Size, out response));
            PutBackClient(sc);
            msg.Dispose();

            var eResult = response.ErrorCode;
            response.Dispose();

            return sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE;
        }

        public override bool Contains(long cellId)
        {
            TrinityMessage request;
            TrinityResponse response;
            TrinityErrorCode eResult;
            bool contains = false;

            request = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.Contains, sizeof(long));
            *(long*)(request.Buffer + TrinityMessage.Offset) = cellId;
            Network.Client.SynClient sc = GetClient();
            eResult = sc.SendMessage(request.Buffer, request.Size, out response);
            PutBackClient(sc);

            if (eResult == TrinityErrorCode.E_SUCCESS)
            {
                //returns E_CELL_FOUND or E_CELL_NOTFOUND
                contains = (response.ErrorCode == TrinityErrorCode.E_CELL_FOUND);
            }

            request.Dispose();
            response.Dispose();

            _error_check(eResult);
            return contains;
        }

        public override TrinityErrorCode GetCellType(long cellId, out ushort cellType)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.GetCellType, sizeof(long));
            *(long*)(msg.Buffer + TrinityMessage.Offset) = cellId;

            TrinityResponse response;
            Network.Client.SynClient sc = GetClient();
            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == sc.SendMessage(msg.Buffer, msg.Size, out response));
            PutBackClient(sc);
            msg.Dispose();

            cellType = *(ushort*)(response.Buffer + response.Offset);
            var eResult = response.ErrorCode;
            response.Dispose();

            return sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE;
        }

        public override TrinityErrorCode AddCell(long cellId, byte[] cellBytes)
        {
            fixed (byte* p = cellBytes)
            {
                return AddCell(cellId, p, cellBytes.Length, StorageConfig.c_UndefinedCellType);
            }
        }

        public override TrinityErrorCode AddCell(long cellId, byte[] cellBytes, int startIndex, int length)
        {
            fixed (byte* p = &cellBytes[startIndex])
            {
                return AddCell(cellId, p, length, StorageConfig.c_UndefinedCellType);
            }
        }

        public override TrinityErrorCode AddCell(long cellId, byte[] cellBytes, int startIndex, int length, ushort cellType)
        {
            fixed (byte* p = &cellBytes[startIndex])
            {
                return AddCell(cellId, p, length, cellType);
            }
        }

        public override TrinityErrorCode AddCell(long cellId, byte* cellBytes, int cellSize)
        {
            return AddCell(cellId, cellBytes, cellSize, StorageConfig.c_UndefinedCellType);
        }

        public override TrinityErrorCode AddCell(long cellId, byte* cellBytes, int cellSize, ushort cellType)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.AddCell, cellSize + 14/*cell_type(2)+ cell_id(8) +cell_size(4)*/);
            byte* p = msg.Buffer + TrinityMessage.Offset;

            *(ushort*)p = cellType;
            p += 2;

            *(long*)p = cellId;
            p += 8;

            *(int*)p = cellSize;
            p += 4;

            Memory.Copy(cellBytes, p, cellSize);

            TrinityResponse response;
            Network.Client.SynClient sc = GetClient();
            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == sc.SendMessage(msg.Buffer, msg.Size, out response));
            PutBackClient(sc);
            msg.Dispose();

            var eResult = response.ErrorCode;
            response.Dispose();
            return sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE;
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte* cellPtr, int length)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.UpdateCell, length + 12/*cell_id(8) +cell_size(4)*/);
            byte* p = msg.Buffer + TrinityMessage.Offset;

            *(long*)p = cellId;
            p += 8;

            *(int*)p = length;
            p += 4;

            Memory.Copy(cellPtr, p, length);

            TrinityResponse response;
            Network.Client.SynClient sc = GetClient();
            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == sc.SendMessage(msg.Buffer, msg.Size, out response));
            PutBackClient(sc);
            msg.Dispose();

            var eResult = response.ErrorCode;
            response.Dispose();
            return sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE;
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte[] cellBytes)
        {
            fixed (byte* p = cellBytes)
            {
                return UpdateCell(cellId, p, cellBytes.Length);
            }
        }

        public override TrinityErrorCode UpdateCell(long cellId, byte[] cellBytes, int startIndex, int length)
        {
            fixed (byte* p = cellBytes)
            {
                return UpdateCell(cellId, p + startIndex, length);
            }
        }
    }
}
