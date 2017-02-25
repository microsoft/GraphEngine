using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Collections.Generic;

namespace Trinity.Modules.Spark.Tests
{
    [TestClass]
    public class DataTypeUnitTests
    {
        [TestMethod]
        public void ConvertFromType_PrimitiveType_ReturnDataType()
        {
            var primitiveTypes = new List<Type>
            {
                typeof(byte), typeof(sbyte), typeof(bool),
                typeof(char), typeof(short), typeof(ushort),
                typeof(int), typeof(uint), typeof(long), typeof(ulong),
                typeof(float), typeof(double)
            };

            primitiveTypes.ForEach(type =>
            {
                var dt = DataType.ConvertFromType(type);
                Assert.AreEqual(typeof(DataType), dt.GetType());
                Assert.AreEqual(type.FullName, dt.TypeName);
                Console.WriteLine(dt.TypeName);
            });
        }

        [TestMethod]
        public void ConvertFromType_SpecialValueType_ReturnDataType()
        {
            var primitiveTypes = new List<Type>
            {
                typeof(decimal), typeof(DateTime), typeof(Guid)
            };

            primitiveTypes.ForEach(type =>
            {
                var dt = DataType.ConvertFromType(type);
                Assert.AreEqual(typeof(DataType), dt.GetType());
                Assert.AreEqual(type.FullName, dt.TypeName);
                Console.WriteLine(dt.TypeName);
            });
        }

        [TestMethod]
        public void ConvertFromType_StringType_ReturnDataType()
        {
            var type = typeof(string);
            var dt = DataType.ConvertFromType(type);
            Assert.AreEqual(typeof(DataType), dt.GetType());
            Assert.AreEqual(type.FullName, dt.TypeName);
        }

        public struct TestStruct
        {
            public int testField;
        }

        [TestMethod]
        public void ConvertFromType_StructType_ReturnStructType()
        {
            var type = typeof(TestStruct);
            var dt = DataType.ConvertFromType(type) as StructType;
            Assert.IsNotNull(dt);
            Assert.AreEqual(typeof(StructType).Name, dt.TypeName);

            type.GetFields().ToList().ForEach(f =>
            {
                var field = dt.Fields.FirstOrDefault(x => x.Name == f.Name);
                Assert.IsNotNull(field);
            });
        }

        [TestMethod]
        public void ConvertFromType_Nullable_ReturnNullableValueType()
        {
            var nullableTypes = new List<Type>
            {
                typeof(int?), typeof(TestStruct?)
            };

            var dataTypes = nullableTypes.Select(type => DataType.ConvertFromType(type) as NullableValueType).ToList();
            dataTypes.ForEach(dt =>
            {
                Assert.IsNotNull(dt);
                Assert.AreEqual(typeof(NullableValueType).Name, dt.TypeName);
            });

            Assert.AreEqual(typeof(int).FullName, dataTypes[0].ArgumentType.TypeName);
            Assert.AreEqual(typeof(StructType), dataTypes[1].ArgumentType.GetType());
        }

        [TestMethod]
        public void ConvertFromType_List_ReturnArrayType()
        {
            var listTypes = new List<Type>
            {
                typeof(List<int>), typeof(List<string>), typeof(List<TestStruct>),
                typeof(List<int?>), typeof(List<List<int>>)
            };

            var expectedElementDataTypes = new List<Type>
            {
                typeof(DataType), typeof(DataType), typeof(StructType),
                typeof(NullableValueType), typeof(ArrayType)
            };

            for (int i = 0; i < listTypes.Count; i++)
            {
                var dt = DataType.ConvertFromType(listTypes[i]) as ArrayType;
                Assert.IsNotNull(dt);
                Assert.AreEqual(typeof(ArrayType).Name, dt.TypeName);
                Assert.AreEqual(expectedElementDataTypes[i], dt.ElementType.GetType());
            }
        }

        [TestMethod]
        public void ConvertFromType_Unsupported_ReturnDataType()
        {
            var unsupportedTypes = new List<Type>
            {
                typeof(Queue<int>), typeof(Tuple<int, bool>), typeof(Dictionary<string, int>)
            };

            unsupportedTypes.ForEach(type =>
            {
                var dt = DataType.ConvertFromType(type);
                Assert.AreEqual(typeof(DataType), dt.GetType());
                Assert.AreEqual(type.FullName, dt.TypeName);
            });
        }
    }
}
