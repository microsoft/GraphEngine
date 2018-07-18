from .type_sys import *
from .type_map import *
from .type_verbs import *
from .mangling import mangling
from Redy.Opt import feature, constexpr, const
from Redy.Tools.TypeInterface import Module
from Redy.Magic.Pattern import Pattern
from Redy.Magic.Classic import cast, execute
from typing import Type, List, Tuple
from ..err import  *

from collections import namedtuple

staging = (const, constexpr)
_undef = object()

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


def tsl_build_src_code(code: str) -> Module:
    raise NotImplemented


def typename_manging(typ: TSLObject) -> str:
    raise NotImplemented


@Pattern
def tsl_generate_methods(tsl_session, cls_def: Type[TSLObject]):
    if issubclass(cls_def, TSLStruct):
        return TSLStruct
    elif issubclass(cls_def, TSLList):
        return TSLList
    else:
        print(cls_def.__name__, cls_def.__bases__)
        raise TypeError


def make_setter_getter_for_primitive(_getter, _setter):
    @property
    def getter(self: TSLStruct):
        return _getter(self.__accessor__)

    @getter.setter
    def setter(self, value):
        assert not isinstance(value, TSLObject)
        _setter(self.__accessor__, value)

    return setter


def make_setter_getter_for_general(_getter, _setter, object_cls: Type[TSLObject]):
    @property
    def getter(self: TSLStruct):
        new = object_cls.__new__(object_cls)
        new.__accessor__ = _getter(self.__accessor__)
        return new

    @getter.setter
    def setter(self, value):
        assert isinstance(value, TSLObject)
        _setter(self.__accessor__, value.__accessor__)

    return setter


def _render_field_err_info(typename, field):
    return 'Invalid field {} visiting for type {}.'.format(typename, field)


@tsl_generate_methods.case(TSLStruct)
def tsl_generate_methods(tsl_session, cls_def):
    spec: StructSpec = cls_def.get_spec()
    typename = type_spec_to_name(spec)

    non_primitive_offset = 0
    non_primitive_field_types: typing.List[typing.Type[TSLNonPrimitiveObject]] = []
    properties = []
    for field_name, field_spec in spec.fields.items():
        is_primitive = isinstance(field_spec, PrimitiveSpec)

        _field_name = mangling(field_name)
        getter_name = SGet(typename, _field_name).__str__()
        setter_name = SSet(typename, _field_name).__str__()
        _getter = getattr(tsl_session.module, getter_name)
        _setter = getattr(tsl_session.module, setter_name)

        # SGet/ SSet
        # cell.lst = tsl_lst
        # cell.foo = 1  # primitive
        # cell.some_struct = some_struct
        # print (cell.bar)



        @property
        @feature(staging)
        def getter(self: TSLStruct):
            struct_get: const = _getter
            if constexpr[is_primitive]:
                return struct_get(self.__accessor__)
            else:
                # return new proxy
                field_types = self.__non_primitive_types__
                field: TSLNonPrimitiveObject = field_types[constexpr[non_primitive_offset]].new_proxy()
                field.__accessor__ = struct_get(self.__accessor__)
                return field

                # fields = self.__non_primitive__
                # field: TSLNonPrimitiveObject = fields[constexpr[non_primitive_offset]]
                # if field.__accessor__ is None:
                #     field.__accessor__ = struct_get(self.__accessor__)
                # return field

        @getter.setter
        @feature(staging)
        def setter(self: TSLStruct, value):
            struct_set: const = _setter
            if constexpr[is_primitive]:
                struct_set(self.__accessor__, value)
            else:
                assert isinstance(value, TSLObject)
                struct_set(self.__accessor__, value.__accessor__)
                #
                # fields = self.__non_primitive__
                # # print(fields, constexpr[non_primitive_offset])
                # fields[constexpr[non_primitive_offset]] = value
                # struct_set(self.__accessor__, value.__accessor__)
                # # delete
                # if value.__accessor__ is None:
                #     self.__accessor__ = None

        @setter.deleter
        def deleter(self):
            struct_set: const = _setter
            if constexpr[is_primitive]:
                return struct_set(self.__accessor__, None)
            else:
                struct_set(self.__accessor__, None)
                # fields = self.__non_primitive__
                # field: TSLNonPrimitiveObject = fields[constexpr[non_primitive_offset]]
                # acc = field.__accessor__
                # if acc is not None:
                #     struct_set(self.__accessor__, None)
                #     field.__accessor__ = None

        if not is_primitive:
            field_cls = tsl_session.type_specs_to_type[field_spec]
            non_primitive_field_types.append(field_cls)
            non_primitive_offset += 1

        properties.append(setter)
        setattr(cls_def, field_name, deleter)

    non_primitive_field_types = tuple(non_primitive_field_types)
    cls_def.__non_primitive_types__ = non_primitive_field_types

    # create a proxy without actual data allocation
    # >>> MyType.new_proxy()
    @feature(staging)
    def new_proxy():
        cls: const = cls_def
        self = cls.__new__(cls)
        self.__accessor__ = None
        if constexpr[non_primitive_field_types]:
            self.__non_primitive__ = [each.new_proxy() for each in constexpr[non_primitive_field_types]]
        return self

    cls_def.new_proxy = staticmethod(new_proxy)

    # __init__ method
    # New a Cell/Struct
    # >>> my_cell = MyCell()
    new_struct_fn_name = BNew(typename).__str__()
    _new_struct = getattr(tsl_session.module, new_struct_fn_name)

    @feature(staging)
    def init(self: TSLStruct):
        struct_initializer: const = _new_struct
        self.__accessor__ =  struct_initializer()
        if constexpr[non_primitive_field_types]:
            self.__non_primitive__ = [each() for each in constexpr[non_primitive_field_types]]

    cls_def.__init__ = init

    # BGet. deepcopy.
    # >>> new_cell = cell.deepcopy()
    deepcopy_fn_name = BGet(typename).__str__()
    _deepcopy = getattr(tsl_session.module, deepcopy_fn_name)

    @feature(staging)
    def deepcopy(self) -> cls_def:
        struct_deepcopy: const = _deepcopy
        proxy_creator: const = new_proxy
        new = proxy_creator()
        new.__accessor__ = struct_deepcopy(self.__accessor__)
        return new

    cls_def.deepcopy = deepcopy

    # BSet. change value by reference.
    # >>> cell.ref_assign(another_cell)
    reference_assign_fn_name = BSet(typename).__str__()
    _reference_assign = getattr(tsl_session.module, reference_assign_fn_name)

    @feature(staging)
    def reference_assign(self, value):
        assert isinstance(value, TSLObject)
        struct_ref_assign: const = _reference_assign
        struct_ref_assign(self.__accessor__, value.__accessor__)

    cls_def.ref_assign = reference_assign


