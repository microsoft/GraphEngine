from initialize import *
import timeit
from pprint import pprint
import sys

file_name, backend, cell_type, *content = sys.argv

if content:
    content = content[0]
else:
    content = None

new_cell, new_cell_id, new_cell_content, save_cell, load_cell = ops[backend]


@test_fn
def test():

    if content:
        number = int(3e6 // len(content))
        cells = [new_cell_content(cell_type, content) for i in range(number)]

        for i in range(number):
            save_cell(i, cells[i])

    else:
        number = int(3e6)
        cells = [new_cell(cell_type) for i in range(number)]

        for i in range(number):
            save_cell(i, cells[i])

    time = timeit.timeit(f"load_cell(i); i+=1;",
                         number=number,
                         globals={'load_cell': load_cell},
                         setup='global i; i=0;')

    title = file_name
    stats_info = dict(backend=backend,
                      cell_type=cell_type,
                      time_per_10000=time * 10000 / number,
                      data_size=content and len(content))

    res = {title: stats_info}

    if backend == 'redis':
        for i in range(number):
            redis_storage.delete(i)

    return pprint(res) or res
