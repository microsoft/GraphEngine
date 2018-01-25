%module GraphEngine

%{
#define SWIG_FILE_WITH_INIT
#include "Trinity.FFI.SWIG.h"
%}

%inline %{
static TRINITY_INTERFACES* g_TrinityInterfaces;
%}

%include "Trinity.FFI.SWIG.h"