@tsl_generate_methods.case(TSLList)
def tsl_generate_methods(tsl_session, cls_def):
    spec: ListSpec = cls_def.get_spec()

    typename = type_spec_to_name(spec)
    (_get, _set, _count, _contains, _insert, _remove, _append, _deepcopy, _reference_assign, _new_lst) = map(
        lambda verb: getattr(tsl_session.module, str(verb)),
        [
            LGet(typename),
            LSet(typename),
            LCount(typename),
            LContains(typename),
            LInsertAt(typename),
            LRemoveAt(typename),
            LAppend(typename),
            BGet(typename),
            BSet(typename),
            BNew(typename)
        ])

    # Index getter/setter, insert, remove, append
    if isinstance(spec.elem_type, PrimitiveSpec):
        @feature(staging)
        def __getitem__(self, i: int):
            lst_get: const = _get
            return lst_get(self.__accessor__, i)

        @feature(staging)
        def __setitem__(self, i: int, value):
            assert not isinstance(value, (TSLList, TSLStruct))
            lst_set: const = _set
            lst_set(self.__accessor__, i, value)

        @feature(staging)
        def insert(self, i: int, value) -> bool:
            lst_insert: const = _insert
            return lst_insert(self.__accessor__, i, value)

        @feature(staging)
        def append(self, value):
            lst_append: const = _append
            lst_append(self.__accessor__, value)

    else:
        field_cls : TSLNonPrimitiveObject = tsl_session.type_specs_to_type[spec.elem_type]

        @feature(staging)
        def __getitem__(self, i: int):
            cls: const = field_cls
            lst_get: const = _get
            elem = cls.new_proxy()
            elem.__accessor__ = lst_get(self.__accessor__, i)
            return elem

        @feature(staging)
        def __setitem__(self, i: int, value):
            assert isinstance(value, (TSLList, TSLStruct))
            lst_set: const = _set
            lst_set(self.__accessor__, i, value.__accessor__)

        @feature(staging)
        def insert(self, i: int, value):
            lst_insert: const = _insert
            return lst_insert(self.__accessor__, i, value.__accessor__)

        @feature(staging)
        def append(self, value):
            lst_append: const = _append
            lst_append(self.__accessor__, value.__accessor__)

    @feature(staging)
    def __delitem__(self, i: int):
        return constexpr[_remove](self.__accessor__, i)

    # create a proxy without actual data allocation
    # >>> MyList.new_proxy().__accessor__ is None # => True
    @feature(staging)
    def new_proxy():
        cls: const = cls_def
        self = cls.__new__(cls)
        self.__accessor__ = None
        return self

    # initialize a List with allocation
    def __init__(self):
        self.__accessor__ = _new_lst()

    cls_def.__init__ = __init__
    cls_def.__getitem__ = __getitem__
    cls_def.__setitem__ = __setitem__
    cls_def.__delitem__ = __delitem__

    cls_def.append = append
    cls_def.insert = insert

    # len(lst: TSLList) -> int
    @feature(staging)
    def __len__(self):
        count: const = _count
        return count(self.__accessor__)

    cls_def.__len__ = __len__

    # test is elem in lst
    @feature(staging)
    def __contains__(self, elem):
        contains: const = _contains
        if constexpr[isinstance(spec.elem_type, PrimitiveSpec)]:
            assert not isinstance(elem, (TSLStruct, TSLList))
            return contains(self.__accessor__, elem)
        else:
            assert isinstance(elem, (TSLList, TSLStruct))
            return contains(self.__accessor__, elem.__accessor__)

    cls_def.__contains__ = __contains__

    @feature(staging)
    def deepcopy(self) -> cls_def:
        lst_deepcopy: const = _deepcopy
        proxy_creator: const = new_proxy
        new = proxy_creator()
        new.__accessor__ = lst_deepcopy(self.__accessor__)
        return new

    cls_def.deepcopy = deepcopy

    # reference_assign

    @feature(staging)
    def reference_assign(self, value):
        assert isinstance(value, TSLObject)
        lst_ref_assign: const = _reference_assign
        lst_ref_assign(self.__accessor__, value.__accessor__)

    cls_def.ref_assign = reference_assign
