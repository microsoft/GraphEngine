// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once

#include "TrinityCommon.h"
#include "os/os.h"
#include "Events/Events.h"

#include <cstdint>
#include <cstdlib>

#if defined(TRINITY_PLATFORM_WINDOWS)
#include <winsock2.h>
#include <ws2tcpip.h>
#include <Mstcpip.h>
#else
#include <sys/types.h>
#include <sys/socket.h>
#include <netinet/tcp.h>
#include <arpa/inet.h>
#include <netdb.h>
#include <netinet/in.h>
#include <fcntl.h>

typedef struct sockaddr_in SOCKADDR_IN;
typedef int SOCKET;
#define INVALID_SOCKET -1
#define SOCKET_ERROR -1
#define closesocket close
#endif

namespace Trinity
{
    namespace Network
    {
        /* Platform-specific socket descriptor, represents a context object associated with each socket */
        struct sock_t
        {
            /**
             * for RECV:
             *   - initialized to rx_buf.
             *   - receive both SOCKET_HEADER & payload
             *   - when SCOKET_HEADER is received, copy to msg_len
             *   - advance to next receive position.
             *   - reset to rx_buf + SOCKET_HEADER after a message is completely received
             * for SEND:
             *   - allocated by a message handler
             *   - advance to next send position
             *   - after message is sent, reset to msg_buf - msg_len, free it.
             */
            char*   msg_buf;          //size: 8 
            /**
             * during send/recv, remains constant and indicates payload length:
             * for SEND: including SOCKET_HEADER (4B)
             * for RECV: excluding SOCKET_HEADER (4B)
             */
            int32_t msg_len;          //size: 4
            /**
             * during send/recv, indicates pending transmission length.
             * for SEND: initialized to msg_len
             * for RECV: set to msg_len after SOCKET_HEADER is received.
             */
            uint32_t pending_len;     //size: 4

            char*    rx_buf;          //size: 8
            uint32_t rx_len;          //size: 4
            // ============== 32 bytes ==============
            SOCKET socket;            // size: WIN=8; UNIX=4
#if !defined(TRINITY_PLATFORM_WINDOWS)
            int32_t __socket_pad; // size: 4
#endif
#if defined(TRINITY_PLATFORM_WINDOWS)
            WSABUF wsa_buf;  // size: 16
#elif defined(TRINITY_PLATFORM_LINUX)
#else
#error("unsupported")
#endif
            Events::work_t* work; // size: 8
            // =============== 64 bytes ==============

            uint32_t wait_handshake; //size:4
            uint32_t avg_rx_len; // size: 4
        };


        int start_socket_server(uint16_t port);
        int shutdown_socket_server();
        bool initialize_network();
        bool uninitialize_network();

        // socket management
        sock_t* alloc_socket(SOCKET sock);
        void free_socket(sock_t* p);
        void add_socket(sock_t* p);
        void remove_socket(sock_t* p);
        void reset_socket(sock_t * p);

        // close_client_conn will also call remove_sock
        void close_client_conn(sock_t* p, bool lingering);
        void close_all_client_conn();

        // I/O interfaces

        void send_async(sock_t* p);
        void recv_async(sock_t* p);

        bool process_send(sock_t* p, uint32_t sz_sent);
        bool process_recv(sock_t* p, uint32_t sz_recv);

        void check_handshake(sock_t* p);

        inline void send_rsp(sock_t* p)
        {
            p->pending_len = p->msg_len;
            send_async(p);
        }

        enum TrinityMessageType : uint16_t
        {
            SYNC,
            SYNC_WITH_RSP,
            ASYNC,
            PRESERVED_SYNC,
            PRESERVED_SYNC_WITH_RSP,
            PRESERVED_ASYNC,
        };

        const uint32_t RX_DEFAULT_LEN = 8192;
        const uint32_t SOCKET_HEADER = 4;

        const double AvgSlideWin_a = 0.85f;
        const double AvgSlideWin_b = 0.15f;
        const double AvgSlideWin_r = 2.15f;

        extern uint8_t  HANDSHAKE_MESSAGE_CONTENT[];
        extern int      HANDSHAKE_MESSAGE_LENGTH;

        uint64_t client_socket();
        bool client_connect(uint64_t clientsocket, uint32_t ip, uint16_t port);
        bool client_send(uint64_t socket, char* buf, int32_t len);
        bool client_send_multi(uint64_t socket, char** bufs, int32_t* lens, int32_t cnt);
        TrinityErrorCode client_recv(uint64_t socket, OUT char* & buf, OUT int32_t & len);
        TrinityErrorCode client_wait(uint64_t socket);
        void client_close(uint64_t socket);
    }
}
