#ifndef __XSECD_HPP__
#define __XSECD_HPP__

//#include "licpp/includes/core.hpp"
//#include "licpp/includes/list_utils.hpp"
//#include "licpp/includes/hacks.hpp"

#include <string>

//namespace l = licpp;

namespace xsecd {
    using saddr_t = uint32_t;

    const size_t default_stack_size = 1024;

    enum TypeId : int16_t {
        T_NULL
            , T_U8, T_U16, T_U32, T_U64
            , T_I8, T_I16, T_I32, T_I64
            , T_F32, T_F64
            , T_BOOL
            , T_CHAR, T_STRING, T_U8STRING
            , T_LIST
            , T_STRUCT, T_CELL
    };

    enum TypeCat {
        PRIM, DYN, LST
    };

    constexpr TypeCat type_cat(TypeId t){
        switch(t){
            case T_LIST:
                return LST;
            case T_STRING:
            case T_U8STRING:
            case T_STRUCT:
            case T_CELL:
                return DYN;
            default:
                return PRIM;
        }
    }

    struct XObj32 {
        TypeId tid;
        union {
            uint8_t     _u8;
            uint16_t    _u16;
            uint32_t    _u32;
            uint64_t    _u64;
            int8_t      _i8;
            int16_t     _i16;
            int32_t     _i32;
            int64_t     _i64;
            float       _f32;
            double      _f64;
            bool        _bool;
            char        _char;
            // S_Addr is the offset on the stack
            saddr_t     _string;
            saddr_t     _u8string;
            saddr_t     _struct;
            saddr_t     _cell;
            saddr_t     _list;
        };
    };

    std::ostream & operator<< (std::ostream&, XObj32);

    struct XStack {
        saddr_t capacity;
        saddr_t sop; // Top of stack....SOP stands for sopra: [it.] n. top
        XObj32  nil[0]; // Bottom of stack

        void operator>>(XObj32*&);
    };

    void operator<<(XStack *, XObj32&);
    std::ostream & operator<<(std::ostream&, XStack *);

};

#endif
