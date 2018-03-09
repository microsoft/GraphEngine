import import_ge
import clr
import GraphEngine as ge

import Trinity
import Trinity.Storage
import Trinity.Storage.Composite

Trinity.Storage.Composite.CompositeStorage.AddStorageExtension("./tsl", "./tsl", "TestTslModule")
Trinity.Global.LocalStorage.SaveStorage()
