#include "../asmjit/src/asmjit/base/operand.h"

using namespace asmjit;

class TypeIdSpecializer
{
public:
    static int LPVOID() { return (int)TypeIdOf<void*>::kTypeId; }
};