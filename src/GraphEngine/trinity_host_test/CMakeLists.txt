# CMakeList.txt : CMake project for GraphEngine.Hosting, include source and define
# project specific logic here.
#
cmake_minimum_required (VERSION 3.0.0)

INCLUDE_DIRECTORIES(${TRINITY_C_DIR}/include)
INCLUDE_DIRECTORIES(${TRINITY_C_DIR}/src)
FILE(GLOB_RECURSE SRCS *.cpp)

ADD_EXECUTABLE (trinity_host_test ${SRCS})
TARGET_LINK_LIBRARIES(trinity_host_test trinity_host)