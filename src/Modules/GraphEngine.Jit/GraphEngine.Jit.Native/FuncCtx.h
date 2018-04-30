#pragma once

#include "asmjit.h"
#include <vector>
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
    std::vector<X86Gp> resizeChain;
    bool wresize;

public:
    FuncCtx(X86Compiler& compiler, TypeId::Id, bool);
    void pushResizeChain();
    void addArg(const Reg& reg);
    void ret();
    void ret(const X86Gp& gp);
    void ret(const X86Xmm& gp);
    asmjit::Error finalize();
};
