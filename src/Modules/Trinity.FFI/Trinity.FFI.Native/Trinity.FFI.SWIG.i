%module ffi

%{
#define SWIG_FILE_WITH_INIT
#include "Trinity.FFI.SWIG.h"
%}

%include "std_vector.i"

namespace std {
	%template(tdesc_vec) vector<TypeDescriptor>;
	%template(ptdesc_vec) vector<TypeDescriptor*>;
	%template(pmdesc_vec) vector<MemberDescriptor*>;
	%template(padesc_vec) vector<AttributeDescriptor*>;

};

%newobject NewCell;
%newobject LoadCell;

%include "Trinity.FFI.Schema.h"
%include "Trinity.FFI.SWIG.h"

