#pragma once
#include "TrinityCommon.h"
#include <unordered_set>
#include <vector>

//method annotation helpers

// !Annotated methods may allocate a transaction context
#define ALLOC_THREAD_CTX

namespace Storage
{
    struct CellIdCollection
    {
        cellid_t                m_inplace;
        bool                    m_inplace_taken = false;
        std::vector<cellid_t>   m_collection;

        inline void Add(const cellid_t &cellId)
        {
            if (!m_inplace_taken)
            {
                m_inplace_taken = true;
                m_inplace = cellId;
            }
            else
            {
                m_collection.push_back(cellId);
            }
        }

        inline void Remove(const cellid_t &cellId)
        {
            if (m_inplace_taken && m_inplace == cellId)
            {
                m_inplace_taken = false;
            }
            else
            {
                m_collection.erase(std::remove(m_collection.begin(), m_collection.end(), cellId), m_collection.end());
            }
        }

        inline std::vector<cellid_t> ToList() const
        {
            std::vector<cellid_t> ret;
            if (m_inplace_taken)ret.push_back(m_inplace);
            ret.insert(ret.end(), m_collection.cbegin(), m_collection.cend());
            return ret;
        }

        inline size_t size() const
        {
            return (m_inplace_taken ? 1 : 0) + m_collection.size();
        }

        inline bool Contains(const cellid_t &cellId) const
        {
            if (m_inplace_taken && m_inplace == cellId) return true;
            return std::find(m_collection.begin(), m_collection.end(), cellId) != m_collection.end();
        }

        template<typename TFunc> inline bool Any(const TFunc& func) const
        {
            if (m_inplace_taken && func(m_inplace)) return true;
            return std::any_of(m_collection.begin(), m_collection.end(), func);
        }

        template<typename TFunc> inline void ForEach(const TFunc& func) const
        {
            if (m_inplace_taken) { func(m_inplace); }
            std::for_each(m_collection.begin(), m_collection.end(), func);
        }
    };
    //  !Note:
    //  The following paragraph applies when a managed thread is not backed
    //  with a dedicated physical thread (which turns out is not the case
    //  for the current CLR implementation). 
    //  See https://github.com/dotnet/coreclr/blob/master/Documentation/botr/threading.md
    //  for more details.
    //  ***
    //  When entering unmanaged code from C#, it is not guaranteed that
    //  the unmanaged thread local storage (TLS) will persist for a managed
    //  thread, because a managed thread is a virtual thread, that might be
    //  backed by multiple physical threads, and one physical thread might
    //  back multiple managed threads. Thus, each time we enter the code that
    //  a thread context is involved, we have to pass it in as a parameter, and
    //  overwrite the unmanaged TLS.
    //  ***
    typedef struct
    {
        //  A cell id that the current operation attempts to lock, 
        //  or the lock is held, and the thread is resizeing the cell.
        //  This field is set by all local memory storage ops except
        //  the whole db operations.
        //  Note:
        //  LockingCell + LockingMTHash        = INVALID
        //  LockingCell + LockingStorage       = INVALID
        //  LockingCell + ReloadingMemoryTrunk = VALID
        //  LockingCell alone: it means that the cell entry lock is not
        //  taken yet. The associated bucket lock status is undetermined.
        //  That being said, LockingMTHash/LockingStorage should never
        //  be propagated back to the user space, because the user code
        //  could decide to acquire a cell lock (toggling LockingCell).
        cellid_t                     LockingCell;    
        //  A set of locked cells.
        CellIdCollection             LockedCells;    
        //  When the value >= 0, indicates a lock mthash operation.
        //  Will be overridden by LockingStorage because there are
        //  parallel MTHash operations, thus the value of LockingMTHash
        //  is undeterministic.
        //  Note:
        //  When this value is found by Arbitrator, it means that all
        //  bucket locks are taken, but some cell locks remain unreleased.
        int32_t                      LockingMTHash;  
        //  When true, overrides LockingMTHash. LockingStorage is only
        //  set by LockingStorageContext, which is possessed by only
        //  the whole-db-level interfaces LoadStorage/SaveStorage/ResetStorage.
        //  Thus, when true, LockingCell is invalid.
        //  Note:
        //  When this value is found by Arbitrator, it means that one of the
        //  MTHash is bucket-locked, but some cell locks remain unreleased.
        bool                         LockingStorage; 
        //  When true, indicates a trunk reload, caused by CellAlloc.
        //  In this case, the thread must be in the memory allocation arena,
        //  and thus possesses a valid LockingCell value.
        //  Note:
        //  When this value is found by Arbitrator, it means that a Reload
        //  operation has problems locking a cell not in the memory 
        //  allocation arena, while the LockingCell field points to the cell
        //  which an operation leading to Reload is being performed on, and
        //  the value of LockingMTHash is undefined.
        bool                         ReloadingMemoryTrunk;
        //  A checksum of the thread context is computed during a lock arbitration.
        //  If the checksum does not change at the next arbitration request, it is
        //  likely that the thread did not detect deadlock during the first request,
        //  but then entered arbitration again. In this case we can decide to increase
        //  the time that the thread stays inside the arbitrator.
        uint64_t                     Checksum;
        //  Used to record how many times that the same context checksum is detected
        //  during arbitration. Used to decide additional wait time that a thread 
        //  should stay inside the arbitrator.
        int32_t                      SameChecksumTimes;

        //  Sets LockingCell
        inline void SetLockingCell(const cellid_t cellId) { LockingCell = cellId; }
        //  When acquired is true, add the locking cell to the locked cell set.
        inline void SetLockAcquired(const bool acquired) { if (acquired) LockedCells.Add(LockingCell); }
        //  Removes the given cell from the locked cell set.
        inline void SetLockReleased(const cellid_t cellId) { LockedCells.Remove(cellId); }
    }THREAD_CONTEXT, *PTHREAD_CONTEXT;

    PTHREAD_CONTEXT              AllocateThreadContext(); // Note, AllocateThreadConatext will also set the thread context.
    void                         DeallocateThreadContext(PTHREAD_CONTEXT ctx);
    void                         SetCurrentThreadContext(PTHREAD_CONTEXT ctx);
    PTHREAD_CONTEXT              GetCurrentThreadContext();

    void                         EnterMemoryAllocationArena(PTHREAD_CONTEXT ctx);
    //  !caller holds alloc_lock of the calling trunk, caller is CellAlloc
    void                         ExitMemoryAllocationArena(PTHREAD_CONTEXT ctx);
    //  !caller is GetAllEntryLocksExceptArena
    std::unordered_set<cellid_t> ReadMemoryAllocationArena();

    //  Detects deadlocks. Returns E_SUCCESS or E_DEADLOCK
    TrinityErrorCode Arbitrate();

#pragma region RAII
    struct LockingStorageContext
    {
        PTHREAD_CONTEXT m_pctx;

        LockingStorageContext(const PTHREAD_CONTEXT p_ctx)
        {
            m_pctx = p_ctx;
            m_pctx->LockingStorage = true;
            SetCurrentThreadContext(m_pctx);
        }

        ~LockingStorageContext()
        {
            m_pctx->LockingStorage = false;
        }
    };

#pragma endregion
}
