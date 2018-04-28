#include "Common.h"
#include <cstring>
#include <algorithm>
#include <functional>
#include <numeric>
#include "VerbMixins.h"

namespace Mixin
{
#define JITCALL(x) Make(VerbCode::VC_##x)->Dispatch(cc, ctx, seq);

    // type-safe call
    template <class TRET, class... TARGS>
    CCFuncCall* safecall(X86Compiler &cc, TRET(*func)(TARGS...))
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
                //TODO optional
                auto tmp = cc.newGpq("tmp");
                cc.movsxd(tmp, x86::ptr(reg, 0, sizeof(int32_t)));
                cc.add(reg, tmp);
                cc.add(reg, sizeof(int32_t));
            }
        }
    }

    std::vector<int32_t> walk(TypeDescriptor* type, MemberDescriptor* this_member = nullptr)
    {
        std::vector<int32_t> plan;
        auto tc = type->get_TypeCode();
        auto tid = GetTypeId(type);

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

        return plan;
    }

    auto is_bool(TypeCode code) { return code == TypeCode::TC_BOOL; }
    auto is_char(TypeCode code) { return code == TypeCode::TC_CHAR; }
    auto is_float(TypeCode code)
    {
        switch (code)
        {
        case TypeCode::TC_F32:
        case TypeCode::TC_F64:
            return true;
        default:
            return false;
        }
    }
    auto is_signed_int(TypeCode code)
    {
        switch (code)
        {
        case TypeCode::TC_I8:
        case TypeCode::TC_I16:
        case TypeCode::TC_I32:
        case TypeCode::TC_I64:
            return true;
        default:
            return false;
        }
    }
    auto is_unsigned_int(TypeCode code)
    {
        switch (code)
        {
        case TypeCode::TC_U8:
        case TypeCode::TC_U16:
        case TypeCode::TC_U32:
        case TypeCode::TC_U64:
            return true;
        default:
            return false;
        }
    }
    auto is_tslhead(TypeCode code)
    {
        switch (code)
        {
        case TypeCode::TC_U8STRING:
        case TypeCode::TC_STRING:
        case TypeCode::TC_LIST:
            return true;
        default:
            return false;
        }
    }
    auto is_atom(TypeCode code) { return is_bool(code) || is_char(code) || is_float(code) || is_signed_int(code) || is_unsigned_int(code); }

    int32_t calculate_shift(int32_t size)
    {
        int bits = 0;
        int shift = 0;
        for (int i = 0; i < 32; ++i)
        {
            if (size & 1)
            {
                ++bits;
                shift = i;
            }

            size >>= 1;
        }

        if (bits != 1) return -1;
        else return shift;
    }

    void div32(X86Compiler& cc, const X86Gp& t, int32_t len)
    {
        auto shift = calculate_shift(len);
        if (shift == 0)
        {
            //just return len
        }
        else if (shift > 0)
        {
            cc.shr(t, shift);
        }
        else
        {
            auto dummy   = cc.newInt32("dummy");
            auto divisor = cc.newInt32("size");
            cc.xor_(dummy, dummy);
            cc.mov(divisor, imm(len));
            cc.idiv(dummy, t, divisor);
        }
    }

    template<bool move_pointers>
    X86Gp compare_block(X86Compiler& cc, X86Gp& p1, X86Gp& p2, X86Gp& len1, X86Gp& len2)
    {
        auto lg = cc.newLabel();
        auto ll = cc.newLabel();
        auto label2 = cc.newLabel();

        cc.cmp(len1, len2);
        cc.jg(lg);
        cc.jl(ll);

        if (move_pointers)
        {
            cc.add(p1, sizeof(int32_t));
            cc.add(p2, sizeof(int32_t));
        }

        auto retreg = cc.newGpReg(TypeId::kI32, "cmpresult");
        auto call = safecall(cc, memcmp);
        call->setArg(0, p1);
        call->setArg(1, p2);
        call->setArg(2, len1);
        call->setRet(0, retreg);
        cc.jmp(label2);

        cc.bind(lg);
        cc.mov(retreg, imm(1));
        cc.jmp(label2);

        cc.bind(ll);
        cc.mov(retreg, imm(-1));
        cc.bind(label2);

        return retreg;
    }

    X86Gp compare_block(X86Compiler& cc, X86Gp& p1, X86Gp& p2)
    {
        auto len1 = cc.newGpd("len1");
        auto len2 = cc.newGpd("len2");

        cc.mov(len1, x86::dword_ptr(p1));
        cc.mov(len2, x86::dword_ptr(p2));

        return compare_block<true>(cc, p1, p2, len1, len2);
    }

    X86Gp push_get_len(X86Compiler& cc, const char* name, X86Gp& base, std::vector<int32_t>& plan)
    {
        auto p = cc.newGpq(name);
        cc.mov(p, base);
        push(cc, p, plan);
        cc.sub(p, base);
        return p;
    }

    X86Gp compare(X86Compiler& cc, FuncCtx& ctx, const VerbSequence& seq)
    {
        auto tid = seq.CurrentTypeId();
        auto tc = seq.CurrentTypeCode();

        X86Gp val;
        X86Gp retreg;

        if (!is_float(tc))
        {
            val = cc.newGpReg(tid, "value");
            ctx.addArg(val);
        }

        if (is_tslhead(tc))
        {
            retreg = compare_block(cc, ctx.cellPtr, val);
        }
        else if (is_atom(tc))
        {
            // simple atom
            auto atomsize = asmjit::TypeId::sizeOf(tid);
            auto l1 = cc.newLabel();
            auto l2 = cc.newLabel();
            retreg = cc.newGpReg(TypeId::kI32, "cmpresult");

            cc.xor_(retreg, retreg);

            if (is_signed_int(tc))
            {
                cc.cmp(x86::ptr(ctx.cellPtr, 0, atomsize), val);
                cc.jg(l1);
                cc.je(l2);
                cc.mov(retreg, imm(-1));
                cc.jmp(l2);
                cc.bind(l1);
                cc.mov(retreg, imm(1));
                cc.bind(l2);
            }
            else if (is_unsigned_int(tc) || is_char(tc) || is_bool(tc))
            {
                cc.cmp(x86::ptr(ctx.cellPtr, 0, atomsize), val);
                cc.ja(l1);
                cc.je(l2);
                cc.mov(retreg, imm(-1));
                cc.jmp(l2);
                cc.bind(l1);
                cc.mov(retreg, imm(1));
                cc.bind(l2);
            }
            else if (is_float(tc))
            {
                auto xmm = cc.newXmm("xmm");
                ctx.addArg(xmm);

                /*
                (V)UCOMISS (all versions)
                RESULT <- UnorderedCompare(DEST[31:0] <> SRC[31:0]) {
                (* Set EFLAGS *) CASE (RESULT) OF
                    UNORDERED:    ZF,PF,CF <- 111;
                    GREATER_THAN: ZF,PF,CF <- 000;
                    LESS_THAN:    ZF,PF,CF <- 001;
                    EQUAL:        ZF,PF,CF <- 100;
                ESAC;
                OF, AF, SF <- 0; }
                */

                if (atomsize == 4) cc.ucomiss(xmm, x86::ptr(ctx.cellPtr, 0, atomsize));
                else cc.ucomisd(xmm, x86::ptr(ctx.cellPtr, 0, atomsize));

                cc.ja(l1);
                cc.je(l2);
                cc.mov(retreg, imm(1));
                cc.jmp(l2);
                cc.bind(l1);
                cc.mov(retreg, imm(-1));
                cc.bind(l2);
            }
            else
            {
                print("unexpected atom type");
                debug(tc);
                throw;
            }
        }
        else
        {
            auto plan = walk(seq.ptype);
            assert(tid == TypeId::kUIntPtr);

            if (plan.size() == 0)
            {
                // empty struct, return true
                retreg = cc.newGpd();
                cc.mov(retreg, imm(1));
            }
            else if (plan.size() == 1 && plan[0] == -1) // single jump block
            {
                retreg = compare_block(cc, ctx.cellPtr, val);
            }
            else if (plan.size() > 1) // walking
            {
                auto p = push_get_len(cc, "p", val, plan);
                auto q = push_get_len(cc, "q", ctx.cellPtr, plan);

                retreg = compare_block<false>(cc, ctx.cellPtr, val, q, p);
            }
            else //fixed 
            {
                retreg = cc.newGpReg(TypeId::kI32, "cmpresult");
                auto call = safecall(cc, memcmp);
                auto len1 = imm(plan[0]);
                call->setArg(0, ctx.cellPtr);
                call->setArg(1, val);
                call->setArg(2, len1);
                call->setRet(0, retreg);
            }
        }

        return retreg;
    }

    X86Gp calc_size(X86Compiler& cc, FuncCtx &ctx, const VerbSequence& seq)
    {
        auto tid = seq.CurrentTypeId();
        auto tc = seq.CurrentTypeCode();

        auto ret = cc.newGpd("size");

        if (is_tslhead(tc))
        {
            cc.mov(ret, x86::dword_ptr(ctx.cellPtr));
        }
        else if (is_atom(tc))
        {
            // simple atom
            auto atomsize = asmjit::TypeId::sizeOf(tid);
            cc.mov(ret, imm(atomsize));
        }
        else
        {
            auto plan = walk(seq.ptype);
            assert(tid == TypeId::kUIntPtr);

            if (plan.size() == 0)
            {
                cc.xor_(ret, ret);
            }
            else if (plan.size() == 1 && plan[0] == -1) // single jump block
            {
                cc.mov(ret, x86::dword_ptr(ctx.cellPtr));
            }
            else if (plan.size() > 1) // walking
            {
                auto q = push_get_len(cc, "q", ctx.cellPtr, plan);
                cc.mov(ret, q.r32());
            }
            else //fixed
            {
                cc.mov(ret, imm(plan[0]));
            }
        }
    }

    CONCRETE_MIXIN_DEFINE(BNew)
    {
        print(__FUNCTION__);

        auto retreg = cc.newGpd("err");
        auto plan = walk(seq.ptype);
        auto len = std::accumulate(plan.begin(), plan.end(), 0, [](int32_t cum, int32_t p) { return cum + (p >= 0 ? p : sizeof(int32_t)); });

        auto call = safecall(cc, tsl_newaccessor);
        call->setArg(0, ctx.cellAccessor);
        call->setArg(1, imm(len));
        call->setRet(0, retreg);

        ctx.ret(retreg);
    }

    CONCRETE_MIXIN_DEFINE(BGet)
    {
        print(__FUNCTION__);

        auto tid = seq.CurrentTypeId();
        auto tc = seq.CurrentTypeCode();
        auto retreg = cc.newGpReg(tid);

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
        else if (is_float(tc))
        {
            auto atomsize = asmjit::TypeId::sizeOf(tid);

            if (atomsize == 4)
            {
                auto xmm = cc.newXmmSs("value");
                cc.vmovss(xmm, x86::dword_ptr(ctx.cellPtr));
                ctx.ret(xmm);
                return;
            }
            else
            {
                auto xmm = cc.newXmmSd("value");
                cc.vmovsd(xmm, x86::qword_ptr(ctx.cellPtr));
                ctx.ret(xmm);
                return;
            }
        }
        else if (is_atom(tc))
        {
            auto atomsize = asmjit::TypeId::sizeOf(tid);
            cc.mov(retreg, x86::ptr(ctx.cellPtr, 0, atomsize));
        }
        else
        {
            auto plan = walk(seq.ptype);

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

    CONCRETE_MIXIN_DEFINE(BSet)
    {
        print(__FUNCTION__);

        auto tid = seq.CurrentTypeId();
        auto tc = seq.CurrentTypeCode();

        X86Gp val;

        if (!is_float(tc))
        {
            val = cc.newGpReg(tid, "value");
            ctx.addArg(val);
        }

        if (tc == TypeCode::TC_STRING)
        {
            auto call = safecall(cc, tsl_setstring);
            call->setArg(0, ctx.cellAccessor);
            call->setArg(1, ctx.cellPtr);
            call->setArg(2, val);
        }
        else if (tc == TypeCode::TC_U8STRING)
        {
            auto call = safecall(cc, tsl_setu8string);
            call->setArg(0, ctx.cellAccessor);
            call->setArg(1, ctx.cellPtr);
            call->setArg(2, val);
        }
        else if (is_float(tc))
        {
            auto atomsize = asmjit::TypeId::sizeOf(tid);
            if (atomsize == 4)
            {
                auto xmm = cc.newXmmSs("value");
                ctx.addArg(xmm);
                cc.vmovss(x86::dword_ptr(ctx.cellPtr), xmm);
                //cc.movss(x86::ptr(ctx.cellPtr, 0, atomsize), xmm);
            }
            else
            {
                auto xmm = cc.newXmmSd("value");
                ctx.addArg(xmm);
                cc.vmovsd(x86::dword_ptr(ctx.cellPtr), xmm);
                //cc.movsd(x86::ptr(ctx.cellPtr, 0, atomsize), xmm);
            }
        }
        else if (is_atom(tc))
        {
            // simple atom
            auto atomsize = asmjit::TypeId::sizeOf(tid);
            cc.mov(x86::ptr(ctx.cellPtr, 0, atomsize), val);
        }
        else
        {
            auto plan = walk(seq.ptype);
            assert(tid == TypeId::kUIntPtr);

            if (plan.size() == 0) return; // empty struct
            else if (plan.size() == 1 && plan[0] == -1) // jump
            {
                auto src_len_gp = cc.newGpd("src_len");
                auto dst_len_gp = cc.newGpd("dst_len");

                //TODO optional

                // load length headers
                cc.mov(src_len_gp, x86::dword_ptr(val));
                cc.mov(dst_len_gp, x86::dword_ptr(ctx.cellPtr));
                // set length header of dst
                cc.mov(x86::dword_ptr(dst_len_gp), src_len_gp);
                cc.add(ctx.cellPtr, 4);
                cc.add(val, 4);

                auto call = safecall(cc, tsl_assign);
                call->setArg(0, ctx.cellAccessor);
                call->setArg(1, ctx.cellPtr);
                call->setArg(2, val);
                call->setArg(3, dst_len_gp);
                call->setArg(4, src_len_gp);
            }
            else if (plan.size() > 1) // walking
            {
                auto p = push_get_len(cc, "p", val, plan);
                auto q = push_get_len(cc, "q", ctx.cellPtr, plan);
                auto call = safecall(cc, tsl_assign);

                call->setArg(0, ctx.cellAccessor);
                call->setArg(1, ctx.cellPtr);
                call->setArg(2, val);
                call->setArg(3, q.r32());
                call->setArg(4, p.r32());
            }
            else //fixed copying
            {
                auto call = safecall(cc, memcpy);
                call->setArg(0, ctx.cellPtr);
                call->setArg(1, val);
                call->setArg(2, imm(plan[0]));
            }
        }

        ctx.ret();
    }

    CONCRETE_MIXIN_DEFINE(BCmp)
    {
        print(__FUNCTION__);
        X86Gp ret = compare(cc, ctx, seq);
        ctx.ret(ret);
    }

    CONCRETE_MIXIN_DEFINE(BLt)
    {
        print(__FUNCTION__);
        X86Gp cmp = compare(cc, ctx, seq);
        X86Gp ret = cc.newGpd("ret");
        cc.xor_(ret, ret);
        cc.cmp(cmp, ret);

        auto l1 = cc.newLabel();

        cc.jnl(l1);
        cc.mov(ret, imm(1));
        cc.bind(l1);

        ctx.ret(ret);
    }

    CONCRETE_MIXIN_DEFINE(BLe)
    {
        print(__FUNCTION__);
        X86Gp cmp = compare(cc, ctx, seq);
        X86Gp ret = cc.newGpd("ret");
        cc.xor_(ret, ret);
        cc.cmp(cmp, ret);

        auto l1 = cc.newLabel();
        auto l2 = cc.newLabel();

        cc.jnle(l1);
        cc.mov(ret, imm(1));
        cc.bind(l1);

        ctx.ret(ret);
    }

    CONCRETE_MIXIN_DEFINE(BGt)
    {
        print(__FUNCTION__);
        X86Gp cmp = compare(cc, ctx, seq);
        X86Gp ret = cc.newGpd("ret");
        cc.xor_(ret, ret);
        cc.cmp(cmp, ret);

        auto l1 = cc.newLabel();

        cc.jng(l1);
        cc.mov(ret, imm(1));
        cc.bind(l1);

        ctx.ret(ret);
    }

    CONCRETE_MIXIN_DEFINE(BGe)
    {
        print(__FUNCTION__);
        X86Gp cmp = compare(cc, ctx, seq);
        X86Gp ret = cc.newGpd("ret");
        cc.xor_(ret, ret);
        cc.cmp(cmp, ret);

        auto l1 = cc.newLabel();

        cc.jnge(l1);
        cc.mov(ret, imm(1));
        cc.bind(l1);

        ctx.ret(ret);
    }

    CONCRETE_MIXIN_DEFINE(BRefEq)
    {
        print(__FUNCTION__);
        auto comparand = cc.newGpq("comparand");
        auto ret = cc.newGpd("ret");
        ctx.addArg(comparand);
        cc.xor_(ret, ret);
        cc.cmp(ctx.cellPtr, comparand);

        auto l = cc.newLabel();
        cc.jne(l);
        cc.mov(ret, imm(1));
        cc.bind(l);

        ctx.ret(ret);
    }

    CONCRETE_MIXIN_DEFINE(BHash)
    {
        print(__FUNCTION__);
        auto ret = cc.newGpq("ret");
        auto tc = seq.CurrentTypeCode();
        auto tid = seq.CurrentTypeId();

        auto call = safecall(cc, tsl_hash);
        call->setArg(0, ctx.cellPtr);

        if (is_atom(tc)) { call->setArg(1, imm(TypeId::sizeOf(tid))); }
        else { call->setArg(1, calc_size(cc, ctx, seq)); }

        call->setRet(0, ret);
        ctx.ret(ret);
    }

    CONCRETE_MIXIN_DEFINE(BCount)
    {
        print(__FUNCTION__);

        auto tc = seq.CurrentTypeCode();
        X86Gp ret;
        if (tc == TypeCode::TC_LIST)
        {
            JITCALL(LCount);
            return;
        }
        else if (tc == TypeCode::TC_U8STRING)
        {
            ret = calc_size(cc, ctx, seq);
        }
        else if (tc == TypeCode::TC_STRING)
        {
            ret = calc_size(cc, ctx, seq);
            cc.shr(ret, 1);
        }
        else
        {
            // single element
            ret = cc.newGpd();
            cc.mov(ret, imm(1));
        }

        ctx.ret(ret);
    }

    CONCRETE_MIXIN_DEFINE(BSize)
    {
        print(__FUNCTION__);
        auto ret = calc_size(cc, ctx, seq);
        ctx.ret(ret);
    }

    CONCRETE_MIXIN_DEFINE(SGet)
    {
        auto plan = walk(seq.parent, seq.pmember);
        push(cc, ctx.cellPtr, plan);
    }

    CONCRETE_MIXIN_DEFINE(SSet)
    {
        JITCALL(SGet);
        JITCALL(BSet);
    }

    CONCRETE_MIXIN_DEFINE(GSGet)
    {
        //TODO reflection layer for generic interfaces
        //TODO type converter
    }

    CONCRETE_MIXIN_DEFINE(GSSet)
    {

    }

    CONCRETE_MIXIN_DEFINE(LGet)
    {
        auto index = cc.newGpd("index");
        auto l = cc.newLabel();
        ctx.addArg(index);
        // jump over length header
        cc.add(ctx.cellPtr, sizeof(int32_t));

        auto plan = walk(seq.ptype);

        if (plan.size() == 1 && plan[0] > 0)
        {
            //fixed element type
            auto shift = calculate_shift(plan[0]);
            if (shift >= 0)
            {
                cc.lea(ctx.cellPtr, x86::ptr(ctx.cellPtr, index, shift));
            }
            //TODO else if (bits == 2)
            //maybe an opt pass for such things
            else
            {
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

    CONCRETE_MIXIN_DEFINE(LSet)
    {
        JITCALL(LGet);
        JITCALL(BSet);
    }

    CONCRETE_MIXIN_DEFINE(LInlineGet)
    {

    }

    CONCRETE_MIXIN_DEFINE(LInlineSet)
    {

    }

    CONCRETE_MIXIN_DEFINE(LContains)
    {
        print(__FUNCTION__);

        auto plan = walk(seq.ptype);
        auto len = cc.newGpd("list_len");
        auto ret = cc.newGpd("ret");
        auto ltrue = cc.newLabel();
        auto lfalse = cc.newLabel();
        auto lret = cc.newLabel();

        cc.mov(len, x86::dword_ptr(ctx.cellPtr));
        cc.cmp(len, imm(0));
        cc.je(lfalse);

        //if (plan.size() == 1 && plan[0] > 0)
        //{
        //    //fixed element type
        //    //TODO
        //}

        //keep walking
        auto l = cc.newLabel();
        auto pend = cc.newGpq("pend");
        cc.add(ctx.cellPtr, imm(sizeof(int)));
        cc.lea(pend, x86::byte_ptr(ctx.cellPtr, len, /*no shift*/ 0, /*no offset*/ 0));
        cc.bind(l);

        auto comp_result = compare(cc, ctx, seq);
        cc.xor_(ret, ret);
        cc.cmp(comp_result, ret);
        cc.je(ltrue);

        push(cc, ctx.cellPtr, plan);
        cc.cmp(ctx.cellPtr, pend);
        cc.jb(l);

        cc.bind(lfalse);
        cc.xor_(ret, ret);
        cc.jmp(lret);

        cc.bind(ltrue);
        cc.mov(ret, imm(1));

        cc.bind(lret);
        ctx.ret(ret);
    }

    CONCRETE_MIXIN_DEFINE(LCount)
    {
        print(__FUNCTION__);

        auto plan = walk(seq.ptype);
        auto len = cc.newGpd("list_len");
        cc.mov(len, x86::dword_ptr(ctx.cellPtr));

        print("LCount element plan:");
        debug(plan.size());
        for (auto i : plan) debug(i);

        if (plan.size() == 1 && plan[0] > 0)
        {
            //fixed element type
            div32(cc, len, plan[0]);
        }
        else
        {
            //keep walking
            auto l = cc.newLabel();
            auto pend = cc.newGpq("pend");
            cc.add(ctx.cellPtr, imm(sizeof(int)));
            cc.lea(pend, x86::byte_ptr(ctx.cellPtr, len, /*no shift*/ 0, /*no offset*/ 0));
            cc.xor_(len, len);
            cc.bind(l);
            push(cc, ctx.cellPtr, plan);
            cc.inc(len);
            cc.cmp(ctx.cellPtr, pend);
            cc.jb(l);
        }

        ctx.ret(len);
    }

}
