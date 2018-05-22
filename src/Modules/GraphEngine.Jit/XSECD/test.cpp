#include <iostream>

#include "xsecd.hpp"

using namespace std;
using namespace xsecd;
using namespace licpp;

int main(){

  auto l1 = licpp::list(1, 2, 3);
  cout << (licpp::m_list_of_v<int, decltype(l1)> ? "true" : "false") << endl;
  auto l2 = licpp::list(1, licpp::list(2, 4), 3);
  cout << (licpp::m_list_of_v<int, decltype(l2)> ? "true" : "false") << endl;

  auto stk = new_stack(10);
  alloc_prim<T_U32>(stk, 1234);
  cout << stk << endl;

  saddr_t a = 1;
  saddr_t b = 2;
  saddr_t c = 3;
  saddr_t d = 4;
  saddr_t e = 5;
  saddr_t f = 6;
  saddr_t g = 7;

  auto test_lst = licpp::list(licpp::list(a, b, c), d, licpp::list(f, g), e);
  cout << (licpp::m_list_of_v<saddr_t, decltype(test_lst)> ? "true" : "false") << endl;
  cout << list_size_v<decltype(test_lst)> << endl;
  cout << list_size(test_lst) << endl;

  // sizeof(a) = 1, sizeof(b) = 2
  xsecd::xalloc(stk, licpp::list(a, b));
  cout << stk << endl;

  return 0;
}
