"""
@tsl.struct
class S:
    a: int

@tsl.cell
class C:
    s: S
    x: A

c : C

it = get(c.s)

it = c.s._


c.s[0]


for each in c.ls:
    result = tsl.apply(f, each)
    tsl.apply(print, result)
    tsl.assign(x.__setitem__, 0, result)

tsl.execute
"""
from GraphEngine.tsl.type.system import *
from GraphEngine.tsl.type.mangling import *
from Redy.ADT.Core import data
from Redy.ADT import traits
from Redy.Opt import feature, const, constexpr, Macro

_internal_macro = Macro()
import ast

staging = (const, constexpr)


def type_spec_to_name(ty: TypeSpec) -> str:
    if isinstance(ty, CellTypeSpec):
        return 'cell_{}'.format(mangling(repr(ty)))

    elif isinstance(ty, StructTypeSpec):
        return 'struct_{}'.format(mangling(repr(ty)))

    elif isinstance(ty, ListTypeSpec):
        elem_str = type_spec_to_name(ty.elem_type)
        return f'list_{elem_str}'

    elif isinstance(ty, PrimitiveTypeSpec):
        return ty.native_name


@data
class LinkedList(traits.ConsInd, traits.Im, traits.Dense):
    Nil: ...
    Cons: lambda head, tail: ...

    def __str__(self):
        if self is LinkedList.Nil:
            return '[]'

        def stream():
            root = self
            _, a, root = root
            while True:
                yield a
                if root is LinkedList.Nil:
                    break

                _, a, root = root

        return '[{}]'.format(', '.join(map(str, stream())))

    @property
    def length(self):
        root = self
        n = 0
        while root is not LinkedList.Nil:
            n += 1
            _, _, root = root
        return n

    def split(self):
        _, head, tail = self
        return head, tail


Nil: LinkedList = LinkedList.Nil
Cons: typing.Callable[[object, LinkedList], LinkedList] = LinkedList.Cons


@record
class State:
    spec_dict: dict  # str -> spec, forward_ref
    method_tb: dict  # str -> jit method
    arg_num: int
    non_primitive_count: int

    def update(self, **kwargs):
        return State(*(kwargs[k] if k in kwargs else getattr(self, k) for k in self.__annotations__))


def take_args(args):
    while args:
        a, args = args
        yield a


@Pattern
def create_cls(spec: TypeSpec, chain: str, state: State):
    return type(spec)


@create_cls.case(StructTypeSpec)
def create_cls(spec: StructTypeSpec, chain: str, state: State):
    bases = (Struct, Proxy)
    cls = type(repr(spec), bases, {'__slots__': ('__accessor__', '__args__')})
    fields = spec.field_types.items()
    non_primitive_count = 0

    @feature(staging)
    def ref_get(self):
        method: const = state.method_tb[chain]
        if constexpr[state.arg_num]:
            return method(*constexpr[take_args](self.__args__), self.__accessor__)
        else:
            return method(self.__accessor__)

    cls.ref_get = ref_get

    for field_name, field_spec in fields:
        field_name_mangled = mangling(field_name)
        # very javascript here
        if isinstance(field_spec, PrimitiveTypeSpec):
            @property
            @feature(staging)
            def get_method(self):
                method: const = state.method_tb[f'{chain}_SGet_{field_name_mangled}_BGet']
                if constexpr[state.arg_num]:
                    return method(*constexpr[take_args](self.__args__), self.__accessor__)
                else:
                    return method(self.__accessor__)

            @get_method.setter
            def set_method(self, value):
                method: const = state.method_tb[f'{chain}_SSet_{field_name_mangled}']
                if constexpr[state.arg_num]:
                    method(*constexpr[take_args](self.__args__), self.__accessor__)
                else:
                    method(self.__accessor__)

        else:
            chaining_getter = f'{chain}_SGet'
            field_cls = create_cls(field_spec, chaining_getter, state)

            _internal_macro.expr(f"def get_specific_field():\n"
                                 f"   return self.__{non_primitive_count}")

            _internal_macro.stmt('def set_specific_field():\n'
                                 f'    v = self.__{non_primitive_count}'
                                 f' = constexpr[field_cls](self.__accessor__, self.__args__)')
            _internal_macro.stmt('def return_proxy():\n'
                                 f'    return v')

            get_specific_field: _internal_macro
            set_specific_field: _internal_macro
            return_proxy: _internal_macro

            @property
            @feature(_internal_macro, staging)
            def get_method(self):
                try:
                    return get_specific_field()
                except AttributeError:
                    set_specific_field()
                    return_proxy()

            @get_method.setter
            @feature(_internal_macro, staging)
            def set_method(self, value):
                method: const = state.method_tb[f'{chain}_SSet']
                if constexpr[state.arg_num]:
                    method(value.ref_get(), *constexpr[take_args](self.__args__), self.__accessor__)
                else:
                    method(value.ref_get(), self.__accessor__)

            @feature(staging)
            def ref_set(self, value):
                method: const = state.method_tb[f'{chain}_SSet']
                if constexpr[state.arg_num]:
                    method(value.ref_get(), *constexpr[take_args](self.__args__), self.__accessor__)
                else:
                    method(value.ref_get(), self.__accessor__)

            field_cls.ref_set = ref_set
            non_primitive_count += 1
        setattr(cls, field_name, set_method)

    def __init__(self, acc, args):
        self.__accessor__ = acc
        self.__args__ = args

    cls.__init__ = __init__

    cls.__slots__ += tuple(f'__{i}' for i in range(non_primitive_count))
    return cls


