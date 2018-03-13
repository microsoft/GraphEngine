from initialize import *
import timeit
from pprint import pprint
import sys

file_name, cell_type, content = sys.argv[:3]


@test_fn
def test():
    number = int(3e6)
    cells = [new_cell_content_swig(cell_type, content) for i in range(number)]

    for i in range(number):
        s = save_cell_swig(i+1, cells[i])
    
    time = timeit.timeit(f"print(i); load_cell_swig(i); i+=1;",
                         number=number,
                         globals={'load_cell_swig': load_cell_swig},
                         setup='global i; i=1;')
                         
    res = {file_name: (cell_type, time)}
    return pprint(res) or res
