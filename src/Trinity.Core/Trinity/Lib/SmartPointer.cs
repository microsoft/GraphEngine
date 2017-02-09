// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Trinity
{
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Explicit)]
    internal unsafe struct SmartPointer
    {
        static public unsafe SmartPointer New(byte* ptr)
        {
            SmartPointer ret = new SmartPointer();
            ret.bp = ptr;
            return ret;
        }
        static public unsafe SmartPointer New(short* ptr)
        {
            SmartPointer ret = new SmartPointer();
            ret.sp = ptr;
            return ret;
        }
        static public unsafe SmartPointer New(int* ptr)
        {
            SmartPointer ret = new SmartPointer();
            ret.ip = ptr;
            return ret;
        }
        static public unsafe SmartPointer New(long* ptr)
        {
            SmartPointer ret = new SmartPointer();
            ret.lp = ptr;
            return ret;
        }
        [System.Runtime.InteropServices.FieldOffset(0)]
        public byte* bp;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public short* sp;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public int* ip;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public long* lp;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public double* dp;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public float* fp;
        [System.Runtime.InteropServices.FieldOffset(0)]
        public char* cp;
    }

}
