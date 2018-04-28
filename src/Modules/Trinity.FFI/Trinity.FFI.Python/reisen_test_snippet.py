from Graph.tsl import type_sys, type_map, type_spec, type_factory
from typing import List

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


assert C1.get_spec().__str__().strip() == """
cell C1
{ 
int a;
List<int> b;
List<S> c;
S d; 
}
""".strip()

from Graph.DotNet.setup import init_trinity_service, build_module

init_trinity_service()

tsl.bind()
# module = build_module(tsl.to_tsl, "testing")


# s = module.New_Struct_S()
#
# module.Struct_S_Set_bar(s, 2)
#
# print(module.Struct_S_Get_bar(s))
