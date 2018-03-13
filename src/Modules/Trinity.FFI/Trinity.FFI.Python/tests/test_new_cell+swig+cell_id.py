from initialize import *
import timeit
from pprint import pprint
import sys

file_name, cell_type = sys.argv[:2]


@test_fn
def test():
    time = timeit.timeit(f"new_cell_id_swig(i, '{cell_type}')",
                         number=int(3e6),
                         globals=globals(),
                         setup='global i; i=0;')
    res = {cell_type: (cell_type, time)}
    return pprint(res) or res
