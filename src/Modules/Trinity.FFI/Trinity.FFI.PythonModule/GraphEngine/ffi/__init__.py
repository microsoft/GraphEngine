import clr, sys, os

sys.path.append(os.path.split(__file__)[0])
clr.AddReference("Trinity.Core")
clr.AddReference("Trinity.Storage.Composite")
clr.AddReference("Trinity.FFI")
from .GraphEngine import Init
from Trinity.FFI import FFIMethods
# from Trinity.StorageVersionController import Center
FFIMethods.Initialize()
Init()

