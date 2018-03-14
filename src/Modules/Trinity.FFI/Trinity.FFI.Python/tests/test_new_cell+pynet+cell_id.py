from initialize import *
import timeit
from pprint import pprint
import sys

file_name, cell_type = sys.argv[:2]


@test_fn
def test():
    number = int(3e6)
    time = timeit.timeit(f"new_cell_id_pynet(i, '{cell_type}'); i+=1;",
                         number=number,
                         globals=globals(),
                         setup='global i; i=0;')
    title = file_name.replace('+pynet', '').replace('+swig', '')
    backend = 'pynet' if 'pynet' in file_name else 'swig'

    stats_info = dict(backend=backend,
                      cell_type=cell_type,
                      time_per_10000=time * 10000 / number,
                      data_size=None)

    res = {title: stats_info}
    return pprint(res) or res
