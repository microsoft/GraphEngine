// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <stdint.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <wchar.h>
#include <thread>
#include <TrinityCommon.h>

namespace Trinity
{
    namespace Network
    {
        uint64_t CreateClientSocket();
        bool ClientSocketConnect(uint64_t clientsocket, uint32_t ip, uint16_t port);
        //uint64_t CreateClientSocketAndConnect(uint32_t ip, uint16_t port);
        bool ClientSend(uint64_t socket, char* buf, int32_t len);
        TrinityErrorCode ClientReceive(uint64_t socket, char* & buf, int32_t & len);
        TrinityErrorCode WaitForAckPackage(uint64_t socket);
        void CloseClientSocket(uint64_t socket);
        uint32_t IP2UInt32(const char* ip);

        inline int SocketClientTestEntry()
        {
            uint32_t ip = IP2UInt32("127.0.0.1");
            int sfd = (int)CreateClientSocket();
            if (-1 == sfd)
                fwprintf(stderr, L"connected failed, fd: %d\n", sfd);
            if (!ClientSocketConnect(sfd, ip, 5304))
                fwprintf(stderr, L"connected failed, fd: %d\n", sfd);

            int count = 0;
            while (true)
            {
                fwprintf(stdout, L"%d: sending a message\n", count++);
                char message[32];
                memset(message, 0, 32);
                *(uint32_t*)message = 7;
                memcpy(message + 4, "hello\n", 6);

                if (!ClientSend(sfd, message, 11))
                {
                    fwprintf(stderr, L"failed write\n");
                    return -1;
                }

                char* buf;
                int len;
                if (!ClientReceive(sfd, buf, len))
                {
                    fwprintf(stderr, L"read failed\n");
                }

                fwprintf(stderr, L"Received %d bytes: ", len);
                for (int i = 0; i < len; i++)
                    fwprintf(stderr, L"%c", buf[i]);
                fwprintf(stderr, L"\n");
                free(buf);

                std::this_thread::sleep_for(std::chrono::seconds(5));
            }
            return 0;
        }
    }
}