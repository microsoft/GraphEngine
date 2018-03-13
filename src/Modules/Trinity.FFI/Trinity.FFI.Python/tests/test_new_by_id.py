from initialize import *
import timeit
from pprint import pprint
import sys

file_name, backend, cell_type, *content = sys.argv

if content:
    raise AssertionError
else:
    content = None

new_cell, new_cell_id, new_cell_content, save_cell, load_cell = ops[backend]


@test_fn
def test():
    if backend == 'redis':
        return {}

    number = int(3e6)

    time = timeit.timeit(f"new_cell_id(i); i+=1;",
                         number=number,
                         globals={
                             'new_cell_id': new_cell_id
                         },
                         setup='global i; i=0;')

    title = file_name
    stats_info = dict(backend=backend,
                      cell_type=cell_type,
                      time_per_10000=time * 10000 / number,
                      data_size=None)

    res = {title: stats_info}

    return pprint(res) or res
