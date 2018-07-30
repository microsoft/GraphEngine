from Redy.Magic.Classic import execute

import socket, abc


def async_apply(fn, *args, **kwargs):
    while True:
        try:
            return fn(*args, **kwargs)
        except BlockingIOError:
            yield


def async_apply_thunk(fn):
    while True:
        try:
            return fn()
        except BlockingIOError:
            yield


def async_apply_one_arg(fn, arg):
    while True:
        try:
            return fn(arg)
        except BlockingIOError:
            yield


class AsyncServer:
    def __init__(self, hostname='127.0.0.1', port=33044):
        self.host = host = socket.gethostbyname(hostname)
        self.port = port

        self.sock = server_sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server_sock.bind((host, port))
        server_sock.setblocking(False)
        server_sock.listen(5)
        self._msg_handlers = []

    def register_msg_handler(self, msg_id, func):
        msg_handlers = self._msg_handlers
        if len(msg_handlers) <= msg_id:
            msg_handlers.extend([None] * (1 + len(msg_handlers) - msg_id))
        self._msg_handlers[msg_id] = func

    def client_filter_rule(self, addr):
        return bool(self)

    def exception_handler(self, exc: Exception):
        self.sock.close()
        raise exc

    @abc.abstractmethod
    def client_processor(self, client):
        raise NotImplemented

    def run(self):
        event_sources = []
        running = True
        sock = self.sock
        sock.setblocking(False)

        @event_sources.append
        @execute
        def process_client_accepting():
            client, addr = yield from async_apply_thunk(sock.accept)
            if self.client_filter_rule(addr):
                client.setblocking(False)
                event_sources.append(self.client_processor(client))

        def process_event_loop(loop):
            while running:
                try:
                    loop.send(None)
                except Exception as e:
                    self.exception_handler(e)
                    sock.close()

        @process_event_loop
        @execute
        def event_loop():
            while running:
                for each in tuple(event_sources):
                    try:
                        each.send(None)
                    except StopIteration:
                        event_sources.remove(each)
                yield


from GraphEngine.DotNet.env import build_module
from .type.factory import TSLServerConnection


class TSLClient:
    def __init__(self, hostname='127.0.0.1', port=33044):
        self.host = host = socket.gethostbyname(hostname)
        self.port = port
        self.sock = client_sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        client_sock.connect((host, port))

    def run(self):
        sock = self.sock
        try:
            while True:
                head = sock.recv(4)
                print(head)
        except:
            sock.close()


def f():
    def process_client_receiving(client_soc: socket.socket):
        def async_recv(n):
            try:
                return client_soc.recv(n)
            except BlockingIOError:
                yield

        client_soc.setblocking(False)
        while running:
            a, b = yield from async_recv(2)  # msgid

            msg_id = int(b) + int(a) * 256
            if msg_id == 0:
                break

            msg_handler = msg_handlers[msg_id]

            a, b, c, d = client_soc.recv(4)  # datasize
            data_size = int(d) + int(c) * 256
            if c != b'0x0':
                data_size += int(b) * 256 ** 2

            if d != b'0x0':
                data_size += int(a) * 256 ** 3

            data = yield from async_recv(data_size)

            client_soc.sendall(msg_handler(data))


class ClientConnection(TSLServerConnection):
    def __init__(self):
        self._module = None
        self._methods_name_mapping = {}

    def build_module(self, tsl_codes, namespace):
        def _inf():
            it = 0
            while True:
                yield it
                it += 1

        module = build_module(tsl_codes, namespace)
        methods = {k: v for k, v in module.__dict__.items() if not k.startswith('__')}
        methods_id_mapping = dict(zip(methods, _inf()))
