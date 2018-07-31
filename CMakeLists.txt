CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)

SET(CMAKE_CXX_STANDARD 14)
SET(CMAKE_CXX_STANDARD_REQUIRED ON)
SET(CMAKE_CXX_EXTENSIONS OFF)

SET(TRINITY_VS_VERSION  "[15.0,16.0)")
SET(TRINITY_REQUIRES    "Microsoft.Component.MSBuild Microsoft.VisualStudio.Component.VC.Tools.x86.x64")
SET(TRINITY_REPO_ROOT   ${CMAKE_CURRENT_LIST_DIR})
SET(TRINITY_INSTALL_DIR ".")
SET(TRINITY_OUTPUT_DIR  ${TRINITY_REPO_ROOT}/bin)
SET(TRINITY_TEST_DIR    ${TRINITY_REPO_ROOT}/tests)
SET(TRINITY_TOOLS_DIR   ${TRINITY_REPO_ROOT}/tools)
SET(TRINITY_LIB_DIR     ${TRINITY_REPO_ROOT}/lib)
SET(TRINITY_SRC_DIR     ${TRINITY_REPO_ROOT}/src)
SET(TRINITY_C_DIR       ${TRINITY_SRC_DIR}/Trinity.C)
SET(TRINITY_NUGET_EXE   ${TRINITY_TOOLS_DIR}/NuGet.exe)

SET(RXCPP_DIR           ${TRINITY_SRC_DIR}/RxCpp)
SET(RX_INCLUDE_DIR      ${RXCPP_DIR}/RxCpp/Rx/v2/src/rxcpp)
SET(IX_INCLUDE_DIR      ${RXCPP_DIR}/Ix/CPP/src/cpplinq)

SET(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${TRINITY_OUTPUT_DIR})
SET(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${TRINITY_OUTPUT_DIR})
SET(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${TRINITY_OUTPUT_DIR})

SET(CMAKE_RUNTIME_OUTPUT_DIRECTORY_RELEASE ${TRINITY_OUTPUT_DIR})
SET(CMAKE_LIBRARY_OUTPUT_DIRECTORY_RELEASE ${TRINITY_OUTPUT_DIR})
SET(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_RELEASE ${TRINITY_OUTPUT_DIR})

SET(CMAKE_RUNTIME_OUTPUT_DIRECTORY_DEBUG ${TRINITY_OUTPUT_DIR})
SET(CMAKE_LIBRARY_OUTPUT_DIRECTORY_DEBUG ${TRINITY_OUTPUT_DIR})
SET(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_DEBUG ${TRINITY_OUTPUT_DIR})

IF(WIN32 AND NOT EXISTS ${TRINITY_NUGET_EXE})
    FILE(DOWNLOAD https://dist.nuget.org/win-x86-commandline/latest/nuget.exe ${TRINITY_NUGET_EXE})
ENDIF()

IF(WIN32)
    SET(CMAKE_CXX_FLAGS           "/volatile:iso /EHa /fp:except- /Zi /Gy /MP /W3 /DEBUG")
    SET(CMAKE_CXX_FLAGS_RELEASE   "/Oi /Ot /Ob2 /Oy /O2 /MT /GS- /GL")
    SET(CMAKE_CXX_FLAGS_DEBUG     "/Od /MTd /GS")
    # SET(CMAKE_SHARED_LINKER_FLAGS "/LTCG /OPT:REF /OPT:ICF /SUBSYSTEM:WINDOWS /INCREMENTAL:NO")
    SET(CMAKE_SHARED_LINKER_FLAGS "/OPT:REF /OPT:ICF /SUBSYSTEM:WINDOWS /INCREMENTAL")
ELSEIF(UNIX)
    ADD_COMPILE_OPTIONS(-fpermissive -Wall -Wno-write-strings -Wno-unknown-pragmas -pthread)
ELSE()
    MESSAGE(FATAL_ERROR "Unsupported platform")
ENDIF()

MACRO(POSTBUILD_COPY_OUTPUT target file)
    ADD_CUSTOM_COMMAND(
        TARGET ${target}
        POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E make_directory "${TRINITY_OUTPUT_DIR}"
        COMMAND ${CMAKE_COMMAND} -E copy ${file} "${TRINITY_OUTPUT_DIR}/")
ENDMACRO()

MACRO(ADD_DOTNET proj pkgname config arch)
    FILE(GLOB_RECURSE dotnet_cs "*.cs")
    FILE(GLOB_RECURSE dotnet_fs "*.fs")
    FILE(GLOB_RECURSE dotnet_xaml "*.xaml")
    FILE(GLOB_RECURSE dotnet_csproj "*.csproj")
    FILE(GLOB_RECURSE dotnet_fsproj "*.fsproj")
    ADD_CUSTOM_TARGET(
        ${pkgname}
        DEPENDS ${dotnet_cs} ${dotnet_fs} ${dotnet_xaml} ${dotnet_csproj} ${dotnet_fsproj}
        COMMAND "dotnet pack ${proj} -c ${config} /p:Platform=${arch}"
    )
ENDMACRO()

MACRO(ADD_MSBUILD proj pkgname config arch)
    FILE(GLOB_RECURSE dotnet_cs "*.cs")
    FILE(GLOB_RECURSE dotnet_fs "*.fs")
    FILE(GLOB_RECURSE dotnet_xaml "*.xaml")
    FILE(GLOB_RECURSE dotnet_csproj "*.csproj")
    FILE(GLOB_RECURSE dotnet_fsproj "*.fsproj")
    ADD_CUSTOM_TARGET(
        ${pkgname}
        # OUTPUT "${TRINITY_OUTPUT_DIR}/${pkgname}.nupkg"
        DEPENDS ${dotnet_cs} ${dotnet_fs} ${dotnet_xaml} ${dotnet_csproj} ${dotnet_fsproj}
        COMMAND "dotnet msbuild ${proj} /p:Configuration=${config};Platform=${arch}"
    )
ENDMACRO()

PROJECT(GraphEngine VERSION 2.0.9328 LANGUAGES CXX)

#  Native components

ADD_SUBDIRECTORY(src/Trinity.TSL)
ADD_SUBDIRECTORY(src/Trinity.C)
ADD_SUBDIRECTORY(src/GraphEngine)
ADD_SUBDIRECTORY(src/Modules/GraphEngine.Jit)
ADD_SUBDIRECTORY(src/Modules/Trinity.FFI/Trinity.FFI.Native)
ADD_SUBDIRECTORY(src/Modules/Trinity.FFI/Trinity.FFI.Python)

#  DotNet components

#ADD_SUBDIRECTORY(src/Trinity.Core)
