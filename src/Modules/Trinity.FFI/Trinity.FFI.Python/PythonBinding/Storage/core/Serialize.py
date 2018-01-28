# -*- coding: utf-8 -*-
"""
Created on Sun Jan 28 21:48:50 2018

@author: yatli
"""

import json

Serializer = {}
def mark_as_serializable(typ, serializing_method):
    Serializer[typ] = serializing_method

class TSLJSONEncoder(json.JSONEncoder):
    def default(self, obj):
        return Serializer[obj.__class__](obj)
    
        
    