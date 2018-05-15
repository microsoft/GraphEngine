// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Trinity.Diagnostics;

namespace Trinity.Utilities
{
    internal enum SortOrder
    {
        Increase, Decrease
    }

    internal delegate int GetChannelFunc<Row>(Row r);
    internal delegate bool ReadRowFunc<Row>(int channel, BinaryReader br, out Row r);
    internal delegate void WriteRowFunc<Row>(BinaryWriter bw, Row r);
    internal delegate bool IsNonNullRowFunc<Row>(Row r);
    internal delegate bool RowNotEqualFunc<Row>(Row r1, Row r2);

    internal class KWayMerge<Row>
    {
        static int input_fs_buff = 32 << 20;
        static int output_fs_buff = 1 << 30;

        Row null_row;
        Row previous_row;
        Comparison<Row> compare;
        GetChannelFunc<Row> GetChannel;
        ReadRowFunc<Row> ReadRow;
        WriteRowFunc<Row> WriteRow;
        IsNonNullRowFunc<Row> IsNonNullRow;
        RowNotEqualFunc<Row> NotEqual;

        SortOrder sort_order;

        public KWayMerge(SortOrder order,
            Row null_row,
            GetChannelFunc<Row> get_channel,
            Comparison<Row> comparison,
            ReadRowFunc<Row> read_row, 
            WriteRowFunc<Row> write_row,
            IsNonNullRowFunc<Row> not_null, 
            RowNotEqualFunc<Row> not_equal
            )
        {
            sort_order = order;
            this.null_row = null_row;
            this.GetChannel = get_channel;
            this.compare = comparison;
            this.ReadRow = read_row;
            this.WriteRow = write_row;
            this.IsNonNullRow = not_null;
            this.NotEqual = not_equal;

            previous_row = null_row;
            compare = comparison;
        }

        public void Merge(string[] files, string merged_file)
        {
            #region initialization
            FileUtility.CompletePath(Path.GetDirectoryName(merged_file), true);

            FileStream fs = new FileStream(merged_file, FileMode.Create, FileAccess.Write, FileShare.ReadWrite, output_fs_buff);

            BinaryWriter bw = new BinaryWriter(fs);

            int k = files.Length;

            FileStream[] fs_array = new FileStream[k];
            BinaryReader[] br_array = new BinaryReader[k];

            for (int i = 0; i < k; i++)
            {
                fs_array[i] = new FileStream(files[i], FileMode.Open, FileAccess.Read, FileShare.ReadWrite, input_fs_buff);
                br_array[i] = new BinaryReader(fs_array[i]);
            }
            #endregion

            Row[] heap = new Row[k];
            for (int i = 0; i < k; i++)
            {
                ReadRow(i, br_array[i], out heap[i]);
            }

            if (sort_order == SortOrder.Increase)
            {
                MinHeap.BuildHeap<Row>(heap, compare);
            }
            else
            {
                MaxHeap.BuildHeap<Row>(heap, compare);
            }
            int channel = -1;

            long count = 0;
            while (true)
            {
                if (IsNonNullRow(heap[0]))
                {
                    if (NotEqual(heap[0], previous_row))
                    {
                        WriteRow(bw, heap[0]);
                        previous_row = heap[0];
                        count++;
                    }
                    channel = GetChannel(heap[0]);
                    ReadRow(channel, br_array[channel], out heap[0]);
                    if (sort_order == SortOrder.Increase)
                    {
                        MinHeap.Adjust<Row>(heap, 0, compare);
                    }
                    else
                    {
                        MaxHeap.Adjust<Row>(heap, 0, compare);
                    }
                }
                else
                {
                    break;
                }
                if (count % 1000000 == 0)
                {
                    Log.WriteLine($"{count}");
                }
            }

            #region Cleanning
            for (int i = 0; i < k; i++)
            {
                br_array[i].Close();
                if (br_array[i] != null)
                    br_array[i].Dispose();
            }
            bw.Close();
            if (bw != null)
                bw.Dispose();
            #endregion
        }


    }
}
