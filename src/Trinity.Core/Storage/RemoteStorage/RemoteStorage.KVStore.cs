// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Threading.Tasks;
using Trinity.Network.Messaging;
using Trinity.Core.Lib;

namespace Trinity.Storage
{
    public partial class RemoteStorage : IStorage, IDisposable
    {
        public unsafe Task<LoadCellResponse> LoadCellAsync(long cellId)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.LoadCellWithType, sizeof(long));
            *(long*)(msg.Buffer + TrinityMessage.Offset) = cellId;

            Network.Client.SynClient sc = GetClient();
            return sc.SendRecvMessageAsync(msg.Buffer, msg.Size)
                     .ContinueWith(
                        t =>
                        {
                            var result = t.Result;
                            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == result.ErrorCode);
                            PutBackClient(sc);
                            msg.Dispose();

                            TrinityResponse response = result.Response;
                            int payload_len = response.Size - response.Offset;
                            byte* payload_ptr = response.Buffer + response.Offset;
                            int cell_len = payload_len - sizeof(ushort);
                            byte[] cellBuff = new byte[cell_len];
                            var eResult = response.ErrorCode;
                            Memory.Copy(payload_ptr, 0, cellBuff, 0, cell_len);
                            ushort cellType = *(ushort*)(payload_ptr + cell_len);
                            response.Dispose();
                            return new LoadCellResponse(
                                sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE,
                                cellBuff,
                                cellType);
                        },
                        TaskContinuationOptions.ExecuteSynchronously);
        }

        public unsafe Task<LoadCellUnsafeResponse> LoadCellUnsafeAsync(long cellId)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.LoadCellWithType, sizeof(long));
            *(long*)(msg.Buffer + TrinityMessage.Offset) = cellId;

            Network.Client.SynClient sc = GetClient();

            return sc.SendRecvMessageAsync(msg.Buffer, msg.Size)
                     .ContinueWith(
                        t =>
                        {
                            var result = t.Result;
                            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == result.ErrorCode);
                            PutBackClient(sc);
                            msg.Dispose();

                            TrinityResponse response = result.Response;
                            int payload_len = response.Size - response.Offset;
                            byte* payload_ptr = response.Buffer + response.Offset;
                            int size = payload_len - sizeof(ushort);
                            byte* cellBuf = (byte*)Memory.malloc((ulong)size);
                            var eResult = response.ErrorCode;
                            Memory.Copy(payload_ptr, 0, cellBuf, 0, size);
                            ushort cellType = *(ushort*)(payload_ptr + size);
                            response.Dispose();
                            return new LoadCellUnsafeResponse(
                                sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE,
                                cellBuf,
                                size,
                                cellType);
                        },
                        TaskContinuationOptions.ExecuteSynchronously);
        }

        public unsafe Task<TrinityErrorCode> SaveCellAsync(long cell_id, byte* cellBytes, int cellSize, ushort cellType)
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

            Network.Client.SynClient sc = GetClient();
            return sc.SendRecvMessageAsync(msg.Buffer, msg.Size)
                     .ContinueWith(
                        t =>
                        {
                            var result = t.Result;
                            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == result.ErrorCode);
                            PutBackClient(sc);
                            msg.Dispose();

                            TrinityResponse response = result.Response;
                            var eResult = response.ErrorCode;
                            response.Dispose();
                            return sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE;
                        },
                        TaskContinuationOptions.ExecuteSynchronously);
        }

        public unsafe Task<TrinityErrorCode> RemoveCellAsync(long cell_id)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.RemoveCell, sizeof(long));
            *(long*)(msg.Buffer + TrinityMessage.Offset) = cell_id;

            Network.Client.SynClient sc = GetClient();
            return sc.SendRecvMessageAsync(msg.Buffer, msg.Size)
                     .ContinueWith(
                        t =>
                        {
                            var result = t.Result;
                            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == result.ErrorCode);
                            PutBackClient(sc);
                            msg.Dispose();

                            TrinityResponse response = result.Response;
                            var eResult = response.ErrorCode;
                            response.Dispose();

                            return sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE;
                        },
                        TaskContinuationOptions.ExecuteSynchronously);
        }

        public unsafe Task<bool> ContainsAsync(long cellId)
        {
            TrinityMessage request;
            request = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.Contains, sizeof(long));
            *(long*)(request.Buffer + TrinityMessage.Offset) = cellId;
            Network.Client.SynClient sc = GetClient();
            return sc.SendRecvMessageAsync(request.Buffer, request.Size)
                     .ContinueWith(
                        t =>
                        {
                            var result = t.Result;
                            PutBackClient(sc);

                            bool contains = false;
                            TrinityErrorCode eResult = result.ErrorCode;
                            TrinityResponse response = result.Response;

                            if (eResult == TrinityErrorCode.E_SUCCESS)
                            {
                                //returns E_CELL_FOUND or E_CELL_NOTFOUND
                                contains = (response.ErrorCode == TrinityErrorCode.E_CELL_FOUND);
                            }

                            request.Dispose();
                            response.Dispose();

                            _error_check(eResult);
                            return contains;
                        },
                        TaskContinuationOptions.ExecuteSynchronously);
        }

        public unsafe Task<(TrinityErrorCode ErrorCode, ushort CellType)> GetCellTypeAsync(long cellId)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.GetCellType, sizeof(long));
            *(long*)(msg.Buffer + TrinityMessage.Offset) = cellId;

            Network.Client.SynClient sc = GetClient();
            return sc.SendRecvMessageAsync(msg.Buffer, msg.Size)
                     .ContinueWith(
                        t =>
                        {
                            var result = t.Result;
                            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == result.ErrorCode);
                            PutBackClient(sc);
                            msg.Dispose();

                            TrinityResponse response = result.Response;
                            ushort cellType = *(ushort*)(response.Buffer + response.Offset);
                            var eResult = response.ErrorCode;
                            response.Dispose();

                            return (sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE, cellType);
                        },
                        TaskContinuationOptions.ExecuteSynchronously);
        }

        public unsafe Task<TrinityErrorCode> AddCellAsync(long cellId, byte* cellBytes, int cellSize, ushort cellType)
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

            Network.Client.SynClient sc = GetClient();
            return sc.SendRecvMessageAsync(msg.Buffer, msg.Size)
                     .ContinueWith(
                        t =>
                        {
                            var result = t.Result;
                            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == result.ErrorCode);
                            PutBackClient(sc);
                            msg.Dispose();

                            TrinityResponse response = result.Response;
                            var eResult = response.ErrorCode;
                            response.Dispose();
                            return sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE;
                        },
                        TaskContinuationOptions.ExecuteSynchronously);
        }

        public unsafe Task<TrinityErrorCode> UpdateCellAsync(long cellId, byte* cellPtr, int length)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.UpdateCell, length + 12/*cell_id(8) +cell_size(4)*/);
            byte* p = msg.Buffer + TrinityMessage.Offset;

            *(long*)p = cellId;
            p += 8;

            *(int*)p = length;
            p += 4;

            Memory.Copy(cellPtr, p, length);

            Network.Client.SynClient sc = GetClient();
            return sc.SendRecvMessageAsync(msg.Buffer, msg.Size)
                     .ContinueWith(
                        t =>
                        {
                            var result = t.Result;
                            bool sendSuccess = (TrinityErrorCode.E_SUCCESS == result.ErrorCode);
                            PutBackClient(sc);
                            msg.Dispose();

                            TrinityResponse response = result.Response;
                            var eResult = response.ErrorCode;
                            response.Dispose();
                            return sendSuccess ? eResult : TrinityErrorCode.E_NETWORK_SEND_FAILURE;
                        },
                        TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
