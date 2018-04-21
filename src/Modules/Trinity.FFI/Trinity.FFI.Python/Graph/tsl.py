from typing import NamedTuple, Tuple, List, Any, Callable, Dict, Union
import typing
from abc import ABC, abstractmethod
from Redy.Collections import Flow, Traversal
from Redy.Tools.TypeInterface import Module
from .tsl.type_spec import *


class TSLObject(ABC):
    spec: TSLTypeSpec
    __accessor__: object


class TSLCell(TSLObject):
    pass


class TSLStruct(TSLObject):
    pass


class TSLList(TSLObject):
    pass


class TSL:
    """
    >>> tsl = TSL()

    >>> @tsl.cell
    >>> class S:
    >>>     a: List[List[int]]
    >>>     b: str
    >>>     c: float

    >>> print(tsl.tsl_definitions)
    take care that you couldn't use a field named `spec`
    """

    def __init__(self):
        self.tsl_definitions: 'Dict[str, Union[TSLCell, TSLStruct, TSLList]]' = {}
        self.module = None
        self._total_type_names = set()  # typenames of all types manipulated in current tsl module.

    def cell(self, cls: type):
        __annotations__ = cls.__annotations__

        def _():
            for field_name, py_type in __annotations__.items():
                tsl_type = tsl_type_map(py_type)
                yield (field_name, tsl_type)

        new_cls: TSLObject = type(cls.__name__,
                                  (TSLCell, *cls.__bases__),
                                  dict(cls.__dict__, __slots__=['__accessor__']))

        new_cls.spec = CellSpec(cls.__name__, dict(_()))

        self.tsl_definitions[new_cls.__name__] = new_cls

        return new_cls

    def struct(self, cls: type):
        __annotations__ = cls.__annotations__

        def _():
            for field_name, py_type in __annotations__.items():
                tsl_type = tsl_type_map(py_type)
                yield (field_name, tsl_type)

        new_cls: TSLObject = type(cls.__name__,
                                  (TSLStruct, *cls.__bases__),
                                  dict(cls.__dict__, __slots__=['__accessor__'])
                                  )

        new_cls.spec = StructSpec(cls.__name__, dict(_()))

        self.tsl_definitions[new_cls.__name__] = new_cls

        return new_cls

    def _jit(self) -> 'Module':
        raise NotImplemented

    def jit(self) -> None:
        self.module = module = self._jit()
        for typename, each_type in self.tsl_definitions.items():
            binding_methods_from_module_to_class(class_definition=each_type, module=module)


def binding_methods_from_module_to_class(class_definition: typing.Union[TSLStruct], module: Module):
    class_name = class_definition.__name__

    class_definition.__init__ = get_init_fn(module, class_name)

    struct_spec: typing.Union[TSLStructSpec, TSLCellSpec] = class_definition.spec

    for each in struct_spec.fields:
        _setter, _getter = get_setter_getter_fn(module, each.name)

        if is_primitive(each.type):
            @property
            def getter(self: TSLObject) -> each.type:
                return _getter(self.__accessor__)
        else:

            # if this field is not of primitive type, the return of `Jit` is of type `void *`,
            # so wrap it as an object to represent.
            object_wrapper = get_wrapper(each.type)

            @property
            def getter(self: TSLObject) -> each.type:
                return object_wrapper(_getter(self.__accessor__))

        @getter.setter
        def setter(self: TSLObject, value: each.type):
            _setter(self.__accessor__, value)

        setattr(class_definition, each.name, setter)


def tsl_typename_map(py_type: type):
    """
    you can completely use python type objects to define a tsl cell/struct.
        a: List[int]
    and you can also use tsl grammar by typing with a string.
        a: 'List<int>'
    what's more, you can even mix them up:
        a: List['int']

    however, do not mix them up in one string:
        a: 'List<int>'
    >>> tsl = TSL()
    >>>
    >>> @tsl.cell
    >>> class S:
    >>>    a: List['S']
    >>>    b: 'List<string>'
    >>>    c: List[int]
    >>> print([each.spec for each in tsl.tsl_definitions.values()])

    """
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

    # noinspection PyUnresolvedReferences
    if isinstance(typ, typing._ForwardRef):
        # noinspection PyUnresolvedReferences
        return tsl_typename_map(typ.__forward_arg__)

    if isinstance(typ, (TSLCell, TSLStruct)):
        return typ.__name__

    def is_tuple(x) -> bool:
        return isinstance(x, tuple)

    def to_generic(generic, *insts):
        return f'{generic}<{", ".join(insts)}>'

    if issubclass(typ, List):
        # noinspection PyUnresolvedReferences
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
