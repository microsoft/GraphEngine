from functools import reduce

def postfix(a, b):
    return b(a)

def and_then(*f):
    def apply(x):
        return reduce(postfix, f, x)

    return apply

def flatten(recur_collections, recursive_collection_types=(list, tuple)):
    
    def _flat(recur):
        for e in recur:
            if e.__class__ in recursive_collection_types:
                yield from _flat(e)
            else:
                yield e
        
    return _flat(recur_collections)   