import initialize
import GraphEngine as ge

import Trinity
import Trinity.Storage
import Trinity.Storage.Composite


def InspectType(typeDesc, indent=0):
    pad = ''.ljust(indent)
    members = typeDesc.Members
    print("{}Type {}".format(pad, typeDesc.TypeName))
    print("{}  QualifiedName={}".format(pad, typeDesc.QualifiedName))
    print("{}  TypeCode={}".format(pad, typeDesc.TypeCode))
    print("{}  NrMember={}".format(pad, len(members)))
    for member in members:
        print("{}  Member {}".format(pad, member.Name))
        InspectType(member.Type, indent + 2)


Trinity.Storage.Composite.CompositeStorage.AddStorageExtension("./tsl", "TestTslModule")
descriptors = ge.__ffi.GetCellDescriptors()
print("#desc = {}".format(len(descriptors)))

for desc in descriptors:
    InspectType(desc)

