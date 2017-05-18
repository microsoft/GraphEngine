#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
CellTypeExtension(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(using Trinity;
using Trinity.TSL;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    
    public static class Storage_CellType_Extension
    {
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
        /// <summary>
        /// Tells whether the cell with the given id is a )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">The id of the cell.</param>
        /// <returns>True if the cell is found and is of the correct type. Otherwise false.</returns>
        public unsafe static bool Is)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::((this Trinity.Storage.LocalMemoryStorage storage, long CellID)
        {
            ushort cellType;
            if (storage.GetCellType(CellID, out cellType) == TrinityErrorCode.E_SUCCESS)
            {
                return cellType == (ushort)CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(;
            }
            return false;
        }
        )::");
}
source->append(R"::(
        /// <summary>
        /// Get the type of the cell.
        /// </summary>
        /// <param name="storage"/>A <see cref="Trinity.Storage.LocalMemoryStorage"/> instance.</param>
        /// <param name="CellID">The id of the cell.</param>
        /// <returns>If the cell is found, returns the type of the cell. Otherwise, returns CellType.Undefined.</returns>
        public unsafe static CellType GetCellType(this Trinity.Storage.LocalMemoryStorage storage, long CellID)
        {
            ushort cellType;
            if (storage.GetCellType(CellID, out cellType) == TrinityErrorCode.E_SUCCESS)
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
)::");

            return source;
        }
    }
}
