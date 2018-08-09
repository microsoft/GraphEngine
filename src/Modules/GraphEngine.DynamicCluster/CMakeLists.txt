CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)
PROJECT(GraphEngine.Storage.Composite)

ADD_DOTNET(
    Trinity.DynamicCluster/Trinity.DynamicCluster.csproj 
    PACKAGE GraphEngine.DynamicCluster
    DEPENDS GraphEngine.Core GraphEngine.Client
)

ADD_DOTNET(
    Trinity.Azure.Storage/Trinity.Azure.Storage.csproj 
    PACKAGE GraphEngine.Azure.BlobStorage
    DEPENDS GraphEngine.DynamicCluster
)

# TODO unit test
