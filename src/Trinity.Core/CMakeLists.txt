CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)

ADD_DOTNET(
    Trinity.Core.csproj 
    PACKAGE GraphEngine.Core
    VERSION ${TRINITY_VERSION})

ADD_DEPENDENCIES(BUILD_Trinity.Core Trinity)
ADD_DEPENDENCIES(BUILD_Trinity.Core Trinity.TSL.CodeGen)

FILE(GLOB_RECURSE smoke_tests ../Trinity.Core.SmokeTest/*.csproj)
FILE(GLOB_RECURSE unit_tests ../Trinity.Core.UnitTest/*.csproj)

FOREACH(smoke_test ${smoke_tests})
    SMOKETEST_DOTNET(${smoke_test} DEPENDS GraphEngine.Core VERSION ${TRINITY_VERSION})
ENDFOREACH()

FOREACH(unit_test ${unit_tests})
    XUNIT_DOTNET(${unit_test} DEPENDS GraphEngine.Core ARGUMENTS -parallel none)
ENDFOREACH()
