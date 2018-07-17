class TSL:
    def __init__(self, namespace=None):
        self._namespace = namespace

    def _define(self, type_cls: type, category):
        raise NotImplemented

    def cell(self, type_cls):
        # self._define(type_cls, )
        raise NotImplemented

    def struct(self, type_cls):
        raise NotImplemented

    def use_list(self, type_cls):
        raise NotImplemented
