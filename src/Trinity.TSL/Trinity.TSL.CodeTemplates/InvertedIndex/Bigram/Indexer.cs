using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Utilities;

using Trinity.TSL;
using Trinity;
using System.Globalization;
/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace.InvertedIndex
{
    [TARGET("NTSL")]
    public unsafe class InvertedBigramIndexer
    {
        const int CacheSize = 1024;
        long[] bg_index;

        IndexItem[][] index = new IndexItem[65536][];
        int[] indexItemCount = new int[65536];

        uint TotalIndexedItemCount = 0;
        long IndexFileOffset = 0;

        BinaryWriter bw_index;

        string index_data_dir;
        string temp_dir;
        private bool disposed = false;

        string tmp_bg_index_file;
        string tmp_index_file;

        string bg_index_file; //bigram index file
        string index_file;

        public InvertedBigramIndexer(string index_name = "")
        {
            index_data_dir = Path.Combine(TrinityConfig.StorageRoot, @"t_Namespace.InvertedIndex\", index_name);
            temp_dir = Path.Combine(index_data_dir, @"temp\");

            try
            {
                Directory.Delete(temp_dir, true);
            }
            catch (Exception) { }

            FileUtility.CompletePath(index_data_dir, true);
            FileUtility.CompletePath(temp_dir, true);

            tmp_bg_index_file = Path.Combine(temp_dir, "bg.bgi");
            tmp_index_file    = Path.Combine(temp_dir, "index.bgi");

            bg_index_file     = Path.Combine(index_data_dir, "bg.bgi");
            index_file        = Path.Combine(index_data_dir, "index.bgi");

            bw_index = new BinaryWriter(new FileStream(tmp_index_file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, 512 << 10, false));

            #region Create folders for bigram indices
            for (int i = 0; i < 256; i++)
            {
                string path_L1 = temp_dir + i + "\\";
                Directory.CreateDirectory(path_L1);
            }
            #endregion
        }

        public void BuildIndex()
        {
            #region Flush all bigram indices
            for (int i = 0; i <= 255; i++)
            {
                for (int j = 0; j <= 255; j++)
                {
                    int idx = (i << 8) + j;
                    if (index[idx] != null && indexItemCount[idx] != 0)
                    {
                        FlushIndex((byte)i, (byte)j);
                    }
                }
            }
            #endregion

            BuildBigramIndex();

            FlushAll();

            Commit();
        }

        public void AddItem(string item, long cellId)
        {
            byte[] itemBytes = Encoding.UTF8.GetBytes(item.ToLowerInvariant());

            if (itemBytes.Length < 2)
                return;

            for (int i = 0; i < itemBytes.Length - 1; i++)
            {
                if (i > ushort.MaxValue)
                    break;
                IndexBigram((int)itemBytes[i], (int)itemBytes[i + 1], cellId, (ushort)i);
            }
            TotalIndexedItemCount++;
        }

        unsafe void IndexBigram(int a, int b, long id, ushort offsetInString)
        {
            int idx = (a << 8) + b;
            if (index[idx] == null)
            {
                index[idx] = new IndexItem[CacheSize];
                indexItemCount[idx] = 0;
            }

            index[idx][indexItemCount[idx]] = new IndexItem { m_cellId = id, Offset = offsetInString };
            indexItemCount[idx]++;

            if (indexItemCount[idx] == CacheSize)
            {
                FlushIndex((int)a, (int)b);
            }
        }

        unsafe void BuildBigramIndex()
        {
            bg_index = new long[65536];
            for (int i = 0; i < 65536; i++)
                bg_index[i] = -1;

            for (int i = 0; i < 256; i++)
            {
                string path_L1 = temp_dir + i + "\\";
                for (int j = 0; j < 256; j++)
                {
                    SortBigram(path_L1 + j, i, j);
                }
            }

            byte[] buffer = new byte[65536 << 3];

            fixed (byte* bp = buffer)
            {
                long* p = (long*)bp;
                for (int i = 0; i < 65536; i++)
                {
                    *(p + i) = bg_index[i];
                }
            }

            using (FileStream fs = new FileStream(tmp_bg_index_file, FileMode.CreateNew, FileAccess.Write, FileShare.Write, 1 << 10, false))
            {
                fs.Write(buffer, 0, buffer.Length);
            }
        }

        void SortBigram(string file, int a, int b)
        {
            if (!File.Exists(file))
                return;
            using (FileStream fs = new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 10, false))
            {
                //!!!!! Be aware of this line
                byte[] buffer = new byte[(int)fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                int count = buffer.Length / sizeof(IndexItem);
                IndexItem[] indexItems = new IndexItem[count];
                fixed (byte* bp = buffer)
                {
                    IndexItem* p = (IndexItem*)bp;
                    for (int i = 0; i < count; i++)
                    {
                        indexItems[i] = *(p + i);
                    }

                    Array.Sort(indexItems, new Comparison<IndexItem>((x, y) =>
                    {
                        int ret = x.m_cellId.CompareTo(y.m_cellId);
                        if (ret != 0)
                            return ret;

                        return x.Offset.CompareTo(y.Offset);
                    }
                    ));

                    for (int i = 0; i < count; i++)
                    {
                        *(p + i) = indexItems[i];
                    }
                }
                bw_index.Write(count);
                bw_index.Write(buffer, 0, buffer.Length);
                bg_index[(a << 8) + b] = IndexFileOffset;
                IndexFileOffset += (sizeof(int) + buffer.Length);
            }
        }

        unsafe void FlushIndex(int a, int b)
        {
            int idx = (a << 8) + b;
            string bigram_index_file = string.Format(CultureInfo.InvariantCulture, "{0}\\{1}\\{2}", temp_dir, a, b);

            byte[] buffer = new byte[indexItemCount[idx] * sizeof(IndexItem)];
            fixed (byte* bp = buffer)
            {
                IndexItem* p = (IndexItem*)bp;
                for (int i = 0; i < indexItemCount[idx]; i++)
                    *(p + i) = index[idx][i];
            }

            using (FileStream fs = new FileStream(bigram_index_file, FileMode.Append, FileAccess.Write, FileShare.Write, 1 << 10, false))
            {
                fs.Write(buffer, 0, buffer.Length);
            }

            indexItemCount[idx] = 0;
        }

        void FlushAll()
        {
            bw_index.Flush();

            bw_index.Close();
        }

        void Commit()
        {
            try
            {
                File.Delete(bg_index_file);
                File.Delete(index_file);
            }
            catch (Exception) { }

            File.Move(tmp_bg_index_file, bg_index_file);
            File.Move(tmp_index_file, index_file);

            Directory.Delete(temp_dir, true);
        }

        public void Dispose()
        {
            if (!this.disposed)
            {
                bw_index.Dispose();
                this.disposed = true;
            }
        }

        ~InvertedBigramIndexer()
        {
            Dispose();
        }
    }
}
