using System;
using System.Threading.Tasks;

namespace Trinity.Storage
{
    public struct LoadCellResponse
    {
        public LoadCellResponse(TrinityErrorCode errorCode, byte[] cellBuff, ushort cellType)
        {
            this.ErrorCode = errorCode;
            this.CellBuff = cellBuff;
            this.CellType = cellType;
        }

        public TrinityErrorCode ErrorCode { get; }
        public byte[] CellBuff { get; }
        public ushort CellType { get; }
    }

    public unsafe struct LoadCellUnsafeResponse
    {
        public LoadCellUnsafeResponse(TrinityErrorCode errorCode, byte* cellBuff, int size, ushort cellType)
        {
            this.ErrorCode = errorCode;
            this.CellBuff = cellBuff;
            this.Size = size;
            this.CellType = cellType;
        }

        public TrinityErrorCode ErrorCode { get; }
        public byte* CellBuff { get; }
        public int Size { get; }
        public ushort CellType { get; }
    }

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
         Task<bool> ContainsAsync(long cellId);

        /// <summary>
        /// Gets the type of the cell with specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellType">The type of the cell specified by cellId.</param>
        /// <returns>A Trinity error code. Possible values are E_SUCCESS and E_NOT_FOUND.</returns>
         Task<(TrinityErrorCode ErrorCode, ushort CellType)> GetCellTypeAsync(long cellId);

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
         Task<TrinityErrorCode> AddCellAsync(long cellId, byte* buff, int size, ushort cellType);

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <returns>true if updating succeeds; otherwise, false.</returns>
         Task<TrinityErrorCode> UpdateCellAsync(long cellId, byte* buff, int size);

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="size">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
         Task<TrinityErrorCode> SaveCellAsync(long cellId, byte* buff, int size, ushort cellType);

        /// <summary>
        /// Loads the bytes of the cell with the specified cell Id.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="cellBuff">The bytes of the cell. An empty byte array is returned if the cell is not found.</param>
        /// <param name="cellType">The type of the cell, represented with a 16-bit unsigned integer.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
         Task<LoadCellResponse> LoadCellAsync(long cellId);

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
         Task<LoadCellUnsafeResponse> LoadCellUnsafeAsync(long cellId);

        /// <summary>
        /// Removes the cell with the specified cell Id from the key-value store.
        /// </summary>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if removing succeeds; otherwise, false.</returns>
         Task<TrinityErrorCode> RemoveCellAsync(long cellId);
        #endregion

    }
}
