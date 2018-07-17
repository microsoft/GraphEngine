
# How To Represent TSL Data In Python

## Root Object and Proxy

Root objects are data containers whose inner data is guaranteed to be
continuous.

```
// in the slides I will use following data schemas to apply introductions
cell C
{
  int i;
  List<int> li;
  List<S> ls;
  S s;
}

struct S
{
    K k;
    int i;
}

struct K
{
    int i;
    int j;
}
```

If we allocate something whose structure is defined by `tsl`,
it's obviously a root.

```
python codes:
    k: K = K(i = 1)

the memory representation of k:
    k : [ i:(+ + + +), j:(+ + + +) ]

 e.g: `int` refers to `int32_t` and take 4 bytes.
```

For above `k` is a **root** object, when you are accessing field/element from `k`,

- if
    the field/element is of primitive type [1]
  you get a copy of data and transform to python(or other foreign lang) runtime object.

- else
    you get a proxy which represents the accessing chain.
    e.g `s: S; k: Proxy[S, K] = s.k`, where the proxy `k` stored the chain `[S, K]` for further
    data accessing.

    The reason why don't really pass the actual (reference of data) of `s.k` to `k` is,
    if so, the rewriting occurred at other parts of root would mutate the actual data of `s.k`.


# Data Accessing Mechanism

Once a tsl type is defined in Python, we generate all the perspective proxy types of it,
and bind corresponding data accessing methods on each proxy type.

For instance, we now have a `cell C` as root type(also, `struct S`, `struct K` could be
root type, respectively), we will export many methods for it including

```
...

cell_C_BSet # set reference of root
cell_C_BGet # deepcopy and return reference
...
cell_C_SSet_s # set S.k
...
cell_C_SGet_s_SGet_k # Get reference of S.k
cell_C_SGet_s_SGet_i_BGet # get deepcopy of S.i
cell_C_SGet_s_SGet_k_SGet_i_BGet # get deepcopy of S.k.i
cell_C_SGet_s_SGet_k_SGet_j_BGet # get deepcopy of S.k.j
...
```
Now we create a proxy type `Proxy[C, S]`,  it may look like following one:

```python
class Proxy[C, S]: # not exactly python codes but for expressing
    __root_obj__ : C

    def _get_ref(self):
        return cell_C_SGet_s(self.__root_obj__)
    def _set_ref(self, value):
        ... # it's too early to define it here :)

    @property
    def i(self):
        return cell_C_SGet_s_SGet_i_BGet(self.__root_obj__)

    @property
    def k(self) -> Proxy[C, S, K]:
        return Proxy[C, S, K](self.__root_obj__)
    ...
```


Sometimes you might want to assign a root object to proxy object, or the vice versa.
```
s: S
c: C
c.s = s # assign root to proxy
```
which could be equivalent to
```
c.s._set_ref(s._get_ref())
```

And
```
ls: List[S]
ls[0] = c.s # assign proxy to root
```
could be equivalent to
```
lst[0]._set_ref(c.s._get_ref())
```

Where we give the definition of `Proxy[C, S]._set_ref`
```
def Proxy[C, S]._set_ref(self, value):
    return cell_C_SGet_s_SSet_k(self.__root_obj__, value._get_ref())
```


[1]: `string`, `int1`, `int8`, `int16`, `int32`, `int64`, `float`, ...
