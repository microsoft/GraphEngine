// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Client/TrinityClient.h"
#include "Server/TrinityServer.h"

using namespace Trinity;

// Client interfaces

DLL_EXPORT uint64_t CreateClientSocket() { return Network::CreateClientSocket(); }
DLL_EXPORT BOOL ClientSocketConnect(uint64_t socket, uint32_t ip, uint16_t port) { return Network::ClientSocketConnect(socket, ip, port) ? TRUE : FALSE; }
DLL_EXPORT BOOL ClientSend(uint64_t socket, char* buf, int32_t len) { return Network::ClientSend(socket, buf, len) ? TRUE : FALSE; }
DLL_EXPORT BOOL ClientSendMulti(uint64_t socket, char** bufs, int32_t* lens, int32_t cnt) { return Network::ClientSendMulti(socket, bufs, lens, cnt) ? TRUE : FALSE; }
DLL_EXPORT TrinityErrorCode ClientReceive(uint64_t socket, char* &buf, int32_t &len) { return Network::ClientReceive(socket, buf, len); }
DLL_EXPORT TrinityErrorCode WaitForAckPackage(uint64_t socket) { return Network::WaitForAckPackage(socket); }
DLL_EXPORT void CloseClientSocket(uint64_t socket) { Network::CloseClientSocket(socket); }

// Server interfaces
DLL_EXPORT int StartSocketServer(uint16_t port) { return Network::StartSocketServer(port); }
DLL_EXPORT int StopSocketServer() { return Network::ShutdownSocketServer(); }
DLL_EXPORT int ShutdownSocketServer() { return Network::ShutdownSocketServer(); }
DLL_EXPORT void AwaitRequest(void*& pContext) { Network::AwaitRequest(pContext); }
DLL_EXPORT void SendResponse(void* pContext) { Network::SendResponse(pContext); }
DLL_EXPORT void EnterSocketServerThreadPool() { Network::EnterSocketServerThreadPool(); }
DLL_EXPORT void ExitSocketServerThreadPool() { Network::ExitSocketServerThreadPool(); }
DLL_EXPORT bool StartWorkerThreadPool() { return Network::StartWorkerThreadPool(); }