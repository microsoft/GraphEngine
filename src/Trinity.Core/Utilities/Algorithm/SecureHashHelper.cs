// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Globalization;

namespace Trinity.Utilities
{
    internal class SecureHashHelper
    {
        public static string GetFileSHA512(string file_name)
        {
            SHA512 mdhasher = SHA512.Create();
            StringBuilder sb = new StringBuilder();
            sb.Clear();
            if (File.Exists(file_name))
            {
                using (FileStream fs = File.OpenRead(file_name))
                {
                    foreach (byte b in mdhasher.ComputeHash(fs))
                    {
                        sb.Append(b.ToString("x2", CultureInfo.InvariantCulture));
                    }
                }
            }
            return sb.ToString();
        }
    }
}
