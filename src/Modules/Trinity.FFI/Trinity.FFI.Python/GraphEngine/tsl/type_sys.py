from .type_spec import TSLTypeSpec
import typing

_basic_types = typing.Union[str, int, float]


class TSLObject:
    get_spec: typing.Callable[[], TSLTypeSpec]
    __accessor__: object

    def __init__(self):
        raise NotImplemented

    def ref(accessor):
        raise NotImplemented

    def ref_assign(self, value):
        raise NotImplemented

    def deepcopy(self):
        raise NotImplemented

    def is_nil(self):
        raise self.__accessor__ is None

    @staticmethod
    def new_proxy():
        """
        create a proxy without accessor.
        """



TSLGeneralObject = typing.Union[TSLObject, _basic_types]


class TSLStruct(TSLObject):
    __non_primitive__: typing.List[TSLObject]
    __non_primitive_types__: typing.List[typing.Type[TSLObject]]

    def check_schema(self):
        print(self.__non_primitive__)
    pass



class TSLCell(TSLStruct):
    pass


class TSLList(TSLObject):
    __elems__: typing.List[TSLObject]
    pass


TSLNonPrimitiveObject = typing.Union[TSLStruct, TSLList]