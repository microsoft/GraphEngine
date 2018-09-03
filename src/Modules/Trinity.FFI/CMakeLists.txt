CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)

ADD_SUBDIRECTORY(Trinity.FFI.Native)
ADD_SUBDIRECTORY(Trinity.FFI.Python)

ADD_DOTNET(Trinity.FFI.Metagen/Trinity.FFI.Metagen.fsproj
    PACKAGE GraphEngine.FFI.Metagen
    VERSION ${TRINITY_VERSION}
    DEPENDS GraphEngine.Core GraphEngine.Jit)

ADD_DOTNET(Trinity.FFI/Trinity.FFI.csproj
    PACKAGE GraphEngine.FFI
    VERSION ${TRINITY_VERSION}
    DEPENDS GraphEngine.Core GraphEngine.Jit GraphEngine.FFI.Metagen)

#TODO unittest

ADD_DEPENDENCIES(BUILD_Trinity.FFI Trinity trinity_ffi trinity_host)
