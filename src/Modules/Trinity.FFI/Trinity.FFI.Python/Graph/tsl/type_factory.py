from .type_map import *
from .type_sys import *
from ..utils import raise_exc, ImmutableDict
from typing import Dict, Union, List, Type
from Redy.Magic.Classic import execute

TSLExplicitDefinitionType = Union[TSLCell, TSLStruct]


class TSL:
    def __init__(self):
        self.tsl_explicit_definitions: 'Dict[str, TSLExplicitDefinitionType]' = {}
        self._total_type_specs = set()

    def _define(self, cls_def, category=Type[Union[TSLStruct, TSLCell]]):
        __annotations__ = cls_def.__annotations__

        def _():
            for field_name, py_type in __annotations__.items():

                @execute
                def tsl_type_spec() -> TSLTypeSpec:
                    if isinstance(py_type, str):
                        return self.tsl_explicit_definitions[py_type].get_spec()
                    else:
                        return type_map_spec(py_type)

                tsl_type_spec: TSLTypeSpec
                if isinstance(tsl_type_spec, ListSpec):
                    self._total_type_specs.add(tsl_type_spec)

                yield (field_name, tsl_type_spec)

        new_cls: TSLObject = type(cls_def.__name__,
                                  (TSLCell, *cls_def.__bases__),
                                  dict(cls_def.__dict__, __slots__=['__accessor__']))

        type_spec = (CellSpec if category is TSLCell else
                     StructSpec if category is TSLStruct else
                     raise_exc(TypeError))

        @const_return
        def get_spec():
            return type_spec(cls_def.__name__, ImmutableDict(_()))

        new_cls.get_spec = get_spec
        self.tsl_explicit_definitions[new_cls.__name__] = new_cls

        return new_cls

    def cell(self, cls_def):
        return self._define(cls_def, TSLCell)

    def struct(self, cls_def):
        return self._define(cls_def, TSLStruct)
