using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Configuration;

namespace Trinity.Storage
{
    public unsafe static class StorageExtensionMethods
    {
        public static TrinityErrorCode LoadCell(this IKeyValueStore storage, long cell_id, out byte[] buff)
        {
            return storage.LoadCell(cell_id, out buff, out _);
        }
        public static TrinityErrorCode SaveCell(this IKeyValueStore storage, long cell_id, byte[] cellBytes)
        {
            fixed(byte* p = cellBytes)
            {
                return storage.SaveCell(cell_id, p, cellBytes.Length, StorageConfig.c_UndefinedCellType);
            }
        }
        public static TrinityErrorCode SaveCell(this IKeyValueStore storage, long cell_id, byte[] cellBytes, int startIndex, int cellSize)
        {
            fixed(byte* p = cellBytes)
            {
                return storage.SaveCell(cell_id, p + startIndex, cellSize, StorageConfig.c_UndefinedCellType);
            }
        }
        public static TrinityErrorCode SaveCell(this IKeyValueStore storage, long cell_id, byte[] cellBytes, int startIndex, int cellSize, ushort cellType)
        {
            fixed(byte* p = cellBytes)
            {
                return storage.SaveCell(cell_id, p + startIndex, cellSize, cellType);
            }
        }
        public static TrinityErrorCode SaveCell(this IKeyValueStore storage, long cell_id, byte[] cellBytes, ushort cellType)
        {
            fixed(byte* p = cellBytes)
            {
                return storage.SaveCell(cell_id, p, cellBytes.Length, cellType);
            }
        }
        public static TrinityErrorCode SaveCell(this IKeyValueStore storage, long cell_id, byte* cellPtr, int length)
        {
            return storage.SaveCell(cell_id, cellPtr, length, StorageConfig.c_UndefinedCellType);
        }
        public static TrinityErrorCode AddCell(this IKeyValueStore storage, long cell_id, byte[] cellBytes)
        {
            fixed(byte* p = cellBytes)
            {
                return storage.AddCell(cell_id, p, cellBytes.Length, StorageConfig.c_UndefinedCellType);
            }
        }
        public static TrinityErrorCode AddCell(this IKeyValueStore storage, long cell_id, byte[] cellBytes, int startIndex, int length)
        {
            fixed(byte* p = cellBytes)
            {
                return storage.AddCell(cell_id, p + startIndex, length, StorageConfig.c_UndefinedCellType);
            }
        }
        public static TrinityErrorCode AddCell(this IKeyValueStore storage, long cell_id, byte[] cellBytes, int startIndex, int length, ushort cellType)
        {
            fixed(byte* p = cellBytes)
            {
                return storage.AddCell(cell_id, p + startIndex, length, cellType);
            }
        }
        public static TrinityErrorCode UpdateCell(this IKeyValueStore storage, long cell_id, byte[] cellBytes)
        {
            fixed(byte* p = cellBytes)
            {
                return storage.UpdateCell(cell_id, p, cellBytes.Length);
            }
        }
        public static TrinityErrorCode UpdateCell(this IKeyValueStore storage, long cell_id, byte[] cellBytes, int startIndex, int length)
        {
            fixed(byte* p = cellBytes)
            {
                return storage.UpdateCell(cell_id, p + startIndex, length);
            }
        }
    }
}
