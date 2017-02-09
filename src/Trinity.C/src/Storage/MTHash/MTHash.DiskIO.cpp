// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "Storage/MTHash/MTHash.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

using namespace Trinity::IO;
namespace Storage
{
    bool MTHash::Save(const String& output_file)
    {
        BinaryWriter bw(output_file);
        bool success = true;

        success = success && bw.Write((char)VERSION);                  // 1B
        success = success && bw.Write(NonEmptyEntryCount);             // 4B
        success = success && bw.Write(FreeEntryCount);                 // 4B
        success = success && bw.Write(FreeEntryList);                  // 4B
        success = success && bw.Write(BucketCount);                    // 4B
        success = success && bw.Write(EntryCount);                     // 4B

        int32_t mtentry_array_length = (int32_t)EntryCount << 4;
        int32_t long_array_length    = (int32_t)EntryCount << 3;
        int32_t int_array_length     = (int32_t)EntryCount << 2;
        int32_t ushort_array_length  = (int32_t)EntryCount << 1;

        uint32_t buffer_size = mtentry_array_length > (BucketCount << 2) ? mtentry_array_length : (BucketCount << 2);
        char* buff = new char[buffer_size];

        // Write buckets array
        memcpy(buff, Buckets, (int32_t)BucketCount << 2);
        success = success && bw.Write(buff, 0, (int32_t)BucketCount << 2); // note here we write more

        // Write entry array
        memcpy(buff, CellEntries, long_array_length);
#pragma region Modify the offset in the entries
        char* p = buff;
        if (memory_trunk->committed_tail < memory_trunk->head.committed_head ||
            (memory_trunk->committed_tail == memory_trunk->head.committed_head && memory_trunk->committed_tail == 0)
            )//one segment
        {
            CellEntry* entryPtr    = (CellEntry*)p;
            CellEntry* entryEndPtr = entryPtr + NonEmptyEntryCount;
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
            CellEntry* entryEndPtr = entryPtr + NonEmptyEntryCount;
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
        success = success && bw.Write(buff, 0, long_array_length);

        memcpy(buff, MTEntries, mtentry_array_length);
        success = success && bw.Write(buff, 0, mtentry_array_length);
        delete[] buff;

        return success;
    }

    bool MTHash::Reload(const String& input_file)
    {
        BinaryReader br(input_file);
        if (!br.Good())
            return false;

        DeallocateMTHash(/** is_dtor: */false);

        /*********** Read meta data ************/
        int32_t version = (int32_t)br.ReadChar();

        if (version != VERSION)
        {
            Trinity::Diagnostics::FatalError("The Trinity disk image version does not match.");
        }

        NonEmptyEntryCount = br.ReadInt32();
        FreeEntryCount     = br.ReadInt32();
        FreeEntryList      = br.ReadInt32();

        if (BucketCount != br.ReadUInt32())
        {
            Trinity::Diagnostics::FatalError("The Trinity disk image is invalid.");
        }

        EntryCount = br.ReadUInt32();

        uint32_t mtentry_array_length = (uint32_t)EntryCount << 4;
        uint32_t long_array_length    = (uint32_t)EntryCount << 3;
        uint32_t int_array_length     = (uint32_t)EntryCount << 2;
        uint32_t ushort_array_length  = (uint32_t)EntryCount << 1;
        /////////////////////////////////////////////////////////

        AllocateMTHash();

        uint32_t buffer_size = mtentry_array_length > (BucketCount << 2) ? mtentry_array_length : (BucketCount << 2);
        char* buff = new char[buffer_size];
        bool  read_success = true;

        // Read buckets array
        read_success = br.Read(buff, 0, (int32_t)BucketCount << 2) && read_success;
        memcpy(Buckets, buff, (int32_t)BucketCount << 2);

        // Read entry array
        read_success = br.Read(buff, 0, long_array_length) && read_success; 
        memcpy(CellEntries, buff, long_array_length);

        // Read entry array
        read_success = br.Read(buff, 0, mtentry_array_length) && read_success;
        memcpy(MTEntries, buff, mtentry_array_length);
        delete[] buff;
        return true;
    }
}