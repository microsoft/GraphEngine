// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Trinity.Configuration;
using Trinity.Core.Lib;
using Trinity.Diagnostics;
using Trinity.TSL.Lib;
using Trinity.Utilities;

namespace Trinity.Storage
{

    #region Write-ahead-log and image signature data structures
    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 16)]
    internal unsafe struct MD5_SIGNATURE
    {
        public long LowBits;
        public long HighBits;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = sizeof(ulong) + 16 * 256)]
    internal unsafe struct TRINITY_IMAGE_SIGNATURE
    {
        public ulong       IMAGE_VERSION;
        public fixed byte  TRUNK_SIGNATURES[256 * 16];
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 1 + 8 + 2 + 4)]
    internal unsafe struct LOG_RECORD_HEADER
    {
        /// <summary>
        /// As the LOG_RECORD_HEADER is only used within LocalMemoryStorage,
        /// which already calls InternalCalls.__init(), it is not necessary to
        /// make a C wrapper for this struct.
        /// </summary>
        static LOG_RECORD_HEADER() { InternalCalls.__init(); }

        public long   CELL_ID;
        public int    CONTENT_LEN;
        public ushort CELL_TYPE;
        public byte   CHECKSUM; // 8-bit second-order check

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern void CWriteAheadLogComputeChecksum(LOG_RECORD_HEADER* plog, byte* bufferPtr);


        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern bool CWriteAheadLogValidateChecksum(LOG_RECORD_HEADER* plog, byte* content);

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 8 + sizeof(ulong) + 16 * 256)]
    internal unsafe struct LOG_FILE_HEADER
    {
        public fixed byte              LOG_MAGIC_HEAD[4];
        public ushort                  LOG_VER_MINOR;
        public ushort                  LOG_VER_MAJOR;
        public TRINITY_IMAGE_SIGNATURE LOG_ASSOCIATED_IMAGE_SIGNATURE;

        public static LOG_FILE_HEADER New()
        {
            LOG_FILE_HEADER ret = new LOG_FILE_HEADER();
            ret.Initialize();
            return ret;
        }

        internal void Initialize()
        {
            fixed (LOG_FILE_HEADER *p = &this)
            {
                p->LOG_MAGIC_HEAD[0] = 0x54;
                p->LOG_MAGIC_HEAD[1] = 0x4c;
                p->LOG_MAGIC_HEAD[2] = 0x4f;
                p->LOG_MAGIC_HEAD[3] = 0x47;// "TLOG"
            }

            LOG_VER_MAJOR = 1;
            LOG_VER_MINOR = 0;
        }

        internal bool CompatibilityCheck()
        {
            LOG_FILE_HEADER default_header = LOG_FILE_HEADER.New();

            fixed (LOG_FILE_HEADER* p_lhs = &this)
            {
                /* Currently we make a strict compare of magic header and version. */
                return Memory.Compare((byte*)p_lhs, (byte*)&default_header, 8);
            }
        }
    }
    #endregion

    /**
     * Note: There are a lot of stdio operations in the code in this file. 
     *       These lines are inside LocalMemoryStorage, which calls InternalCalls.__init(),
     *       so that we can call CStdio directly.
     */

    public unsafe partial class LocalMemoryStorage : Storage, IDisposable, IEnumerable<CellInfo>, IEnumerable
    {
        #region Write-ahead-log logic
        /// <summary>
        /// Tries to close the WAL file.
        /// Only called when the local storage is being disposed.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private unsafe void CloseWriteAheadLogFile()
        {
            if (m_logfile == null) return;
            if (0 != CStdio.fclose(m_logfile))
            {
                Log.WriteLine(LogLevel.Error, "Failed to close the log file");
            }
        }

        /// <summary>
        /// Initialises the write-ahead logging file associated
        /// with the primary image.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private unsafe void InitializeWriteAheadLogFile()
        {
            if (TrinityConfig.ReadOnly)
                return;

            Log.WriteLine(LogLevel.Info, "Initializing logging facility");

            try
            {

                LoadWriteAheadLogFile();
                /* After loading, the log file will be dropped. 
                 * So we proceed to create a new one.  
                 */
                CreateWriteAheadLogFile();

            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Warning, "Failed to setup the log-ahead directory: {0}", ex);
            }
        }

        private unsafe void _update_write_ahead_log_file(string path, void* fp)
        {
            m_logfile      = fp;
            m_logfile_path = path;
            CLocalMemoryStorage.CSetWriteAheadLogFile(fp);
        }

        /// <summary>
        /// Drops the current log file and clean up the member variables.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private unsafe void DropWriteAheadLogFile()
        {
            if (TrinityConfig.ReadOnly)
                return;

            if (m_logfile == null)
                return;

            Debug.Assert(m_logfile      != null);
            Debug.Assert(m_logfile_path != null);

            Log.WriteLine(LogLevel.Info, "Dropping write-ahead-log file {0}", m_logfile_path);

            if (0 != CStdio.fclose(m_logfile))
            {
                Log.WriteLine(LogLevel.Error, "Failed to close the log file");
            }

            try
            {
                File.Delete(m_logfile_path);
            }
            catch (Exception ex)
            {
                Log.WriteLine(LogLevel.Error, "Failed to delete the log file: {0}", ex);
            }

            _update_write_ahead_log_file(null, null);
        }

        /// <summary>
        /// Creates a new log file for current storage.
        /// If the file exists, it will be overwritten.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private unsafe void CreateWriteAheadLogFile()
        {
            if (TrinityConfig.ReadOnly)
                return;

            string path = WriteAheadLogFilePath;

            Log.WriteLine(LogLevel.Info, "Creating write-ahead log file {0}", path);

            DropWriteAheadLogFile();

            if (File.Exists(path))
            {
                BackupWriteAheadLogFile(path);
            }

            void* new_fp = null;
            if (0 != Stdio._wfopen_s(out new_fp, path, "wb"))
            {
                Log.WriteLine(LogLevel.Error, "Cannot open the log file");
                return;
            }

            LOG_FILE_HEADER header = LOG_FILE_HEADER.New();

            GetTrinityImageSignature(&header.LOG_ASSOCIATED_IMAGE_SIGNATURE);

            CStdio.fwrite(&header, (ulong)sizeof(LOG_FILE_HEADER), 1, new_fp);
            CStdio.fflush(new_fp);

            _update_write_ahead_log_file(path, new_fp);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        private void ResetWriteAheadLog(string path)
        {
            if (TrinityConfig.ReadOnly)
                return;

            DropWriteAheadLogFile();

            if (File.Exists(path))
                BackupWriteAheadLogFile(path);

            InitializeWriteAheadLogFile();
        }

        /// <summary>
        /// Move the current log file to a backup(.old file).
        /// Caller should guarantee that log is not opened.
        /// </summary>
        /// <param name="path">Path of the log file to backup.</param>
        private void BackupWriteAheadLogFile(string path)
        {
            try
            {
                Log.WriteLine(LogLevel.Info, "Backing up current log file {0}", path);

                string path_old = Path.Combine(Path.GetDirectoryName(path), Path.GetFileName(path) + ".old");

                if (File.Exists(path_old))
                {
                    Log.WriteLine(LogLevel.Warning, "Deleting old log file {0}", path_old);
                    File.Delete(path_old);
                }

                Log.WriteLine(LogLevel.Info, "Moving current log file {0} to {1}", path, path_old);
                File.Move(path, path_old);
            }
            catch (Exception ex)
            {

                Log.WriteLine(LogLevel.Error, "Cannot backup the log file {0}: {1}", path, ex);

            }

        }

        /// <summary>
        /// Opens the log file in read mode and replay the actions inside,
        /// and when the logs are synced, save the storage to an image, then
        /// drop the old log file.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        private void LoadWriteAheadLogFile()
        {
            if (TrinityConfig.ReadOnly)
                return;

            string path = WriteAheadLogFilePath;

            Log.WriteLine(LogLevel.Info, "Loading write-ahead log file {0}", path);

            LOG_FILE_HEADER         header              = LOG_FILE_HEADER.New();
            TRINITY_IMAGE_SIGNATURE current_sig         = new TRINITY_IMAGE_SIGNATURE();
            LOG_RECORD_HEADER       record_header       = new LOG_RECORD_HEADER();
            long                    record_cnt          = 0;
            byte[]                  cell_buff           = new byte[128];
            void*                   new_fp              = null;
            bool                    ver_compatible      = true;
            bool                    img_compatible      = true;

            GetTrinityImageSignature(&current_sig);

            DropWriteAheadLogFile();

            if (!File.Exists(path))
            {
                Log.WriteLine(LogLevel.Info, "Write ahead log doesn't exist, quit loading.");
                return;
            }

            if (0 != Stdio._wfopen_s(out new_fp, path, "rb"))
            {
                Log.WriteLine(LogLevel.Fatal, "Cannot open write ahead log for read. Exiting.");
                goto load_fail;
            }

            /* Read log header */

            if (1 != CStdio.fread(&header, (ulong)sizeof(LOG_FILE_HEADER), 1, new_fp))
            {
                Log.WriteLine(LogLevel.Fatal, "Cannot read write-ahead-log header. Exiting.");
                goto load_fail;
            }

            ver_compatible = header.CompatibilityCheck();
            img_compatible = Memory.Compare((byte*)&header.LOG_ASSOCIATED_IMAGE_SIGNATURE, (byte*)&current_sig, sizeof(TRINITY_IMAGE_SIGNATURE));

            if (!ver_compatible || !img_compatible)
            {
                /* The log is not ours. Ignore if it's empty. */
                if (0 == CStdio.feof(new_fp))
                {
                    Log.WriteLine(LogLevel.Warning, "Found incompatible empty write-ahead-log file, ignoring.");
                    CStdio.fclose(new_fp);
                    return;
                }
                else if (this.CellCount != 0)
                {
                    goto load_incompatible;
                }
                /* Otherwise, (CellCount is 0), it indicates that we're recovering from a fresh start. */
            }

            Log.WriteLine(LogLevel.Info, "Reading log file.");

            while (1 == CStdio.fread(&record_header, (ulong)sizeof(LOG_RECORD_HEADER), 1, new_fp))
            {
                if (record_header.CONTENT_LEN >= 0)
                {
                    /* Ensure space for the cell buffer */
                    if (record_header.CONTENT_LEN > cell_buff.Length)
                    {
                        if (record_header.CONTENT_LEN < 1<<20)
                        {
                            cell_buff = new byte[record_header.CONTENT_LEN * 2];
                        }
                        else
                        {
                            cell_buff = new byte[record_header.CONTENT_LEN];
                        }
                    }

                    fixed (byte* p_buff = cell_buff)
                    {
                        if (1 != CStdio.fread(p_buff, (ulong)record_header.CONTENT_LEN, 1, new_fp) && record_header.CONTENT_LEN != 0)
                        {
                            Log.WriteLine(LogLevel.Error, "Incomplete write-ahead-log record at the end of file");
                            break;
                        }

                        if (false == LOG_RECORD_HEADER.CWriteAheadLogValidateChecksum(&record_header, p_buff))
                        {
                            Log.WriteLine(LogLevel.Fatal, "Checksum mismatch for log record #{0}", record_cnt);
                            goto load_fail;
                        }

                        this.SaveCell(record_header.CELL_ID, p_buff, record_header.CONTENT_LEN, record_header.CELL_TYPE);
                    }
                }
                else /* if (record_header.CONTENT_LEN < 0) */
                {
                    if (false == LOG_RECORD_HEADER.CWriteAheadLogValidateChecksum(&record_header, null))
                    {
                        Log.WriteLine(LogLevel.Fatal, "Checksum mismatch for log record #{0}", record_cnt);
                        goto load_fail;
                    }
                    this.RemoveCell(record_header.CELL_ID);
                }

                ++record_cnt;
            }

            goto load_success;

        ////////////////////////////////////////
        load_incompatible:

            if (ver_compatible)
            {
                Log.WriteLine(LogLevel.Fatal, "The log file is incompatible with the current version. Cannot recover.");
            }

            if (img_compatible)
            {
                Log.WriteLine(LogLevel.Fatal, "The log file has a different signature than the current image. Cannot recover.");
            }

            goto load_fail;

        ////////////////////////////////////////
        load_success:

            Log.WriteLine(LogLevel.Info, "Write-ahead-log successfully loaded. Recovered {0} records.", record_cnt);

            if (0 != CStdio.fclose(new_fp))
            {
                Log.WriteLine(LogLevel.Error, "Cannot close the write-ahead-log file. Logging disabled.");
                return;
            }

            /* Only save storage when the log is not empty. */
            if (record_cnt == 0 ? true : SaveStorage())
            {
                /* Save storage succeded. Dropping old logs now. */
                try
                {
                    File.Delete(path);
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Failed to delete the old logs: {0}", ex);
                }
            }
            else
            {
                /* Save storage failed. */
                Log.WriteLine(LogLevel.Fatal, "Failed to save the recovered storage. The old log is retained");
                goto load_fail;
            }

            return;

        ////////////////////////////////////////
        load_fail:

            if (new_fp != null)
                CStdio.fclose(new_fp);
            Environment.Exit(-1);

        }

        /// <summary>
        /// Only for unit test purpose.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void _reset_write_ahead_log_status()
        {
            if (m_logfile != null)
                CStdio.fclose(m_logfile);

            _update_write_ahead_log_file(null, null);
        }
        #endregion

        #region Overridden write-ahead logged cell interfaces

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode SaveCell(CellAccessOptions writeAheadLogOptions, long cellId, byte[] buff)
        {
            fixed (byte* p = buff)
            {
                TrinityErrorCode eResult= CLocalMemoryStorage.CSaveCell(cellId, p, buff.Length, StorageConfig.c_UndefinedCellType, writeAheadLogOptions);
                return eResult;
            }
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode SaveCell(CellAccessOptions writeAheadLogOptions, long cellId, byte[] buff, ushort cellType)
        {
            fixed (byte* p = buff)
            {
                TrinityErrorCode eResult= CLocalMemoryStorage.CSaveCell(cellId, p, buff.Length, cellType, writeAheadLogOptions);
                return eResult;
            }
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode SaveCell(CellAccessOptions writeAheadLogOptions, long cellId, byte[] buff, int offset, int cellSize)
        {
            fixed (byte* p = buff)
            {
                TrinityErrorCode eResult= CLocalMemoryStorage.CSaveCell(cellId, p + offset, cellSize, StorageConfig.c_UndefinedCellType, writeAheadLogOptions);
                return eResult;
            }
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode SaveCell(CellAccessOptions writeAheadLogOptions, long cellId, byte[] buff, int offset, int cellSize, ushort cellType)
        {
            fixed (byte* p = buff)
            {
                TrinityErrorCode eResult= CLocalMemoryStorage.CSaveCell(cellId, p + offset, cellSize, cellType, writeAheadLogOptions);
                return eResult;
            }
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode SaveCell(CellAccessOptions writeAheadLogOptions, long cellId, byte* buff, int offset, int cellSize, ushort cellType)
        {
            TrinityErrorCode eResult= CLocalMemoryStorage.CSaveCell(cellId, buff + offset, cellSize, cellType, writeAheadLogOptions);
            return eResult;
        }


        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode SaveCell(CellAccessOptions writeAheadLogOptions, long cellId, byte* buff, int offset, int cellSize)
        {
            TrinityErrorCode eResult= CLocalMemoryStorage.CSaveCell(cellId, buff + offset, cellSize, StorageConfig.c_UndefinedCellType, writeAheadLogOptions);
            return eResult;
        }

        /// <summary>
        /// Adds a new cell to the key-value store if the cell Id does not exist, or updates an existing cell in the key-value store if the cell Id already exists.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns>true if saving succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode SaveCell(CellAccessOptions writeAheadLogOptions, long cellId, byte* buff, int cellSize)
        {
            TrinityErrorCode eResult= CLocalMemoryStorage.CSaveCell(cellId, buff, cellSize, StorageConfig.c_UndefinedCellType, writeAheadLogOptions);
            return eResult;
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode AddCell(CellAccessOptions writeAheadLogOptions, long cellId, byte* buff, int cellSize)
        {
            TrinityErrorCode eResult= CLocalMemoryStorage.CAddCell(cellId, buff, cellSize, StorageConfig.c_UndefinedCellType, writeAheadLogOptions);
            return eResult;
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode AddCell(CellAccessOptions writeAheadLogOptions, long cellId, byte* buff, int offset, int cellSize)
        {
            TrinityErrorCode eResult= CLocalMemoryStorage.CAddCell(cellId, buff + offset, cellSize, StorageConfig.c_UndefinedCellType, writeAheadLogOptions);
            return eResult;
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode AddCell(CellAccessOptions writeAheadLogOptions, long cellId, byte[] buff)
        {
            fixed (byte* p = buff)
            {
                TrinityErrorCode eResult= CLocalMemoryStorage.CAddCell(cellId, p, buff.Length, StorageConfig.c_UndefinedCellType, writeAheadLogOptions);
                return eResult;
            }
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode AddCell(CellAccessOptions writeAheadLogOptions, long cellId, byte[] buff, int offset, int cellSize)
        {
            fixed (byte* p = buff)
            {
                TrinityErrorCode eResult= CLocalMemoryStorage.CAddCell(cellId, p + offset, cellSize, StorageConfig.c_UndefinedCellType, writeAheadLogOptions);
                return eResult;
            }
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode AddCell(CellAccessOptions writeAheadLogOptions, long cellId, byte[] buff, int offset, int cellSize, ushort cellType)
        {
            fixed (byte* p = buff)
            {
                TrinityErrorCode eResult= CLocalMemoryStorage.CAddCell(cellId, p + offset, cellSize, cellType, writeAheadLogOptions);
                return eResult;
            }
        }

        /// <summary>
        /// Adds a new cell to the Trinity key-value store.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <param name="cellType">Indicates the cell type.</param>
        /// <returns>true if adding succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode AddCell(CellAccessOptions writeAheadLogOptions, long cellId, byte* buff, int offset, int cellSize, ushort cellType)
        {
            TrinityErrorCode eResult= CLocalMemoryStorage.CAddCell(cellId, buff + offset, cellSize, cellType, writeAheadLogOptions);
            return eResult;
        }

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns>true if updating succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode UpdateCell(CellAccessOptions writeAheadLogOptions, long cellId, byte* buff, int offset, int cellSize)
        {
            TrinityErrorCode eResult= CLocalMemoryStorage.CUpdateCell(cellId, buff + offset, cellSize, writeAheadLogOptions);
            return eResult;
        }

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns>true if updating succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode UpdateCell(CellAccessOptions writeAheadLogOptions, long cellId, byte* buff, int cellSize)
        {
            TrinityErrorCode eResult= CLocalMemoryStorage.CUpdateCell(cellId, buff, cellSize, writeAheadLogOptions);
            return eResult;
        }

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <returns>true if updating succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode UpdateCell(CellAccessOptions writeAheadLogOptions, long cellId, byte[] buff)
        {
            fixed (byte* p = buff)
            {
                TrinityErrorCode eResult= CLocalMemoryStorage.CUpdateCell(cellId, p, buff.Length, writeAheadLogOptions);
                return eResult;
            }
        }

        /// <summary>
        /// Updates an existing cell in the Trinity key-value store.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <param name="buff">A memory buffer that contains the cell content.</param>
        /// <param name="offset">The byte offset into the buff.</param>
        /// <param name="cellSize">The size of the cell.</param>
        /// <returns>true if updating succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode UpdateCell(CellAccessOptions writeAheadLogOptions, long cellId, byte[] buff, int offset, int cellSize)
        {
            fixed (byte* p = buff)
            {
                TrinityErrorCode eResult= CLocalMemoryStorage.CUpdateCell(cellId, p + offset, cellSize, writeAheadLogOptions);
                return eResult;
            }
        }

        /// <summary>
        /// Removes the cell with the specified cell Id from the key-value store.
        /// </summary>
        /// <param name="writeAheadLogOptions">Specifies write-ahead logging behavior. Valid values are CellAccessOptions.StrongLogAhead(default) and CellAccessOptions.WeakLogAhead. Other values are ignored.</param>
        /// <param name="cellId">A 64-bit cell Id.</param>
        /// <returns>true if removing succeeds; otherwise, false.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TrinityErrorCode RemoveCell(CellAccessOptions writeAheadLogOptions, long cellId)
        {
            TrinityErrorCode eResult= CLocalMemoryStorage.CRemoveCell(cellId, writeAheadLogOptions);
            return eResult;
        }

        #endregion
    }
}
