// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.IO;

using Trinity;
using Trinity.Core.Lib;
using Trinity.Utilities;
using Trinity.Network.Messaging;
using System.Reflection;
using Trinity.Storage;
using Trinity.Diagnostics;
using Trinity.Network.Sockets;
using Trinity.FaultTolerance;

namespace Trinity.Network.Messaging
{
    internal static class DefaultSyncReqRspHandlerSet
    {
        private static List<TypeSyncRequestResponseHandlerTuple> tupleList;
        private static bool initialized = false;

        internal static unsafe List<TypeSyncRequestResponseHandlerTuple> MessageHandlerList
        {
            get
            {
                if (initialized)
                {
                    return tupleList;
                }

                tupleList = new List<TypeSyncRequestResponseHandlerTuple>();

                #region LoadCell
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.LoadCell,
                    Handler = delegate(SynReqRspArgs args)
                    {
                        int index, cellSize;
                        byte* cellPtr = null;
                        TrinityErrorCode eResult;
                        long cellId = *(long*)(args.Buffer + args.Offset);
                        if ((eResult = CLocalMemoryStorage.CGetLockedCellInfo4LoadCell(cellId, out cellSize, out cellPtr, out index)) == TrinityErrorCode.E_CELL_NOT_FOUND)
                        {
                            args.Response = new TrinityMessage(eResult);
                        }
                        else
                        {
                            args.Response = new TrinityMessage(eResult, cellSize);
                            Memory.memcpy(args.Response.Buffer + TrinityMessage.Offset, cellPtr, (ulong)cellSize);
                        }
                        CLocalMemoryStorage.CReleaseCellLock(cellId, index);
                    }
                });
                #endregion

