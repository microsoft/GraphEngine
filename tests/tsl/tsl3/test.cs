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

    public static unsafe class Utils
    {
        public static byte* MakeCopyOfDataInReqWriter(ReqWriter writer)
        {
            byte* newbuf = (byte*)Memory.malloc((ulong)writer.Length);
            Memory.memcpy(newbuf, writer.CellPtr, (ulong)writer.Length);
            return newbuf;
        }
    }

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

    [Collection("TestServer Collection")]
    public unsafe class ProtocolTest
    {
        private TrinityServerFixture Fixture { get; }
        public ProtocolTest(TrinityServerFixture fixture) { Fixture = fixture; }

        [Theory]
        [InlineData(new byte[] {1, 2, 3, 4})]
        [InlineData(new byte[] {2, 0, 4, 8})]
        [InlineData(new byte[] {123, 124, 75, 43})]
        [InlineData(new byte[] {77, 88, 9, 8})]
        [InlineData(new byte[] {77, 88, 9, 8, 77, 88, 9, 8, 77, 88, 9, 8, 77, 88, 9, 8, })]
        public void SynWithRsp_Test(byte[] nums)
        {
            Assert.True(nums.Length >= 4);
            var numList = nums.ToList();
            using (var writer = new ReqWriter())
            {
                writer.FieldBeforeList = numList[0];
                writer.Nums.Add(numList[1]);
                Console.WriteLine(string.Join(" ", numList.Skip(2).Select(Convert.ToInt32).ToList()));
                writer.Nums.AddRange(numList.Skip(2).Select(Convert.ToInt32).ToList());
                writer.Nums.RemoveAt(writer.Nums.Count - 1);
                writer.FieldAfterList = numList.Last();
                using (var response = Global.CloudStorage.TestSynWithRspToTestServer(Global.MyServerID, writer))
                {
                    Assert.Equal(string.Join(" ", numList), response.result);
                }
            }
        }

        [Fact]
        public void Syn_Test()
        {
            using (var writer = new ReqWriter())
            {
                writer.FieldBeforeList = 2;
                writer.Nums.Add(0);
                writer.Nums.Add(7);
                writer.FieldAfterList = 3;
                Assert.Throws<IOException>(() => Global.CloudStorage.TestSynToTestServer(Global.MyServerID, writer));

                using (var reader = new ReqReader(Utils.MakeCopyOfDataInReqWriter(writer), 0))
                    Assert.Throws<NotImplementedException>(() => Fixture.Server.TestSynHandler(reader));

                writer.FieldBeforeList = writer.FieldAfterList = 42;
                using (var reader = new ReqReader(Utils.MakeCopyOfDataInReqWriter(writer), 0))
                    Assert.Throws<ArgumentException>(() => Fixture.Server.TestSynHandler(reader));

                writer.FieldBeforeList = 41;
                writer.FieldAfterList = 42;
                writer.Nums.Clear();
                writer.Nums.AddRange(Enumerable.Repeat(1, 10).ToList());
                using (var reader = new ReqReader(Utils.MakeCopyOfDataInReqWriter(writer), 0))
                    Assert.Throws<InvalidDataException>(() => Fixture.Server.TestSynHandler(reader));
            }
        }


        [Fact]
        public void Asyn_Test()
        {
            using (var writer = new ReqWriter())
            {
                writer.FieldBeforeList = 2;
                writer.Nums.Add(0);
                writer.Nums.Add(7);
                writer.FieldAfterList = 3;
                // assert: here won't throw
                Global.CloudStorage.TestAsynToTestServer(Global.MyServerID, writer);
                using (var reader = new ReqReader(Utils.MakeCopyOfDataInReqWriter(writer), 0))
                    Assert.Throws<NotImplementedException>(() => Fixture.Server.TestAsynHandler(reader));

                writer.FieldBeforeList = writer.FieldAfterList = 42;
                using (var reader = new ReqReader(Utils.MakeCopyOfDataInReqWriter(writer), 0))
                    Assert.Throws<ArgumentException>(() => Fixture.Server.TestAsynHandler(reader));

                writer.FieldBeforeList = 41;
                writer.FieldAfterList = 42;
                writer.Nums.Clear();
                writer.Nums.AddRange(Enumerable.Repeat(1, 10).ToList());
                using (var reader = new ReqReader(Utils.MakeCopyOfDataInReqWriter(writer), 0))
                    Assert.Throws<InvalidDataException>(() => Fixture.Server.TestAsynHandler(reader));
            }
        }
    }
}
