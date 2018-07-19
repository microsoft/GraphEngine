from Redy.Magic.Classic import discrete_cache
import copy


class NotDefinedYet:
    def __init__(self, name):
        self.name = name

    def __repr__(self):
        return self.name

    def __str__(self):
        raise NotImplemented


# noinspection PyPep8Naming
class _List_Ty_Annotate(type):
    @discrete_cache
    def __getitem__(self, item):
        if hasattr(self, '__garg__'):
            raise TypeError("Cannot generalise a concrete type.")
        if isinstance(item, str):
            item = NotDefinedYet(item)
        new_ty = self.__class__(f'{self.__name__}<{item.__name__}>', self.__bases__, dict(self.__dict__))
        new_ty.__gbase__ = self
        new_ty.__garg__ = item
        return new_ty

    def __instancecheck__(self, instance):
        return any(issubclass(each, List) for each in instance.__class__.__mro__)

    def __subclasscheck__(self, subclass):
        return hasattr(subclass, '__gbase__') and subclass.__gbase__ is List


class List(metaclass=_List_Ty_Annotate):
    __ty_spec__: object
    __accessor__: object
    __args__ = None
    __garg__: type

    @classmethod
    def get_spec(self):
        return self.__ty_spec__
