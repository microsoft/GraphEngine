CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)
PROJECT(Trinity.FFI.Python)

FIND_PACKAGE(PythonInterp 3)
FIND_PACKAGE(SWIG 3.0.10)

SET(TRINITY_FFI_NATIVE_DIR     "${CMAKE_CURRENT_SOURCE_DIR}/../Trinity.FFI.Native")
SET(GRAPHENGINE_JIT_NATIVE_DIR "${CMAKE_CURRENT_SOURCE_DIR}/../../GraphEngine.Jit/GraphEngine.Jit.Native")
SET(TRINITY_C_INCLUDE_DIR      "${TRINITY_C_DIR}/include")
SET(TRINITY_C_SOURCE_DIR       "${TRINITY_C_DIR}/src")
SET(PYPACKAGE_DIR              "${CMAKE_CURRENT_SOURCE_DIR}/GraphEngine")

FILE(GLOB_RECURSE pysrc   "${PYPACKAGE_DIR}/*.py")
FILE(GLOB         swigi   "${TRINITY_FFI_NATIVE_DIR}/Trinity.FFI.SWIG.i")
FILE(GLOB         swigh   "${TRINITY_FFI_NATIVE_DIR}/Trinity.FFI.SWIG.h"
                          "${TRINITY_FFI_NATIVE_DIR}/Trinity.FFI.Schema.h"
                          "${TRINITY_FFI_NATIVE_DIR}/Trinity.FFI.Native.h")
SET(SETUP_PY_IN           "${CMAKE_CURRENT_SOURCE_DIR}/setup.py.in")
SET(SETUP_PY              "${CMAKE_CURRENT_BINARY_DIR}/setup.py")
SET(swigpy                "${CMAKE_CURRENT_BINARY_DIR}/ffi.py")
SET(OUTPUT                "${CMAKE_CURRENT_BINARY_DIR}/build/timestamp")
SET(typesys               "${GRAPHENGINE_JIT_NATIVE_DIR}/TypeSystem.cpp")
SET(swigcxx               "${CMAKE_CURRENT_BINARY_DIR}/Trinity.FFI.SWIG_wrap.cxx")

SET(DEPS                  ${pysrc} ${swigi} ${typesys} ${swigh} Trinity GraphEngine.Jit.Native trinity_ffi)

IF (PYTHONINTERP_FOUND AND SWIG_FOUND)
    IF(WIN32)
        SET(ext_cxx_flags "['/Zi', '/D_CRT_SECURE_NO_WARNINGS', '/DUNICODE', '/D_UNICODE']")
        SET(ext_linker_flags "['/DEBUG']")
    ELSE()
        SET(ext_cxx_flags "['--std=c++14', '-fpermissive', '-D_CRT_SECURE_NO_WARNINGS', '-DUNICODE', '-D_UNICODE']")
        SET(ext_linker_flags "[]")
    ENDIF()

    CONFIGURE_FILE(${SETUP_PY_IN} ${SETUP_PY})

    ADD_CUSTOM_COMMAND(OUTPUT ${OUTPUT}
        COMMAND ${SWIG_EXECUTABLE} -modern -c++ -builtin -outcurrentdir -python "${swigi}"
                       COMMAND ${CMAKE_COMMAND} -E copy ${swigpy} ${PYPACKAGE_DIR}
                       COMMAND ${PYTHON_EXECUTABLE} ${SETUP_PY} bdist_wheel
                       COMMAND ${CMAKE_COMMAND} -E touch ${OUTPUT}
                       DEPENDS ${DEPS} trinity_ffi)

    ADD_CUSTOM_TARGET(REISEN_PY ALL DEPENDS ${OUTPUT})

    INSTALL(CODE "execute_process(COMMAND ${PYTHON_EXECUTABLE} ${SETUP_PY} install)")
ELSE()
    MESSAGE("Python environment not setup, skipping build")
ENDIF()
