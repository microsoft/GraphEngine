using System;
using System.IO;
using System.Net.Cache;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Storage;
using Xunit;
using Xunit.Sdk;

namespace tsl3
{
    public class TestServer : TestServerBase
    {
        [Result] public int SynWithRspResult { get; private set; } = 0;
        public override void TestSynWithRspHandler(ReqReader request, RespWriter response)
        {
            string resp;
            SynWithRspResult = Utils.CalcForSynRsp(request.FieldBeforeList, request.Nums, request.FieldAfterList, out resp);
            response.result = resp;
        }

        [Result] public int SynResult { get; private set; } = 0;
        public override void TestSynHandler(ReqReader request)
        {
            SynResult = Utils.CalcForSyn(request.FieldBeforeList, request.Nums, request.FieldAfterList);
        }

        [Result] public int AsynResult { get; private set; } = 0;
        public override void TestAsynHandler(ReqReader request)
        {
            AsynResult = Utils.CalcForAsyn(request.FieldBeforeList, request.Nums, request.FieldAfterList);
        }

        [Result] public int SynWithRsp1Result { get; private set; } = 0;
        public override void TestSynWithRsp1Handler(ReqReader request, RespWriter response)
        {
            string resp;
            SynWithRsp1Result = Utils.CalcForSynRsp(request.FieldBeforeList, request.Nums, request.FieldAfterList, out resp);
            response.result = resp;
        }

        [Result] public int Syn1Result { get; private set; } = 0;
        public override void TestSyn1Handler(ReqReader request)
        {
            Syn1Result = Utils.CalcForSyn(request.FieldBeforeList, request.Nums, request.FieldAfterList);
        }

        [Result] public int Asyn1Result { get; private set; } = 0;
        public override void TestAsyn1Handler(ReqReader request)
        {
            Asyn1Result = Utils.CalcForAsyn(request.FieldBeforeList, request.Nums, request.FieldAfterList);
        }

        public void ResetCounts()
        {
            SynResult = AsynResult = SynWithRspResult = 0;
            Syn1Result = Asyn1Result = SynWithRsp1Result = 0;
        }
    }

    [CollectionDefinition("TestServer Collection")]
    public class TestServerCollection : ICollectionFixture<TrinityServerFixture>
    {
    }

    public class TrinityServerFixture : IDisposable
    {
        public TrinityServerFixture()
        {
            Server = new TestServer();
            Server.Start();
        }

        public TestServer Server { get; private set; }

        public void Dispose()
        {
            Server.Stop();
        }
    }
}
