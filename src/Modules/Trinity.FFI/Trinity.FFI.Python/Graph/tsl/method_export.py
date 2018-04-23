from .type_sys import *
from .type_map import *
from Redy.Tools.TypeInterface import Module
from Redy.Magic.Pattern import Pattern
from typing import Type


def tsl_build_src_code(code: str) -> Module:
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
