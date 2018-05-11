using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Trinity;
using Trinity.Storage;
using Trinity.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace EchoOnConsole
{
    public class EchoOnConsole
    {
        [DllImport("Kernel32.dll", SetLastError = true) ]
        private static extern IntPtr GetStdHandle(int device);
        [DllImport("Kernel32.dll", SetLastError = true) ]
        private static extern bool   SetStdHandle(int device, IntPtr handle);
        private const int            STD_INPUT_HANDLE = -10;
        private const int            STD_OUTPUT_HANDLE = -11;
        private const int            STD_ERROR_HANDLE = -12;

        [Fact]
        public void T1()
        {
            var stdout = GetStdHandle(STD_OUTPUT_HANDLE);
            try
            {
                var filestream = new FileStream("tmp.txt", FileMode.OpenOrCreate);
                SetStdHandle(STD_OUTPUT_HANDLE, filestream.Handle);
                TrinityConfig.LogEchoOnConsole = true;
                Global.Initialize();
                filestream.Close();
                Assert.NotEqual(0, File.ReadAllText("tmp.txt").Length);
            }
            finally
            {
                SetStdHandle(STD_OUTPUT_HANDLE, stdout);
            }
        }
    }
}
