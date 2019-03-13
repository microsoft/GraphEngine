// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#include "Network.h"
#include "Events/Events.h"
#include "SocketOptionsHelper.h"
#include "Trinity/Threading/TrinityLock.h"
#include "Trinity/Diagnostics/Log.h"
#include "Trinity/Configuration/TrinityConfig.h"

#include <cstdio>
#include <cstring>
#include <unordered_set>

namespace Trinity
{
    namespace Network
    {
        TrinityLock psco_spinlock; // psco = per socket context object
        std::unordered_set<sock_t*> incoming_psco_set;
        using Trinity::Events::work_t;

#pragma region networking subsystem initialization
        TrinityLock spinlock;
        bool initialized = false;

        bool initialize_network()
        {
            spinlock.lock();
            if (!initialized)
            {
#if defined(TRINITY_PLATFORM_WINDOWS)
                WSAData wsaData;
                if (WSAStartup(MAKEWORD(2, 2), &wsaData) != 0)
                {
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Fatal, "Cannot initialize winsock.");
                    spinlock.unlock();
                    return false;
                }
#endif
                initialized = true;
            }
            spinlock.unlock();
            return true;
        }

        bool uninitialize_network()
        {
            spinlock.lock();
            if (initialized)
            {
#if defined(TRINITY_PLATFORM_WINDOWS)
                //  All client sockets are shut down. We have no network activity now.
                //  Proceed to bring down WSA.
                WSACleanup();
#endif

                initialized = false;
            }
            spinlock.unlock();

            return true;
        }
#pragma endregion

#pragma region socket desc management

        void close_all_incoming_conn()
        {
            psco_spinlock.lock();
            auto psco_set_shadow = incoming_psco_set;
            psco_spinlock.unlock();

            for (auto& pctx : psco_set_shadow) { close_conn(pctx, false); }
        }

        void close_conn(sock_t* p, bool lingering)
        {
            if (p->is_incoming)
            {
                // Remove it from the client socket set
                psco_spinlock.lock();
                incoming_psco_set.erase(p);
                psco_spinlock.unlock();
            }

            if (!lingering)
            {
                LINGER _linger;
                _linger.l_onoff = 1; //The socket will remain open for a specified amount of time.
                _linger.l_linger = 0; //
                setsockopt(p->socket, SOL_SOCKET, SO_LINGER, (char*)&_linger, sizeof(_linger));
            }

#if defined(TRINITY_PLATFORM_WINDOWS)
            // closesocket function implicitly causes a shutdown sequence
            // closesocket deallocates socket handles and free up associated resources.
            // closesocket function implicitly causes a shutdown sequence to occur if it has not already happened,
            // we can use closesocket to both initiate the shutdown sequence and deallocate the socket handle.

            // the sockets interface provides for controls by way of the socket option mechanism that
            // allow us to indicate whether the implicit shutdown sequence should be graceful or abortive.
            // and also whether the closesocket function should linger(that is not complete immediately) to
            // allow time for a graceful shutdown sequence to complete.

            // More info: https://msdn.microsoft.com/en-us/library/windows/desktop/ms738547%28v=vs.85%29.aspx
            closesocket(p->socket);
#else
            shutdown(p->socket, SHUT_RDWR);
            close(p->socket);
#endif
            if (p->is_incoming)
            {
                Events::free_work(p->work);
                free(p->rx_buf);
                // when not overlapped with rx_buf, msg_buf is a send buffer.
                if (p->msg_buf < p->rx_buf || p->msg_buf >= p->rx_buf + p->rx_len)
                {
                    free(p->msg_buf + p->pending_len - p->msg_len);
                }
            }
            else
            {
                // detach the socket from the work entry.
                // this way, the wait chain can be preserved,
                // and continued into with a failure status.
                p->work->psock = nullptr;
            }

            free(p);
        }

