using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity.Storage.Composite;
using Xunit;

namespace Trinity.FFI.UnitTests
{
    public class JitInterop
    {
        [Fact]
        public void LoadsJitAssembly()
        {
            CompositeStorage.AddStorageExtension(".\\tsl3", "TSLTest3");
            foreach(var tdesc in Global.StorageSchema.CellDescriptors.Select(GraphEngine.Jit.TypeSystem.Make))
            {
                Console.WriteLine(tdesc);
            }
        }
    }
}
