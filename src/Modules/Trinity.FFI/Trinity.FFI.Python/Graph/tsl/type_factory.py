from .type_map import *
from .type_sys import *
from .method_export import tsl_generate_methods
from ..utils import raise_exc, ImmutableDict
from ..err import StateError

from typing import Dict, Union, List, Type, Set
from Redy.Magic.Classic import execute

TSLExplicitDefinitionType = Union[TSLCell, TSLStruct]


class TSL:
    def __init__(self):
        self.tsl_explicit_definitions: 'Dict[str, Type[TSLExplicitDefinitionType]]' = {}
        self.type_specs_to_type: Dict[TSLTypeSpec, type] = {}
        self.module = None
        self.compiled = False
        self._list_type_id_enum = 0

    def _define(self, cls_def, category=Type[Union[TSLStruct, TSLCell]]):
        if self.compiled:
            raise StateError("This tsl module has been generated, compiled and built yet.")

        __annotations__ = cls_def.__annotations__

        def ref(accessor):
            new = new_cls.__new__(new_cls)
            new.__accessor__ = accessor
            return new

        new_cls: Type[TSLExplicitDefinitionType] = type(cls_def.__name__,
                                                        (TSLCell, *cls_def.__bases__),
                                                        dict(cls_def.__dict__, __slots__=['__accessor__'], ref=ref))

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
                    if tsl_type_spec not in self.type_specs_to_type:
                        lst_type = type(f'List{self._list_type_id_enum}',
                                        (TSLList,),
                                        {'__slots__': ['__accessor__']})
                        lst_type.get_spec = lambda: tsl_type_spec
                        self.type_specs_to_type[tsl_type_spec] = lst_type

                yield (field_name, tsl_type_spec)

        type_spec = (CellSpec if category is TSLCell else
                     StructSpec if category is TSLStruct else
                     raise_exc(TypeError))

        @const_return
        def get_spec():
            return type_spec(cls_def.__name__, ImmutableDict(_()))

        new_cls.get_spec = get_spec

        if new_cls.__name__ in self.tsl_explicit_definitions:
            # Forbidden redefinitions/definitions of same names.
            raise TypeError(f"Tsl type named {new_cls.__name__} has been used.")

        self.tsl_explicit_definitions[new_cls.__name__] = new_cls

        return new_cls

    def cell(self, cls_def):
        return self._define(cls_def, TSLCell)

    def struct(self, cls_def):
        return self._define(cls_def, TSLStruct)

    def use_list(self, cls_def):
        """
        provide an api for list constructing and dynamically type checking.
        >>> @tsl.use_list
        >>> class MyList(List[int]):
        >>>     pass
        """
        lst_type = cls_def.__bases__[0]
        return self.type_specs_to_type[type_map_spec(lst_type)]

    @property
    def to_tsl(self):
        return '\n'.join(str(typedef.get_spec()) for _, typedef in self.tsl_explicit_definitions.items())

    def fly(self) -> None:
        """
        TODO: I don't know how to name this function exactly.
        Action:
            generate, build and load tsl scripts, and generate methods by using Trinity.FFI.Metagen and swig builder.
        """
        for _, typ in self.tsl_explicit_definitions.items():
            self.type_specs_to_type[typ.get_spec()] = typ

        module = self.module = tsl_build_src_code(self.to_tsl)
        for name, typ in self.tsl_explicit_definitions.items():
            tsl_generate_methods(self, typ)

        for lst_type in self.type_specs_to_type:
            tsl_generate_methods(self, lst_type)
