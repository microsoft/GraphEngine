import os, linq, json

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

for each in singles:
    for cell_type in large_data.keys():
        os.system(f'python {each} {cell_type}')
        os.system(f'python {each} {cell_type}')


for each in with_contents:
    for k, v in tiny_data.items():
        os.system(f'python {each} {k} {json.dumps(v)}')

    for k, v in large_data.items():
        os.system(f'python {each} {k} {json.dumps(v)}')
