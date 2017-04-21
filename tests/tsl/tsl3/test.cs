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
    public class ResultAttribute : Attribute { }

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

        public static void EnsureResults<T, R>(T obj, string nonZeroField, R expectedValue)
        {
            var type = typeof(T);
            var nonZeroProp = (PropertyInfo)type.GetMembers().Single(p => p.MemberType == MemberTypes.Property && p.Name == nonZeroField);
            Assert.Equal((R)nonZeroProp.GetValue(obj), expectedValue);
            var zeroProps = type.GetMembers()
                .Where(i => i.MemberType == MemberTypes.Property)
                .Where(i => i.CustomAttributes.Any(_ => _.AttributeType == typeof(ResultAttribute)))
                .Where(i => i.Name != nonZeroField)
                .Cast<PropertyInfo>();
            Assert.True(zeroProps.All(p => ((R)p.GetValue(obj)).Equals(default(R))));
        }
    }

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
        [Theory]
        [InlineData(new byte[] {1, 2, 3, 4})]
        [InlineData(new byte[] {2, 0, 4, 8})]
        [InlineData(new byte[] {123, 124, 75, 43})]
        [InlineData(new byte[] {77, 88, 9, 8})]
        [InlineData(new byte[] {77, 88, 9, 8, 77, 88, 9, 8, 77, 88, 9, 8, 77, 88, 9, 8, })]
        public void Writer_ShouldBasicallyWork(byte[] nums)
        {
            using (var writer = new ReqWriter())
            {
                writer.FieldBeforeList = nums.First();
                writer.Nums.AddRange(nums.Skip(1).Select(_ => (int)_).ToList());
                writer.Nums.RemoveAt(writer.Nums.Count - 1);
                writer.FieldAfterList = nums.Last();
                Assert.Equal(writer.FieldBeforeList, nums.First());
                Assert.Equal(writer.Nums.Count, nums.Length);
                Assert.Equal(writer.Nums[0], nums[1]);
                Assert.Equal(writer.FieldAfterList, nums.Last());
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
                Assert.True(((PropertyInfo)typeof(RespWriter).GetMember("result").Single()).PropertyType == typeof(StringAccessor));
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
                Assert.True(((PropertyInfo)typeof(RespReader).GetMember("result").Single()).PropertyType == typeof(StringAccessor));
            }
        }

    }

    [Collection("TestServer Collection")]
    public unsafe class ProtocolTest
    {
        private TrinityServerFixture Fixture { get; }
        public ProtocolTest(TrinityServerFixture fixture) { Fixture = fixture; }

        public IEnumerable<object[]> GetData()
        {
            yield return new object[] { 1, new int[] { 1, 2, 3, 4 }, 4 };
            yield return new object[] { 2, new int[] { 2, 0, 4, 8 }, 8 };
            yield return new object[] { 233, new int[] { 123, 124, 75, 43 }, 128 };
            yield return new object[] { 12, new int[] { 77, 88, 9, 8 }, 8 };
            yield return new object[] { 12, new int[] { 77, 88, 9, 8, 77, 88, 9, 8, 77, 88, 9, 8, 77, 88, 9, 8 }, 8 };
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void SynWithRsp_Test(int before, int[] nums, byte after)
        {
            using (var writer = PrepareWriter(before, nums, after))
            {
                using (var response = Global.CloudStorage.TestSynWithRspToTestServer(Global.MyServerID, writer))
                {
                    string expectedResp;
                    var expectedResult = Utils.CalcForSynRsp(before, nums, after, out expectedResp);
                    Utils.EnsureResults(Fixture.Server, nameof(Fixture.Server.SynWithRspResult), expectedResult);
                    // ensures that it is exactly SynWithRspHandler is called
                    Assert.Equal(expectedResp, response.result);
                }
                Fixture.Server.ResetCounts();
                using (var response = Global.CloudStorage.TestSynWithRsp1ToTestServer(Global.MyServerID, writer))
                {
                    string expectedResp;
                    var expectedResult = Utils.CalcForSynRsp(before, nums, after, out expectedResp);
                    Utils.EnsureResults(Fixture.Server, nameof(Fixture.Server.SynWithRsp1Result), expectedResult);
                    Assert.Equal(expectedResp, response.result);
                }
                Fixture.Server.ResetCounts();
            }
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void Syn_Test(int before, int[] nums, byte after)
        {
            using (var writer = PrepareWriter(before, nums, after))
            {
                Global.CloudStorage.TestSynToTestServer(Global.MyServerID, writer);
                {
                    Utils.EnsureResults(Fixture.Server, nameof(Fixture.Server.SynResult), Utils.CalcForSyn(before, nums, after));
                }
                Fixture.Server.ResetCounts();

                Global.CloudStorage.TestSyn1ToTestServer(Global.MyServerID, writer);
                {
                    Utils.EnsureResults(Fixture.Server, nameof(Fixture.Server.Syn1Result), Utils.CalcForSyn(before, nums, after));
                }
                Fixture.Server.ResetCounts();
            }
        }

        [Theory]
        [MemberData(nameof(GetData))]
        public void Asyn_Test(int before, int[] nums, byte after)
        {
            using (var writer = PrepareWriter(before, nums, after))
            {
                Global.CloudStorage.TestAsynToTestServer(Global.MyServerID, writer);
                {
                    Utils.EnsureResults(Fixture.Server, nameof(Fixture.Server.AsynResult), Utils.CalcForAsyn(before, nums, after));
                }
                Fixture.Server.ResetCounts();

                Global.CloudStorage.TestAsyn1ToTestServer(Global.MyServerID, writer);
                {
                    Utils.EnsureResults(Fixture.Server, nameof(Fixture.Server.Asyn1Result), Utils.CalcForAsyn(before, nums, after));
                }
                Fixture.Server.ResetCounts();
            }
        }

        private static ReqWriter PrepareWriter(int before, int[] nums, byte after)
        {
            var writer = new ReqWriter();
            writer.FieldBeforeList = before;
            writer.Nums.Add(nums[0]);
            writer.Nums.AddRange(nums.Skip(1).ToList());
            writer.Nums.RemoveAt(writer.Nums.Count - 1);
            writer.Nums.Add(nums.Last());
            writer.FieldAfterList = after;
            return writer;
        }
    }
}
