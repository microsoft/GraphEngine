using System.Collections.Generic;
using Xunit;

namespace Trinity.FFI.UnitTests
{
    public struct TestStruct_1
    {
        public int foo;
        public double bar;
    }

    public struct TestStruct_2
    {
        public string f1;
        public string f2;
        public string f3;
        public string f4;
    }

    public struct TestStruct_3
    {
        public TestStruct_2 f1;
        public string f2;
        public List<TestStruct_1> f3;
    }

    public class TestClass_1
    {
        public int foo;
        public double bar;
    }

    public class TestClass_2
    {
        public string f1;
        public string f2;
        public string f3;
        public string f4;
    }

    public class TestClass_3
    {
        public TestClass_2 f1;
        public string f2;
        public List<TestClass_1> f3;
    }

    public unsafe class Tests
    {
        [Fact]
        public void Allocate_Deallocate_Fine()
        {
            var ptype = TypeCodec.EncodeType<int>();
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<uint>();
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<string>();
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<double>();
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<List<int>>();
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<List<long>>();
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<TestStruct_1>();
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<TestStruct_2>();
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<TestStruct_3>();
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<TestClass_1>();
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<TestClass_2>();
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<TestClass_3>();
            TypeCodec.FreeTypeCode(ptype);
        }

        [Fact]
        public void Decode_Fine()
        {
            var ptype = TypeCodec.EncodeType<int>();
            Assert.Equal(typeof(int), TypeCodec.DecodeType(ptype));
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<uint>();
            Assert.Equal(typeof(uint), TypeCodec.DecodeType(ptype));
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<string>();
            Assert.Equal(typeof(string), TypeCodec.DecodeType(ptype));
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<double>();
            Assert.Equal(typeof(double), TypeCodec.DecodeType(ptype));
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<List<int>>();
            Assert.Equal(typeof(List<int>), TypeCodec.DecodeType(ptype));
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<List<long>>();
            Assert.Equal(typeof(List<long>), TypeCodec.DecodeType(ptype));
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<TestStruct_1>();
            Assert.Equal(typeof(TestStruct_1), TypeCodec.DecodeType(ptype));
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<TestStruct_2>();
            Assert.Equal(typeof(TestStruct_2), TypeCodec.DecodeType(ptype));
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<TestStruct_3>();
            Assert.Equal(typeof(TestStruct_3), TypeCodec.DecodeType(ptype));
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<TestClass_1>();
            Assert.Equal(typeof(TestClass_1), TypeCodec.DecodeType(ptype));
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<TestClass_2>();
            Assert.Equal(typeof(TestClass_2), TypeCodec.DecodeType(ptype));
            TypeCodec.FreeTypeCode(ptype);

            ptype = TypeCodec.EncodeType<TestClass_3>();
            Assert.Equal(typeof(TestClass_3), TypeCodec.DecodeType(ptype));
            TypeCodec.FreeTypeCode(ptype);
        }
    }
}