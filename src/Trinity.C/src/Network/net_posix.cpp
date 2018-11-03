// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#if !defined(TRINITY_PLATFORM_WINDOWS)
#include "Network.h"
#include "SocketOptionsHelper.h"

#include <thread>

namespace Trinity
{
    namespace Network
    {
        std::thread* socket_accept_thread = nullptr;
        int accept_sock; // accept socket

        void SocketAcceptThreadProc()
        {
            char clientaddr[INET6_ADDRSTRLEN];
            while (true)
            {
                sockaddr addr;
                socklen_t addrlen = sizeof(sockaddr);
                int connected_sock_fd = accept4(accept_sock, &addr, &addrlen, SOCK_NONBLOCK);
                if (-1 == connected_sock_fd)
                {
                    /* Break the loop if listening socket is shut down. */
                    auto code = GetLastError();
                    if (EINVAL == code)
                    {
                        break;
                    }
                    else
                    {
                        Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Network: Cannot accept incoming connection, errno = {0}", code);
                        continue;
                    }
                }

                if (NULL == inet_ntop(addr.sa_family, addr.sa_data, clientaddr, INET6_ADDRSTRLEN))
                {
                    strcpy(clientaddr, "Unknown address");
                }
                Diagnostics::WriteLine(Diagnostics::LogLevel::Debug, "Network: Incomming connection from {0}", String(clientaddr));

                sock_t * p = alloc_incoming_socket(connected_sock_fd);

                if (TrinityErrorCode::E_SUCCESS != Events::enter_eventloop(p))
                {
                    close_incoming_conn(p, false);
                    continue;
                }

                // post the first async receive
                recv_async(p);
            }
        }

        int start_socket_server(uint16_t port)
        {
            accept_sock = -1;
            struct addrinfo hints, *addrinfos, *addrinfop;
            memset(&hints, 0, sizeof(struct addrinfo));
            hints.ai_family   = AF_INET;
            hints.ai_socktype = SOCK_STREAM;
            hints.ai_flags    = AI_PASSIVE; // wildcard addresses
            char port_buf[6];
            sprintf(port_buf, "%u", port);
            int error_code = getaddrinfo(NULL, port_buf, &hints, &addrinfos);
            if (error_code != 0)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot obtain local address info.");
                return -1;
            }

            for (addrinfop = addrinfos; addrinfop != NULL; addrinfop = addrinfop->ai_next)
            {
                accept_sock = socket(addrinfop->ai_family, addrinfop->ai_socktype,
                                     addrinfop->ai_protocol);
                // TODO bind on any address
                if (-1 == accept_sock)
                    continue;
                if (0 == bind(accept_sock, addrinfop->ai_addr, addrinfop->ai_addrlen))
                    break;
                close(accept_sock);
            }
            if (addrinfop == NULL)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot bind the socket to an IP endpoint {0}.", port);
                shutdown_socket_server();
                return -1;
            }
            freeaddrinfo(addrinfos);

            if (-1 == listen(accept_sock, SOMAXCONN))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "Cannot turn the socket into listening state.");
                shutdown_socket_server();
                return -1;
            }

            Diagnostics::WriteLine(Diagnostics::LogLevel::Info, "Listening endpoint :{0}", port);

            Diagnostics::WriteLine(Diagnostics::LogLevel::Info, "Waiting for client connection ...");

            socket_accept_thread = new std::thread(SocketAcceptThreadProc);

            return accept_sock;
        }

        int shutdown_socket_server()
        {
            /* signal the accept thread that we are shutting down */
            shutdown(accept_sock, SHUT_RD);
            if (socket_accept_thread != nullptr)
            {
                socket_accept_thread->join();
                delete socket_accept_thread;
                socket_accept_thread = nullptr;
            }
            close(accept_sock);

            /* Proceed to close all client sockets. */
            close_all_incoming_conn();

            return 0;
        }
    }
}
#endif
