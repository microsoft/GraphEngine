// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <cstdint>
#include <cmath>
#include "Array.h"
#include <os/os.h>
namespace Trinity
{
    class Random
    {
    public:
        Random() : Random(TickCount()) {}
        Random(int32_t Seed)
        {
            int32_t num = (Seed == INT_MIN) ? INT_MAX : abs(Seed);
            int32_t num2 = 161803398 - num;
            SeedArray[55] = num2;
            int32_t num3 = 1;
            for (int32_t i = 1; i < 55; i++)
            {
                int32_t num4 = 21 * i % 55;
                SeedArray[num4] = num3;
                num3 = num2 - num3;
                if (num3 < 0)
                {
                    num3 += 2147483647;
                }
                num2 = SeedArray[num4];
            }
            for (int32_t j = 1; j < 5; j++)
            {
                for (int32_t k = 1; k < 56; k++)
                {
                    SeedArray[k] -= SeedArray[1 + (k + 30) % 55];
                    if (SeedArray[k] < 0)
                    {
                        SeedArray[k] += 2147483647;
                    }
                }
            }
            inext = 0;
            inextp = 21;
        }
        int32_t Next()
        {
            return InternalSample();
        }
        int32_t Next(int32_t minValue, int32_t maxValue)
        {
            if (minValue > maxValue)
            {
                maxValue = minValue; //TODO exception
            }
            int64_t num = (int64_t)maxValue - (int64_t)minValue;
            if (num <= 2147483647L)
            {
                return (int32_t) (Sample() * (double) num) + minValue;
            }
            return (int32_t)((int64_t)(GetSampleForLargeRange() * (double)num) + (int64_t)minValue);
        }
        int32_t Next(int32_t maxValue)
        {
            if (maxValue < 0)
            {
                maxValue = 0;//TODO exception
            }
            return (int32_t)(Sample() * (double)maxValue);
        }

        double NextDouble()
        {
            return Sample();
        }
        void NextBytes(Array<char> buffer)
        {
            if (buffer == NULL)
            {
                //TODO exception
            }
            for (size_t i = 0; i < buffer.Length(); i++)
            {
                buffer[i] = (char) (InternalSample() % 256);
            }
        }
    private:
        static inline int32_t TickCount()
        {
#pragma warning(suppress: 28159)
#if defined(TRINITY_PLATFORM_WINDOWS)
            return GetTickCount();
#else
            return time(NULL);
#endif
        }
        double GetSampleForLargeRange()
		{
			int32_t num = InternalSample();
			bool flag = InternalSample() % 2 == 0;
			if (flag)
			{
				num = -num;
			}
			double num2 = (double)num;
			num2 += 2147483646.0;
			return num2 / 4294967293.0;
		}
        double Sample()
		{
			return (double)InternalSample() * 4.6566128752457969E-10;
		}
        int32_t InternalSample()
        {
            int32_t num = inext;
            int32_t num2 = inextp;
            if (++num >= 56)
            {
                num = 1;
            }
            if (++num2 >= 56)
            {
                num2 = 1;
            }
            int32_t num3 = SeedArray[num] - SeedArray[num2];
            if (num3 == 2147483647)
            {
                num3--;
            }
            if (num3 < 0)
            {
                num3 += 2147483647;
            }
            SeedArray[num] = num3;
            inext = num;
            inextp = num2;
            return num3;
        }

        int32_t inext;
        int32_t inextp;
        Array<int32_t> SeedArray = Array<int32_t>(56);
        const int32_t MBIG = 2147483647;
        const int32_t MSEED = 161803398;
        const int32_t MZ = 0;
    };
}
