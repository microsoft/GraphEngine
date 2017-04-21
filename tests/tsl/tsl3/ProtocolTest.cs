using System.Collections.Generic;
using System.Linq;
using Trinity;
using Xunit;

namespace tsl3
{
    [Collection("TestServer Collection")]
    public unsafe class ProtocolTest
    {
        private TrinityServerFixture Fixture { get; }
        public ProtocolTest(TrinityServerFixture fixture) { Fixture = fixture; }

        public static IEnumerable<object[]> GetData()
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