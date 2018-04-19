from typing import NamedTuple, Tuple, List, Any, Callable
from abc import ABC, abstractmethod
from Redy.Magic.Pattern import Pattern
from Redy.Collections import Flow, Traversal


class TSLSpec: ...


class TSLObject(ABC):
    tsl_spec: TSLSpec


class TSLCell(TSLObject):
    pass


class TSLStruct(TSLObject):
    pass


class TslFieldSpec(TSLSpec, NamedTuple):
    name: str
    typename: str

    def __str__(self):
        return f"{self.typename} {self.name};"


class TSLStructSpec(TSLSpec, NamedTuple):
    name: str
    fields: Tuple[TslFieldSpec]

    def __str__(self):
        n = '\n'
        return f'struct {self.name}{{\n{n.join(map(str, self.fields))}\n}}'


class TSLCellSpec(TSLSpec, TSLStructSpec):

    def __str__(self):
        n = '\n'
        return f'cell {self.name}{{\n{n.join(map(str, self.fields))}\n}}'


class TSL:
    """
    >>> tsl = TSL()

    >>> @tsl.cell
    >>> class S:
    >>>     a: List[List[int]]
    >>>     b: str
    >>>     c: float

    >>> print(tsl.tsl_definitions)

    """

    def __init__(self):
        self.tsl_definitions: 'List[TSLObject]' = []

    def cell(self, cls: type):

        __annotations__ = cls.__annotations__

        def _():
            for field_name, py_type in __annotations__.items():
                tsl_typename = tsl_typename_map(py_type)
                yield TslFieldSpec(field_name, tsl_typename)

        new_cls = type(cls.__name__, (TSLCell, *cls.__bases__), dict(cls.__dict__))
        new_cls.tsl_spec = TSLCellSpec(cls.__name__, tuple(_()))
        self.tsl_definitions.append(new_cls)
        return new_cls

    def struct(self, cls: type):
        # noinspection PyDecorator

        __annotations__ = cls.__annotations__

        def _():
            for field_name, py_type in __annotations__.items():
                tsl_typename = tsl_typename_map(py_type)
                yield TslFieldSpec(field_name, tsl_typename)

        new_cls = type(cls.__name__, (TSLStruct, *cls.__bases__), dict(cls.__dict__))
        new_cls.tsl_spec = TSLStructSpec(cls.__name__, tuple(_()))
        self.tsl_definitions.append(new_cls)
        return new_cls


@Pattern
def tsl_typename_map(py_type: type):
    return py_type


@tsl_typename_map.match(int)
def tsl_typename_map(_):
    return 'int'


@tsl_typename_map.match(chr)
def tsl_typename_map(_):
    return 'char'


@tsl_typename_map.match(str)
def tsl_typename_map(_):
    return 'string'


@tsl_typename_map.match(float)
def tsl_typename_map(_):
    return 'double'


@tsl_typename_map.match(float)
def tsl_typename_map(_):
    return 'double'


@tsl_typename_map.match(any)
def tsl_typename_map(typ: type):
    if isinstance(typ, str):
        return typ

    if isinstance(typ, (TSLCell, TSLStruct)):
        return typ.__name__

    def is_tuple(x) -> bool:
        return isinstance(x, tuple)

    def to_generic(generic, *insts):
        return f'{generic}<{", ".join(insts)}>'

    if issubclass(typ, List):
        trees = typ._subs_tree()
        if not is_tuple(trees):
            return 'List'

        return Flow(trees)[
            Traversal.flatten_if(is_tuple)
        ][
            Traversal.map_by(tsl_typename_map)
        ][
            tuple
        ][
            reversed
        ][
            Traversal.reduce_by(lambda a, b: to_generic(b, a))
        ].unbox

    raise TypeError(f'Not supported type {typ.__name__}')
