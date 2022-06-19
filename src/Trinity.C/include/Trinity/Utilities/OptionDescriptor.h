// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include <Trinity/String.h>
namespace Trinity
{
    namespace Utilities
    {
        template <typename OptionValueType> class OptionDescriptor
        {
        public:
            OptionDescriptor(
                const Trinity::String& shortOpt,
                const Trinity::String& longOpt)
            {
                LongOption = longOpt;
                ShortOption = shortOpt;
            }
            Trinity::String LongOption;
            Trinity::String ShortOption;
            OptionValueType value;
            bool set = false;
        };
    }
}
