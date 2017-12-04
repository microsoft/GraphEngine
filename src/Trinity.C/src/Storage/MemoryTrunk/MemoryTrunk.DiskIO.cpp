// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"
#include <Utility/FileIO.h>
#include <fstream>
#include <io>
#include <diagnostics>

namespace Storage
{
    using namespace Trinity::IO;
    bool MemoryTrunk::Save(String storage_root)
    {
        bool success = true;
        ShrinkLOContainer();
        if (storage_root.Empty())
        {
            if (!SaveMP())
            {
                Diagnostics::WriteLine(LogLevel::Debug, "Error saving MP file on trunk {0}", TrunkId);
                success = false;
            }
            if (!SaveIndex())
            {
                Diagnostics::WriteLine(LogLevel::Debug, "Error saving index file on trunk {0}", TrunkId);
                success = false;
            }
            if (!SaveLOFile())
            {
                Diagnostics::WriteLine(LogLevel::Debug, "Error saving LO file on trunk {0}", TrunkId);
                success = false;
            }
        }
        else
        {
            String root = Path::CompletePath(Path::CompletePath(storage_root, false) + "MemoryPool\\", true);
            if (!SaveMP(root + TrunkId + ".mp"))
            {
                Diagnostics::WriteLine(LogLevel::Debug, "Error saving MP file on trunk {0}", TrunkId);
                success = false;
            }
            if (!SaveIndex(root + TrunkId + ".index"))
            {
                Diagnostics::WriteLine(LogLevel::Debug, "Error saving index file on trunk {0}", TrunkId);
                success = false;
            }
            if (!SaveLOFile(root + TrunkId + ".lo"))
            {
                Diagnostics::WriteLine(LogLevel::Debug, "Error saving LO file on trunk {0}", TrunkId);
                success = false;
            }
        }
        return success;
    }

    bool MemoryTrunk::Load(String storage_root)
    {
        bool success = true;
        if (storage_root.Empty())
        {
            if (!LoadMP())
            {
                Diagnostics::WriteLine(LogLevel::Debug, "Error loading MP file on trunk {0}", TrunkId);
                success = false;
            }
            if (!LoadIndex())
            {
                Diagnostics::WriteLine(LogLevel::Debug, "Error loading index file on trunk {0}", TrunkId);
                success = false;
            }
            if (!LoadLOFile())
            {
                Diagnostics::WriteLine(LogLevel::Debug, "Error loading LO file on trunk {0}", TrunkId);
                success = false;
            };
        }
        else
        {
            String root = Path::CompletePath(Path::Combine(Path::CompletePath(storage_root, false), "MemoryPool"), true);
            if (!LoadMP(root + TrunkId + ".mp"))
            {
                Diagnostics::WriteLine(LogLevel::Debug, "Error loading MP file on trunk {0}", TrunkId);
                success = false;
            }
            if (!LoadIndex(root + TrunkId + ".index"))
            {
                Diagnostics::WriteLine(LogLevel::Debug, "Error loading index file on trunk {0}", TrunkId);
                success = false;
            }
            if (!LoadLOFile(root + TrunkId + ".lo"))
            {
                Diagnostics::WriteLine(LogLevel::Debug, "Error loading LO file on trunk {0}", TrunkId);
                success = false;
            };
        }
        return success;
    }

    bool MemoryTrunk::SaveMP(String mp_file)
    {
        /* Save to secondary slot, overwrite the older image */
        String output_file = MPFile(false);
        if (mp_file.Length() != 0)
        {
            output_file = mp_file;
        }

        HeadGroup head_shadow;
        head_shadow.head_group.store(head.head_group.load());
        bool success = true;

        if ((committed_tail < head_shadow.committed_head) ||
            (committed_tail == head_shadow.committed_head && committed_tail == 0))
        {
            success = success && FileIO::WriteBufferToFile(output_file.ToWcharArray(), trunkPtr + committed_tail, (uint64_t)head.append_head - committed_tail);
        }
        else
        {
            //Two region
            success = success && FileIO::WriteBufferToFile(output_file.ToWcharArray(), trunkPtr, head.append_head);
            success = success && FileIO::AppendBufferToFile(output_file.ToWcharArray(), trunkPtr + committed_tail, TrunkLength - committed_tail);
        }

        return success;
    }

