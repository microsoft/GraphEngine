#include "TypeSystem.h"

void __deepcopy(char* &dst, char* const &src)
{
    if (src) { dst = _strdup(src); }
    else { dst = nullptr; }
}

TypeDescriptor::~TypeDescriptor()
{
    for (int i = 0; i < ElementArity; ++i) { ElementType[i].~TypeDescriptor(); }
    for (int i = 0; i < NrMember; ++i) { Members[i].~MemberDescriptor(); }
    for (int i = 0; i < NrTSLAttribute; ++i) { TSLAttributes[i].~AttributeDescriptor(); }

    if (TypeName) free(TypeName);
    if (ElementType) free(ElementType);
    if (Members) free(Members);
    if (TSLAttributes) free(TSLAttributes);

    TypeName = nullptr;
    ElementType = nullptr;
    Members = nullptr;
    TSLAttributes = nullptr;
}