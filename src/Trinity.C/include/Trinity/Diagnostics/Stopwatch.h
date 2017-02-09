// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <cstdint>
#include <chrono>
#include <ratio>
namespace Trinity
{
    namespace Diagnostics
    {
        class Stopwatch
        {
            std::chrono::high_resolution_clock::time_point StartingTime;
            std::chrono::high_resolution_clock::time_point EndingTime;
        public:

            static Stopwatch StartNew()
            {
                Stopwatch sw;
                sw.Start();
                return sw;
            }

            static int64_t Frequency()
            {
                return 
                    std::chrono::high_resolution_clock::period::den /
                    std::chrono::high_resolution_clock::period::num;
            }

            void Start()
            {
                StartingTime = std::chrono::high_resolution_clock::now();
            }

            void Stop()
            {
                EndingTime = std::chrono::high_resolution_clock::now();
            }

            void Restart()
            {
                StartingTime = std::chrono::high_resolution_clock::now();
            }

            int64_t GetElapsedTicks()
            {
                auto time_span = EndingTime - StartingTime;
                return time_span.count();
            }

            int64_t GetElapsedMicroseconds()
            {
                auto time_span = EndingTime - StartingTime;
                return std::chrono::duration_cast<std::chrono::microseconds>(time_span).count();
            }

            int64_t GetElapsedMilliseconds()
            {
                auto time_span = EndingTime - StartingTime;
                return std::chrono::duration_cast<std::chrono::milliseconds>(time_span).count();
            }

            int64_t GetSeconds()
            {
                auto time_span = EndingTime - StartingTime;
                return std::chrono::duration_cast<std::chrono::seconds>(time_span).count();
            }
        };


    }
}