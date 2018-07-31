from GraphEngine.tsl.type.system import *
from GraphEngine.tsl.type.mangling import *
from Redy.Opt import feature, const, constexpr, Macro
import json

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
def create_cls(spec: TypeSpec, chain: str, method_tb, arg_num, cls_tb):
    return type(spec)


@create_cls.case(StructTypeSpec)
def create_cls(spec: StructTypeSpec, chain: str, method_tb, arg_num, cls_tb):
    print(chain)
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
        if isinstance(field_spec, NotDefinedYet):
            try:
                field_spec = cls_tb[field_spec.name].get_spec()
            except KeyError:
                raise NameError(f"No class named {field_spec.name}")
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
            @feature(staging)
            def set_method(self, value):
                method: const = method_tb[f'{chain}_SSet_{field_name_mangled}']
                if constexpr[arg_num]:
                    method(value, *constexpr[take_args](self.__args__), self.__accessor__)
                else:
                    method(value, self.__accessor__)

        else:
            chaining_getter = f'{chain}_SGet_{field_name_mangled}'
            field_cls = create_cls(field_spec, chaining_getter, method_tb, arg_num, cls_tb)

            _internal_macro.expr(f"def get_specific_field():\n"
                                 f"   return self.__{non_primitive_count}")

            _internal_macro.stmt('def set_specific_field():\n'
                                 f'    v = self.__{non_primitive_count} = constexpr[field_type](self.__accessor__, self.__args__)\n'
                                 f'    return v')

            get_specific_field: _internal_macro
            set_specific_field: _internal_macro

            @property
            @feature(_internal_macro, staging)
            def get_method(self):
                field_type: const = field_cls
                try:
                    return get_specific_field()
                except AttributeError:
                    set_specific_field()

            @get_method.setter
            @feature(staging)
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
def create_cls(spec: ListTypeSpec, chain: str, method_tb: dict, arg_num: int, cls_tb):
    print(chain)
    bases = (List, Proxy)
    cls = type(repr(spec), bases, {'__slots__': ('__accessor__', '__args__')})
    elem = spec.elem_type

    if isinstance(elem, NotDefinedYet):
        try:
            field_spec = cls_tb[elem.name].get_spec()
        except KeyError:
            raise NameError(f"No class named {field_spec.name}")

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
        def append(self, value):
            method: const = method_tb[f'{chain}_LAppend']
            if constexpr[arg_num]:
                return method(value, *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value, self.__accessor__)

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

        @feature(staging)
        def __iter__(self):
            method_len: const = method_tb[f'{chain}_LCount']
            method_get: const = method_tb[f'{chain}_LGet_BGet']
            acc = self.__accessor__
            if constexpr[arg_num]:
                args = constexpr[take_args](self.__args__)
                for i in constexpr[range](method_len(*args, acc)):
                    yield method_get(i, *args, acc)
            else:
                for i in constexpr[range](method_len(acc)):
                    yield method_get(i, acc)

    else:
        chaining_getter = f'{chain}_LGet'
        elem_cls = create_cls(elem, chaining_getter, method_tb, arg_num + 1, cls_tb)

        @feature(staging)
        def ref_set(self, value):
            method: const = method_tb[f'{chain}_LSet']
            method(value.ref_get(), *constexpr[take_args](self.__args__), self.__accessor__)

        elem_cls.ref_set = ref_set

        @feature(staging)
        def append(self, value):
            method: const = method_tb[f'{chain}_LAppend']
            if constexpr[arg_num]:
                return method(value.ref_get(), *constexpr[take_args](self.__args__), self.__accessor__)
            else:
                return method(value.ref_get(), self.__accessor__)

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
        def __iter__(self):
            method_len: const = method_tb[f'{chain}_LCount']
            __args__: const = [None, None]
            proxy: const = elem_cls(None, __args__)
            proxy.__accessor__ = acc = self.__accessor__

            if constexpr[arg_num]:
                args = __args__[1] = self.__args__
                for i in constexpr[range](method_len(*take_args(args), acc)):
                    __args__[0] = i
                    yield proxy
            else:
                for i in constexpr[range](method_len(acc)):
                    __args__[0] = i
                    yield proxy

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
    cls.append = append
    cls.__getitem__ = __getitem__
    cls.__iter__ = __iter__
    return cls


