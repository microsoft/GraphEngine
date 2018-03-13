from initialize import *
import timeit
from pprint import pprint
import sys

file_name, cell_type, content = sys.argv[:3]


@test_fn
def test():
    number = int(3e6)
    cells = [new_cell_content_pynet(cell_type, content) for i in range(number)]
    time = timeit.timeit(f"save_cell_pynet(i, cells[i])",
                         number=number,
                         globals={'cells': cells, 'save_cell_pynet': save_cell_pynet},
                         setup='global i; i=0;')
    res = {file_name: (cell_type, time)}
    return pprint(res) or res
