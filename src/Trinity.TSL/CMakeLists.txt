CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)
INCLUDE_DIRECTORIES(../Trinity.C/include/)
file(GLOB_RECURSE parser_srcs Trinity.TSL.Parser/*.cpp)
file(GLOB_RECURSE codegen_srcs Trinity.TSL.CodeGen/*.cpp)
# remove lexer generated for windows
list(REMOVE_ITEM parser_srcs "${CMAKE_SOURCE_DIR}/Trinity.TSL.Parser/lex.flex.cpp")
message("${CMAKE_SOURCE_DIR}")

MESSAGE(STATUS "Building Trinity.TSL.Parser")
SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fpermissive -Wall -std=c++14 -Wno-write-strings -Wno-unknown-pragmas -pthread")
ADD_LIBRARY(parser ${parser_srcs})

MESSAGE(STATUS "Building Trinity.TSL.CodeGen")
INCLUDE_DIRECTORIES(Trinity.TSL.CodeGen/CodeGen/ Trinity.TSL.Parser/)
LINK_LIBRARIES(parser)
ADD_EXECUTABLE(Trinity.TSL.CodeGen ${codegen_srcs})
