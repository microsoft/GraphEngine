CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)
PROJECT(GraphEngine.Client)

ADD_DOTNET(Trinity.Client/Trinity.Client.csproj
    PACKAGE GraphEngine.Client
    DEPENDS GraphEngine.Core)

# TODO unit test