    bool MemoryTrunk::LoadMP(String mp_file)
    {
        /* Load from primary slot, reading the newer image */
        String input_file = MPFile(true);
        if (mp_file.Length() != 0)
        {
            input_file = mp_file;
        }

        bool success = true;

        if (!File::Exists(input_file))
        {
            head.append_head = 0;
            head.committed_head = 0;
        }
        else
        {
            auto fname_arr = input_file.ToWcharArray();
            FILE* fp = (FILE*)FileIO::OpenFile4Read(fname_arr);
            if (!fp) { success = false; }
            else
            {
                int64_t size = FileIO::GetFileSize(fp);
                success = success && (size >= 0);
                if (success)
                {
                    DisposeTrunkBuffer();
                    AllocateTrunk(trunkPtr, size, MTHash::PhysicalMemoryLocking);// <---head.committed_head and committed_tail are set in this routine
                    success = success && FileIO::ReadBuffer(fp, trunkPtr, size);
                    head.append_head = (uint32_t)size;
                }
                FileIO::CloseFileHandle(fp);
            }
        }
        return success;
    }

    bool MemoryTrunk::SaveIndex(String index_file)
    {
        /* Save to secondary slot, overwrite the older image */
        String output_file = IndexFile(false);
        if (index_file.Length() != 0)
        {
            output_file = index_file;
        }

        return hashtable->Save(output_file);
    }

    bool MemoryTrunk::LoadIndex(String index_file)
    {
        /* Load from primary slot, reading the newer image */
        String input_file = IndexFile(true);
        if (index_file.Length() != 0)
        {
            input_file = index_file;
        }

        return hashtable->Initialize(input_file, this);
    }

    bool MemoryTrunk::SaveLOFile(String lo_file)
    {
        /* Save to secondary slot, overwrite the older image */
        String output_file = LO_File(false);
        if (lo_file.Length() != 0)
        {
            output_file = lo_file;
        }

        bool success = true;

        if (LOCount > 0)
        {
            void* fp = FileIO::OpenFile4Write(output_file.ToWcharArray());

            if (fp)
            {
                success = success && FileIO::WriteInt(fp, LOCount);
                MT_ENUMERATOR it(hashtable);
                while (it.MoveNext() == TrinityErrorCode::E_SUCCESS)
                {
                    char* cellPtr = it.LOCellPtr();
                    if (cellPtr != nullptr)
                    {
                        success = success && FileIO::WriteInt(fp, it.CellSize());
                        success = success && FileIO::WriteBuffer(fp, cellPtr, (size_t)it.CellSize());
                    }
                }
                FileIO::CloseFileHandle(fp);
            }
            else
            {
                success = false;
            }
        }
        else
        {
#if defined(TRINITY_PLATFORM_WINDOWS)
            _wremove(output_file.ToWcharArray());
#else
            remove(output_file.c_str());
#endif
        }

        return success;
    }

#pragma warning(push)
#pragma warning(disable:6385)
#pragma warning(disable:6386)
    bool MemoryTrunk::LoadLOFile(String lo_file)
    {
        DisposeLargeObjects();
        /* Load from primary slot, reading the newer image */
        String input_file = LO_File(true);
        bool read_success = true;
        if (lo_file.Length() != 0)
        {
            input_file = lo_file;
        }

        if (File::Exists(input_file))
        {
            void* fp = FileIO::OpenFile4Read(input_file.ToWcharArray());
            LOCount = FileIO::ReadInt(fp);
            EnsureLOCapacity(LOCount);
            LOPtrs = new char*[LOCapacity];
            LOPreservedSizeArray = new int32_t[LOCapacity];
            LOIndex = LOCount + 1;

            MT_ENUMERATOR it(hashtable);

            while (it.MoveNext() == TrinityErrorCode::E_SUCCESS)
            {
                int32_t lo_idx = it.CellEntryPtr->offset;
                if (lo_idx < 0)
                {
                    lo_idx = -lo_idx;
                    int32_t lo_size = FileIO::ReadInt(fp);
                    LOPtrs[lo_idx] = (char*)AllocateLargeObject(lo_size);
                    LOPreservedSizeArray[lo_idx] = 0;
                    read_success = FileIO::ReadBuffer(fp, LOPtrs[lo_idx], (size_t)lo_size) && read_success;
                }
            }

            FileIO::CloseFileHandle(fp);
        }
        else
        {
            InitLOContainer();
        }
        return read_success;
    }
#pragma warning(pop)
}
