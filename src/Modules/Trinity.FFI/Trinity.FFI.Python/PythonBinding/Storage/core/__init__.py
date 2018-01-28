#

"""

TODO: 
    TSL generation and sending msg to Graph Machine to compile TSL. 
"""

from .CellSymTable import sync, SymTable, CellType
from .Serialize import mark_as_serializable, Serializer, TSLJSONEncoder

def using(closure):
    if callable(closure):
        closure().update(globals())
    
    else:
        closure.update(globals())        

