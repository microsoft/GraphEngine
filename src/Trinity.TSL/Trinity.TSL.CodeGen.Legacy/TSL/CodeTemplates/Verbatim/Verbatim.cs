using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Utilities;

namespace Trinity.TSL
{
    internal static partial class Verbatim
    {
        internal static string GetHeader(SpecificationScript script)
        {
            CodeWriter cw = new CodeWriter();
            cw +=
@"
#pragma warning disable 162,168,649,660,661,1522
using System;
using System.Text;
using System.Collections.Generic;
using System.Collections;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Data;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.ExceptionServices;
using System.Security;
using System.Runtime.CompilerServices;
using System.Globalization;

using Trinity;
using Trinity.Core.Lib;
using Trinity.Storage;
using Trinity.Utilities;
using Trinity.TSL.Lib;
using Trinity.Network;
using Trinity.Network.Sockets;
using Trinity.Network.Messaging;
using Trinity.TSL;
";
            return cw.ToString();
        }

        internal static string GetFooter()
        {
            return @"
#pragma warning restore 162,168,649,660,661,1522
";
        }
    }
}
