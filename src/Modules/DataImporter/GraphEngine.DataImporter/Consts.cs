using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphEngine.DataImporter
{
    internal static class Consts
    {
        internal const string         c_KW_TreeImport            = "TreeImport";
        internal const string         c_KW_TreeParent            = "TreeParent";
        internal const string         c_KW_CellIdKey             = "CellIdKey";
        internal const string         c_KW_HashImport            = "HashImport";
        internal const string         c_KW_CsvArray              = "CsvArray";
        internal const string         c_KW_ReverseEdgeImport     = "ReverseEdgeImport";
        internal const string         c_KW_Guid                  = "Guid";
        internal const string         c_KW_DateTime              = "DateTime";
        internal static readonly Type c_TYPE_CellId              = typeof(long);
        internal static readonly Type c_TYPE_CellIdList          = typeof(List<long>);
        internal static readonly Type c_TYPE_OptionalCellId      = typeof(long?);
        internal static readonly Type c_Type_Guid                = typeof(Guid);
        internal static readonly Type c_Type_DateTime            = typeof(DateTime);
    }
}
