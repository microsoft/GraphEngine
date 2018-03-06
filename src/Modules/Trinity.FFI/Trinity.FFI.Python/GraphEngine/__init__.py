import clr, sys, os
from os.path import dirname, abspath, join, exists, expanduser

# when we implement hosting in Trinity.FFI.Native, we can remove the dependency on pythonnet

__dep_packages = [
    'Newtonsoft.Json/9.0.1/lib/netstandard1.0/Newtonsoft.Json.dll',
    'GraphEngine.Core/1.0.9083/lib/netstandard2.0/Trinity.Core.dll',
    'GraphEngine.Storage.Composite/1.0.9083/lib/netstandard2.0/Trinity.Storage.Composite.dll',
    'GraphEngine.FFI/1.0.9083/lib/netstandard2.0/Trinity.FFI.dll',
]
__module_dir   = dirname(abspath(__file__))
__dep_proj     = join(__module_dir, "Dependencies.csproj")
__nuget_root   = expanduser('~/.nuget/packages')
__package_dirs = [join(__nuget_root, f) for f in __dep_packages]

if not all([exists(f) for f in __package_dirs]):
    os.system('dotnet restore "{}"'.format(__dep_proj))

sys.path.append(__module_dir)

for f in __package_dirs:
    clr.AddReference(f)

__Trinity = __import__('Trinity')
# set default storage root to cwd/storage
__Trinity.TrinityConfig.StorageRoot = join(os.getcwd(), "storage")
# set default logging level
__Trinity.TrinityConfig.LoggingLevel = __Trinity.Diagnostics.LogLevel.Debug
# load default configuration file
__Trinity.TrinityConfig.LoadConfig(join(os.getcwd(), "trinity.xml"))
# then initialize Trinity
__Trinity.Global.Initialize()
__ffi = __import__('ffi')
__ffi.Init()
