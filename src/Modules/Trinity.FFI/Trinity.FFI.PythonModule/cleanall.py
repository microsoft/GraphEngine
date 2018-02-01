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
from cytoolz.curried import curry
from functools import partial
import argparse
import shutil

cmdparser = argparse.ArgumentParser(description='clean the generated.')
cmdparser.add_argument("--all",
                       help='clean all the binary dependences of GraphEngine.',
                       default=False, nargs='?',
                       const=True)


def log(var, operation=None, then_call=lambda x: x):
    return print(var if operation is None else '{} {}'.format(operation, var)) or then_call(var)


log.within = lambda **kwargs: partial(log, **kwargs)


@curry
def endswith(postfix, filename):
    if isinstance(postfix, str):
        return filename.endswith(postfix)
    return any(filename.endswith(e) for e in postfix)


clean_paths = ["./GraphEngine/ffi"]
if cmdparser.parse_args().all:
    clean_paths += [CORECLR_PATH, pardir_of(BUILD_SCRIPT_PATH)]
    
for clean_path in clean_paths:
    (linq.Flow(recur_listdir(
        pardir_of(clean_path)))
     .Filter(endswith(['dll', 'pyd', 'lib', 'obj', 'iobj', 'ilib', 'pdb']))
     .Each(log.within(operation='remove', then_call=os.remove)))

(linq.Flow(["./GraphEngine.egg-info",
            "./__pycache__",
            "./build",
            "./dist",
            "./GraphEngine/ffi/storage"])
 .Each(log.within(then_call=partial(shutil.rmtree, ignore_errors=True))))
