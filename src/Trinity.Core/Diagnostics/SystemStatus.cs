// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using Trinity.Win32;
using System.Globalization;

namespace Trinity.Diagnostics
{
    struct WorkloadInfo
    {
        internal long AvailableMemoryBytes;
        internal long CellCount;
        internal long SliceSize;
        internal int SliceCount;

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("AvailableMemoryBytes: ").AppendLine(AvailableMemoryBytes.ToString(CultureInfo.InvariantCulture));
            sb.Append("CellCount: ").AppendLine(CellCount.ToString(CultureInfo.InvariantCulture));
            sb.Append("SliceSize: ").AppendLine(SliceSize.ToString(CultureInfo.InvariantCulture));
            sb.Append("SliceCount: ").AppendLine(SliceCount.ToString(CultureInfo.InvariantCulture));
            return sb.ToString();
        }
    }

    static class SystemStatus
    {
        public static WorkloadInfo Workloads
        {
            get
            {
                WorkloadInfo load;
                load.AvailableMemoryBytes = PerformanceMonitor.AvailableMemoryBytes;
                int sliceCount = 0;
                long cellCount = 0;
                long sliceSize = 0;

                //TODO:
                //foreach (var trunk in Global.LocalStorage.memory_trunks)
                //{
                //    sliceCount++;
                //    cellCount += trunk.CellCount;
                //    sliceSize += trunk.TrunkSize;
                //}
                load.CellCount = cellCount;
                load.SliceSize = sliceSize;
                load.SliceCount = sliceCount;

                return load;
            }
        }
    }
}
