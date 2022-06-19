// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.Utilities
{
    class HelpWriter
    {
        public static void WriteLine(int indent, int max_option_length, string option, string description)
        {
            for (int i = 0; i < indent; i++)
                Console.Write(' ');

            Console.Write(option);

            int option_padding_len = max_option_length - option.Length;

            for (int i = 0; i < option_padding_len; i++)
                Console.Write(' ');

            Console.Write("    ");

            int width = Console.WindowWidth - indent - max_option_length - 4 - 1/*margin*/;

            int remaining_text = description.Length;

            int desc_index = 0;

            while (remaining_text > 0)
            {
                int lenght_to_write = remaining_text > width ? width : remaining_text;
                string desc_str = description.Substring(desc_index, lenght_to_write);
                Console.WriteLine(desc_str);

                int header_length = indent + max_option_length+4;

                for (int i = 0; i < header_length; i++)
                    Console.Write(' ');

                desc_index += lenght_to_write;
                remaining_text -= lenght_to_write;
            }

            Console.WriteLine();
        }
    }
}
