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
    if backend == 'redis':
        return {}

    if content:
        number = int(3e6 // len(content))
        time = timeit.timeit(f"new_cell_content('{cell_type}', '{content}');",
                             number=number,
                             globals={'new_cell_content': new_cell_content})

    else:
        number = int(3e6)
        time = timeit.timeit(f"new_cell('{cell_type}');",
                             number=number,
                             globals={'new_cell': new_cell})

    title = file_name
    stats_info = dict(backend=backend,
                      cell_type=cell_type,
                      time_per_10000=time * 10000 / number,
                      data_size=content and len(content))

    res = {title: stats_info}

    return pprint(res) or res