        /// Allocates a Per-socket context object(PSCO).
        sock_t* alloc_sock_t(SOCKET socket, bool is_incoming)
        {
            sock_t* p = (sock_t*)malloc(sizeof(sock_t));
            Diagnostics::WriteLine(Diagnostics::LogLevel::Verbose, "Network: sock_t {0} allocated. fd = {1}", p, socket);

            Events::worktype_t mode;
            bool handshake;

            if (is_incoming)
            {
                mode           = Events::worktype_t::Receive;
                handshake      = TrinityConfig::Handshake();
                p->rx_buf      = (char*)malloc(RX_DEFAULT_LEN);
                p->pending_len = RX_DEFAULT_LEN;
                p->msg_buf     = p->rx_buf;
                p->msg_len     = 0;
            }
            else
            {
                // send buffer will be supplied shortly after.
                mode           = Events::worktype_t::Send;
                handshake      = false;
                p->rx_buf      = nullptr;
                p->pending_len = 0;
                p->msg_buf     = nullptr;
                p->msg_len     = 0;
            }

            p->socket          = socket;
            p->rx_len          = RX_DEFAULT_LEN;
            p->work            = Events::alloc_work(mode);
            p->work->psock     = p;
            p->wait_handshake  = handshake;
            p->avg_rx_len      = RX_DEFAULT_LEN;
            p->is_incoming     = is_incoming;

            if (is_incoming)
            {
                psco_spinlock.lock();
                incoming_psco_set.insert(p);
                psco_spinlock.unlock();
            }

            return p;
        }

#pragma endregion

        /// The handshake protocol:
        /// Client sends handshake signature:
        /// [4B Len|SIGNATURE_BODY]
        /// Server checks the signature and reply:
        /// [4B TrinityErrorCode]/[Disconnect]
        TrinityErrorCode check_handshake(sock_t* psock)
        {
            if (psock->msg_len != HANDSHAKE_MESSAGE_LENGTH)
            {
                return TrinityErrorCode::E_INIT_FAIL;
            }

            if (memcmp(psock->msg_buf, HANDSHAKE_MESSAGE_CONTENT, HANDSHAKE_MESSAGE_LENGTH) != 0)
            {
                return TrinityErrorCode::E_INIT_FAIL;
            }

            // handshake_check_success: acknowledge the handshake and then switch into recv mode
            psock->wait_handshake = false;
            psock->msg_buf = (char*)malloc(sizeof(int32_t));
            psock->msg_len = sizeof(int32_t);
            *(TrinityErrorCode*)psock->msg_buf = TrinityErrorCode::E_SUCCESS;
            return send_message(psock);
        }

        // return value indicates whether the whole message has been sent.
        TrinityErrorCode process_send(sock_t* p_sock, uint32_t bytesSent)
        {
            p_sock->pending_len -= (int32_t)bytesSent;
            p_sock->msg_buf += (int32_t)bytesSent;

            if (p_sock->pending_len == 0)
            {
                return TrinityErrorCode::E_SUCCESS;
            }

            if (TrinityErrorCode::E_SUCCESS == Network::send_async(p_sock))
            {
                return TrinityErrorCode::E_RETRY;
            }
            else
            {
                return TrinityErrorCode::E_NETWORK_RECV_FAILURE;
            }
        }

