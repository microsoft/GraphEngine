// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "os/os.h"
#include "Network/Server/TrinityServer.h"
#ifdef TRINITY_PLATFORM_WINDOWS
#include "Network/Server/iocp/TrinitySocketServer.h"
#else
#include "Network/Server/posix/TrinitySocketServer.h"
#endif

#include <stdio.h>
#include <string.h>

#include <iostream>

namespace Trinity
{
  namespace Network
  {
    bool register_message_handler(uint16_t msg_type, message_handler_t * handler) {
      handlers[msg_type] = handler;
      return true;
    }

    void default_handler(MessageBuff * buff) {
        buff->Buffer = (char *)malloc(sizeof(TrinityErrorCode));
        buff->BytesToSend = sizeof(TrinityErrorCode);
        *((TrinityErrorCode *)(buff->Buffer)) = TrinityErrorCode::E_RPC_EXCEPTION;
    }

    bool StartWorkerThreadPool()
    {
      int thread_count = std::thread::hardware_concurrency();

      for (size_t i = 0; i < MAX_HANDLERS_COUNT; ++i) {
          handlers[i] = &default_handler;
      }

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
        fwprintf(stderr, L">>> cannot start socket server\n");
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
      fwprintf(stderr, L">>> in message handler, message length: %d \n", msg->BytesReceived);
      // Parse MsgBuff
      auto buff = msg->Buffer;
      int len = msg->BytesReceived;
      uint16_t msg_type = *((uint16_t *)buff);
      handlers[msg_type](msg);

      //msg->Buffer = (char*)malloc(11);
      //memset(msg->Buffer, 0, 11);
      //*(uint32_t*)(msg->Buffer) = 7;
      //memcpy(msg->Buffer + 4, "hello\n", 6);
      //msg->BytesToSend = 11;
    }

    void WorkerThreadProc(int tid)
    {
      fprintf(stderr, ">>> Worker Thread: %d\n", tid);
      EnterSocketServerThreadPool();
      while (true)
      {
        void* _pContext = NULL;
        AwaitRequest(_pContext);
        if (_pContext == NULL) { break; }
        PerSocketContextObject* pContext = (PerSocketContextObject*)_pContext;
        MessageHandler((MessageBuff*)pContext);
        SendResponse(pContext);
      }
      fprintf(stderr, "Exit Thread %d\n", tid);
      ExitSocketServerThreadPool();
    }

    void CheckHandshakeResult(PerSocketContextObject* pContext)
    {
      if (pContext->ReceivedMessageBodyBytes != HANDSHAKE_MESSAGE_LENGTH)
      {
        Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ServerSocket: Client {0} responds with invalid handshake message header.", pContext);
        goto handshake_check_fail;
      }

      if (memcmp(pContext->Message, HANDSHAKE_MESSAGE_CONTENT, HANDSHAKE_MESSAGE_LENGTH) != 0)
      {
        Diagnostics::WriteLine(Diagnostics::LogLevel::Error, "ServerSocket: Client {0} responds with invalid handshake message.", pContext);
        goto handshake_check_fail;
      }

      // handshake_check_success: acknowledge the handshake and then switch into recv mode
      pContext->WaitingHandshakeMessage = false;
      pContext->Message = (char*)malloc(sizeof(int32_t));
      pContext->RemainingBytesToSend = sizeof(int32_t);
      *(int32_t*)pContext->Message = (int32_t)TrinityErrorCode::E_SUCCESS;
      SendResponse(pContext);
      return;

handshake_check_fail:
      CloseClientConnection(pContext, false);
      return;
    }

  }
}
