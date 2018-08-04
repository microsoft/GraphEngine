# CMakeList.txt : Top-level CMake project file, do global configuration
# and include sub-projects here.
#
cmake_minimum_required (VERSION 3.0)

PROJECT ("GraphEngine")

ADD_DEFINITIONS(-D_CRT_SECURE_NO_WARNINGS -DUNICODE -D_UNICODE)
IF(NOT WIN32)
    INCLUDE_DIRECTORIES(/usr/include/rpc)
ENDIF()

# Include sub-projects.
ADD_SUBDIRECTORY ("trinity_host")
ADD_SUBDIRECTORY ("trinity_host_test")
