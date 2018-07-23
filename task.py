from Redy.Tools.PathLib import Path
import os

os.system('pip uninstall GraphEngine -y')
try:
    Path(r"~/.nuget/packages/graphengine.ffi").delete()
except FileNotFoundError:
    pass

current = Path('.')
os.system(r"cd {}\build && cmake --build . --config Release".format(current))
os.system(
        r'cd {}\src\Modules\Trinity.FFI\Trinity.FFI && dotnet build Trinity.FFI.csproj && dotnet pack Trinity.FFI.csproj'.format(
                current))
bin = current.into('bin')
whl = sorted(map(str, bin.list_dir(lambda it: it.endswith(".whl"))))[-1]
os.system(f"cd {bin} && pip install {whl}")
os.system(r"cd {} && python task2.py".format(current))
