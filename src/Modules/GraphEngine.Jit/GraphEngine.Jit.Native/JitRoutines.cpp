#include "GraphEngine.Jit.Native.h"
#include <cstring>
#include <algorithm>

extern "C" char* tsl_getstring(int32_t* trinity_string_ptr)
{
    auto str = Trinity::String::FromWcharArray((u16char*)(trinity_string_ptr + 1), *trinity_string_ptr);
    return _strdup(str.c_str());
}

extern "C" char* tsl_getu8string(int32_t* trinity_string_ptr)
{
    int32_t len = 1 + *trinity_string_ptr;
    char* buf = (char*)malloc(len);
    if (strcpy_s(buf, len, (char*)(trinity_string_ptr + 1))) return nullptr;
    else return buf;
}

std::vector<int32_t>&& walk(TypeDescriptor* type)
{
    std::vector<int32_t> plan;
    auto tc  = type->get_TypeCode();
    auto tid = _get_typeid(type);

    if (tc == TypeCode::TC_STRING || tc == TypeCode::TC_U8STRING || tc == TypeCode::TC_LIST)
    {
        plan.push_back(-1); // direct jump
    }
    else if (tc == TypeCode::TC_CELL || tc == TypeCode::TC_STRUCT)
    {
        auto members = type->get_Members();
        int32_t accumulated_size = 0;

        for (auto *m : *members)
        {
            //TODO optional
            auto sub_walk = walk(&m->Type);
            plan.insert(plan.cend(), sub_walk.begin(), sub_walk.end());
        }

        delete members;
    }
    else 
    {
        plan.push_back(TypeId::sizeOf(tid));
    }

    return std::move(plan);
}

void simplify(std::vector<int32_t>& plan)
{
    auto adj = plan.begin();
    while (true)
    {
        adj = std::adjacent_find(adj, plan.end(), [](auto val) { return val > 0; });
        if (adj == plan.end())break;
        *adj += *(adj + 1);
        adj = plan.erase(adj + 1);
    }
}

JitRoutine(BGet)
{
    auto address = x86::ptr(ctx.cellPtr);
    auto tid = JitTypeId();
    auto tc = seq.CurrentType()->get_TypeCode();
    auto retreg = ctx.cellPtr.clone();
    retreg.setTypeAndId(tid, retreg.getId());

    if (tc == TypeCode::TC_STRING)
    {
        auto call = cc.call(imm_ptr(tsl_getstring), FuncSignature1<void*, void*>());
        call->setArg(0, ctx.cellPtr);
        call->setRet(0, retreg);
    }
    else if (tc == TypeCode::TC_U8STRING)
    {
        auto call = cc.call(imm_ptr(tsl_getu8string), FuncSignature1<void*, void*>());
        call->setArg(0, ctx.cellPtr);
        call->setRet(0, retreg);
    }
    else if (tid != TypeId::kUIntPtr)
    {
        // simple atom
        cc.mov(retreg, address);
    }
    else 
    {
        auto plan = walk(seq.CurrentType());
        simplify(plan);

        //TODO execute the plan
    }

    ctx.ret(retreg);
}

JitRoutine(BSet)
{
    auto address = x86::ptr(ctx.cellPtr);
    auto tid = JitTypeId();
    auto target = cc.newGpReg(tid);
    ctx.addArg(target);

    if (tid == TypeId::kUIntPtr)
    {
        throw;
    }
    else
    {
        cc.mov(address, target);
    }
    ctx.ret();
}

JitRoutine(SGet)
{
    int member_offset = 0; //TODO calculate offset of member

    cc.add(ctx.cellAccessor, member_offset);
}

JitRoutine(SSet)
{
    int member_offset = 0; //TODO calculate offset of member

    cc.add(ctx.cellPtr, member_offset);
    cc.ret();
    //TODO assign
}

JitRoutine(GSGet)
{

}
JitRoutine(GSSet)
{

}

JitRoutine(LGet)
{

}
JitRoutine(LSet)
{

}

JitRoutine(LInlineGet)
{

}
JitRoutine(LInlineSet)
{

}

JitRoutine(LContains)
{

}
JitRoutine(LCount)
{

}
