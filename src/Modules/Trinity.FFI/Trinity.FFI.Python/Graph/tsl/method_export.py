from .type_sys import *
from .type_map import *
from .type_verbs import *
from .mangling import mangling

from Redy.Tools.TypeInterface import Module
from Redy.Magic.Pattern import Pattern
from Redy.Magic.Classic import cast
from typing import Type, List, Tuple


def type_spec_to_name(typ: TSLTypeSpec) -> str:
    """
    consistent to Trinity.FFI.Metagen: MetaGen.`make'name`.
    """
    if isinstance(typ, ListSpec):
        return 'List{_}{elem}'.format(_=mangling_code, elem=type_spec_to_name(typ.elem_type))
    if isinstance(typ, CellSpec):
        return 'Cell{_}{name}'.format(_=mangling_code, name=typ.name)
    elif isinstance(typ, StructSpec):
        return 'Struct{_}{name}'.format(_=mangling_code, name=typ.name)
    else:
        return str(typ)


@Pattern
def get_verbs(spec: TSLTypeSpec) -> List[Tuple[str, List[Verb]]]:
    """
    get ffi function name from
    """
    # noinspection PyTypeChecker
    return type(spec)


@get_verbs.match(ListSpec)
def get_verbs(spec):
    typename = type_spec_to_name(spec)
    return [(list.__name__,
             [
                 LGet(typename),
                 LSet(typename),
                 LCount(typename),
                 LCotains(typename),
                 BGet(typename),
                 BSet(typename)
             ])]


@get_verbs.match(StructSpec)
@cast(list)
def get_verbs(spec: StructSpec):
    typename = type_spec_to_name(spec)
    for field_name, _ in spec.fields.items():
        field_name = mangling(field_name)
        yield (
            field_name,
            [
                SGet(typename, field_name),
                SSet(typename, field_name),
            ])
    yield from [BGet(typename), BGet(typename)]


def tsl_build_src_code(code: str) -> Module:
    raise NotImplemented


def typename_manging(typ: TSLObject) -> str:
    raise NotImplemented


@Pattern
def tsl_generate_methods(module, cls_def: Type[TSLObject]):
    if issubclass(cls_def, TSLStruct):
        return TSLStruct
    elif isinstance(cls_def, TSLList):
        return TSLList
    else:
        raise TypeError


@tsl_generate_methods.match(TSLStruct)
def tsl_generate_methods(module, cls_def):
    raise NotImplemented


@tsl_generate_methods.match(TSLList)
def tsl_generate_methods(module, cls_def):
    raise NotImplemented
