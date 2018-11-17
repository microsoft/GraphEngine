// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#include "TrinityCommon.h"
#include "Trinity/Configuration/TrinityConfig.h"
#include "Storage/LocalStorage/LocalMemoryStorage.h"
#include "Memory/Memory.h"

namespace TrinityConfig
{
    String storage_root           = "";
    bool read_only                = false;
    bool handshake                = true;
    bool client_disable_sndbuffer = false;
    int32_t trunk_count           = 256;

    /// <summary>
    /// Default Value = 16
    /// </summary>
    int32_t GCParallelism = 16;

    /// <summary>
    /// Defragmentation frequency, Default Value = 600
    /// </summary>
    int32_t DefragInterval = 600;


    StorageCapacityProfile CapacityProfile = StorageCapacityProfile::Max8G;
    uint32_t VMAllocUnit = 0x200000;
    int32_t MaxLargeObjectCount = 128;
    int32_t LOReservationSize = 0x100000;
    int32_t _LargeObjectThreshold = 10 << 20; //10MB, cannot be larger than 16MB

    String StorageRoot()
    {
        return storage_root;
    }

    void SetStorageRoot(String storageRoot)
    {
        storage_root = storageRoot;
        Diagnostics::WriteLine(LogLevel::Verbose, "StorageRoot set to {0}", storage_root);
    }

    void SetGCDefragInterval(int32_t interval)
    {
        TrinityConfig::DefragInterval = interval;
    }

    bool ReadOnly()
    {
        return read_only;
    }

    void SetReadOnly(bool value)
    {
        if (Storage::LocalMemoryStorage::initialized)
        {
            Diagnostics::WriteLine(LogLevel::Error, "TrinityConfig: Cannot change property 'ReadOnly' after LocalMemoryStorage initialization.");
            return;
        }

        read_only = value;
    }

    bool Handshake()
    {
        return handshake;
    }

    void SetHandshake(bool value)
    {
        handshake = value;
    }

    bool ClientDisableSendBuffer()
    {
        return client_disable_sndbuffer;
    }

    void SetClientDisableSendBuffer(bool value)
    {
        client_disable_sndbuffer = value;
    }

    int32_t TrunkCount()
    {
        return trunk_count;
    }

    void SetTrunkCount(int32_t value)
    {
        if (Storage::LocalMemoryStorage::initialized)
        {
            Diagnostics::WriteLine(LogLevel::Error, "TrinityConfig: Cannot change property 'TrunkCount' after LocalMemoryStorage initialization.");
            return;
        }

        int32_t bit_cnt = 0;
        for (int32_t v = value; v; v = v >> 1)
        {
            bit_cnt += (v & 1);
        }

        if (bit_cnt != 1 || value > 256)
        {
            Diagnostics::WriteLine(LogLevel::Error, "TrinityConfig: 'TrunkCount' should be power of 2 between range [1, 256].");
            return;
        }

        trunk_count = value;
    }

    int32_t LargeObjectThreshold()
    {
        return _LargeObjectThreshold;
    }

    void SetLargeObjectThreshold(int32_t value)
    {
        if (value >= 0xFFFFFF)
        {
            _LargeObjectThreshold = 0xFFFFFF - 1;
        }
        else
        {
            _LargeObjectThreshold = value;
        }
    }

    uint64_t MemoryReserveUnit()
    {
        switch (CapacityProfile)
        {
        case StorageCapacityProfile::Max32G:
            return 0x8000000; // 128 M
        case StorageCapacityProfile::Max16G:
            return 0x4000000; // 64 M
        case StorageCapacityProfile::Max4G:
            return 0x1000000; // 16 M
        case StorageCapacityProfile::Max2G:
            return 0x800000;  // 8M
        case StorageCapacityProfile::Max1G:
            return 0x400000;  // 4M
        case StorageCapacityProfile::Max512M:
            return 0x200000;  // 2M
        case StorageCapacityProfile::Max256M:
            return 0x100000;  // 1M
        default:
            return 0x2000000;// 32 M
        }
    }

