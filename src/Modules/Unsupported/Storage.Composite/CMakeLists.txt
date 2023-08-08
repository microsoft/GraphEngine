CMAKE_MINIMUM_REQUIRED(VERSION 3.16.0)
PROJECT(GraphEngine.Storage.Composite)

ADD_DOTNET(Trinity.Storage.Composite/Trinity.Storage.Composite.csproj
    PACKAGE GraphEngine.Storage.Composite
    VERSION ${TRINITY_VERSION}
    DEPENDS GraphEngine.Core)

# TODO unit test
