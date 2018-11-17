IF(${TRINITY_VERSION})
    RETURN()
ENDIF()

EXECUTE_PROCESS(
    COMMAND ${DOTNET_EXE} help # to avoid "FIRST RUN EXPERIENCE" leaking into our version string
    OUTPUT_QUIET
    ERROR_QUIET)

EXECUTE_PROCESS(
    WORKING_DIRECTORY ${CMAKE_CURRENT_LIST_DIR}/../tools/versioning
    COMMAND ${DOTNET_EXE} run -c Release
    OUTPUT_VARIABLE TRINITY_VERSION
    OUTPUT_STRIP_TRAILING_WHITESPACE)

IF(TRINITY_VERSION STREQUAL "")
    SET(TRINITY_VERSION 0)
ENDIF()

SET(TRINITY_VERSION "2.0.${TRINITY_VERSION}")
MESSAGE("-- GraphEngine version is now ${TRINITY_VERSION}")

# Workaround: during package restore, Directory.Build.props leaks into the project files.
# And thus if we fix the version property in the root Directory.Build.props, all the dependant
# projects will refer to this (non-existing) version. This is bad. We have to overwrite the
# root props file every time.

SET(_DN_OUTPUT_PATH ${CMAKE_BINARY_DIR})
SET(_DN_XPLAT_LIB_DIR ${CMAKE_BINARY_DIR})
SET(_DN_VERSION ${TRINITY_VERSION})
CONFIGURE_FILE(${CMAKE_CURRENT_LIST_DIR}/DotnetImports.props.in ${CMAKE_CURRENT_LIST_DIR}/../Directory.Build.props)
UNSET(_DN_OUTPUT_PATH)
UNSET(_DN_XPLAT_LIB_DIR)
UNSET(_DN_VERSION)