from GraphEngine.tsl.type.system import *
from GraphEngine.tsl.type.mangling import *
from Redy.Opt import feature, const, constexpr, Macro

_internal_macro = Macro()
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
        return ty.type_code


def take_args(args):
    while args:
        a, args = args
        yield a


@Pattern
def create_cls(spec: TypeSpec, chain: str, method_tb, arg_num):
    return type(spec)


@create_cls.case(StructTypeSpec)
def create_cls(spec: StructTypeSpec, chain: str, method_tb, arg_num):
    bases = (Struct, Proxy)
    cls = type(repr(spec), bases, {'__slots__': ('__accessor__', '__args__')})
    fields = spec.field_types.items()
    non_primitive_count = 0

    @feature(staging)
    def ref_get(self):
        method: const = method_tb[chain]
        if constexpr[arg_num]:
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
                method: const = method_tb[f'{chain}_SGet_{field_name_mangled}_BGet']
                if constexpr[arg_num]:
                    return method(*constexpr[take_args](self.__args__), self.__accessor__)
                else:
                    return method(self.__accessor__)

            @get_method.setter
            def set_method(self, value):
                method: const = method_tb[f'{chain}_SSet_{field_name_mangled}']
                if constexpr[arg_num]:
                    method(value, *constexpr[take_args](self.__args__), self.__accessor__)
                else:
                    method(value, self.__accessor__)

        else:
            chaining_getter = f'{chain}_SGet_{field_name_mangled}'
            field_cls = create_cls(field_spec, chaining_getter, method_tb, arg_num)
            print(field_cls.__slots__)

            _internal_macro.expr(f"def get_specific_field():\n"
                                 f"   return self.__{non_primitive_count}")

            _internal_macro.stmt('def set_specific_field():\n'
                                 f'    v = self.__{non_primitive_count}'
                                 f' = constexpr[field_type](self.__accessor__, self.__args__)')
            _internal_macro.stmt('def return_proxy():\n'
                                 f'    return v')

            get_specific_field: _internal_macro
            set_specific_field: _internal_macro
            return_proxy: _internal_macro

            @property
            @feature(_internal_macro, staging)
            def get_method(self):
                field_type: const = field_cls
                try:
                    return get_specific_field()
                except AttributeError:
                    set_specific_field()
                    return_proxy()

            @get_method.setter
            @feature(_internal_macro, staging)
            def set_method(self, value):
                method: const = method_tb[f'{chain}_SSet_{field_name_mangled}']
                if constexpr[arg_num]:
                    method(value.ref_get(), *constexpr[take_args](self.__args__), self.__accessor__)
                else:
                    method(value.ref_get(), self.__accessor__)

            @feature(staging)
            def ref_set(self, value):
                method: const = method_tb[f'{chain}_SSet_{field_name_mangled}']
                if constexpr[arg_num]:
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
def create_cls(spec: ListTypeSpec, chain: str, method_tb: dict, arg_num: int):
    bases = (List, Proxy)
    cls = type(repr(spec), bases, {'__slots__': ('__accessor__', '__args__')})
    elem = spec.elem_type

    @feature(staging)
    def ref_get(self):
        method: const = method_tb[chain]
        if constexpr[arg_num]:
            return method(*constexpr[take_args](self.__args__), self.__accessor__)
        else:
            return method(self.__accessor__)

    cls.ref_get = ref_get

    if isinstance(elem, PrimitiveTypeSpec):
        @feature(staging)
        def __getitem__(self, idx):
            method: const = method_tb[f'{chain}_LGet_BGet']
            if constexpr[arg_num]:
                return method(idx, *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(idx, self.__accessor__)

        @feature(staging)
        def __setitem__(self, idx, value):
            method: const = method_tb[f'{chain}_LSet']
            if constexpr[arg_num]:
                return method(value, idx, *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value, idx, self.__accessor__)

        @feature(staging)
        def __contains__(self, value):
            method: const = method_tb[f'{chain}_LContains']
            if constexpr[arg_num]:
                return method(value, *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value, self.__accessor__)

        @feature(staging)
        def insert_at(self, idx, value):
            method: const = method_tb[f'{chain}_LInsertAt']
            if constexpr[arg_num]:
                return method(value, idx, *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value, idx, self.__accessor__)

    else:
        chaining_getter = f'{chain}_LGet'
        elem_cls = create_cls(elem, chaining_getter, method_tb, arg_num + 1)

        @feature(staging)
        def ref_set(self, value):
            method: const = method_tb[f'{chain}_LSet']
            method(value.ref_get(), *constexpr[take_args](self.__args__), self.__accessor__)

        elem_cls.ref_set = ref_set

        @feature(staging)
        def __getitem__(self, idx):
            proxy_cls: const = elem_cls
            return proxy_cls(self.__accessor__, [idx, self.__args__])

        @feature(staging)
        def __setitem__(self, idx, value):
            method: const = method_tb[f'{chain}_LSet']
            if constexpr[arg_num]:
                return method(value.ref_get(), idx, *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value.ref_get(), idx, self.__accessor__)

        @feature(staging)
        def __contains__(self, value):
            method: const = method_tb[f'{chain}_LContains']
            if constexpr[arg_num]:
                return method(value.ref_get(), *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value.ref_get(), self.__accessor__)

        @feature(staging)
        def insert_at(self, idx, value):
            method: const = method_tb[f'{chain}_LInsertAt']
            if constexpr[arg_num]:
                return method(value.ref_get(), idx, *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value.ref_get(), idx, self.__accessor__)

    @feature(staging)
    def __delitem__(self, idx):
        method: const = method_tb[f'{chain}_LRemoveAt']
        if constexpr[arg_num]:
            method(idx, *constexpr[take_args](self.__args__), self.__accessor__)
        else:
            method(idx, self.__accessor__)

    @feature(staging)
    def __len__(self):
        method: const = method_tb[f'{chain}_LCount']
        if constexpr[arg_num]:
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


@Pattern
def make_class(ty: TSLType, method_tb):
    return issubclass(ty, Struct)


@make_class.case(True)
def make_class(ty: Struct, method_tb):
    spec: StructTypeSpec = ty.get_spec()
    ty_name_mangled = type_spec_to_name(spec)

    chain = ty_name_mangled

    fields = spec.field_types.items()
    non_primitive_count = 0

    @feature(staging)
    def ref_get(self):
        method: const = method_tb['Unbox']
        return method(self.__accessor_)

    ty.ref_get = ref_get

    @feature(staging)
    def ref_set(self, value):
        method: const = method_tb[f'{chain}_BSet']
        return method(self.__accessor__, value.ref_get())

    for field_name, field_spec in fields:
        field_name_mangled = mangling(field_name)
        # very javascript here
        if isinstance(field_spec, PrimitiveTypeSpec):
            @property
            @feature(staging)
            def get_method(self):
                method: const = method_tb[f'{chain}_SGet_{field_name_mangled}_BGet']
                return method(self.__accessor__)

            @get_method.setter
            def set_method(self, value):
                method: const = method_tb[f'{chain}_SSet_{field_name_mangled}']
                method(self.__accessor_, value)

        else:
            chaining_getter = f'{chain}_SGet_{field_name_mangled}'
            field_cls = create_cls(field_spec, chaining_getter, methods, 0)
            print(field_cls.__slots__)

            _internal_macro.expr(f"def get_specific_field():\n"
                                 f"   return self.__{non_primitive_count}")

            _internal_macro.stmt('def set_specific_field():\n'
                                 f'    v = self.__{non_primitive_count}'
                                 f' = constexpr[field_type](self.__accessor__, self.__args__)')
            _internal_macro.stmt('def return_proxy():\n'
                                 f'    return v')

            get_specific_field: _internal_macro
            set_specific_field: _internal_macro
            return_proxy: _internal_macro

            @property
            @feature(_internal_macro, staging)
            def get_method(self):
                field_type: const = field_cls
                try:
                    return get_specific_field()
                except AttributeError:
                    set_specific_field()
                    return_proxy()

            @get_method.setter
            @feature(_internal_macro, staging)
            def set_method(self, value):
                method: const = method_tb[f'{chain}_SSet_{field_name_mangled}']
                method(value.ref_get(), self.__accessor__)

            @feature(staging)
            def ref_set(self, value):
                method: const = method_tb[f'{chain}_SSet_{field_name_mangled}']
                method(value.ref_get(), self.__accessor__)

            field_cls.ref_set = ref_set
            non_primitive_count += 1
        setattr(ty, field_name, set_method)

    ty.__slots__ += tuple(f'__{i}' for i in range(non_primitive_count))

    def __init__(self):
        method = method_tb[f'create_{ty_name_mangled}']
        self.__accessor__ = method()

    ty.__init__ = __init__


@make_class.case(False)
def make_class(ty: List, method_tb):
    spec: ListTypeSpec = ty.get_spec()
    ty_name_mangled = type_spec_to_name(spec)

    def __init__(self):
        method = method_tb[f'create_{ty_name_mangled}']
        self.__accessor__ = method()

    ty.__init__ = __init__
    chain = ty_name_mangled

    # TODO: deepcopy
    # @feature(staging)
    # def deepcopy(self):
    #     method: const = method_tb[f'{chain}_BGet']
    #     cls: const = ty
    #     new = cls()
    #     new.ref_set(method(self.__accessor__))
    #
    # ty.deepcopy = deepcopy

    @feature(staging)
    def ref_get(self):
        method: const = method_tb['Unbox']
        return method(self.__accessor__)

    ty.ref_get = ref_get

    @feature(staging)
    def ref_set(self, value):
        method: const = method_tb[f'{chain}_BSet']
        return method(self.__accessor__, value.ref_get())

    ty.ref_set = ref_get
    elem = spec.elem_type

    if isinstance(elem, PrimitiveTypeSpec):
        @feature(staging)
        def __getitem__(self, idx):
            method: const = method_tb[f'{chain}_LGet_BGet']
            return method(idx, self.__accessor__)

        @feature(staging)
        def __setitem__(self, idx, value):
            method: const = method_tb[f'{chain}_LSet']
            return method(value, idx, self.__accessor__)

        @feature(staging)
        def __contains__(self, value):
            method: const = method_tb[f'{chain}_LContains']
            return method(value, self.__accessor__)

        @feature(staging)
        def insert_at(self, idx, value):
            method: const = method_tb[f'{chain}_LInsertAt']
            return method(value, idx, self.__accessor__)

    else:
        chaining_getter = f'{chain}_LGet'
        elem_cls = create_cls(elem, chaining_getter, method_tb, 1)
        print(elem_cls.__dict__.keys())

        @feature(staging)
        def ref_set(self, value):
            method: const = method_tb[f'{chain}_LSet']
            method(value.ref_get(), self.__args__[0], self.__accessor__)

        elem_cls.ref_set = ref_set

        @feature(staging)
        def __getitem__(self, idx):
            proxy_cls: const = elem_cls
            return proxy_cls(self.__accessor__, [idx, self.__args__])

        @feature(staging)
        def __setitem__(self, idx, value):
            method: const = method_tb[f'{chain}_LSet']
            return method(value.ref_get(), idx, self.__accessor__)

        @feature(staging)
        def __contains__(self, value):
            method: const = method_tb[f'{chain}_LContains']
            return method(value.ref_get(), self.__accessor__)

        @feature(staging)
        def insert_at(self, idx, value):
            method: const = method_tb[f'{chain}_LInsertAt']
            return method(value.ref_get(), idx, self.__accessor__)

    @feature(staging)
    def __delitem__(self, idx):
        method: const = method_tb[f'{chain}_LRemoveAt']
        method(idx, self.__accessor__)

    @feature(staging)
    def __len__(self):
        method: const = method_tb[f'{chain}_LCount']
        return method(self.__accessor__)

    @feature(staging)
    def __init__(self, acc, args):
        self.__args__ = args
        self.__accessor__ = acc

    ty.__init__ = __init__
    ty.insert = insert_at
    ty.__contains__ = __contains__
    ty.__len__ = __len__
    ty.__delitem__ = __delitem__
    ty.__setitem__ = __setitem__
    ty.__getitem__ = __getitem__


if __name__ == '__main__':
    methods = {
        'cell_C_SGet_s_SGet_i_BGet': lambda acc: print('get C.s.i<basic>'),
        'cell_C_SGet_s_SSet_i': lambda acc, value: print('set C.s.i = {}'.format(value)),
        'cell_C_SGet_s_SGet_k': lambda acc, value: print('get C.s.k'),
        'cell_C_SGet_s_SSet_k': lambda acc, value: print(f'set C.s.k = {value}'),
        'cell_C_SGet_s_SGet_k_SGet_a_BGet': lambda acc: print('get C.s.k.a<basic>'),
        'cell_C_SGet_s_SGet_k_SSet_a': lambda acc, value: print(f'set C.s.k.a = {value}'),
        "cell_C_SGet_s": lambda acc: print('get_ref_of_S'), 'Unbox': lambda self: print('Unbox'),
        'cell_C_BSet': lambda self, value: print('call ref_set'), 'cell_C_SSet_s': lambda acc, value: print('set C.s'),

    }


    class K(Struct):
        a: int


    class S(Struct):
        k: K
        i: int


    class C(Cell):
        s: S


    make_class(C, methods)
    print(C.__slots__)
    assert C.__slots__ == ('__accessor__', '__0')
    assert Cell.__slots__ == ('__accessor__',)

    # K = StructTypeSpec('K', ImmutableDict([('a', type_map_spec(int))]))  # S = StructTypeSpec("S", ImmutableDict([('i', type_map_spec(int)), ('k', K)]))  # S_ = create_cls(S, "cell_C_SGet_s", methods, 0)  # s = S_("acc", None)  # print(S_.__dict__.keys(), S_.__slots__)  # print(s.k)  # print(s.__dict__)  # print(s.k.a)
