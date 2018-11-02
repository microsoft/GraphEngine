// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Network.h"

using namespace Trinity;

// Client interfaces

DLL_EXPORT uint64_t CreateClientSocket() { return Network::client_socket(); }
DLL_EXPORT BOOL ClientSocketConnect(uint64_t socket, uint32_t ip, uint16_t port) { return Network::client_connect(socket, ip, port) ? TRUE : FALSE; }
DLL_EXPORT BOOL ClientSend(uint64_t socket, char* buf, int32_t len) { return Network::client_send(socket, buf, len) ? TRUE : FALSE; }
DLL_EXPORT BOOL ClientSendMulti(uint64_t socket, char** bufs, int32_t* lens, int32_t cnt) { return Network::client_send_multi(socket, bufs, lens, cnt) ? TRUE : FALSE; }
DLL_EXPORT TrinityErrorCode ClientReceive(uint64_t socket, char* &buf, int32_t &len) { return Network::client_recv(socket, buf, len); }
DLL_EXPORT TrinityErrorCode WaitForAckPackage(uint64_t socket) { return Network::client_wait(socket); }
DLL_EXPORT void CloseClientSocket(uint64_t socket) { Network::client_close(socket); }

// Server interfaces
DLL_EXPORT int StartSocketServer(uint16_t port) { return Network::start_socket_server(port); }
DLL_EXPORT int StopSocketServer() { return Network::shutdown_socket_server(); }