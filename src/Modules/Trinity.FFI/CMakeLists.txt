CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)

ADD_SUBDIRECTORY(Trinity.FFI.Native)
ADD_SUBDIRECTORY(Trinity.FFI.Python)

ADD_DOTNET(Trinity.FFI/Trinity.FFI.csproj
    PACKAGE GraphEngine.FFI
    DEPENDS GraphEngine.Core GraphEngine.Jit)

#TODO unittest

ADD_DEPENDENCIES(BUILD_Trinity.FFI Trinity trinity_ffi trinity_host)
