import os, linq, json
from subprocess import call


def cmd(args):
    call(args)


tiny_data = {
    'C1': {
        'lst': [1, 2, 3],
        'bar': "1213"
    },

    'C2': {
        'foo': 42
    },

    'C3': {
        f'bar{i}': list(range(10)) for i in range(1, 12)
    }
}

large_data = {
    'C1': {
        'lst': list(range(500)),
        'bar': "1213"
    },

    'C2': {
        'foo': 42
    },

    'C3': {
        f'bar{i}': list(range(500)) for i in range(1, 12)
    }
}

test_files = list(filter(lambda x: x.startswith('test_'), os.listdir('.')))
split_set = linq.Flow(test_files).GroupBy(lambda _: 'content' in _).Unboxed()

with_contents = split_set[True]
singles = split_set[False]

with open('result.json', 'w') as f:
    f.write('{}')

for spec_script in ('test_load.py', 'test_new.py', 'test_save.py', 'test_new_by_id.py'):

    for cell_type in ('C1', 'C2', 'C3'):

        for backend in ('co', 'swig', 'redis', 'pynet'):

            # cmd(['python', spec_script, backend, cell_type])
            if cell_type != 'C2':
                cmd(['python', spec_script, backend, cell_type, json.dumps(tiny_data[cell_type])])
                cmd(['python', spec_script, backend, cell_type, json.dumps(large_data[cell_type])])
            else:
                cmd(['python', spec_script, backend, cell_type, json.dumps(tiny_data[cell_type])])

# for each in singles:
#     # TODO debug on swig mode
#     if 'swig' in each and 'load' in each:
#         continue
#
#     for cell_type in large_data.keys():
#         cmd(['python', each, cell_type])
#
# for each in with_contents:
#
#     if 'swig' in each and 'load' in each:
#         continue
#
#     for k, v in tiny_data.items():
#         cmd(['python', each, k, json.dumps(v)])
#
#     for k, v in large_data.items():
#         cmd(['python', each, k, json.dumps(v)])
