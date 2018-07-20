# CMakeList.txt : Top-level CMake project file, do global configuration
# and include sub-projects here.
#
cmake_minimum_required (VERSION 3.0)

PROJECT ("GraphEngine")

ADD_DEFINITIONS(-D_CRT_SECURE_NO_WARNINGS -DUNICODE -D_UNICODE)

# Include sub-projects.
add_subdirectory ("trinity_host")
add_subdirectory ("trinity_host_test")
