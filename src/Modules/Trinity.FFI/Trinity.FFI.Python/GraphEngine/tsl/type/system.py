import abc
import numpy as np
import typing
from GraphEngine.tsl.type.spec import *
from Redy.Magic.Classic import discrete_cache
from Redy.Magic.Pattern import Pattern
from Redy.Magic.Classic import const_return


class TSLType(abc.ABC):
    __accessor__: object
    __meta_cls__: type
    __ty_args__: typing.Tuple[type] = ()
    __proxy__: typing.Tuple[typing.Union['Proxy', 'TSLType']]

    @classmethod
    @abc.abstractmethod
    def get_spec(cls):
        raise NotImplemented

    def ref_set(self, value: 'TSLType'):
        raise NotImplemented

    def ref_get(self):
        raise NotImplemented


class Struct(TSLType):

    @classmethod
    @discrete_cache
    def get_spec(cls):
        annotations: dict = getattr(cls, '__annotations__', {})
        return StructTypeSpec(cls.__name__, ImmutableDict((k, type_map_spec(v)) for k, v in annotations.items()))


class Cell(Struct):
    pass


class List(TSLType):
    __ty_spec__: object

    @discrete_cache
    def get_spec(self):
        return self.__ty_spec__


class NotDefinedYet:
    def __init__(self, name):
        self.name = name

    def __repr__(self):
        return self.name

    def __str__(self):
        raise NotImplemented


class Proxy:
    __arg_num__: int
    __args__: tuple
    __proxy__: tuple

    def __init__(self, acc, proxy_args):
        raise NotImplemented


# noinspection PyTypeChecker


@Pattern
def type_map_spec(ty) -> TypeSpec:
    # noinspection PyUnresolvedReferences,PyProtectedMember
    if isinstance(ty, str):
        return "not_defined_yet"
    elif issubclass(ty, typing.List):
        return list
    elif issubclass(ty, TSLType):
        return 'established_ty'
    elif isinstance(ty, typing._ForwardRef):
        return None
    return ty


@type_map_spec.case.ret_pattern(chr)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("char", 'char')


@type_map_spec.case.ret_pattern("not_defined_yet")
@const_return
def type_map_spec(ty: str):
    return NotDefinedYet(ty)


@type_map_spec.case.ret_pattern(np.int8)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("int8", 'int8_t')


@type_map_spec.case.ret_pattern(np.int16)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("int16", 'int16_t')


@type_map_spec.case.ret_pattern(int)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("int32", 'int32_t')


@type_map_spec.case.ret_pattern(np.int32)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("int32", 'int32_t')


@type_map_spec.case.ret_pattern(np.int64)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("int64", 'int64_t')


@type_map_spec.case.ret_pattern(str)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec('string', 'wchar*')


@type_map_spec.case.ret_pattern(bytes)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec('u8string', 'char*')


@type_map_spec.case.ret_pattern(bool)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec('bool', 'bool')


@type_map_spec.case.ret_pattern(float)
@const_return
def type_map_spec(_):
    return PrimitiveTypeSpec("double", 'double')


def _generic_type_map(typ_tuple):
    if not isinstance(typ_tuple, tuple):
        return type_map_spec(typ_tuple)

    # only work for list generic
    return ListTypeSpec(_generic_type_map(typ_tuple[1]))


@type_map_spec.case(list)
def type_map_spec(typ: typing.List):
    # noinspection PyUnresolvedReferences
    trees = typ._subs_tree()
    if not isinstance(trees, tuple):
        raise TypeError("List without type params")
    return _generic_type_map(trees)


@type_map_spec.case('established_ty')
def type_map_spec(typ: TSLType):
    return typ.get_spec()


@type_map_spec.case(None)
def type_map_spec(typ):
    return NotDefinedYet(typ.__forward_arg__)


if __name__ == '__main__':
    class A(Struct):
        a: int


    class S(Struct):
        i: int
        a: 'A'
        c: typing.List[typing.List[A]]


    print(S.get_spec(), A.get_spec(), sep='\n')
