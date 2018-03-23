%module ffi

%{
#define SWIG_FILE_WITH_INIT
#include "Trinity.FFI.SWIG.h"
%}

%include "std_vector.i"

namespace std {
	%template(cdesc_vec) vector<CellDescriptor>;
};

%inline %{
static TRINITY_INTERFACES* g_TrinityInterfaces;
%}

%newobject NewCell;
%newobject LoadCell;

%include "Trinity.FFI.SWIG.h"
