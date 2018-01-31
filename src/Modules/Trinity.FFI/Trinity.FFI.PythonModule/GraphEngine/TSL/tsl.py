# -*- coding: utf-8 -*-
"""
Created on Wed Jan 31 17:54:07 2018

@author: twshe
"""
from collections import namedtuple
from GraphEngine.Storage.core.TypeMap import CellType
from GraphEngine.Storage.core.Serialize import TSLJSONEncoder
import json
from GraphEngine.Command import code_gen

_join = ''.join


def to_tsl(obj):
    if isinstance(obj, str):
        return obj

    elif isinstance(obj, CellType):
        return _join((
            '{}\n'.format(obj.name),
            '{\n',
            '{}'.format(
                '\n'.join(
                    ['{} {};'.format(attr_type, attr_name)
                     for attr_name, attr_type in obj.attrs.items()])),
            '\n}\n'))

    elif isinstance(obj, dict):
        return _join((
            '{}\n'.format(obj['name']),
            '{\n',
            '{}'.format(
                '\n'.join(
                    ['{} {};'.format(attr_type, attr_name)
                     for attr_name, attr_type in obj['attrs'].items()])),
        ))

    else:
        raise TypeError('invalid parameters.\n',
                        '\texpected: \n',
                        '\tTSL(str)      | '
                        '\tTSL(CellType) | '
                        '\tTSL(dict, name=str)\n')


class TSL:
    def __init__(self, definition):
        if isinstance(definition, list):
            self.codes = '\n'.join([to_tsl(cell_def) for cell_def in definition])

    def create(self, at='./'):
        """
         TODO:
            Create a new piece of assembly and get the cell descriptors.
        """

    def __str__(self):
        return self.codes

    def __repr__(self):
        return self.__str__()
