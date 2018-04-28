from .env import Env


class Library:
    def __init__(self, package_name, version, where):
        self.package_name = package_name
        self.version = version
        self.where = where

    def all(self):
        return Env.nuget_root.into("{}/{}/{}".format(self.package_name, self.version, self.where)).list_dir()
