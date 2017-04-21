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
    #region Fixture and Utils
    public static unsafe class Utils
    {
        public static byte* MakeCopyOfDataInReqWriter(ReqWriter writer)
        {
            byte* newbuf = (byte*)Memory.malloc((ulong)writer.Length);
            Memory.memcpy(newbuf, writer.CellPtr, (ulong)writer.Length);
            return newbuf;
        }

        public static int CalcForSynRsp(int before, IEnumerable<int> nums, byte after, out string response)
        {
            var numList = nums.ToList();
            response = $"{before} {string.Join(" ", numList)} {after}";
            return before + numList.Sum() + after;
        }

        public static int CalcForSyn(int before, IEnumerable<int> nums, byte after)
        {
            List<int> numList = nums.ToList();
            if (before == after)
                return before + after;
            if (numList.Count == 10)
                return numList.Sum();
            return -1;
        }

        public static int CalcForAsyn(int before, IEnumerable<int> nums, byte after)
        {
            List<int> numList = nums.ToList();
            return numList.Sum() - before + after;
        }
    }

    public class TestServer : TestServerBase
    {
        public int SynWithRspResult { get; private set; } = 0;
        public override void TestSynWithRspHandler(ReqReader request, RespWriter response)
        {
            string resp;
            SynWithRspResult = Utils.CalcForSynRsp(request.FieldBeforeList, request.Nums, request.FieldAfterList, out resp);
            response.result = resp;
        }

        public int SynResult { get; private set; } = 0;
        public override void TestSynHandler(ReqReader request)
        {
            SynResult = Utils.CalcForSyn(request.FieldBeforeList, request.Nums, request.FieldAfterList);
        }

        public int AsynResult { get; private set; } = 0;
        public override void TestAsynHandler(ReqReader request)
        {
            AsynResult = Utils.CalcForAsyn(request.FieldBeforeList, request.Nums, request.FieldAfterList);
        }

        public int SynWithRsp1Result { get; private set; } = 0;
        public override void TestSynWithRsp1Handler(ReqReader request, RespWriter response)
        {
            string resp;
            SynWithRsp1Result = Utils.CalcForSynRsp(request.FieldBeforeList, request.Nums, request.FieldAfterList, out resp);
            response.result = resp;
        }

        public int Syn1Result { get; private set; } = 0;
        public override void TestSyn1Handler(ReqReader request)
        {
            Syn1Result = Utils.CalcForSyn(request.FieldBeforeList, request.Nums, request.FieldAfterList);
        }

        public int Asyn1Result { get; private set; } = 0;
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

    public unsafe class MessageAccessorTest
    {
        [Fact]
        public void Writer_ShouldBasicallyWork()
        {
            using (var writer = new ReqWriter())
            {
                writer.FieldBeforeList = -42;
                writer.Nums.Add(100);
                writer.FieldAfterList = 42;
                Assert.Equal(writer.FieldBeforeList, -42);
                Assert.Equal(writer.Nums.Count, 1);
                Assert.Equal(writer.Nums[0], 100);
                Assert.Equal(writer.FieldAfterList, 42);
            }
        }

        [Fact]
        public void Writer_ShouldHaveProperlyTypedProperties()
        {
            {
                var type = typeof(ReqWriter);
                var properties = type.GetMembers().Where(m => m.MemberType == MemberTypes.Property).Cast<PropertyInfo>().ToList();
                Assert.True(properties.Single(m => m.Name == "FieldBeforeList").PropertyType == typeof(int), "FieldBeforeList");
                Assert.True(properties.Single(m => m.Name == "Nums").PropertyType == typeof(intListAccessor), "Nums");
                Assert.True(properties.Single(m => m.Name == "FieldAfterList").PropertyType == typeof(byte), "FieldAfterList");
            }
            {
                var type = typeof(RespWriter);
                var property = (PropertyInfo) type.GetMember("result").Single();
                Assert.True(property.PropertyType == typeof(StringAccessor));
            }
        }

        [Fact]
        public void Reader_ShouldBasicallyWork()
        {
            using (var writer = new ReqWriter())
            {
                writer.FieldBeforeList = -42;
                writer.Nums.Add(100);
                writer.FieldAfterList = 42;
                using (var reader = new ReqReader(Utils.MakeCopyOfDataInReqWriter(writer), 0))
                {
                    Assert.Equal(reader.FieldBeforeList, -42);
                    Assert.Equal(reader.Nums.Count, 1);
                    Assert.Equal(reader.Nums[0], 100);
                    Assert.Equal(reader.FieldAfterList, 42);
                }
            }
        }

        [Fact]
        public void Reader_ShouldHaveProperlyTypedProperties()
        {
            {
                var type = typeof(ReqReader);
                var properties = type.GetMembers().Where(m => m.MemberType == MemberTypes.Property).Cast<PropertyInfo>().ToList();
                Assert.True(properties.Single(m => m.Name == "FieldBeforeList").PropertyType == typeof(int), "FieldBeforeList");
                Assert.True(properties.Single(m => m.Name == "Nums").PropertyType == typeof(intListAccessor), "Nums");
                Assert.True(properties.Single(m => m.Name == "FieldAfterList").PropertyType == typeof(byte), "FieldAfterList");
            }
            {
                var type = typeof(RespReader);
                var property = (PropertyInfo) type.GetMember("result").Single();
                Assert.True(property.PropertyType == typeof(StringAccessor));
            }
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
