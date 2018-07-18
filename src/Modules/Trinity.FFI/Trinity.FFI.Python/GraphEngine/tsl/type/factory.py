import typing

from GraphEngine.tsl.type.system import Struct, Cell, List, type_map_spec, TSLType
from GraphEngine.tsl.type.method_export import make_class
from GraphEngine.DotNet.env import Env, build_module


class _AttrGetProxy:
    def __init__(self, obj):
        self._ = obj

    def __getitem__(self, item):
        return getattr(self._, item)


class TSL:
    _default_num = 0

    def __init__(self, namespace=None):
        if namespace:
            self._namespace = namespace
        else:
            self._namespace = f'default.{TSL._default_num}'
            TSL._default_num += 1
        self._root_types = {}
        self._module = None

    def __call__(self, cls_def: type):
        if isinstance(cls_def, list):
            tp = getattr(cls_def, ' __orig_bases__', None)
            if not tp:
                raise TypeError("Require a generic list type like `class MyLi(typing.List[T]): ...`")
            lst_type = tp[0]
            spec = type_map_spec(lst_type)
            cls_def.__ty_spec__ = spec
        self._root_types[repr(cls_def)] = cls_def
        if self._module:
            cls: typing.Type[TSLType] = cls_def
            make_class(cls, self._module, self._root_types)
        return cls_def

    @property
    def to_tsl(self):
        return '\n'.join(
                str(typedef.get_spec()) for typedef in self._root_types.values() if not isinstance(typedef, list))

    def bind(self):
        self._module = _AttrGetProxy(build_module(self.to_tsl, self._namespace))
        for each in self._root_types:
            make_class(each, self._module, self._root_types)


if __name__ == '__main__':
    tsl = TSL()


    @tsl
    class C(Cell):
        a: int


    print(tsl.to_tsl)
