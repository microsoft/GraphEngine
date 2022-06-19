using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Storage
{
    /// <summary>
    /// Represents a low-level key-value store.
    /// </summary>
    public unsafe interface IKeyValueStore
    {
        #region Key-value Store interfaces
        /// <summary>
        /// Determines whether there is a cell with the specified cell Id in Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if a cell whose Id is cellId is found; otherwise, false.</returns>
         bool Contains(long cellId);

        /// <summary>
        /// Gets the type of the cell with specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellType">The type of the cell specified by cellId.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
         TrinityErrorCode GetCellType(long cellId, out ushort cellType);

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
         TrinityErrorCode AddCell(long cellId, byte* buff, int size, ushort cellType);

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if updating succeeds; otherwise, false.</returns>
         TrinityErrorCode UpdateCell(long cellId, byte* buff, int size);

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
         TrinityErrorCode SaveCell(long cellId, byte* buff, int size, ushort cellType);

        /// <summary>
        /// Loads the bytes of the cell with the specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellBuff">The bytes of the cell. An empty byte array is returned if the cell is not found.</param>
        /// <param name="cellType">The type of the cell, represented with a 16-bit unsigned integer.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
         TrinityErrorCode LoadCell(long cellId, out byte[] cellBuff, out ushort cellType);

        /// <summary>
        /// Loads the bytes of the cell with the specified cell Id.
        /// The content is stored in unmanaged heap, pointed by cellBuf, which must be
        /// freed after use.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="size">The size of the cell.</param>
        /// <param name="cellBuff">The bytes of the cell. An empty byte array is returned if the cell is not found.</param>
        /// <param name="cellType">The type of the cell, represented with a 16-bit unsigned integer.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
         TrinityErrorCode LoadCell(long cellId, out byte* cellBuff, out int size, out ushort cellType);

        /// <summary>
        /// Removes the cell with the specified cell Id from the key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if removing succeeds; otherwise, false.</returns>
         TrinityErrorCode RemoveCell(long cellId);
        #endregion

    }
}
