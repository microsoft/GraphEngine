CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)

LIST(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_LIST_DIR}/cmake")
FIND_PACKAGE(Dotnet 2.0 REQUIRED)
INCLUDE(PostBuild)

PROJECT(GraphEngine VERSION 2.0.9328 LANGUAGES CXX)
SET(TRINITY_VERSION "${PROJECT_VERSION_MAJOR}.${PROJECT_VERSION_MINOR}.${PROJECT_VERSION_PATCH}")

SET(CMAKE_CXX_STANDARD 14)
SET(CMAKE_CXX_STANDARD_REQUIRED ON)
SET(CMAKE_CXX_EXTENSIONS OFF)

SET(TRINITY_REPO_ROOT   ${CMAKE_CURRENT_LIST_DIR})
SET(TRINITY_OUTPUT_DIR  ${TRINITY_REPO_ROOT}/bin)
SET(TRINITY_TEST_DIR    ${TRINITY_REPO_ROOT}/tests)
SET(TRINITY_SRC_DIR     ${TRINITY_REPO_ROOT}/src)

SET(XPLAT_LIB_DIR ${TRINITY_REPO_ROOT}/lib)
SET_OUTPUT_DIRECTORY(${CMAKE_BINARY_DIR})
DOTNET_REGISTER_LOCAL_REPOSITORY("Graph Engine OSS Local" "${CMAKE_BINARY_DIR}")

IF(WIN32)
    ADD_COMPILE_OPTIONS(/volatile:iso /EHa /fp:except- /Zi /Gy /W3 /nologo /MP)
    SET(CMAKE_SHARED_LINKER_FLAGS "${CMAKE_SHARED_LINKER_FLAGS} /DEBUG:FULL")
    SET(CMAKE_EXE_LINKER_FLAGS "${CMAKE_EXE_LINKER_FLAGS} /DEBUG:FULL")
ELSEIF(UNIX)
    ADD_COMPILE_OPTIONS(-fpermissive -Wall -Wno-write-strings -Wno-unknown-pragmas -pthread)
ELSE()
    MESSAGE(FATAL_ERROR "Unsupported platform")
ENDIF()

ADD_SUBDIRECTORY(src/Trinity.TSL)
ADD_SUBDIRECTORY(src/Trinity.C)
ADD_SUBDIRECTORY(src/Trinity.Core)
ADD_SUBDIRECTORY(src/Modules/LIKQ)
ADD_SUBDIRECTORY(src/Modules/GraphEngine.Storage.Composite)
ADD_SUBDIRECTORY(src/Modules/GraphEngine.Client)
ADD_SUBDIRECTORY(src/Modules/GraphEngine.DynamicCluster)

ADD_MSBUILD(
    src/Modules/GraphEngine.ServiceFabric/GraphEngine.ServiceFabric.sln 
    PACKAGE 
        GraphEngine.ServiceFabric.Infrastructure
        GraphEngine.ServiceFabric.ListenerRuntime
        GraphEngine.ServiceFabric.Remoting
    DEPENDS
        GraphEngine.Core
        GraphEngine.Client
        GraphEngine.Storage.Composite
        GraphEngine.DynamicCluster
        GraphEngine.Azure.BlobStorage
    X64
    CONFIG RelSkipSFApp
    VERSION ${TRINITY_VERSION}
)

# ADD_DOTNET(
#     src/GraphEngine.DataImporter/GraphEngine.DataImporter.sln
#     PACKAGE GraphEngine.DataImporter
#     DEPENDS GraphEngine.Core)

