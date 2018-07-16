from Redy.Magic.Classic import record
from Redy.Magic.Pattern import Pattern
from abc import abstractmethod, ABC
from typing import Dict, Tuple
from ..utils import ImmutableDict

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


def _generate_proxy_type_from_spec(spec: TSLTypeSpec, previous_chain):
    """
    class S:
        lst: List[int]
    class C:
        a : int
        s : S


    c : Root[C]
    c.a -> int
    c.s -> Proxy[C, S]
    c.s.lst -> Proxy[C, S, List[int]]
    c.s.lst.__getitem__(i) -> int

    Root[C]

    property a -> int
    property s -> Proxy[C, S]
        Proxy[C, S]
            property lst -> Proxy[C, S, List[int]]
                Proxy[C, S, List[int]]

                    method [i] -> int
                    bind cell_c_SGet_s_LGet_BGet

                    method [i] = value
                    bind cell_c_SGet_s_LSet

                    method count() -> int
                    bind cell_c_SGet_s_LCount

    :param spec:
    :param previous_chain:
    :return:
    """
    if isinstance(spec, StructSpec):

        spec.fields


if __name__ == '__main__':
    C1 = CellSpec("C1", {"foo": PrimitiveSpec("int", int)})
