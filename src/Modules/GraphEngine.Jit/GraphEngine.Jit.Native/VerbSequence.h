#pragma once
#include "TypeSystem.h"
#include "Verb.h"

struct VerbSequence
{
    TypeDescriptor*   parent;
    TypeDescriptor*   ptype;
    Verb*             pstart;
    Verb*             pend;
    Verb*             pcurrent;

    MemberDescriptor* pmember;
    int32_t           imember;
    int64_t           iidx; //inline integer index

    VerbSequence(FunctionDescriptor* f);
    bool Next();

    TypeId::Id CurrentTypeId() const { return GetTypeId(ptype); }
    TypeCode CurrentTypeCode() const { return (TypeCode)ptype->get_TypeCode(); }
};