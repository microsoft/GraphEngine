
from Ruikowa.ObjectRegex.Node import Ref, AstParser, SeqParser, LiteralParser, CharParser, MetaInfo, DependentAstParser
try:
    from .etoken import token
except:
    from etoken import token
import re
namespace     = globals()
recurSearcher = set()
Generic = AstParser([Ref('Identifier'),LiteralParser('<', name='\'<\''),SeqParser([Ref('Type'),SeqParser([LiteralParser(',', name='\',\''),Ref('Type')])], atmost = 1),LiteralParser('>', name='\'>\'')], name = 'Generic', toIgnore = [{},{',','<','>'}])
Identifier = LiteralParser('[a-zA-Z_][a-z0-9A-Z_]*', name = 'Identifier', isRegex = True)
Type = AstParser([LiteralParser('optional', name='\'optional\''),Ref('Type')],[Ref('Generic')],[Ref('Identifier')], name = 'Type')
Generic.compile(namespace, recurSearcher)
Type.compile(namespace, recurSearcher)
