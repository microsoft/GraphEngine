// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

namespace Storage
{
    void MemoryTrunk::DefragmentTwoRegion(AddressTableEntry* addressTable, int32_t addressTableLength, AddressTableEndPoint endpoint)
    {
        /// Backward index in the address table.
        int32_t bwd_index = endpoint.bwd_index;
        /// Forward index in the address table.
        int32_t fwd_index = endpoint.fwd_index;

        int32_t AddressTableHead = bwd_index;
        uint32_t hole_right_offset = addressTable[bwd_index].offset;

        int32_t bwd_cell_entry_index = 0;
        int32_t fwd_cell_entry_index = 0;
        uint32_t hole_size;
        bool done = false;

        while (true)
        {
            /************ Termination : bwd_index meets fwd_index ***************/
            if (bwd_index == fwd_index)
            {
                bwd_cell_entry_index = addressTable[bwd_index].index;
                if (hashtable->TryGetEntryLockForDefragment(bwd_cell_entry_index))
                {
                    cellid_t _bwd_cell_id = hashtable->MTEntries[bwd_cell_entry_index].Key;
                    uint32_t _bwd_cell_size = (uint32_t)hashtable->CellSize(bwd_cell_entry_index);
                    int32_t _bwd_cell_offset = hashtable->CellEntries[bwd_cell_entry_index].offset;

                    if ((int32_t)_bwd_cell_size <= 0 /*removed*/ || _bwd_cell_offset != addressTable[bwd_index].offset/*moved*/)
                    {
                        addressTable[fwd_index].offset = hole_right_offset;
                        addressTable[fwd_index].offset = addressTable[fwd_index].offset % TrunkLength;
                    }
                    else if (
                        (hole_right_offset >= (uint32_t)_bwd_cell_offset && (hole_right_offset - _bwd_cell_offset - _bwd_cell_size > 0)) ||
                        (hole_right_offset < (uint32_t)_bwd_cell_offset && hole_right_offset >= _bwd_cell_size)
                        )
                    {
                        if (_bwd_cell_size > 0)
                        {
                            memmove(trunkPtr + (hole_right_offset - _bwd_cell_size), trunkPtr + _bwd_cell_offset, (uint64_t)_bwd_cell_size);
                            hashtable->CellEntries[bwd_cell_entry_index].offset = (int32_t)(hole_right_offset - _bwd_cell_size);
                        }
                        addressTable[fwd_index].offset = hole_right_offset - _bwd_cell_size;
                    }

                    hashtable->ResetSizeEntryUnsafe(bwd_cell_entry_index);
                    hashtable->ReleaseEntryLock(bwd_cell_entry_index);
                }
                goto Defragment_Unlock_And_Exit;
            }
            ////////////////////////////////////////////////////////////////////////////

            bwd_cell_entry_index = addressTable[bwd_index].index;

            //! First get the bwd cell entry lock
            if (!hashtable->TryGetEntryLockForDefragment(bwd_cell_entry_index))
                goto Defragment_Unlock_And_Exit;

            cellid_t bwd_cell_id = hashtable->MTEntries[bwd_cell_entry_index].Key;
            uint32_t bwd_cell_size = (uint32_t)hashtable->CellSize(bwd_cell_entry_index);
            int32_t bwd_cell_offset = hashtable->CellEntries[bwd_cell_entry_index].offset;

            /***** Skip the cells that have been removed or moved after taking the address table snapshot ******/
            if ((int32_t)bwd_cell_size <= 0 /* removed */ || bwd_cell_offset != addressTable[bwd_index].offset /* moved */)
            {
                hashtable->ReleaseEntryLock(bwd_cell_entry_index); //! Release the entry lock before continuing
                if (--bwd_index < 0)
                {
                    bwd_index += addressTableLength; //warped to the back
                }
                continue;
            }
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            /************** Skip the first bwd cell *****************/
            if (bwd_index == AddressTableHead)
            {
                hashtable->ReleaseEntryLock(bwd_cell_entry_index); //! Release the entry lock before continuing
                if (--bwd_index < 0)
                {
                    bwd_index += addressTableLength;//warped to the back
                }
                continue;
            }
            //////////////////////////////////////////////////////////

            // Not AddressTableHead and not removed or moved
            if (hole_right_offset >= (uint32_t)bwd_cell_offset) // on the same side
            {
                hole_size = hole_right_offset - (uint32_t)bwd_cell_offset - bwd_cell_size;
            }
            else // memory hole and bwd_cell are on two sides
            {
                hole_size = hole_right_offset;
                if (hole_size == 0)
                {
                    hole_right_offset = TrunkLength;
                    hole_size = hole_right_offset - (uint32_t)bwd_cell_offset - bwd_cell_size;
                }
            }

            if (hole_size == 0)
            {
                hashtable->ResetSizeEntryUnsafe(bwd_cell_entry_index);
                hashtable->ReleaseEntryLock(bwd_cell_entry_index);
                hole_right_offset -= bwd_cell_size;
                if (hole_right_offset == 0)
                {
                    hole_right_offset = TrunkLength;
                }
                if (--bwd_index < 0)
                {
                    bwd_index += addressTableLength;//warped to the back
                }
                continue;
            }

            fwd_cell_entry_index = addressTable[fwd_index].index;
            if (hashtable->TryGetEntryLockForDefragment(fwd_cell_entry_index))
            {
                cellid_t fwd_cell_id = hashtable->MTEntries[fwd_cell_entry_index].Key;
                uint32_t fwd_cell_size = (uint32_t)hashtable->CellSize(fwd_cell_entry_index);
                int32_t fwd_cell_offset = hashtable->CellEntries[fwd_cell_entry_index].offset;

                if ((int32_t)fwd_cell_size <= 0 || fwd_cell_offset != addressTable[fwd_index].offset)
                {
                    fwd_index = (fwd_index + 1) % addressTableLength;
                    if (fwd_index == bwd_index)
                        done = true;
                }
                else if (fwd_cell_size <= hole_size)
                {
                    if (fwd_cell_size > 0)
                    {
                        memmove(trunkPtr + (hole_right_offset - fwd_cell_size), trunkPtr + fwd_cell_offset, (uint64_t)fwd_cell_size);
                        hashtable->CellEntries[fwd_cell_entry_index].offset = (int32_t)(hole_right_offset - fwd_cell_size);
                        hashtable->ResetSizeEntryUnsafe(fwd_cell_entry_index); //! Reset size array
                        hole_right_offset -= (uint32_t)fwd_cell_size;
                        if (hole_right_offset == 0)
                            hole_right_offset = TrunkLength;
                    }
                    fwd_index = (fwd_index + 1) % addressTableLength;
                    if (fwd_index == bwd_index)
                        done = true;
                }
                hashtable->ReleaseEntryLock(fwd_cell_entry_index); //! Release fwd cell entry lock
            }

            if ((uint32_t)bwd_cell_offset > hole_right_offset)//bwd_cell is in front region , hole is in back region
            {
                //so that [0..hole_r_offset] is the hole
                //length == hole_r_offset
                if (bwd_cell_size > hole_right_offset)//the hole cannot hold the bwd_cell
                {
                    hole_right_offset = TrunkLength;
                    hole_size = hole_right_offset - (uint32_t)bwd_cell_offset - bwd_cell_size;
                }
                else
                {
                    hole_size = hole_right_offset;
                }
            }
            else//bwd_cell and hole is in the same region
            {
                hole_size = hole_right_offset - (uint32_t)bwd_cell_offset - bwd_cell_size;
            }

            if (hole_size > 0 && bwd_cell_size > 0)
            {
                memmove(trunkPtr + (hole_right_offset - bwd_cell_size), trunkPtr + bwd_cell_offset, (uint64_t)bwd_cell_size);
                hashtable->CellEntries[bwd_cell_entry_index].offset = (int32_t)(hole_right_offset - bwd_cell_size);
            }
            hole_right_offset -= bwd_cell_size;
            if (hole_right_offset == 0)
                hole_right_offset = TrunkLength;

            hashtable->ResetSizeEntryUnsafe(bwd_cell_entry_index);
            hashtable->ReleaseEntryLock(bwd_cell_entry_index);
            if (done)
            {
                addressTable[fwd_index].offset = hole_right_offset % TrunkLength;
                break;
            }
            if (--bwd_index < 0)
                bwd_index += addressTableLength;
        }

        /******************************************* Epilog ****************************************************/
    Defragment_Unlock_And_Exit:
        uint64_t mem_to_decommit = 0;
        if (addressTable[fwd_index].offset >= committed_tail)
        {
            mem_to_decommit = (uint64_t)(addressTable[fwd_index].offset - committed_tail) & Memory::PAGE_MASK;
            if (mem_to_decommit > 0 && committed_tail < TrunkLength)
                BufferedDecommitMemory(trunkPtr + committed_tail, mem_to_decommit);
        }
        else
        {
            mem_to_decommit = (TrunkLength - committed_tail) & Memory::PAGE_MASK;

            if (mem_to_decommit > 0)
                BufferedDecommitMemory(trunkPtr + committed_tail, mem_to_decommit);

            //Console::WriteLine("Two Region Decommitted {0}", mem_to_decommit);

            mem_to_decommit = (uint64_t)(addressTable[fwd_index].offset & Memory::PAGE_MASK);

            if (mem_to_decommit > 0)
                BufferedDecommitMemory(trunkPtr, mem_to_decommit);
            //Console::WriteLine("Two Region Decommitted {0}", mem_to_decommit);
        }

        committed_tail = addressTable[fwd_index].offset & Memory::PAGE_MASK;
        /////////////////////////////////////////////////////////////////////////////////////////////////////////
    }
}