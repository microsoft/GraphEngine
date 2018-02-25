__all__ = ['CacheStorageManager', 'Agent']

import clr, sys, os
sys.path.append(os.path.split(__file__)[0])
clr.AddReference("Trinity.Core")
clr.AddReference("Trinity.Storage.Composite")
clr.AddReference("Trinity.FFI")
Trinity = __import__('Trinity')
# from .GraphEngine import Init
# Trinity.FFI.FFIMethods.Initialize()
# Init()