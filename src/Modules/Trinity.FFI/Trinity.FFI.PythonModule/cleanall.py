# -*- coding: utf-8 -*-
"""
Created on Tue Jan 30 20:50:24 2018

@author: twshe
"""

import os
import linq

from conf import and_then
from conf import (CURRENT_DIR, CORECLR_PATH,
                  BUILD_SCRIPT_CMD, BUILD_SCRIPT_PATH, is_windows)
from conf import copy_to, recur_listdir, pardir_of

for clean_path in [CORECLR_PATH, BUILD_SCRIPT_PATH]:
    (linq.Flow(recur_listdir(
        pardir_of(clean_path)))
     .Filter(lambda _: _.endswith('.exe') or
                       _.endswith('.dll') or
                       _.endswith('.pyd') or
                       _.endswith('.pdb') or
                       _.endswith('.lib') or
                       _.endswith('.obj') or
                       _.endswith('.iobj') or
                       _.endswith('.ilib'))
     .Each(os.remove))

(linq.Flow(["./GraphEngine.egg-info",
            "./__pycache__",
            "./build",
            "./dist",
            "./GraphEngine/ffi/storage"])
 .Each(lambda _: (lambda _2: print(_2) or os.system(_2))('rm -rf {}'.format(_))))
