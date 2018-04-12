#include "GraphEngine.Jit.Native.h"
#include "Trinity.h"
#include <cstring>
#include <algorithm>
#include <functional>

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

extern "C" void* tsl_copy(char* ptr, int32_t size)
{
    auto buf = malloc(size);
    return memcpy(buf, ptr, size);
}

extern "C" void* tsl_copy_dynamic(int32_t* ptr)
{
    auto len = sizeof(int) + *ptr;
    return tsl_copy((char*)ptr, len);
}

extern "C" void tsl_assign(CellAccessor* accessor, char* dst, char* src, int32_t size_dst, int32_t size_src)
{
    if (size_dst != size_src) 
    {
        auto offset = dst - accessor->cellPtr;
        ::CResizeCell(accessor->cellId, accessor->entryIndex, offset, size_src - size_dst, accessor->cellPtr);
        dst = accessor->cellPtr + offset;
    }

    memcpy(dst, src, size_src);
}

extern "C" void tsl_setstring(CellAccessor* accessor, int32_t* p, u16char* str)
{
    auto tlen = *p;
    auto slen = wcslen(str);

    *p = slen * sizeof(u16char);
    tsl_assign(accessor, (char*)(p + 1), (char*)str, tlen, slen);
}

extern "C" char* tsl_setu8string(CellAccessor* accessor, int32_t* p, char* trinity_string_ptr)
{
    Trinity::String str(trinity_string_ptr);
    int32_t tlen = *p;
    int32_t slen = str.Length();

    *p = slen;
    tsl_assign(accessor, (char*)(p + 1), (char*)str.c_str(), tlen, slen);
}

// type-safe call
template <class TRET, class... TARGS>
CCFuncCall* safecall(X86Compiler &cc, TRET (*func)(TARGS...))
{
    return cc.call(imm_ptr(func), FuncSignatureT<TRET, TARGS...>());
}

void push(X86Compiler &cc, X86Gp& reg, std::vector<int32_t>& plan)
{
    for (auto i : plan)
    {
        if (i > 0) 
        {
            cc.add(reg, i);
        }
        else
        {
            cc.add(reg, x86::dword_ptr(reg));
            cc.add(reg, sizeof(int32_t));
        }
    }
}

