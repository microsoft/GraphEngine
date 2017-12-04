#include "SyntaxNode.h"

using namespace std;

#define DEFINE_GET_CHILDREN(_class, code) \
    vector<Node*> _class::get_children() { \
    vector<Node*> children; \
    code; \
    return children; \
    }
#define ADD_CHILDREN(list)	children.insert(children.end(), list->begin(), list->end());
#define ADD_CHILD(child)	children.push_back(child);

DEFINE_GET_CHILDREN(NKVPair, {})
DEFINE_GET_CHILDREN(NFieldType, {
    switch (fieldType)
    {
    case FT_ARRAY:
        ADD_CHILD(arrayInfo.arrayElement);
        break;
    case FT_LIST:
        ADD_CHILD(listElementType);
        break;
    }
})
DEFINE_GET_CHILDREN(NField, {
    ADD_CHILDREN(attributes);
    ADD_CHILD(fieldType);
})
DEFINE_GET_CHILDREN(NStruct, {
    ADD_CHILDREN(attributes);
    ADD_CHILDREN(fieldList);
})
DEFINE_GET_CHILDREN(NCell, {
    ADD_CHILDREN(attributes);
    ADD_CHILDREN(fieldList);
})
DEFINE_GET_CHILDREN(NTrinitySettings, {
    ADD_CHILDREN(settings);
})
DEFINE_GET_CHILDREN(NProtocol, ADD_CHILDREN(protocolPropertyList));//in semantic check we will process the properties and trim them out.
DEFINE_GET_CHILDREN(NProtocolReference, {});
DEFINE_GET_CHILDREN(NProtocolProperty, {});
DEFINE_GET_CHILDREN(NServer, ADD_CHILDREN(protocolList));// the protocol list should not be hidden
DEFINE_GET_CHILDREN(NProxy, ADD_CHILDREN(protocolList));
DEFINE_GET_CHILDREN(NModule, ADD_CHILDREN(protocolList));
DEFINE_GET_CHILDREN(NEnum, ADD_CHILDREN(enumEntryList));
DEFINE_GET_CHILDREN(NEnumEntry, {});
DEFINE_GET_CHILDREN(NTSL, {
    ADD_CHILDREN(structList);
    ADD_CHILDREN(cellList);
    ADD_CHILDREN(settingsList);
    ADD_CHILDREN(protocolList);
    ADD_CHILDREN(serverList);
    ADD_CHILDREN(proxyList);
    ADD_CHILDREN(moduleList);
    ADD_CHILDREN(enumList);
});// TODO