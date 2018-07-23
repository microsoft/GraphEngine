from GraphEngine.tsl.type.factory import *
from GraphEngine.DotNet.setup import init_trinity_service, build_module, Env
init_trinity_service()
tsl = TSL()
@tsl
class C(Cell):
    a: int

tsl.bind()
