#pragma once

#include "asmjit.h"
#include "TrinityCommon.h"
#include "TypeSystem.h"
#include <vector>
#include <map>
#include <memory>
#include "Storage\LocalStorage\LocalMemoryStorage.h"
#include "CellAccessor.h"
using namespace asmjit;

#define print(x) std::wcout << x << std::endl
#define debug(x) std::wcout << #x << " = " << (x) << std::endl


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

struct VerbSequence
{
private:
    TypeDescriptor*   parent;
    TypeDescriptor*   type;
    Verb*             pstart;
    Verb*             pend;
    Verb*             pcurrent;

    MemberDescriptor* pmember;
    int32_t           imember;
public:

    VerbSequence(FunctionDescriptor* f);

    bool Next();
    Verb* CurrentVerb();
    TypeDescriptor* CurrentType();
    TypeDescriptor* ParentType();
    MemberDescriptor* CurrentMember();
};

class ErrHandler : public ErrorHandler
{
public:
    bool handleError(asmjit::Error err, const char* message, CodeEmitter* origin)
    {
        debug(message);
        return false;
    }
};

TypeId::Id _get_typeid(IN TypeDescriptor* const type);
TypeId::Id _get_retid(IN FunctionDescriptor* fdesc);
void _get_args(IN FunctionDescriptor* fdesc, OUT uint8_t* &pargs, OUT int32_t& nargs);

#define JitRoutine(x) void x(X86Compiler &cc, FuncCtx& ctx, VerbSequence& seq)
#define JitTypeId() _get_typeid(seq.CurrentType())

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
