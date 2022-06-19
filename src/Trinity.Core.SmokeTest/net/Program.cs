using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using Trinity;
using Trinity.Diagnostics;
using Trinity.Storage;
using net.S;

namespace net
{
    class S_impl : SBase
    {
        public override void TestAsynHandler(RequestTReader request)
        {
            if(request.p0 == 123 && request.p1 == "456")
            {
                Log.WriteLine("AsynHandler: message reaches server correctly.");
            }
            else
            {
                Log.WriteLine("AsynHandler: message corrupted.");
                TestSuccess = false;
            }

            AsynHandlerReachesServer = true;
        }

        public override void TestSynHandler(RequestTReader request)
        {
            if(request.p0 == 123 && request.p1 == "456")
            {
                Log.WriteLine("SynHandler: message reaches server correctly.");
            }
            else
            {
                Log.WriteLine("SynHandler: message corrupted.");
                TestSuccess = false;
            }

            SynHandlerReachesServer = true;
        }

        public override void TestSynRspHandler(RequestTReader request, ResponseTWriter response)
        {
            response.p0 = request.p1;
            response.p1 = request.p0;

            SynWithRspHandlerReachesServer = true;
        }

        public static bool TestSuccess = true;

        public static bool SynHandlerReachesServer = false;
        public static bool AsynHandlerReachesServer = false;
        public static bool SynWithRspHandlerReachesServer = false;

        internal static bool AllHandlersReached()
        {
            Log.WriteLine("SynHandlerReachesServer = {0}", SynHandlerReachesServer);
            Log.WriteLine("AsynHandlerReachesServer = {0}", AsynHandlerReachesServer);
            Log.WriteLine("SynWithRspHandlerReachesServer = {0}", SynWithRspHandlerReachesServer);

            return SynHandlerReachesServer && SynWithRspHandlerReachesServer && AsynHandlerReachesServer;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Global.Initialize("trinity.xml");
            TrinityConfig.StorageRoot  = Environment.CurrentDirectory;

            if(args.Length > 0)
            {
                TrinityConfig.CurrentRunningMode = RunningMode.Client;

                using(var req = new RequestTWriter(123, "456"))
                {
                    Global.CloudStorage[0].TestSyn(req);
                }

                using(var req = new RequestTWriter(123, "456"))
                using(var rsp = Global.CloudStorage[0].TestSynRsp(req))
                {
                    if(rsp.p0 != "456" || rsp.p1 != 123){
                        throw new Exception("Child failure");
                    }
                    Log.WriteLine("Server responded with correct data.");
                }

                using(var req = new RequestTWriter(123, "456"))
                {
                    Global.CloudStorage[0].TestAsyn(req);
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
                psi.RedirectStandardOutput = true;
                proc.StartInfo = psi;

                proc.OutputDataReceived += Show;
                proc.ErrorDataReceived += Show;

                proc.Start();
                proc.BeginOutputReadLine();
                proc.BeginErrorReadLine();

                if(!proc.WaitForExit(1000000))
                {
                    proc.Kill();
                    throw new Exception("Child timeout");
                }

                if(proc.ExitCode != 0)
                {
                    throw new Exception($"Child failure, exit code = {proc.ExitCode}");
                }

                server.Stop();

                if(!S_impl.AllHandlersReached())
                {
                    throw new Exception("Handler not called");
                }

                if (!S_impl.TestSuccess)
                {
                    throw new Exception("Test failure");
                }

                Log.WriteLine("Done!");
            }
        }

        private static void Show(object sender, DataReceivedEventArgs e)
        {
            Log.WriteLine("[Child] {0}", e.Data);
        }
    }
}
