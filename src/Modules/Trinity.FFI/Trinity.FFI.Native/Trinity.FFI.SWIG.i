%module ffi

%{
#define SWIG_FILE_WITH_INIT
#include "Trinity.FFI.SWIG.h"
%}

%inline %{
static TRINITY_INTERFACES* g_TrinityInterfaces;
%}

%newobject NewCell;
%newobject LoadCell;
%typemap(newfree) Cell* "delete $1;";

%include "Trinity.FFI.SWIG.h"
