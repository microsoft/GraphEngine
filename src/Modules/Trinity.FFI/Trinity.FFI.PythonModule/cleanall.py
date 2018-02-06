# -*- coding: utf-8 -*-
"""
Created on Tue Jan 30 20:50:24 2018

@author: twshe
"""

import os
from linq import Flow, extension_class
from conf import and_then
from conf import (CURRENT_DIR, CORECLR_PATH,
                  BUILD_SCRIPT_CMD, BUILD_SCRIPT_PATH, is_windows)
from conf import copy_to, recur_listdir, pardir_of
from cytoolz.curried import curry
from functools import partial
from collections import namedtuple
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
def endswith(suffix, filename):
    if isinstance(suffix, str):
        return filename.endswith(suffix)
    return any(filename.endswith(e) for e in suffix)


Strategy = namedtuple('strategy', ['dir', 'suffixes'])


def _strategy(arguments: dict):
    return Strategy(*arguments.keys(), *arguments.values())


Strategy.new = _strategy  # pretend to be Ruby:)
suffixes1 = ['.dll', '.lib', '.obj', '.iobj', '.ilib', '.pdb']
suffixes2 = ['.pyd']
suffixes3 = ['.json', '.exp', '.bsc']


def delete(strategy: Strategy):

    if strategy.suffixes is all:
        log(strategy.dir, operation='remove', then_call=partial(shutil.rmtree, ignore_errors=True))
        return

    (Flow(recur_listdir(strategy.dir))
        .Filter(endswith(strategy.suffixes))
        .Each(log.within(operation='remove', then_call=os.remove)))


additional = []
if cmdparser.parse_args().all:
    additional = [{CORECLR_PATH: suffixes1 + suffixes2},
                  {pardir_of(BUILD_SCRIPT_PATH): suffixes1}]

(Flow(map(Strategy.new,
          [
              *additional,
              {"./GraphEngine/ffi": suffixes1 + suffixes2 + suffixes3 + ['.sig']},
              {"./GraphEngine.egg-info": all},
              {"./__pycache__": all},
              {"./build": all},
              {"./dist": all},
              {'storage': all},
              {"GraphEngine/ffi/storage": all},
              {'cache': all},
              {'GraphEngine/ffi/A': all},
              {'GraphEngine/ffi/B': all},
              {'GraphEngine/ffi/composite-helper': all},
              {'GraphEngine/ffi/write_ahead_log': all}
          ]))
    .Each(delete))
