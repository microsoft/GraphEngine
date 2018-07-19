CMAKE_MINIMUM_REQUIRED(VERSION 3.8.0)
PROJECT(Trinity.C)

INCLUDE_DIRECTORIES(src)
INCLUDE_DIRECTORIES(include)

FILE(GLOB SRC src/*.cpp)
FILE(GLOB_RECURSE SRC ${SRC} include/*.cpp)
FILE(GLOB_RECURSE SRC ${SRC} src/BackgroundThread/*.cpp)
FILE(GLOB_RECURSE SRC ${SRC} src/Debugger/*.cpp)
FILE(GLOB_RECURSE SRC ${SRC} src/Mathematics/*.cpp)
FILE(GLOB_RECURSE SRC ${SRC} src/Memory/*.cpp)
FILE(GLOB_RECURSE SRC ${SRC} src/Runtime/*.cpp)
FILE(GLOB_RECURSE SRC ${SRC} src/Storage/*.cpp)
FILE(GLOB_RECURSE SRC ${SRC} src/Threading/*.cpp)
FILE(GLOB_RECURSE SRC ${SRC} src/Trinity/*.cpp)
FILE(GLOB_RECURSE SRC ${SRC} src/Utility/*.cpp)
FILE(GLOB SRC ${SRC} src/Network/*.cpp)
FILE(GLOB SRC ${SRC} src/Network/Client/*.cpp)
FILE(GLOB SRC ${SRC} src/Network/Server/*.cpp)

IF(UNIX) 
    FILE(GLOB_RECURSE SRC ${SRC} src/Network/*/posix/*.cpp)

    ADD_LIBRARY(Trinity SHARED ${SRC})

    ADD_DEFINITIONS(-DTRINITYC_EXPORTS -D_CRT_SECURE_NO_WARNINGS -DUNICODE -D_UNICODE)
    TARGET_COMPILE_DEFINITIONS(Trinity PRIVATE -DCORECLR)
    TARGET_LINK_LIBRARIES(Trinity PRIVATE crypto;ssl)

    POSTBUILD_COPY_OUTPUT(Trinity      "${TRINITY_LIB_DIR}/Trinity.dll")
    POSTBUILD_COPY_OUTPUT(Trinity      "${TRINITY_LIB_DIR}/Trinity.C.dll")

    INSTALL(TARGETS Trinity 
            LIBRARY DESTINATION ${TRINITY_INSTALL_DIR})
    INSTALL(FILES ${TRINITY_LIB_DIR}/Trinity.dll 
                  ${TRINITY_LIB_DIR}/Trinity.C.dll 
            DESTINATION ${TRINITY_INSTALL_DIR})

ELSEIF(WIN32)

    FILE(GLOB_RECURSE SRC ${SRC} src/Network/*/iocp/*.cpp)
    FILE(GLOB_RECURSE SRC ${SRC} src/Network/*/windows/*.cpp)

    ADD_LIBRARY(Trinity   SHARED ${SRC})
    ADD_LIBRARY(Trinity.C SHARED ${SRC})

    ADD_DEFINITIONS(-DTRINITYC_EXPORTS -D_CRT_SECURE_NO_WARNINGS -DUNICODE -D_UNICODE)
    ADD_DEFINITIONS(-DWIN32 -DNDEBUG -D_WINDOWS -D_USRDLL -D_WINSOCK_DEPRECATED_NO_WARNINGS -D_SCL_SECURE_NO_WARNINGS)

    TARGET_COMPILE_DEFINITIONS(Trinity PRIVATE -DCORECLR)
    TARGET_COMPILE_OPTIONS(Trinity     PRIVATE "/Fd Trinity.pdb")
    TARGET_COMPILE_OPTIONS(Trinity.C   PRIVATE "/Fd Trinity.C.pdb")
    TARGET_LINK_LIBRARIES(Trinity      PRIVATE Psapi.lib;ws2_32.lib;Dbghelp.lib;Synchronization.lib)
    TARGET_LINK_LIBRARIES(Trinity.C    PRIVATE Psapi.lib;ws2_32.lib;Dbghelp.lib;Synchronization.lib)

    POSTBUILD_COPY_OUTPUT(Trinity      "${PROJECT_BINARY_DIR}/Trinity.pdb")
    POSTBUILD_COPY_OUTPUT(Trinity      "${TRINITY_LIB_DIR}/libTrinity.so")
    POSTBUILD_COPY_OUTPUT(Trinity.C    "${PROJECT_BINARY_DIR}/Trinity.C.pdb")

    INSTALL(TARGETS Trinity Trinity.C
            DESTINATION ${TRINITY_INSTALL_DIR})
    INSTALL(FILES ${PROJECT_BINARY_DIR}/Trinity.C.pdb 
                  ${PROJECT_BINARY_DIR}/Trinity.pdb 
                  ${TRINITY_LIB_DIR}/libTrinity.so 
            DESTINATION ${TRINITY_INSTALL_DIR})

ENDIF()
