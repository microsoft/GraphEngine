# -*- coding: utf-8 -*-
"""
Created on Tue Jan 30 15:14:14 2018

@author: v-wazhao
"""

from Ruikowa.ObjectRegex.ASTDef import Ast
from cytoolz.curried import curry
from collections import namedtuple

CellType = namedtuple('CellType', ['name', 'attrs'])

# begin configure
# these settings are for handling type mapping automatically.
# TODO: do not use parser to speed up type mapping.


curry_map = curry(map)


@curry
def isa(typ, v):
    return isinstance(v, typ)


Types = {'int': int,
         'string': str,
         'float': float,
         'double': float,
         'list': list}

GenericManager = {'List': lambda typ: (list, lambda _: all(curry_map(isa(typ))(_))),
                  'Dictionary': lambda typ: (dict, lambda _: all(curry_map(isa(typ))(_)))}


# end configure

def type_register(typ):
    if typ.name not in Types:
        Types[typ.name] = typ


# type class
class TSLTypeConstructor:
    def __new__(cls, tsl_type_ast: Ast):
        # current : Type ::=  Generic ['?'] | Identifier ['?'];

        is_optional = len(tsl_type_ast) is 2
        tsl_type_ast = tsl_type_ast[0]
        
        if is_optional:
            if len(tsl_type_ast) is 2 and tsl_type_ast[-1] is '?':
                raise SyntaxError('double `optional` prefix!')

        if not isinstance(tsl_type_ast, Ast):
            # current = identifier
            # Identifier := R'[a-zA-Z_][a-z0-9A-Z_]*';

            type_name = tsl_type_ast

            type_constructor = Types.get(type_name)

            if type_constructor is None:
                raise TypeError('Unknown type {}.'.format(type_name))

            type_checker = lambda v: isinstance(v, type_constructor)

        else:
            #  current = Generic
            #  Generic Throw ['<', '>', ','] ::= Identifier '<' [Type (',' Type)*] L'>';
            type_class, *type_args = tsl_type_ast
            _ = GenericManager.get(type_class)
            if _ is None:
                raise TypeError('Unknown generic type {}.'.format(type_class))

            constructor_and_checker = _

            tail = [TSLTypeConstructor(type_arg) for type_arg in type_args]

            type_constructor, type_checker = constructor_and_checker(*tail)
            type_name = '{}<{}>'.format(type_class, ','.join(map(lambda typ: typ._name, tail)))

        type_name = type_name if not is_optional else '{}?'.format(type_name)

        class TSLType:
            _name = type_name

            def __init__(self, *args, **kwargs):
                self._v = type_constructor(*args, **kwargs)
                self._is_optional = is_optional

            @property
            def is_optional(self):
                return self._is_optional

            @property
            def name(self):
                return self._name

            def mut_by(self, fn):
                res = fn(self._v)
                if not type_checker(res):
                    raise TypeError('TypeError, get `{}`'.format(res.__class__))
                self._v = res

            def apply(self, fn):
                return fn(self._v)

            def check(self):
                return type_checker(self._v)

            def set(self, v):
                if not type_checker(v):
                    raise TypeError('TypeError, get `{}`'.format(v.__class__))

                self._v = v

            def __str__(self):
                return '{}[{}]'.format(self._name, self._v)

            def __repr__(self):
                return self.__str__()

            def __getattr__(self, item):

                return getattr(self._v, item)

        type_register(TSLType)
        return TSLType