                #region LoadCellWithType
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.LoadCellWithType,
                    Handler = delegate(SynReqRspArgs args)
                    {
                        int index, cellSize;
                        byte* cellPtr = null;
                        TrinityErrorCode eResult;
                        long cellId = *(long*)(args.Buffer + args.Offset);
                        ushort cellType;
                        if ((eResult = CLocalMemoryStorage.CGetLockedCellInfo4CellAccessor(cellId, out cellSize, out cellType, out cellPtr, out index)) == TrinityErrorCode.E_CELL_NOT_FOUND)
                        {
                            args.Response = new TrinityMessage(eResult, sizeof(ushort));
                        }
                        else
                        {
                            args.Response = new TrinityMessage(eResult, cellSize + sizeof(ushort));
                            Memory.memcpy(args.Response.Buffer + TrinityMessage.Offset, cellPtr, (ulong)cellSize);
                            *(ushort*)(args.Response.Buffer + TrinityMessage.Offset + cellSize) = cellType;
                        }
                        CLocalMemoryStorage.CReleaseCellLock(cellId, index);
                    }
                });
                #endregion

                #region RemoveCell
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.RemoveCell,
                    Handler = delegate(SynReqRspArgs args)
                    {
                        var eResult = Global.LocalStorage.RemoveCell(*(long*)(args.Buffer + args.Offset));
                        args.Response = new TrinityMessage(eResult);
                    }
                });
                #endregion

                #region SaveCell
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.SaveCell,
                    Handler = delegate(SynReqRspArgs args)
                    {
                        //cell_type(2) + cell_id(8) +cell_size(4) + cell_bytes (8 + 4 +cell_bytes)
                        var eResult = Global.LocalStorage.SaveCell(*(long*)(args.Buffer + args.Offset + 2),
                            args.Buffer + args.Offset + 14,
                            *(int*)(args.Buffer + args.Offset + 10),
                            *(UInt16*)(args.Buffer + args.Offset));
                        args.Response = new TrinityMessage(eResult);
                    }
                });
                #endregion

                #region AddCell
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.AddCell,
                    Handler = delegate(SynReqRspArgs args)
                    {
                        //cell_type(2) + cell_id(8) +cell_size(4) + cell_bytes (8 + 4 +cell_bytes)
                        var eResult = Global.LocalStorage.AddCell(*(long*)(args.Buffer + args.Offset + 2),
                            args.Buffer + args.Offset + 14,
                            *(int*)(args.Buffer + args.Offset + 10),
                            *(UInt16*)(args.Buffer + args.Offset));
                        args.Response = new TrinityMessage(eResult);
                    }
                });
                #endregion

                #region UpdateCell
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.UpdateCell,
                    Handler = delegate(SynReqRspArgs args)
                    {
                        //cell_id(8) +cell_size(4) + cell_bytes (8 + 4 +cell_bytes)
                        var eResult = Global.LocalStorage.UpdateCell(*(long*)(args.Buffer + args.Offset),
                            args.Buffer + args.Offset + 12,
                            *(int*)(args.Buffer + args.Offset + 8));

                        args.Response = new TrinityMessage(eResult);
                    }
                });
                #endregion

                #region Sampling
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.Sampling,
                    Handler = delegate(SynReqRspArgs args)
                    {
                        int count;
                        {
                            count = *((int*)(args.Buffer + args.Offset));
                        }

                        long[] id_array = new long[count + 256];
                        int k = 0;

                        int count_per_trunk = (count >> 8) + 1;

                        Random r = new Random();
                        //for (int i = 0; i < Global.LocalStorage.memory_trunks.Length; i++)
                        //{
                        //    Global.LocalStorage.memory_trunks[i].hashtable.Sampling(count_per_trunk, ref id_array, ref k, r);
                        //}

                        for (int i = k; i < count; ++i)
                            id_array[i] = -1;

                        args.Response = new TrinityMessage(sizeof(int) + sizeof(int) + (count << 3));// msg_length + cell_id_count + ids

                        {
                            byte* p = args.Response.Buffer;
                            *(int*)p = sizeof(int) + (count << 3);
                            p += sizeof(int);

                            *(int*)p = count;
                            p += sizeof(int);

                            for (int i = 0; i < count; i++)
                            {
                                *(long*)p = id_array[i];
                                p += sizeof(long);
                            }
                        }
                    }
                });
                #endregion

                #region EchoPing
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.EchoPing,
                    Handler = delegate(SynReqRspArgs args)
                    {
                        args.Response = new TrinityMessage(args.Buffer, args.Offset, args.Size);
                    }
                });
                #endregion

                #region FailureNotification
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.FailureNotification,
                    Handler = delegate(SynReqRspArgs args)
                    {
                        DefaultFailureHandler.FailureNotificationMessageHandler(ref args);
                    }
                });
                #endregion

                #region GetCommunicationSchema
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.GetCommunicationSchema,
                    Handler = delegate(SynReqRspArgs args)
                    {
                        var schema        = Global.CommunicationSchema;
                        var name_str      = schema.Name;
                        var protocols_str = CommunicationSchemaSerializer.SerializeProtocols(schema);
                        int msg_len       = (name_str.Length + protocols_str.Length) * sizeof(char) + 2 * sizeof(int);
                        args.Response     = new TrinityMessage(TrinityErrorCode.E_SUCCESS, msg_len);

                        PointerHelper sp   = PointerHelper.New(args.Response.Buffer + TrinityMessage.Offset);

                        *sp.ip++          = name_str.Length;
                        BitHelper.WriteString(name_str, sp.bp);
                        sp.cp            += name_str.Length;
                        *sp.ip++          = protocols_str.Length;
                        BitHelper.WriteString(protocols_str, sp.bp);
                    }
                });
                #endregion

                #region GetCommunicationModuleOffsets
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.GetCommunicationModuleOffsets,
                    Handler = delegate(SynReqRspArgs args)
                    {
                        TrinityErrorCode errno = TrinityErrorCode.E_FAILURE;
                        var comm_instance      = Global.CommunicationInstance;
                        int syn_req_id         = -1;
                        int syn_req_rsp_id     = -1;
                        int asyn_req_id        = -1;
                        int asyn_req_rsp_id    = -1;
                        PointerHelper req_sp    = PointerHelper.New(args.Buffer + args.Offset);
                        int moduleName_len     = *req_sp.ip++;
                        string moduleName      = BitHelper.GetString(req_sp.bp, moduleName_len * 2);

                        if(comm_instance != null)
                        {
                            var comm_module    = comm_instance._GetCommunicationModuleByName(moduleName);
                            if(comm_module != null)
                            {
                                syn_req_id     = comm_module.SynReqIdOffset;
                                syn_req_rsp_id = comm_module.SynReqRspIdOffset;
                                asyn_req_id    = comm_module.AsynReqIdOffset;
                                asyn_req_rsp_id= comm_module.AsynReqRspIdOffset;
                                errno          = TrinityErrorCode.E_SUCCESS;
                            }
                        }

                        args.Response          = new TrinityMessage(errno, sizeof(int) * 4);
                        PointerHelper rsp_sp    = PointerHelper.New(args.Response.Buffer + TrinityMessage.Offset);
                        *rsp_sp.ip++           = syn_req_id;
                        *rsp_sp.ip++           = syn_req_rsp_id;
                        *rsp_sp.ip++           = asyn_req_id;
                        *rsp_sp.ip++           = asyn_req_rsp_id;

                    }
                });
                #endregion

                #region Contains
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.Contains,
                    Handler = delegate(SynReqRspArgs args)
                    {
                        if (Global.LocalStorage.Contains(*(long*)(args.Buffer + args.Offset)))
                        {
                            args.Response = new TrinityMessage(TrinityErrorCode.E_CELL_FOUND);
                        }
                        else
                        {
                            args.Response = new TrinityMessage(TrinityErrorCode.E_CELL_NOT_FOUND);
                        }
                    }
                });
                #endregion

                #region GetCellType
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.GetCellType,
                    Handler = delegate(SynReqRspArgs args)
                    {
                        ushort cellType;
                        var eResult = Global.LocalStorage.GetCellType(*(long*)(args.Buffer + args.Offset), out cellType);
                        args.Response = new TrinityMessage(eResult, sizeof(ushort));
                        *(ushort*)(args.Response.Buffer + TrinityProtocol.MsgHeader) = cellType;
                    }
                });
                #endregion

                #region QueryMemoryWorkingSet
                tupleList.Add(new TypeSyncRequestResponseHandlerTuple
                {
                    Id = (ushort)RequestType.QueryMemoryWorkingSet,
                    Handler = (args) =>
                        {
                            args.Response = new TrinityMessage(TrinityProtocol.MsgHeader + sizeof(long));
                            *(long*)(args.Response.Buffer + TrinityProtocol.MsgHeader) = PerformanceMonitor.GetMemoryWorkingSet();
                        }
                });
                #endregion

                initialized = true;
                return tupleList;
            }
        }
    }
}