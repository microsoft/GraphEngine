CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)
PROJECT(GraphEngine.LIKQ)

ADD_DOTNET(FanoutSearch/FanoutSearch.csproj
    PACKAGE GraphEngine.LIKQ
    DEPENDS GraphEngine.Core)

# TODO unit test
