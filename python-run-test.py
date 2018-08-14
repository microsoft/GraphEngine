from GraphEngine.DotNet.setup import init_trinity_service, Env
from GraphEngine.tsl.type.factory import TSL, Cell, build_module
init_trinity_service()

tsl = TSL()

@tsl
class C(Cell):
    a: int



m = build_module(tsl.to_tsl, 'mer', False)

from GraphEngine.tsl.inline_to_bytes import make_fast_to_bytes_function

fn = make_fast_to_bytes_function(C, {})

x = b''.join(fn({"a": 1}))

print('done', m.use_cell_C_with_data(1, x))
