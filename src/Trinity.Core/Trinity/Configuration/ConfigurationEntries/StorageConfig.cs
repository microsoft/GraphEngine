// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Diagnostics;
using Trinity.Storage;
using Trinity.Utilities;

namespace Trinity.Configuration
{
    /// <summary>
    /// Contains settings for the configuration section "Storage".
    /// </summary>
    public sealed class StorageConfig
    {
        #region Singleton
        static StorageConfig s_StorageConfig = new StorageConfig();
        private StorageConfig()
        {
            StorageRoot     = DefaultStorageRoot;
            TrunkCount      = c_MaxTrunkCount;
            ReadOnly        = c_DefaultReadOnly;
            StorageCapacity = c_DefaultStorageCapacityProfile;
            DefragInterval  = c_DefaultDefragInterval;
        }
        /// <summary>
        /// Gets the configuration entry singleton instance.
        /// </summary>
        [ConfigInstance]
        public static StorageConfig Instance { get { return s_StorageConfig; } }
        [ConfigEntryName]
        internal static string ConfigEntry { get { return ConfigurationConstants.Tags.STORAGE.LocalName; } }
        #endregion

        #region Private static helpers
        private static void ThrowLargeObjectThresholdException()
        {
            throw new ArgumentOutOfRangeException("LargeObjectThreshold cannot be larger than 16MB.");
        }

        private static void ThrowDisableReadOnlyException()
        {
            throw new ArgumentException("ReadOnly flag cannot be disabled once enabled.");
        }

        private static string DefaultStorageRoot { get { return Path.Combine(AssemblyUtility.MyAssemblyPath, "storage"); } }
        #endregion

        #region Fields
        internal const int    c_MaxTrunkCount = ConfigurationConstants.Values.MAX_TRUNK_COUNT;
        internal const bool   c_DefaultReadOnly = ConfigurationConstants.Values.DEFAULT_VALUE_FALSE;
        public const ushort   c_UndefinedCellType = ConfigurationConstants.Values.UNDEFINED_CELL_TYPE;
        internal const int    c_DefaultDefragInterval = ConfigurationConstants.Values.DEFAULT_DEFRAG_INTERVAL;
        internal const StorageCapacityProfile 
                              c_DefaultStorageCapacityProfile = StorageCapacityProfile.Max8G;
        internal int          m_GCParallelism = ConfigurationConstants.Values.DEFALUT_GC_PATRALLELISM;
        internal int          m_DefragInterval;
        private  string       m_StorageRoot = ConfigurationConstants.Values.BLANK;
        #endregion

        #region Properties
        /// <summary>
        /// Represents a value to specify the number of memory trunks in the local memory storage.
        /// </summary>
        [ConfigSetting(Optional: true)]
        public int TrunkCount
        {
            get
            {
                return CTrinityConfig.CTrunkCount();
            }

            set
            {
                CTrinityConfig.CSetTrunkCount(value);
            }
        }

        /// <summary>
        /// Represents a value to specify whether the local memory storage is read-only.
        /// if the value is true read-only mode is on and any changes in a cell is forbidden
        /// </summary>
        [ConfigSetting(Optional: true)]
        public bool ReadOnly
        {
            get
            {
                return CTrinityConfig.CReadOnly();
            }

            set
            {
                CTrinityConfig.CSetReadOnly(value);
            }
        }

        /// <summary>
        /// Represents a value to specify the local memory storage capacity profile.
        /// </summary>
        [ConfigSetting(Optional: true)]
        public StorageCapacityProfile StorageCapacity
        {
            get
            {
                return (StorageCapacityProfile)CTrinityConfig.GetStorageCapacityProfile();
            }
            set
            {
                CTrinityConfig.SetStorageCapacityProfile((int)value);
            }
        }

        /// <summary>
        /// Represents the path for saving persistent storage disk images.Defaults to AssemblyPath\storage.
        /// </summary>
        [ConfigSetting(Optional: true)]
        public unsafe string StorageRoot
        {
            get
            {
                if (m_StorageRoot == null || m_StorageRoot.Length == 0)
                {
                    m_StorageRoot = DefaultStorageRoot;
                }

                if (m_StorageRoot[m_StorageRoot.Length - 1] != Path.DirectorySeparatorChar)
                {
                    m_StorageRoot = m_StorageRoot + Path.DirectorySeparatorChar;
                }

                return m_StorageRoot;
            }

            set
            {
                m_StorageRoot = value;
                if (m_StorageRoot == null || m_StorageRoot.Length == 0)
                {
                    Log.WriteLine(LogLevel.Warning, "StorageConfig: Invalid StorageRoot, falling back to the default setting.");
                    m_StorageRoot = DefaultStorageRoot;
                }

                if (m_StorageRoot[m_StorageRoot.Length - 1] != Path.DirectorySeparatorChar)
                {
                    m_StorageRoot += Path.DirectorySeparatorChar;
                }

            }
        }

        /// <summary>
        /// Represents a value to specify the Large Object Threshold, Default = 10 M
        /// </summary>
        internal int LargeObjectThreshold
        {
            get
            {
                return CTrinityConfig.CLargeObjectThreshold();
            }
            set
            {
                if (value >= 0xFFFFFF)
                {
                    ThrowLargeObjectThresholdException();
                }
                else
                {
                    CTrinityConfig.CSetLargeObjectThreshold(value);
                }
            }
        }

        /// <summary>
        /// Represents a value to specify the Defragmentation frequency, Default Value = 600
        /// </summary>
        [ConfigSetting(Optional: true)]
        public int DefragInterval
        {
            get { return m_DefragInterval; }
            set { m_DefragInterval = value; CTrinityConfig.CSetGCDefragInterval(m_DefragInterval); }
        }
        #endregion
    }
}
#region tips
/*
 Rough storage profiling
+-------------------------+--------------------
  StorageCapacityProfile  |   CommittedMemory
  256M                    |   2127652K
  512M                    |   2144752K
  1G                      |   2177592K
  2G                      |   2243280K
  4G                      |   2374544K
  8G                      |   2636648K
  16G                     |   3162496K
  32G                     |   4213124K
*/
#endregion
