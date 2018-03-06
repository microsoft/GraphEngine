import clr, sys, os
from os.path import dirname, abspath, join, exists, expanduser

dep_packages = [ 
    'GraphEngine.Core/1.0.9083/lib/netstandard2.0/Trinity.Core.dll',
    'GraphEngine.Storage.Composite/1.0.9083/lib/netstandard2.0/Trinity.Storage.Composite.dll',
    'GraphEngine.FFI/1.0.9083/lib/netstandard2.0/Trinity.FFI.dll',
]
module_dir   = dirname(abspath(__file__))
dep_proj     = join(module_dir, "Dependencies.csproj")
nuget_root   = expanduser('~/.nuget/packages')
package_dirs = [join(nuget_root, f) for f in dep_packages] 

if not all([exists(f) for f in package_dirs]):
    os.system('dotnet restore "{}"'.format(dep_proj))

sys.path.append(module_dir)

for f in package_dirs:
    clr.AddReference(f)

Trinity = __import__('Trinity')
ffi = __import__('ffi')

ffi.Init()