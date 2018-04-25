#include "Verb.h"

Verb::~Verb()
{
    if (Code == VerbCode::VC_GSGet || Code == VerbCode::VC_GSSet)
    {
        Data.GenericTypeArgument->~TypeDescriptor();
        free(Data.GenericTypeArgument);
    }
    else if (Code == VerbCode::VC_SGet || Code == VerbCode::VC_SGet)
    {
        free(Data.MemberName);
    }
}

bool Verb::is_setter()
{
    switch (Code) {
    case VC_BSet:
    case VC_GSSet:
    case VC_SSet:
    case VC_LSet:
    case VC_LInlineSet:
        return true;
    default:
        return false;
    }
}