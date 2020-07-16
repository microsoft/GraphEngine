// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Daemon;
using Trinity.Diagnostics;
using Trinity.Network.Messaging;

namespace Trinity.Storage
{
    public partial class FixedMemoryCloud
    {
        internal unsafe Task<string> EchoPingAsync(int serverId, string msg)
        {
            TrinityMessage trinity_msg = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.EchoPing, msg.Length << 1);
            BitHelper.WriteString(msg, trinity_msg.Buffer + TrinityMessage.Offset);

            return StorageTable[serverId].SendRecvMessageAsync(trinity_msg.Buffer, trinity_msg.Size)
                                         .ContinueWith(
                                            t =>
                                            {
                                                TrinityResponse response = t.Result;
                                                return BitHelper.GetString(response.Buffer + response.Offset, response.Size);
                                            },
                                            TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Shutdown the server with the specified serverId.
        /// </summary>
        /// <param name="serverId">The id of the server.</param>
        public unsafe Task ShutDownAsync(int serverId)
        {
            byte[] message_bytes = new byte[TrinityProtocol.MsgHeader];

            GCHandle gch = GCHandle.Alloc(message_bytes, GCHandleType.Pinned);
            byte* byte_p = (byte*)gch.AddrOfPinnedObject();
            byte* p = byte_p;
            *(int*)p = TrinityProtocol.TrinityMsgHeader;

            *(TrinityMessageType*)(p + TrinityProtocol.MsgTypeOffset) = TrinityMessageType.PRESERVED_ASYNC;
            *(RequestType*)(p + TrinityProtocol.MsgIdOffset) = RequestType.Shutdown;
            return StorageTable[serverId].SendMessageAsync(byte_p, message_bytes.Length)
                                         .ContinueWith(t => gch.Free(), TaskContinuationOptions.ExecuteSynchronously);
        }

        internal unsafe Task ShutDownProxyAsync(RemoteStorage proxy)
        {
            TrinityMessage msg = new TrinityMessage(TrinityMessageType.PRESERVED_ASYNC, (ushort)RequestType.Shutdown, 0);
            return proxy.SendMessageAsync(msg.Buffer, msg.Size);
        }

        /// <summary>
        /// Shutdown all running Trinity servers. This only works in the Client mode.
        /// </summary>
        public async Task ShutDownAsync()
        {
            //TODO should be IDisposable, not ShutDown
            //TODO move this to base implementation;
            if (TrinityConfig.CurrentRunningMode == RunningMode.Client)
            {
                try
                {
                    // Trigger the initialization if not initialized yet
                    var proxies = ProxyList;
                    Task[] tasks;

                    BackgroundThread.ClearAllTasks(); // To disable heartbeat

                    tasks = Enumerable.Range(0, PartitionCount)
                                      .Select(i => ShutDownAsync(i))
                                      .ToArray();
                    await Task.WhenAll(tasks);

                    tasks = proxies.Select(proxy => ShutDownProxyAsync(proxy).ContinueWith(t => proxy.Dispose(), TaskContinuationOptions.ExecuteSynchronously))
                                   .ToArray();
                    await Task.WhenAll(tasks);

                    Dispose();
                    Thread.MemoryBarrier();
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Errors occurred during shutdown the Trinity servers.");
                    Log.WriteLine(LogLevel.Error, ex.ToString());
                }
            }
        }

        public override unsafe Task<long> GetTotalMemoryUsageAsync()
        {
            TrinityMessage tm = new TrinityMessage(TrinityMessageType.PRESERVED_SYNC_WITH_RSP, (ushort)RequestType.QueryMemoryWorkingSet, 0);
            Task<TrinityResponse>[] tasks = new Task<TrinityResponse>[PartitionCount];

            for (int sid = 0; sid < PartitionCount; sid++)
            {
                Task<TrinityResponse> task = StorageTable[sid].SendRecvMessageAsync(tm.Buffer, tm.Size);
                tasks[sid] = task;
            }

            return Task.WhenAll(tasks)
                       .ContinueWith(
                            _ =>
                            {
                                return tasks.Sum(
                                    t =>
                                    {
                                        TrinityResponse response = t.Result;
                                        return *(long*)(response.Buffer + response.Offset);
                                    });
                            },
                            TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
