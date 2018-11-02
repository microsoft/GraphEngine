// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#include "Network.h"
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
        std::unordered_set<sock_t*> psco_set;
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

        void add_socket(sock_t * pContext)
        {
            psco_spinlock.lock();
            psco_set.insert(pContext);
            psco_spinlock.unlock();
        }

        void remove_socket(sock_t* psco)
        {
            psco_spinlock.lock();
            psco_set.erase(psco);
            psco_spinlock.unlock();
        }

        void close_all_client_conn()
        {
            psco_spinlock.lock();
            auto psco_set_shadow = psco_set;
            psco_spinlock.unlock();

            for (auto& pctx : psco_set_shadow) { close_client_conn(pctx, false); }
        }

        void free_socket(sock_t* p)
        {
            Events::free_work(p->work);
            free(p->rx_buf);
            // when not overlapped with rx_buf, msg_buf is a send buffer.
            if (p->msg_buf < p->rx_buf || p->msg_buf >= p->rx_buf + p->rx_len)
            {
                free(p->msg_buf + p->pending_len - p->msg_len);
            }

            free(p);
        }

        /// Allocates a Per-socket context object(PSCO).
        /// The handshake protocol:
        /// Client sends handshake signature:
        /// [4B Len|SIGNATURE_BODY]
        /// Server checks the signature and reply:
        /// [4B TrinityErrorCode]/[Disconnect]
        sock_t* alloc_socket(SOCKET socket)
        {
            sock_t* p = (sock_t*)malloc(sizeof(sock_t));
            Diagnostics::WriteLine(Diagnostics::LogLevel::Verbose, "Network: sock_t {0} allocated. fd = {1}", p, socket);

            p->socket         = socket;
            p->rx_buf         = (char*)malloc(RX_DEFAULT_LEN);
            p->rx_len         = RX_DEFAULT_LEN;
            p->pending_len    = p->rx_len;
            p->msg_buf        = p->rx_buf;
            p->msg_len        = 0;
            p->work           = Events::alloc_work(Events::worktype_t::Receive);
            p->work->psock    = p;
            p->wait_handshake = TrinityConfig::Handshake();
            p->avg_rx_len     = p->rx_len;

            return p;
        }

        /// Resets context object so that it is ready for a receive operation.
        /// This routine is called after the buffer passed from C# is sent.
        void reset_socket(sock_t * p)
        {
            // If the average received message length drops below half of current recv buf len, adjust it.
            if (p->avg_rx_len < p->rx_len / AvgSlideWin_r)
            {
                free(p->rx_buf);
                p->rx_len = p->avg_rx_len;
                p->rx_buf = (char*)malloc(p->rx_len);
            }

            //free response buffer passed from handler, rearm at rx_buf
            free(p->msg_buf - p->msg_len);
            p->msg_buf = p->rx_buf;
            p->msg_len = 0;
            p->pending_len = p->rx_len;
        }
#pragma endregion

        void check_handshake(sock_t* psock)
        {
            if (psock->msg_len != HANDSHAKE_MESSAGE_LENGTH)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ServerSocket: Client {0} responds with invalid handshake message header.", psock);
                goto handshake_check_fail;
            }

            if (memcmp(psock->msg_buf, HANDSHAKE_MESSAGE_CONTENT, HANDSHAKE_MESSAGE_LENGTH) != 0)
            {
                Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ServerSocket: Client {0} responds with invalid handshake message.", psock);
                goto handshake_check_fail;
            }

            // handshake_check_success: acknowledge the handshake and then switch into recv mode
            psock->wait_handshake = false;
            psock->msg_buf = (char*)malloc(sizeof(int32_t));
            psock->msg_len = sizeof(int32_t);
            *(TrinityErrorCode*)psock->msg_buf = TrinityErrorCode::E_SUCCESS;
            send_rsp(psock);
            return;

        handshake_check_fail:
            close_client_conn(psock, false);
            return;
        }

        // return value indicates whether the whole message has been sent.
        bool process_send(sock_t* p_sock, uint32_t bytesSent)
        {
            p_sock->pending_len -= (int32_t)bytesSent;
            p_sock->msg_buf += (int32_t)bytesSent;
            return p_sock->pending_len == 0;
        }

        // return value indicates whether the whole message is received
        bool process_recv(sock_t * psock, uint32_t bytesRecvd)
        {
            if (bytesRecvd == 0)
            {
                close_client_conn(psock, false);
                return false;
            }

            auto recv = psock->msg_buf - psock->rx_buf;

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
                        Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Error, "ServerSocket: Incorrect client handshake sequence, Client = {0}", psock);
                        close_client_conn(psock, false);
                        return false;
                    }

                    if (psock->msg_len < 0)
                    {
                        Trinity::Diagnostics::WriteLine(Trinity::Diagnostics::LogLevel::Error, "ServerSocket: Incorrect message header received, Client = {0}", psock);
                        close_client_conn(psock, false);
                        return false;
                    }

                    // Message prefix received into msg_len. Calibrate rx_buf now.

                    auto new_len = psock->msg_len + SOCKET_HEADER;
                    if (new_len > psock->rx_len)
                    {
                        char* new_buf = (char*)malloc(new_len);
                        if (NULL == new_buf)
                        {
                            Diagnostics::WriteLine(Diagnostics::Error, "ServerSocket: Cannot allocate memory during message receiving.");
                            close_client_conn(psock, false);
                            return false;
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
                    Network::recv_async(psock);
                    return false;
                }
            }

            if (psock->pending_len > bytesRecvd)
            {
                // the whole message body is not yet completely received
                psock->msg_buf     += bytesRecvd;
                psock->pending_len -= bytesRecvd;
                Network::recv_async(psock);
                return false;
            }
            else if (psock->pending_len == bytesRecvd)
            {
                // we have the complete message.
                psock->msg_buf = psock->rx_buf + SOCKET_HEADER;
                return true;
            }
            else
            {
                Diagnostics::WriteLine(Diagnostics::Error, "Network: receiving more than a complete message.");
                close_client_conn(psock, false);
                return false;
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
