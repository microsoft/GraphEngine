#include "FuncCtx.h"
#include "Common.h"

FuncCtx::FuncCtx(X86Compiler& compiler, TypeId::Id ret) : cc(compiler), argIndex(0), returned(false), retId(ret)
{
    cellAccessor = cc.newGpq("cellAccessor");
    cellPtr      = cc.newGpq("cellPtr");
    addArg(cellAccessor);

    // prolog: we always load the cell pointer into a gp register "cellPtr"
    // void* cellPtr = cellAccessor->cellPtr;
    cc.mov(cellPtr, x86::qword_ptr(cellAccessor));
}

void FuncCtx::addArg(const Reg& reg)
{
    cc.setArg(argIndex++, reg);
}

void FuncCtx::ret()
{
    cc.ret();
    returned = true;
}

void FuncCtx::ret(const X86Gp& gp)
{
    cc.ret(gp);
    returned = true;
}

void FuncCtx::ret(const X86Xmm& xmm)
{
    cc.ret(xmm);
    returned = true;
}

asmjit::Error FuncCtx::finalize()
{
    if (!returned) //assume ret value in reg cellPtr
    {
        auto size = asmjit::TypeId::sizeOf(retId);
        switch (size)
        {
        case 0:
            ret();
            break;
        case 1:
            ret(cellPtr.r8());
            break;
        case 2:
            ret(cellPtr.r16());
            break;
        case 4:
            ret(cellPtr.r32());
            break;
        case 8:
            ret(cellPtr);
            break;
        default:
            print(__FUNCTION__);
            print("unsupported default return size");
            debug(size);
            throw;
        }
    }
    return cc.finalize();
}
