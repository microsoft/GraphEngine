#pragma once
#include <unordered_map>
#include <functional>
#include "Common.h"
#include "Verb.h"
#include "VerbSequence.h"
#include "FuncCtx.h"

#define CONCRETE_MIXIN_DECLARE(v) \
    void Dispatch(asmjit::X86Compiler &cc, FuncCtx& ctx, const VerbSequence& seq) override; \
    bool WouldResize(VerbSequence&) override;

#define CONCRETE_MIXIN_DEFINE(v) \
    void v::Dispatch(asmjit::X86Compiler &cc, FuncCtx& ctx, const VerbSequence& seq)

namespace Mixin
{
    struct VerbMixin
    {
        virtual bool GetArgs(const VerbSequence& seq, std::vector<uint8_t> &args) { return false; }
        virtual bool GetRetId(const VerbSequence& seq, OUT TypeId::Id &retId) { return false; }
        virtual void SequenceNext(VerbSequence& seq) {}
        virtual void Dispatch(X86Compiler &cc, FuncCtx& ctx, const VerbSequence& seq) = 0;
        virtual bool WouldResize(VerbSequence&) { return false; }
    };

    TypeId::Id GetReturnId(FunctionDescriptor* fdesc);
    void GetArgument(IN FunctionDescriptor* fdesc, OUT uint8_t* &pargs, OUT int32_t& nargs);
    void DoDispatch(X86Compiler &cc, FuncCtx& ctx, VerbSequence& seq);
    bool WouldResize(FunctionDescriptor* fdesc);
    bool SequenceNext(VerbSequence& seq);
    VerbMixin* Make(VerbCode code);

#pragma region traits
#define HAS(x) \
    class HAS_##x \
    { \
        typedef struct { char onechar[1]; } good; \
        typedef struct { char twochar[2]; } bad; \
        template<typename U, U u> struct Check; \
        struct Fallback { int x; }; \
 \
        template <typename U> static bad&  test(Check<int Fallback::*, &U::x>*); \
        template <typename U> static bad&  test(Check<decltype(&VerbMixin::x), &U::x>*); \
        template <typename U> static good& test(...); \
 \
    public: \
        template <typename T> static constexpr bool value() \
        { \
            return sizeof(test<T>(0)) == sizeof(good); \
        } \
    };

    HAS(GetArgs);
    HAS(GetRetId);
    HAS(Dispatch);
    HAS(SequenceNext);

    template <typename K, typename T> static constexpr bool any()
    {
        return K::template value<T>();
    }

    template <typename K, typename T1, typename ... TS>
    static constexpr typename std::enable_if<(sizeof...(TS) > 0), bool>::type any()
    {
        return any<K, T1>() || any<K, TS...>();
    }
#pragma endregion

#pragma region helper mixins
    template <class B1, class ... BRest>
    struct M : virtual B1, virtual BRest...
    {
    private:
        template <typename K>
        using TSeq = std::conditional<(any<K, B1, BRest...>()), VerbSequence, void*>;
        template <typename K, typename ... T>
        using CSeq = std::conditional<(any<K, T...>()), VerbSequence, void*>;

        template <typename T>
        bool _GetArgs(...)
        {
            return false;
        }
        template <typename T>
        bool _GetArgs(const typename CSeq<HAS_GetArgs, T>::type & seq, std::vector<uint8_t> &args)
        {
            return T::GetArgs(seq, args);
        }
        template <typename T1, typename ... TS>
        typename std::enable_if< (sizeof...(TS) > 0), bool>::type _GetArgs(const typename CSeq<HAS_GetArgs, T1, TS...>::type & seq, std::vector<uint8_t> &args)
        {
            if (_GetArgs<T1>(seq, args)) return true;
            return _GetArgs<TS...>(seq, args);
        }

        template <typename T>
        bool _GetRetId(...)
        {
            return false;
        }
        template <typename T>
        bool _GetRetId(const typename CSeq<HAS_GetRetId, T>::type & seq, TypeId::Id &id)
        {
            return T::GetRetId(seq, id);
        }
        template <typename T1, typename ... TS>
        typename std::enable_if< (sizeof...(TS) > 0), bool>::type _GetRetId(const typename CSeq<HAS_GetRetId, T1, TS...>::type & seq, TypeId::Id &id)
        {
            if (_GetRetId<T1>(seq, id)) return true;
            else return _GetRetId<TS...>(seq, id);
        }

