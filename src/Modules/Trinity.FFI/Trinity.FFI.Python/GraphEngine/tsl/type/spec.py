from GraphEngine.utils import ImmutableDict
from Redy.Magic.Classic import record


class TypeSpec:
    pass


@record
class PrimitiveTypeSpec(TypeSpec):
    name: str
    type_code: str

    def __repr__(self):
        return self.name


@record
class StructTypeSpec(TypeSpec):
    name: str
    field_types: ImmutableDict[str, TypeSpec]

    def __repr__(self):
        return self.name

    def __str__(self):
        return 'struct {}\n{{\n{}\n}}'.format(
            self.name, '\n'.join('    {!r} {};'.format(v, k)
                                 for k, v in self.field_types.items()))


class CellTypeSpec(StructTypeSpec):
    def __str__(self):
        return 'cell {}\n{{\n{}\n}}'.format(
            self.name, '\n'.join('    {!r} {};'.format(v, k)
                                 for k, v in self.field_types.items()))


@record
class ListTypeSpec(TypeSpec):
    elem_type: TypeSpec

    def __repr__(self):
        return 'List<{!r}>'.format(self.elem_type)
