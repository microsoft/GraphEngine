import initialize
import clr
import GraphEngine as ge

import Trinity
import Trinity.Storage
import Trinity.Storage.Composite

import System
import System.Linq
from System.Linq import *

Trinity.Storage.Composite.CompositeStorage.AddStorageExtension("./tsl", "TestTslModule")

descriptors = ge.__ffi.GetCellDescriptors()

print("#desc = {}".format(len(descriptors)))

for desc in descriptors:
    print(desc.Name)
    print(desc.Handle)
    for f in ge.__ffi.GetFieldDescriptors(desc):
        print('    {} {}'.format(f.TypeName, f.Name))