        template <typename T>
        void _SequenceNext(VerbSequence& seq)
        {
            if (HAS_SequenceNext::value<T>()) T::SequenceNext(seq);
        }
        template <typename T1, typename ... TS>
        typename std::enable_if< (sizeof...(TS) > 0), void>::type _SequenceNext(VerbSequence& seq)
        {
            _SequenceNext<T1>(seq);
            _SequenceNext<TS...>(seq);
        }

        //template <typename T>
        //typename void _SequenceNext(...)
        //{
        //}
        //template <typename T>
        //typename void _SequenceNext(typename CSeq<HAS_SequenceNext, T>::type & seq)
        //{
        //    T::SequenceNext(seq);
        //}
        //template <typename T1, typename ... TS>
        //typename std::enable_if< (sizeof...(TS) > 0), void>::type _SequenceNext(typename CSeq<HAS_SequenceNext, T1, TS...>::type& seq)
        //{
        //    _SequenceNext<T1>(seq);
        //    _SequenceNext<TS...>(seq);
        //}

    public:
        //template <typename Q1 = B1, typename QRest = BRest>
        //typename std::enable_if<any<HAS_GetArgs, Q1, QRest...>(), bool >::type GetArgs(const VerbSequence& seq, std::vector<uint8_t> &args)
        typename std::conditional<any<HAS_GetArgs, B1, BRest...>(), bool, void* >::type
            GetArgs(const typename TSeq<HAS_GetArgs>::type &seq, std::vector<uint8_t> &args) 
        {
            print(__FUNCTION__);
            return _GetArgs<B1, BRest...>(seq, args);
        }

        //bool GetRetId(const VerbSequence& seq, OUT TypeId::Id &retId)

        //typename std::conditional<any<HAS_GetRetId, B1, BRest...>(), bool, void* >::type 
        //bool GetRetId(typename const TSeq<HAS_GetRetId>::type &seq, TypeId::Id &retId)


        template <typename Q1 = B1>
        typename std::enable_if<any<HAS_GetRetId, Q1, BRest...>(), bool >::type
            GetRetId(const VerbSequence& seq, TypeId::Id &retId) 

            //typename std::conditional<any<HAS_GetRetId, B1, BRest...>(), bool, void* >::type
            //    GetRetId(typename const TSeq<HAS_GetRetId>::type &seq, TypeId::Id &retId)
        {
            print(__FUNCTION__);
            return _GetRetId<Q1, BRest...>(seq, retId);
            //return _GetRetId<B1, BRest...>(seq, retId);
        }

        //virtual void Dispatch(X86Compiler &cc, FuncCtx& ctx, const VerbSequence& seq) {}

        //template <typename Q1 = B1, typename QRest = BRest>
        //typename std::enable_if<any<HAS_SequenceNext, Q1, QRest...>(), void >::type SequenceNext(VerbSequence& seq)

        typename std::conditional<any<HAS_SequenceNext, B1, BRest...>(), void, void* >::type
            SequenceNext(typename TSeq<HAS_SequenceNext>::type &seq) 

            //void SequenceNext(VerbSequence& seq)
        {
            print(__FUNCTION__);
            _SequenceNext<B1, BRest...>(seq);
        }
    };
    //template <typename B> struct M<B> : virtual B {};

    struct IndexInlined : virtual VerbMixin
    {
        void SequenceNext(VerbSequence& seq) override
        {
            seq.iidx = seq.pcurrent->Data.Index;
        }

        // cancel the index parameter so as to inline
        bool GetArgs(const VerbSequence& seq, std::vector<uint8_t> &args) override
        {
            args.pop_back(); return false;
        }
    };

    template<TypeId::Id tid, bool intercept = true>
    struct Ret : virtual VerbMixin
    {
        bool GetRetId(const VerbSequence& seq, OUT TypeId::Id &retId) override 
        { retId = tid; return intercept; }
    };

    struct NoRet : Ret<TypeId::kVoid> {};

    template <bool intercept = true>
    struct RetT : virtual VerbMixin
    {
        bool GetRetId(const VerbSequence& seq, OUT TypeId::Id &retId) override
        { retId = GetTypeId(seq.ptype); return intercept; }
    };

    template <bool intercept = true>
    struct RetRef : Ret<TypeId::kUIntPtr, intercept> {};

    template <bool intercept = true>
    struct NoArgs : virtual VerbMixin
    {
        // simply interrupt inference
        bool GetArgs(const VerbSequence& seq, std::vector<uint8_t> &args) override 
        { return intercept; }
    };

