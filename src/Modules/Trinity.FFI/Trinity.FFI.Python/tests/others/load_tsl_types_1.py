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

cnt = sum([1 for i in Trinity.Global.StorageSchema.CellDescriptors])

print("after adding storage extension: {}".format(cnt))

Trinity.Global.LocalStorage.SaveStorage()

cnt = sum([1 for i in Trinity.Global.StorageSchema.CellDescriptors])

print("after save: {}".format(cnt))

Trinity.Global.LocalStorage.LoadStorage()

cnt = sum([1 for i in Trinity.Global.StorageSchema.CellDescriptors])

print("after load: {}".format(cnt))

descriptors = ge.__ffi.GetCellDescriptors()

print("#desc = {}".format(len(descriptors)))

for desc in descriptors:
    print(desc)
