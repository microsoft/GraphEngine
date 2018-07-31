CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)

IF(WIN32)
    ADD_MSBUILD(Trinity.TSL.CodeTemplates.csproj Trinity.TSL.CodeTemplates Release x64)
ENDIF()