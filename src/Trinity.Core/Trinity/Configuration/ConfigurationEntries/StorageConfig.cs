using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Storage;
using Trinity.Utilities;

namespace Trinity.Configuration
{
    public sealed class StorageConfig
    {
        static StorageConfig s_StorageConfig = new StorageConfig();
        private StorageConfig() { m_StorageRoot = DefaultStorageRoot; }
        [ConfigInstance]
        public static StorageConfig Instance { get { return s_StorageConfig; } }
        [ConfigEntryName]
        internal static string ConfigEntry { get { return ConfigurationConstants.Tags.STORAGE; } }

        #region Private static helpers
        private static void ThrowCreatingStorageRootException(string storageroot)
        {
            throw new IOException("WARNNING: Error occurs when creating StorageRoot: " + storageroot);
        }

        private static void ThrowLargeObjectThresholdException()
        {
            throw new InvalidOperationException("LargeObjectThreshold cannot be larger than 16MB.");
        }

        private static void ThrowDisableReadOnlyException()
        {
            throw new InvalidOperationException("ReadOnly flag cannot be disabled once enabled.");
        }

        private static string DefaultStorageRoot { get { return Path.Combine(AssemblyPath.MyAssemblyPath, "storage"); } }
        #endregion

        internal const int    c_MaxTrunkCount = 256;
        internal const ushort c_UndefinedCellType = 0;
        internal int          m_GCParallelism = 16;
        internal int          m_DefragInterval = 600;
        private  string       m_StorageRoot = "";

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

        [ConfigSetting(Optional: true)]
        public unsafe string StorageRoot
        {
            get
            {
                if (m_StorageRoot == null || m_StorageRoot.Length == 0)
                {
                        m_StorageRoot = DefaultStorageRoot;
                }

                if (!Directory.Exists(m_StorageRoot))
                {
                    try
                    {
                        Directory.CreateDirectory(m_StorageRoot);
                    }
                    catch (Exception)
                    {
                        ThrowCreatingStorageRootException(m_StorageRoot);
                    }
                }

                if (m_StorageRoot[m_StorageRoot.Length - 1] != Path.DirectorySeparatorChar)
                {
                    m_StorageRoot = m_StorageRoot + Path.DirectorySeparatorChar;
                }

                try
                {
                    byte[] buff = BitHelper.GetBytes(m_StorageRoot);
                    fixed (byte* p = buff)
                    {
                        CTrinityConfig.SetStorageRoot(p, buff.Length);
                    }
                }
                catch (Exception) { }

                return m_StorageRoot;
            }

            set
            {
                m_StorageRoot = value;
                if (m_StorageRoot == null || m_StorageRoot.Length == 0)
                {
                    m_StorageRoot = DefaultStorageRoot;
                }

                if (m_StorageRoot[m_StorageRoot.Length - 1] != Path.DirectorySeparatorChar)
                {
                    m_StorageRoot += Path.DirectorySeparatorChar;
                }

                try
                {
                    byte[] buff = BitHelper.GetBytes(m_StorageRoot);
                    fixed (byte* p = buff)
                    {
                        CTrinityConfig.SetStorageRoot(p, buff.Length);
                    }
                }
                catch (Exception) { }

                if (!Directory.Exists(m_StorageRoot))
                {
                    try
                    {
                        Directory.CreateDirectory(m_StorageRoot);
                    }
                    catch (Exception)
                    {
                        ThrowCreatingStorageRootException(m_StorageRoot);
                    }
                }
            }
        }


        /// <summary>
        /// Default = 10 M
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
        /// Defragmentation frequency, Default Value = 600
        /// </summary>
        [ConfigSetting(Optional: true)]
        public int DefragInterval
        {
            get { return m_DefragInterval; }
            set { m_DefragInterval = value; CTrinityConfig.CSetGCDefragInterval(m_DefragInterval); }
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