    uint32_t MaxEntryCount()
    {
        switch (CapacityProfile)
        {
        case StorageCapacityProfile::Max32G:
            return 0x8000000; // 128 M
        case StorageCapacityProfile::Max16G:
            return 0x4000000; // 64 M
        case StorageCapacityProfile::Max4G:
            return 0x1000000; // 16 M
        case StorageCapacityProfile::Max2G:
            return 0x800000;  // 8M
        case StorageCapacityProfile::Max1G:
            return 0x400000;  // 4M
        case StorageCapacityProfile::Max512M:
            return 0x200000;  // 2M
        case StorageCapacityProfile::Max256M:
            return 0x100000;  // 1M
        default:
            return 0x2000000;// 32 M
        }
    }

    uint64_t TrinityReservedSpace()
    {
        return ReservedSpacePerTrunk() * TrunkCount();
    }

    /// <summary>
    /// Value = 2G + MemoryReserveUnit * 32
    /// </summary>
    uint64_t ReservedSpacePerTrunk()
    {
        switch (CapacityProfile)
        {
        case StorageCapacityProfile::Max32G:
            return 0x180000000; // 6G
        case StorageCapacityProfile::Max16G:
            return 0x100000000; // 4G
        case StorageCapacityProfile::Max4G:
            return 0xA0000000; // 2.5 G
        case StorageCapacityProfile::Max2G:
            return 0x90000000; // 2 G + 256 M
        case StorageCapacityProfile::Max1G:
            return 0x88000000; // 2 G + 128 M
        case StorageCapacityProfile::Max512M:
            return 0x84000000; // 2 G + 64 M
        case StorageCapacityProfile::Max256M:
            return 0x82000000; // 2 G + 32 M
        default:
            return 0xC0000000;// 3G
        }
    }

    int32_t GetStorageCapacityProfile()
    {
        return (int32_t)CapacityProfile;
    }

    void SetStorageCapacityProfile(int32_t capacityProfile)
    {
        CapacityProfile = (StorageCapacityProfile)capacityProfile;
    }
}

// exported getter/setter
DLL_EXPORT BOOL    CHandshake() { return TrinityConfig::Handshake() ? TRUE : FALSE; }
DLL_EXPORT VOID    CSetHandshake(bool value) { TrinityConfig::SetHandshake(value); }
DLL_EXPORT BOOL    CClientDisableSendBuffer() { return TrinityConfig::ClientDisableSendBuffer() ? TRUE : FALSE; }
DLL_EXPORT VOID    CSetClientDisableSendBuffer(bool value) { TrinityConfig::SetClientDisableSendBuffer(value); }
DLL_EXPORT VOID    SetStorageRoot(const u16char* buffer, int32_t length) { TrinityConfig::SetStorageRoot(String::FromWcharArray(buffer, length >> 1)); }
DLL_EXPORT BOOL    CReadOnly() { return TrinityConfig::ReadOnly() ? TRUE : FALSE; }
DLL_EXPORT void    CSetReadOnly(bool value) { TrinityConfig::SetReadOnly(value); }
DLL_EXPORT int32_t CTrunkCount() { return TrinityConfig::TrunkCount(); }
DLL_EXPORT void    CSetTrunkCount(int32_t value) { TrinityConfig::SetTrunkCount(value); }
DLL_EXPORT int32_t GetStorageCapacityProfile() { return TrinityConfig::GetStorageCapacityProfile(); }
DLL_EXPORT void    SetStorageCapacityProfile(int32_t value) { TrinityConfig::SetStorageCapacityProfile(value); }
DLL_EXPORT int32_t CLargeObjectThreshold() { return TrinityConfig::LargeObjectThreshold(); }
DLL_EXPORT void    CSetLargeObjectThreshold(int32_t value) { TrinityConfig::SetLargeObjectThreshold(value); }
DLL_EXPORT void    CSetGCDefragInterval(int32_t value) { TrinityConfig::SetGCDefragInterval(value); }