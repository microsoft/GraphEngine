// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include <os/os.h>
#include "Network.h"
#include "Trinity/Diagnostics/Log.h"

namespace Trinity
{
    namespace Network
    {
        inline uint64_t SetSocketOptions(uint64_t socket, bool enable_keepalive, bool disable_sendbuf)
        {
            // No Delay: Disables the Nagle algorithm for send coalescing
            int32_t NoDelay = 1;
            if (SOCKET_ERROR == setsockopt(socket, IPPROTO_TCP, TCP_NODELAY, (char*)&NoDelay, sizeof(int32_t)))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "SetSocketOptions: Cannot disable Nagle algorithm.");
                closesocket(socket);
                return INVALID_SOCKET;
            }

            if (enable_keepalive)
            {
                // Enable sending keep-alive packets for a socket connections
                // If TCP keep-alive is enabled with SO_KEEPALIVE,
                // then the default TCP settings are used for keep-alive timeout and interval
                // unless these values have been changed using SIO_KEEPALIVE_VALS.
                int32_t KeepAlive = 1;
                setsockopt(socket, SOL_SOCKET, SO_KEEPALIVE, (char*)&KeepAlive, sizeof(int32_t));

#ifdef TRINITY_PLATFORM_WINDOWS
                tcp_keepalive tcpKeepAlive{
                    1,     // onoff

                    60000, // keepalivetime, specifies the timeout, in milliseconds,
                    // with no activity until the first keep-alive packet is sent

                    3000   // keepaliveinterval, specifies the interval, in milliseconds,
                    // between when successive keep-alive packets are sent if no acknowledgement is received
                };
                DWORD lpcbBytesReturned;
                if (SOCKET_ERROR == WSAIoctl(socket, SIO_KEEPALIVE_VALS, &tcpKeepAlive, sizeof(tcp_keepalive), NULL, 0, &lpcbBytesReturned, NULL, NULL))
                {
                    int errorCode = WSAGetLastError();
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "SetSocketOptions: Cannot set TCP KeepAlive value, error code {0}.", errorCode);
                    closesocket(socket);
                    return INVALID_SOCKET;
                }
#else
                int32_t keepidle  = 60000;
                int32_t keepintvl = 3000;
                setsockopt(socket, IPPROTO_TCP, TCP_KEEPIDLE, &keepidle, sizeof(int32_t));
                setsockopt(socket, IPPROTO_TCP, TCP_KEEPINTVL, &keepintvl, sizeof(int32_t));
#endif
            }

            if (disable_sendbuf)
            {
                // Disable socket send buffer
                int32_t zero = 0;
                if (SOCKET_ERROR == setsockopt(socket, SOL_SOCKET, SO_SNDBUF, (char*)&zero, sizeof(int32_t)))
                {
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "SetSocketOptions: Cannot set disable socket send buffer.");
                    closesocket(socket);
                    return INVALID_SOCKET;
                }
            }

            return socket;
        }

        inline uint64_t GetSocketOptions(uint64_t socket)
        {
#ifdef TRINITY_PLATFORM_WINDOWS
            int iOptVal = 0;
            int iOptLen = sizeof(int);

            if (SOCKET_ERROR == getsockopt(socket, IPPROTO_TCP, TCP_NODELAY, (char*)&iOptVal, &iOptLen))
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "GetSocketOptions: cannot get TCP_NODELAY, error code {0}.", WSAGetLastError());
            else
                Diagnostics::WriteLine(Diagnostics::LogLevel::Info, "TCP_NODELAY Value : {0}", iOptVal);

            if (SOCKET_ERROR == getsockopt(socket, SOL_SOCKET, SO_KEEPALIVE, (char *)&iOptVal, &iOptLen))
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "GetSocketOptions: cannot get SO_KEEPALIVE, error code {0}.", WSAGetLastError());
            else
                Diagnostics::WriteLine(Diagnostics::LogLevel::Info, "SO_KEEPALIVE Value : {0}", iOptVal);

            SSIZE_T szOptVal = 0; //TODO: double check whether a 64-bit value is necessary
            int szOptLen = sizeof(SSIZE_T);
            if (SOCKET_ERROR == getsockopt(socket, SOL_SOCKET, SO_SNDBUF, (char *)&szOptVal, &szOptLen))
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "GetSocketOptions: cannot get SO_SNDBUF, error code {0}.", WSAGetLastError());
            else
                Diagnostics::WriteLine(Diagnostics::LogLevel::Info, "SO_SNDBUF Value : {0}", szOptVal);

            return socket;
#else
            //TODO GetSocketOptions
            return socket;
#endif
        }

#if !defined(TRINITY_PLATFORM_WINDOWS)
    static bool make_nonblocking(int fd)
    {
        int status_flags = fcntl(fd, F_GETFL, 0);
        if (-1 == status_flags)
            return false;
        status_flags |= O_NONBLOCK;
        if (-1 == fcntl(fd, F_SETFL, status_flags))
            return false;
        return true;
    }

#endif
    }
}

