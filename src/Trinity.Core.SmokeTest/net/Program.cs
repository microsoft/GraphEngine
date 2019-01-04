using System;
using System.Diagnostics;
using System.Reflection;
using Trinity;

namespace net
{
    class S_impl : SBase
    {
        public override void TestProtocolHandler(RequestTReader request, ResponseTWriter response)
        {
            response.p0 = request.p1;
            response.p1 = request.p0;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            TrinityConfig.LoadConfig("trinity.xml");

            if(args.Length > 0)
            {
                TrinityConfig.CurrentRunningMode = RunningMode.Client;
                using(var req = new RequestTWriter(123, "456"))
                using(var rsp = Global.CloudStorage.TestProtocolToS(0, req))
                {
                    if(rsp.p0 != "456" || rsp.p1 != 123){
                        throw new Exception("Child failure");
                    }
                }
            }
            else
            {
                var server = new S_impl();
                server.Start();

                var proc = new Process();
                var psi = new ProcessStartInfo("dotnet");
                psi.Arguments = $"{Assembly.GetExecutingAssembly().Location} Child";
                psi.CreateNoWindow = true;
                psi.UseShellExecute = false;
                psi.RedirectStandardError = true;
                psi.RedirectStandardInput = true;
                psi.RedirectStandardOutput = true;
                proc.StartInfo = psi;
                proc.Start();

                if(!proc.WaitForExit(10000))
                {
                    proc.Kill();
                    throw new Exception("Child timeout");
                }

                if(proc.ExitCode != 0)
                {
                    throw new Exception("Child failure");
                }

                server.Stop();
            }
        }
    }
}
