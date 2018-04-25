#include "FuncCtx.h"

FuncCtx::FuncCtx(X86Compiler& compiler) : cc(compiler), argIndex(0), returned(false)
{
    cellAccessor = cc.newGpq("cellAccessor");
    cellPtr      = cc.newGpq("cellPtr");
    addArg(cellAccessor);

    // prolog: we always load the cell pointer into a gp register "cellPtr"
    // void* cellPtr = cellAccessor->cellPtr;
    cc.mov(cellPtr, x86::qword_ptr(cellAccessor));
}

void FuncCtx::addArg(Reg& reg)
{
    cc.setArg(argIndex++, reg);
}

void FuncCtx::ret()
{
    cc.ret();
    returned = true;
}

void FuncCtx::ret(X86Gp& gp)
{
    cc.ret(gp);
    returned = true;
}

asmjit::Error FuncCtx::finalize()
{
    if (!returned) ret(cellPtr);
    return cc.finalize();
}
