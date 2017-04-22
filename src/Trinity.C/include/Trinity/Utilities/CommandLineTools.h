// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
#pragma once
#include "OptionDescriptor.h"
namespace Trinity
{
    namespace Utilities
    {
        namespace CommandLineTools
        {
            template<typename OptionValueType>
            inline OptionDescriptor<OptionValueType>
                DefineOption(String shortOption, String longOption = "")
            {
                if (longOption == "")
                    longOption = shortOption;
                return OptionDescriptor < OptionValueType >(shortOption, longOption);
            }
            template<typename T>inline size_t Index(List<String>& command, OptionDescriptor<T> descriptor)
            {
                for (size_t i = 0, length = command.size(); i < length; ++i)
                {
                    if (command[i].StartsWith("--"))
                    {
                        if (command[i].Substring(2) == descriptor.LongOption)
                            return i;
                    }
                    else if (command[i].StartsWith("-"))
                    {
                        if (command[i].Substring(1) == descriptor.ShortOption)
                            return i;
                    }
                }
                return String::npos;
            }
            inline void GetOpt(List<String>& command, OptionDescriptor<bool>& descriptor)
            {
                size_t idx = Index(command, descriptor);
                descriptor.value = false;
                if (idx != String::npos)
                {
                    descriptor.value = true;
                    descriptor.set = true;
                    command.erase(command.begin() + idx);
                }
            }
            template<typename T>
            inline void GetOpt(List<String>& command, OptionDescriptor<T>& descriptor)
            {
                size_t idx = Index(command, descriptor);
                if (idx != String::npos && idx < command.size() - 1)
                {
                    descriptor.set = command[idx + 1].TryParse(descriptor.value);
                    if (descriptor.set)
                    {
                        command.erase(command.begin() + idx);
                        command.erase(command.begin() + idx);
                    }
                }
            }
            template<typename FirstArg, typename ...Args>
            inline void GetOpt(List<String>& command, FirstArg& firstArg, Args& ...restArgs)
            {
                GetOpt(command, firstArg);
                GetOpt(command, restArgs...);
            }


            inline List<String> GetArguments(int argc, u16char** argv)
            {
                List<String> ret(argc - 1);
                for (int i = 1; i < argc; ++i)
                {
                    ret[i - 1] = String::FromWcharArray(argv[i], -1);
                }
                return ret;
            }

            inline List<String> GetArguments(int argc, char** argv)
            {
                List<String> ret(argc - 1);
                for (int i = 1; i < argc; ++i)
                    ret[i - 1] = argv[i];
                return ret;
            }
        }
    }
}