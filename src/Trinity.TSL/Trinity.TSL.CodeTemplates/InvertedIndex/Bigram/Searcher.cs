using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Core.Lib;
using Trinity.TSL;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
namespace t_Namespace.InvertedIndex
{
    [TARGET("NTSL")]
    struct PairInfo
    {
        public int pos;
        public byte A;
        public byte B;
    }

    internal unsafe class InvertedBigramSearcher
    {
        string bg_index_file; //bigram index file
        string index_file;
        string index_data_dir;
        string temp_dir;

        long[] bg_index;
        byte* IndexBufferPointer;
        BinaryReader br_index;

        private bool InMemory = true;
        private bool disposed = false;

        static char[] wildcards = new char[] { '*' };

        internal InvertedBigramSearcher(bool inMemory = true, string index_name = "")
        {
            index_data_dir = Path.Combine(TrinityConfig.StorageRoot, @"t_Namespace.InvertedIndex\", index_name);
            temp_dir = Path.Combine(index_data_dir, @"temp\");

            bg_index_file = Path.Combine(index_data_dir, "bg.bgi");
            index_file = Path.Combine(index_data_dir, "index.bgi");

            InMemory = inMemory;
            LoadIndex();
        }

        internal void Dispose()
        {
            if (!this.disposed)
            {
                if (InMemory)
                {
                    Memory.free(IndexBufferPointer);
                    IndexBufferPointer = null;
                }
                else
                    br_index.Dispose();
                this.disposed = true;
            }
        }

        ~InvertedBigramSearcher()
        {
            Dispose();
        }

        private unsafe void LoadIndex()
        {
            byte[] buffer = new byte[65536 << 3];

            using (FileStream fs = new FileStream(bg_index_file, FileMode.Open, FileAccess.Read, FileShare.Read, 1 << 10, false))
            {
                fs.Read(buffer, 0, buffer.Length);
            }

            bg_index = new long[65536];

            fixed (byte* bp = buffer)
            {
                long* p = (long*)bp;
                for (int i = 0; i < 65536; i++)
                {
                    bg_index[i] = *(p + i);
                }
            }

            if (InMemory)
            {
                FileInfo fi = new FileInfo(index_file);

                long index_len = fi.Length;

                IndexBufferPointer = (byte*)Memory.malloc((ulong)index_len);

                int buff_len = 32 << 20;

                byte[] reading_buffer = new byte[buff_len];

                using (FileStream fs = new FileStream(index_file, FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    long p = 0;
                    while (index_len > 0)
                    {
                        if (index_len > buff_len)
                        {
                            if (fs.Read(reading_buffer, 0, buff_len) == buff_len)
                            {
                                Memory.Copy(reading_buffer, IndexBufferPointer + p, buff_len);
                                index_len -= buff_len;
                                p += buff_len;
                            }
                            else
                            {
                                Console.WriteLine("Read index file error.");
                            }
                        }
                        else
                        {
                            fs.Read(reading_buffer, 0, (int)index_len);
                            Memory.Copy(reading_buffer, IndexBufferPointer + p, (int)index_len);
                            index_len = 0;
                        }
                    }
                }
            }
            else
                br_index = new BinaryReader(new FileStream(index_file, FileMode.Open, FileAccess.Read, FileShare.Read, 512 << 10, false));
        }

        /// <summary>
        /// Performs a substring search.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        internal List<long> SubstringSearch(string query)
        {
            List<long> Result = new List<long>();
            if (query.Trim().Length == 0)
                return Result;

            string q = query.ToLowerInvariant();
            byte[] bytes = Encoding.UTF8.GetBytes(q);

            if (bytes.Length < 2 || bytes.Length > 255)
                return Result;

            List<PairInfo> pairList = SplitKeyword(bytes);

            if (pairList.Count == 0)
                return Result;

            //var _result = SearchWord(pairList);
            var _result = SearchSubString(pairList);
            return _result == null ? Result : _result;
        }

        /// <summary>
        /// Performs a wildcard search for the specified query.
        /// </summary>
        /// <param name="query">A query string that can contain arbitrary number of wildcard symbol *.</param>
        /// <returns>A list of matched cell Ids.</returns>
        internal List<long> WildcardSearch(string query)
        {
            return SubstringSearch(query.Split(wildcards, StringSplitOptions.RemoveEmptyEntries));
        }

        /// <summary>
        /// Performs a substring search for a list of keywords. The match pattern is:
        /// keywords[0]*keywords[1]..., where * is the wildcard symbol.
        /// </summary>
        /// <param name="keywords"></param>
        /// <returns></returns>
        internal List<long> SubstringSearch(List<string> keywords)
        {
            return SubstringSearch(keywords.ToArray());
        }

        /// <summary>
        /// Performs a substring search using the specified keywords. The match pattern is:
        /// keywords[0]*keywords[1]..., where * is the wildcard symbol.
        /// </summary>
        /// <param name="keywords">A list of keywords.</param>
        /// <returns>A list of matched cell Ids.</returns>
        internal List<long> SubstringSearch(params string[] keywords)
        {
            if (keywords == null || keywords.Length == 0)
                return new List<long>(0);

            if (keywords.Length == 1)
                return SubstringSearch(keywords[0]);

            List<long> Result = new List<long>();

            List<List<IndexItem>> partialResults = new List<List<IndexItem>>(keywords.Length);

            for (int i = 0; i < keywords.Length; i++)
            {
                string query = keywords[i];

                if (query.Trim().Length == 0)
                    return new List<long>(0);

                string q = query.ToLowerInvariant();
                byte[] bytes = Encoding.UTF8.GetBytes(q);

                if (bytes.Length < 2 || bytes.Length > 255)
                    return new List<long>(0);

                List<PairInfo> pairList = SplitKeyword(bytes);

                if (pairList.Count == 0)
                    return new List<long>(0);

                var _result = SearchSubString4WildcardSearch(pairList);
                if (_result == null || _result.Count == 0)
                    return new List<long>(0);

                partialResults.Add(_result);
            }

            #region Another K-way search, similar to that of SearchSubString
            /****************************************************************************************************
             * Each element in the partialResults represents the result list of a substring in the keywords list.
             * Each IndexItem list is sorted by IndexItem.CellId and IndexItem.Offset
             * We perform k-way search starting from the first indexitem of the first list
             * For a cellId at partialResults[0][iterator_0],
             * if we can successfully go through all the pairCount elements in IndexItemList,
             * then we found a match. Otherwise, we continue by checking the next elements in IndexItemList[0].
             *
             * Successfully going through all the elements in IndexItemList means:
             * The distance between the current item offset in partialResults[i] and
             * the current item offset in partialResults[i-1] >= keywords[i-1].Length.
             * ***************************************************************************************************/
            int keywordCount = keywords.Length;
            int[] IteratorList = new int[keywordCount]; // with default initialized value 0

            int iterator_0 = 0; // iterator for the first IndexItem list
            long cellId = long.MinValue;
            int offset = 0;
            do
            {
                IndexItem current_item = partialResults[0][iterator_0];
                if (current_item.CellId >= cellId)
                {
                    offset = current_item.Offset;
                    int i = 1;
                    for (; i < keywordCount; i++)
                    {
                        IndexItem item = new IndexItem();
                        bool match = false;
                        int j = IteratorList[i];
                        // skip the items whose CellIds are smaller than the current CellId
                        for (; j < partialResults[i].Count; j++)
                        {
                            item = partialResults[i][j];
                            if (item.CellId >= current_item.CellId)
                            {
                                break;
                            }
                        }
                        if (j == partialResults[i].Count) // We reach the end of the current index item list
                            return Result;

                        IteratorList[i] = j;
                        if (item.CellId > current_item.CellId) // did not find any item whose CellId equals the current CellId
                        {
                            cellId = item.CellId;
                            iterator_0++; // jump to the first index item, and check the next index item
                            break;
                        }

                        // _item.CellId == item.CellId
                        Debug.Assert(item.CellId == current_item.CellId);
                        do
                        {
                            item = partialResults[i][j];
                            int distance = item.Offset - offset;
                            if (distance >= keywords[i - 1].Length)
                            {
                                offset = item.Offset;
                                match = true;
                                break;
                            }

                            j++;
                        } while (j < partialResults[i].Count && partialResults[i][j].CellId == current_item.CellId);

                        if (!match)
                        {
                            iterator_0++;
                            break;
                        }
                    }
                    if (i == keywordCount)
                    {
                        Result.Add(current_item.CellId);
                        while (iterator_0 < partialResults[0].Count && current_item.CellId == partialResults[0][iterator_0].CellId)
                            iterator_0++;
                    }
                }
                else
                    iterator_0++; // skip current item if its CellId is smaller than the current CellId
            } while (iterator_0 < partialResults[0].Count);
            #endregion
            return Result;
        }

        /// <summary>
        /// Split a key word into a list of bigrams.
        /// </summary>
        /// <param name="bytes">A array of bytes whose length is larger than 1.</param>
        /// <returns>pairList</returns>
        private static List<PairInfo> SplitKeyword(byte[] bytes)
        {
            var pairList = new List<PairInfo>();
            PairInfo pi;
            int pos = 0;
            int p = 0;
            byte pre_byte = 0;

            do
            {
                if ((p + 1) != bytes.Length)
                {
                    pi.A = bytes[p];
                    pi.B = bytes[p + 1];
                    pi.pos = pos;
                    pairList.Add(pi);

                    pre_byte = bytes[p + 1];

                    pos += 2;
                    p += 2;
                }
                else // bytes[p] is the last byte
                {
                    pi.A = pre_byte;
                    pi.B = bytes[p];
                    pi.pos = pos - 1;
                    pairList.Add(pi);

                    pos++;
                    p++;
                }
            } while (p != bytes.Length);
            return pairList;
        }

        private void ReleaseGCHandle(List<GCHandle> gchandlers)
        {
            foreach (var gchandle in gchandlers)
            {
                gchandle.Free();
            }
        }

        private List<long> SearchSubString(List<PairInfo> pairList)
        {
            List<GCHandle> gchandlers = null;
            if (!InMemory)
                gchandlers = new List<GCHandle>();

            List<long> Result = new List<long>();
            int pairCount = pairList.Count;
            if (pairCount == 0)
                return Result;

            IndexItem*[] IndexItemList = new IndexItem*[pairCount];
            int[] DistanceList = new int[pairCount];
            int[] ItemCountList = new int[pairCount];
            int[] IteratorList = new int[pairCount]; // with default initialized value 0

            int offset = pairList[0].pos;
            for (int i = 0; i < pairCount; i++)
            {
                if (InMemory)
                    IndexItemList[i] = ReadIndexItemListFromRAM(pairList[i].A, pairList[i].B, out ItemCountList[i]);
                else
                    IndexItemList[i] = ReadIndexItemListFromDisk(pairList[i].A, pairList[i].B, out ItemCountList[i], ref gchandlers);
                DistanceList[i] = pairList[i].pos - offset;
                offset = pairList[i].pos;
                if (ItemCountList[i] == 0)
                {
                    if (!InMemory)
                        ReleaseGCHandle(gchandlers);
                    return null;
                }
            }

            #region K-way search
            /****************************************************************************************************
             * Each element in the IndexItemList represents a bigram pair (e.g., ab or bc).
             * Each IndexItem is sorted by IndexItem.CellId and IndexItem.Offset
             * We perform k-way search starting from IndexItemList[0]
             * For a cellId at IndexItemList[0][iterator_0],
             * if we can successfully go through all the pairCount elements in IndexItemList,
             * then we found a match. Otherwise, we continue by checking the next elements in IndexItemList[0].
             *
             * Successfully going through all the elements in IndexItemList means:
             * The distance between the current item offset in IndexItemList[i] and
             * the current item offset in IndexItemList[i-1] equals DistanceList[i].
             * ***************************************************************************************************/
            int iterator_0 = 0; // iterator for the first IndexItem list IndexItemList[0]
            long cellId = long.MinValue;
            offset = 0;
            do
            {
                IndexItem current_item = IndexItemList[0][iterator_0];
                if (current_item.CellId >= cellId)
                {
                    offset = current_item.Offset;
                    int i = 1;
                    for (; i < pairCount; i++)
                    {
                        IndexItem item = new IndexItem();
                        bool match = false;
                        int j = IteratorList[i];
                        // skip the items whose CellIds are smaller than the current CellId
                        for (; j < ItemCountList[i]; j++)
                        {
                            item = IndexItemList[i][j];
                            if (item.CellId >= current_item.CellId)
                            {
                                break;
                            }
                        }
                        if (j == ItemCountList[i]) // We reach the end of one index item list for a bigram pair
                        {
                            if (!InMemory)
                                ReleaseGCHandle(gchandlers);
                            return Result;
                        }

                        IteratorList[i] = j;
                        if (item.CellId > current_item.CellId) // did not find any item whose CellId equals the current CellId
                        {
                            cellId = item.CellId;
                            iterator_0++; // jump to the first index item, and check the next index item
                            break;
                        }

                        // _item.CellId == item.CellId
                        Debug.Assert(item.CellId == current_item.CellId);
                        do
                        {
                            item = IndexItemList[i][j];
                            int distance = item.Offset - offset;
                            if (distance == DistanceList[i])
                            {
                                offset = item.Offset;
                                match = true;
                                break;
                            }
                            else if (distance > DistanceList[i])
                                break;
                            j++;
                        } while (j < ItemCountList[i] && IndexItemList[i][j].CellId == current_item.CellId);

                        if (!match)
                        {
                            iterator_0++;
                            break;
                        }
                    }
                    if (i == pairCount)
                    {
                        Result.Add(current_item.CellId);
                        while (iterator_0 < ItemCountList[0] && current_item.CellId == IndexItemList[0][iterator_0].CellId)
                            iterator_0++;
                    }
                }
                else
                    iterator_0++; // skip current item if its CellId is smaller than the current CellId
            } while (iterator_0 < ItemCountList[0]);
            #endregion
            if (!InMemory)
                ReleaseGCHandle(gchandlers);
            return Result;
        }

        /// <summary>
        /// The only difference from SearchSubString is that the returning result is a List of IntexIndex.
        /// </summary>
        /// <param name="pairList"></param>
        /// <returns></returns>
        private List<IndexItem> SearchSubString4WildcardSearch(List<PairInfo> pairList)
        {
            List<GCHandle> gchandlers = null;
            if (!InMemory)
                gchandlers = new List<GCHandle>();

            List<IndexItem> Result = new List<IndexItem>();
            int pairCount = pairList.Count;
            if (pairCount == 0)
                return Result;

            IndexItem*[] IndexItemList = new IndexItem*[pairCount];
            int[] DistanceList = new int[pairCount];
            int[] ItemCountList = new int[pairCount];
            int[] IteratorList = new int[pairCount]; // with default initialized value 0

            int offset = pairList[0].pos;
            for (int i = 0; i < pairCount; i++)
            {
                if (InMemory)
                    IndexItemList[i] = ReadIndexItemListFromRAM(pairList[i].A, pairList[i].B, out ItemCountList[i]);
                else
                    IndexItemList[i] = ReadIndexItemListFromDisk(pairList[i].A, pairList[i].B, out ItemCountList[i], ref gchandlers);
                DistanceList[i] = pairList[i].pos - offset;
                offset = pairList[i].pos;
                if (ItemCountList[i] == 0)
                {
                    if (InMemory)
                        ReleaseGCHandle(gchandlers);
                    return null;
                }
            }

            #region K-way search
            /****************************************************************************************************
             * Each element in the IndexItemList represents a bigram pair (e.g., ab or bc).
             * Each IndexItem is sorted by IndexItem.CellId and IndexItem.Offset
             * We perform k-way search starting from IndexItemList[0]
             * For a cellId at IndexItemList[0][iterator_0],
             * if we can successfully go through all the pairCount elements in IndexItemList,
             * then we found a match. Otherwise, we continue by checking the next elements in IndexItemList[0].
             *
             * Successfully going through all the elements in IndexItemList means:
             * The distance between the current item offset in IndexItemList[i] and
             * the current item offset in IndexItemList[i-1] equals DistanceList[i].
             * ***************************************************************************************************/
            int iterator_0 = 0; // iterator for the first IndexItem list IndexItemList[0]
            long cellId = long.MinValue;
            offset = 0;
            do
            {
                IndexItem current_item = IndexItemList[0][iterator_0];
                if (current_item.CellId >= cellId)
                {
                    offset = current_item.Offset;
                    int i = 1;
                    for (; i < pairCount; i++)
                    {
                        IndexItem item = new IndexItem();
                        bool match = false;
                        int j = IteratorList[i];
                        // skip the items whose CellIds are smaller than the current CellId
                        for (; j < ItemCountList[i]; j++)
                        {
                            item = IndexItemList[i][j];
                            if (item.CellId >= current_item.CellId)
                            {
                                break;
                            }
                        }
                        if (j == ItemCountList[i]) // We reach the end of one index item list for a bigram pair
                        {
                            if (InMemory)
                                ReleaseGCHandle(gchandlers);
                            return Result;
                        }

                        IteratorList[i] = j;
                        if (item.CellId > current_item.CellId) // did not find any item whose CellId equals the current CellId
                        {
                            cellId = item.CellId;
                            iterator_0++; // jump to the first index item, and check the next index item
                            break;
                        }

                        // _item.CellId == item.CellId
                        Debug.Assert(item.CellId == current_item.CellId);
                        do
                        {
                            item = IndexItemList[i][j];
                            int distance = item.Offset - offset;
                            if (distance == DistanceList[i])
                            {
                                offset = item.Offset;
                                match = true;
                                break;
                            }
                            else if (distance > DistanceList[i])
                                break;
                            j++;
                        } while (j < ItemCountList[i] && IndexItemList[i][j].CellId == current_item.CellId);

                        if (!match)
                        {
                            iterator_0++;
                            break;
                        }
                    }
                    if (i == pairCount)
                    {
                        Result.Add(current_item);
                        while (iterator_0 < ItemCountList[0] && current_item.CellId == IndexItemList[0][iterator_0].CellId)
                            iterator_0++;
                    }
                }
                else
                    iterator_0++; // skip current item if its CellId is smaller than the current CellId
            } while (iterator_0 < ItemCountList[0]);
            #endregion
            if (InMemory)
                ReleaseGCHandle(gchandlers);
            return Result;
        }

        private IndexItem* ReadIndexItemListFromDisk(int a, int b, out int count, ref List<GCHandle> gchandlers)
        {
            long pos = bg_index[(a << 8) + b];
            if (pos == -1)
            {
                count = 0;
                return null;
            }
            br_index.BaseStream.Seek(pos, SeekOrigin.Begin);
            count = br_index.ReadInt32();
            if (count == 0)
                return null;

            int length = sizeof(IndexItem) * count;
            byte[] buffer = new byte[length];
            br_index.Read(buffer, 0, buffer.Length);

            var gchandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            gchandlers.Add(gchandle);
            return (IndexItem*)gchandle.AddrOfPinnedObject().ToPointer();
        }

        private IndexItem* ReadIndexItemListFromRAM(int a, int b, out int count)
        {
            long pos = bg_index[(a << 8) + b];
            if (pos == -1)
            {
                count = 0;
                return null;
            }

            count = *(int*)(IndexBufferPointer + pos);
            if (count == 0)
                return null;

            return (IndexItem*)(IndexBufferPointer + pos + sizeof(int));
        }
    }
}
