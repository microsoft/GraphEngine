CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)
PROJECT(GraphEngine.Jit)

ADD_SUBDIRECTORY(GraphEngine.Jit.Native)
ADD_DOTNET(GraphEngine.Jit.sln 
    PACKAGE GraphEngine.Jit
    DEPENDS GraphEngine.Core)

ADD_DEPENDENCIES(BUILD_GraphEngine.Jit GraphEngine.Jit.Native)