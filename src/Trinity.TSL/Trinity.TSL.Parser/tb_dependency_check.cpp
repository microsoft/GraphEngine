#include "SyntaxNode.h"
#include "parser.tab.h"
#include "flex_bison_common.h"
using namespace std;

#define DEFINE_TB_DEPENDENCY_CHECK(_class, code) \
    void _class::tb_dependency_check(DependencyResolver& resolver) { \
code; \
    }

/* Children are automatically added in the dependency graph. Only report additional dependency here. */


DEFINE_TB_DEPENDENCY_CHECK(NKVPair, {})

DEFINE_TB_DEPENDENCY_CHECK(NFieldType, {
    NStruct* nstruct = NULL;
    NEnum* nenum = NULL;
    if (fieldType == FT_STRUCT || fieldType == FT_ENUM || fieldType == FT_REFERENCE)
    {
        nstruct = tsl->find_struct(referencedTypeName);
        nenum = tsl->find_enum(referencedTypeName);
        if (nstruct != NULL)
            resolver.declareDependency(this, nstruct);
        if (nenum != NULL)
            resolver.declareDependency(this, nenum);
    }
})

DEFINE_TB_DEPENDENCY_CHECK(NField, {})
DEFINE_TB_DEPENDENCY_CHECK(NStruct, {})
DEFINE_TB_DEPENDENCY_CHECK(NCell, {})
DEFINE_TB_DEPENDENCY_CHECK(NTrinitySettings, {})
DEFINE_TB_DEPENDENCY_CHECK(NProtocol, {
    NStructBase *nstruct = NULL;
    if (this->request_message_struct != NULL && NULL != (nstruct = tsl->find_struct_or_cell(request_message_struct)))
        resolver.declareDependency(this, nstruct);
    if (this->response_message_struct != NULL && NULL != (nstruct = tsl->find_struct_or_cell(response_message_struct)))
        resolver.declareDependency(this, nstruct);
})
DEFINE_TB_DEPENDENCY_CHECK(NProtocolProperty, {})
DEFINE_TB_DEPENDENCY_CHECK(NProtocolReference, {
    NProtocol *nproto = tsl->find_protocol(name);
    if (nproto != NULL)
        resolver.declareDependency(this, nproto);
})

DEFINE_TB_DEPENDENCY_CHECK(NServer, {})
DEFINE_TB_DEPENDENCY_CHECK(NProxy, {})
DEFINE_TB_DEPENDENCY_CHECK(NModule, {})
DEFINE_TB_DEPENDENCY_CHECK(NEnum, {})
DEFINE_TB_DEPENDENCY_CHECK(NEnumEntry, {})
DEFINE_TB_DEPENDENCY_CHECK(NTSL, {})
