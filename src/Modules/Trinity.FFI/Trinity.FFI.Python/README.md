
# Reisen

**Reisen** is a service framework base on [GraphEngine](https://github.com/Microsoft/GraphEngine) which is basically for
 storage and persistency in modern applications.  

## Storage like Redis with strong type

When it comes to some logistic scenes, a specific structure is needed for efficient storage.

```python

import redis

# we need a structure with a mutable sequential container member `lst` and
spec = {'lst': [1, 2, 3, 4, 5],
        'bar': '123'}

redis_server = redis.StrictRedis()
redis_server.append('instances', spec)
```

[Redis](https://redis.io/topics/data-types) is commonly used for storage but without rich data types. For instance Redis has `List`, and it's simply the list of strings. The only primitive type of Redis is `string`! When you want to set or get something on operating the local/cloud strorage, the data stored by redis will be directly transformed to the specific language runtime.  
Thanks to the fundamental components of **GrapheEngine**, we can have strongly typed data structure to do more efficient operations. **Reisen** is totally different from Redis. 

When you're using `Reisen` to storing, you're supposed to define the structure of the data.

- some_types.tsl

```TSL
cell C1{
    List<int> lst,
    string bar;
}
```
And then use simply compile it in Python.

```python
import GraphEngine as ge
gm = ge.GraphMachine('<your storage directory>')
gm.start()  # you can make your own decision about when to start the storage service.

spec = {'lst': [1, 2, 3, 4, 5],
        'bar': '123'}

ge.Storage.load_symbols('xxxx/some_types.tsl')  # load tsl simply
#  additionally, you don't have to write write TSL files for yourself, it's welcome to
#  use GrapheEngine.TSL module, you can simply use a dictionary to generate a cell type,
#  just like this way:
#       ge.TSL.create_tsl[type](C1 = {'lst': list, 'bar': str}, C2 = ...). to('some_types.tsl')
#   or  ge.TSL.create_tsl[object](C1 = spec, C2=...). to('some_types.tsl')


C1 = ge.symtable['C1']  # ge.symtable.C1 is ok.
c1 = ge.Storage.Cell(C1)  # create an instance of C1.

for field in spec:
    c1[field] = spec[field]

c1.save()  # save cell c1 to the storage.
```

The storage operation with a naive implementation of Reisen is almost 3 times faster than Redis C/S. Sooner the speed could be a hundredfold.  

Other storing related methods in Reisen is definitely much much faster than Reisen(take care again it's only a naive Reisen implementation now!)

There is a [further introduction](./notavailablenow) of Reisen Storage, it will help you to know what's the accessor of GraphEngine Storage Service and how to use it to access something in your local/cloud storage much faster than normal way.  


## Service Integration

A reason why to use **Reisen** is for Distributed Computing, Data Mining and Machine Learning.  

It could be a hard task to dealing with billions of big data block: managing the storage most efficiently, applying the corresponding methods with least cost, make a maximum use of parallel and distributed computing and combining your service with other functional modules.

**Reisen** is furtherly developed for solving the above problems.

Here is an exmaple for handling a NLP task, which is to find out the hotest 10 topics in programming languages area in the last year.

```python
import GraphEngine as ge
import reisen
from nltk.tokenize import word_tokenize
from gensim import corpora, models
import datetime, time
gm = ge.GraphMachine('<your storage directory>')  # create local node(machine)
gm.start()
reisen.use_machine(gm)  # set the beginning node(machine) of distributed query.

@reisen.AOT
def query_data_from_storage(field):
    """Get the related corpus about specific field from cloud servers.
    """
    data : ge.dtype.List = []
    now = datetime.datetime.now()
    one_year = datetime.timedelta(365)
    last_year = now - one_year

    def filter_fn(block):
        """conditions to filter corpus blocks.
        """
        if block.lang == 'english' and block.time > last_year and block.tag.like(field):
            return block
        
    for server in gm.cloud.servers.available:  
    # traverse all the available servers(in fact it'll be done in distributed/parallel ways)
        if server.create_time > last_year:
            continue
        corpus = server.storage.corpus
        blocks = corpus.where(filter_fn).fetch
        data.AddRange(blocks)
    
    return data

data = query_data_from_storage(field = 'programming language')

# do something with python asynchronously
"""
    ...
    do something

"""

while True:
    if not data.got:
        time.sleep(200)
    break

data: list = data.result
docs = [word_tokenize(doc) for doc in data]
id2word = corpora.Dictionary(docs)
matrix_market = [id2word.doc2bow(doc) for doc in docs]
trained = models.ldamodel.LdaModel(corpus=matrix_market, id2word=id2word, num_topics=10)
print('the hotest 10 topics in programming language are')
for top in trained.print_topics():
    print(top)

```



## Not Only for Python

In the near future there will be multi-language supports(`Node.js, Ruby and more`) and the efficiency could be amazingly high.




