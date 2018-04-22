from .type_spec import TSLTypeSpec
from typing import Callable
from abc import ABC


class TSLObject(ABC):
    get_spec: Callable[[], TSLTypeSpec]
    __accessor__: object


class TSLCell(TSLObject):
    pass


class TSLStruct(TSLObject):
    pass


class TSLList(TSLObject):
    pass
