#pragma warning disable 162,168,649,660,661,1522
using Trinity;
using Trinity.TSL;
namespace Trinity.Extension
{
    public static class Storage_CellType_Extension
    {
        
        /// <summary>
        /// Tells whether the cell with the given id is a C1.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the cell.</param>
        /// <returns>True if the cell is found and is of the correct type. Otherwise false.</returns>
        public unsafe static bool IsC1(this Trinity.Storage.LocalMemoryStorage storage, long cellId)
        {
            ushort cellType;
            if (storage.GetCellType(cellId, out cellType) == TrinityErrorCode.E_SUCCESS)
            {
                return cellType == (ushort)CellType.C1;
            }
            return false;
        }
        
        /// <summary>
        /// Tells whether the cell with the given id is a C2.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the cell.</param>
        /// <returns>True if the cell is found and is of the correct type. Otherwise false.</returns>
        public unsafe static bool IsC2(this Trinity.Storage.LocalMemoryStorage storage, long cellId)
        {
            ushort cellType;
            if (storage.GetCellType(cellId, out cellType) == TrinityErrorCode.E_SUCCESS)
            {
                return cellType == (ushort)CellType.C2;
            }
            return false;
        }
        
        /// <summary>
        /// Get the type of the cell.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="cellId">The id of the cell.</param>
        /// <returns>If the cell is found, returns the type of the cell. Otherwise, returns CellType.Undefined.</returns>
        public unsafe static CellType GetCellType(this Trinity.Storage.LocalMemoryStorage storage, long cellId)
        {
            ushort cellType;
            if (storage.GetCellType(cellId, out cellType) == TrinityErrorCode.E_SUCCESS)
            {
                return (CellType)cellType;
            }
            else
            {
                return CellType.Undefined;
            }
        }
    }
}

#pragma warning restore 162,168,649,660,661,1522
