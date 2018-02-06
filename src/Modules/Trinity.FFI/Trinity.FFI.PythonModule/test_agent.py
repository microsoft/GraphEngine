import GraphEngine.ffi
import Trinity
import os, clr
from linq import Flow
from Trinity.FFI import Agent
import Trinity


IncludeDirectory = os.path.join(GraphEngine.__path__[0], 'ffi')
StorageRoot = os.path.abspath('storage')
TSLCodeGenExeLocation = os.path.join(GraphEngine.__path__[0], 'Command', 'Trinity.TSL.CodeGen.exe')
DotNetExeLocation = 'dotnet.exe'
Agent.Configure(IncludeDirectory, StorageRoot, TSLCodeGenExeLocation, DotNetExeLocation, 10, 10, 10)

Trinity.Global.LocalStorage.LoadStorage()
Agent.Initialize()

if not os.path.exists(r"storage\composite-helper\Trinity.Extension.abc.dll"):
    tsl_path = os.path.abspath('./tests/tsl')
    Agent.LoadTSL(*[tsl_path, tsl_path, "abc", None])

Flow(Trinity.Global.StorageSchema.CellDescriptors).Each(
    lambda cell_desc: print(f'{cell_desc.TypeName}{list(cell_desc.GetFieldNames())}'))

#C1 = Trinity.Global.LocalStorage.NewGenericCell('C1')
#Trinity.Global.LocalStorage.SaveStorage()
#Agent.Uninitialize()