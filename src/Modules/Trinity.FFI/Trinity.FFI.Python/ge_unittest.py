import unittest

from GraphEngine.tsl import type_sys, type_map, type_spec, type_factory
from typing import List, Type


class Test(unittest.TestCase):

    def test_type_mapping(self):
        self.assertEqual(
            type_map.type_map_spec(str),
            type_spec.PrimitiveSpec("string", str)
        )

        self.assertEquals(
            type_map.type_map_spec(List[List[str]]),
            type_spec.ListSpec(
                type_spec.ListSpec(type_spec.PrimitiveSpec("string", str)))
        )

    def test_tsl_build(self):
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

        C1_expected_tsl_code = ("\n"
                                "cell C1\n"
                                "{\n"
                                "int32 a;\n"
                                "List<int32> b;\n"
                                "List<S> c;\n"
                                "S d;\n"
                                "}\n")
        self.assertEqual(C1.get_spec().__str__().strip(), C1_expected_tsl_code.strip())

        from GraphEngine.DotNet.setup import init_trinity_service, build_module

        init_trinity_service()

        tsl.bind()

        @tsl.use_list
        class LI(List[int]):
            pass

        module = tsl.module
        # l = module.New_List_int32()
        # module.Insert_List_int32(l, 0, 1)

        lst = LI()
        self.assertEqual(len(lst), 0)

        # lst.append(1)
        # self.assertEqual(len(lst), 1)

        # lst.append(2)
        # self.assertEqual(lst[1], 2)

        c1 = C1()
        c1.a = 1

        self.assertEqual(c1.a, 1)

        s = S()
        s.bar = 5
        c1.d = s


if __name__ == '__main__':
    unittest.main()
