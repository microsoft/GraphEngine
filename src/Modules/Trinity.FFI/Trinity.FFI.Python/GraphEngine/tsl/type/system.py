import abc
import numpy as np
import typing
from GraphEngine.tsl.type.spec import *
from Redy.Magic.Classic import discrete_cache, execute, const_return, singleton
from Redy.Magic.Pattern import Pattern
from GraphEngine.tsl.type._system import NotDefinedYet

T = typing.TypeVar('T')


def _remove_bases(cls: type):
    return type(cls.__name__, tuple(each for each in cls.__bases__ if each is not TSLType), {**cls.__dict__})


class TSLTypeMeta(type):
    def __instancecheck__(self, instance):
        return isinstance(instance, (Struct, List))

    def __subclasscheck__(self, subclass):
        return issubclass(subclass, (Struct, List))


class TSLType(metaclass=TSLTypeMeta):
    __accessor__: object
    __args__ = None

    @classmethod
    @abc.abstractmethod
    def get_spec(cls):
        raise NotImplemented

    def ref_set(self, value: 'TSLType'):
        raise NotImplemented

    def ref_get(self):
        raise NotImplemented


@_remove_bases
class Struct(TSLType):
    __ty_spec__: object
    __accessor__: object
    __args__ = None

    @classmethod
    @discrete_cache
    def get_spec(cls):
        annotations: dict = getattr(cls, '__annotations__', {})
        return StructTypeSpec(cls.__name__, ImmutableDict((k, type_map_spec(v)) for k, v in annotations.items()))


@_remove_bases
class Cell(Struct, TSLType):
    """
    All methods here except `get_spec` and `access` is only for type hinting.
    """

    @classmethod
    def load(cls: typing.Type['_TCell'], cell_id: int) -> '_TCell':
        raise NotImplemented

    @classmethod
    def use(cls: typing.Type['_TCell'], cell_id: int, cell_access_options: int) -> '_TCell':
        return NotImplemented

    @staticmethod
    def access(cell_id: int, cell_access_options: int) -> 'FutureAccessor':
        raise NotImplemented

    def save(self):
        raise NotImplemented

    def release(self):
        raise NotImplemented

    def __enter__(self: '_TCell') -> '_TCell':
        raise NotImplemented

    def __exit__(self, exc_type, exc_val, exc_tb):
        raise NotImplemented

    @classmethod
    @discrete_cache
    def get_spec(cls):
        annotations: dict = getattr(cls, '__annotations__', {})
        # noinspection PyArgumentList
        return CellTypeSpec(cls.__name__, ImmutableDict((k, type_map_spec(v)) for k, v in annotations.items()))


class List(typing.List[T]):
    __ty_spec__: object
    __accessor__: object
    __args__ = None
    __garg__: type

    @classmethod
    def get_spec(cls):
        return cls.__ty_spec__


TSLType: typing.Type[typing.Union[List, Struct, Cell]]


@execute
def __deep_dark_fantasy__():
    from GraphEngine.tsl.type._system import List
    globals()['List'] = List


class Proxy:
    __arg_num__: int
    __args__: tuple
    __proxy__: tuple

    def __init__(self, acc, proxy_args):
        raise NotImplemented


_TCell = typing.TypeVar('_TCell', bound='Cell')

_Generic = typing.Generic[T]


class _CellAccessorMeta(type):
    def __getitem__(self, item):
        return self


class FutureAccessor(metaclass=_CellAccessorMeta):
    """
    This means the real accessor in Trinity Storage
    """
    __slots__ = ('_cell', 'cell_id', 'cell_ops', 'ty')

    def __init__(self, cell_id: int, cell_ops: int, ty: typing.Type[_TCell]):
        self.cell_id = cell_id
        self.cell_ops = cell_ops
        self.ty: typing.Type[_TCell] = ty

    def __enter__(self) -> _TCell:
        self._cell = cell = self.ty.use(self.cell_id, self.cell_ops)
        cell.__enter__()
        return cell

    def __exit__(self, exc_type, exc_val, exc_tb):
        return self._cell.__exit__(exc_type, exc_val, exc_tb)


# noinspection PyTypeChecker


@Pattern
def type_map_spec(ty):
    # noinspection PyUnresolvedReferences,PyProtectedMember
    if isinstance(ty, str):
        return "not_defined_yet"
    elif issubclass(ty, List):
        return List
    elif issubclass(ty, TSLType):
        return 'established_ty'
    elif isinstance(ty, NotDefinedYet):
        return None
    return ty


@type_map_spec.case.ret_pattern(chr)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("char", 'CAHR')


@type_map_spec.case.ret_pattern("not_defined_yet")
@const_return
def type_map_spec(ty: str):
    return NotDefinedYet(ty)


@type_map_spec.case.ret_pattern(np.int8)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("int8", 'I8')


@type_map_spec.case.ret_pattern(np.int16)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("int16", 'I16')


@type_map_spec.case.ret_pattern(int)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("int32", 'I32')


@type_map_spec.case.ret_pattern(np.int32)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("int32", 'I32')


@type_map_spec.case.ret_pattern(np.int64)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("int64", 'I64')


@type_map_spec.case.ret_pattern(str)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec('string', 'STRING')


@type_map_spec.case.ret_pattern(bytes)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec('byte[]', 'U8STRING')


@type_map_spec.case.ret_pattern(bool)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec('bool', 'BOOL')


@type_map_spec.case.ret_pattern(float)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("double", 'DOUBLE')


@type_map_spec.case(List)
def type_map_spec(typ: List):
    # noinspection PyUnresolvedReferences
    return ListTypeSpec(type_map_spec(typ.__garg__))


@type_map_spec.case('established_ty')
def type_map_spec(typ: TSLType):
    return typ.get_spec()


@type_map_spec.case(None)
def type_map_spec(typ):
    return typ


if __name__ == '__main__':
    class A(Struct):
        a: int


    class S(Cell):
        i: int
        a: 'A'
        c: List[List[A]]


    class L(List[A]):
        pass


    print(S.get_spec(), A.get_spec(), sep='\n')
