import clr, sys

clr.AddReference("Trinity.Core")
clr.AddReference("Trinity.Storage.Composite")
clr.AddReference("Trinity.FFI")

Trinity = __import__('Trinity')

import ffi

ffi.Init()