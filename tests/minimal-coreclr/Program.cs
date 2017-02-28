using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Trinity;
using Trinity.Network;

namespace minimal
{
    class Program
    {
        static void Main(string[] args)
        {
            TrinityConfig.LoggingLevel = Trinity.Diagnostics.LogLevel.Debug;
            Global.LocalStorage.ResetStorage();
            TrinityServer server = new TrinityServer();
            server.Start();
            server.Stop();
            Parallel.For(0, 1000, i =>
            {
                Global.LocalStorage.SaveCell(i, new byte[i]);
            });
            Global.LocalStorage.SaveStorage();
            Global.LocalStorage.LoadStorage();

            for(int i = 0; i<1000; ++i)
            {
                byte[] cell_content;
                Debug.Assert(TrinityErrorCode.E_SUCCESS == Global.LocalStorage.LoadCell(i, out cell_content));
                Debug.Assert(i == cell_content.Length);
            }

            Global.Uninitialize();
            Environment.Exit(0);
        }
    }
}
