// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
#if !CORECLR
using System.Runtime.ConstrainedExecution;
#endif
using System.Runtime;
using System.Security;

namespace Trinity.Core.Lib
{
    internal enum MemoryAllocationProfile : int
    {
        Aggressive = 0, TrinityDefault = 1, Modest = 2, SystemDefault = 3
    }

    public static unsafe partial class Memory
    {
        static Memory()
        {
            //Trigger the InternalCalls constructor, inject
            //native code if possible
            InternalCalls.__init();
        }

        /// <summary>
        /// Copies a specified number of bytes from a source buffer to a destination buffer.
        /// </summary>
        /// <param name="src">The source buffer.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="count">The number of bytes to copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void Copy(void* src, void* dst, int count) { CMemory.Copy(src, dst, count); }

        /// <summary>
        /// C-style memmove. Copies a specified number of bytes from a source buffer to a destination buffer. 
        /// If some regions of the source and destination buffer overlap, the bytes of the overlapping region are copied before being overwritten.
        /// </summary>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="src">The source buffer.</param>
        /// <param name="count">The number of bytes to copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* memmove(void* dst, void* src, ulong count) { return CMemory.C_memmove(dst, src, count); }

        /// <summary>
        /// C-style memcpy. Copies a specified number of bytes from a source buffer to a destination buffer.
        /// </summary>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="src">The source buffer.</param>
        /// <param name="count">The number of bytes to copy.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* memcpy(void* dst, void* src, ulong count) { return CMemory.C_memcpy(dst, src, count); }

        /// <summary>
        /// C-style memcmp. Compares the specified number of bytes in two buffers and returns a value indicating their relationship.
        /// </summary>
        /// <param name="ptr1">Pointer to the first buffer.</param>
        /// <param name="ptr2">Pointer to the second buffer.</param>
        /// <param name="count">The number of bytes to compare.</param>
        /// <returns>Zero indicates the bytes of two buffers are identical. A value less than zero indicates the first buffer less than the second. Otherwise, a value great than zero is returned.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe int memcmp(void* ptr1, void* ptr2, ulong count) { return CMemory.C_memcmp(ptr1, ptr2, count); }

        /// <summary>
        /// C-style memset. Sets the specified number of bytes in a buffer to the specified value.
        /// </summary>
        /// <param name="buff">Pointer to memory buffer.</param>
        /// <param name="value">Value to set.</param>
        /// <param name="count">The number of bytes to set.</param>
        /// <returns>The pointer of memory buffer is returned.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* memset(void* buff, int value, ulong count) { return CMemory.C_memset(buff, value, count); }

        /// <summary>
        /// C-style realloc. Reallocate a previously allocated memory buffer. The content of the memory buffer is preserved up to the shorter of the old and new buffer. The expanded portion of the new buffer is indeterminate. If size is zero, the previously allocated buffer is freed, NULL is returned. If buff is NULL, it behaves the same way as malloc.
        /// </summary>
        /// <param name="buff">Pointer to a previously allocated memory buffer.</param>
        /// <param name="size">Number of bytes of the reallocated memory buffer.</param>
        /// <returns>A pointer to the reallocated memory buffer.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* realloc(void* buff, ulong size) { return CMemory.C_realloc(buff, size); }

        /// <summary>
        /// C-style malloc. Allocates specified number of bytes of memory.
        /// </summary>
        /// <param name="size">Number of bytes to allocate.</param>
        /// <returns>Pointer to the allocated memory buffer. A NULL pointer is returned if there is insufficient memory available. </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* malloc(ulong size) { return CMemory.C_malloc(size); }

        /// <summary>
        /// C-style free. Deallocate a previously allocated memory buffer. 
        /// If buff is NULL, the pointer is ignored and free immediately returns. 
        /// </summary>
        /// <param name="buff">Pointer to a previously allocated memory buffer.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void free(void* buff) { CMemory.C_free(buff); }

