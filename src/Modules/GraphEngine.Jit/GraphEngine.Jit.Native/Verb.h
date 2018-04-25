#pragma once
#pragma pack(push, 1)

#include "Common.h"

//  !Keep in sync with Verbs.fs
enum VerbCode : int32_t {
    /* BasicVerb */
    VC_BGet,
    VC_BSet,
    VC_BEq,
    VC_BLe,
    VC_BLt,
    VC_BGe,
    VC_BGt,
    VC_BHash,
    VC_BCount, // Nr. elements, e.g. wcstrlen
    VC_BSize,  // Size in bytes

    /* ListVerb */
    VC_LInlineGet, // VerbData :: int64_t
    VC_LInlineSet, // VerbData :: int64_t
    VC_LGet,
    VC_LSet,
    VC_LContains,
    VC_LCount,
    VC_LInsertAt,
    VC_LRemoveAt,
    VC_LAppend,
    VC_LConv, // converts TSL list to native list

    /* StructVerb */
    VC_SGet, // VerbData :: char* 
    VC_SSet, // VerbData :: char* 

    /* GenericStructVerb */
    VC_GSGet, // VerbData :: TypeDescriptor* 
    VC_GSSet, // VerbData :: TypeDescriptor*

    /* Enumerable */
    VC_EAlloc ,
    VC_EFree ,
    VC_ENext ,
    VC_ECurrent ,

    // below invalid for native
    VC_ComposedVerb
};

union VerbData
{
    char*           MemberName;
    int64_t         Index;
    TypeDescriptor* GenericTypeArgument;
};

struct Verb
{
    VerbCode Code;
    VerbData Data;

    ~Verb();
    bool is_setter();
};

struct FunctionDescriptor
{
    TypeDescriptor Type; // embedded
    Verb*          Verbs;
    int32_t        NrVerbs;

    ~FunctionDescriptor()
    {
        for (auto p = Verbs, pend = Verbs + NrVerbs; p != pend; ++p)
        {
            p->~Verb();
        }

        if (Verbs) free(Verbs);
    }
};


#pragma pack(pop)