// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
using System.Text;

using Trinity;
using Trinity.Utilities;
using Trinity.Diagnostics;
using Trinity.Storage;
using Trinity.Core.Lib;
using System.Runtime.CompilerServices;

namespace Trinity
{
    public unsafe static partial class TrinityConfig
    {
        private static void ThrowCreatingStorageRootException(string root_root)
        {
            throw new IOException("WARNNING: Error occurs when creating StorageRoot: " + storage_root);
        }

        private static void ThrowCreatingLogDirectoryException(string log_dir)
        {
            throw new IOException("WARNNING: Error occurs when creating LogDirectory: " + log_dir);
        }

        private static void ThrowLargeObjectThresholdException()
        {
            throw new InvalidOperationException("LargeObjectThreshold cannot be larger than 16MB.");
        }

        private static void ThrowDisableReadOnlyException()
        {
            throw new InvalidOperationException("ReadOnly flag cannot be disabled once enabled.");
        }

        /// <summary>
        /// Set the number of memory trunks in the Trinity memory storage.
        /// Note:
        /// 1)	This property should be switched on at the very beginning of your program.
        /// 2)	The value will be consumed during the initialization of the local memory storage.
        ///     After the local memory storage initialized, this property cannot be changed.
        /// 3)  The value of the property should be power of two between [1,256]. 
        ///     Values other than these will be ignored.
        /// </summary>
        public static int TrunkCount
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
        /// Set readonly flag of the Trinity memory storage. The caller hereby promise that it does not modify the trinity memory storage.
        /// Note:
        /// 1)	This flag should be switched on at the very beginning of your program.
        /// 2)	Once this flag is enabled, it cannot be disabled in the current process.
        /// 3)	The current process SHOULD NOT update/modify Trinity memory storage any more.  Operations, such as SaveCell(), AddCell(), cannot be called in this mode.
        /// </summary>
        public static bool ReadOnly
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
        /// Gets and Sets the current storage capacity profile.
        /// </summary>
        public static StorageCapacityProfile StorageCapacity
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

        internal const ushort UndefinedCellType = 0;
        internal static string storage_root = "";
        private static string DefaultStorageRoot
        {
            get
            {
                return MyAssemblyPath + "storage" + Path.DirectorySeparatorChar;
            }
        }

        /// <summary>
        /// Gets or sets the root data directory for the current Trinity instance.
        /// </summary>
        public static string StorageRoot
        {
            get
            {
                if (storage_root == null || storage_root.Length == 0)
                {
                    if (CurrentClusterConfig != null && CurrentClusterConfig.RunningMode == RunningMode.Server)
                    {
                        storage_root = CurrentClusterConfig.GetMyServerInfo().StorageRoot;
                        if (storage_root == null || storage_root.Length == 0)               // when the server config entry
                        {                                                                   // does not have the StorageRoot
                            storage_root = DefaultStorageRoot;                              // attribute, it will be null again
                        }                                                                   // and we have to fall back to DefaultStorageRoot.
                    }
                    else
                    {
                        storage_root = DefaultStorageRoot;
                    }
                }

                if (!Directory.Exists(storage_root))
                {
                    try
                    {
                        Directory.CreateDirectory(storage_root);
                    }
                    catch (Exception)
                    {
                        ThrowCreatingStorageRootException(storage_root);
                    }
                }

                if (storage_root[storage_root.Length - 1] != Path.DirectorySeparatorChar)
                {
                    storage_root = storage_root + Path.DirectorySeparatorChar;
                }

                try
                {
                    byte[] buff = BitHelper.GetBytes(storage_root);
                    fixed (byte* p = buff)
                    {
                        CTrinityConfig.SetStorageRoot(p, buff.Length);
                    }
                }
                catch (Exception) { }

                return storage_root;
            }

            set
            {
                storage_root = value;
                if (storage_root == null || storage_root.Length == 0)
                {
                    storage_root = DefaultStorageRoot;
                }

                if (storage_root[storage_root.Length - 1] != Path.DirectorySeparatorChar)
                {
                    storage_root += Path.DirectorySeparatorChar;
                }

                try
                {
                    byte[] buff = BitHelper.GetBytes(storage_root);
                    fixed (byte* p = buff)
                    {
                        CTrinityConfig.SetStorageRoot(p, buff.Length);
                    }
                }
                catch (Exception) { }

                if (!Directory.Exists(storage_root))
                {
                    try
                    {
                        Directory.CreateDirectory(storage_root);
                    }
                    catch (Exception)
                    {
                        ThrowCreatingStorageRootException(storage_root);
                    }
                }
            }
        }

        /// <summary>
        /// Default Value = 256
        /// </summary>
        internal const int c_MaxTrunkCount = 256;

        /// <summary>
        /// Default Value = 16
        /// </summary>
        internal static int GCParallelism = 16;


        /// <summary>
        /// Default = 10 M
        /// </summary>
        internal static int LargeObjectThreshold
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

        internal static int s_DefragInterval = 600;
        /// <summary>
        /// Defragmentation frequency, Default Value = 600
        /// </summary>
        public static int DefragInterval
        {
            get { return s_DefragInterval; }
            set { s_DefragInterval = value; CTrinityConfig.CSetGCDefragInterval(s_DefragInterval); }
        }
    }
}

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