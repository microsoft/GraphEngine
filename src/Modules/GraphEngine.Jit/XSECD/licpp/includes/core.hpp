/*
 * This file is a part of the LICPP project,
 * licensed under MIT liense.
 * Please read LICENSE.TXT and README.MKD for more information.
 */

#ifdef _MSC_VER
#if _MSC_VER < 1900
#error At least Visual Studio 2015 is required
#endif
#else
#if __cplusplus <= 201103L
#error At least C++ 14 is required
#endif
#endif // _MSC_VER

#ifndef __LICPP_CORE_HPP__
#define __LICPP_CORE_HPP__

#include <cstdlib>
#include <vector>
#include <functional>
#include <string>
#include <sstream>
#include <type_traits>

using std::nullptr_t;

namespace licpp {
#define caar(X) car(car(X))
#define cadr(X) car(cdr(X))
#define cdar(X) cdr(car(X))
#define caaar(X) car(car(car(X)))
#define caadr(X) car(car(cdr(X)))
#define cadar(X) car(cdr(car(X)))
#define cdaar(X) cdr(car(car(X)))
#define caddr(X) car(cdr(cdr(X)))
#define cdadr(X) cdr(car(cdr(X)))
#define cddar(X) cdr(cdr(car(X)))
#define caaaar(X) car(car(car(car(X))))
#define caaadr(X) car(car(car(cdr(X))))
#define caadar(X) car(car(cdr(car(X))))
#define cadaar(X) car(cdr(car(car(X))))
#define cdaaar(X) cdr(car(car(car(X))))
#define caaddr(X) car(car(cdr(cdr(X))))
#define cadadr(X) car(cdr(car(cdr(X))))
#define cdaadr(X) cdr(car(car(cdr(X))))
#define caddar(X) car(cdr(cdr(car(X))))
#define cdadar(X) cdr(car(cdr(car(X))))
#define cddaar(X) cdr(cdr(car(car(X))))
#define cadddr(X) car(cdr(cdr(cdr(X))))
#define cdaddr(X) cdr(car(cdr(cdr(X))))
#define cddadr(X) cdr(cdr(car(cdr(X))))
#define cdddar(X) cdr(cdr(cdr(car(X))))
#define cddddr(X) cdr(cdr(cdr(cdr(X))))

#define var auto
  // A 2-Element Pair Structure That Holds Arbitary Types
  template <typename T, typename U> class Cons;
  using nil_t = Cons<nullptr_t, nullptr_t> *;
  static const nil_t nil = nullptr;
  template <typename T, typename U>
    class Cons {
      private:
        T _car;
        U _cdr;

      public:
        Cons(T car) :_car(car), _cdr(nil) {}
        Cons(T car, U cdr) :_car(car), _cdr(cdr) {}

        T car() const {
          return _car;
        }
        U cdr() const {
          return _cdr;
        }
        Cons<T, U> * set_car(T car) {
          _car = car;
          return this;
        }
        Cons<T, U> * set_cdr(U cdr) {
          _cdr = cdr;
          return this;
        }
    };

  template<typename T, typename U>
    inline auto cons(T car, U cdr) {
      return new Cons<T, U>(car, cdr);
    }

  template <typename T>
    inline auto list(T car) {
      return cons(car, nil);
    }
  template <typename T, typename U>
    inline auto list(T car, U cadr) {
      return cons(car, cons(cadr, nil));
    }
  template<typename T, typename... Us>
    inline auto list(T car, Us... rest) {
      auto tmp = list(rest...);
      return cons(car, tmp);
    }

  template<typename T, typename U>
    inline T car(Cons<T, U> * c) {
      return c->car();
    }

  template<typename T, typename U>
    inline U cdr(Cons<T, U> * c) {
      return c->cdr();
    }

  template <typename T>
    struct _consp : std::false_type {};
  template <typename T, typename U>
    struct _consp<Cons<T, U> * >: std::true_type {};
  template <typename T>
    inline bool consp(T) {
      return _consp<T>::value;
    }
  template <typename T> constexpr bool consp_v = _consp<T>::value;

  template <typename ... T>
    struct _list_t {
      using type = nullptr_t;
    };
  template <>
    struct _list_t<nullptr_t> {
      using type = nil_t;
    };
  template <typename T>
    struct _list_t<T> {
      using type = Cons<T, nil_t> *;
    };
  template <typename T, typename ... U>
    struct _list_t<T, U... > {
      using type = Cons<T, typename _list_t<U ... >::type> *;
    };

  template <typename T>
    struct _car_t {};
  template <>
    struct _car_t<nil_t> {
      using type = nil_t;
    };
  template <typename T, typename U>
    struct _car_t<Cons<T, U> * > {
      using type = T;
    };
  template <typename T>
    struct _cdr_t {};
  template <>
    struct _cdr_t<nil_t> {
      using type = nil_t;
    };
  template <typename T, typename U>
    struct _cdr_t<Cons<T, U> * > {
      using type = U;
    };


  template <typename T>
    struct _listp : std::false_type {};
  template <>
    struct _listp<nil_t> : std::true_type  {};
  template <typename T, typename U>
    struct _listp<Cons<T, U> *> : _listp<U> {
      //			static const bool value = _listp<U>::value;
    };
  template <typename T>
    inline bool listp(T) {
      return _listp<T>::value;
    }
  template <typename T> constexpr bool listp_v = _listp<T>::value;

