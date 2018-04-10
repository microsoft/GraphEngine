#pragma once

#include "asmjit.h"
using namespace asmjit;

struct FuncCtx
{
    FuncCtx(X86Gp* cp) : cellPtr(cp) { argIndex = 0; }
    int newArg() { return ++argIndex; }

    int argIndex;
    X86Gp* cellPtr;
};
#define JitRoutine(x) void x(X86Compiler &cc, FuncCtx& ctx, TypeDescriptor* type, Verb* v)
typedef void(*JitProc)(X86Compiler&, FuncCtx&, TypeDescriptor*, Verb*);

JitRoutine(BGet);
JitRoutine(BSet);

JitRoutine(SGet);
JitRoutine(SSet);

JitRoutine(GSGet);
JitRoutine(GSSet);

JitRoutine(GSGet);
JitRoutine(GSSet);

JitRoutine(LGet);
JitRoutine(LSet);

JitRoutine(LInlineGet);
JitRoutine(LInlineSet);

JitRoutine(LContains);
JitRoutine(LCount);
