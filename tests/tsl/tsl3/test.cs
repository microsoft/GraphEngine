using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Cache;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity;
using Trinity.Core.Lib;
using Trinity.Storage;
using Xunit;
using Xunit.Sdk;

namespace tsl3
{
    #region Fixture
    public class TestServer : TestServerBase
    {
        public override void TestSynWithRspHandler(ReqReader request, RespWriter response)
        {
            response.result = $"{request.FieldBeforeList} {string.Join(" ", request.Nums)} {request.FieldAfterList}";
        }

        public override void TestSynHandler(ReqReader request)
        {
            if (request.FieldBeforeList == (int) request.FieldAfterList)
                throw new ArgumentException();
            if (request.Nums.Count == 10)
                throw new InvalidDataException();
            throw new NotImplementedException();
        }

        public override void TestAsynHandler(ReqReader request)
        {
            TestSynHandler(request);
        }
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

    [CollectionDefinition("TestServer Collection")]
    public class TestServerCollection : ICollectionFixture<TrinityServerFixture>
    {
    }
    #endregion

    public class StructTest
    {
        [Fact]
        public void ReqStruct_TypeIsValueType()
        {
            Assert.True(typeof(Req).IsValueType);
        }

        [Fact]
        public void ReqStruct_ShouldHaveDefaultCtor()
        {
            var req = new Req();
        }

        [Fact]
        public void ReqStruct_ShouldHaveDefinedFields()
        {
            var req = new Req();
            req.FieldBeforeList = 1984;
            req.FieldAfterList = 42;
            Assert.Null(req.Nums);
            req.Nums = Enumerable.Concat(Enumerable.Repeat(2, 1), Enumerable.Repeat(3, 10)).ToList();
        }

        [Fact]
        public void ReqStruct_FieldTypeShouldMatch()
        {
            var type = typeof(Req);
            var fields = type.GetMembers().Where(m => m.MemberType == MemberTypes.Field).Cast<FieldInfo>().ToList();
            Assert.True(fields.First(m => m.Name == "FieldBeforeList").FieldType == typeof(int), "FieldBeforeList");
            Assert.True(fields.First(m => m.Name == "Nums").FieldType == typeof(List<int>), "Nums");
            Assert.True(fields.First(m => m.Name == "FieldAfterList").FieldType == typeof(byte), "FieldAfterList");
        }
    }

}
