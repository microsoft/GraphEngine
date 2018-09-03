CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)

LIST(APPEND CMAKE_MODULE_PATH "${CMAKE_CURRENT_LIST_DIR}/cmake")
FIND_PACKAGE(dotnet 2.0 REQUIRED)
INCLUDE(PostBuild)

PROJECT(GraphEngine VERSION 2.0.9328 LANGUAGES CXX)
SET(TRINITY_VERSION "${PROJECT_VERSION_MAJOR}.${PROJECT_VERSION_MINOR}.${PROJECT_VERSION_PATCH}")
SET(DOTNET_PACKAGE_VERSION ${TRINITY_VERSION})

SET(CMAKE_CXX_STANDARD 14)
SET(CMAKE_CXX_STANDARD_REQUIRED ON)
SET(CMAKE_CXX_EXTENSIONS OFF)

SET(TRINITY_REPO_ROOT   ${CMAKE_CURRENT_LIST_DIR})
SET(TRINITY_OUTPUT_DIR  ${TRINITY_REPO_ROOT}/bin)
SET(TRINITY_TEST_DIR    ${TRINITY_REPO_ROOT}/tests)
SET(TRINITY_TOOLS_DIR   ${TRINITY_REPO_ROOT}/tools)
SET(TRINITY_SRC_DIR     ${TRINITY_REPO_ROOT}/src)
SET(TRINITY_NUGET_EXE   ${TRINITY_TOOLS_DIR}/NuGet.exe)

SET(XPLAT_LIB_DIR ${TRINITY_REPO_ROOT}/lib)
SET_OUTPUT_DIRECTORY(${TRINITY_OUTPUT_DIR})

IF(WIN32 AND NOT EXISTS ${TRINITY_NUGET_EXE})
    FILE(DOWNLOAD https://dist.nuget.org/win-x86-commandline/latest/nuget.exe ${TRINITY_NUGET_EXE})
ENDIF()

IF(WIN32)
    SET(CMAKE_CXX_FLAGS           "/volatile:iso /EHa /fp:except- /Zi /Gy /MP /W3 /nologo")
    SET(CMAKE_CXX_FLAGS_RELEASE   "/Oi /Ot /Ob2 /Oy /O2 /MT /GS- /GL")
    SET(CMAKE_CXX_FLAGS_DEBUG     "/Od /MTd /GS")
    SET(CMAKE_SHARED_LINKER_FLAGS "/LTCG /OPT:REF /OPT:ICF /SUBSYSTEM:WINDOWS /INCREMENTAL /DEBUG:FULL")
    SET(CMAKE_EXE_LINKER_FLAGS "/LTCG /OPT:REF /OPT:ICF /INCREMENTAL /DEBUG:FULL")
ELSEIF(UNIX)
    ADD_COMPILE_OPTIONS(-fpermissive -Wall -Wno-write-strings -Wno-unknown-pragmas -pthread)
ELSE()
    MESSAGE(FATAL_ERROR "Unsupported platform")
ENDIF()

ADD_SUBDIRECTORY(src/GraphEngine)
ADD_SUBDIRECTORY(src/Trinity.TSL)
ADD_SUBDIRECTORY(src/Trinity.C)
ADD_SUBDIRECTORY(src/Trinity.Core)
ADD_SUBDIRECTORY(src/Modules/GraphEngine.Jit)
ADD_SUBDIRECTORY(src/Modules/Trinity.FFI)
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
)

# ADD_DOTNET(
#     src/GraphEngine.DataImporter/GraphEngine.DataImporter.sln
#     PACKAGE GraphEngine.DataImporter
#     DEPENDS GraphEngine.Core)

