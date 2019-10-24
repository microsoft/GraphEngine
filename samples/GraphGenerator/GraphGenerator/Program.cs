// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            GraphGen gg = new GraphGen(1 << 20, 16, 100);
            gg.CreateGraph();
        }
    }
}
