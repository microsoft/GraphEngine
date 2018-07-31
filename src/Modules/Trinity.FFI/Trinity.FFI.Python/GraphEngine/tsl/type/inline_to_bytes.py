import types

from Redy.Magic.Pattern import Pattern
from GraphEngine.tsl.type.system import (TSLType, Cell, List, StructTypeSpec,
                                         ListTypeSpec, PrimitiveTypeSpec,
                                         Struct, NotDefinedYet)
import struct
import ast
import typing
# from rbnf._py_tools.unparse import Unparser

struct_pack = struct.pack


def int_to_bytes(value, byte_num=4):
    attr = ast.Attribute(value=value, attr='to_bytes', ctx=ast.Load())
    return ast.Call(
        func=attr,
        keywords=[],
        args=[ast.Num(n=byte_num), ast.Str(s="little")])


def float_to_bytes(value):
    return ast.Call(
        func=ast.Name(id='struct_pack', ctx=ast.Load()),
        keywords=[],
        args=[ast.Str(s='=f'), value])


def length_to_bytes(value):
    length = ast.Call(
        func=ast.Name(id='len', ctx=ast.Load()), keywords=[], args=[value])
    return int_to_bytes(length, 4)


def double_to_bytes(value):
    return ast.Call(
        func=ast.Name(id='struct_pack', ctx=ast.Load()),
        keywords=[],
        args=[ast.Str(s='=d'), value])


def str_to_bytes(value):
    return ast.Subscript(
        value=ast.Call(
            func=ast.Attribute(value=value, attr='encode', ctx=ast.Load()),
            keywords=[],
            args=[ast.Str(s='utf-16')]),
        slice=ast.Slice(lower=ast.Num(n=2), upper=None, step=None),
        ctx=ast.Load())


def assign(name, value):
    return ast.Assign(
        targets=[ast.Name(id=name, ctx=ast.Store())], value=value)


def yield_value(value):
    return ast.Expr(value=ast.Yield(value=value))


def make_get_bytes(spec, lazy: dict, depth=0) -> typing.Callable:
    """
    # TODO: finally I found write bytecode directly is much more easy and even makes a higher performance.
    # refactor after pre
    """
    if isinstance(spec, NotDefinedYet):
        spec = lazy[spec.name]

    def call(node: ast.AST):
        identifier = f'obj_{depth}'
        value = node

        if isinstance(spec, PrimitiveTypeSpec):
            if spec.type_code == 'I8':
                return yield_value(int_to_bytes(value, 1))

            if spec.type_code == 'I16':
                return yield_value(int_to_bytes(value, 2))

            if spec.type_code == 'I32':
                return yield_value(int_to_bytes(value, 4))

            if spec.type_code == 'I64':
                return yield_value(int_to_bytes(value, 8))

            if spec.type_code == 'STRING':
                bind = assign("tmp", str_to_bytes(value))
                value = ast.Name(id='tmp', ctx=ast.Load())
                return [
                    bind,
                    yield_value(length_to_bytes(value)),
                    yield_value(value)
                ]

            if spec.type_code == 'U8STRING':
                bind = assign("tmp", value)
                value = ast.Name(id='tmp', ctx=ast.Load())

                return [
                    bind,
                    yield_value(length_to_bytes(value)),
                    yield_value(value)
                ]

            if spec.type_code == 'F32':
                return yield_value(float_to_bytes(value))

            if spec.type_code == 'F64':
                return yield_value(double_to_bytes(value))

            raise TypeError

        alias = assign(identifier, node)
        value = ast.Name(id=identifier, ctx=ast.Load())

        if isinstance(spec, StructTypeSpec):
            ret = [alias]
            for name, field_spec in spec.field_types.items():
                sub = make_get_bytes(field_spec, lazy,
                                     depth + 1)(ast.Subscript(
                                         value=value,
                                         slice=ast.Index(value=ast.Str(name)),
                                         ctx=ast.Load()))
                if isinstance(sub, list):
                    ret.extend(sub)
                else:
                    ret.append(sub)

            return ret

        else:
            assert isinstance(spec, ListTypeSpec)

            head_bytes = length_to_bytes(value)
            iter_var_identifier = f'each{depth}'

            return [
                alias,
                yield_value(head_bytes),
                ast.For(
                    target=ast.Name(id=iter_var_identifier, ctx=ast.Store()),
                    iter=value,
                    body=make_get_bytes(spec.elem_type, lazy,
                                        depth + 1)(ast.Name(
                                            id=iter_var_identifier,
                                            ctx=ast.Load())),
                    orelse=[],
                )
            ]

    return call


def make_fast_to_bytes_function(ty,
                                lazy: dict) -> typing.Callable[[dict], bytes]:
    assert issubclass(ty, (Struct, List))
    func_ast = make_get_bytes(ty.get_spec(), lazy)
    bind_struct_pack = ast.Assign(
        targets=[ast.Name(id="struct_pack", ctx=ast.Store())],
        value=ast.Attribute(
            value=ast.Name(id='struct', ctx=ast.Load()),
            attr='pack',
            ctx=ast.Load()))

    node = ast.FunctionDef(
        name='f',
        args=ast.arguments(
            args=[ast.arg(arg='obj', annotation=None)],
            vararg=None,
            kwonlyargs=[],
            kw_defaults=[],
            kwarg=None,
            defaults=[]),
        decorator_list=[],
        returns=None,
        body=[bind_struct_pack, *func_ast(ast.Name(id='obj', ctx=ast.Load()))])
    ast.fix_missing_locations(node)
    # Unparser(node)
    local = {'struct': struct}
    exec(compile(ast.Module([node]), "<TSL compiling time>", "exec"), local)
    return local['f']


if __name__ == '__main__':

    class S(Struct):
        i: int
        s: str

    class C(Cell):
        i: int
        s: S
        ls: List[S]

    class LS(List[S]):
        pass

    fn = make_fast_to_bytes_function(C, {})
    print(list(b''.join(fn(dict(i=1, s=dict(i=1, s="555"), ls=[])))))

    print(list(b''.join(fn(dict(i=1, s=dict(i=1, s="555"), ls=[])))))
    """
    1 0 0 0 
    0 0 0 0 
    1 0 0 0 
    6 0 0 0 
    53 0 53 0 53 0
    """
