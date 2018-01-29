# -*- coding: utf-8 -*-
"""
Created on Sun Jan 28 21:48:50 2018

@author: yatli/thautwarm
"""

import json
import warnings

Serializer = {int: None,
              float: None,
              str: None,
              dict: None,
              list: None,
              tuple: None
              }


def mark_as_serializable(typ, serializing_method):
    Serializer[typ] = serializing_method


class TSLJSONEncoder(json.JSONEncoder):
    def encode(self, obj):
        cls = Serializer[obj.__class__]
        if cls is None:
            return json.JSONEncoder.encode(self, obj)
        return cls(obj)

    def default(self, o):
        warnings.warn(
                'unsolved type for JSON encoder, use `{}.__str__` as default'.format(o.__class__))
        return str(o)
