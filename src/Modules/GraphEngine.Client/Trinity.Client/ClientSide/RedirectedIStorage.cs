using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Network;
using Trinity.Network.Messaging;
using Trinity.Storage;

namespace Trinity.Client
{
    /// <summary>
    /// Redirected IStorage, each representing a remote partition.
    /// Under the hood, it communicates with the remote cloud with
    /// a shared message passing endpoint (the TrinityClient ep)
    /// and speaks wrapped messages.
    /// </summary>
    internal class RedirectedIStorage : IStorage
    {
        //  !Note, do not send module messages directly through m_ep.
        //  m_mod will properly align message id offsets.
        private IMessagePassingEndpoint m_ep;
        private CommunicationInstance m_comminst;
        private TrinityClientModule.TrinityClientModule m_mod;
        private int partitionId;

        public RedirectedIStorage(IMessagePassingEndpoint ep, TrinityClient tc, int p)
        {
            m_ep = ep;
            m_comminst = tc;
            partitionId = p;
            m_mod = GetCommunicationModule<TrinityClientModule.TrinityClientModule>();
        }

        public unsafe Task<TrinityErrorCode> AddCellAsync(long cellId, byte* buff, int size, ushort cellType)
        {
            return _SendCellPayloadAsync(cellId, buff, size, cellType, (short)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.AddCell);
        }

        public async Task<bool> ContainsAsync(long cellId)
        {
            using (var req = new __CellIdStructWriter(cellId))
            using (var rsp = await m_mod.AddCellAsync(0, req))
            {
                // remote returns E_CELL_FOUND or E_CELL_NOT_FOUND
                return rsp.code == (int)TrinityErrorCode.E_CELL_FOUND;
            }
        }

        public async Task<(TrinityErrorCode, ushort)> GetCellTypeAsync(long cellId)
        {
            using (var req = new __CellIdStructWriter(cellId))
            using (var rsp = await m_mod.GetCellTypeAsync(0, req))
            {
                if (rsp.code >= 0)
                {
                    return (TrinityErrorCode.E_SUCCESS, (ushort)rsp.code);
                }
                else
                {
                    return ((TrinityErrorCode)rsp.code, 0);
                }
            }
        }

        public unsafe Task<LoadCellResponse> LoadCellAsync(long cellId)
        {
            var req = new __CellIdStructWriter(cellId);

            var sp = PointerHelper.New(req.buffer);
            *sp.ip++ = TrinityProtocol.TrinityMsgHeader + sizeof(long);
            *sp.sp++ = (short)TrinityMessageType.SYNC_WITH_RSP;
            *sp.sp++ = (short)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.LoadCell;

            return m_mod.SendRecvMessageAsync(m_ep, req.buffer, req.BufferLength).ContinueWith(
                t =>
                {
                    TrinityResponse rsp = null;;
                    byte[] cellBuff = null;
                    ushort cellType = 0;

                    try
                    {
                        rsp = t.Result;
                        TrinityErrorCode errcode = TrinityErrorCode.E_RPC_EXCEPTION;
                        sp = PointerHelper.New(rsp.Buffer);
                        errcode = (TrinityErrorCode)(*sp.ip++);
                        if (errcode == TrinityErrorCode.E_SUCCESS)
                        {
                            var length = *sp.ip++;
                            cellType = (ushort)(*sp.sp++);
                            cellBuff = new byte[length];
                            fixed (byte* p = cellBuff)
                            {
                                Memory.memcpy(p, sp.bp, (ulong)length);
                            }
                        }
                        /* otherwise, fails with returned error code */

                        return Task.FromResult(new LoadCellResponse(errcode, cellBuff, cellType));
                    }
                    finally
                    {
                        req?.Dispose();
                        rsp?.Dispose();
                    }
                }).Unwrap();
        }

