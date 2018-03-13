from initialize import *
import timeit
from pprint import pprint
import sys

file_name, cell_type = sys.argv[:2]
@test_fn
def test():
    number = int(3e6)
    cells = [new_cell_swig(cell_type) for i in range(number)]
    
    for i in range(number):
        save_cell_swig(i+1, cells[i])
    
    time = timeit.timeit(f"load_cell_swig(i); i+=1;",
                         number=number,
                         globals={'load_cell_swig': load_cell_swig},
                         setup='global i; i=1;')

    res = {file_name: (cell_type, time)}
    return pprint(res) or res