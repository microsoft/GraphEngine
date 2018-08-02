CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)

ADD_SUBDIRECTORY(Trinity.FFI.Native)
ADD_SUBDIRECTORY(Trinity.FFI.Python)

ADD_DOTNET(Trinity.FFI.sln 
    PACKAGE GraphEngine.FFI GraphEngine.FFI.MetaGen
    DEPENDS GraphEngine.Core)

ADD_DEPENDENCIES(BUILD_Trinity.FFI trinity_ffi)