    template <bool intercept = true>
    struct ArgT : virtual VerbMixin
    {
        bool GetArgs(const VerbSequence& seq, std::vector<uint8_t> &args) override
        { args.push_back(seq.CurrentTypeId()); return intercept; }
    };

    template <TypeId::Id tid, bool intercept = true>
    struct Arg : virtual VerbMixin
    {
        bool GetArgs(const VerbSequence& seq, std::vector<uint8_t> &args) override
        { args.push_back(tid); return intercept; }
    };
#pragma endregion

    struct Basic : virtual VerbMixin {};
    struct BNew : M<Basic, NoArgs<true>>, Ret<TypeId::kI32> { CONCRETE_MIXIN_DECLARE(BNew); };
    struct BGet : M<Basic, NoArgs<true>>, RetT<true> { CONCRETE_MIXIN_DECLARE(BGet); };
    struct BSet : M<Basic, ArgT<true>>, NoRet { CONCRETE_MIXIN_DECLARE(BSet); };
    struct BCmp : M<Basic, ArgT<true>, Ret<TypeId::kI32>> { CONCRETE_MIXIN_DECLARE(BCmp); };
    struct BLt : M<Basic, ArgT<true>, Ret<TypeId::kU32>> { CONCRETE_MIXIN_DECLARE(BLt); };
    struct BLe : M<Basic, ArgT<true>, Ret<TypeId::kU32>> { CONCRETE_MIXIN_DECLARE(BLe); };
    struct BGt : M<Basic, ArgT<true>, Ret<TypeId::kU32>> { CONCRETE_MIXIN_DECLARE(BGt); };
    struct BGe : M<Basic, ArgT<true>, Ret<TypeId::kU32>> { CONCRETE_MIXIN_DECLARE(BGe); };
    struct BRefEq : M<Basic, ArgT<true>, Ret<TypeId::kU32>> { CONCRETE_MIXIN_DECLARE(BRefEq); };
    struct BHash : M<Basic, NoArgs<true>, Ret<TypeId::kU64>> { CONCRETE_MIXIN_DECLARE(BHash); };
    struct BCount : M<Basic, NoArgs<true>, Ret<TypeId::kI32>> { CONCRETE_MIXIN_DECLARE(BCount); };
    struct BSize : M<Basic, NoArgs<true>, Ret<TypeId::kI32>> { CONCRETE_MIXIN_DECLARE(BSize); };

    struct List : Arg<TypeId::kI32, false>
    {
        void SequenceNext(VerbSequence& seq)
        {
            auto vec = seq.ptype->get_ElementType();
            seq.parent = seq.ptype;
            seq.ptype = vec->at(0);
            delete vec;
        }
    };
    struct LGet : M<List, NoArgs<false>>, RetRef<false> { CONCRETE_MIXIN_DECLARE(LGet); };
    struct LSet : M<List, ArgT<true>>, NoRet { CONCRETE_MIXIN_DECLARE(LSet); };
    struct LContains : M<List, ArgT<true>, Ret<TypeId::kU32>> { CONCRETE_MIXIN_DECLARE(LContains); };
    struct LCount : M<List, NoArgs<true>, Ret<TypeId::kI32>> { CONCRETE_MIXIN_DECLARE(LCount); };
    struct LInsertAt : M<List, ArgT<true>, Ret<TypeId::kU32>> { CONCRETE_MIXIN_DECLARE(LInsertAt); };
    struct LRemoveAt : M<List, NoArgs<true>, Ret<TypeId::kU32>> { CONCRETE_MIXIN_DECLARE(LRemoveAt); };
    struct LAppend : M<ArgT<true>, /*Intercepts Arg chain*/ List, Ret<TypeId::kU32>> { CONCRETE_MIXIN_DECLARE(LAppend); };
    struct LConv : M<NoArgs<true>, /*Intercepts Arg chain */ List, Ret<TypeId::kUIntPtr>> { CONCRETE_MIXIN_DECLARE(LConv); };

    struct InlinedList : M<List, IndexInlined> {};
    struct LInlineGet : M<InlinedList, NoArgs<false>, RetRef<false>> { CONCRETE_MIXIN_DECLARE(InlinedLGet); };
    struct LInlineSet : M<InlinedList, ArgT<true>, NoRet> { CONCRETE_MIXIN_DECLARE(InlinedLSet); };

