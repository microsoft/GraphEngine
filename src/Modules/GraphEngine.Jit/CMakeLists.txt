CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)
PROJECT(GraphEngine.Jit)

ADD_SUBDIRECTORY(GraphEngine.Jit.Native)
ADD_DOTNET(GraphEngine.Jit/GraphEngine.Jit.fsproj
    PACKAGE GraphEngine.Jit
    DEPENDS GraphEngine.Core)

# TODO xplat native components

# TODO unit test

ADD_DEPENDENCIES(BUILD_GraphEngine.Jit GraphEngine.Jit.Native)
