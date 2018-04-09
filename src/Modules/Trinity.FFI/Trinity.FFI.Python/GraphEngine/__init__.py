import clr, sys, os, ctypes
from os.path import dirname, abspath, join, exists, expanduser

# when we implement hosting in Trinity.FFI.Native, we can remove the dependency on pythonnet

__dep_packages = [
    'FSharp.Core/4.3.4/lib/netstandard1.6/FSharp.Core.dll',
    'GraphEngine.Core/2.0.9328/lib/netstandard2.0/Trinity.Core.dll',
    'GraphEngine.Storage.Composite/2.0.9328/lib/netstandard2.0/Trinity.Storage.Composite.dll',
    'GraphEngine.Jit/2.0.9328/lib/netstandard2.0/GraphEngine.Jit.dll',
    'GraphEngine.FFI/2.0.9328/lib/netstandard2.0/Trinity.FFI.dll',
    'Newtonsoft.Json/9.0.1/lib/netstandard1.0/Newtonsoft.Json.dll',
    'Microsoft.Extensions.ObjectPool/2.0.0/lib/netstandard2.0/Microsoft.Extensions.ObjectPool.dll',
]
__module_dir   = dirname(abspath(__file__))
__dep_proj     = join(__module_dir, "Dependencies.csproj")
__nuget_root   = expanduser('~/.nuget/packages')
__package_dirs = [join(__nuget_root, f) for f in __dep_packages]

if not all([exists(f) for f in __package_dirs]):
    os.system('dotnet restore "{}"'.format(__dep_proj))

#todo detect os and determine .net rid
ge_native_lib  = join(__nuget_root, "GraphEngine.Core/2.0.9328/runtimes/win-x64/native/Trinity.dll")
ffi_native_lib = join(__nuget_root, "GraphEngine.FFI/2.0.9328/runtimes/win-x64/native/trinity_ffi.dll")
jit_native_lib = join(__nuget_root, "GraphEngine.Jit/2.0.9328/runtimes/win-x64/native/GraphEngine.Jit.Native.dll")

sys.path.append(__module_dir)
ctypes.cdll.LoadLibrary(ge_native_lib)
ctypes.cdll.LoadLibrary(ffi_native_lib)
ctypes.cdll.LoadLibrary(jit_native_lib)

for f in __package_dirs:
    clr.AddReference(f)

__Trinity = __import__('Trinity')
# set default storage root to cwd/storage
__Trinity.TrinityConfig.StorageRoot = join(os.getcwd(), "storage")
# set default logging level
__Trinity.TrinityConfig.LoggingLevel = __Trinity.Diagnostics.LogLevel.Info
# load default configuration file
__Trinity.TrinityConfig.LoadConfig(join(os.getcwd(), "trinity.xml"))
# then initialize Trinity
__Trinity.Global.Initialize()
__ffi = __import__('ffi')
__ffi.Init()
