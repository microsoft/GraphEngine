CMAKE_MINIMUM_REQUIRED(VERSION 3.16.0)

LIST(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_LIST_DIR}/cmake")
FIND_PACKAGE(Dotnet 2.0 REQUIRED)
INCLUDE(PostBuild)
INCLUDE(TrinityVersion)

GEN_DOTNET_PROPS(${CMAKE_CURRENT_LIST_DIR}/Directory.Build.props PACKAGE_VERSION ${TRINITY_VERSION})

PROJECT(GraphEngine VERSION ${TRINITY_VERSION} LANGUAGES CXX)
ENABLE_TESTING()

SET(CMAKE_CXX_STANDARD 14)
SET(CMAKE_CXX_STANDARD_REQUIRED ON)
SET(CMAKE_CXX_EXTENSIONS OFF)

SET(XPLAT_LIB_DIR ${CMAKE_CURRENT_LIST_DIR}/lib)
SET_OUTPUT_DIRECTORY(${CMAKE_BINARY_DIR})
DOTNET_REGISTER_LOCAL_REPOSITORY("Graph Engine OSS Local" "${CMAKE_BINARY_DIR}")

IF(WIN32)
    ADD_COMPILE_OPTIONS(/volatile:iso /EHsc /fp:except- /Zi /Gy /W3 /nologo /MP /utf-8 /wd4267)
    SET(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} /DEBUG:FULL")
    SET(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} /DEBUG:FULL")
    ADD_DEFINITIONS(-D_CRT_SECURE_NO_WARNINGS)
ELSEIF(UNIX)
    ADD_COMPILE_OPTIONS(-fpermissive -Wno-write-strings -Wno-unknown-pragmas -Wno-attributes -Wno-unused-result -pthread)
ELSE()
    MESSAGE(FATAL_ERROR "Unsupported platform")
ENDIF()

# Graph Engine Core
ADD_SUBDIRECTORY(src/Trinity.C)
ADD_SUBDIRECTORY(src/Trinity.C.UnitTest)
ADD_SUBDIRECTORY(src/Trinity.TSL)
ADD_SUBDIRECTORY(src/Trinity.Core)

# LIKQ: Language Integrated Knowledge Query
ADD_SUBDIRECTORY(src/Modules/LIKQ)

ADD_DOTNET(
    src/Modules/DataImporter/GraphEngine.DataImporter/GraphEngine.DataImporter.csproj
    VERSION ${TRINITY_VERSION}
    PACKAGE GraphEngine.DataImporter
    DEPENDS GraphEngine.Core)
TEST_DOTNET(
    src/Modules/DataImporter/GraphEngine.DataImporter.UnitTest/GraphEngine.DataImporter.UnitTest.csproj
    DEPENDS GraphEngine.DataImporter
    ARGUMENTS -- RunConfiguration.TargetPlatform=x64 RunConfiguration.MaxCpuCount=1)
# The following Modules are Unsupported, provided as-is.

# ADD_SUBDIRECTORY(src/Modules/Unsupported/Storage.Composite)
# ADD_SUBDIRECTORY(src/Modules/Unsupported/Client)
# ADD_SUBDIRECTORY(src/Modules/Unsupported/DynamicCluster)
# ADD_SUBDIRECTORY(src/Modules/Unsupported/VSExtension)

# ADD_MSBUILD(
#     src/Modules/Unsupported/ServiceFabric/GraphEngine.ServiceFabric.sln
#     PACKAGE
#         GraphEngine.ServiceFabric.Infrastructure
#         GraphEngine.ServiceFabric.Remoting
#     DEPENDS
#         GraphEngine.Core
#         GraphEngine.Client
#         GraphEngine.Storage.Composite
#         GraphEngine.DynamicCluster
#         GraphEngine.Azure.BlobStorage
#     X64
#     CONFIG RelSkipSFApp
#     VERSION ${TRINITY_VERSION}
# )
