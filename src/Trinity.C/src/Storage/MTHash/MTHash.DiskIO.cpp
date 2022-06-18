// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Storage/MTHash/MTHash.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

#pragma warning( disable : 4244 )

using namespace Trinity::IO;
namespace Storage
{
    bool MTHash::Save(const String& output_file)
    {
        BinaryWriter bw(output_file);
        bool success = true;

        success = success && bw.Write((char)VERSION);                    // 1B
        success = success && bw.Write(ExtendedInfo->NonEmptyEntryCount); // 4B
        success = success && bw.Write(ExtendedInfo->FreeEntryCount);     // 4B
        success = success && bw.Write(ExtendedInfo->FreeEntryList);      // 4B
        success = success && bw.Write(BucketCount);                      // 4B
        success = success && bw.Write(ExtendedInfo->EntryCount);         // 4B

        int32_t mtentry_array_length   = (int32_t)ExtendedInfo->EntryCount * szMTEntry();
        int32_t cellentry_array_length = (int32_t)ExtendedInfo->EntryCount * szCellEntry();
        int32_t bucket_array_length    = (int32_t)BucketCount * szBucket();

        // Write buckets array
        success = success && bw.Write((char*)Buckets, 0, bucket_array_length); // note here we write more

        // Write entry array

        char* buff = new char[cellentry_array_length];
        memcpy(buff, CellEntries, cellentry_array_length);
#pragma region Modify the offset in the entries
        char* p = buff;
        if (memory_trunk->committed_tail < memory_trunk->head.committed_head ||
            (memory_trunk->committed_tail == memory_trunk->head.committed_head && memory_trunk->committed_tail == 0)
            )//one segment
        {
            CellEntry* entryPtr    = (CellEntry*)p;
            CellEntry* entryEndPtr = entryPtr + ExtendedInfo->NonEmptyEntryCount;
            for (; entryPtr != entryEndPtr; ++entryPtr)
            {
                if (entryPtr->offset > 0 && entryPtr->size > 0)
                {
                    entryPtr->offset -= (int32_t)memory_trunk->committed_tail;
                }
            }
        }
        else//two segments
        {
            CellEntry* entryPtr    = (CellEntry*)p;
            CellEntry* entryEndPtr = entryPtr + ExtendedInfo->NonEmptyEntryCount;
            for (; entryPtr != entryEndPtr; ++entryPtr)
            {
                if (entryPtr->offset >= (int32_t)memory_trunk->head.committed_head && entryPtr->size > 0)
                {
                    entryPtr->offset -=
                        ((int32_t)memory_trunk->committed_tail - (int32_t)memory_trunk->head.append_head);
                }
            }
        }

#pragma endregion
        success = success && bw.Write(buff, 0, cellentry_array_length);
        delete[] buff;

        success = success && bw.Write((char*)MTEntries, 0, mtentry_array_length);

        return success;
    }

    bool MTHash::Reload(const String& input_file)
    {
        BinaryReader br(input_file);
        if (!br.Good())
            return false;

        DeallocateMTHash();

        /*********** Read meta data ************/
        int32_t version = (int32_t)br.ReadChar();

        if (version != VERSION)
        {
            Trinity::Diagnostics::FatalError("The Trinity disk image version does not match.");
        }

        ExtendedInfo->NonEmptyEntryCount = br.ReadInt32();
        ExtendedInfo->FreeEntryCount     = br.ReadInt32();
        ExtendedInfo->FreeEntryList      = br.ReadInt32();

        if (BucketCount != br.ReadUInt32())
        {
            Trinity::Diagnostics::FatalError("The Trinity disk image is invalid.");
        }

        ExtendedInfo->EntryCount = br.ReadUInt32();

        int32_t mtentry_array_length   = (int32_t)ExtendedInfo->EntryCount * szMTEntry();
        int32_t cellentry_array_length = (int32_t)ExtendedInfo->EntryCount * szCellEntry();
        int32_t bucket_array_length    = (int32_t)BucketCount * szBucket();
        /////////////////////////////////////////////////////////

        AllocateMTHash();

        bool read_success = true;

        read_success = read_success && br.Read((char*)Buckets, 0, bucket_array_length);
        read_success = read_success && br.Read((char*)CellEntries, 0, cellentry_array_length);
        read_success = read_success && br.Read((char*)MTEntries, 0, mtentry_array_length);

        return read_success;
    }
}
