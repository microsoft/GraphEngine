// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#if defined(SOCKET_SERVER_TEST)
#include "TrinityServer.h"

int TrinityServerTestEntry()
{
    int sock_fd = StartSocketServer(5304);
    if (-1 == sock_fd)
    {
        fwprintf(stderr, L">>> cannot start socket server\n");
        return -1;
    }
    StartWorkerThreadPool();
    while (true)
    {
        std::this_thread::sleep_for(std::chrono::seconds(5));
    }
    return 0;
}

int main(int argc, char *argv[])
{
    return Trinity::Network::TrinityServerTestEntry();
}
#endif