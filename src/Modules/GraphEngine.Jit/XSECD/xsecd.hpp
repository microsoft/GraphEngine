#ifndef __XSECD_HPP__
#define __XSECD_HPP__

#include "licpp/includes/core.hpp"
#include "licpp/includes/base_hacks.hpp"
#include "licpp/includes/hacks.hpp"
#include "licpp/includes/list_utils.hpp"
//#include "TypeSystem.h"

#include <type_traits>
#include <string>
#include <limits>
#include <unordered_map>

namespace xsecd {

  namespace l = licpp;

  using saddr_t = uint32_t;

#ifdef _MSC_VER
  using u16char_t = wchar_t;
#else
  using u16char_t = char16_t;
#endif

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

  template <TypeId>
    struct prim_t {};
  template <>
    struct prim_t<T_NULL> { using type = std::nullptr_t; };
  template <>
    struct prim_t<T_U8> { using type = uint8_t; };
  template <>
    struct prim_t<T_U16> { using type = uint16_t; };
  template <>
    struct prim_t<T_U32> { using type = uint32_t; };
  template <>
    struct prim_t<T_U64> { using type = uint64_t; };
  template <>
    struct prim_t<T_I8> { using type = int8_t; };
  template <>
    struct prim_t<T_I16> { using type = int16_t; };
  template <>
    struct prim_t<T_I32> { using type = int32_t; };
  template <>
    struct prim_t<T_I64> { using type = int64_t; };
  template <>
    struct prim_t<T_F32> { using type = float; };
  template <>
    struct prim_t<T_F64> { using type = double; };
  template <>
    struct prim_t<T_BOOL> { using type = bool; };
  template <>
    struct prim_t<T_CHAR> { using type = u16char_t; };
  template <TypeId i>
    using prim_t_t = typename prim_t<i>::type;

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
    TypeId tid : 8;
    uint32_t type_desc : 24;
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
      u16char_t   _char;
      // S_Addr is the offset on the stack
      saddr_t     _string;
      saddr_t     _u8string;
      saddr_t     _struct;
      saddr_t     _cell;
      struct {
        saddr_t     car;
        saddr_t     cdr;
      } _list;
    };
  };

  template <typename T>
    struct is_prim : std::false_type {};
  template <>
    struct is_prim<std::nullptr_t> : std::true_type {};
  template <>
    struct is_prim<uint8_t> : std::true_type {};
  template <>
    struct is_prim<uint16_t> : std::true_type {};
  template <>
    struct is_prim<uint32_t> : std::true_type {};
  template <>
    struct is_prim<uint64_t> : std::true_type {};
  template <>
    struct is_prim<int8_t> : std::true_type {};
  template <>
    struct is_prim<int16_t> : std::true_type {};
  template <>
    struct is_prim<int32_t> : std::true_type {};
  template <>
    struct is_prim<int64_t> : std::true_type {};
  template <>
    struct is_prim<float> : std::true_type {};
  template <>
    struct is_prim<double> : std::true_type {};
  template <>
    struct is_prim<bool> : std::true_type {};
  template <>
    struct is_prim<char> : std::true_type {};
  template <typename T>
    constexpr bool is_prim_v = is_prim<T>::value;

  std::ostream & operator<< (std::ostream&, XObj32);

  struct XStack {
    saddr_t capacity;
    saddr_t sop; // Top of stack....SOP stands for sopra: [it.] n. top
    XObj32  nil[0]; // Bottom of stack

    void operator>>(XObj32*&);
  };

  XStack * new_stack(size_t);

  saddr_t salloc(XStack *, saddr_t size); // S Alloc, S stands for Stack

  template <typename T>
    saddr_t _xalloc(XStack *, T);
  template <>
    inline saddr_t _xalloc(XStack *, l::nil_t){
      return 0;
    }
  template <typename U, typename = std::enable_if_t<!std::is_same_v<U, std::nullptr_t>>>
    inline saddr_t _xalloc(XStack * stack, l::Cons<saddr_t, U> * lst){
      auto p = stack->sop;
      stack->sop++;
      stack->nil[p].tid = T_LIST;
      stack->nil[p]._list.car = salloc(stack, car(lst));
      stack->nil[p]._list.cdr = _xalloc(stack, cdr(lst));
      return p;
    }
  template <typename T, typename U, typename = std::enable_if_t<!std::is_same_v<U, std::nullptr_t>>>
    inline saddr_t _xalloc(XStack * stack, l::Cons<T, U> * lst){
      auto p = stack->sop;
      stack->sop++;
      stack->nil[p].tid = T_LIST;
      stack->nil[p]._list.car = _xalloc(stack, car(lst));
      stack->nil[p]._list.cdr = _xalloc(stack, cdr(lst));
      return p;
    }

  template <typename T, typename U, typename MListOfSaddr = std::enable_if_t<l::m_list_of_v<saddr_t, l::Cons<T, U> *>>>
  //template <typename T, typename U>
  //  saddr_t xalloc(XStack *, l::Cons<T, U> *); // X Alloc, X stands for eXtension
    saddr_t xalloc(XStack * stack, l::Cons<T, U> * lst){
      auto lsize = list_size(lst);
      if(stack->sop + lsize >= stack->capacity){
        cerr << "Stack Overflow" << endl;
        return 0;
      }
      return _xalloc(stack, lst);
    }

  template <typename T>
    struct _list_size {
      static const saddr_t value = 0;
    };
  template <>
    struct _list_size<l::nil_t> {
      static const saddr_t value = 0;
    };
  template <typename T>
    struct _list_size<l::Cons<T, l::nil_t> *> {
      static const saddr_t value = 1 + _list_size<T>::value;
    };
  template <typename T, typename U>
    struct _list_size<l::Cons<T, U> *> {
      static const saddr_t value = 1 + _list_size<T>::value + _list_size<U>::value;
    };
  template <typename T> // This computes the extra (List-structure) size of a list
    constexpr saddr_t list_size_v = _list_size<T>::value;

  //template <typename T, typename U>
    //unsigned int list_size(l::Cons<T, U> *);

  template <typename T>
    saddr_t __list_size(T);
  template <>
    inline saddr_t __list_size(l::Cons<std::nullptr_t, std::nullptr_t> *){
      return 0;
    }
  template <>
    inline saddr_t __list_size(saddr_t v){
      return v;
    }
  template <typename T, typename U, typename = std::enable_if_t<!std::is_same_v<T, std::nullptr_t>>>
    inline saddr_t __list_size(l::Cons<T, U> * l){
      return __list_size(car(l)) + __list_size(cdr(l));
    }
  template <typename T, typename U,
           typename IsListOfSAddr = std::enable_if_t<l::m_list_of_v<saddr_t, l::Cons<T, U> *>>>
    inline saddr_t list_size(l::Cons<T, U> * lst){
      return __list_size(car(lst)) + __list_size(cdr(lst)) + list_size_v<l::Cons<T, U>*>;
    }

  template <TypeId tid>
    void alloc_prim(XStack *, prim_t_t<tid>);

  void operator<<(XStack *, XObj32&);
  std::ostream & operator<<(std::ostream&, XStack *);

  //struct XTypeDesc {
  //// This is the struct for storing user defined structural types
  //static uint32_t type_id;
  //static std::unordered_map<uint32_t, TypeDescriptor *> types;
  //};



};

#endif
