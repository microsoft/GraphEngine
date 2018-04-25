from Redy.Magic.Classic import data
from typing import Callable
from .mangling import mangling_code



@data
class Verb:
    # TODO: to report jetbrains f-string error issues.
    LGet: lambda list_name: f'{list_name}{mangling_code}Get'
    LSet: lambda list_name: f'{list_name}{mangling_code}Set'
    LCount: lambda list_name: f'Count{mangling_code}{list_name}'
    LContains: lambda list_name: f'Contains{mangling_code}{list_name}'

    BGet: lambda typename: f"Get{mangling_code}{typename}"
    BSet: lambda typename: f"Set{mangling_code}{typename}"

    SGet: lambda typename: lambda member_name: f"{typename}{mangling_code}Get{mangling_code}{member_name}"
    SSet: lambda typename: lambda member_name: f"{typename}{mangling_code}Set{mangling_code}{member_name}"


LSet: Callable[[str], Verb] = Verb.LSet
LGet: Callable[[str], Verb] = Verb.LGet
LCount: Callable[[str], Verb] = Verb.LCount
LCotains: Callable[[str], Verb] = Verb.LContains

BGet: Callable[[str], Verb] = Verb.BGet
BSet: Callable[[str], Verb] = Verb.BSet

SGet: Callable[[str, str], Verb] = Verb.SGet
SSet: Callable[[str, str], Verb] = Verb.SSet
