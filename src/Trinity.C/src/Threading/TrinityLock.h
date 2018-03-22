// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"

#ifdef TRINITY_PLATFORM_WINDOWS

// std::mutex is not as fast on windows.

class TrinityLock
{
private:
    CRITICAL_SECTION m_csection;

public:
    TrinityLock()
    {
        // WinXP onwards and this function does not fail.
        InitializeCriticalSectionAndSpinCount(&m_csection, 4000);
    }

    ~TrinityLock()
    {
        DeleteCriticalSection(&m_csection);
    }


    void lock()
    {
        EnterCriticalSection(&m_csection);
    }

    void unlock()
    {
        LeaveCriticalSection(&m_csection);
    }

    void lock(std::atomic<int32_t> & pending_flag)
    {
        if (!TryEnterCriticalSection(&m_csection)) 
        {
            pending_flag.store(1);
            EnterCriticalSection(&m_csection);
        }
    }

    bool trylock()
    {
        return TryEnterCriticalSection(&m_csection);
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

