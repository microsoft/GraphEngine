import sys
import types
from Redy.Tools.TypeInterface import Module, BuiltinMethod

_PY36 = sys.version_info[:2] >= (3, 6)


def raise_exc(e):
    raise e


class Record:
    """
    doctest
    >>> @Record
    >>> class S:
    >>>        a: int
    >>> s = S(1)
    >>> print(s)

    >>> try:
    >>>  s = S("1")
    >>> except TypeError:
    >>>     pass
    """

    def __new__(cls, cls_def: type):
        if not _PY36:
            raise TypeError("Record is only supported"
                            " in Python 3.6+")

        def __init__(self, *args, **kwargs):
            __annotations__ = cls_def.__annotations__

            if args:
                kwargs.update(zip(__annotations__, args))

            for k in __annotations__:
                setattr(self,
                        k,
                        kwargs.get(k))

        cls_def.__init__ = __init__

        if isinstance(cls_def.__str__, BuiltinMethod):
            def __str__(self):
                __annotations__ = cls_def.__annotations__
                return (f"{cls_def.__name__}"
                        f"[{', '.join(f'{field_name}={value}' for field_name, value in map(lambda _: (_, getattr(self, _, None)),__annotations__) )}]")

            cls_def.__str__ = __str__
        return cls_def


def binding(cls):
    return cls
