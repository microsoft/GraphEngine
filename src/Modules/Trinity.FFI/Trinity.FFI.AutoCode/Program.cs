using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trinity.Utilities;

namespace Trinity.FFI.AutoCode
{
    class Program
    {
        static void Main(string[] args)
        {
            File.WriteAllText("../Trinity.FFI/FFIMethods.cs", new FFIMethods().TransformText());
            File.WriteAllText("../Trinity.FFI/Native.cs", new Native().TransformText());
            File.WriteAllText("../Trinity.FFI.Native/Trinity.FFI.Native.h", new Cpp().TransformText());
        }
    }
}
