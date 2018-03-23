#include "MT_SHADOW_ENUMERATOR.h"
#include "MT_ENUMERATOR.h"
#include "Storage/LocalStorage/LocalMemoryStorage.h"
#include "MTHash.h"

namespace Storage
{
    void MT_SHADOW_ENUMERATOR::_releaseCellLock()
    {
        LocalMemoryStorage::ReleaseCellLock(m_CellId, m_CellEntryIndex);
        m_CellEntryIndex = -1;
    }

    void MT_SHADOW_ENUMERATOR::Initialize(MTHash* mth)
    {
        Invalidate();
        MT_ENUMERATOR it(mth);
        while (TrinityErrorCode::E_SUCCESS == it.MoveNext())
        {
            m_cellids.push_back(it.CellId());
        }
        m_iter = m_cellids.begin();
        m_end  = m_cellids.end();
    }


    TrinityErrorCode MT_SHADOW_ENUMERATOR::MoveNext()
    {
        if (m_CellEntryIndex != -1)
        {
            _releaseCellLock();
            ++m_iter;
        }

        if (m_iter == m_end)
        {
            Invalidate();
            return TrinityErrorCode::E_ENUMERATION_END;
        }
        else
        {
            m_CellId = *m_iter;
            return LocalMemoryStorage::CGetLockedCellInfo4CellAccessor(m_CellId, m_CellSize, m_CellType, m_CellPtr, m_CellEntryIndex);
        }
    }
}