@Pattern
def make_class(ty: typing.Type[TSLType], method_tb, cls_tb):
    return issubclass(ty, Struct)


@make_class.case(True)
def make_class(ty: typing.Type[Struct], method_tb, cls_tb):
    ty.__slots__ = getattr(ty, '__slots__', ()) + ('__accessor__',)
    spec: StructTypeSpec = ty.get_spec()
    chain = type_spec_to_name(spec)

    if issubclass(ty, Cell):
        # An accessor type of a cell is locked once it's created.
        # So it don't require to lock one more when __enter__ is called.

        @feature(staging)
        def use(cell_id: int, cell_access_options: int):
            method: const = method_tb[f'use_{chain}']
            acc_ty: const = ty
            new = acc_ty.__new__(acc_ty)
            new.__accessor__ = method(cell_id, cell_access_options)
            return new

        ty.use = staticmethod(use)

        @feature(staging)
        def access(cell_id, cell_access_options: int):
            acc_ty: const = ty
            return FutureAccessor(cell_id, cell_access_options, acc_ty)

        ty.access = staticmethod(access)

        @feature(staging)
        def __enter__(self, cell_access_options: int):
            method: const = method_tb[f'reuse_{chain}']
            method(self.__accessor__, cell_access_options)
            return self

        ty.__enter__ = __enter__

        @feature(staging)
        def __exit__(self):
            method: const = method_tb[f'unlock']
            method(self.__accessor__)

        ty.__exit__ = __exit__
        ty.release = __exit__

        @feature(staging)
        def save(self):
            method: const = method_tb[f'save_cell']
            method(self.__accessor__)

        ty.save = save

        @feature(staging)
        def load(cell_id: int):
            ty_: const = ty
            method: const = method_tb[f'load_{chain}']
            new = ty_.__new__(ty_)
            new.__accessor__ = method(cell_id)
            return new

        ty.load = staticmethod(load)

    fields = spec.field_types.items()

    non_primitive_count = 0

    @feature(staging)
    def ref_get(self):
        method: const = method_tb['Unbox']
        return method(self.__accessor__)

    ty.ref_get = ref_get

    @feature(staging)
    def ref_set(self, value):
        method: const = method_tb[f'{chain}_BSet']
        return method(value.ref_get(), self.__accessor__)

    ty.ref_set = ref_set

    for field_name, field_spec in fields:
        if isinstance(field_spec, NotDefinedYet):
            try:
                field_spec = cls_tb[field_spec.name].get_spec()
            except KeyError:
                raise NameError(f"No class named {field_spec.name}")

        field_name_mangled = mangling(field_name)
        # very javascript here
        if isinstance(field_spec, PrimitiveTypeSpec):
            @property
            @feature(staging)
            def get_method(self):
                method: const = method_tb[f'{chain}_SGet_{field_name_mangled}_BGet']
                return method(self.__accessor__)

            @get_method.setter
            @feature(staging)
            def set_method(self, value):
                method: const = method_tb[f'{chain}_SSet_{field_name_mangled}']
                method(value, self.__accessor__)

        else:
            chaining_getter = f'{chain}_SGet_{field_name_mangled}'
            field_cls = create_cls(field_spec, chaining_getter, method_tb, 0, cls_tb)

            _internal_macro.expr(f"def get_specific_field():\n"
                                 f"   return self.__{non_primitive_count}")

            _internal_macro.stmt('def set_specific_field():\n'
                                 f'    v = self.__{non_primitive_count} = constexpr[field_type](self.__accessor__, self.__args__)\n'
                                 f'    return v')

            get_specific_field: _internal_macro
            set_specific_field: _internal_macro

            @property
            @feature(_internal_macro, staging)
            def get_method(self):
                field_type: const = field_cls
                try:
                    return get_specific_field()
                except AttributeError:
                    set_specific_field()

            @get_method.setter
            @feature(staging)
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

    @feature(staging)
    def __init__(self, data: object = None):
        default_method: const = method_tb[f'create_{chain}']
        valued_method: const = method_tb[f'create_{chain}_with_data']
        if data is None:
            self.__accessor__ = default_method()
        else:
            # not `constexpr[json.dumps], for the attribute `dumps` is not fixed.
            self.__accessor__ = valued_method(constexpr[json].dumps(data))

    ty.__init__ = __init__


