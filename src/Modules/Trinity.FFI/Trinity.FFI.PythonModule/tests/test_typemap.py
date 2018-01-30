
from GraphEngine.TSL.tsl_type_parser import token, TSLTypeParse
from GraphEngine.Storage.core.TypeMap import TSLTypeConstructor
from Ruikowa.ObjectRegex.MetaInfo import MetaInfo


def test():

    type_string = token('List<int>')
    ast = TSLTypeParse(type_string, MetaInfo(), partial=False)
    int_list = TSLTypeConstructor(ast)
    inst = int_list()
    print(inst)
    



