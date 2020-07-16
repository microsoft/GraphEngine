using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Trinity.Configuration;

namespace Trinity.Storage
{
    public static unsafe class StorageExtensionMethods
    {
        public static Task<TrinityErrorCode> SaveCellAsync(this IKeyValueStore storage, long cell_id, byte[] cellBytes)
        {
            GCHandle gch = GCHandle.Alloc(cellBytes, GCHandleType.Pinned);
            byte* p = (byte*)gch.AddrOfPinnedObject();
            return storage.SaveCellAsync(cell_id, p, cellBytes.Length, StorageConfig.c_UndefinedCellType)
                          .ContinueWith(t => { gch.Free(); return t.Result; }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public static Task<TrinityErrorCode> SaveCellAsync(this IKeyValueStore storage, long cell_id, byte[] cellBytes, int startIndex, int cellSize)
        {
            GCHandle gch = GCHandle.Alloc(cellBytes, GCHandleType.Pinned);
            byte* p = (byte*)gch.AddrOfPinnedObject();
            return storage.SaveCellAsync(cell_id, p + startIndex, cellSize, StorageConfig.c_UndefinedCellType)
                          .ContinueWith(t => { gch.Free(); return t.Result; }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public static Task<TrinityErrorCode> SaveCellAsync(this IKeyValueStore storage, long cell_id, byte[] cellBytes, int startIndex, int cellSize, ushort cellType)
        {
            GCHandle gch = GCHandle.Alloc(cellBytes, GCHandleType.Pinned);
            byte* p = (byte*)gch.AddrOfPinnedObject();
            return storage.SaveCellAsync(cell_id, p + startIndex, cellSize, cellType)
                          .ContinueWith(t => { gch.Free(); return t.Result; }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public static Task<TrinityErrorCode> SaveCellAsync(this IKeyValueStore storage, long cell_id, byte[] cellBytes, ushort cellType)
        {
            GCHandle gch = GCHandle.Alloc(cellBytes, GCHandleType.Pinned);
            byte* p = (byte*)gch.AddrOfPinnedObject();
            return storage.SaveCellAsync(cell_id, p, cellBytes.Length, cellType)
                          .ContinueWith(t => { gch.Free(); return t.Result; }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public static Task<TrinityErrorCode> SaveCellAsync(this IKeyValueStore storage, long cell_id, byte* cellPtr, int length)
        {
            return storage.SaveCellAsync(cell_id, cellPtr, length, StorageConfig.c_UndefinedCellType);
        }

        public static Task<TrinityErrorCode> AddCellAsync(this IKeyValueStore storage, long cell_id, byte[] cellBytes)
        {
            GCHandle gch = GCHandle.Alloc(cellBytes, GCHandleType.Pinned);
            byte* p = (byte*)gch.AddrOfPinnedObject();
            return storage.AddCellAsync(cell_id, p, cellBytes.Length, StorageConfig.c_UndefinedCellType)
                          .ContinueWith(t => { gch.Free(); return t.Result; }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public static Task<TrinityErrorCode> AddCellAsync(this IKeyValueStore storage, long cell_id, byte[] cellBytes, int startIndex, int length)
        {
            GCHandle gch = GCHandle.Alloc(cellBytes, GCHandleType.Pinned);
            byte* p = (byte*)gch.AddrOfPinnedObject();
            return storage.AddCellAsync(cell_id, p + startIndex, length, StorageConfig.c_UndefinedCellType)
                          .ContinueWith(t => { gch.Free(); return t.Result; }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public static Task<TrinityErrorCode> AddCellAsync(this IKeyValueStore storage, long cell_id, byte[] cellBytes, int startIndex, int length, ushort cellType)
        {
            GCHandle gch = GCHandle.Alloc(cellBytes, GCHandleType.Pinned);
            byte* p = (byte*)gch.AddrOfPinnedObject();
            return storage.AddCellAsync(cell_id, p + startIndex, length, cellType)
                          .ContinueWith(t => { gch.Free(); return t.Result; }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public static Task<TrinityErrorCode> UpdateCellAsync(this IKeyValueStore storage, long cell_id, byte[] cellBytes)
        {
            GCHandle gch = GCHandle.Alloc(cellBytes, GCHandleType.Pinned);
            byte* p = (byte*)gch.AddrOfPinnedObject();
            return storage.UpdateCellAsync(cell_id, p, cellBytes.Length)
                          .ContinueWith(t => { gch.Free(); return t.Result; }, TaskContinuationOptions.ExecuteSynchronously);
        }

        public static Task<TrinityErrorCode> UpdateCellAsync(this IKeyValueStore storage, long cell_id, byte[] cellBytes, int startIndex, int length)
        {
            GCHandle gch = GCHandle.Alloc(cellBytes, GCHandleType.Pinned);
            byte* p = (byte*)gch.AddrOfPinnedObject();
            return storage.UpdateCellAsync(cell_id, p + startIndex, length)
                          .ContinueWith(t => { gch.Free(); return t.Result; }, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