std::vector<int32_t>&& walk(TypeDescriptor* type, MemberDescriptor* this_member = nullptr)
{
    std::vector<int32_t> plan;
    auto tc = type->get_TypeCode();
    auto tid = _get_typeid(type);

    if (tc == TypeCode::TC_STRING || tc == TypeCode::TC_U8STRING || tc == TypeCode::TC_LIST)
    {
        // jump
        plan.push_back(-1);
    }
    else if (tc == TypeCode::TC_CELL || tc == TypeCode::TC_STRUCT)
    {
        // walk
        auto members = type->get_Members();
        int32_t accumulated_size = 0;

        for (auto *m : *members)
        {
            // arrived at this member?
            if (m == this_member) break;

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

JitRoutine(BGet)
{
    auto address = x86::ptr(ctx.cellPtr);
    auto tid = JitTypeId();
    auto tc = seq.CurrentType()->get_TypeCode();
    auto retreg = ctx.cellPtr.clone();
    retreg.setTypeAndId(tid, retreg.getId());

    if (tc == TypeCode::TC_STRING)
    {
        auto call = safecall(cc, tsl_getstring);
        call->setArg(0, ctx.cellPtr);
        call->setRet(0, retreg);
    }
    else if (tc == TypeCode::TC_U8STRING)
    {
        auto call = safecall(cc, tsl_getu8string);
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

        if (plan.size() == 0) return; // empty struct
        else if (plan.size() == 1 && plan[0] == -1) // jump
        {
            auto call = safecall(cc, tsl_copy_dynamic);
            call->setArg(0, ctx.cellPtr);
            call->setRet(0, retreg);
        }
        else if (plan.size() > 1) // walking
        {
            // void* p = cellPtr;
            auto p = cc.newGpq("p");
            cc.mov(p, ctx.cellPtr);

            push(cc, p, plan);

            // p at end, calculate len
            cc.sub(p, ctx.cellPtr);
            auto call = safecall(cc, tsl_copy);
            call->setArg(0, ctx.cellPtr);
            call->setArg(1, p.as<X86Gpd>());
            call->setRet(0, retreg);
        }
        else //fixed copying
        {
            auto call = safecall(cc, tsl_copy);
            call->setArg(0, ctx.cellPtr);
            call->setArg(1, imm(plan[0]));
            call->setRet(0, retreg);
        }
    }

    ctx.ret(retreg);
}

JitRoutine(BSet)
{
    auto address = x86::ptr(ctx.cellPtr);
    auto tid = JitTypeId();
    auto tc = seq.CurrentType()->get_TypeCode();
    auto src = cc.newGpReg(tid);
    ctx.addArg(src);

    if (tc == TypeCode::TC_STRING)
    {
        auto call = safecall(cc, tsl_setstring);
        call->setArg(0, ctx.cellAccessor);
        call->setArg(1, ctx.cellPtr);
        call->setArg(2, src);
    }
    else if (tc == TypeCode::TC_U8STRING)
    {
        auto call = safecall(cc, tsl_setstring);
        call->setArg(0, ctx.cellAccessor);
        call->setArg(1, ctx.cellPtr);
        call->setArg(2, src);
    }
    else if (tid != TypeId::kUIntPtr)
    {
        // simple atom
        cc.mov(address, src);
    }
    else
    {
        auto plan = walk(seq.CurrentType());

        if (plan.size() == 0) return; // empty struct
        else if (plan.size() == 1 && plan[0] == -1) // jump
        {
            auto src_len_gp = cc.newGpw("src_len");
            auto dst_len_gp = cc.newGpw("dst_len");

            //TODO optional

            // load length headers
            cc.mov(src_len_gp, x86::dword_ptr(src));
            cc.mov(dst_len_gp, x86::dword_ptr(ctx.cellPtr));
            // set length header of dst
            cc.mov(x86::dword_ptr(dst_len_gp), src_len_gp);
            cc.add(ctx.cellPtr, 4);
            cc.add(src, 4);

            auto call = safecall(cc, tsl_assign);
            call->setArg(0, ctx.cellAccessor);
            call->setArg(1, ctx.cellPtr);
            call->setArg(2, src);
            call->setArg(3, dst_len_gp);
            call->setArg(4, src_len_gp);
        }
        else if (plan.size() > 1) // walking
        {
            // void* p = src;
            // void* q = cellPtr
            auto p = cc.newGpq("p");
            auto q = cc.newGpq("q");
            cc.mov(p, src);
            cc.mov(q, ctx.cellPtr);
            push(cc, p, plan);
            push(cc, q, plan);
            // p & q at end, calculate len
            cc.sub(p, src);
            cc.sub(q, ctx.cellPtr);

            auto call = safecall(cc, tsl_assign);

            call->setArg(0, ctx.cellAccessor);
            call->setArg(1, ctx.cellPtr);
            call->setArg(2, src);
            call->setArg(3, q);
            call->setArg(4, p);
        }
        else //fixed copying
        {
            auto call = safecall(cc, memcpy);
            call->setArg(0, ctx.cellPtr);
            call->setArg(1, src);
            call->setArg(2, imm(plan[0]));
        }
    }

    ctx.ret();
}

JitRoutine(SGet)
{
    auto plan = walk(seq.ParentType(), seq.CurrentMember());
    push(cc, ctx.cellPtr, plan);
}

JitRoutine(SSet)
{
    SGet(cc, ctx, seq);
    BSet(cc, ctx, seq);
}

JitRoutine(GSGet)
{
    //TODO reflection layer for generic interfaces
}

JitRoutine(GSSet)
{

}

JitRoutine(LGet)
{
    auto index = cc.newGpd("index");
    auto l = cc.newLabel();
    ctx.addArg(index);
    // jump over length header
    cc.add(ctx.cellPtr, sizeof(int32_t));

    auto plan = walk(seq.CurrentType());

    if (plan.size() == 1 && plan[0] > 0)
    {
        //fixed element type

        int size = plan[0];
        int bits = 0;
        int shift = 0;
        for (int i = 0; i < 32; ++i) {
            if (size & 1) {
                ++bits;
                shift = i;
            }

            size >>= 1;
        }

        if (bits == 1)
        {
            cc.lea(ctx.cellPtr, x86::ptr(ctx.cellPtr, index, shift));
        }
        //TODO else if (bits == 2)
        //maybe an opt pass for such things
        else
        {
            auto eax = x86::eax;
            cc.imul(index, plan[0]);
            cc.lea(ctx.cellPtr, x86::ptr(index));
        }
    }
    else
    {
        //keep walking
        auto loop = cc.newGpd("loop");
        cc.xor_(loop, loop);

        cc.bind(l);
        push(cc, ctx.cellPtr, plan);
        cc.inc(loop);
        cc.cmp(loop, index);
        cc.jb(l);
    }
}

JitRoutine(LSet)
{
    LGet(cc, ctx, seq);
    BSet(cc, ctx, seq);
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
