using System;
using System.Collections.Generic;
using System.Linq;
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

        public unsafe TrinityErrorCode AddCell(long cellId, byte* buff, int size, ushort cellType)
        {
            //header: cellId, size, type
            int    header_len = TrinityProtocol.MsgHeader + sizeof(long) + sizeof(int) + sizeof(ushort);
            byte*  header = stackalloc byte[header_len];
            byte** holder = stackalloc byte*[2];
            int*   length = stackalloc int[2];
            PointerHelper sp = PointerHelper.New(header);
            *sp.ip++ = header_len + size;
            *sp.sp++ = (short)TrinityMessageType.SYNC_WITH_RSP;
            *sp.sp++ = (short)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.AddCell;
            *sp.lp++ = cellId;
            *sp.ip++ = size;
            *sp.sp++ = (short)cellType;

            holder[0] = header;
            holder[1] = buff;

            TrinityResponse rsp = null;
            try
            {
                m_mod.SendMessage(m_ep, holder, length, 2, out rsp);
                return *(TrinityErrorCode*)rsp.Buffer;
            }
            catch
            {
                return TrinityErrorCode.E_RPC_EXCEPTION;
            }
            finally
            {
                rsp?.Dispose();
            }
        }

        public bool Contains(long cellId)
        {
            using (var req = new __CellIdStructWriter(cellId))
            using (var rsp = m_mod.AddCell(0, req))
            {
                return rsp.code == (int)TrinityErrorCode.E_CELL_FOUND;
            }
        }

        public TrinityErrorCode GetCellType(long cellId, out ushort cellType)
        {
            using (var req = new __CellIdStructWriter(cellId))
            using (var rsp = m_mod.AddCell(0, req))
            {
                if (rsp.code >= 0)
                {
                    cellType = (ushort)rsp.code;
                    return TrinityErrorCode.E_SUCCESS;
                }
                else
                {
                    cellType = 0;
                    return (TrinityErrorCode)rsp.code;
                }
            }
        }

        public TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType)
        {
            throw new NotImplementedException();
        }

        public TrinityErrorCode RemoveCell(long cellId)
        {
            throw new NotImplementedException();
        }

        public unsafe TrinityErrorCode SaveCell(long cellId, byte* buff, int size, ushort cellType)
        {
            throw new NotImplementedException();
        }

        public unsafe TrinityErrorCode UpdateCell(long cellId, byte* buff, int size)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            // note, do not dispose shared comm. endpoint and comm. instance.
        }

        public T GetCommunicationModule<T>() where T : CommunicationModule
            => m_comminst.GetCommunicationModule<T>();

        public unsafe void SendMessage(byte* message, int size)
        {
            byte* header                                    = stackalloc byte[TrinityProtocol.MsgHeader + sizeof(int)];
            byte** bufs                                     = stackalloc byte*[2];
            int* sizes                                      = stackalloc int[2];

            bufs[0]                                         = header;
            bufs[1]                                         = message;
            sizes[0]                                        = TrinityProtocol.MsgHeader + sizeof(int);
            sizes[1]                                        = size;

            PointerHelper sp                                = PointerHelper.New(header);
            *sp.ip                                          = size + sizeof(int) + TrinityProtocol.TrinityMsgHeader;
            *(TrinityMessageType*)(sp.bp + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.SYNC;
            *(ushort*)(sp.bp + TrinityProtocol.MsgIdOffset) = (ushort)TSL.CommunicationModule.TrinityClientModule.SynReqMessageType.RedirectMessage;
            *(int*)(sp.bp + TrinityProtocol.MsgHeader)      = partitionId;

            m_mod.SendMessage(m_ep, bufs, sizes, 2);
        }

        public unsafe void SendMessage(byte* message, int size, out TrinityResponse response)
        {
            byte* header                                    = stackalloc byte[TrinityProtocol.MsgHeader + sizeof(int)];
            byte** bufs                                     = stackalloc byte*[2];
            int* sizes                                      = stackalloc int[2];

            bufs[0]                                         = header;
            bufs[1]                                         = message;
            sizes[0]                                        = TrinityProtocol.MsgHeader + sizeof(int);
            sizes[1]                                        = size;

            PointerHelper sp                                = PointerHelper.New(header);
            *sp.ip                                          = size + sizeof(int) + TrinityProtocol.TrinityMsgHeader;
            *(TrinityMessageType*)(sp.bp + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.SYNC_WITH_RSP;
            *(ushort*)(sp.bp + TrinityProtocol.MsgIdOffset) = (ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.RedirectMessageWithResponse;
            *(int*)(sp.bp + TrinityProtocol.MsgHeader)      = partitionId;

            m_mod.SendMessage(m_ep, bufs, sizes, 2, out response);
        }

        public unsafe void SendMessage(byte** _bufs, int* _sizes, int count)
        {
            byte* header                                    = stackalloc byte[TrinityProtocol.MsgHeader + sizeof(int)];
            ulong  bufs_len                                 = (ulong)(sizeof(byte*) * (count + 1));
            ulong  sizes_len                                = (ulong)(sizeof(int) * (count + 1));
            byte** bufs                                     = (byte**)Memory.malloc(bufs_len);
            int*   sizes                                    = (int*)Memory.malloc(sizes_len);

            bufs[0]                                         = header;
            sizes[0]                                        = TrinityProtocol.MsgHeader + sizeof(int);
            Memory.memcpy(bufs + 1, _bufs, bufs_len);
            Memory.memcpy(sizes + 1, _sizes, sizes_len);

            PointerHelper sp                                = PointerHelper.New(header);
            *sp.ip                                          = sizeof(int) + TrinityProtocol.TrinityMsgHeader + Utils._sum(_sizes, count);
            *(TrinityMessageType*)(sp.bp + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.SYNC;
            *(ushort*)(sp.bp + TrinityProtocol.MsgIdOffset) = (ushort)TSL.CommunicationModule.TrinityClientModule.SynReqMessageType.RedirectMessage;
            *(int*)(sp.bp + TrinityProtocol.MsgHeader)      = partitionId;

            m_mod.SendMessage(m_ep, bufs, sizes, count + 1);
        }

        public unsafe void SendMessage(byte** _bufs, int* _sizes, int count, out TrinityResponse response)
        {
            byte* header                                    = stackalloc byte[TrinityProtocol.MsgHeader + sizeof(int)];
            ulong  bufs_len                                 = (ulong)(sizeof(byte*) * (count + 1));
            ulong  sizes_len                                = (ulong)(sizeof(int) * (count + 1));
            byte** bufs                                     = (byte**)Memory.malloc(bufs_len);
            int*   sizes                                    = (int*)Memory.malloc(sizes_len);

            bufs[0]                                         = header;
            sizes[0]                                        = TrinityProtocol.MsgHeader + sizeof(int);
            Memory.memcpy(bufs + 1, _bufs, bufs_len);
            Memory.memcpy(sizes + 1, _sizes, sizes_len);

            PointerHelper sp                                = PointerHelper.New(header);
            *sp.ip                                          = sizeof(int) + TrinityProtocol.TrinityMsgHeader + Utils._sum(_sizes, count);
            *(TrinityMessageType*)(sp.bp + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.SYNC_WITH_RSP;
            *(ushort*)(sp.bp + TrinityProtocol.MsgIdOffset) = (ushort)TSL.CommunicationModule.TrinityClientModule.SynReqRspMessageType.RedirectMessageWithResponse;
            *(int*)(sp.bp + TrinityProtocol.MsgHeader)      = partitionId;

            m_mod.SendMessage(m_ep, bufs, sizes, count + 1, out response);
        }
    }
}
