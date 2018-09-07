#pragma once
#include "corelib"
#include "SyntaxNode.h"

namespace Trinity
{
    namespace Codegen
    {
        class OptionalFieldCalculator
        {
        public:
            OptionalFieldCalculator(NStructBase* node, std::string pointer_name = "")
            {
                this->nstruct = node;
                fieldCount = 0;
                for (auto f : *node->fieldList)
                {
                    if (f->is_optional())
                    {
                        ++fieldCount;
                    }
                }
                headerLength = (fieldCount + 0x07) >> 3;
                pointerName = pointer_name;
            }
            std::string GenerateMaskOnCode(NField* field)
            {
                int fieldSequence = _getFieldSequence(field);
                int offset = fieldSequence / 8;
                fieldSequence %= 8;
                String mask = _getHex(1 << fieldSequence);
                return String::Format(R":(*({0} + {1}) |= {2};):", pointerName, offset, mask);
            }
            std::string GenerateMaskOffCode(NField* field)
            {
                int fieldSequence = _getFieldSequence(field);
                int offset = fieldSequence / 8;
                fieldSequence %= 8;
                String mask = _getHex(~(1 << fieldSequence));
                return String::Format(R":(*({0} + {1}) &= {2};):", pointerName, offset, mask);
            }
            std::string GenerateReadBitExpression(NField* field)
            {
                int fieldSequence = _getFieldSequence(field);
                int offset = fieldSequence / 8;
                fieldSequence %= 8;
                String mask = _getHex(1 << fieldSequence);
                return String::Format(R":((0 != (*({0} + {1}) & {2}))):", pointerName, offset, mask);
            }
            std::string GenerateClearAllBitsCode()
            {
                string ret = "";
                for (int i = 0; i < headerLength; ++i)
                {
                    ret += String::Format(R":(*({0} + {1}) = 0x00;):", pointerName, i);
                }
                return ret;
            }
            int fieldCount;
            int headerLength;
        private:

            int _getFieldSequence(NField* field)
            {
                int seq = 0;
                for (auto f : *nstruct->fieldList)
                {
                    if (f == field) return seq;
                    if (f->is_optional())
                        ++seq;
                }
                return seq;
            }

            String _getHex(int value)
            {
                char* hexarr = "0123456789ABCDEF";
                value &= 0xFF;
                String ret = "0x";
                ret.Append(hexarr[value >> 4]);
                ret.Append(hexarr[value & 0x0F]);
                return ret;
            }

            NStructBase* nstruct;
            std::string pointerName;
        };
    }
}