        public unsafe Task<LoadCellUnsafeResponse> LoadCellUnsafeAsync(long cellId)
        {
            var req = new __CellIdStructWriter(cellId);

            var sp = PointerHelper.New(req.buffer);
            *sp.ip++ = TrinityProtocol.TrinityMsgHeader + sizeof(long);
            *sp.sp++ = (short)TrinityMessageType.SYNC_WITH_RSP;
            *sp.sp++ = (short)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.LoadCell;

            return m_mod.SendRecvMessageAsync(m_ep, req.buffer, req.BufferLength).ContinueWith(
                t =>
                {
                    TrinityResponse rsp = null;
                    TrinityErrorCode errcode = TrinityErrorCode.E_RPC_EXCEPTION;

                    byte* cellBuff = null;
                    ushort cellType = 0;
                    int cellSize = -1;

                    try
                    {
                        rsp = t.Result;
                        sp = PointerHelper.New(rsp.Buffer);
                        errcode = (TrinityErrorCode)(*sp.ip++);
                        if (errcode == TrinityErrorCode.E_SUCCESS)
                        {
                            cellSize = *sp.ip++;
                            cellType = (ushort)(*sp.sp++);
                            cellBuff = (byte*)Memory.malloc((ulong)cellSize);
                            Memory.memcpy(cellBuff, sp.bp, (ulong)cellSize);
                        }
                        /* otherwise, fails with returned error code */

                        return Task.FromResult(new LoadCellUnsafeResponse(errcode, cellBuff, cellSize, cellType));
                    }
                    finally
                    {
                        req?.Dispose();
                        rsp?.Dispose();
                    }
                }).Unwrap();
        }

        public async Task<TrinityErrorCode> RemoveCellAsync(long cellId)
        {
            using (var req = new __CellIdStructWriter(cellId))
            using (var rsp = await m_mod.RemoveCellAsync(0, req))
            {
                return (TrinityErrorCode)rsp.code;
            }
        }

        public unsafe Task<TrinityErrorCode> SaveCellAsync(long cellId, byte* buff, int size, ushort cellType)
        {
            return _SendCellPayloadAsync(cellId, buff, size, cellType, (short)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.SaveCell);
        }

        public unsafe Task<TrinityErrorCode> UpdateCellAsync(long cellId, byte* buff, int size)
        {
            return _SendCellPayloadAsync(cellId, buff, size, null, (short)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.UpdateCell);
        }

        public void Dispose()
        {
            // note, do not dispose shared comm. endpoint and comm. instance.
        }

        public T GetCommunicationModule<T>() where T : CommunicationModule
            => m_comminst.GetCommunicationModule<T>();

