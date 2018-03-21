using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Trinity.Extension;
using Trinity.Network;
using Trinity.Storage;
using Trinity.Utilities;
using Xunit;

namespace Trinity.FFI.UnitTests
{
    public class MockRuntime : ILanguageRuntime
    {
        public static bool s_dispose_called = false;
        public static string s_loadprogram_called = null;

        public void Dispose()
        {
            s_dispose_called = true;
        }

        public int LoadProgram(string path)
        {
            s_loadprogram_called = path;
            return 0;
        }

        public string SynHandler(int methodId, string input)
        {
            return "";
        }

        public void AsynHandler(int methodId, string input)
        {
            return;
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

    public class StartupTask : IStartupTask
    {
        public static bool run = false;
        public void Run()
        {
            run = true;
        }
    }

    public class RuntimeTests
    {
        [Fact]
        public void IStartupTaskExecuted()
        {
            Global.Initialize();
            Assert.True(StartupTask.run);
        }

        [Fact]
        public void ModuleAutoload()
        {
            TrinityServer server = new TrinityServer();
            server.Start();

            Assert.NotNull(server.GetCommunicationModule<TrinityFFIModule>());

            server.Stop();
        }

        [Fact]
        public void LoadsRuntime()
        {
            Global.Initialize();

            FileUtility.CompletePath(FFIConfig.Instance.ProgramDirectory, create_nonexistent: true);
            var fp = Path.Combine(FFIConfig.Instance.ProgramDirectory, "test.txt");
            File.WriteAllText(fp, " ");

            TrinityServer server = new TrinityServer();
            server.Start();

            Assert.Equal(fp, MockRuntime.s_loadprogram_called);
            Assert.Equal(1, MockRuntimeProvider_SS.s_runtime_cnt);

            server.Stop();
            Global.Uninitialize();

            Assert.Equal(true, MockRuntime.s_dispose_called);
        }
    }
}
