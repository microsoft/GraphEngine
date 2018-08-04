# CMakeList.txt : CMake project for GraphEngine.Hosting, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.0.0)

INCLUDE_DIRECTORIES(${TRINITY_C_DIR}/src)
INCLUDE_DIRECTORIES(${TRINITY_C_DIR}/include)
INCLUDE_DIRECTORIES(${IX_INCLUDE_DIR})
FILE(GLOB_RECURSE SRCS *.cpp)
# FILE(DOWNLOAD https://raw.githubusercontent.com/dotnet/coreclr/master/src/pal/prebuilt/inc/mscoree.h ${CMAKE_CURRENT_SOURCE_DIR}/mscoree.h)
# FILE(DOWNLOAD https://raw.githubusercontent.com/dotnet/coreclr/master/src/coreclr/host/inc/coreclrhost.h ${CMAKE_CURRENT_SOURCE_DIR}/coreclrhost.h)

ADD_LIBRARY(trinity_host SHARED ${SRCS})
ADD_DEFINITIONS(-DTRINITYC_EXPORT)
TARGET_INCLUDE_DIRECTORIES(trinity_host PUBLIC ${CMAKE_CURRENT_SOURCE_DIR})
