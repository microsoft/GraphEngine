# -*- coding: utf-8 -*-
"""
Created on Tue Jan 30 15:14:14 2018

@author: v-wazhao
"""
from GraphEngine.configure import PY3
from Ruikowa.ObjectRegex.ASTDef import Ast

# begin configure
# these settings are for handling type mapping automatically.
# TODO: do not use parser to speed up type mapping.


from typing import Callable, List, Any


def isa(typ):
    def inner(v):
        return isinstance(v, typ)

    return inner


def default(constructor):
    return {int: 0,
            str: None,
            list: None,
            tuple: None,
            float: .0}[constructor]


class FieldType:
    sig: str
    constructor: type
    checker: Callable

    def __init__(self, sig: str, constructor: type, checker: Callable[[Any], bool], is_optional: bool):
        self.sig = sig
        self.constructor = constructor
        self.checker = checker
        self.is_optional = is_optional
        self.default = None if is_optional else default(constructor)

    def to_optional(self):
        if self.sig.endswith('?'):
            return self
        sig = '{}?'.format(self.sig)
        if sig in Types:
            return Types[sig]
        new_spec = FieldType(sig=sig,
                             constructor=self.constructor,
                             checker=self.checker,
                             is_optional=True)
        Types[new_spec.sig] = new_spec
        return new_spec


def _make_spec_from_primitive(primitive_type: type, is_optional=False):
    return FieldType(primitive_type.__name__, primitive_type, isa(primitive_type), is_optional)


Types = {'int': _make_spec_from_primitive(int),
         'string': _make_spec_from_primitive(str),
         'float': _make_spec_from_primitive(float),
         'double': _make_spec_from_primitive(float),
         'list': _make_spec_from_primitive(list)}


# `Types` Records the types of fields.
def list_constructor_and_checker(field_type: FieldType):
    def checker(inst):
        return isinstance(inst, list) and all(map(field_type.checker, inst))

    checker.for_elem = field_type.checker

    return list, checker


def dict_constructor_and_checker(typ_from: FieldType, type_to: FieldType):
    def checker(inst):
        return isinstance(inst, dict) and all(
            [typ_from.checker(k) and type_to.checker(v) for k, v in inst.items()])

    def for_elem(kv):
        try:
            k, v = kv
            return typ_from.checker(k) and type_to.checker(v)
        except Exception as e:
            print(e)
            return False

    checker.for_elem = for_elem

    return dict, checker


GenericManager = {'List': list_constructor_and_checker,
                  'Dictionary': dict_constructor_and_checker}


# end configure

def type_register(type_spec):
    if type_spec.sig not in Types:
        Types[type_spec.sig] = type_spec


# type class
class TSLTypeConstructor:
    def __new__(cls, tsl_type_ast: Ast):
        # current : Type ::=  Generic ['?'] | Identifier ['?'];

        is_optional = len(tsl_type_ast) is 2
        tsl_type_ast = tsl_type_ast[0]

        if is_optional:
            if len(tsl_type_ast) is 2 and tsl_type_ast[-1] is '?':
                raise SyntaxError('double `optional` postfix!')

        if not isinstance(tsl_type_ast, Ast):
            # current = identifier
            # Identifier := R'[a-zA-Z_][a-z0-9A-Z_]*';

            type_name = tsl_type_ast

            type_spec = Types.get(type_name)

            if type_spec is None:
                raise TypeError('Unknown type {}.'.format(type_name))

            return type_spec.to_optional() if is_optional else type_spec

        # current = Generic
        #  Generic Throw ['<', '>', ','] ::= Identifier '<' [Type (',' Type)*] L'>';
        type_class, *type_args = tsl_type_ast
        type_spec_constructor = GenericManager.get(type_class)

        if type_spec_constructor is None:
            raise TypeError('Unknown generic type {}.'.format(type_class))

        tail = [TSLTypeConstructor(type_arg) for type_arg in type_args]

        constructor, checker = type_spec_constructor(*tail)
        signature = '{}<{}>'.format(type_class, ','.join(map(lambda typ: typ.sig, tail)))
        if signature in Types:
            return Types[signature]

        type_spec = FieldType(sig=signature,
                              constructor=constructor,
                              checker=checker,
                              is_optional=is_optional)

        type_register(type_spec)
        return type_spec
