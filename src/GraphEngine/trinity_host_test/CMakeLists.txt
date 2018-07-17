# CMakeList.txt : CMake project for GraphEngine.Hosting, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.0.0)

INCLUDE_DIRECTORIES(../../Trinity.C/src)
INCLUDE_DIRECTORIES(../../Trinity.C/include)
INCLUDE_DIRECTORIES(../trinity_host)
FILE(GLOB_RECURSE SRCS *.cpp)

IF(WIN32)
    ADD_DEFINITIONS(-D_CRT_SECURE_NO_WARNINGS -DUNICODE -D_UNICODE -D_WINDOWS)
ENDIF()

TARGET_LINK_LIBRARIES("trinity_host")

ADD_EXECUTABLE (trinity_host_test ${SRCS})
