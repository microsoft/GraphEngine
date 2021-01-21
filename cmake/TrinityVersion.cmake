IF(NOT TRINITY_VERSION)
    EXECUTE_PROCESS(COMMAND ${DOTNET_EXE} build ${CMAKE_CURRENT_LIST_DIR}/../tools/versioning/versioning.csproj -c Release --framework netcoreapp3.1 OUTPUT_QUIET)

    EXECUTE_PROCESS(COMMAND ${DOTNET_EXE} run --no-build --project ${CMAKE_CURRENT_LIST_DIR}/../tools/versioning/versioning.csproj -c Release -framework netcoreapp3.1 OUTPUT_VARIABLE TRINITY_VERSION OUTPUT_STRIP_TRAILING_WHITESPACE)

    IF(TRINITY_VERSION STREQUAL "")
        SET(TRINITY_VERSION 0)
    ENDIF()

    SET(TRINITY_VERSION "2.0.${TRINITY_VERSION}")
    MESSAGE("-- IKW GraphEngine version is now ${TRINITY_VERSION}")
ENDIF()
