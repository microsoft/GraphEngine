using FanoutSearch.Standard;
using FanoutSearch.Test.TSL;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity;
using Xunit;

namespace FanoutSearch.UnitTest
{
    [Collection("All")]
    public class LambdaTraversal : IDisposable
    {
        private FanoutSearchModule mod;

        public LambdaTraversal()
        {
            Global.LocalStorage.ResetStorage();
            mod = Global.CommunicationInstance.GetCommunicationModule<FanoutSearchModule>();
        }

        public void Dispose() { }

        [Fact]
        public void T1()
        {
            Global.LocalStorage.SaveMyCell(0, new List<long> { 11, 22 });
            Global.LocalStorage.SaveMyCell(11, new List<long> { 1 });
            Global.LocalStorage.SaveMyCell(22, new List<long> { 2 });
            Global.LocalStorage.SaveMyCell(1);
            Global.LocalStorage.SaveMyCell(2);

            var rsp = JArray.Parse(mod.LambdaQuery($@"MAG.StartFrom(0).FollowEdge(""edges"").VisitNode(Action.Continue).VisitNode(Action.Return);"));
            Assert.Contains(rsp, p => p[1].Value<long>("CellId") == 11 && p[2].Value<long>("CellId") == 1);
            Assert.Contains(rsp, p => p[1].Value<long>("CellId") == 22 && p[2].Value<long>("CellId") == 2);
        }

        [Fact]
        public void T2()
        {
            Global.LocalStorage.SaveMyCell(0, new List<long> { 11, 22 });
            Global.LocalStorage.SaveMyCell(11, new List<long> { 1 });
            Global.LocalStorage.SaveMyCell(22, new List<long> { 2 });
            Global.LocalStorage.SaveMyCell(1);
            Global.LocalStorage.SaveMyCell(2);

            var rsp = JArray.Parse(mod.LambdaQuery($@"MAG.StartFrom(0).VisitNode(Action.Continue).VisitNode(Action.Return);"));
            Assert.Contains(rsp, p => p[1].Value<long>("CellId") == 11 && p[2].Value<long>("CellId") == 1);
            Assert.Contains(rsp, p => p[1].Value<long>("CellId") == 22 && p[2].Value<long>("CellId") == 2);
        }

        [Fact]
        public void T3()
        {
            Global.LocalStorage.SaveMyCell(0, new List<long> { -11, -22 });
            Global.LocalStorage.SaveMyCell(-11, new List<long> { 1 });
            Global.LocalStorage.SaveMyCell(-22, new List<long> { 2 });
            Global.LocalStorage.SaveMyCell(1);
            Global.LocalStorage.SaveMyCell(2);

            var rsp = JArray.Parse(mod.LambdaQuery($@"MAG.StartFrom(0).FollowEdge(""edges"").VisitNode(Action.Continue).VisitNode(Action.Return);"));
            Assert.Contains(rsp, p => p[1].Value<long>("CellId") == -11 && p[2].Value<long>("CellId") == 1);
            Assert.Contains(rsp, p => p[1].Value<long>("CellId") == -22 && p[2].Value<long>("CellId") == 2);
        }

        [Fact]
        public void T4()
        {
            Global.LocalStorage.SaveMyCell(0, new List<long> { -11, -22 });
            Global.LocalStorage.SaveMyCell(-11, new List<long> { 1 });
            Global.LocalStorage.SaveMyCell(-22, new List<long> { 2 });
            Global.LocalStorage.SaveMyCell(1);
            Global.LocalStorage.SaveMyCell(2);

            var rsp = JArray.Parse(mod.LambdaQuery($@"MAG.StartFrom(0).VisitNode(Action.Continue).VisitNode(Action.Return);"));
            Assert.Contains(rsp, p => p[1].Value<long>("CellId") == -11 && p[2].Value<long>("CellId") == 1);
            Assert.Contains(rsp, p => p[1].Value<long>("CellId") == -22 && p[2].Value<long>("CellId") == 2);
        }
    }
}
