using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Utilities;

namespace Trinity.TSL
{
    partial class StorageCodeTemplate
    {
        private static string GenerateCellTypeExtension(SpecificationScript script)
        {
            if (script.CellDescriptors.Count == 0)
                return "";

            CodeWriter ret = new CodeWriter();
            ret += @"
    /// <summary>
    /// Provides cell type interfaces on <see cref=""Trinity.Storage.LocalMemoryStorage""/>.
    /// </summary>
    static public class Storage_CellType_Extension
    {";
            foreach (var desc in script.CellDescriptors)
            {
                ret += @"
        /// <summary>
        /// Tells whether the cell with the given id is a " + desc.Name + @" .
        /// </summary>
        /// <param name=""storage""/>A <see cref=""Trinity.Storage.LocalMemoryStorage""/> instance.</param>
        /// <param name=""CellID"">The id of the cell.</param>
        /// <returns>True if the cell is found and is of the correct type. Otherwise false.</returns>
        public unsafe static bool Is" + desc.Name + @"(this Trinity.Storage.LocalMemoryStorage storage, long CellID)
        {
            ushort cellType;
            if(storage.GetCellType(CellID, out cellType) == TrinityErrorCode.E_SUCCESS)
            {
                return cellType == (ushort)CellType." + desc.Name + @"; 
            }
            return false;
        }";
            }
            ret += @"
        /// <summary>
        /// Get the type of the cell.
        /// </summary>
        /// <param name=""storage""/>A <see cref=""Trinity.Storage.LocalMemoryStorage""/> instance.</param>
        /// <param name=""CellID"">The id of the cell.</param>
        /// <returns>If the cell is found, returns the type of the cell. Otherwise, returns CellType.Undefined.</returns>
        public unsafe static CellType GetCellType(this Trinity.Storage.LocalMemoryStorage storage, long CellID)
        {
            ushort cellType;
            if(storage.GetCellType(CellID, out cellType) == TrinityErrorCode.E_SUCCESS)
            {
                return (CellType)cellType; 
            }
            else
            {
                return CellType.Undefined;
            }
        }
//        public unsafe static void Transform(this Trinity.Storage.LocalMemoryStorage storage, params CellConverter[] converters)
//        {
//            Dictionary<ushort, CellTransformAction<long,int,ushort>> converterMap = new Dictionary<ushort, CellTransformAction<long,int,ushort>>();
//            foreach (var converter in converters)
//            {
//                converterMap[(ushort)converter.SourceCellType] = converter._action;
//            }
//            storage.TransformCells((byte* ptr, long id,  int count, ref ushort cellType) =>
//                {
//                    //ushort cellType = *(ushort*)ptr;
//                    if (converterMap.ContainsKey(cellType))
//                    {
//                        return converterMap[cellType](ptr, id, count, ref cellType);
//                    }
//                    else
//                    {
//                        byte[] ret = new byte[count];
//                        Memory.Copy(ptr, 0, ret, 0, count);
//                        return ret;
//                    }
//                });
//        }
    }";
            return ret;
        }

    }
}