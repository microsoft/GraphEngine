# -*- coding: utf-8 -*-
import os, sys

directory = os.path.split(__file__)[0]


def code_gen():
    os.system('{} {}'.format(os.path.join(directory, 'Trinity.TSL.CodeGen.exe'), ' '.join(sys.argv[2:])))