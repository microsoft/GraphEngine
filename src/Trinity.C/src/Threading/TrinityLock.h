// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"

#ifdef TRINITY_PLATFORM_WINDOWS

class TrinityLock
{
private:
    volatile LONG m_spinlock;
    const LONG c_available = 1;
    const LONG c_acquired = 0;

public:
    TrinityLock() 
    {
        m_spinlock = c_available;
    }

    void lock()
    {
        while (true) 
        {
            auto val = InterlockedDecrement(&m_spinlock);
            if (val == c_acquired) return;
            //  val is a negative integer, and when we
            //  wake a thread, m_spinlock is reset and
            //  we have to try again to grab the lock.
            WaitOnAddress(&m_spinlock, &val, sizeof(LONG), INFINITE);
        }
    }

    void unlock()
    {
        m_spinlock = c_available;
        WakeByAddressSingle((PVOID)&m_spinlock);
    }

    void lock(std::atomic<int32_t> & pending_flag)
    {
        if (!trylock())
        {
            pending_flag.store(1);
            lock();
        }
    }

    bool trylock()
    {
        auto val = InterlockedDecrement(&m_spinlock);
        return (val == c_acquired);
        //  we don't have to make up the value, because
        //  the unlocker will rearm the lock flag.
    }
};
#else

// A straightforward std::mutex wrapper
#include <mutex>

class TrinityLock
{
private:
    std::mutex m_mutex;
public:
    TrinityLock() : m_mutex() { }
    void lock()
    {
        m_mutex.lock();
    }

    void unlock()
    {
        m_mutex.unlock();
    }

    void lock(std::atomic<int32_t> & pending_flag)
    {
        if (!m_mutex.trylock())
        {
            pending_flag.store(1);
            m_mutex.lock();
        }
    }

    bool trylock()
    {
        return m_mutex.trylock();
    }
};
#endif

