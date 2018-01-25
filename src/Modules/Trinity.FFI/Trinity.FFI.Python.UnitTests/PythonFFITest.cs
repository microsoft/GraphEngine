using Python.Runtime;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Trinity.Network;
using Trinity.Utilities;
using Xunit;

namespace Trinity.FFI.Python.UnitTests
{
    public class PythonFFITest
    {
        [Fact]
        public void LoadPythonFFI_Success()
        {
            // The test engine may trigger the initialization
            Global.Initialize();
            FileUtility.CompletePath(FFIConfig.Instance.ProgramDirectory, create_nonexistent: true);

            // try to invoke trinity-specific python code here
            var fp = Path.Combine(FFIConfig.Instance.ProgramDirectory, "test.py");
            File.WriteAllText(fp, @"
with open('MyCell.txt', 'w') as f:
    a = ge.NewCell('MyCell')
    print(a)
    f.write(a)
");
            TrinityServer server = new TrinityServer();
            server.Start();

            Global.Uninitialize();

            Assert.True(File.Exists("MyCell.txt"));
            Debug.WriteLine(File.ReadAllText("MyCell.txt"));
        }
    }
}
