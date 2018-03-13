import os, linq, json
from subprocess import call

def cmd(args):
	call(args)


tiny_data = {
    'Normal': {
        'lst': [1, 2, 3],
        'bar': "1213"
    },

    'Tiny':{
        'foo': 42
    },

    'Large':{
        f'bar{i}': list(range(10)) for i in range(1, 12)
    }
}

large_data = {
    'Normal': {
        'lst': list(range(1000)),
        'bar': "1213"
    },

    'Tiny':{
        'foo': 42
    },

    'Large':{
        f'bar{i}': list(range(1000)) for i in range(1, 12)
    }
}

test_files = list(filter(lambda x: x.startswith('test_'), os.listdir('.')))
split_set = linq.Flow(test_files).GroupBy(lambda _: 'content' in  _).Unboxed()

with_contents = split_set[True]
singles = split_set[False]

with open('result.json', 'w') as f:
	f.write('{}')

for each in singles:
    
    # TODO debug on swig mode
    if 'swig' in each and 'load' in each:
        continue

    for cell_type in large_data.keys():
        cmd(['python', each, cell_type])
        cmd(['python', each, cell_type])


for each in with_contents:
    
    if 'swig' in each and 'load' in each:
        continue

    for k, v in tiny_data.items():
        cmd(['python', each, k, json.dumps(v)])

    for k, v in large_data.items():
        cmd(['python', each, k, json.dumps(v)])