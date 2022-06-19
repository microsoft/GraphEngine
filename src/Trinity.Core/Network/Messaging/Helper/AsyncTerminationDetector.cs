// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Trinity;
using System.Threading.Tasks;

namespace Trinity.Network.Messaging
{
    internal delegate void TerminationSignal();
    internal delegate void SendDetectionToken(DetectionToken token);
    internal enum ATDColor : int
    {
        White = 0,
        Black = 1
    }
    internal class AsyncTerminationDetector
    {
        public AsyncTerminationDetector()
        {
            if (Global.MyPartitionId == 0)
            {
                MyToken = new DetectionToken
                {
                    Color = (int)ATDColor.White,
                    Counter = 0//for the ignition message
                };
                HoldingToken = 1;
            }
            else
            {
                HoldingToken = 0;
            }
            //Task.Factory.StartNew(() =>
            //    {
            //        while (true)
            //        {
            //            Thread.Sleep(1000);
            //            Console.WriteLine("Current pending job count = {0}", PendingMessageCount);
            //        }
            //    });
        }
        public SendDetectionToken SendDetectionTokenCallBack = null;
        public event TerminationSignal Terminated = null;
        /// <summary>
        /// Call before sending, otherwise the algorithm fails.
        /// </summary>
        public void SendingMessage()
        {
            Interlocked.Increment(ref LocalCounter);
        }
        /// <summary>
        /// Call before acknowledge the sender, otherwise the algorithm fails.
        /// </summary>
        public void ReceivingMessage()
        {
            lock (TokenLocker)
            {
                Interlocked.Increment(ref PendingMessageCount);
                Interlocked.Decrement(ref LocalCounter);
                Thread.VolatileWrite(ref LocalColor, (int)ATDColor.Black);
            }
        }
        public void ReceivingMessage(int count)
        {
            lock (TokenLocker)
            {
                Interlocked.Add(ref PendingMessageCount, count);
                Interlocked.Add(ref LocalCounter, -count);
                Thread.VolatileWrite(ref LocalColor, (int)ATDColor.Black);
            }
        }
        public void MessageProcessComplete()
        {
            lock (TokenLocker)
                if (0 == Interlocked.Decrement(ref PendingMessageCount))
                {
                    ProcessToken();
                }
        }
        public void MessageProcessComplete(int count)
        {
            lock (TokenLocker)
            {
                if (0 == Interlocked.Add(ref PendingMessageCount, -count))
                {
                    ProcessToken();
                }
            }
        }
        /// <summary>
        /// Caller should guarantee that current machine is holding the token, and guarantee lock.
        /// </summary>
        private void ProcessToken()
        {
            if (HoldingToken == 0)
                return;
            Console.WriteLine("Processing token.");
            if (Global.MyPartitionId == 0)
            {
                Console.WriteLine("0'storage local counter = {0},Token counter = {1}", LocalCounter, MyToken.Counter);
                if (LocalCounter + MyToken.Counter == 0 &&
                    MyToken.Color == (int)ATDColor.White &&
                    LocalColor == (int)ATDColor.White)
                {
                    Console.WriteLine("Terminating...");
                    if(Terminated != null)
                        Terminated();
                    return;
                }
                else
                {
                    MyToken.Counter = 0;
                    MyToken.Color = (int)ATDColor.White;
                    LocalColor = (int)ATDColor.White;
                }
            }
            else
            {
                MyToken.Counter += LocalCounter;
                if (LocalColor == (int)ATDColor.Black)
                {
                    MyToken.Color = (int)ATDColor.Black;
                }
                LocalColor = (int)ATDColor.White;
            }
            Thread.VolatileWrite(ref HoldingToken , 0);
            if(SendDetectionTokenCallBack != null)
                SendDetectionTokenCallBack(MyToken);
        }
        public void ReceiveToken(DetectionToken token)
        {
            Console.WriteLine("Receiving token: Color={0},Counter={1}", token.Color, token.Counter);
            lock (TokenLocker)
            {
                Console.WriteLine("Lock acquired.");
                Thread.VolatileWrite(ref HoldingToken, 1);
                MyToken = token;
                if (Thread.VolatileRead(ref PendingMessageCount) == 0)
                {
                    Task.Factory.StartNew(() =>
                    ProcessToken());
                }
                else
                {
                    Console.WriteLine("Still busy, pending message count = {0}", PendingMessageCount);
                }
            }
        }
        object TokenLocker = new object();
        int PendingMessageCount = 0;
        public int LocalCounter = 0;
        int LocalColor = (int)ATDColor.White;
        int HoldingToken = 0;
        DetectionToken MyToken;

        public void Destroy()
        {
            Terminated = null;
            SendDetectionTokenCallBack = null;
        }
    }

    internal struct DetectionToken
    {
        public int Counter ;
        public int Color ;
    }
}
