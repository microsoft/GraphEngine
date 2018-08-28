cmake_minimum_required (VERSION 3.0.0)

FILE(GLOB_RECURSE SRCS *.cpp)

ADD_EXECUTABLE (trinity_host_test ${SRCS})
TARGET_LINK_LIBRARIES(trinity_host_test trinity_host)