CMAKE_MINIMUM_REQUIRED(VERSION 2.8.0)
MESSAGE(STATUS "Cmake version ${CMAKE_VERSION}")

IF(WIN32) 
    MESSAGE(STATUS "Build on Windows")
ENDIF()

INCLUDE_DIRECTORIES(../../../src)
INCLUDE_DIRECTORIES(../../../include)

IF(UNIX) 
    MESSAGE(STATUS "Build on Unix/Linux") 
    # SET (CMAKE_C_COMPILER             "/usr/bin/clang")
    # SET (CMAKE_C_FLAGS                "-Wall -std=c99")
    # SET (CMAKE_C_FLAGS_DEBUG          "-g")
    # SET (CMAKE_C_FLAGS_MINSIZEREL     "-Os -DNDEBUG")
    # SET (CMAKE_C_FLAGS_RELEASE        "-O4 -DNDEBUG")
    # SET (CMAKE_C_FLAGS_RELWITHDEBINFO "-O2 -g")

    # SET (CMAKE_CXX_COMPILER "/usr/bin/clang++")
    # SET (CMAKE_CXX_FLAGS_DEBUG "-g")
    # SET (CMAKE_CXX_FLAGS_MINSIZEREL "-Os -DNDEBUG")
    # SET (CMAKE_CXX_FLAGS_RELEASE "-O4 -DNDEBUG")
    # SET (CMAKE_CXX_FLAGS_RELWITHDEBINFO "-O2 -g")
    ADD_DEFINITIONS(-DTRINITYC_EXPORTS -D_CRT_SECURE_NO_WARNINGS -DUNICODE -D_UNICODE -DSOCKET_SERVER_TEST)
    SET(CMAKE_CXX_FLAGS "${CMAKE_CXX_FLAGS} -fpermissive -Wall -std=c++11 -Wno-write-strings -Wno-unknown-pragmas -g -pthread")
ENDIF()

IF(WIN32) 
    LINK_LIBRARIES(Psapi.lib;ws2_32.lib;Dbghelp.lib)
ENDIF()

IF(MSVC)
  ADD_DEFINITIONS(-DWCHAR_OUTPUT -DWIN32 -DNDEBUG -D_WINDOWS -D_USRDLL -DTRINITYC_EXPORTS -D_CRT_SECURE_NO_WARNINGS -DUNICODE -D_UNICODE)
  SET(COMPILE_DEFINITIONS  "WCHAR_OUTPUT;WIN32;NDEBUG;_WINDOWS;_USRDLL;TRINITYC_EXPORTS;_CRT_SECURE_NO_WARNINGS")
  SET(CMAKE_CXX_FLAGS "/volatile:iso /EHa /fp:except- /Zi /Gy /Oi /Ot /Ob2 /MP /Oy /O2 /MT /GS- /W3")
  SET(CMAKE_CXX_FLAGS_RELEASE "/GL")
  SET(CMAKE_SHARED_LINKER_FLAGS "/LTCG /OPT:REF /OPT:ICF /SUBSYSTEM:WINDOWS")
ENDIF()

#file(GLOB_RECURSE SRCS .cpp *.h *.rc)

SET(SRCS
./epoll/TrinitySocketServer.cpp
./iocp/TrinitySocketServer.ContextObject.cpp
./iocp/TrinitySocketServer.cpp
./iocp/TrinitySocketServer.Recv.cpp
./iocp/TrinitySocketServer.Send.cpp
./iocp/TrinitySocketServer.Threading.cpp
./kqueue/TrinitySocketServer.cpp
./posix/TrinityServer.cpp
./Common.cpp
./TrinitySocketServerTest.cpp
)

add_executable(socketserver.tapp ${SRCS})

