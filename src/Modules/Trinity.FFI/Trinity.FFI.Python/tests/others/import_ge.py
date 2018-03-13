import sys, os

__mypath = os.path.abspath(os.path.dirname(__file__))

os.chdir(os.path.join(__mypath, '..'))
os.system('python ./setup.py build')
os.chdir(__mypath)
sys.path.append('../build/lib.win-amd64-3.6')

import GraphEngine as ge

