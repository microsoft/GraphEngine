#include "VerbSequence.h"
#include "VerbMixins.h"
#include "Common.h"

VerbSequence::VerbSequence(FunctionDescriptor* f)
{
    ptype = &f->Type;
    pstart = f->Verbs;
    pend = pstart + f->NrVerbs;
    pcurrent = pstart - 1;

    pmember = nullptr;
    imember = -1;
    parent = nullptr;
	iidx = -1;

    debug(f->NrVerbs);
}

// for all setters, lcontains, lcount and bget, we allow no further sub-verbs.

bool VerbSequence::Next()
{
    return Mixin::SequenceNext(*this);
}
