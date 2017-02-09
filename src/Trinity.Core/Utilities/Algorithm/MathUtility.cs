// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using Trinity.Core.Lib;
using System.Security;

namespace Trinity.Mathematics
{
    internal unsafe class CMathUtility
    {
        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern double multiply_double_vector(double* dv1, double* dv2, int count) ;

        [SecurityCritical]
        [MethodImpl(MethodImplOptions.InternalCall)]
        internal static extern double multiply_sparse_double_vector(double* dv1, double* dv2, int* idx, int count) ;
    }

    unsafe class MathUtility
    {
        static MathUtility()
        {
            InternalCalls.__init();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double multiply_double_vector(double* dv1, double* dv2, int count) { return CMathUtility.multiply_double_vector(dv1, dv2, count); }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double multiply_sparse_double_vector(double* dv1, double* dv2, int* idx, int count) { return CMathUtility.multiply_sparse_double_vector(dv1, dv2, idx, count); }

        public static int GetMedian(int low, int high)
        {
            return (low + ((high - low) >> 1));
        }

        public static double[] SoftMax(double[] values)
        {
            double[] results = new double[values.Length];
            double sum = 0.0d;
            double max = values[0];
            for (int i = 0; i < values.Length; i++)
            {
                if (values[i] > max) max = values[i];
            }

            for (int i = 0; i < results.Length; i++)
            {
                results[i] = Math.Exp(values[i] - max);
                sum += results[i];
            }

            for (int i = 0; i < values.Length; i++)
            {
                results[i] = results[i] / sum;
            }
            return results;
        }

        public static double Mean(double[] values)
        {
            double sum = 0.0d;
            for (int i = 0; i < values.Length; i++)
                sum += values[i];
            return sum / values.Length;
        }

        public static double LogisticFunction(double value)
        {
            return 1.0d / (Math.Exp(-value) + 1.0d);
        }

        public static double TanhPrime(double value)
        {
            return 1 - Math.Tanh(value) * Math.Tanh(value);
        }

        public static void IncreasingRandomFill(int* array, int min, int max, int count)
        {
            if ((max - min) < count)
                throw new Exception("Data range is too narrow to generate " + count + " random numbers.");

            int[] temp = new int[count];
            for (int i = 0; i < count; i++)
            {
                int value = RandomFactory.Random.Next(min, max);
                while (Contains(temp, value))
                    value = RandomFactory.Random.Next(min, max);
                temp[i] = value;
            }
            Array.Sort(temp);
            fixed (int* ip = temp)
            {
                Memory.Copy(ip, array, count << 2);
            }
        }

        static bool Contains(int[] array, int value)
        {
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == value)
                    return true;
            }
            return false;
        }
    }
}