        // return value indicates whether the whole message is received
        TrinityErrorCode process_recv(sock_t * psock, uint32_t bytesRecvd)
        {
            if (bytesRecvd == 0)
            {
                return TrinityErrorCode::E_NOENTRY;
            }

            auto recv = (uint32_t)(psock->msg_buf - psock->rx_buf);

            // Message prefix is not yet completely received
            if (recv < SOCKET_HEADER)
            {
                if (recv + bytesRecvd >= SOCKET_HEADER)
                {
                    bytesRecvd -= (SOCKET_HEADER - recv);
                    psock->msg_len = *(int32_t*)psock->rx_buf;

                    // Checking length boundaries

                    if (psock->wait_handshake && psock->msg_len != HANDSHAKE_MESSAGE_LENGTH)
                    {
                        return TrinityErrorCode::E_INIT_FAIL;
                    }

                    if (psock->msg_len < 0)
                    {
                        return TrinityErrorCode::E_RPC_EXCEPTION;
                    }

                    // Message prefix received into msg_len. Calibrate rx_buf now.

                    auto new_len = psock->msg_len + SOCKET_HEADER;
                    if (new_len > psock->rx_len)
                    {
                        char* new_buf = (char*)malloc(new_len);
                        if (NULL == new_buf)
                        {
                            return TrinityErrorCode::E_NOMEM;
                        }
                        memcpy(new_buf + SOCKET_HEADER,
                               psock->rx_buf + SOCKET_HEADER,
                               bytesRecvd);
                        free(psock->rx_buf);
                        psock->rx_len = new_len;
                        psock->rx_buf = new_buf;
                    }

                    // calculate average received message length with a sliding window.
                    psock->avg_rx_len = (uint32_t)(psock->avg_rx_len * AvgSlideWin_a +
                                                   psock->msg_len * AvgSlideWin_b);

                    // Make sure that average received message length is capped above default value.
                    psock->avg_rx_len = std::max(psock->avg_rx_len, RX_DEFAULT_LEN);

                    // continue to process body payload, as if we've done
                    // a recv from the beginning of msg_buf
                    psock->msg_buf     = psock->rx_buf + SOCKET_HEADER;
                    psock->pending_len = psock->msg_len;
                }
                else
                {
                    psock->msg_buf += bytesRecvd;
                    return Network::recv_async(psock);
                }
            }

            if (psock->pending_len > bytesRecvd)
            {
                // the whole message body is not yet completely received
                psock->msg_buf     += bytesRecvd;
                psock->pending_len -= bytesRecvd;

                return Network::recv_async(psock);
            }
            else if (psock->pending_len == bytesRecvd)
            {
                // we have the complete message.
                psock->msg_buf = psock->rx_buf + SOCKET_HEADER;
                return TrinityErrorCode::E_SUCCESS;
            }
            else
            {
                // receiving more than a message
                return TrinityErrorCode::E_MSG_OVERFLOW;
            }
        }

#pragma region client implementation

        bool client_handshake(uint64_t clientsocket)
        {
            // send handshake sequence

            char* send_buff = (char*)malloc(HANDSHAKE_MESSAGE_LENGTH + sizeof(int32_t));
            *(int32_t*)send_buff = HANDSHAKE_MESSAGE_LENGTH;
            memcpy(send_buff + sizeof(int32_t), HANDSHAKE_MESSAGE_CONTENT, HANDSHAKE_MESSAGE_LENGTH);

            if (!client_send(clientsocket, send_buff, sizeof(int32_t) + HANDSHAKE_MESSAGE_LENGTH))
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ClientSocket: Server rejected handshake request message. Thread Id = {0}, Socket = {1}", std::this_thread::get_id(), clientsocket);
                goto handshake_check_fail;
            }

