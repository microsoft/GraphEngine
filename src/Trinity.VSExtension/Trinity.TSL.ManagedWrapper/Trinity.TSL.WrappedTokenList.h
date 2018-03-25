#pragma once
#include <Trinity.TSL.Parser.h>
#include "Trinity.TSL.WrappedTokenType.h"

using namespace System;

namespace Trinity
{
    namespace TSL
    {
        public ref class WrappedTokenInfo
        {
        public:

            WrappedTokenInfo(yytokentype t, int o1, int o2)
            {
                type = (WrappedTokenType) t;
                this->FirstOffset = o1;
                this->SecondOffset = o2;
            }

            WrappedTokenType type;
            int FirstOffset;
            int SecondOffset;
        };
        public ref class WrappedTokenList
        {
        public:
            WrappedTokenList(
                System::String^ buffer,
                System::Collections::Generic::List<System::String^>^ bufferLines,
                System::Collections::Generic::List<int>^ lineOffsets);
            System::Collections::Generic::List<WrappedTokenInfo^> ^tokens;
        };
    }
}
