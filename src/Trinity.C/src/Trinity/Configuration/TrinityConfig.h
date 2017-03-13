// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "TrinityCommon.h"
#include <Trinity/String.h>

#define PROPERTY(type, name) type name(); void Set##name(type value);

namespace TrinityConfig
{
    using Trinity::String;

    enum MemoryAllocationProfile :int32_t
    {
        Aggressive = 0, TrinityDefault = 1, Modest = 2, SystemDefault = 3
    };

    enum StorageCapacityProfile :int32_t
    {
        /// <summary>
        /// Maximum 256 million cells supported by each LocalStorage instance.
        /// </summary>
        Max256M,

        /// <summary>
        /// Maximum 512 million cells supported by each LocalStorage instance.
        /// </summary>
        Max512M,

        /// <summary>
        /// Maximum 1 billion cells supported by each LocalStorage instance.
        /// </summary>
        Max1G,

        /// <summary>
        /// Maximum 2 billion cells supported by each LocalStorage instance.
        /// </summary>
        Max2G,

        /// <summary>
        /// Maximum 4 billion cells supported by each LocalStorage instance.
        /// </summary>
        Max4G,

        /// <summary>
        /// Maximum 8 billion cells supported by each LocalStorage instance.
        /// </summary>
        Max8G,
        /// <summary>
        /// Maximum 16 billion cells supported by each LocalStorage instance.
        /// </summary>
        Max16G,
        /// <summary>
        /// Maximum 32 billion cells supported by each LocalStorage instance.
        /// </summary>
        Max32G
    };

    const uint16_t UndefinedCellType = 0;
    const uint16_t CellNotFound      = INT16_MAX;
    const uint64_t MemoryPoolSize    = 0x10000000;

    extern uint32_t VMAllocUnit;
    extern int32_t MaxLargeObjectCount;
    extern int32_t LOReservationSize;

    /// <summary>
    /// Default Value = 16
    /// </summary>
    extern int32_t  GCParallelism;

    /// <summary>
    /// Defragmentation frequency, Default Value = 600
    /// </summary>
    extern int32_t  DefragInterval;

    /// <summary>
    /// Default Value = 256
    /// </summary>
    PROPERTY(int32_t, TrunkCount);
    PROPERTY(bool, ReadOnly);
    PROPERTY(bool, Handshake);
    PROPERTY(bool, ClientDisableSendBuffer);
    PROPERTY(int32_t, LargeObjectThreshold);

    String StorageRoot();
    void SetStorageRoot(String storageRoot);
    String LogDirectory();
    void SetLogDirectory(String logDir);
    void SetGCDefragInterval(int32_t interval);

    /// <summary>
    /// Indicates the in-memory storage capacity profile.
    /// </summary>
    extern StorageCapacityProfile CapacityProfile;

    uint32_t MaxEntryCount();

    uint64_t MemoryReserveUnit();

    uint64_t TrinityReservedSpace();

    /// <summary>
    /// Value = 2G + MemoryReserveUnit * 32
    /// </summary>
    uint64_t ReservedSpacePerTrunk();

    int32_t GetStorageCapacityProfile();
    void SetStorageCapacityProfile(int32_t capacityProfile);
}