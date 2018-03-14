#include "ThreadContext.h"
#include "Trinity/Diagnostics/Log.h"
#include "LocalMemoryStorage.h"
#include "Storage/MTHash/MTHash.h"
#include <threading>
#include "Trinity/Diagnostics/Log.h"
#include "Trinity/Hash/NonCryptographicHash.h"

#define SAME_CHECKSUM_TIMES_MAX 16384

namespace Storage
{
#pragma region helper structures and functions
    using Diagnostics::WriteLine;
    using Diagnostics::LogLevel;

    struct THREAD_CONTEXT_SET
    {
        TrinitySpinlock                     m_mutex;
        std::unordered_set<PTHREAD_CONTEXT> m_pcontexts;

        void Enter(PTHREAD_CONTEXT pctx)
        {
            m_mutex.lock();
            m_pcontexts.insert(pctx);
            m_mutex.unlock();
        }

        void Exit(PTHREAD_CONTEXT pctx)
        {
            m_mutex.lock();
            m_pcontexts.erase(pctx);
            m_mutex.unlock();
        }

        std::unordered_set<cellid_t> GetArenaCellIds()
        {
            //  Memory allocation arena cell ids = locked cells + locking cells
            //  Because when we enter memory allocation arena, the "locking cell"
            //  must be already locked (or the thread will go to arbitrator for lock
            //  resolution)
            std::unordered_set<cellid_t> cellids;
            m_mutex.lock();
            for (auto& pctx : m_pcontexts)
            {
                auto locked_cells = pctx->LockedCells.ToList();
                cellids.insert(locked_cells.begin(), locked_cells.end());
                cellids.insert(pctx->LockingCell);
            }
            m_mutex.unlock();
            return cellids;
        }

        //  Copy the thread contexts, with self at the beginning
        std::vector<THREAD_CONTEXT> Copy(PTHREAD_CONTEXT self)
        {
            std::vector<THREAD_CONTEXT> ret;
            ret.push_back(*self);
            m_mutex.lock();
            for (auto& pctx : m_pcontexts)
            {
                if (self != pctx)
                    ret.push_back(*pctx);//context copied
            }
            m_mutex.unlock();
            return ret;
        }
    };

    THREAD_LOCAL PTHREAD_CONTEXT     t_thread_ctx = nullptr;
    static       THREAD_CONTEXT_SET  g_memory_arena;
    static       THREAD_CONTEXT_SET  g_arbitrator_arena;

    PTHREAD_CONTEXT AllocateThreadContext()
    {
        PTHREAD_CONTEXT pctx       = new THREAD_CONTEXT();

        pctx->LockingMTHash        = -1;
        pctx->LockingStorage       = false;
        pctx->ReloadingMemoryTrunk = false;
        pctx->Checksum             = 0;
        pctx->SameChecksumTimes    = 0;

        WriteLine(LogLevel::Verbose, "Allocated thread context {0}.", pctx);
        SetCurrentThreadContext(pctx);
        return pctx;
    }

    void DeallocateThreadContext(PTHREAD_CONTEXT pctx)
    {
        if (pctx->LockedCells.size() != 0)
        {
            WriteLine(LogLevel::Error, "Thread context {0} still holds {1} cell locks.", pctx, pctx->LockedCells.size());
            //TODO release cell locks.
            //TODO return error code
        }
        delete pctx;
        WriteLine(LogLevel::Verbose, "Deallocated thread context {0}.", pctx);
    }

    void SetCurrentThreadContext(PTHREAD_CONTEXT ctx)
    {
        // TODO free t_thread_ctx if it exists
        // TODO error if t_thread_ctx has content
        WriteLine(LogLevel::Debug, "Thread {0}: Set ThreadContext {1}.", std::this_thread::get_id(), t_thread_ctx);
        t_thread_ctx = ctx;
    }

    PTHREAD_CONTEXT GetCurrentThreadContext()
    {
        if (t_thread_ctx == nullptr) return AllocateThreadContext();
        else return t_thread_ctx;
    }

    void EnterMemoryAllocationArena(PTHREAD_CONTEXT pctx)
    {
        g_memory_arena.Enter(pctx);
    }

    void ExitMemoryAllocationArena(PTHREAD_CONTEXT pctx)
    {
        g_memory_arena.Exit(pctx);
    }

    std::unordered_set<cellid_t> ReadMemoryAllocationArena()
    {
        return g_memory_arena.GetArenaCellIds();
    }

