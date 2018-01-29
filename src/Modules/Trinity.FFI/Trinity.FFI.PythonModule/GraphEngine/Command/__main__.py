# -*- coding: utf-8 -*-
"""
Created on Tue Jan 30 03:24:05 2018

@author: twshe
"""
import os, sys
directory = os.path.split(__file__)[0]

def code_gen():
    os.system('{} {}'.format(os.path.join(directory, 'Trinity.TSL.CodeGen.exe'), ' '.join(sys.argv[2:])))


def meta_gen():
    os.system('{} {}'.format(os.path.join(directory, 'Trinity.TSL.Metagen.exe'),' '.join(sys.argv[2:])))

