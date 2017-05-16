using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;

namespace tsl3
{
    public class StructTest
    {
        [Test]
        public void ReqStruct_TypeIsValueType()
        {
            Assert.That(typeof(Req).IsValueType);
        }

        [Test]
        public void ReqStruct_ShouldHaveDefaultCtor()
        {
            var req = new Req();
        }

        [Test]
        public void ReqStruct_ShouldHaveDefinedFields()
        {
            var req = new Req();
            req.FieldBeforeList = 1984;
            req.FieldAfterList = 42;
            Assert.Null(req.Nums);
            req.Nums = Enumerable.Concat(Enumerable.Repeat(2, 1), Enumerable.Repeat(3, 10)).ToList();
        }

        [Test]
        public void ReqStruct_FieldTypeShouldMatch()
        {
            var type = typeof(Req);
            var fields = type.GetMembers().Where(m => m.MemberType == MemberTypes.Field).Cast<FieldInfo>().ToList();
            Assert.That(fields.First(m => m.Name == "FieldBeforeList").FieldType == typeof(int), "FieldBeforeList");
            Assert.That(fields.First(m => m.Name == "Nums").FieldType == typeof(List<int>), "Nums");
            Assert.That(fields.First(m => m.Name == "FieldAfterList").FieldType == typeof(byte), "FieldAfterList");
        }
    }
}