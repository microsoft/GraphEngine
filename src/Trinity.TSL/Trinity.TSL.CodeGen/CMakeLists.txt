CMAKE_MINIMUM_REQUIRED(VERSION 3.0.0)

FILE(GLOB codegen_srcs
    common.cpp
    AccessorInitialization.cpp
    AccessorToAccessorFieldAssignment.cpp
    Containers.cpp
    PushPointer.cpp
    StructAccessorReverseImplicitOperator.cpp
    Trinity.TSL.CodeGen.cpp
    main.cpp)

# At configuration time, these files may be unavailable, so we set instead of glob.
SET(metagen_out
    ${CMAKE_CURRENT_BINARY_DIR}/Cell.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/CellSelectors.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/CellTypeEnum.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/CellTypeExtension.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/CommunicationSchema.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Enum.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/ExtensionAttribute.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/ExternalParser.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/GenericCell.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/GenericFieldAccessor.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/HTTP.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Index.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/MessageAccessors.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Protocols.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Serializer.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/StorageSchema.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Struct.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Throw.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Traits.cs.cpp

    ${CMAKE_CURRENT_BINARY_DIR}/Containers/Array.cs.cpp                                                                                                                                                                                                                                       
    ${CMAKE_CURRENT_BINARY_DIR}/Containers/List.cs.cpp

    ${CMAKE_CURRENT_BINARY_DIR}/InvertedIndex/Bigram/Indexer.cs.cpp                                                                                                                                                                                                                                     
    ${CMAKE_CURRENT_BINARY_DIR}/InvertedIndex/Bigram/IndexItem.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/InvertedIndex/Bigram/Searcher.cs.cpp

    ${CMAKE_CURRENT_BINARY_DIR}/Libraries/BufferAllocator.cs.cpp                                                                                                                                                                                                                             
    ${CMAKE_CURRENT_BINARY_DIR}/Libraries/byteListAccessor.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Libraries/DateTimeAccessor.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Libraries/doubleListAccessor.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Libraries/EnumAccessor.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Libraries/GuidAccessor.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Libraries/intListAccessor.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Libraries/longListAccessor.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Libraries/StringAccessor.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/Libraries/U8StringAccessor.cs.cpp

    ${CMAKE_CURRENT_BINARY_DIR}/LINQ/ExpressionTreeRewritter.cs.cpp                                                                                                                                                                                                                     
    ${CMAKE_CURRENT_BINARY_DIR}/LINQ/IndexQueryTreeExecutor.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/LINQ/IndexQueryTreeNode.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/LINQ/PLINQWrapper.cs.cpp

    ${CMAKE_CURRENT_BINARY_DIR}/MODULES/AccessorFieldsDefinition.cs.cpp                                                                                                                                                                                                                    
    ${CMAKE_CURRENT_BINARY_DIR}/MODULES/AccessorReverseImplicitOperator.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/MODULES/CommunicationClass.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/MODULES/CommunicationSchemaModule.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/MODULES/EnumerateFromFieldModule.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/MODULES/HTTPModule.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/MODULES/LegacyMessagePassingMethods.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/MODULES/MessagePassingMethods.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/MODULES/OptionalFields.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/MODULES/SerializeParametersToBuffer.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/MODULES/StructAccessorEqualOperator.cs.cpp
    ${CMAKE_CURRENT_BINARY_DIR}/MODULES/ValueToAccessorFieldAssignment.cs.cpp)


ADD_DOTNET(../Trinity.TSL.CodeTemplates/Trinity.TSL.CodeTemplates.csproj)
ADD_DOTNET(../Trinity.TSL.Metagen/Trinity.TSL.Metagen.csproj)
RUN_DOTNET(../Trinity.TSL.Metagen/Trinity.TSL.Metagen.csproj
           ARGUMENTS ../Trinity.TSL.CodeTemplates ${CMAKE_CURRENT_BINARY_DIR}
           OUTPUT ${metagen_out})

# before transpilation, validate the templates by compiling the project
ADD_DEPENDENCIES(RUN_Trinity.TSL.Metagen BUILD_Trinity.TSL.CodeTemplates)
ADD_EXECUTABLE(Trinity.TSL.CodeGen ${codegen_srcs} ${metagen_out})

TARGET_LINK_LIBRARIES(Trinity.TSL.CodeGen PRIVATE Trinity.TSL.Parser Trinity.BCL)
TARGET_INCLUDE_DIRECTORIES(Trinity.TSL.CodeGen PRIVATE ${CMAKE_CURRENT_SOURCE_DIR})
INSTALL(TARGETS Trinity.TSL.CodeGen RUNTIME DESTINATION bin)

POSTBUILD_XPLAT_OUTPUT(Trinity.TSL.CodeGen ${XPLAT_LIB_DIR})