    //  The behavior of each status is documented above each state.
    //  All hash table lock operations are mutually exclusive.
    //  Two hash-table lock operations will not inter-lock, as long as:
    //  1. The lock operation is a single lock operation (have not locked another hash table).
    //  2. The lock operation is a sequential lock-all operation.
    //  Note that LOCKHASH/LOCKDB/RELOAD states do not propagate into user-space. 
    //  Any time when user code is executing, the state must be LOCKCELL. The state
    //  only transist to others when the code have entered Trinity, and will resume to
    //  LOCKCELL when it returns to userspace.
    enum _THREAD_CONTEXT_STATUS
    {
        // Have not locked hash tables.
        // Attempting to lock a cell.
        // Could have locked other cells. 
        LOCKCELL,
        // Locking a single hash table. Must not lock other hash tables.
        // Could have locked cells.
        LOCKHASH,
        // Locking all hash tables sequentially. 
        // Could have locked cells.
        LOCKDB,
        // Locking all entry locks except arena in a hash table. 
        // Must not lock any hash tables.
        // Could have locked cells, which are currently in memory allocation arena.
        RELOAD
    };

    static _THREAD_CONTEXT_STATUS _GetThreadContextStatus(const THREAD_CONTEXT& ctx)
    {
        if (ctx.ReloadingMemoryTrunk)
            return _THREAD_CONTEXT_STATUS::RELOAD;
        else if (ctx.LockingStorage)
            return _THREAD_CONTEXT_STATUS::LOCKDB;
        //  We are neither locking storage nor reloading trunk.
        //  Which means that, we are either locking a hash table,
        //  or just locking a cell.
        //  There are several paths to call MTHash::Lock:
        //  1. MemoryTrunk::EmptyCommittedMemory by a GC thread
        //     - Guaranteed no cell locks held. See comments in that routine.
        //  2. LocalMemoryStorage::_enter_db_critical
        //     - Marked with LockingStorage. Won't hit in this case.
        //  3. During enumeration.
        else if (ctx.LockingMTHash >= 0)
            return _THREAD_CONTEXT_STATUS::LOCKHASH;
        else
            return _THREAD_CONTEXT_STATUS::LOCKCELL;
    }
#pragma endregion

    // true if v[source_idx] -> ... -> v[idx] -> v[source_idx]
    static bool _dep_visit(const std::vector<std::vector<bool>> &dep_graph, std::vector<bool> &dep_visited, const size_t idx, const size_t source_idx, const size_t n)
    {
        dep_visited[idx] = true;
        for (int i=0; i < n; ++i)
        {
            if (i == idx)continue;
            if (dep_graph[idx][i])
            {
                if (!dep_visited[i])
                {
                    if (_dep_visit(dep_graph, dep_visited, i, source_idx, n))
                        return true;
                }
                else if (i == source_idx)
                {
                    return true;
                }
            }
        }
        dep_visited[idx] = false;
        return false;
    }

