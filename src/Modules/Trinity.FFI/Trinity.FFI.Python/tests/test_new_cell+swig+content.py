from initialize import *
import timeit
from pprint import pprint
import sys

file_name, cell_type, content = sys.argv[:3]


@test_fn
def test():
    time = timeit.timeit(f"new_cell_content_swig('{cell_type}', '{content}')",
                         number=int(3e6),
                         globals=globals(),
                         setup='global i; i=0;')
    res = {file_name: (cell_type, time)}
    return pprint(res) or res
