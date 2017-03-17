#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
Throw(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Storage;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    class Throw
    {
        )::");
for (size_t iterator_1 = 0; iterator_1 < (Trinity::Codegen::TSLExternalParserDataTypeVector)->size();++iterator_1)
{
if (data_type_need_set_field((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1], Trinity::Codegen::TSLExternalParserDataTypeVector) )
{
source->append(R"::(
        internal static void parse_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::((string value)
        {
            throw new ArgumentException("Cannot parse \""+value+"\" into )::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(.");
        }
        internal static void incompatible_with_)::");
source->append(Codegen::GetString(Trinity::Codegen::GetDataTypeDisplayString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1])));
source->append(R"::(()
        {
            throw new DataTypeIncompatibleException("Data type ')::");
source->append(Codegen::GetString((*(Trinity::Codegen::TSLExternalParserDataTypeVector))[iterator_1]));
source->append(R"::(' not compatible with the target field.");
        }
        )::");
}
}
source->append(R"::(
        internal static void data_type_incompatible_with_list(string type)
        {
            throw new DataTypeIncompatibleException("Data type '" + type + "' not compatible with the target list.");
        }
        internal static void data_type_incompatible_with_field(string type)
        {
            throw new DataTypeIncompatibleException("Data type '" + type + "' not compatible with the target field.");
        }
        internal static void target__field_not_list()
        {
            throw new DataTypeIncompatibleException("Target field is not a List, value or a string, cannot perform append operation.");
        }
        internal static void list_incompatible_list(string type)
        {
            throw new DataTypeIncompatibleException("List type '" + type + "' not compatible with the target list.");
        }
        internal static void incompatible_with_cell()
        {
            throw new DataTypeIncompatibleException("Data type incompatible with the cell.");
    )::");
source->append(R"::(    }
        internal static void array_dimension_size_mismatch(string type)
        {
            throw new ArgumentException(type + ": Array dimension size mismatch.");
        }
        internal static void invalid_cell_type()
        {
            throw new ArgumentException("Invalid cell type name. If you want a new cell type, please define it in your TSL.");
        }
        internal static void undefined_field()
        {
            throw new ArgumentException("Undefined field.");
        }
        
        internal static void member_access_on_non_struct__field(string field_name_string)
        {
            throw new DataTypeIncompatibleException("Cannot apply member access method on a non-struct field'" + field_name_string + "'.");
        }
        internal static void cell_id_is_null()
        {
            throw new NullReferenceException("The cell Id is null.");
        }
        internal static void cell_not_found()
        {
            throw new CellNotFoundExceptio)::");
source->append(R"::(n("The cell is not found.");
        }
        internal static void cell_not_found(long CellID)
        {
            throw new CellNotFoundException("The cell with id = " + CellID + " not found.");
        }
        internal static void wrong_cell_type()
        {
            throw new CellTypeNotMatchException("Cell type mismatched.");
        }
        internal static unsafe void cannot_parse(string value, string type_str)
        {
            throw new ArgumentException("Cannot parse \""+value+"\" into " + type_str + ".");
        }
    }
}
)::");

            return source;
        }
    }
}
