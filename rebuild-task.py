from Redy.Tools.PathLib import Path
import argparse
import os

os.system('pip uninstall GraphEngine -y')
nuget = Path(r"~/.nuget/packages")

for each in ('graphengine.ffi', 'graphengine.ffi.metagen'):
    try:
        nuget.into(each).delete()
    except FileNotFoundError:
        pass

current = Path('.')
bin = current.into('bin')
try:
    for each in bin.list_dir(lambda it: 'FFI' in it):
        try:
            each.delete()
        except FileNotFoundError:
            pass
except OSError:
    pass

os.system(r"cd {}\build && cmake --build . --config Release".format(current))
# os.system(r'cd {} && powershell -F build.ps1'.format(current.into('tools')))
os.system(r'cd {0}\src\Modules\Trinity.FFI\Trinity.FFI '
          r'&& dotnet build Trinity.FFI.csproj '
          r'&& dotnet pack Trinity.FFI.csproj'
          r'&& cd {0}\src\Modules\Trinity.FFI\Trinity.FFI.MetaGen'
          r'&& dotnet build Trinity.FFI.MetaGen.fsproj'
          r'&& dotnet pack Trinity.FFI.MetaGen.fsproj'.format(current))

whl = sorted(map(str, bin.list_dir(lambda it: it.endswith(".whl"))))[-1]
os.system(f"cd {bin} && pip install {whl}")
os.system(r"cd {} && python python-run-test.py".format(current))
