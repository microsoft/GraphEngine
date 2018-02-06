# -*- coding: utf-8 -*-
"""
Created on Wed Jan 31 17:54:07 2018

@author: twshe
"""
from GraphEngine.TSL.TypeMap import CellType

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

    elif isinstance(obj, TSL):
        return obj.codes
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
        with open(at, 'w') as to_write:
            to_write.write(self.codes)

    def __str__(self):
        return self.codes

    def __repr__(self):
        return self.__str__()
