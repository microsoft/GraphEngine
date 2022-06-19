// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Storage/LocalStorage/LocalMemoryStorage.h"
#include "Storage/LocalStorage/GCTask.h"
#include "BackgroundThread/BackgroundThread.h"
#include <mutex>

#define EXTEND_SHIFT_LEFT(x, cnt) (((int64_t)x) << cnt)

namespace Storage
{
    namespace LocalMemoryStorage
    {
        namespace Logging
        {
            static std::mutex              s_write_ahead_log_mutex;
            static FILE*                   s_write_ahead_log_fp = nullptr;
            static const CellAccessOptions c_WALFlags = (CellAccessOptions) (CellAccessOptions::StrongLogAhead | CellAccessOptions::WeakLogAhead);

            static uint8_t compute_checksum_impl(LOG_RECORD_HEADER* plog, char* bufferPtr)
            {
                int64_t  sum  = plog->CELL_ID + plog->CONTENT_LEN + EXTEND_SHIFT_LEFT(plog->CELL_TYPE ,32);
                int64_t  s2   = 0;
                int32_t  len  = plog->CONTENT_LEN;

                while (len > 0)
                {
                    switch (len)
                    {
                    default:
                    case 8:
                        s2        += *(int64_t*) bufferPtr;
                        bufferPtr += sizeof(int64_t);
                        len       -= 8;
                        break;
                    case 7:
                    case 6:
                    case 5:
                    case 4:
                        s2        += *(int32_t*) bufferPtr;
                        bufferPtr += sizeof(int32_t);
                        len       -= 4;
                        break;
                    case 3:
                    case 2:
                        s2        += *(int16_t*) bufferPtr;
                        bufferPtr += sizeof(int16_t);
                        len       -= 2;
                        break;
                    case 1:
                        s2        += *(int8_t*) bufferPtr;
                        bufferPtr += 1;
                        len       -= 1;
                        break;
                    }

                    sum += s2;
                }

                /* Fold sum into a byte */

                sum += sum >> 32;
                sum += sum >> 16;
                sum += sum >> 8;

                return (uint8_t) sum;
            }

            void ComputeChecksum(PLOG_RECORD_HEADER plog, char* bufferPtr)
            {
                plog->CHECKSUM = compute_checksum_impl(plog, bufferPtr);
            }

            bool ValidateChecksum(PLOG_RECORD_HEADER plog, char* content)
            {
                return plog->CHECKSUM == compute_checksum_impl(plog, content);
            }
            /// <summary>
            /// Log a cell action to persistent storage.
            /// </summary>
            /// <param name="cellPtr">If null, it indicates a RemoveCell action.</param>
            /// <param name="cellSize">If less than 0, it indicates a RemoveCell action.</param>
            /// <param name="options"></param>
            void WriteAheadLog(cellid_t cellId, char* cellPtr, int32_t cellSize, uint16_t cellType, CellAccessOptions options)
            {
                std::lock_guard<std::mutex> lock(s_write_ahead_log_mutex);

                if ((options & c_WALFlags) == CellAccessOptions::None)
                    /* No logging options. */
                    return;

                if (s_write_ahead_log_fp == nullptr)
                    /* The log file is somehow absent... We should quit now. */
                    return;

                LOG_RECORD_HEADER record_header =
                {
                    cellId,
                    cellSize,
                    cellType,
                    0, // CHECKSUM to be computed
                };

                if (cellPtr == nullptr || cellSize < 0)
                {
                    /* This is a RemoveCell action, calibrate the header */
                    record_header.CONTENT_LEN = -1;
                }

                ComputeChecksum(&record_header, cellPtr);

                fwrite(&record_header, sizeof(LOG_RECORD_HEADER), 1, s_write_ahead_log_fp);

                if (record_header.CONTENT_LEN >= 0)
                {
                    // This is a SaveCell/UpdateCell/UseCell action
                    fwrite(cellPtr, cellSize, 1, s_write_ahead_log_fp);
                }

                if ((options & CellAccessOptions::WeakLogAhead) == CellAccessOptions::None)
                {
                    if (0 != fflush(s_write_ahead_log_fp))
                    {
                        Trinity::Diagnostics::WriteLine(LogLevel::Error, "Failed to flush the log file");
                    }
                }
            }

            void SetWriteAheadLogFile(FILE* fp)
            {
                s_write_ahead_log_mutex.lock();

                s_write_ahead_log_fp = fp;

                s_write_ahead_log_mutex.unlock();
            }
        }
    }
}
