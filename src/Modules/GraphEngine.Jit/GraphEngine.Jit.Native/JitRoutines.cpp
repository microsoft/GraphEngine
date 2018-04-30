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
                cc.movsxd(tmp, x86::dword_ptr(reg));
                cc.lea(reg, x86::byte_ptr(reg, tmp, /*shift*/0, /*offset*/sizeof(int32_t)));
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
    auto is_float32(TypeCode code) { return code == TypeCode::TC_F32; }
    auto is_float64(TypeCode code) { return code == TypeCode::TC_F64; }
    auto is_float(TypeCode code) { return is_float32(code) || is_float64(code); }
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
    auto is_fixed(const std::vector<int32_t>& plan) { return plan.size() == 1 && plan[0] >= 0; }
    auto is_fixed(TypeDescriptor* t)
    {
        auto tc = (TypeCode)t->get_TypeCode();
        if (is_atom(tc))return true;
        auto plan = walk(t);
        return plan.size() == 1 && plan[0] >= 0;
    }

    Reg new_reg(X86Compiler& cc, TypeCode tc, TypeId::Id tid)
    {
        if (is_float32(tc))
        {
            return cc.newXmmSs("f32_reg");
        }
        else if (is_float64(tc))
        {
            return cc.newXmmSd("f64_reg");
        }
        else if (is_atom(tc))
        {
            return cc.newGpReg(tid, "atom_reg");
        }
        else
        {
            return cc.newGpq("ptr_reg");
        }
    }

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

    X86Gp push_get_len(X86Compiler& cc, const char* name, const X86Gp& base, std::vector<int32_t>& plan)
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

        auto arg = new_reg(cc, tc, tid);
        ctx.addArg(arg);
        X86Gp retreg;

        if (is_tslhead(tc))
        {
            retreg = compare_block(cc, ctx.cellPtr, arg.as<X86Gp>());
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
                cc.cmp(x86::ptr(ctx.cellPtr, 0, atomsize), arg.as<X86Gp>());
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
                cc.cmp(x86::ptr(ctx.cellPtr, 0, atomsize), arg.as<X86Gp>());
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

                if (atomsize == 4) cc.ucomiss(arg.as<X86Xmm>(), x86::ptr(ctx.cellPtr, 0, atomsize));
                else cc.ucomisd(arg.as<X86Xmm>(), x86::ptr(ctx.cellPtr, 0, atomsize));

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
                retreg = compare_block(cc, ctx.cellPtr, arg.as<X86Gp>());
            }
            else if (plan.size() > 1) // walking
            {
                auto p = push_get_len(cc, "p", arg.as<X86Gp>(), plan);
                auto q = push_get_len(cc, "q", ctx.cellPtr, plan);

                retreg = compare_block<false>(cc, ctx.cellPtr, arg.as<X86Gp>(), q, p);
            }
            else //fixed 
            {
                retreg = cc.newGpReg(TypeId::kI32, "cmpresult");
                auto call = safecall(cc, memcmp);
                auto len1 = imm(plan[0]);
                call->setArg(0, ctx.cellPtr);
                call->setArg(1, arg.as<X86Gp>());
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

    void seek(X86Compiler& cc, FuncCtx& ctx, Label& l_outofrange, Label& l_resize, X86Gp& idx, X86Gp& ptr, TypeCode tc, TypeId::Id tid, std::vector<int32_t> &plan)
    {
        if (is_atom(tc))
        {
            auto atom_size = TypeId::sizeOf(tid);
            auto shift = calculate_shift(atom_size);
            cc.lea(ptr, x86::ptr(0, idx, shift));
            cc.cmp(ptr.r32(), x86::dword_ptr(ctx.cellPtr));
            cc.jg(l_outofrange);
            cc.lea(ptr, x86::ptr(ctx.cellPtr, ptr.r32(), /*shift*/0, /*offset*/sizeof(int32_t), /*size*/1));
        }
        else if (is_fixed(plan))
        {
            auto shift = calculate_shift(plan[0]);
            if (shift >= 0)
            {
                cc.lea(ctx.cellPtr, x86::ptr(ctx.cellPtr, idx, shift));
            }
            else
            {
                cc.imul(idx, imm(plan[0]));
                cc.lea(ctx.cellPtr, x86::ptr(idx, /*offset*/ sizeof(int32_t)));
            }
        }
        else
        {
            auto l_seek = cc.newLabel();
            auto pend = cc.newGpq("pend");
            auto cnt = cc.newGpd("cnt");
            cc.movsxd(pend, x86::dword_ptr(ctx.cellPtr));
            cc.lea(pend, x86::byte_ptr(ctx.cellPtr, pend, /*shift*/0, /*offset*/ sizeof(int32_t)));
            cc.lea(ptr, x86::byte_ptr(ctx.cellPtr, /* offset */sizeof(int32_t)));

            cc.bind(l_seek);
            cc.cmp(ptr, pend);
            cc.jg(l_outofrange);

            cc.lea(cnt, x86::ptr(idx, -1, 1));
            cc.test(idx, idx);
            cc.mov(idx, cnt);
            cc.jz(l_resize);

            push(cc, ptr, plan);
            cc.jmp(l_seek);
        }
    }

    enum ResizeOp
    {
        RO_INSERT,
        RO_REMOVE,
        RO_REPLACE
    };

    //  elen: output target, could be delta or target len.
    //  if target is tsl-headed and the op is RO_REPLACE,
    //  target pointer ("to" register) will be advanced
    //  to preserve length header and thus resize chain
    //  adjustment. after resize, we can thus directly
    //  call assign on the content, without the header.
    //
    //  Cases for RO_INSERT:
    //    1. String with pre-calculated len in *elen*
    //       Resize request is to insert elen+sizeof(int32_t)
    //       == AFTER RESIZE, AFTER RESIZE CHAIN ADJUST ==
    //       Restore elen.
    //       Set target length (register "to") , Advance to; 
    //       Do not advance from, length preserved in elen
    //    2. TSL-headed data and dynamic structures
    //       == BEFORE RESIZE ==
    //       Do not advance. Store pushed target length in *elen* 
    //       for resize/assign.
    //    3. Fixed-length element
    //       Just call tsl_resize.
    //
    //  Cases for RO_REPLACE:
    //    1. String with pre-calculated len in pstrlen
    //       Advance to, do not advance from, length preserved in pstrlen
    //    2. TSL-headed data and dynamic structures
    //       Do not advance. Store pushed target length in *tlen* for assign.
    //       TODO:
    //       Note, target length covers the header. We can thus pop elements
    //       from resize chain if there are any (RO_REPLACE only). 
    //       Currently unimplemented.
    void resize(X86Compiler& cc, FuncCtx& ctx,
                Reg& from, const X86Gp& to, OUT X86Gp& elen,
                TypeCode tc, TypeId::Id tid, std::vector<int32_t> &plan,
                ResizeOp op, X86Gp* pstrlen = nullptr, X86Mem* tlen = nullptr)
    {
        auto fixed = (is_atom(tc) || is_fixed(plan));

        if (is_atom(tc) || is_fixed(plan))
        {
            Imm i;
            int32_t s = is_atom(tc) ? TypeId::sizeOf(tid) : plan[0];
            if (op == RO_INSERT) i = imm(s);
            else if (op == RO_REMOVE) i = imm(-s);
            else { print("do not call replace resize on fixed type"); throw; }

            elen = cc.newGpd("atom_len");
            cc.mov(elen, i);
        }
        else if (op == RO_INSERT)
        {
            if (tc != TypeCode::TC_STRING && tc != TypeCode::TC_U8STRING)
            {
                elen = push_get_len(cc, "elen", from.as<X86Gp>(), plan);
            }
            else
            {
                // string length pre-calculated in elen r32
                cc.add(elen, imm(sizeof(int32_t)));
            }
        }
        else if (op == RO_REMOVE)
        {
            elen = push_get_len(cc, "elen", to, plan);
            cc.neg(elen);
        }
        else if (op == RO_REPLACE)
        {
            if (pstrlen != nullptr) { elen = cc.newGpd(); cc.mov(elen, pstrlen->r32()); cc.add(elen, imm(sizeof(int32_t))); }
            else elen = push_get_len(cc, "src_push", from.as<X86Gp>(), plan);

            if (tlen != nullptr) cc.mov(*tlen, elen.r32());

            auto tmp = push_get_len(cc, "dst_push", to, plan);
            cc.sub(elen.r32(), tmp.r32());

            if (pstrlen != nullptr) { cc.add(to, imm(sizeof(int32_t))); }
        }

        auto resize_call = safecall(cc, tsl_resize);
        resize_call->setRet(0, to);
        resize_call->setArg(0, ctx.cellAccessor);
        resize_call->setArg(1, to);
        resize_call->setArg(2, elen.r32());

        auto bp = cc.newGpq("cell-base");
        cc.mov(bp, x86::qword_ptr(ctx.cellAccessor));

        std::for_each(ctx.resizeChain.rbegin(), ctx.resizeChain.rend(), [&](auto &r) {
            cc.add(x86::dword_ptr(bp, r), elen.r32());
        });

        if (op == RO_INSERT && (tc == TypeCode::TC_STRING || tc == TypeCode::TC_U8STRING))
        {
            cc.sub(elen, imm(sizeof(int32_t)));
            cc.mov(x86::dword_ptr(to), elen.r32());
            cc.add(to, imm(sizeof(int32_t)));
        }
    }

    void my_memcpy(void* dst, void* src, size_t len)
    {
        print(__FUNCTION__);

        debug((int64_t)dst);
        debug((char*)src);
        debug(len);

        wprintf(L"==============\n");

        for (int i=-4; i < (int32_t)len; ++i)
        {
            wprintf(L"%2X  src=%2X  dst=%2X\n", i, ((uint8_t*)src)[i], ((uint8_t*)dst)[i]);
        }
        wprintf(L"==============\n");

        memcpy(dst, src, len);

        for (int i=-4; i < (int32_t)len; ++i)
        {
            wprintf(L"%2X  src=%2X  dst=%2X\n", i, ((uint8_t*)src)[i], ((uint8_t*)dst)[i]);
        }
        wprintf(L"==============\n");
    }

    void assign(X86Compiler& cc, const Reg& from, const X86Gp& to, const X86Gp& size, TypeCode tc, TypeId::Id tid)
    {
        if (is_float32(tc))
        {
            cc.vmovss(x86::dword_ptr(to), from.as<X86Xmm>());
        }
        else if (is_float64(tc))
        {
            cc.vmovsd(x86::qword_ptr(to), from.as<X86Xmm>());
        }
        else if (is_atom(tc))
        {
            cc.mov(x86::ptr(to, 0, TypeId::sizeOf(tid)), from.as<X86Gp>());
        }
        else
        {
            auto assign_call = safecall(cc, my_memcpy);
            assign_call->setArg(0, to);
            assign_call->setArg(1, from);
            assign_call->setArg(2, size);
        }
    }

    CONCRETE_MIXIN_DEFINE(BNew)
    {
        print(__FUNCTION__);

        auto retreg = cc.newGpd("err");
        auto plan = walk(seq.ptype);
        auto len = std::accumulate(plan.begin(), plan.end(), 0, [](int32_t cum, int32_t p) { return cum + (p >= 0 ? p : sizeof(int32_t)); });
        auto tc = seq.CurrentTypeCode();

        auto call = safecall(cc, tsl_newaccessor);
        call->setRet(0, retreg);
        call->setArg(0, ctx.cellAccessor);
        call->setArg(1, imm(len));

        if (tc == TypeCode::TC_CELL)
        {
            call->setArg(2, imm_u(seq.ptype->get_CellType()));
            call->setArg(3, imm_u(1));
        }
        else
        {
            call->setArg(2, imm_u(0));
            call->setArg(3, imm_u(0));
        }

        ctx.ret(retreg);
    }

    CONCRETE_MIXIN_DEFINE(BGet)
    {
        print(__FUNCTION__);

        auto tid = seq.CurrentTypeId();
        auto tc = seq.CurrentTypeCode();
        auto retreg = new_reg(cc, tc, tid);

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
            auto xmm = retreg.as<X86Xmm>();

            if (atomsize == 4) { cc.vmovss(xmm, x86::dword_ptr(ctx.cellPtr)); }
            else { cc.vmovsd(xmm, x86::qword_ptr(ctx.cellPtr)); }

            ctx.ret(xmm);
            return;
        }
        else if (is_atom(tc))
        {
            auto atomsize = asmjit::TypeId::sizeOf(tid);
            cc.mov(retreg.as<X86Gp>(), x86::ptr(ctx.cellPtr, 0, atomsize));
        }
        else
        {
            auto plan = walk(seq.ptype);

            if (plan.size() == 0) return; // empty struct
            else if (plan.size() == 1 && plan[0] == -1) // jump
            {
                auto call = safecall(cc, tsl_dup_dynamic);
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
                auto call = safecall(cc, tsl_dup);
                call->setArg(0, ctx.cellPtr);
                call->setArg(1, p.as<X86Gpd>());
                call->setRet(0, retreg);
            }
            else //fixed copying
            {
                auto call = safecall(cc, tsl_dup);
                call->setArg(0, ctx.cellPtr);
                call->setArg(1, imm(plan[0]));
                call->setRet(0, retreg);
            }
        }

        ctx.ret(retreg.as<X86Gp>());
    }

    CONCRETE_MIXIN_DEFINE(BSet)
    {
        print(__FUNCTION__);

        auto tid = seq.CurrentTypeId();
        auto tc = seq.CurrentTypeCode();
        auto val = new_reg(cc, tc, tid);
        auto plan = walk(seq.ptype);
        X86Gp delta;

        ctx.addArg(val);

        if (tc == TypeCode::TC_STRING)
        {
            ctx.pushResizeChain();
            X86Gp r_tmp = cc.newGpq("r_tmp");
            auto call = safecall(cc, wcslen);
            call->setArg(0, val);
            call->setRet(0, r_tmp);
            cc.shl(r_tmp, 1);

            resize(cc, ctx, val, ctx.cellPtr, delta, tc, tid, plan, RO_REPLACE, &r_tmp);

            assign(cc, val, ctx.cellPtr, r_tmp, tc, tid);
        }
        else if (tc == TypeCode::TC_U8STRING)
        {
            ctx.pushResizeChain();

            X86Gp r_tmp = cc.newGpq("r_tmp");
            X86Mem stack_l = cc.newStack(sizeof(int32_t), sizeof(int32_t), "str_len");
            X86Gp p = cc.newGpq("str_p");

            cc.lea(p, stack_l);
            auto call = safecall(cc, tsl_utf16_utf8);
            call->setRet(0, p);
            call->setArg(0, val);
            call->setArg(1, p);

            cc.movsxd(r_tmp, stack_l);
            resize(cc, ctx, p, ctx.cellPtr, delta, tc, tid, plan, RO_REPLACE, &r_tmp);

            assign(cc, p, ctx.cellPtr, r_tmp, tc, tid);

            call = safecall(cc, free);
            call->setArg(0, p);
        }
        else if (is_atom(tc) || is_fixed(plan))
        {
            X86Gp _;
            assign(cc, val, ctx.cellPtr, _, tc, tid);
        }
        else
        {
            if (plan.size() == 0) return; // empty struct
            ctx.pushResizeChain();
            X86Mem tlen = cc.newStack(sizeof(int32_t), sizeof(int32_t), "tlen");
            resize(cc, ctx, val, ctx.cellPtr, delta, tc, tid, plan, RO_REPLACE, nullptr, &tlen);
            cc.mov(delta, tlen);
            assign(cc, val, ctx.cellPtr, delta, tc, tid);
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
        ctx.addArg(index);
        auto plan = walk(seq.ptype);
        auto tc = seq.CurrentTypeCode();
        auto tid = seq.CurrentTypeId();
        auto p = cc.newGpq("walk_ptr");

        auto l_outofrange = cc.newLabel();
        auto l_break = cc.newLabel();
        auto l_begin = cc.newLabel();

        cc.jmp(l_begin);
        cc.bind(l_outofrange);
        //  return null
        cc.mov(index, imm(0));
        cc.ret(index);

        cc.bind(l_begin);
        seek(cc, ctx, l_outofrange, l_break, index, p, tc, tid, plan);

        cc.bind(l_break);
        cc.mov(ctx.cellPtr, p);
    }

    CONCRETE_MIXIN_DEFINE(LSet)
    {
        ctx.pushResizeChain();
        JITCALL(LGet);
        JITCALL(BSet);
    }

    CONCRETE_MIXIN_DEFINE(LInlineGet)
    {

    }

    CONCRETE_MIXIN_DEFINE(LInlineSet)
    {
        ctx.pushResizeChain();
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

        cc.cmp(ctx.cellPtr, pend);
        cc.jge(lfalse);

        auto comp_result = compare(cc, ctx, seq);
        cc.xor_(ret, ret);
        cc.cmp(comp_result, ret);
        cc.je(ltrue);

        push(cc, ctx.cellPtr, plan);
        cc.jmp(l);

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

    CONCRETE_MIXIN_DEFINE(LInsertAt)
    {
        print(__FUNCTION__);
        ctx.pushResizeChain();

        auto plan = walk(seq.ptype);
        auto tc = seq.CurrentTypeCode();
        auto tid = seq.CurrentTypeId();

        auto idx = cc.newGpd("idx");
        auto element = new_reg(cc, tc, tid);
        auto p = cc.newGpq("walk_ptr");
        ctx.addArg(idx);
        ctx.addArg(element);

        X86Gp elen;

        //  labels

        auto l_outofrange = cc.newLabel();
        auto l_resize     = cc.newLabel();
        auto l_ret        = cc.newLabel();

        //  1. seek insertion point
        seek(cc, ctx, l_outofrange, l_resize, idx, p, tc, tid, plan);

        cc.bind(l_resize);
        cc.mov(ctx.cellPtr, p);

        if (tc == TypeCode::TC_STRING)
        {
            elen = cc.newGpq("str_len");
            auto call = safecall(cc, wcslen);
            call->setRet(0, elen);
            call->setArg(0, element);
            cc.shl(elen, 1);

            resize(cc, ctx, element, ctx.cellPtr, elen, tc, tid, plan, RO_INSERT);
            // header set, ctx.cellPtr advanced.
            assign(cc, element, ctx.cellPtr, elen, tc, tid);
        }
        else if (tc == TypeCode::TC_U8STRING)
        {
            elen = cc.newGpq("str_len");
            X86Mem stack_l = cc.newStack(sizeof(int32_t), sizeof(int32_t), "mem_strlen");
            X86Gp p = cc.newGpq("str_p");

            cc.lea(p, stack_l);
            auto call = safecall(cc, tsl_utf16_utf8);
            call->setRet(0, p);
            call->setArg(0, element);
            call->setArg(1, p);

            cc.movsxd(elen, stack_l);
            resize(cc, ctx, p, ctx.cellPtr, elen, tc, tid, plan, RO_INSERT);

            assign(cc, p, ctx.cellPtr, elen, tc, tid);

            call = safecall(cc, free);
            call->setArg(0, p);
        }
        else
        {
            resize(cc, ctx, element, ctx.cellPtr, elen, tc, tid, plan, RO_INSERT);

            assign(cc, element, ctx.cellPtr, elen, tc, tid);
        }

        auto ret = cc.newGpd("ret");
        cc.mov(ret, imm(1));
        cc.bind(l_ret);
        ctx.ret(ret);

        cc.bind(l_outofrange);
        cc.mov(ret, imm(0));
        cc.jmp(l_ret);
    }

    CONCRETE_MIXIN_DEFINE(LAppend)
    {
        print(__FUNCTION__);
        ctx.pushResizeChain();

        auto plan = walk(seq.ptype);
        auto tc = seq.CurrentTypeCode();
        auto tid = seq.CurrentTypeId();

        auto len = cc.newGpd("len");
        auto element = new_reg(cc, tc, tid);
        ctx.addArg(element);

        X86Gp elen;

        //  1. seek insertion point
        cc.mov(len, x86::dword_ptr(ctx.cellPtr));
        cc.lea(ctx.cellPtr, x86::byte_ptr(ctx.cellPtr, len, /*shift*/0, sizeof(int32_t)));

        if (tc == TypeCode::TC_STRING)
        {
            elen = cc.newGpq("str_len");
            auto call = safecall(cc, wcslen);
            call->setRet(0, elen);
            call->setArg(0, element);
            cc.shl(elen, 1);

            resize(cc, ctx, element, ctx.cellPtr, elen, tc, tid, plan, RO_INSERT);
            // header set, ctx.cellPtr advanced.
            assign(cc, element, ctx.cellPtr, elen, tc, tid);
        }
        else if (tc == TypeCode::TC_U8STRING)
        {
            elen = cc.newGpq("str_len");
            X86Mem stack_l = cc.newStack(sizeof(int32_t), sizeof(int32_t), "mem_strlen");
            X86Gp p = cc.newGpq("str_p");

            cc.lea(p, stack_l);
            auto call = safecall(cc, tsl_utf16_utf8);
            call->setRet(0, p);
            call->setArg(0, element);
            call->setArg(1, p);

            cc.movsxd(elen, stack_l);
            resize(cc, ctx, p, ctx.cellPtr, elen, tc, tid, plan, RO_INSERT);

            assign(cc, p, ctx.cellPtr, elen, tc, tid);

            call = safecall(cc, free);
            call->setArg(0, p);
        }
        else
        {
            resize(cc, ctx, element, ctx.cellPtr, elen, tc, tid, plan, RO_INSERT);

            assign(cc, element, ctx.cellPtr, elen, tc, tid);
        }

        ctx.ret();
    }

    CONCRETE_MIXIN_DEFINE(LRemoveAt)
    {
        print(__FUNCTION__);
        ctx.pushResizeChain();

        auto plan = walk(seq.ptype);
        auto tc = seq.CurrentTypeCode();
        auto tid = seq.CurrentTypeId();
        auto offset = cc.newGpq("offset");

        auto idx = cc.newGpd("idx");
        auto ret = cc.newGpd("ret");
        ctx.addArg(idx);

        X86Gp elen;

        //  labels

        auto l_outofrange = cc.newLabel();
        auto l_resize     = cc.newLabel();
        auto l_ret        = cc.newLabel();

        //  1. seek insertion point
        seek(cc, ctx, l_outofrange, l_resize, idx, offset, tc, tid, plan);

        cc.bind(l_resize);

        //  2. obtain length of the element, resize
        X86Gp _;
        resize(cc, ctx, _, offset, elen, tc, tid, plan, RO_REMOVE);

        cc.mov(ret, imm(1));
        cc.bind(l_ret);
        ctx.ret(ret);

        cc.bind(l_outofrange);
        cc.mov(ret, imm(0));
        cc.jmp(l_ret);
    }

    bool BNew::WouldResize(VerbSequence&) { return false; }
    bool BGet::WouldResize(VerbSequence&) { return false; }
    bool BSet::WouldResize(VerbSequence& seq) { return !is_fixed(seq.ptype); }
    bool BCmp::WouldResize(VerbSequence&) { return false; }
    bool BLe::WouldResize(VerbSequence&) { return false; }
    bool BLt::WouldResize(VerbSequence&) { return false; }
    bool BGe::WouldResize(VerbSequence&) { return false; }
    bool BGt::WouldResize(VerbSequence&) { return false; }
    bool BRefEq::WouldResize(VerbSequence&) { return false; }

    bool LGet::WouldResize(VerbSequence&) { return false; }
    bool LSet::WouldResize(VerbSequence& seq) { return !is_fixed(seq.ptype); }
    bool LContains::WouldResize(VerbSequence&) { return false; }
    bool LCount::WouldResize(VerbSequence&) { return false; }
    bool LInsertAt::WouldResize(VerbSequence&) { return true; }
    bool LAppend::WouldResize(VerbSequence&) { return true; }
    bool LRemoveAt::WouldResize(VerbSequence&) { return true; }

    bool SGet::WouldResize(VerbSequence&) { return false; }
    bool SSet::WouldResize(VerbSequence& seq) { return !is_fixed(seq.ptype); }

}
