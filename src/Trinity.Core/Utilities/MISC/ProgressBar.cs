// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Trinity.Utilities
{
    internal class ProgressBar
    {
        long totalSteps;
        long step = 0;
        long printStepSize = 0;
        HighPrecisionTimer hpt;

        public void MoveForward()
        {
            if (printStepSize == 0)
            {
                return;
            }
            if ((++step) % printStepSize == 0)
            {
                Console.Write(">");
            }
            if (step == totalSteps)
            {
                hpt.Stop();
                Console.WriteLine();
                Console.WriteLine("Time consumed: {0} seconds" , hpt.Duration);
            }
        }

        public ProgressBar(long totalSteps)
        {
            hpt = new HighPrecisionTimer();
            Console.SetCursorPosition(64, Console.CursorTop);
            Console.Write("|");
            Console.SetCursorPosition(0, Console.CursorTop);
            this.totalSteps = totalSteps;
            printStepSize = totalSteps >>6; // divided by 64 
            hpt.Start();
        }
    }
}
