#pragma once

#include "asmjit.h"
using namespace asmjit;

struct FuncCtx
{
public:
    TypeId::Id retId;
    bool returned;
    int argIndex;
    X86Compiler& cc;
    X86Gp cellAccessor;
    X86Gp cellPtr;

public:
    FuncCtx(X86Compiler& compiler, TypeId::Id);
    void addArg(const Reg& reg);
    void ret();
    void ret(const X86Gp& gp);
    void ret(const X86Xmm& gp);
    asmjit::Error finalize();
};
