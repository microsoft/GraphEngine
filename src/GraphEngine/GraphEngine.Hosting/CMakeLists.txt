# CMakeList.txt : CMake project for GraphEngine.Hosting, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.0.0)

INCLUDE_DIRECTORIES(../../Trinity.C/src/)
INCLUDE_DIRECTORIES(../../Trinity.C/include/)
FILE(GLOB_RECURSE SRCS *.cpp)
FILE(DOWNLOAD https://raw.githubusercontent.com/dotnet/coreclr/master/src/pal/prebuilt/inc/mscoree.h ${CMAKE_CURRENT_SOURCE_DIR}/mscoree.h)

add_library (TrinityHosting SHARED ${SRCS})