@make_class.case(False)
def make_class(ty: List, method_tb, cls_tb):
    spec: ListTypeSpec = ty.get_spec()
    chain = type_spec_to_name(spec)

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
        return method(value.ref_get(), self.__accessor__)

    ty.ref_set = ref_set
    elem = spec.elem_type

    if isinstance(elem, NotDefinedYet):
        try:
            elem = cls_tb[elem.name].get_spec()
        except KeyError:
            raise NameError(f"No class named {elem.name}")

    if isinstance(elem, PrimitiveTypeSpec):
        @feature(staging)
        def append(self, value):
            method: const = method_tb[f'{chain}_LAppend']
            return method(value, self.__accessor__)

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

        @feature(staging)
        def __iter__(self):
            method_len: const = method_tb[f'{chain}_LCount']
            method_get: const = method_tb[f'{chain}_LGet_BGet']
            acc = self.__accessor__
            for i in constexpr[range](method_len(acc)):
                yield method_get(i, acc)

    else:
        chaining_getter = f'{chain}_LGet'
        elem_cls = create_cls(elem, chaining_getter, method_tb, 1, cls_tb)

        @feature(staging)
        def ref_set(self, value):
            method: const = method_tb[f'{chain}_LSet']
            method(value.ref_get(), self.__args__[0], self.__accessor__)

        elem_cls.ref_set = ref_set

        @feature(staging)
        def append(self, value):
            method: const = method_tb[f'{chain}_LAppend']
            return method(value.ref_get(), self.__accessor__)

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
        def __iter__(self):
            method_len: const = method_tb[f'{chain}_LCount']
            __args__: const = [None, None]
            proxy: const = elem_cls(None, __args__)
            proxy.__accessor__ = acc = self.__accessor__
            for i in constexpr[range](method_len(acc)):
                __args__[0] = i
                yield proxy

    @feature(staging)
    def __delitem__(self, idx):
        method: const = method_tb[f'{chain}_LRemoveAt']
        method(idx, self.__accessor__)

    @feature(staging)
    def __len__(self):
        method: const = method_tb[f'{chain}_LCount']
        return method(self.__accessor__)

    @feature(staging)
    def __init__(self, data: object = None):
        default_method: const = method_tb[f'create_{chain}']
        valued_method: const = method_tb[f'create_{chain}_with_data']
        if data is None:
            self.__accessor__ = default_method()
        else:
            # not `constexpr[json.dumps], for the attribute `dumps` is not fixed.
            self.__accessor__ = valued_method(constexpr[json].dumps(data))

    ty.__init__ = __init__
    ty.insert = insert_at
    ty.__contains__ = __contains__
    ty.__len__ = __len__
    ty.__delitem__ = __delitem__
    ty.__setitem__ = __setitem__
    ty.__getitem__ = __getitem__
    ty.__iter__ = __iter__
    ty.append = append


if __name__ == '__main__':
    from collections import defaultdict

    methods = defaultdict(lambda: lambda *args: print(args))


    class K(Struct):
        a: int


    class S(Struct):
        k: 'K'
        i: int


    class C(Cell):
        s: S
        b: List[S]


    cls_tb = {'C': C, 'K': K, 'S': S}
    make_class(C, methods, cls_tb)
    print(C.__slots__)
    assert C.__slots__ == ('__accessor__', '__0', '__1')
