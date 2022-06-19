// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Trinity.Modules.Spark;
using Xunit;

namespace SparkTrinityModule.UnitTests
{
    public class DataTypeUnitTests
    {
        [Fact]
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
                Assert.Equal(typeof(DataType), dt.GetType());
                Assert.Equal(type.FullName, dt.TypeName);
            });
        }

        [Fact]
        public void ConvertFromType_SpecialValueType_ReturnDataType()
        {
            var primitiveTypes = new List<Type>
            {
                typeof(decimal), typeof(DateTime), typeof(Guid)
            };

            primitiveTypes.ForEach(type =>
            {
                var dt = DataType.ConvertFromType(type);
                Assert.Equal(typeof(DataType), dt.GetType());
                Assert.Equal(type.FullName, dt.TypeName);
            });
        }

        [Fact]
        public void ConvertFromType_StringType_ReturnDataType()
        {
            var type = typeof(string);
            var dt = DataType.ConvertFromType(type);
            Assert.Equal(typeof(DataType), dt.GetType());
            Assert.Equal(type.FullName, dt.TypeName);
        }

        public struct TestStruct
        {
            public int testField;
        }

        [Fact]
        public void ConvertFromType_StructType_ReturnStructType()
        {
            var type = typeof(TestStruct);
            var dt = DataType.ConvertFromType(type) as StructType;
            Assert.NotNull(dt);
            Assert.Equal(typeof(StructType).Name, dt.TypeName);

            type.GetFields().ToList().ForEach(f =>
            {
                var field = dt.Fields.FirstOrDefault(x => x.Name == f.Name);
                Assert.NotNull(field);
            });
        }

        [Fact]
        public void ConvertFromType_Nullable_ReturnNullableValueType()
        {
            var nullableTypes = new List<Type>
            {
                typeof(int?), typeof(TestStruct?)
            };

            var dataTypes = nullableTypes.Select(type => DataType.ConvertFromType(type) as NullableValueType).ToList();
            dataTypes.ForEach(dt =>
            {
                Assert.NotNull(dt);
                Assert.Equal(typeof(NullableValueType).Name, dt.TypeName);
            });

            Assert.Equal(typeof(int).FullName, dataTypes[0].ArgumentType.TypeName);
            Assert.Equal(typeof(StructType), dataTypes[1].ArgumentType.GetType());
        }

        [Fact]
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
                Assert.NotNull(dt);
                Assert.Equal(typeof(ArrayType).Name, dt.TypeName);
                Assert.Equal(expectedElementDataTypes[i], dt.ElementType.GetType());
            }
        }

        [Fact]
        public void ConvertFromType_Unsupported_ReturnDataType()
        {
            var unsupportedTypes = new List<Type>
            {
                typeof(Queue<int>), typeof(Tuple<int, bool>), typeof(Dictionary<string, int>)
            };

            unsupportedTypes.ForEach(type =>
            {
                var dt = DataType.ConvertFromType(type);
                Assert.Equal(typeof(DataType), dt.GetType());
                Assert.Equal(type.FullName, dt.TypeName);
            });
        }

        class TestClass
        {
            public int Foo { get; set; }

            [DataMember(Name = "bar")]
            public int Bar { get; set; }
        }

        [Fact]
        public void ConvertFromType_DataMemberName()
        {
            var dt = DataType.ConvertFromType(typeof(TestClass)) as StructType;
            Assert.NotNull(dt);
            Assert.Equal(2, dt.Fields.Count());
            Assert.Equal("Foo", dt.Fields.ElementAt(0).Name);
            Assert.Equal("bar", dt.Fields.ElementAt(1).Name);
        }
    }
}
