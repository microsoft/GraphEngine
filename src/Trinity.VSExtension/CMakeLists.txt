ADD_MSBUILD(
    Trinity.VSExtension.sln
    DEPENDS
        GraphEngine.Core
        GraphEngine.Client
        GraphEngine.Storage.Composite
        GraphEngine.DynamicCluster
		GraphEngine.LIKQ
        GraphEngine.Azure.BlobStorage
    X86
    VERSION ${TRINITY_VERSION}
)