FUNCTION(POSTBUILD_COPY_OUTPUT target file)
    CMAKE_PARSE_ARGUMENTS(
        # prefix
        _PB 
        # options (flags)
        "ARCHIVE;RUNTIME;LIBRARY" 
        # oneValueArgs
        "RENAME" 
        # multiValueArgs
        ""
        # the input arguments
        ${ARGN})
    GET_FILENAME_COMPONENT(filename ${file} NAME)

    SET(dir ${CMAKE_LIBRARY_OUTPUT_DIRECTORY})
    IF(_PB_ARCHIVE)
        SET(dir ${CMAKE_ARCHIVE_OUTPUT_DIRECTORY})
    ELSEIF(_PB_RUNTIME)
        SET(dir ${CMAKE_RUNTIME_OUTPUT_DIRECTORY})
    ELSEIF(_PB_LIBRARY)
        SET(dir ${CMAKE_LIBRARY_OUTPUT_DIRECTORY})
    ENDIF()

    IF(NOT dir)
        SET(dir ${CMAKE_BINARY_DIR})
    ENDIF()

    ADD_CUSTOM_COMMAND(
        TARGET ${target}
        POST_BUILD
        COMMAND ${CMAKE_COMMAND} -E make_directory "${dir}"
        COMMAND ${CMAKE_COMMAND} -E copy ${file} "${dir}/${_PB_RENAME}"
        BYPRODUCTS "${dir}/${filename}"
    )
ENDFUNCTION()

FUNCTION(POSTBUILD_XPLAT_OUTPUT target xplat_dir)
    GET_TARGET_PROPERTY(target_type ${target} TYPE)

    IF(target_type STREQUAL "EXECUTABLE")

        INSTALL(TARGETS ${target} RUNTIME DESTINATION bin)
        IF(UNIX)
            POSTBUILD_COPY_OUTPUT(${target} "${xplat_dir}/${target}.exe" RUNTIME)
            INSTALL(FILES ${xplat_dir}/${target}.exe DESTINATION bin)
        ELSEIF(WIN32)
            POSTBUILD_COPY_OUTPUT(${target} "${xplat_dir}/${target}" RUNTIME)
            INSTALL(FILES ${xplat_dir}/${target} DESTINATION bin)
        ENDIF()

    ELSEIF(target_type STREQUAL "SHARED_LIBRARY")

        INSTALL(TARGETS ${target} DESTINATION lib)
        IF(UNIX)
            POSTBUILD_COPY_OUTPUT(${target} "${xplat_dir}/${target}.dll" LIBRARY)
            POSTBUILD_COPY_OUTPUT(${target} "${xplat_dir}/${target}.lib" LIBRARY)
            INSTALL(FILES ${xplat_dir}/${target}.dll DESTINATION lib)
        ELSEIF(WIN32)
            POSTBUILD_COPY_OUTPUT(${target} "${xplat_dir}/lib${target}.so" LIBRARY)
            INSTALL(FILES ${xplat_dir}/lib${target}.so DESTINATION lib)
        ENDIF()

    ELSE()
        MESSAGE(ERROR "Unsupported target type")
    ENDIF()

    IF(WIN32)
    ENDIF()

ENDFUNCTION()

FUNCTION(SET_OUTPUT_DIRECTORY dir)

    IF(OUTPUT_DIRECTORY_IS_SET)
        RETURN()
    ENDIF()

    SET(CMAKE_RUNTIME_OUTPUT_DIRECTORY ${dir} PARENT_SCOPE)
    SET(CMAKE_LIBRARY_OUTPUT_DIRECTORY ${dir} PARENT_SCOPE)
    SET(CMAKE_ARCHIVE_OUTPUT_DIRECTORY ${dir} PARENT_SCOPE)

    SET(CMAKE_RUNTIME_OUTPUT_DIRECTORY_RELEASE ${dir} PARENT_SCOPE)
    SET(CMAKE_LIBRARY_OUTPUT_DIRECTORY_RELEASE ${dir} PARENT_SCOPE)
    SET(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_RELEASE ${dir} PARENT_SCOPE)

    SET(CMAKE_RUNTIME_OUTPUT_DIRECTORY_DEBUG ${dir} PARENT_SCOPE)
    SET(CMAKE_LIBRARY_OUTPUT_DIRECTORY_DEBUG ${dir} PARENT_SCOPE)
    SET(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_DEBUG ${dir} PARENT_SCOPE)

    SET(CMAKE_RUNTIME_OUTPUT_DIRECTORY_RELWITHDEBINFO ${dir} PARENT_SCOPE)
    SET(CMAKE_LIBRARY_OUTPUT_DIRECTORY_RELWITHDEBINFO ${dir} PARENT_SCOPE)
    SET(CMAKE_ARCHIVE_OUTPUT_DIRECTORY_RELWITHDEBINFO ${dir} PARENT_SCOPE)

    SET(OTUPUT_DIRECTORY_IS_SET TRUE PARENT_SCOPE)

ENDFUNCTION()