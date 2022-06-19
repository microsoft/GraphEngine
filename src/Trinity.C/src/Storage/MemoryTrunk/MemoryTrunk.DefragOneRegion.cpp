// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/MemoryTrunk/MemoryTrunk.h"

namespace Storage
{
    void MemoryTrunk::DefragmentOneRegion(AddressTableEntry* addressTable, int32_t addressTableLength, AddressTableEndPoint endpoint)
    {
        /// Backward index in the address table.
        int32_t bwd_index  = endpoint.bwd_index;
        /// Forward index in the address table.
        int32_t fwd_index = endpoint.fwd_index;

        int32_t AddressTableHead = bwd_index;
        uint32_t hole_right_offset = addressTable[bwd_index].offset;

        int32_t bwd_cell_entry_index = 0; // entry index in MTHash table
        int32_t fwd_cell_entry_index = 0; // entry index in MTHash table
        uint32_t current_hole_size;
        bool done = false;

        while (true)
        {
            /********************** Termination : bwd_index meets fwd_index *****************************/
            if (bwd_index == fwd_index)
            {
                bwd_cell_entry_index = addressTable[bwd_index].index;
                if (hashtable->TryGetEntryLockForDefragment(bwd_cell_entry_index))
                {
                    cellid_t _bwd_cell_id = hashtable->MTEntries[bwd_cell_entry_index].Key;
                    uint32_t _bwd_cell_size = (uint32_t)hashtable->CellSize(bwd_cell_entry_index);
                    int32_t _bwd_cell_offset = hashtable->CellEntries[bwd_cell_entry_index].offset;

                    if ((int32_t)_bwd_cell_size <= 0/*removed*/ || _bwd_cell_offset != addressTable[bwd_index].offset/*moved*/)
                    {
                        addressTable[fwd_index].offset = hole_right_offset;
                        addressTable[fwd_index].offset = addressTable[fwd_index].offset % TrunkLength;
                    }
                    else if (hole_right_offset - _bwd_cell_offset - _bwd_cell_size > 0)
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
            /////////////////////////////////////////////////////////////////////////////////////////////////

            bwd_cell_entry_index = addressTable[bwd_index].index;

            //! First get the bwd cell entry lock
            if (!hashtable->TryGetEntryLockForDefragment(bwd_cell_entry_index))
                goto Defragment_Unlock_And_Exit;

            cellid_t bwd_cell_id = hashtable->MTEntries[bwd_cell_entry_index].Key;
            uint32_t bwd_cell_size = (uint32_t)hashtable->CellSize(bwd_cell_entry_index);
            int32_t bwd_cell_offset = hashtable->CellEntries[bwd_cell_entry_index].offset;

            /******* Skip the cells that have been removed or moved after taking the address table snapshot **********/
            if ((int32_t)bwd_cell_size <= 0 /* removed */ || bwd_cell_offset != addressTable[bwd_index].offset /* moved, or turned into LO */)
            {
                --bwd_index;
                hashtable->ReleaseEntryLock(bwd_cell_entry_index);
                continue;
            }
            ///////////////////////////////////////////////////////////////////////////////////////////////////////////

            /******** skip the first cell *******/
            if (bwd_index == AddressTableHead)
            {
                //check whether it has reserved space
                //int32_t sizeRecord = hashtable->_sizeArray[bwd_cell_entry_index];
                //byte reservation_factor = (byte)(sizeRecord >> 24);
                //int32_t reserved_bytes = 1 << reservation_factor;
                //int32_t occupied_bytes = (reserved_bytes - 1) & sizeRecord;
                //int32_t free_bytes = reserved_bytes - occupied_bytes;

                //if (free_bytes > 0)
                //{
                //    try
                //    {
                //        Memory.memmove(trunkPtr + bwd_cell_offset + free_bytes, trunkPtr + bwd_cell_offset, (uint64_t)bwd_cell_size);
                //    }
                //    catch (Exception)
                //    {
                //    }
                //    hashtable->entries[bwd_cell_entry_index].offset = bwd_cell_offset + free_bytes;
                //    hole_right_offset = (uint32_t)bwd_cell_offset + (uint32_t)free_bytes; // reset the hole_right_offset
                //}

                ////!++ Must reset reserved memory space
                //hashtable->ResetSizeEntryUnsafe(bwd_cell_entry_index);

                hashtable->ReleaseEntryLock(bwd_cell_entry_index);
                --bwd_index;
                continue;
            }
            //////////////////////////////////////////////////

            /********* Not AddressTableHead and not removed and not moved : == = >
            bwd_cell_offset
            |
            +------+----------+-------+----------+
            |      |          |       | 1st Cell |
            +------+----------+-------+----------+
            |
            hole_right_offset
            ***************************************************************************/
            current_hole_size = hole_right_offset - (uint32_t)bwd_cell_offset - bwd_cell_size;
            ///////////////////////////////////////////////////////////////////////////////////

            /*************** Keep moving backward if no memory hole is found *****************/
            if (current_hole_size == 0)
            {
                hole_right_offset -= bwd_cell_size;
                --bwd_index;

                hashtable->ResetSizeEntryUnsafe(bwd_cell_entry_index);
                hashtable->ReleaseEntryLock(bwd_cell_entry_index);
                continue;
            }
            ////////////////////////////////////////////////////////////////////////////////////

            // We start the cell moving from the left tail
            fwd_cell_entry_index = addressTable[fwd_index].index;
            //! Get fwd cell entry lock
            if (hashtable->TryGetEntryLockForDefragment(fwd_cell_entry_index))
            {
                cellid_t fwd_cell_id = hashtable->MTEntries[fwd_cell_entry_index].Key;
                uint32_t fwd_cell_size = (uint32_t)hashtable->CellSize(fwd_cell_entry_index);
                int32_t fwd_cell_offset = hashtable->CellEntries[fwd_cell_entry_index].offset;

                /******************************* Skip the cell that is removed or moved *************************************/
                if ((int32_t)fwd_cell_size <= 0 /* removed */ || fwd_cell_offset != addressTable[fwd_index].offset /* moved */)
                {
                    ++fwd_index;
                    if (fwd_index == bwd_index)
                        done = true;
                }
                ////////////////////////////////////////////////////////////////////////////////////////////

                /**************** Move current fwd cell to the right - most memory hole *******************/
                else if (fwd_cell_size <= current_hole_size)
                {

                    memmove(trunkPtr + (hole_right_offset - fwd_cell_size), trunkPtr + fwd_cell_offset, (uint64_t)fwd_cell_size);


                    hole_right_offset -= (uint32_t)fwd_cell_size;
                    hashtable->CellEntries[fwd_cell_entry_index].offset = (int32_t)hole_right_offset;
                    hashtable->ResetSizeEntryUnsafe(fwd_cell_entry_index); //! Reset size array
                    current_hole_size -= fwd_cell_size;
                    ++fwd_index;
                    if (fwd_index == bwd_index)
                        done = true;
                }
                ////////////////////////////////////////////////////////////////////////////////////////////

                hashtable->ReleaseEntryLock(fwd_cell_entry_index); //! Release fwd cell entry lock
            }

            /************************* Move the bwd cell on the left side to the right side of the memory hole ******************/
            if (current_hole_size > 0 && bwd_cell_size > 0)
            {
                memmove(trunkPtr + (hole_right_offset - bwd_cell_size), trunkPtr + bwd_cell_offset, (uint64_t)bwd_cell_size);
                hashtable->CellEntries[bwd_cell_entry_index].offset = (int32_t)(hole_right_offset - bwd_cell_size);
            }
            //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            hole_right_offset -= bwd_cell_size;

            /****************************** Bug Fix notes ****************************
            * We previously do not have the line below above. A bug scenario would be:
            *
            * +----------+-----------+-----+---------+
            * | fwd cell |  bwd cell |*****|         |
            * +----------+-----------+-----+---------+
            * A          B           C     D         E
            *
            * AB: fwd cell
            * BD: bwd cell
            * BC: occupied cell bytes
            * CD: Reserved free bytes
            *
            * After moving fwd cell to CD, assume AB == CD, then actually no free bytes left for bwd cell.
            * If we do not reset the size array, the bwd cell would still think it has the free bytes (CD).
            * //! When bwd cell adding new stuff, it will overwrite the contents in CD.
            ****************************************************************************/

            //! Must reset reserved memory space
            hashtable->ResetSizeEntryUnsafe(bwd_cell_entry_index);
            //! Release the bwd cell entry lock at last
            hashtable->ReleaseEntryLock(bwd_cell_entry_index);

            if (done)
            {
                addressTable[fwd_index].offset = hole_right_offset % TrunkLength;
                break;
            }
            --bwd_index;
        }

        /*********************************************** Epilog ****************************************************/
    Defragment_Unlock_And_Exit:
        uint64_t mem_to_decommit = 0;
        mem_to_decommit = (uint64_t)((addressTable[fwd_index].offset - committed_tail) & Memory::PAGE_MASK_32);
        if (mem_to_decommit > 0 && committed_tail < TrunkLength)
        {
            BufferedDecommitMemory(trunkPtr + committed_tail, mem_to_decommit);
            //Console::WriteLine("One Region Decommitted {0}", mem_to_decommit);
        }
        committed_tail = addressTable[fwd_index].offset & Memory::PAGE_MASK_32;
        ////////////////////////////////////////////////////////////////////////////////////////////////
    }
}