    //  Returns true if a depends on b, that is,
    //  a can proceed only if b releases some locks.
    static bool _CalculateLockDependency(const THREAD_CONTEXT& a, const THREAD_CONTEXT& b)
    {
        auto sa = _GetThreadContextStatus(a);
        auto sb = _GetThreadContextStatus(b);

        if (sa == LOCKCELL) switch (sb)
        {
        case LOCKCELL:
            // a is locking a cell held by b
            return b.LockedCells.Contains(a.LockingCell);
        case LOCKHASH:
            // a is locking a cell, whose bucket lock is taken by b.
            // or b is holding the lock.
            return (Storage::LocalMemoryStorage::GetTrunkId(a.LockingCell) == 
                    b.LockingMTHash)
                    || b.LockedCells.Contains(a.LockingCell);
        case RELOAD:
            // a is locking a cell, whose trunk is being reloaded
            // or b is holding the lock.
            return (Storage::LocalMemoryStorage::GetTrunkId(a.LockingCell) ==
                    Storage::LocalMemoryStorage::GetTrunkId(b.LockingCell))
                    || b.LockedCells.Contains(a.LockingCell);
        case LOCKDB:
            // a is locking a cell while the db is being locked
            return true;
        }
        else if (sb == LOCKCELL) switch (sa)
        {
            //case LOCKCELL: <-- already executed in previous branch
        case LOCKHASH:
            // a is locking a MTHash, while b possesses cell locks in this MTHash
            return b.LockedCells.Any([&](cellid_t lockedCell)
            {
                return 
                    Storage::LocalMemoryStorage::GetTrunkId(lockedCell) == 
                    a.LockingMTHash;
            });
        case RELOAD:
            // a is reloading while b has locked a cell in the same MTHash
            return b.LockedCells.Any([&](cellid_t lockedCell)
            {
                return 
                    Storage::LocalMemoryStorage::GetTrunkId(lockedCell) == 
                    Storage::LocalMemoryStorage::GetTrunkId(a.LockingCell);
            });
        case LOCKDB:
            // a is locking db, while b possesses cell locks
            return (b.LockedCells.size() != 0);
        }
        /*(sa != LOCKCELL && sb != LOCKCELL)*/
        else if (sa == RELOAD) switch(sb)
        {
        case LOCKHASH:
        case LOCKDB:
            // a is reloading a MTHash, and b possesses some locks in that trunk.
            return b.LockedCells.Any([&](cellid_t lockedCell)
            {
                return 
                    Storage::LocalMemoryStorage::GetTrunkId(lockedCell) == 
                    Storage::LocalMemoryStorage::GetTrunkId(a.LockingCell);
            });
        case RELOAD:
            // when both a and b are reloading (they should be reloading different trunks
            // due to the alloc_lock), they do not lock each other, because they share
            // g_memory_arena and thus are aware of each other's context.
            return false;
        }
        else if (sa == LOCKHASH) switch (sb) 
        {
        case LOCKHASH:
        case LOCKDB:
            // a is locking a MTHash, while b possesses cell locks in that trunk.
            return b.LockedCells.Any([&](cellid_t lockedCell)
            {
                return 
                    Storage::LocalMemoryStorage::GetTrunkId(lockedCell) == 
                    a.LockingMTHash;
            });
            break;
        case RELOAD:
            // a is locking a MTHash, while b is reloading this MTHash (taken all entries)
            return 
                a.LockingMTHash ==
                Storage::LocalMemoryStorage::GetTrunkId(b.LockingCell);
            break;
        }
        else if (sa == LOCKDB) switch (sb) 
        {
        case LOCKHASH:
        case LOCKDB:
            // a is locking db and b possesses cell locks.
            return (b.LockedCells.size() != 0);
        case RELOAD:
            // a is locking db and b is reloading, and thus possesses at least one cell lock.
            return true;
        }

        assert(false && "_CalculateDependency_LOCKCELL");
        return false;
    }

    static bool _DeadlockDetect(const std::vector<THREAD_CONTEXT>& contexts, const size_t source_idx)
    {
        // prepare dependency graph
        size_t n = contexts.size();
        std::vector<std::vector<bool>> dep_graph(n, std::vector<bool>(n));
        for (int i=0; i < n; ++i)
            for (int j=0; j < n; ++j)
            {
                dep_graph[i][j] = _CalculateLockDependency(contexts[i], contexts[j]);
            }
        std::vector<bool> dep_visited(n, false);
        return _dep_visit(dep_graph, dep_visited, 0, 0, n);
    }

    using Trinity::Hash::H;
    using Trinity::Hash::hash_64;
    static uint64_t _CalculateThreadContextChecksum(const PTHREAD_CONTEXT pctx)
    {
        uint64_t h = 0;
        H(&h, &pctx->LockingCell, hash_64);
        H(&h, &pctx->LockingMTHash, hash_64);
        H(&h, &pctx->LockingStorage, hash_64);
        H(&h, &pctx->ReloadingMemoryTrunk, hash_64);
        pctx->LockedCells.ForEach([&](const cellid_t id) {H(&h, &id, hash_64); });
        return h;
    }
    //  Arbitrator priority:
    //  1. RELOAD has highest priority, and will not get E_DEADLOCK as arbitration result.
    //  2. LOCKHASH and LOCKDB have high priority. They do not receive notification about
    //     deadlocks with respect to LOCKCELL. However, deadlocks between LOCKHASH, LOCKDB
    //     and RELOAD do get reported.
    //  3. LOCKCELL has low priority, and will get E_DEADLOCK when dead lock is detected with
    //     any state.

