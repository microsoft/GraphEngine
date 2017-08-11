using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Trinity.Network;
using Trinity.Storage;
using Xunit;

namespace Trinity.FFI.UnitTests
{
    public class MockRuntime : ILanguageRuntime
    {
        public static bool s_dispose_called = false;
        public static string s_loadprogram_called = null;
        public static bool s_registeroperations_called = false;

        public void Dispose()
        {
            s_dispose_called = true;
        }

        public int LoadProgram(string path)
        {
            s_loadprogram_called = path;
            return 0;
        }

        public void RegisterOperations(IGenericCellOperations storageOperations, IGenericMessagePassingOperations messagePassingOperations)
        {
            s_registeroperations_called = true;
        }

        public string Run(int methodId, string input)
        {
            return "";
        }

        public int RunAsync(int methodId, string input)
        {
            return 0;
        }

        public int Wait(int handle, int timeout, out string output)
        {
            output = null;
            return 0;
        }
    }

    public class MockRuntimeProvider_SS : ILanguageRuntimeProvider
    {
        public static int s_runtime_cnt = 0;

        public string Name => "MockL";

        public ThreadingModel ThreadingModel => ThreadingModel.SingleThreaded;

        public RuntimeModel RuntimeModel => RuntimeModel.SingleRuntime;

        public string[] SupportedSuffix => new[] { ".txt" };

        public ILanguageRuntime NewRuntime()
        {
            ++s_runtime_cnt;
            return new MockRuntime();
        }
    }

    public class RuntimeTests
    {
        [Fact]
        public void ModuleAutoload()
        {
            TrinityServer server = new TrinityServer();
            server.Start();

            Assert.NotNull(server.GetCommunicationModule<FFIModule>());

            server.Stop();
        }

        [Fact]
        public void LoadsRuntime()
        {
            Global.Initialize();
            TrinityServer server = new TrinityServer();
            server.Start();

            var fp = Path.Combine(FFIConfig.Instance.ProgramDirectory, "test.txt");

            File.WriteAllText(fp, " ");

            Assert.Equal(fp, MockRuntime.s_loadprogram_called);
            Assert.Equal(1, MockRuntimeProvider_SS.s_runtime_cnt);

            server.Stop();
        }
    }
}