  template <typename T>
    struct _list_len {
      static const int value = 0;
    };
  template <>
    struct _list_len<nil_t> {
      static const int value = 0;
    };
  template <typename T>
    struct _list_len<Cons<T, nil_t> * > {
      static const int value = 1;
    };
  template <typename T, typename U>
    struct _list_len<Cons<T, U> * > {
      static const int value = 1 + _list_len<U>::value;
    };
  //template <typename T, typename U>
  //inline int length(Cons<T, U> * l) {
  //if (!(listp(l))) {
  //throw "LENGTH only works on proper list.";
  //}
  //return _list_len<Cons<T, U> * >::value;
  //}
  template <typename T, typename U, typename IsProperList = std::enable_if_t<listp_v<Cons<T, U>*>>>
    constexpr int length(Cons<T, U> * l) {
      return _list_len<Cons<T, U> * >::value;
    }

  template <std::size_t N, typename T>
    struct _nth_t {};
  template <typename T, typename U>
    struct _nth_t <0, Cons<T, U> *> {
      using type = T;
    };
  template <std::size_t N, typename T, typename U>
    struct _nth_t <N, Cons<T, U> *> {
      using type = typename _nth_t<N - 1, U>::type;
    };
  template <std::size_t N>
    struct _nth;
  template <>
    struct _nth<0>{
      template <typename T, typename U>
        T operator() (Cons<T, U>* lst) const {
          return car(lst);
        }
    };
  template <std::size_t N>
    struct _nth{
      template <typename T, typename U>
        typename _nth_t<N, Cons<T, U>*>::type
        operator() (Cons<T, U>* lst) const {
          return _nth<N - 1>()(cdr(lst));
        }
    };
  template <std::size_t N, typename T, typename U,
           typename WithInListRange = std::enable_if_t<(N < _list_len<Cons<T, U> *>::value) && listp_v<Cons<T, U> *>>>
             auto nth(Cons<T, U> * lst) -> typename _nth_t<N, Cons<T, U> * >::type {
               return _nth<N>()(lst);
             }

  template <typename T>
    struct _nullp : std::false_type {};
  template <>
    struct _nullp<nil_t> : std::true_type {};
  template <typename T>
    inline bool nullp(T) {
      return _nullp<T>::value;
    }
  template <typename T> constexpr bool nullp_v = _nullp<T>::value;


  inline std::ostream & operator<<(std::ostream& os, Cons<nullptr_t, nullptr_t> *) {
    os << "nil";
    return os;
  }
  template <typename T>
    inline std::ostream & operator<<(std::ostream& os, Cons<T, nil_t> * c) {
      os << "(";
      os << car(c);
      os << ". nil)";
      return os;
    }
  template <typename T, typename U>
    inline std::ostream & operator<<(std::ostream& os, Cons<T, U> * c) {
      os << "(";
      os << car(c);
      if (cdr(c)) {
        os << " . ";
        os << cdr(c);
      }
      os << ")";
      return os;
    }

  template <typename T, typename U>
    inline std::string to_string(Cons<T, U> * c) {
      std::stringstream ss;
      ss << c;
      return ss.str();
    }

  template <typename T>
    inline bool equals(T a, T b) {
      return a == b;
    }
  template <typename T, typename U>
    inline bool equals(Cons<T, U> * lhs, Cons<T, U> * rhs) {
      if (car(lhs) == car(rhs)) {
        if (consp(cdr(lhs)) && consp(cdr(rhs))) {
          return equals(cdr(lhs), cdr(lhs));
        }
        return cdr(lhs) == cdr(rhs);
      }
      return false;
    }


  // A List Type That Holds Dynamic Number Of Element Of One Type
  template <typename T>
    class List {
      private:
        T _head;
        List<T> * _tail;

      public:
        T head() const { return _head; }
        List<T> * tail() const { return _tail; }
        List() :
          _head(nullptr),
          _tail(nullptr)
      {}

        List(T hd) :
          _head(hd),
          _tail(nullptr)
      {}
        List(T hd, T tl) :
          _head(hd),
          _tail(new List(tl))
      {}
        List(T hd, List<T> * lst) :
          _head(hd),
          _tail(lst)
      {}

        // A Trick To Use nullptr In The Places Require A Type Of List<T>*
        static List<T> *
          Nil() {
            return nullptr;
          }

        // Determine Whether Two Lists are Identical, Order Matters
        bool
          equals(List<T> & b, std::function<bool(T, T)> pred = [](T a, T b) { return a == b; }) {
            if (length(this) != length(b)) {
              return false;
            }
            if (pred(_head, b->head())) {
              if (_tail != nullptr) {
                return _tail->equals(b->tail(), pred);
              }
              else {
                return true;
              }
            }
            return false;
          }

    };

  // Some Handy Sugars
  template <typename T>
    inline T
    car(List<T> * lst) {
      return lst->head();
    }

  template <typename T>
    inline List<T> *
    cdr(List<T> * lst) {
      return lst->tail();
    }

  template <typename T>
    inline List<T> *
    lcons(T ele, List<T> * lst) {
      return new List<T>(ele, lst);
    }


  template <typename T>
    inline List<T> *
    tlist(T head) {
      return new List<T>(head, nullptr);
    }

  template <typename T, typename ...Ts>
    inline List<T> *
    tlist(T head, Ts... rest) {
      return lcons(head, list(rest...));
    }
};

#endif
