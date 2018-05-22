#include <iostream>
#include <string>

#include "xsecd.hpp"

using namespace std;

namespace l = licpp;

namespace xsecd {

  saddr_t salloc(XStack * stack, saddr_t size){
    if(stack->sop + size >= stack->capacity){
      cerr << "Stack Overflow" << endl;
      return 0;
    }
    auto res = stack->sop; // return the very first address of the allocated block
    for(saddr_t i = 0; i < size; i++){
      stack->nil[stack->sop].tid = T_NULL;
      stack->nil[stack->sop].type_desc = 0;
      stack->sop ++;
    }
    return res;
  }

  XStack * new_stack(size_t cap){
    auto res = (XStack *) malloc (2 * sizeof(saddr_t) + cap * sizeof(XObj32));
    res->capacity = cap;
    res->nil[0].tid = T_NULL;
    res->sop = 1; // The stack starts from 1, and 0 will always be NIL
    return res;
  }

  void operator<< (XStack * stack, XObj32& o){
    if(stack->sop + 1 >= stack->capacity){
      cerr << "Stack Overflow" << endl;
      // Or should we do auto resize here?
      return;
    }
    memcpy(&(stack->nil[stack->sop]), &o, sizeof(XObj32));
    stack->sop += 1;
  }

  template <TypeId tid>
    void alloc_prim(XStack *, prim_t_t<tid>){}
  template <>
    void alloc_prim<T_NULL>(XStack *, std::nullptr_t){}
  template <>
    void alloc_prim<T_U8>(XStack * stack, uint8_t v){
      XObj32 o;
      o.tid = T_U8;
      o._u8 = v;
      stack << o;
    }
  template <>
    void alloc_prim<T_U16>(XStack * stack, uint16_t v){
      XObj32 o;
      o.tid = T_U16;
      o._u16 = v;
      stack << o;
    }
  template <>
    void alloc_prim<T_U32>(XStack * stack, uint32_t v){
      XObj32 o;
      o.tid = T_U32;
      o._u32 = v;
      stack << o;
    }
  template <>
    void alloc_prim<T_U64>(XStack * stack, uint64_t v){
      XObj32 o;
      o.tid = T_U64;
      o._u64 = v;
      stack << o;
    }
  template <>
    void alloc_prim<T_I8>(XStack * stack, int8_t v){
      XObj32 o;
      o.tid = T_I8;
      o._i8 = v;
      stack << o;
    }
  template <>
    void alloc_prim<T_I16>(XStack * stack, int16_t v){
      XObj32 o;
      o.tid = T_I16;
      o._i16 = v;
      stack << o;
    }
  template <>
    void alloc_prim<T_I32>(XStack * stack, int32_t v){
      XObj32 o;
      o.tid = T_I32;
      o._i32 = v;
      stack << o;
    }
  template <>
    void alloc_prim<T_I64>(XStack * stack, int64_t v){
      XObj32 o;
      o.tid = T_I64;
      o._i64 = v;
      stack << o;
    }
  template <>
    void alloc_prim<T_F32>(XStack * stack, float v){
      XObj32 o;
      o.tid = T_F32;
      o._f32 = v;
      stack << o;
    }
  template <>
    void alloc_prim<T_F64>(XStack * stack, double v){
      XObj32 o;
      o.tid = T_F64;
      o._f64= v;
      stack << o;
    }
  template <>
    void alloc_prim<T_BOOL>(XStack * stack, bool v){
      XObj32 o;
      o.tid = T_BOOL;
      o._bool = v;
      stack << o;
    }
  template <>
    void alloc_prim<T_CHAR>(XStack * stack, u16char_t v){
      XObj32 o;
      o.tid = T_CHAR;
      o._char = v;
      stack << o;
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
    s << "SOP: " << stack->sop << endl;
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
      case T_LIST:
        s << "LIST:\t";
        s << "CAR: " << o._list.car;
        s << " | CDR: " << o._list.cdr;
        break;
      default:
        s << "Non Prim";
        break;
    }
    return s;
  }

};
