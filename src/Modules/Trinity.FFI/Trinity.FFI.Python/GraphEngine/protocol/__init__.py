import socket

client = socket.socket(socket.AF_INET, socket.SOCK_STREAM)


class Server:
    def __init__(self, hostname='127.0.0.1', port=33044):
        self.host = host = socket.gethostbyname(hostname)
        self.port = port
        self.sock = server_sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        server_sock.bind((host, port))
        server_sock.listen(5)
        self.msg_handlers = {}

    def register_message_handler(self, msg_id, handler):
        self.msg_handlers[msg_id] = handler

    def run(self):
        """
        instruction: uint64
        arg: uint64

        NewCell (typeid: uint16)  -> (new_cell_location: uint64)
        LoadType (typeid: uint16) -> (typed_cell_caching_vec: std::vector<uint64>)
        Index (arg: uint64)  -> (addr)

        class S:
            a : int
            b: List[int]


        s = S() => NewCell "S"
        s = S(dict(a=1, b=[1,2,3]))

        =>
        Var.Load "a"
        Var.Load 1
        Var.Load 1
        Var.Load 2
        Var.Load 3

        Var.MakeList 3
        Var.BuildKeyValuePair 2
        InitCell "S"

        s.a = 1
        Var.Load 1
        LoadCell s
        Attr.Store 'a'

        SaveStorage <nil>
        LoadStorage <nil>
        ResetStorage <nil>

        """
