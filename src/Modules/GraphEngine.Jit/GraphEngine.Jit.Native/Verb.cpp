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
