from initialize import *
import timeit
from pprint import pprint
import sys

file_name, cell_type, content = sys.argv[:3]


@test_fn
def test():
    number = int(3e7//content)
    cells = [new_cell_content_pynet(cell_type, content) for i in range(number)]
    time = timeit.timeit(f"save_cell_pynet(i, cells[i])",
                         number=number,
                         globals={'cells': cells, 'save_cell_pynet': save_cell_pynet},
                         setup='global i; i=0;')
    title = file_name.replace('+pynet', '').replace('+swig', '')
    backend = 'pynet' if 'pynet' in file_name else 'swig'
    
    stats_info = dict(backend = backend, 
                      cell_type=cell_type, 
                      time_per_10000 = time*10000/number, 
                      data_size = len(content))

    res = {title: stats_info}
    return pprint(res) or res
