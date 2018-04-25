#pragma once

#include "asmjit.h"
using namespace asmjit;

struct FuncCtx
{
public:
    bool returned;
    int argIndex;
    X86Compiler& cc;
    X86Gp cellAccessor;
    X86Gp cellPtr;

public:
    FuncCtx(X86Compiler& compiler);
    void addArg(Reg& reg);
    void ret();
    void ret(X86Gp& gp);
    asmjit::Error finalize();
};
