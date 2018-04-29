from Graph.tsl import type_sys, type_map, type_spec, type_factory
from typing import List, Type

assert type_map.type_map_spec(str) == type_spec.PrimitiveSpec("string", str)

assert type_map.type_map_spec(List[List[str]]) == type_spec.ListSpec(
    type_spec.ListSpec(type_spec.PrimitiveSpec("string", str)))

tsl = type_factory.TSL()


@tsl.struct
class S:
    foo: List[str]
    bar: int


@tsl.cell
class C1:
    a: int
    b: List[int]
    c: List[S]
    d: S


# assert C1.get_spec().__str__().strip() == """
# cell C1
# {
# int a;
# List<int> b;
# List<S> c;
# S d;
# }
# """.strip()

from Graph.DotNet.setup import init_trinity_service, build_module

init_trinity_service()

# module = build_module(tsl.to_tsl, "testing")
#
# s = module.New_Struct_S()
#
# module.Struct_S_Set_bar(s, 2)
#
# print(module.Struct_S_Get_bar(s))

tsl.bind()


@tsl.use_list
class LI(List[int]):
    pass


module = tsl.module

lst = LI()

print(len(lst))

c1 = C1()

c1.a = 1

print(c1.a)

s = S()
s.bar = 5

s_acc = s.__accessor__

module.Cell_C1_Set_d(c1.__accessor__, s_acc)

# c1.d = s

# lst2 = module.New_List_int32()
# module.Insert_List_int32(lst2, 0, 1)
