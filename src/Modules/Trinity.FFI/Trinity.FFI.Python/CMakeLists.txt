CMAKE_MINIMUM_REQUIRED(VERSION 3.8.0)
PROJECT(Trinity.FFI.Python)

FIND_PROGRAM(PYTHON "python")
FIND_PROGRAM(SWIG   "swig")

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

IF (PYTHON AND SWIG)
    IF(WIN32)
        SET(ext_cxx_flags "['/Zi']")
        SET(ext_linker_flags "['/DEBUG']")
    ELSE()
        SET(ext_cxx_flags "[]")
        SET(ext_linker_flags "[]")
    ENDIF()

    CONFIGURE_FILE(${SETUP_PY_IN} ${SETUP_PY})

    ADD_CUSTOM_COMMAND(OUTPUT ${OUTPUT}
                       COMMAND ${SWIG} -modern -c++ -builtin -outcurrentdir -python "${swigi}"
                       COMMAND ${CMAKE_COMMAND} -E copy ${swigpy} ${PYPACKAGE_DIR}
                       COMMAND ${PYTHON} ${SETUP_PY} bdist_wheel
                       COMMAND ${CMAKE_COMMAND} -E touch ${OUTPUT}
                       DEPENDS ${DEPS})

    ADD_CUSTOM_TARGET(target ALL DEPENDS ${OUTPUT})

    INSTALL(CODE "execute_process(COMMAND ${PYTHON} ${SETUP_PY} install)")
ENDIF()
