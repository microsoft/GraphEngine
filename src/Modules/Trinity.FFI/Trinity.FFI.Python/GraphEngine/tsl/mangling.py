mangling_code = '_'


def mangling(name: str):
    return name.replace(mangling_code, mangling_code * 2)
