from .type_spec import TSLTypeSpec
from typing import Callable


class TSLObject:
    get_spec: Callable[[], TSLTypeSpec]
    __accessor__: object

    def __init__(self, accessor):
        raise NotImplemented

    def ref(accessor):
        raise NotImplemented

    def deepcopy(self):
        raise NotImplemented


class TSLStruct(TSLObject):
    pass


class TSLCell(TSLStruct):
    pass


class TSLList(TSLObject):
    pass
