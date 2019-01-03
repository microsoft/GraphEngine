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
    public unsafe partial class RemoteStorage : IStorage, IDisposable
    {
        public TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType)
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

        public TrinityErrorCode LoadCell(long cellId, out byte* cellBuf, out int size, out ushort cellType)
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
            size = payload_len - sizeof(ushort);
            cellBuf = (byte*)Memory.malloc((ulong)size);
            var eResult = response.ErrorCode;
            Memory.Copy(payload_ptr, 0, cellBuf, 0, size);
            cellType = *(ushort*)(payload_ptr + size);
            response.Dispose();
            return sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE;
        }

        public TrinityErrorCode SaveCell(long cell_id, byte* cellBytes, int cellSize, ushort cellType)
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

        public TrinityErrorCode RemoveCell(long cell_id)
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

        public bool Contains(long cellId)
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

        public TrinityErrorCode GetCellType(long cellId, out ushort cellType)
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

        public TrinityErrorCode AddCell(long cellId, byte* cellBytes, int cellSize, ushort cellType)
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

        public TrinityErrorCode UpdateCell(long cellId, byte* cellPtr, int length)
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
    }
}
