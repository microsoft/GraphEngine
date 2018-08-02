from GraphEngine.fast_asyncio.server import AsyncSocketServer, async_apply_thunk, async_apply_one_arg, async_apply
from GraphEngine.tsl.type.factory import TSLServerConnection
from GraphEngine.DotNet.setup import Env, build_module
from Redy.Tools.TypeInterface import Module
from Redy.Opt import feature, const, constexpr

import socket
import marshal

staging = (const, constexpr)
_default_msg_handlers = []


class RedServer(AsyncSocketServer, TSLServerConnection):

    def __init__(self, *args, **kwargs):
        super().__init__(*args, **kwargs)
        self._msg_handlers = []
        self.module = None
        self._accessors = []

    def register_msg_handler(self, msg_id, func):
        msg_handlers = self._msg_handlers

        if len(msg_handlers) <= msg_id:
            msg_handlers.extend([None] * (len(msg_handlers) - msg_id + 1))

        msg_handlers[msg_id] = func

    def build_module(self, tsl_codes, namespace):
        module: Module = build_module(tsl_codes, namespace)
        method_names, method_objs = zip(*((k, v) for k, v in module.__dict__.items() if not k.startswith('__')))

        def stream():
            for each in method_objs:
                @feature(staging)
                def f(args=()):
                    method: const = each
                    return method(*args)

                yield f

        methods_len = len(self._msg_handlers)
        id2method_names = dict(zip(method_names, range(methods_len, methods_len + len(method_names))))

        self._msg_handlers.extend(method_objs)

        return id2method_names

    def client_processor(self, client: socket.socket):
        client_recv = client.recv

        client_sendall = client.sendall

        msg_handlers = self._msg_handlers

        while True:
            buff: bytes = yield from async_apply_one_arg(client_recv, 2)
            msg_handler_id = int.from_bytes(buff, 'big')
            msg_handler = msg_handlers[msg_handler_id]

            buff = yield from async_apply_one_arg(client_recv, 4)
            buff_len = int.from_bytes(buff, 'big')

            buff = yield from async_apply_one_arg(client_recv, buff_len)
            ret_buf = msg_handler(buff)
            client_sendall(ret_buf)

    def client_filter_rule(self, addr):
        return True


# some default message handler


def build_tsl(self, buf: bytes):
    tsl_codes, namespace = marshal.loads(buf)

    module: Module = build_module(tsl_codes, namespace)

    method_names, method_objs = zip(*(k, v) for k, v in module.__dict__.items() if not k.startswith('__'))

    id2method_names = {name: idx for idx, name in enumerate(method_names)}

    self._msg_handlers.extend(method_objs)

    return marshal.dumps(id2method_names)
