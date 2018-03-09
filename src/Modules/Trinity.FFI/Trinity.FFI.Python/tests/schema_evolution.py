import import_ge
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

Trinity.Storage.Composite.CompositeStorage.AddStorageExtension("./tsl2", "TestTslModule")

cnt = sum([1 for i in Trinity.Global.StorageSchema.CellDescriptors])

print("after 2nd storage extension: {}".format(cnt))

