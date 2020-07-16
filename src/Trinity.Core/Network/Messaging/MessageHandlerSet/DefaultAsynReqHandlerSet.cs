// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System.Collections.Generic;
using System.Threading.Tasks;
using Trinity.Network.Sockets;

namespace Trinity.Network.Messaging
{
    static class DefaultAsynReqHandlerSet
    {
        private static List<TypeAsyncRequestHandlerTuple> tupleList;
        private static bool initialized = false;

        internal unsafe static List<TypeAsyncRequestHandlerTuple> MessageHandlerList
        {
            get
            {
                if (initialized)
                {
                    return tupleList;
                }

                tupleList = new List<TypeAsyncRequestHandlerTuple>();

                tupleList.Add(new TypeAsyncRequestHandlerTuple
                {
                    Id = (ushort)RequestType.Shutdown,
                    Handler = delegate(AsynReqArgs user_token)
                    {
                        return Task.Delay(1000).ContinueWith(t => Global.Exit(0));
                    }
                });

                initialized = true;
                return tupleList;
            }
        }
    }
}