        /// <summary>
        /// C-style _aligned_malloc. Allocates memory on a specified alignment boundary.
        /// </summary>
        /// <param name="size">Size of the requested memory allocation.</param>
        /// <param name="alignment">The alignment value, which must be an integer power of 2.</param>
        /// <returns>A pointer to the memory block that was allocated or NULL if the operation failed. The pointer is a multiple of alignment.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* _aligned_malloc(ulong size, ulong alignment) { return CMemory.C_aligned_malloc(size, alignment); }

        /// <summary>
        /// C-style _aligned_malloc. Allocates memory on a specified alignment boundary.
        /// </summary>
        /// <param name="size">Size of the requested memory allocation.</param>
        /// <param name="alignment">The alignment value, which must be an integer power of 2.</param>
        /// <returns>A pointer to the memory block that was allocated or NULL if the operation failed. The pointer is a multiple of alignment.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe void* _aligned_malloc(long size, long alignment) { return CMemory.C_aligned_malloc(size, alignment); }

        /// <summary>
        /// C-style _aligned_free. Frees a block of memory that was allocated with _aligned_malloc or _aligned_offset_malloc.
        /// </summary>
        /// <param name="mem">A pointer to the memory block that was returned to the _aligned_malloc or _aligned_offset_malloc function.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void _aligned_free(void* mem) { CMemory.C_aligned_free(mem); }

        /// <summary>
        /// Aligned memory allocation. Compared with _aligned_malloc, this method zeros out the allocated memory. The size must be 8x bytes.
        /// </summary>
        /// <param name="size">Size of the requested memory allocation.</param>
        /// <param name="alignment">The alignment value, which must be an integer power of 2.</param>
        /// <returns>A pointer to the memory block that was allocated or NULL if the operation failed. The pointer is a multiple of alignment.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void* AlignedAlloc(long size, long alignment) { return CMemory.AlignedAlloc(size, alignment); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void SetWorkingSetProfile(int profile) { CMemory.SetWorkingSetProfile(profile); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SetMemoryAllocationProfile(MemoryAllocationProfile profile) { CMemory.SetMemoryAllocationProfile(profile); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void SetMaxWorkingSet(ulong size) { CMemory.SetMaxWorkingSet(size); }
    }

    internal static unsafe class CMemory
    {
        /// <summary>
        /// Copies a specified number of bytes from a source buffer to a destination buffer.
        /// </summary>
        /// <param name="src">The source buffer.</param>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="count">The number of bytes to copy.</param>
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        public static extern unsafe void Copy(void* src, void* dst, int count);

        /// <summary>
        /// C-style memmove. Copies a specified number of bytes from a source buffer to a destination buffer. 
        /// If some regions of the source and destination buffer overlap, the bytes of the overlapping region are copied before being overwritten.
        /// </summary>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="src">The source buffer.</param>
        /// <param name="count">The number of bytes to copy.</param>
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        public static extern unsafe void* C_memmove(void* dst, void* src, ulong count);

        /// <summary>
        /// C-style memcpy. Copies a specified number of bytes from a source buffer to a destination buffer.
        /// </summary>
        /// <param name="dst">The destination buffer.</param>
        /// <param name="src">The source buffer.</param>
        /// <param name="count">The number of bytes to copy.</param>
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        public static extern unsafe void* C_memcpy(void* dst, void* src, ulong count);

        /// <summary>
        /// C-style memcmp. Compares the specified number of bytes in two buffers and returns a value indicating their relationship.
        /// </summary>
        /// <param name="ptr1">Pointer to the first buffer.</param>
        /// <param name="ptr2">Pointer to the second buffer.</param>
        /// <param name="count">The number of bytes to compare.</param>
        /// <returns>Zero indicates the bytes of two buffers are identical. A value less than zero indicates the first buffer less than the second. Otherwise, a value great than zero is returned.</returns>
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        public static extern unsafe int C_memcmp(void* ptr1, void* ptr2, ulong count);

