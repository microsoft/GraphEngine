
from Ruikowa.ObjectRegex.MetaInfo import MetaInfo

from GraphEngine.tsl.TypeMap import TSLTypeConstructor
from GraphEngine.tsl.tsl_type_parser import token, TSLTypeParse


def test():

    type_string = token('List<int>?')
    ast = TSLTypeParse(type_string, MetaInfo(), partial=False)
    int_list = TSLTypeConstructor(ast)
    inst = int_list()
    print(inst)
    



