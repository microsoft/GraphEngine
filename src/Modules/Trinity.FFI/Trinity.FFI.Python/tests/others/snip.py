# -*- coding: utf-8 -*-
"""
Created on Wed Apr 18 16:35:40 2018

@author: twshe
"""

import initialize
import clr
import GraphEngine as ge
import Trinity
import Trinity.Storage
import Trinity.Storage.Composite
import Trinity.FFI.Metagen
import Trinity.FFI as tf


schema = Trinity.Storage.Composite.CompositeStorage.AddStorageExtension("./tsl4", "TestTslModule")

tf.JitTools.SwigGen(schema, "test")
