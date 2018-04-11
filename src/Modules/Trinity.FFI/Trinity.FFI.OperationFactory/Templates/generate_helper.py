from itertools import takewhile


class Method:
    def __init__(self, func):
        self.func = func

    def __call__(self, cls):
        return self.func(cls)


def reformat(v: str):
    space_count = len(tuple(takewhile(lambda _: _ == ' ', v.splitlines()[1])))
    return '\n'.join(map(lambda _: _[space_count:], v.splitlines()))


def generate(cls: type):
    return {name: reformat(value(cls)) for name, value in cls.__dict__.items() if isinstance(value, Method)}
