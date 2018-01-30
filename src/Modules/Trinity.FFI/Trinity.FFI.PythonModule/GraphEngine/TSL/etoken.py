
import re
token = re.compile('|'.join(['\>','\<','\,','[a-zA-Z_][a-z0-9A-Z_]*'])).findall
