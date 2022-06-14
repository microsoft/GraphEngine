IF(NOT WIN32)
    MESSAGE("Skip Trinity.VSExtension on non-Win32 platform")
    RETURN()
ENDIF()

FIND_PROGRAM(VSWHERE_EXE vswhere PATHS ${CMAKE_BINARY_DIR}/tools)
IF(VSWHERE_EXE)
    MESSAGE("-- Found vswhere: ${VSWHERE_EXE}")
ELSE()
    SET(VSWHERE_EXE ${CMAKE_BINARY_DIR}/tools/vswhere.exe)
    MESSAGE("-- Downloading vswhere...")
	FILE(DOWNLOAD https://github.com/Microsoft/vswhere/releases/download/2.6.7/vswhere.exe ${VSWHERE_EXE})
    MESSAGE("vswhere.exe downloaded and saved to ${VSWHERE_EXE}")
ENDIF()

EXECUTE_PROCESS(
    COMMAND ${VSWHERE_EXE} -latest -products * -requires Microsoft.VisualStudio.Component.VC.Tools.x86.x64 -property installationPath
    OUTPUT_VARIABLE VS_PATH
    OUTPUT_STRIP_TRAILING_WHITESPACE
)

FIND_PROGRAM(MSBUILD_EXE msbuild PATHS "${VS_PATH}/MSBuild/15.0/Bin")

IF(NOT MSBUILD_EXE)
	FIND_PROGRAM(MSBUILD_EXE msbuild PATHS "${VS_PATH}/MSBuild/Current/Bin")
ENDIF()

IF(NOT MSBUILD_EXE)
	MESSAGE(FATAL_ERROR "msbuild not found.")
ENDIF()

MESSAGE("-- Visual Studio path: ${VS_PATH}")
MESSAGE("-- MSBuild: ${MSBUILD_EXE}")

SET(vsext_target BUILD_Trinity.VSExtension)
SET(vsext_output ${CMAKE_BINARY_DIR}/GraphEngineVSExtension.vsix)
SET(tsl_codegen_path ${CMAKE_CURRENT_LIST_DIR}/../Trinity.TSL/Trinity.TSL.CodeGen)
FILE(GLOB_RECURSE vsext_src *.cs *.fs *.xaml *.csproj *.fsproj *.tsl ${tsl_codegen_path}/*.h ${tsl_codegen_path}/*.cpp)

SET(_DN_OUTPUT_PATH ${CMAKE_BINARY_DIR})
SET(_DN_VERSION ${TRINITY_VERSION})
SET(_DN_IMPORT_PROP ${CMAKE_CURRENT_BINARY_DIR}/${_DN_projname}.imports.props)
CONFIGURE_FILE(${DOTNET_MODULE_DIR}/DotnetImports.props.in ${_DN_IMPORT_PROP})

IF (MSVC_TOOLSET_VERSION EQUAL 142)
  SET(vsext_sln ${CMAKE_CURRENT_LIST_DIR}/Trinity.VSExtension.VS2019.sln)
ELSEIF(MSVC_TOOLSET_VERSION EQUAL 141)
  SET(vsext_sln ${CMAKE_CURRENT_LIST_DIR}/Trinity.VSExtension.VS2017.sln)
ENDIF()
    
ADD_CUSTOM_COMMAND(
    OUTPUT ${vsext_output}
    COMMAND ${NUGET_EXE} restore ${vsext_sln}
    COMMAND ${MSBUILD_EXE} ${vsext_sln} /t:Clean /p:Configuration=Release
    COMMAND ${MSBUILD_EXE} ${vsext_sln} /t:Build "/p:DirectoryBuildPropsPath=${_DN_IMPORT_PROP}" /p:Configuration=Release /p:Platform=x86
    DEPENDS ${vsext_src}
)

ADD_CUSTOM_TARGET(
    ${vsext_target} ALL
    DEPENDS ${vsext_output})

ADD_DEPENDENCIES(${vsext_target} 
    PKG_GraphEngine.Core
    PKG_GraphEngine.Client
    PKG_GraphEngine.Storage.Composite
    PKG_GraphEngine.DynamicCluster
    PKG_GraphEngine.LIKQ
    PKG_GraphEngine.Azure.BlobStorage)

