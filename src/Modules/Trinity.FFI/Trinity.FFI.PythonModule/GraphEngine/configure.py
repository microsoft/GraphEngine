# -*- coding: utf-8 -*-
"""
Created on Tue Feb  6 20:37:52 2018

@author: v-wazhao
"""

import os, json
import GraphEngine as ge
from sys import version_info

PY3 = version_info.major is 3
MODERN_PY = version_info.major is 3 and version_info.minor > 5
home = os.path.expanduser("~")

# For `chardet` cannot be accurate enough
_encodings = ('utf8', 'gb18030', 'latin1')


class Settings:
    spec = {'include', 'storage_root',
            'tsl_tool_path', 'dotnet_tool_path',
            'avg_max_asm_num', 'avg_cell_num',
            'avg_field_num'}

    # The path of reference dlls
    include = os.path.join(ge.__path__[0], 'ffi')

    # The path of current storage path
    storage_root = None

    # The path of TSL code generating tool
    tsl_tool_path = os.path.join(ge.__path__[0], 'Command', 'Trinity.TSL.CodeGen')

    # The path of `dotnet`
    dotnet_tool_path = 'dotnet'

    # The average number of max assembly count
    avg_max_asm_num = 10

    # The average number of cells in each assembly
    avg_cell_num = 20

    # The average number of fields in each cell
    avg_field_num = 3

    # Path of configuration file 
    configure_path = os.path.join(home, '.graphengine-py')

    # Configured or not
    configured = False

    def __new__(cls):
        raise TypeError

    @staticmethod
    def configure(attempt_count=0):

        if (Settings.include and
                Settings.storage_root and
                Settings.tsl_tool_path and
                Settings.dotnet_tool_path and
                Settings.avg_max_asm_num and
                Settings.avg_cell_num and
                Settings.avg_field_num):

            from GraphEngine.ffi import Agent
            Agent.Configure(Settings.include,
                            Settings.storage_root,
                            Settings.tsl_tool_path,
                            Settings.dotnet_tool_path,
                            Settings.avg_max_asm_num,
                            Settings.avg_cell_num,
                            Settings.avg_field_num)
            Settings.configured = True
        else:

            for encoding in ('utf8', 'gb18030', 'latin1'):
                with open(Settings.configure_path, 'r', encoding=encoding) as m_file:
                    configure = json.load(m_file)
                for k, v in configure.items():
                    if k in Settings.spec and isinstance(v, str):
                        setattr(Settings, k, v)
                if not all(getattr(Settings, k) for k in Settings.spec):
                    raise EnvironmentError(
                        "Configure file at {} wasn't filled with enough informations.".format(Settings.configure_path))
                Settings.configure(attempt_count + 1)
        if attempt_count > 3:
            raise EnvironmentError("Unknown error, check your path settings at {}".format(Settings.configure_path))
