using FanoutSearch.Standard;
using FanoutSearch.Test.TSL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Trinity;
using Xunit;

namespace FanoutSearch.UnitTest
{
    [Collection("All")]
    public class Traversal : IDisposable
    {
        public Traversal()
        {
            Global.LocalStorage.ResetStorage();
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

            var rsp = g.v(0).outE("edges").outV(Action.Continue).outV(Action.Return);
            Assert.Equal(2, rsp.Count());
            Assert.Contains(rsp, p => p[1].id == 11 && p[2].id == 1);
            Assert.Contains(rsp, p => p[1].id == 22 && p[2].id == 2);
        }

        [Fact]
        public void T2()
        {
            Global.LocalStorage.SaveMyCell(0, new List<long> { 11, 22 });
            Global.LocalStorage.SaveMyCell(11, new List<long> { 1 });
            Global.LocalStorage.SaveMyCell(22, new List<long> { 2 });
            Global.LocalStorage.SaveMyCell(1);
            Global.LocalStorage.SaveMyCell(2);

            var rsp = g.v(0).outV(Action.Continue).outV(Action.Return);
            Assert.Equal(2, rsp.Count());
            Assert.Contains(rsp, p => p[1].id == 11 && p[2].id == 1);
            Assert.Contains(rsp, p => p[1].id == 22 && p[2].id == 2);
        }

        [Fact]
        public void T3()
        {
            Global.LocalStorage.SaveMyCell(0, new List<long> { -11, -22 });
            Global.LocalStorage.SaveMyCell(-11, new List<long> { 1 });
            Global.LocalStorage.SaveMyCell(-22, new List<long> { 2 });
            Global.LocalStorage.SaveMyCell(1);
            Global.LocalStorage.SaveMyCell(2);

            var rsp = g.v(0).outE("edges").outV(Action.Continue).outV(Action.Return);
            Assert.Equal(2, rsp.Count());
            Assert.Contains(rsp, p => p[1].id == -11 && p[2].id == 1);
            Assert.Contains(rsp, p => p[1].id == -22 && p[2].id == 2);
        }

        [Fact]
        public void T4()
        {
            Global.LocalStorage.SaveMyCell(0, new List<long> { -11, -22 });
            Global.LocalStorage.SaveMyCell(-11, new List<long> { 1 });
            Global.LocalStorage.SaveMyCell(-22, new List<long> { 2 });
            Global.LocalStorage.SaveMyCell(1);
            Global.LocalStorage.SaveMyCell(2);

            var rsp = g.v(0).outV(Action.Continue).outV(Action.Return);
            Assert.Equal(2, rsp.Count());
            Assert.Contains(rsp, p => p[1].id == -11 && p[2].id == 1);
            Assert.Contains(rsp, p => p[1].id == -22 && p[2].id == 2);
        }
    }
}
