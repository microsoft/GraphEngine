CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)

PROJECT(Trinity.TSL)

INCLUDE_DIRECTORIES(${PROJECT_SOURCE_DIR}/../Trinity.C/include/)

ADD_DEFINITIONS(-D_CRT_SECURE_NO_WARNINGS -DUNICODE -D_UNICODE)
ADD_SUBDIRECTORY(Trinity.TSL.Parser)
ADD_SUBDIRECTORY(Trinity.TSL.CodeGen)
ADD_DOTNET(Trinity.TSL.CodeTemplates/Trinity.TSL.CodeTemplates.csproj)
ADD_DOTNET(Trinity.TSL.Metagen/Trinity.TSL.Metagen.csproj)
RUN_DOTNET(Trinity.TSL.Metagen/Trinity.TSL.Metagen.csproj ARGUMENTS "${CMAKE_CURRENT_SOURCE_DIR}/Trinity.TSL.CodeTemplates")
ADD_DEPENDENCIES(Trinity.TSL.CodeGen RUN_Trinity.TSL.Metagen)
ADD_DEPENDENCIES(Trinity.TSL.CodeGen BUILD_Trinity.TSL.CodeTemplates BUILD_Trinity.TSL.Metagen)