        /// <summary>
        /// C-style memset. Sets the specified number of bytes in a buffer to the specified value.
        /// </summary>
        /// <param name="buff">Pointer to memory buffer.</param>
        /// <param name="value">Value to set.</param>
        /// <param name="count">The number of bytes to set.</param>
        /// <returns>The pointer of memory buffer is returned.</returns>
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        public static extern unsafe void* C_memset(void* buff, int value, ulong count);

        /// <summary>
        /// C-style realloc. Reallocate a previously allocated memory buffer. The content of the memory buffer is preserved up to the shorter of the old and new buffer. The expanded portion of the new buffer is indeterminate. If size is zero, the previously allocated buffer is freed, NULL is returned. If buff is NULL, it behaves the same way as malloc.
        /// </summary>
        /// <param name="buff">Pointer to a previously allocated memory buffer.</param>
        /// <param name="size">Number of bytes of the reallocated memory buffer.</param>
        /// <returns>A pointer to the reallocated memory buffer.</returns>
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        public static extern unsafe void* C_realloc(void* buff, ulong size);

        /// <summary>
        /// C-style malloc. Allocates specified number of bytes of memory.
        /// </summary>
        /// <param name="size">Number of bytes to allocate.</param>
        /// <returns>Pointer to the allocated memory buffer. A NULL pointer is returned if there is insufficient memory available. </returns>
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        public static extern unsafe void* C_malloc(ulong size);

        /// <summary>
        /// C-style free. Deallocate a previously allocated memory buffer. 
        /// If buff is NULL, the pointer is ignored and free immediately returns. 
        /// </summary>
        /// <param name="buff">Pointer to a previously allocated memory buffer.</param>
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        public static extern unsafe void C_free(void* buff);

        /// <summary>
        /// C-style _aligned_malloc. Allocates memory on a specified alignment boundary.
        /// </summary>
        /// <param name="size">Size of the requested memory allocation.</param>
        /// <param name="alignment">The alignment value, which must be an integer power of 2.</param>
        /// <returns>A pointer to the memory block that was allocated or NULL if the operation failed. The pointer is a multiple of alignment.</returns>
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        public static extern unsafe void* C_aligned_malloc(ulong size, ulong alignment);

        /// <summary>
        /// C-style _aligned_malloc. Allocates memory on a specified alignment boundary.
        /// </summary>
        /// <param name="size">Size of the requested memory allocation.</param>
        /// <param name="alignment">The alignment value, which must be an integer power of 2.</param>
        /// <returns>A pointer to the memory block that was allocated or NULL if the operation failed. The pointer is a multiple of alignment.</returns>
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        public static extern unsafe void* C_aligned_malloc(long size, long alignment);

        /// <summary>
        /// C-style _aligned_free. Frees a block of memory that was allocated with _aligned_malloc or _aligned_offset_malloc.
        /// </summary>
        /// <param name="mem">A pointer to the memory block that was returned to the _aligned_malloc or _aligned_offset_malloc function.</param>
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        public static extern void C_aligned_free(void* mem);

        /// <summary>
        /// Aligned memory allocation. Compared with _aligned_malloc, this method zeros out the allocated memory. The size must be 8x bytes.
        /// </summary>
        /// <param name="size">Size of the requested memory allocation.</param>
        /// <param name="alignment">The alignment value, which must be an integer power of 2.</param>
        /// <returns>A pointer to the memory block that was allocated or NULL if the operation failed. The pointer is a multiple of alignment.</returns>
#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        public static extern void* AlignedAlloc(long size, long alignment);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern void SetWorkingSetProfile(int profile);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern void SetMemoryAllocationProfile(MemoryAllocationProfile profile);

#if !CORECLR
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
#else
        [DllImport(TrinityC.AssemblyName)]
#endif
        internal static extern void SetMaxWorkingSet(ulong size);
    }
}
