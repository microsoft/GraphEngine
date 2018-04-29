from abc import abstractmethod, ABC
from typing import Dict, Tuple
from ..utils import record, ImmutableDict

isa = isinstance


def _field_to_str(kv: 'Tuple[str, TSLTypeSpec]'):
    field_name, field_type = kv
    return f"{field_type.name if isinstance(field_type, StructSpec) else field_type} {field_name};"


class TSLTypeSpec(ABC):

    @abstractmethod
    def __str__(self):
        pass


@record
class PrimitiveSpec(TSLTypeSpec):
    tsl_name: str
    py_name: str

    def __str__(self):
        return f'{self.tsl_name}'


@record
class ListSpec(TSLTypeSpec):
    elem_type: TSLTypeSpec

    def __str__(self):
        return f'List<{(lambda _: _.name if isinstance(_, StructSpec) else _)(self.elem_type)}>'


@record
class StructSpec(TSLTypeSpec):
    """
    >>> print(StructSpec("S", {"i": PrimitiveSpec("int", "int")}))
    """
    name: str
    fields: ImmutableDict[str, TSLTypeSpec]

    def __str__(self):
        n = '\n'

        return f'struct {self.name}\n{{\n{n.join(map(_field_to_str, self.fields.items()))}\n}}'


@record
class CellSpec(StructSpec):
    name: str
    fields: ImmutableDict[str, TSLTypeSpec]

    def __str__(self):
        n = '\n'
        return f'cell {self.name}\n{{\n{n.join(map(_field_to_str, self.fields.items()))}\n}}'


if __name__ == '__main__':
    C1 = CellSpec(
        "C1",
        {"foo": PrimitiveSpec("int", int)}
    )