        public unsafe Task SendMessageAsync(byte* message, int size)
        {
            byte* header = stackalloc byte[TrinityProtocol.MsgHeader + sizeof(int)];
            byte** bufs  = stackalloc byte*[2];
            int* sizes   = stackalloc int[2];

            bufs[0]      = header;
            bufs[1]      = message;
            sizes[0]     = TrinityProtocol.MsgHeader + sizeof(int);
            sizes[1]     = size;

            *(int*)header = TrinityProtocol.TrinityMsgHeader + sizeof(int) + size;
            *(TrinityMessageType*)(header + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.SYNC;
            *(ushort*)(header + TrinityProtocol.MsgIdOffset) = (ushort)TSL.CommunicationModule.TrinityClientModule.SynReqMessageType.RedirectMessage;
            *(int*)(header + TrinityProtocol.MsgHeader) = partitionId;

            return m_mod.SendMessageAsync(m_ep, bufs, sizes, 2);
        }

        public unsafe Task<TrinityResponse> SendRecvMessageAsync(byte* message, int size)
        {
            byte* header = stackalloc byte[TrinityProtocol.MsgHeader + sizeof(int)];
            byte** bufs  = stackalloc byte*[2];
            int* sizes   = stackalloc int[2];

            bufs[0]      = header;
            bufs[1]      = message;
            sizes[0]     = TrinityProtocol.MsgHeader + sizeof(int);
            sizes[1]     = size;

            *(int*)header = TrinityProtocol.TrinityMsgHeader + sizeof(int) + size;
            *(TrinityMessageType*)(header + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.SYNC_WITH_RSP;
            *(ushort*)(header + TrinityProtocol.MsgIdOffset) = (ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.RedirectMessageWithResponse;
            *(int*)(header + TrinityProtocol.MsgHeader) = partitionId;

            return m_mod.SendRecvMessageAsync(m_ep, bufs, sizes, 2);
        }

        public unsafe Task SendMessageAsync(byte** _bufs, int* _sizes, int count)
        {
            byte* header     = stackalloc byte[TrinityProtocol.MsgHeader + sizeof(int)];
            ulong  bufs_len  = (ulong)(sizeof(byte*) * (count + 1));
            ulong  sizes_len = (ulong)(sizeof(int) * (count + 1));
            byte** bufs      = (byte**)Memory.malloc(bufs_len);
            int*   sizes     = (int*)Memory.malloc(sizes_len);

            bufs[0]          = header;
            sizes[0]         = TrinityProtocol.MsgHeader + sizeof(int);
            Memory.memcpy(bufs + 1, _bufs, bufs_len);
            Memory.memcpy(sizes + 1, _sizes, sizes_len);

            *(int*)header = TrinityProtocol.TrinityMsgHeader + sizeof(int) + Utils._sum(_sizes, count);
            *(TrinityMessageType*)(header + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.SYNC;
            *(ushort*)(header + TrinityProtocol.MsgIdOffset) = (ushort)TSL.CommunicationModule.TrinityClientModule.SynReqMessageType.RedirectMessage;
            *(int*)(header + TrinityProtocol.MsgHeader) = partitionId;

            return m_mod.SendMessageAsync(m_ep, bufs, sizes, count + 1);
        }

        public unsafe Task<TrinityResponse> SendRecvMessageAsync(byte** _bufs, int* _sizes, int count)
        {
            byte* header     = stackalloc byte[TrinityProtocol.MsgHeader + sizeof(int)];
            ulong  bufs_len  = (ulong)(sizeof(byte*) * (count + 1));
            ulong  sizes_len = (ulong)(sizeof(int) * (count + 1));
            byte** bufs      = (byte**)Memory.malloc(bufs_len);
            int*   sizes     = (int*)Memory.malloc(sizes_len);

            bufs[0]          = header;
            sizes[0]         = TrinityProtocol.MsgHeader + sizeof(int);
            Memory.memcpy(bufs + 1, _bufs, bufs_len);
            Memory.memcpy(sizes + 1, _sizes, sizes_len);

            *(int*)header = TrinityProtocol.TrinityMsgHeader + sizeof(int) + Utils._sum(_sizes, count);
            *(TrinityMessageType*)(header + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.SYNC_WITH_RSP;
            *(ushort*)(header + TrinityProtocol.MsgIdOffset) = (ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.RedirectMessageWithResponse;
            *(int*)(header + TrinityProtocol.MsgHeader) = partitionId;

            return m_mod.SendRecvMessageAsync(m_ep, bufs, sizes, count + 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe Task<TrinityErrorCode> _SendCellPayloadAsync(long cellId, byte* buff, int size, ushort? cellType, short msgId)
        {
            //header: cellId, size, type
            int    header_len = TrinityProtocol.MsgHeader + sizeof(long) + sizeof(int) + (cellType.HasValue? sizeof(ushort): 0);
            byte*  header = stackalloc byte[header_len];
            byte** holder = stackalloc byte*[2];
            int*   length = stackalloc int[2];
            PointerHelper sp = PointerHelper.New(header);
            *sp.ip++ = header_len + size - TrinityProtocol.SocketMsgHeader;
            *sp.sp++ = (short)TrinityMessageType.SYNC_WITH_RSP;
            *sp.sp++ = msgId;
            *sp.lp++ = cellId;
            *sp.ip++ = size;

            if (cellType.HasValue)
            {
                *sp.sp++ = (short)cellType;
            }

            holder[0] = header;
            holder[1] = buff;
            length[0] = header_len;
            length[1] = size;

            return m_mod.SendRecvMessageAsync(m_ep, holder, length, 2).ContinueWith(
                t =>
                {
                    TrinityResponse rsp = null;
                    try
                    {
                        rsp = t.Result;
                        return *(TrinityErrorCode*)(rsp.Buffer);
                    }
                    finally
                    {
                        rsp?.Dispose();
                    }
                });
        }
    }
}
