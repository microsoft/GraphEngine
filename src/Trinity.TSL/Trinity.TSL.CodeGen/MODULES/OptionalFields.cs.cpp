#include "common.h"
#include <string>
#include <SyntaxNode.h>

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        namespace Modules
        {
            string* 
OptionalFields(
NStructBase* node, ModuleContext* context)
            {
                string* source = new string();
                
OptionalFieldCalculator optcalc_1 = OptionalFieldCalculator(node);
source->append(R"::(
        internal static string[] optional_field_names = null;
        ///<summary>
        ///Get an array of the names of all optional fields for object type t_struct_name.
        ///</summary>
        public static string[] GetOptionalFieldNames()
        {
            if (optional_field_names == null)
                optional_field_names = new string[]
                {
                    )::");
for (size_t iterator_1 = 0; iterator_1 < (node->fieldList)->size();++iterator_1)
{
if(!(*(node->fieldList))[iterator_1]->is_optional())continue;
source->append(R"::(
                    ")::");
source->append(Codegen::GetString((*(node->fieldList))[iterator_1]->name));
source->append(R"::("
                    )::");
if (iterator_1 < (node->fieldList)->size() - 1)
source->append(",");
}
source->append(R"::(
                };
            return optional_field_names;
        }
        ///<summary>
        ///Get a list of the names of available optional fields in the object being operated by this accessor.
        ///</summary>
        internal List<string> GetNotNullOptionalFields()
        {
            List<string> list = new List<string>();
            BitArray ba = new BitArray(GetOptionalFieldMap());
            string[] optional_fields = GetOptionalFieldNames();
            for (int i = 0; i < ba.Count; i++)
            {
                if (ba[i])
                    list.Add(optional_fields[i]);
            }
            return list;
        }
        internal unsafe byte[] GetOptionalFieldMap()
        {
            )::");
if (optcalc_1.headerLength > 0)
{
source->append(R"::(
            byte [] bytes = new byte[)::");
source->append(Codegen::GetString(optcalc_1.headerLength));
source->append(R"::(];
            Memory.Copy(CellPtr, 0, bytes, 0, )::");
source->append(Codegen::GetString(optcalc_1.headerLength));
source->append(R"::();
            return bytes;
            )::");
}
else
{
source->append(R"::(
            return new byte[0];
            )::");
}
source->append(R"::(
        }
        )::");

                return source;
            }
        }
    }
}
