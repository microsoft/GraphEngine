// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Utilities
{
    class ComparerFactory
    {
        public static Comparison<int> Int32Comparer = (int x, int y) =>
           {
               return x > y ? 1 : (x < y ? -1 : 0);
           };

        public static Comparison<uint> UInt32Comparer = (uint x, uint y) =>
        {
            return x > y ? 1 : (x < y ? -1 : 0);
        };

        public static Comparison<long> Int64Comparer = (long x, long y) =>
        {
            return x > y ? 1 : (x < y ? -1 : 0);
        };

        public static Comparison<float> FloatComparer = (float x, float y) =>
        {
            return x > y ? 1 : (x < y ? -1 : 0);
        };

        public static Comparison<double> DoubleComparer = (double x, double y) =>
        {
            return x > y ? 1 : (x < y ? -1 : 0);
        };
    }
}
