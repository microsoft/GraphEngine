using System.Linq;
using System.Reflection;
using Xunit;

namespace tsl3
{
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
                Assert.Equal(nums.First(), writer.FieldBeforeList);
                Assert.Equal(nums.Length - 2, writer.Nums.Count);
                Assert.Equal(nums[1], writer.Nums[0]);
                Assert.Equal(nums.Last(), writer.FieldAfterList);
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
                    Assert.Equal(-42, reader.FieldBeforeList);
                    Assert.Equal(1, reader.Nums.Count);
                    Assert.Equal(100, reader.Nums[0]);
                    Assert.Equal(42, reader.FieldAfterList);
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
}