from operations import getter_setter, index_getter_setter, single_object_method_return, only_subject_method_return
from generate_helper import *


@generate
class List:
    @index_getter_setter
    class Set:
        pass

    @index_getter_setter
    class Get:
        pass

    @only_subject_method_return('int')
    class Count:
        pass

    @single_object_method_return('bool')
    class Contains:
        pass


@generate
class Struct:
    @getter_setter
    class Set:
        pass

    @getter_setter
    class Get:
        pass


for k, v in Struct.items():
    print(v)
# for each in [Struct, List]:
#     for _, v in List.items():
#         print(v)
