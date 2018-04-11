#include "GraphEngine.Jit.Native.h"

VerbSequence::VerbSequence(FunctionDescriptor* f)
{
    type = &f->Type;
    pstart = f->Verbs;
    pend = pstart + f->NrVerbs;
    pcurrent = pstart - 1;

    pmember = nullptr;
    imember = -1;
}

// for all setters, lcontains, lcount and bget, we allow no further sub-verbs.

bool VerbSequence::Next()
{
    if (++pcurrent == pend) return false;

    switch (pcurrent->Code) {
    case VerbCode::VC_GSGet:
    {
        type = pcurrent->Data.GenericTypeArgument;
        break;
    }
    case VerbCode::VC_SGet:
    {
        auto members = type->get_Members();
        auto i = std::find_if(members->begin(), members->end(), [=](auto m) { return !strcmp(m->Name, pcurrent->Data.MemberName); });
        pmember = *i;
        imember = i - members->begin();
        delete members;
        type = &pmember->Type;
        break;
    }
    case VerbCode::VC_LGet:
    case VerbCode::VC_LInlineGet:
    case VerbCode::VC_LContains:
    {
        auto vec = type->get_ElementType();
        type = vec->at(0);
        delete vec;
    }
    }

    return true;
}

Verb* VerbSequence::CurrentVerb()
{
    return pcurrent;
}

TypeDescriptor* VerbSequence::CurrentType()
{
    return type;
}