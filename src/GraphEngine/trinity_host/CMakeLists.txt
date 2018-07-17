# CMakeList.txt : CMake project for GraphEngine.Hosting, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.0.0)

INCLUDE_DIRECTORIES(../../Trinity.C/src)
INCLUDE_DIRECTORIES(../../Trinity.C/include)
FILE(GLOB_RECURSE SRCS *.cpp)
FILE(DOWNLOAD https://raw.githubusercontent.com/dotnet/coreclr/master/src/pal/prebuilt/inc/mscoree.h ${CMAKE_CURRENT_SOURCE_DIR}/mscoree.h)
ADD_DEFINITIONS(-DTRINITYC_EXPORTS)

IF(WIN32)
    ADD_DEFINITIONS(-D_CRT_SECURE_NO_WARNINGS -DUNICODE -D_UNICODE -D_WINDOWS)
ENDIF()

add_library (trinity_host SHARED ${SRCS})
