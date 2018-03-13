from initialize import *
import timeit
from pprint import pprint
import sys

file_name, cell_type = sys.argv[:2]


@test_fn
def test():
    number = int(3e6)
    cells = [new_cell_swig(cell_type) for i in range(number)]
    time = timeit.timeit(f"save_cell_swig(i, cells[i])",
                         number=number,
                         globals={'cells': cells, 'save_cell_swig': save_cell_swig},
                         setup='global i; i=0;')
    res = {file_name: (cell_type, time)}
    return pprint(res) or res
