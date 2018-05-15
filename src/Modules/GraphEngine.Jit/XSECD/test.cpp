#include <iostream>

#include "xsecd.hpp"

using namespace std;
using namespace xsecd;

int main(){
    XStack * stack = (XStack *)malloc(2 * sizeof(saddr_t) + 10 * sizeof(XObj32));
    stack->sop = 0;
    for(; stack->sop < 10; ){
        XObj32 o;
        o.tid = T_U16;
        o._u16 = 123 * stack->sop;
        cout << "Pushing: ";
        cout << o;
        cout << endl;
        stack << o;
        cout << "Pushed" << endl;
    }

    cout << stack << endl;

    return 0;
}
