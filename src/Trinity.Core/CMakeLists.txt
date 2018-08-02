CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)

ADD_DOTNET(
    Trinity.Core.csproj 
    PACKAGE GraphEngine.Core)

ADD_DEPENDENCIES(BUILD_Trinity.Core Trinity)
IF(WIN32)
    ADD_DEPENDENCIES(BUILD_Trinity.Core Trinity.C)
ENDIF()
ADD_DEPENDENCIES(BUILD_Trinity.Core Trinity.TSL.CodeGen)