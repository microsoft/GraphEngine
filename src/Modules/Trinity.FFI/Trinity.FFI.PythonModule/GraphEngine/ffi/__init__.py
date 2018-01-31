import clr, sys, os

sys.path.append(os.path.split(__file__)[0])
clr.AddReference("Trinity.FFI")
clr.AddReference("TslAssembly")
from .GraphEngine import Init
from Trinity.FFI import FFIMethods
import TslAssembly
FFIMethods.Initialize()
Init()

