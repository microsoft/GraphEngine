from Redy.Tools.PathLib import Path
from GraphEngine.tsl.type.factory import *
from GraphEngine.DotNet.setup import init_trinity_service

# import os
# nuget = Path("~/.nuget/packages")
# for each in nuget.list_dir(lambda it: 'graphengine.ffi' in it):
#     print(f'deleting {each}')
#     each.delete()
#
# os.system('powershell -F ../build.ps1')

init_trinity_service()

tsl = TSL()


@tsl
class C(Cell):
    a: int
    s: 'S'


@tsl
class S(Struct):
    i: int


tsl.bind()

c = C()
