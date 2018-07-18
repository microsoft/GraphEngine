from Redy.Magic.Classic import singleton, template, cast
from Redy.Tools.Version import Version as _Version

from typing import Tuple, Callable
from .env import Env
import operator


@singleton
class Version:

    def __init__(self, fns=()):
        self._fns: Tuple[Callable[[_Version], bool], ...] = fns

    def __call__(self, version):
        if not self._fns:
            return True
        elif not isinstance(version, _Version):
            version = _Version(version)
        for each in self._fns:
            if not each(version):
                return False
        return True

    @template
    def compare(self, version, op):
        if not isinstance(version, _Version):
            version = _Version(version)
        return self.__class__(self._fns + (lambda x: op(x, version),))

    @compare(op=operator.lt)
    def __lt__(self, other):
        ...

    @compare(op=operator.eq)
    def __eq__(self, other):
        ...

    @compare(op=operator.gt)
    def __gt__(self, other):
        ...

    @compare(op=operator.le)
    def __le__(self, other):
        ...

    @compare(op=operator.ge)
    def __ge__(self, other):
        ...


class Dependency:

    def __init__(self, package_name, version, target_framework_check=None):
        self.package_name = package_name
        self.version = version
        self.target_framework_check = target_framework_check if target_framework_check else Env.target_framework

    def all(self):

        for each in Env.nuget_root.into("{}/{}/lib".format(self.package_name, self.version)).list_dir():
            if each.is_dir() and self.target_framework_check(each[-1]):
                return each.list_dir()

