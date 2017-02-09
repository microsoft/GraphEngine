// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "BackgroundThread/BackgroundThread.h"
#include <Trinity/Diagnostics/Log.h>

namespace Trinity
{
    namespace Diagnostics
    {
        class LogAutoFlushTask : public BackgroundThread::BackgroundTask
        {
        public:
            DWORD Execute() override
            {
                Trinity::Diagnostics::Flush();
                return 10000; //XXX hard coded time
            }
        };
    }
}