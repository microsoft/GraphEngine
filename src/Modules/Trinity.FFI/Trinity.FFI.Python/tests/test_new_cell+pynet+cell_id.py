from initialize import *
import timeit
from pprint import pprint
import sys

file_name, cell_type = sys.argv[:2]


@test_fn
def test():
    time = timeit.timeit(f"new_cell_id_pynet(i, '{cell_type}'); i+=1;",
                         number=int(3e6),
                         globals=globals(),
                         setup='global i; i=0;')
    res = {file_name: (cell_type, time)}
    return pprint(res) or res
