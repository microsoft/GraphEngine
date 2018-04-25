from .type_spec import TSLTypeSpec
from typing import Callable
from abc import ABC, abstractmethod


class TSLObject(ABC):
    get_spec: Callable[[], TSLTypeSpec]
    __accessor__: object

    @staticmethod
    @abstractmethod
    def ref(accessor):
        raise NotImplemented

    @staticmethod
    @abstractmethod
    def deepcopy(self):
        raise NotImplemented


class TSLCell(TSLObject):
    pass


class TSLStruct(TSLObject):
    pass


class TSLList(TSLObject):
    pass
