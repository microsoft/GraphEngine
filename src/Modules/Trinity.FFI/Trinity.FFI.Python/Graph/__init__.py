"""
new module of graphengine. would be renamed to `GraphEngine` after finished.
"""
from abc import abstractmethod, ABC
from typing import *


class ICellDescriptors(ABC):
    pass


class IStorageSchema(ABC):

    # noinspection PyPep8Naming
    @abstractmethod
    def GetCellDescriptors(self) -> List[ICellDescriptors]:
        raise NotImplemented


def load_tsl_extension(path, namespace) -> IStorageSchema:
    pass
