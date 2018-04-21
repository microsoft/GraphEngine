from abc import abstractmethod, ABC
from typing import Dict, Tuple
from Graph.utils import Record

isa = isinstance


def _field_to_str(kv: 'Tuple[str, TSLTypeSpec]'):
    field_name, field_type = kv
    return f"{field_type} {field_name};"


class TSLTypeSpec(ABC):

    @abstractmethod
    def __str__(self):
        pass


@Record
class PrimitiveSpec(TSLTypeSpec):
    tsl_name: str
    py_name: str

    def __str__(self):
        return f'{self.tsl_name}'


@Record
class ListSpec(TSLTypeSpec):
    elem_type: TSLTypeSpec

    def __str__(self):
        return f'List<{self.elem_type}>'


@Record
class StructSpec(TSLTypeSpec):
    """
    print(StructSpec("S", {"i": PrimitiveSpec("int", "int")}))
    """
    name: str
    fields: Dict[str, TSLTypeSpec]

    def __str__(self):
        n = '\n'

        return f'struct {self.name}{{ \n{n.join(map(_field_to_str, self.fields.items()))} \n}}'


@Record
class CellSpec(StructSpec):
    def __str__(self):
        n = '\n'

        return f'cell {self.name}{{ \n{n.join(map(_field_to_str, self.fields.items()))} \n}}'