    struct Struct : virtual VerbMixin
    {
        void SequenceNext(VerbSequence& seq)
        {
            auto members = seq.ptype->get_Members();
            auto i = std::find_if(members->begin(), members->end(), [=](auto m) { return !strcmp(m->Name, seq.pcurrent->Data.MemberName); });
            seq.pmember = *i;
            seq.imember = i - members->begin();
            delete members;
            seq.parent = seq.ptype;
            seq.ptype = &seq.pmember->Type;
        }
    };
    struct SGet : M<Struct, NoArgs<false>, RetRef<false>> { CONCRETE_MIXIN_DECLARE(SGet); };
    struct SSet : M<Struct, ArgT<true>, NoRet> { CONCRETE_MIXIN_DECLARE(SSet); };

    struct GenericStruct : Arg<TypeId::kUIntPtr, false>
    {
        void SequenceNext(VerbSequence& seq)
        {
            seq.ptype = seq.pcurrent->Data.GenericTypeArgument;
        }
    };
    struct GSGet : M<GenericStruct, NoArgs<false>, RetRef<false>> { CONCRETE_MIXIN_DECLARE(GSGet); };
    struct GSSet : M<GenericStruct, ArgT<true>, NoRet> { CONCRETE_MIXIN_DECLARE(GSSet); };

    // =========================== static tests ==================================

    struct BBB : Basic
    {
        void Dispatch(X86Compiler& cc, FuncCtx& ctx, const VerbSequence& seq)
        {
            BBB b1;
        }
    };

    struct RRR : RetT<true>
    {
        void Dispatch(X86Compiler& cc, FuncCtx& ctx, const VerbSequence& seq)
        {
            RRR r1;

            static_assert(!HAS_GetArgs::value<RRR>(), "RRR should not have GetArgs");
        }
    };

    struct ILIL : M<List, IndexInlined>
    {
        void Dispatch(X86Compiler& cc, FuncCtx& ctx, const VerbSequence& seq)
        {
            ILIL il;
            static_assert(HAS_SequenceNext::value<ILIL>(), "ILIL should have SequenceNext");
            static_assert(HAS_GetArgs::value<ILIL>(), "ILIL should have GetArgs");
        }
    };

    struct MMM : M<Basic, NoArgs<true>>
    {
        void Dispatch(X86Compiler& cc, FuncCtx& ctx, const VerbSequence& seq)
        {
            MMM m1;

            static_assert(HAS_GetArgs::value<List>(), "List should have GetArgs");
            //static_assert(!HAS_GetRetId::value<List>(), "List should not have GetRetId");

            static_assert(!HAS_GetArgs::value<Basic>(), "Basic should not have GetArgs");
            static_assert(HAS_GetArgs::value<NoArgs>(), "NoArgs should have GetArgs");
            static_assert(any<HAS_GetArgs, Basic, NoArgs>(), "should pass any<Has_GetArgs, Basic, NoArgs>");

            static_assert(!HAS_GetArgs::value<Ret<TypeId::kVoid>>(), "Ret<> should not have GetArgs");
            static_assert(!HAS_GetArgs::value<RetT<>>(), "RetT<> should not have GetArgs");
            static_assert(!any<HAS_GetArgs, Ret<TypeId::kVoid>, RetT<>>(), "should not pass any<HAS_GetArgs, Ret<TypeId::kVoid>, RetT<>>");

            static_assert(!HAS_GetRetId::value<Basic>(), "Basic should have GetArgs");
            static_assert(!HAS_GetRetId::value<NoArgs>(), "NoArgs should have GetArgs");
            static_assert(!any<HAS_GetRetId, Basic, NoArgs>(), "should not pass any<Has_GetRetId, Basic, NoArgs>");

            static_assert(HAS_GetRetId::value<Ret<TypeId::kVoid>>(), "Ret<> should have GetRetId");
            static_assert(HAS_GetRetId::value<RetT<>>(), "RetT<> should have GetRetId");
            static_assert(any<HAS_GetRetId, Ret<TypeId::kVoid>, RetT<>>(), "should pass any<HAS_GetRetId, Ret<TypeId::kVoid>, RetT<>>");

            static_assert(std::is_same<std::conditional<any<HAS_GetRetId, Basic, NoArgs>(), bool, void* >::type, void*>::value, "conditional type branch");
            //static_assert(std::is_same<std::conditional<any<HAS_GetRetId, MMM>(), bool, void* >::type, void*>::value, "conditional type branch");

            static_assert(HAS_GetArgs::value<MMM>(), "MMM should have GetArgs");
            //static_assert(!HAS_GetRetId::value<MMM>(), "MMM should not have GetRetId");
        }
    };
}
