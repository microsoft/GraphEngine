CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)

ADD_SUBDIRECTORY(Trinity.FFI.Native)
ADD_SUBDIRECTORY(Trinity.FFI.Python)

ADD_DOTNET(Trinity.FFI.sln 
    PACKAGE GraphEngine.FFI GraphEngine.FFI.MetaGen
    DEPENDS GraphEngine.Core GraphEngine.Jit)

ADD_DEPENDENCIES(BUILD_Trinity.FFI Trinity trinity_ffi trinity_host)