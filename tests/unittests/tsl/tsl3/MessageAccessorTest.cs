using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace tsl3
{
    public unsafe class MessageAccessorTest
    {
        [TestCase(new byte[] {1, 2, 3, 4})]
        [TestCase(new byte[] {2, 0, 4, 8})]
        [TestCase(new byte[] {123, 124, 75, 43})]
        [TestCase(new byte[] {77, 88, 9, 8})]
        [TestCase(new byte[] {77, 88, 9, 8, 77, 88, 9, 8, 77, 88, 9, 8, 77, 88, 9, 8, })]
        public void Writer_ShouldBasicallyWork(byte[] nums)
        {
            using (var writer = new ReqWriter())
            {
                writer.FieldBeforeList = nums.First();
                writer.Nums.AddRange(nums.Skip(1).Select(_ => (int)_).ToList());
                writer.Nums.RemoveAt(writer.Nums.Count - 1);
                writer.FieldAfterList = nums.Last();
                Assert.AreEqual(nums.First(), writer.FieldBeforeList);
                Assert.AreEqual(nums.Length - 2, writer.Nums.Count);
                Assert.AreEqual(nums[1], writer.Nums[0]);
                Assert.AreEqual(nums.Last(), writer.FieldAfterList);

                var req = (Req)writer;
                Assert.AreEqual(nums.First(), req.FieldBeforeList);
                Assert.AreEqual(nums.Length - 2, req.Nums.Count);
                Assert.AreEqual(nums[1], req.Nums[0]);
                Assert.AreEqual(nums.Last(), req.FieldAfterList);
            }
        }

        [Test]
        public void Writer_ShouldHaveProperlyTypedProperties()
        {
            {
                var type = typeof(ReqWriter);
                var properties = type.GetMembers().Where(m => m.MemberType == MemberTypes.Property).Cast<PropertyInfo>().ToList();
                Assert.That(properties.Single(m => m.Name == "FieldBeforeList").PropertyType == typeof(int), "FieldBeforeList");
                Assert.That(properties.Single(m => m.Name == "Nums").PropertyType == typeof(intListAccessor), "Nums");
                Assert.That(properties.Single(m => m.Name == "FieldAfterList").PropertyType == typeof(byte), "FieldAfterList");
            }
            {
                Assert.That(((PropertyInfo)typeof(RespWriter).GetMember("result").Single()).PropertyType == typeof(StringAccessor));
            }
        }

        [Test]
        public void Reader_ShouldBasicallyWork()
        {
            using (var writer = new ReqWriter())
            {
                writer.FieldBeforeList = -42;
                writer.Nums.Add(100);
                writer.FieldAfterList = 42;
                using (var reader = new ReqReader(Utils.MakeCopyOfDataInReqWriter(writer), 0))
                {
                    Assert.AreEqual(-42, reader.FieldBeforeList);
                    Assert.AreEqual(1, reader.Nums.Count);
                    Assert.AreEqual(100, reader.Nums[0]);
                    Assert.AreEqual(42, reader.FieldAfterList);
                }
            }
        }

        [Test]
        public void Reader_ShouldHaveProperlyTypedProperties()
        {
            {
                var type = typeof(ReqReader);
                var properties = type.GetMembers().Where(m => m.MemberType == MemberTypes.Property).Cast<PropertyInfo>().ToList();
                Assert.That(properties.Single(m => m.Name == "FieldBeforeList").PropertyType == typeof(int), "FieldBeforeList");
                Assert.That(properties.Single(m => m.Name == "Nums").PropertyType == typeof(intListAccessor), "Nums");
                Assert.That(properties.Single(m => m.Name == "FieldAfterList").PropertyType == typeof(byte), "FieldAfterList");
            }
            {
                Assert.That(((PropertyInfo)typeof(RespReader).GetMember("result").Single()).PropertyType == typeof(StringAccessor));
            }
        }

    }
}