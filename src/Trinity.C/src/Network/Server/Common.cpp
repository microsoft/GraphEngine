// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <Network/Server/TrinityServer.h>
#include <stdio.h>
#include <string.h>

namespace Trinity
{
    namespace Network
    {
        bool StartWorkerThreadPool()
        {
            int thread_count = std::thread::hardware_concurrency();
            
            for (int i = 0; i < thread_count; i++)
            {
                std::thread t = std::thread(WorkerThreadProc, i);
                t.detach();
            }

            return true;
        }

        int TrinityServerTestEntry()
        {
            int sock_fd = StartSocketServer(5304);
            if (-1 == sock_fd)
            {
                fwprintf(stderr, L"cannot start socket server\n");
                return -1;
            }
            StartWorkerThreadPool();
            while (true)
                std::this_thread::sleep_for(std::chrono::seconds(5));
            return 0;
        }

        // This is only a sample handler
        void MessageHandler(MessageBuff * msg)
        {
            fwprintf(stderr, L"in message handler, message length: %d \n", msg->BytesReceived);
            // TODO: do some user-specified logic
            msg->Buffer = (char*)malloc(11);
            memset(msg->Buffer, 0, 11);
            *(uint32_t*)(msg->Buffer) = 7;
            memcpy(msg->Buffer + 4, "hello\n", 6);
            msg->BytesToSend = 11;
        }
    }
}
