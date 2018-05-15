#include <iostream>
#include <string>

#include "xsecd.hpp"

using namespace std;

namespace xsecd {

    void operator<< (XStack * stack, XObj32& o){
        // Check overflow
        if(stack->sop + 1 >= stack->capacity){
            cerr << "Stack Overflow" << endl;
            // Or should we do auto resize here?
        }
        switch(type_cat(o.tid)){
            case LST:

                break;
            case DYN:

                break;
            case PRIM:
                memcpy(&(stack->nil[stack->sop]), &o, sizeof(XObj32));
                stack->sop += 1;
                break;
        }
    }

    void XStack::operator>> (XObj32*& o){
        // Dummy
        (void)o;
    }

    ostream& operator<< (ostream& s, XStack * stack){
        s << "[" << endl;
        for(saddr_t i = 0; i < stack->sop; ++i){
            s << "  " << i << "\t| " << stack->nil[i];
            s << endl;
        }
        s << "]" << endl;
        return s;
    }

    ostream& operator<< (ostream& s, XObj32 o){
        switch(o.tid){
            case T_NULL:
                s << "NIL";
                break;
            case T_U8:
                s << "U8:\t " << o._u8;
                break;
            case T_U16:
                s << "U16:\t " << o._u16;
                break;
            case T_U32:
                s << "U32:\t " << o._u32;
                break;
            case T_U64:
                s << "U64:\t " << o._u64;
                break;
            case T_I8:
                s << "I8:\t " << o._i8;
                break;
            case T_I16:
                s << "I16:\t " << o._i16;
                break;
            case T_I32:
                s << "I32:\t " << o._i32;
                break;
            case T_I64:
                s << "I64:\t " << o._i64;
                break;
            case T_F32:
                s << "F32:\t " << o._f32;
                break;
            case T_F64:
                s << "F64:\t " << o._f64;
                break;
            case T_BOOL:
                s << "BOOL:\t " << (o._bool ? "#t" : "#f");
                break;
            case T_CHAR:
                s << "CHAR:\t #\\" << o._char;
                break;
            default:
                s << "Non Prim";
                break;
        }
        return s;
    }

};
