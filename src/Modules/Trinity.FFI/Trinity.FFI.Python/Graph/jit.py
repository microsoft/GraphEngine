from .utils import record
import types


# @binding(_TypeDescriptor)
class TypeDescriptor:
    pass


# @binding(_Verb)
class Verb:
    pass


# @binding(_FunctionDescriptor)
class FunctionDescriptor:
    DeclaringType: TypeDescriptor
    Verb: Verb


class NativeFunctionDescriptor:
    # noinspection SpellCheckingInspection
    Callsite: int


def compile_function(fn_desc: FunctionDescriptor) -> NativeFunctionDescriptor:
    pass
