CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)

PROJECT(Trinity.TSL)

INCLUDE_DIRECTORIES(${PROJECT_SOURCE_DIR}/../Trinity.C/include/)

ADD_DEFINITIONS(-D_CRT_SECURE_NO_WARNINGS -DUNICODE -D_UNICODE)
ADD_SUBDIRECTORY(Trinity.TSL.Parser)
ADD_SUBDIRECTORY(Trinity.TSL.CodeGen)
ADD_DOTNET(Trinity.TSL.CodeTemplates/Trinity.TSL.CodeTemplates.csproj   Trinity.TSL.CodeTemplates   Release     AnyCPU)
ADD_DOTNET(Trinity.TSL.Metagen/Trinity.TSL.Metagen.csproj               Trinity.TSL.Metagen         Release     AnyCPU)
ADD_DEPENDENCIES(Trinity.TSL.CodeGen Trinity.TSL.CodeTemplates Trinity.TSL.Metagen)
RUN_DOTNET(Trinity.TSL.Metagen/Trinity.TSL.Metagen.csproj Trinity.TSL.Metagen "../Trinity.TSL.CodeTemplates")