    //  Deadlock examples:
    //  ====================================
    //  a locks cell 1
    //  b locks cell 256
    //  b reload, block on cell 1
    //  a locks mthash 1, block on cell 256
    // 
    //  --> Result: a=DEADLOCK, b=SUCCESS
    //  ====================================
    //  a lock cell 1
    //  b lock cell 256
    //  a locks mthash 1, blocks on cell 256
    //  b locks mthash 1, blocks on cell 1
    // 
    //  --> Result: a=DEADLOCK, b=DEADLOCK
    //  ====================================
    //  a lock cell 1
    //  b lock cell 2
    //  a locks mthash 2, blocks on cell 2
    //  b locks mthash 1, blocks on cell 1
    // 
    //  --> Result: a=DEADLOCK, b=DEADLOCK
    //  ====================================

    // Calculating checksum for a thread context during arbitration:
    // 
    // Arbitration retry count scales up when the same thread requests arbitration
    // repeatidly without changing the thread context. This is likely to happen
    // when a deadlock is formed but the arbitrator fails to detect it due to
    // the retry count being too low. If so, different threads will request for
    // arbitration at different time, thus will never be aware of each other.
    // preliminary profiling has shown that retry_max reaches 28800 before
    // two dead-locked threads (LOCKCELL vs. LOCKHASH) are finally aware of each other.
    // currently we have a linear increasing scheme for the retry max count. It scales
    // up from 128, which means that 225 arbitrations are requested...
    // The solution: We detect whether a thread has repeatidly requested arbitration
    // without changing its state (that nothing new happened between arbitrations, to
    // this thread). To do so we calculate the checksum of a thread context and record
    // it. We also log down how many times it 'polled' arbitration in 
    // pctx->SameChecksumTimes. Then, we use the counter as a multiplier to increase
    // the time that such a thread stays in the arbitration room.
    TrinityErrorCode Arbitrate()
    {
        PTHREAD_CONTEXT        pctx             = GetCurrentThreadContext();
        _THREAD_CONTEXT_STATUS status           = _GetThreadContextStatus(*pctx);
        uint64_t               checksum         = _CalculateThreadContextChecksum(pctx);
        TrinityErrorCode       eResult          = TrinityErrorCode::E_SUCCESS;
        size_t                 h_contexts_count = 0;
        int32_t                retry_max        = 128;

        if (checksum == pctx->Checksum)
        {
            if (pctx->SameChecksumTimes < SAME_CHECKSUM_TIMES_MAX)
                ++pctx->SameChecksumTimes;
            retry_max *= pctx->SameChecksumTimes;
        }
        else
        {
            pctx->Checksum = checksum;
            pctx->SameChecksumTimes = 0;
        }

        Diagnostics::WriteLine(LogLevel::Verbose, "Arbitrator: Thread context {0} entering with status {1}, retry max={2}.", pctx, status, retry_max);
        g_arbitrator_arena.Enter(pctx);


        for (int retry = 0; retry < retry_max; ++retry)
        {
            auto   contexts = g_arbitrator_arena.Copy(pctx);
            size_t size     = contexts.size();
            
            if (size > h_contexts_count) 
            {
                // The arbitration room gets more threads, wait longer.
                retry = 0; 
            }

            h_contexts_count = size;

            switch (_GetThreadContextStatus(*pctx))
            {
            case RELOAD:
                // Reload is critical high priority.
                // Do nothing
                break;
            case LOCKDB:
            case LOCKHASH:
                // remove LOCKCELL contexts to gain priority.
                contexts.erase(
                    std::remove_if( contexts.begin(), contexts.end(),
                    [](const THREAD_CONTEXT& ctx){ return _GetThreadContextStatus(ctx) == LOCKCELL; }),
                    contexts.end());
                /* fallthrough */
            case LOCKCELL:
                // Run the deadlock detector.
                if (_DeadlockDetect(contexts, 0))
                {
                    Diagnostics::WriteLine(LogLevel::Error, "Arbitrator: lock cell operation: dead lock detected.");
                    eResult = TrinityErrorCode::E_DEADLOCK;
                    goto arbitrator_exit;
                }
                // Otherwise, we should not cause deadlocks.
                break;
            default:
                assert(false && "Arbitrator: Invalid thread context status");
                break;
            }

            Runtime::Spinwait(ENTRY_LOCK_SPIN_COUNT);
        }

    arbitrator_exit:

        g_arbitrator_arena.Exit(pctx);
        Diagnostics::WriteLine(LogLevel::Verbose, "Arbitrator: Thread context {0} exited with error code {1}.", pctx, eResult);
        return eResult;
    }

}