@create_cls.case(ListTypeSpec)
def create_cls(spec: ListTypeSpec, chain: str, state: State):
    bases = (List, Proxy)
    cls = type(repr(spec), bases, {'__slots__': ('__accessor__', '__args__')})
    elem = spec.elem_type

    @feature(staging)
    def ref_get(self):
        method: const = state.method_tb[chain]
        if constexpr[state.arg_num]:
            return method(*constexpr[take_args](self.__args__), self.__accessor__)
        else:
            return method(self.__accessor__)

    cls.ref_get = ref_get

    if isinstance(elem, PrimitiveTypeSpec):
        @feature(staging)
        def __getitem__(self, idx):
            method: const = state.method_tb[f'{chain}_LGet_BGet']
            if constexpr[state.arg_num]:
                return method(idx, *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(idx, self.__accessor__)

        @feature(staging)
        def __setitem__(self, idx, value):
            method: const = state.method_tb[f'{chain}_LSet']
            if constexpr[state.arg_num]:
                return method(value, idx, *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value, idx, self.__accessor__)

        @feature(staging)
        def __contains__(self, value):
            method: const = state.method_tb[f'{chain}_LContains']
            if constexpr[state.arg_num]:
                return method(value, *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value, self.__accessor__)

        @feature(staging)
        def insert_at(self, idx, value):
            method: const = state.method_tb[f'{chain}_LInsertAt']
            if constexpr[state.arg_num]:
                return method(value, idx, *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value, idx, self.__accessor__)

    else:
        chaining_getter = f'{chain}_LGet'
        elem_cls = create_cls(elem, chaining_getter, state.update(arg_num=state.arg_num + 1))

        @feature(staging)
        def ref_set(self, value):
            method: const = state.method_tb[f'{chain}_LSet']
            if constexpr[state.arg_num + 1]:
                method(value.ref_get(), *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                method(value.ref_get(), self.__accessor__)

        elem_cls.ref_set = ref_set

        @feature(staging)
        def __getitem__(self, idx):
            proxy_cls: const = elem_cls
            return proxy_cls(self.__accessor__, [idx, self.__args__])

        @feature(staging)
        def __setitem__(self, idx, value):
            method: const = state.method_tb[f'{chain}_LSet']
            if constexpr[state.arg_num]:
                return method(value.ref_get(), idx, *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value.ref_get(), idx, self.__accessor__)

        @feature(staging)
        def __contains__(self, value):
            method: const = state.method_tb[f'{chain}_LContains']
            if constexpr[state.arg_num]:
                return method(value.ref_get(), *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value.ref_get(), self.__accessor__)

        @feature(staging)
        def insert_at(self, idx, value):
            method: const = state.method_tb[f'{chain}_LInsertAt']
            if constexpr[state.arg_num]:
                return method(value.ref_get(), idx, *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value.ref_get(), idx, self.__accessor__)

    @feature(staging)
    def __delitem__(self, idx):
        method: const = state.method_tb[f'{chain}_LRemoveAt']
        if constexpr[state.arg_num]:
            method(idx, *constexpr[take_args](self.__args__), self.__accessor__)
        else:
            method(idx, self.__accessor__)

    @feature(staging)
    def __len__(self):
        method: const = state.method_tb[f'{chain}_LCount']
        if constexpr[state.arg_num]:
            return method(*constexpr[take_args](self.__args__), self.__accessor__)
        else:
            return method(self.__accessor__)

    @feature(staging)
    def __init__(self, acc, args):
        self.__args__ = args
        self.__accessor__ = acc

    cls.__init__ = __init__
    cls.insert = insert_at
    cls.__contains__ = __contains__
    cls.__len__ = __len__
    cls.__delitem__ = __delitem__
    cls.__setitem__ = __setitem__
    cls.__getitem__ = __getitem__
