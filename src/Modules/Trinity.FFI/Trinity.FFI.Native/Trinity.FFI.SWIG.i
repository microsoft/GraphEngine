%module ffi
%include <stdint.i>
%include <pybuffer.i>
%pybuffer_mutable_string(char *str);

%{
#define SWIG_FILE_WITH_INIT
#define SWIG_PYTHON_STRICT_BYTE_CHAR
#include "Trinity.FFI.SWIG.h"
%}

%include "attribute.i"
%include "std_vector.i"
%include "std_wstring.i"

namespace std {
	%template(tdesc_vec) vector<TypeDescriptor>;
	%template(ptdesc_vec) vector<TypeDescriptor*>;
	%template(pmdesc_vec) vector<MemberDescriptor*>;
	%template(padesc_vec) vector<AttributeDescriptor*>;
	%template(wstring_vec) vector<wstring>;
};

%newobject NewCell;
%newobject LoadCell;

%attribute(TypeDescriptor, char*, TypeName, get_TypeName)
%attribute(TypeDescriptor, char*, QualifiedName, get_QualifiedName)
%attribute(TypeDescriptor, int16_t, TypeCode, get_TypeCode)
%attribute(TypeDescriptor, uint16_t, CellType, get_CellType)
%attribute(TypeDescriptor, std::vector<TypeDescriptor*>, ElementType, get_ElementType)
%attribute(TypeDescriptor, std::vector<MemberDescriptor*>, Members, get_Members)
%attribute(TypeDescriptor, std::vector<AttributeDescriptor*>, TSLAttributes, get_TSLAttributes)

%include "../../GraphEngine.Jit/GraphEngine.Jit.Native/TypeSystem.h"
%include "Trinity.FFI.SWIG.h"

