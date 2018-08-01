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


class AsyncSocketServer(abc.ABC):
    def __init__(self, hostname='127.0.0.1', port=33044):
        self.host = host = socket.gethostbyname(hostname)
        self.port = port
        self.sock = server_sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server_sock.bind((host, port))
        server_sock.setblocking(False)
        server_sock.listen(5)

    @abc.abstractmethod
    def client_filter_rule(self, addr):
        raise NotImplemented

    def exception_handler(self, exc: Exception):
        self.sock.close()
        raise exc

    @abc.abstractmethod
    def client_processor(self, client: socket.socket):
        raise NotImplemented

    def run_async(self):
        event_sources = []
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
            _next = next
            while True:
                try:
                    yield _next(loop)
                except Exception as e:
                    self.exception_handler(e)
                    sock.close()

        @process_event_loop
        @execute
        def event_loop():
            _next = next
            _tuple = tuple
            while True:
                for each in _tuple(event_sources):
                    try:
                        yield _next(each)
                    except StopIteration:
                        event_sources.remove(each)

        yield from event_loop

    def run(self):
        coroutine = self.run_async()
        try:
            while True:
                coroutine.send(None)
        except StopIteration as e:
            return e.value
