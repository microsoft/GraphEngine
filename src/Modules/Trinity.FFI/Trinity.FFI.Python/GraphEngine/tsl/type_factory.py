from .type_map import *
from .type_sys import *
from .method_export import tsl_generate_methods
from ..utils import raise_exc, ImmutableDict
from ..err import StateError
from ..DotNet.env import Env, build_module

from typing import Dict, Union, List, Type, TypeVar, cast as typing_cast
from Redy.Magic.Classic import execute

TSLExplicitDefinitionType = Union[TSLCell, TSLStruct]

T = TypeVar('T')


class TSL:
    _default_num = 0

    def __init__(self, namespace=None):
        if namespace is None:
            namespace = f'default{self._default_num}'
            TSL._default_num += 1

        self._namespace = namespace
        self.type_specs_to_type: Dict[TSLTypeSpec, type] = {}

        self._tsl_explicit_definitions: 'Dict[str, Type[TSLExplicitDefinitionType]]' = {}
        self._module = None
        self._compiled = False
        self._inferred = False
        self._list_type_id_enum = 0

    @property
    def module(self):
        return self._module

    @property
    def compiled(self):
        return self._compiled

    @property
    def inferred(self):
        return self._inferred

    @property
    def namespace(self):
        return self._namespace

    def _define(self, cls_def, category=Type[Union[TSLStruct, TSLCell]]):
        if self._compiled:
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
                        return self._tsl_explicit_definitions[py_type].get_spec()
                    else:
                        return type_map_spec(py_type)

                tsl_type_spec: TSLTypeSpec
                if isinstance(tsl_type_spec, ListSpec):
                    if tsl_type_spec not in self.type_specs_to_type:
                        lst_type = type(f'List{self._list_type_id_enum}',
                                        (TSLList,),
                                        {'__slots__': ['__accessor__']})

                        def make_get_spec(it):
                            return lambda: it

                        lst_type.get_spec = make_get_spec(tsl_type_spec)
                        self.type_specs_to_type[tsl_type_spec] = lst_type

                yield (field_name, tsl_type_spec)

        type_spec = (CellSpec if category is TSLCell else
                     StructSpec if category is TSLStruct else
                     raise_exc(TypeError))

        @const_return
        def get_spec():
            return type_spec(cls_def.__name__, ImmutableDict(_()))

        new_cls.get_spec = get_spec

        if new_cls.__name__ in self._tsl_explicit_definitions:
            # Forbidden redefinitions/definitions of same names.
            raise TypeError(f"Tsl type named {new_cls.__name__} has been used.")

        self._tsl_explicit_definitions[new_cls.__name__] = new_cls
        # do not add explicit types(struct, cell) to `type_specs_to_type` now,
        # it would be done at `self.bind()`

        return new_cls

    def cell(self, cls_def):
        return self._define(cls_def, TSLCell)

    def struct(self, cls_def):
        return self._define(cls_def, TSLStruct)

    def use_list(self, cls_def: T) -> T:
        """
        provide an api for list constructing and dynamically type checking.
        >>> @tsl.use_list
        >>> class MyList(List[int]):
        >>>     pass
        """
        lst_type = cls_def.__orig_bases__[0]

        return self.type_specs_to_type[type_map_spec(lst_type)]

    def __str__(self):
        return f"TSL module[{self._namespace}] with {','.join(map(lambda _: _.__name__, self.type_specs_to_type.values()))}"

    @property
    def to_tsl(self):
        return '\n'.join(str(typedef.get_spec()) for _, typedef in self._tsl_explicit_definitions.items())

    def bind(self) -> None:
        """
        TODO: I don't know how to name it exactly.
        Action:
            generate, build and load tsl scripts, and generate methods by using Trinity.FFI.Metagen and swig builder.
        """
        if not self._inferred:
            for _, typ in self._tsl_explicit_definitions.items():
                self.type_specs_to_type[typ.get_spec()] = typ
            self._inferred = True

        self._module = build_module(self.to_tsl, self._namespace)

        for type in self.type_specs_to_type.values():
            tsl_generate_methods(self, type)
