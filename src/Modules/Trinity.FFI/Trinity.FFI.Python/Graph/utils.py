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
        def __init__(self, **kwargs):
            __annotations__ = cls_def.__annotations__
            for k, typ in __annotations__.items():
                setattr(self,
                        k,
                        (lambda _v:
                         _v if isinstance(_v, typ) else raise_exc(
                             TypeError(f"{cls_def.__name__}.{k}: Requires {typ.__name__}, get {type(_v).__name__}.")))(
                            kwargs.get(k)))

        def __str__(self):
            __annotations__ = cls_def.__annotations__
            return (f"{cls_def.__name__}"
                    f"[{', '.join(f'{field_name}={value}' for field_name, value in map(lambda _: (_, getattr(self, _, None)),__annotations__) )}]")

        cls_def.__init__ = __init__
        cls_def.__str__ = __str__
        return cls_def


def binding(cls):
    return cls
