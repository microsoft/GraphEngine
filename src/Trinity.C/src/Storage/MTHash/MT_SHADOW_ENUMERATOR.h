#pragma once
#include "TrinityCommon.h"

#include <vector>
#include "CellEntry.h"

namespace Storage
{
    class MTHash;
    /// Copies a MTHash into the shadow and iterate.
    class MT_SHADOW_ENUMERATOR
    {
    private:
#pragma region fields
        char*       m_CellPtr;
        cellid_t    m_CellId;
        int32_t     m_CellSize;
        int32_t     m_CellEntryIndex;
        uint16_t    m_CellType;
        std::vector<cellid_t> m_cellids;
        std::vector<cellid_t>::iterator m_iter;
        std::vector<cellid_t>::iterator m_end;
#pragma endregion

        void _releaseCellLock();

    public:
        inline MT_SHADOW_ENUMERATOR() { m_CellEntryIndex = -1; }
        // Caller guarantees that MTHash is locked.
        void Initialize(MTHash* mth);

        inline void Invalidate()
        {
            if (m_CellEntryIndex != -1)
            {
                _releaseCellLock();
            }

            m_cellids.clear();
            m_iter           = m_cellids.end();
            m_end            = m_cellids.end();
        }
        TrinityErrorCode MoveNext();

        inline char* CellPtr()
        {
            return m_CellPtr;
        }
        inline cellid_t CellId()
        {
            return m_CellId;
        }
        inline int32_t CellSize()
        {
            return m_CellSize;
        }
        inline uint16_t CellType()
        {
            return m_CellType;
        }
        inline int32_t CellEntryIndex()
        {
            return m_CellEntryIndex;
        }
    };
}
