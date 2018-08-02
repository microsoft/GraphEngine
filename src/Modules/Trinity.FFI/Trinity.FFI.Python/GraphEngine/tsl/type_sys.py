from Redy.Magic.Classic import cast, execute
import Redy.Opt.bytecode_api
import ast


def code(node):
    return compile(
        ast.fix_missing_locations(
            ast.Module(node if isinstance(node, list) else [node])), __file__,
        "exec")


@exec
@execute
@cast(code)
def def_cell():

    return ast.ClassDef(
        name='Cell',
        bases=[],
        keywords=[],
        body=[
            ast.Assign(
                targets=[ast.Name(id='cell.id', ctx=ast.Store())],
                value=ast.NameConstant(value=None))
        ],
        decorator_list=[])


print(Cell.__dict__.keys())