            if (client_wait(clientsocket) != TrinityErrorCode::E_SUCCESS)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ClientSocket: Incorrect server handshake ACK message. Thread Id = {0}, Socket = {1}", std::this_thread::get_id(), clientsocket);
                goto handshake_check_fail;
            }

            //handshake_check_success: cleanup and return now.
            free(send_buff);
            return true;

        handshake_check_fail:
            free(send_buff);
            closesocket((SOCKET)clientsocket);
            return false;

        }

        uint64_t client_socket()
        {
#if defined(TRINITY_PLATFORM_WINDOWS)
            initialize_network();
#endif

            SOCKET clientsocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);

            if (INVALID_SOCKET == clientsocket)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ClientSocket: Cannot create a client socket.");
                closesocket(clientsocket);
                return INVALID_SOCKET;
            }

            // The client side already have heartbeat packages. Don't enable keepalive messages.
            return SetSocketOptions(clientsocket, /*enable_keepalive:*/ false, /*disable_sendbuf:*/TrinityConfig::ClientDisableSendBuffer());
        }

        bool client_connect(uint64_t clientsocket, uint32_t ip, uint16_t port)
        {
            SOCKADDR_IN ipe;

            ipe.sin_family = AF_INET;
            ipe.sin_port = htons(port); // converts a u_short from host to network byte order (big-endian)
#if defined(TRINITY_PLATFORM_WINDOWS)
            ipe.sin_addr.S_un.S_addr = ip;
#else
            ipe.sin_addr.s_addr = ip;
#endif

            if (SOCKET_ERROR == connect((SOCKET)clientsocket, (sockaddr*)&ipe, sizeof(SOCKADDR_IN)))
            {
                //Diagnostics::WriteLine(Diagnostics::LogLevel::Warning, "Cannot connect to an IP endpoint.");
                closesocket((SOCKET)clientsocket);
                return false;
            }

            // the initial
            if (TrinityConfig::Handshake())
            {
                return client_handshake(clientsocket);
            }
            else
            {
                return true;
            }
        }

        bool client_send(uint64_t socket, char* buf, int32_t len)
        {
            do
            {
#if defined(TRINITY_PLATFORM_WINDOWS)
                int32_t bytesSent = send((SOCKET)socket, buf, len, 0);
#else
                ssize_t bytesSent = write((int)socket, buf, len);
#endif
                if (SOCKET_ERROR == bytesSent)
                {
#if defined(TRINITY_PLATFORM_WINDOWS)
                    int error_code = WSAGetLastError();
#else
                    int error_code = errno;
#endif
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ClientSocket: Errors occur during network send: {0} bytes to send. Error code = {1}, Thread Id = {2}", len, error_code, std::this_thread::get_id());

                    if (INVALID_SOCKET == socket)
                    {
                        Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ClientSocket: Provided socket for send is invalid.");
                    }

                    closesocket((SOCKET)socket);
                    return false;
        }
                buf += bytesSent;
                len -= bytesSent;
    } while (len > 0);
    return true;
}

        bool client_send_multi(uint64_t socket, char** bufs, int32_t* lens, int32_t cnt)
        {
            for (; cnt > 0; --cnt)
            {
                if (!client_send(socket, *bufs, *lens)) return false;
                ++bufs;
                ++lens;
            }
            return true;
        }

        bool _do_recv(SOCKET socket, char* buf, int32_t len)
        {
            while (len)
            {
#if defined(TRINITY_PLATFORM_WINDOWS)
                int32_t bytesRecvd = recv(socket, buf, len, 0);
#else
                ssize_t bytesRecvd = read(socket, buf, len);
#endif
                if (SOCKET_ERROR == bytesRecvd || 0 == bytesRecvd)
                {
#if defined(TRINITY_PLATFORM_WINDOWS)
                    int error_code = WSAGetLastError();
#else
                    int error_code = errno;
#endif
                    Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ClientSocket: Errors occur during network recv: Error code = {0}, Thread Id={1}", error_code, std::this_thread::get_id());
                    closesocket((SOCKET)socket);
                    return false;
        }
                buf += bytesRecvd;
                len -= bytesRecvd;
            }

            return true;
        }

        TrinityErrorCode client_recv(uint64_t _socket, OUT char* & buf, OUT int32_t & len)
        {
            SOCKET socket = (SOCKET)_socket;
            if (!_do_recv(socket, (char*)&len, sizeof(int32_t)))
            {
                return TrinityErrorCode::E_NETWORK_RECV_FAILURE;
            }

            if (len < 0) return (TrinityErrorCode)len;

            buf = (char*)malloc(len); // will be freed by a C# message reader.
            if (NULL == buf)
            {
                Trinity::Diagnostics::FatalError("ClientSocket: Cannot allocate memory in network Receive.");
                return TrinityErrorCode::E_NOMEM;
            }

            if (!_do_recv(socket, buf, len))
            {
                free(buf);
                return TrinityErrorCode::E_NETWORK_RECV_FAILURE;
            }

            return TrinityErrorCode::E_SUCCESS;
        }

        TrinityErrorCode client_wait(uint64_t _socket)
        {
            int32_t buf;
            char *recvbuf = (char*)&buf;
            SOCKET socket = (SOCKET)_socket;
            if (!_do_recv(socket, recvbuf, sizeof(int32_t)))
            {
                return TrinityErrorCode::E_NETWORK_RECV_FAILURE;
            }
            else
            {
                return (TrinityErrorCode)buf;
            }
        }

        void client_close(uint64_t socket)
        {
#if defined (TRINITY_PLATFORM_WINDOWS)
            closesocket((SOCKET)socket);
#else
            shutdown((int)socket, SHUT_RDWR);
            close((int)socket);
#endif
        }

#pragma endregion
    }
}
