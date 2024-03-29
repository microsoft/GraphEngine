using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;
using Trinity;
using Trinity.Storage;

namespace tsl4
{

    public class IntListAccessorTests
    {
        #region Util

        private int[] GetArray(int size) => new int[size];

        #endregion

        #region CopyAll
        [Fact]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(10);
            writer.IntList.CopyTo(array);
            Assert.Equal(writer.IntList.Select(e => (int)e), array);
        });

        [Fact]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.IntList.CopyTo(array); });
        });
        #endregion

        #region CopyAllWithDstOffset
        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.IntList.CopyTo(array, 5);
            Assert.Equal(new int[5], array.Take(5));
            Assert.Equal(writer.IntList.Select(e => (int)e), array.Skip(5));
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.IntList.CopyTo(array, 3); });
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.IntList.CopyTo(array, 102); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.IntList.CopyTo(array, -20); });
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.IntList.CopyTo(2, array, 3, 5);
            Assert.Equal(Enumerable.Range(0, 3).Select(_ => default(int)), array.Take(3));
            Assert.Equal(writer.IntList.Skip(2).Take(5).Select(e => (int)e), array.Skip(3).Take(5));
            Assert.Equal(Enumerable.Range(0, 15 - 8).Select(_ => default(int)), array.Skip(8));
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.IntList.CopyTo(2, array, 3, 5); });
            Assert.Throws<ArgumentException>(() => { writer.IntList.CopyTo(0, array, 1, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.IntList.CopyTo(2, array, 102, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.IntList.CopyTo(2, array, -20, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.IntList.CopyTo(200, array, 0, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.IntList.CopyTo(-200, array, 0, 200); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.IntList.CopyTo(200, array, 0, -200); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.IntList.CopyTo(0, array, 0, -200); });
        });
        #endregion
    }


    public class ByteListAccessorTests
    {
        #region Util

        private byte[] GetArray(int size) => new byte[size];

        #endregion

        #region CopyAll
        [Fact]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(10);
            writer.ByteList.CopyTo(array);
            Assert.Equal(writer.ByteList.Select(e => (byte)e), array);
        });

        [Fact]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.ByteList.CopyTo(array); });
        });
        #endregion

        #region CopyAllWithDstOffset
        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.ByteList.CopyTo(array, 5);
            Assert.Equal(new byte[5], array.Take(5));
            Assert.Equal(writer.ByteList.Select(e => (byte)e), array.Skip(5));
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.ByteList.CopyTo(array, 3); });
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.ByteList.CopyTo(array, 102); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.ByteList.CopyTo(array, -20); });
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.ByteList.CopyTo(2, array, 3, 5);
            Assert.Equal(Enumerable.Range(0, 3).Select(_ => default(byte)), array.Take(3));
            Assert.Equal(writer.ByteList.Skip(2).Take(5).Select(e => (byte)e), array.Skip(3).Take(5));
            Assert.Equal(Enumerable.Range(0, 15 - 8).Select(_ => default(byte)), array.Skip(8));
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.ByteList.CopyTo(2, array, 3, 5); });
            Assert.Throws<ArgumentException>(() => { writer.ByteList.CopyTo(0, array, 1, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.ByteList.CopyTo(2, array, 102, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.ByteList.CopyTo(2, array, -20, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.ByteList.CopyTo(200, array, 0, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.ByteList.CopyTo(-200, array, 0, 200); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.ByteList.CopyTo(200, array, 0, -200); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.ByteList.CopyTo(0, array, 0, -200); });
        });
        #endregion
    }


    public class DoubleListAccessorTests
    {
        #region Util

        private double[] GetArray(int size) => new double[size];

        #endregion

        #region CopyAll
        [Fact]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(10);
            writer.DoubleList.CopyTo(array);
            Assert.Equal(writer.DoubleList.Select(e => (double)e), array);
        });

        [Fact]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.DoubleList.CopyTo(array); });
        });
        #endregion

        #region CopyAllWithDstOffset
        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.DoubleList.CopyTo(array, 5);
            Assert.Equal(new double[5], array.Take(5));
            Assert.Equal(writer.DoubleList.Select(e => (double)e), array.Skip(5));
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.DoubleList.CopyTo(array, 3); });
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.DoubleList.CopyTo(array, 102); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.DoubleList.CopyTo(array, -20); });
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.DoubleList.CopyTo(2, array, 3, 5);
            Assert.Equal(Enumerable.Range(0, 3).Select(_ => default(double)), array.Take(3));
            Assert.Equal(writer.DoubleList.Skip(2).Take(5).Select(e => (double)e), array.Skip(3).Take(5));
            Assert.Equal(Enumerable.Range(0, 15 - 8).Select(_ => default(double)), array.Skip(8));
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.DoubleList.CopyTo(2, array, 3, 5); });
            Assert.Throws<ArgumentException>(() => { writer.DoubleList.CopyTo(0, array, 1, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.DoubleList.CopyTo(2, array, 102, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.DoubleList.CopyTo(2, array, -20, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.DoubleList.CopyTo(200, array, 0, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.DoubleList.CopyTo(-200, array, 0, 200); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.DoubleList.CopyTo(200, array, 0, -200); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.DoubleList.CopyTo(0, array, 0, -200); });
        });
        #endregion
    }


    public class LongListAccessorTests
    {
        #region Util

        private long[] GetArray(int size) => new long[size];

        #endregion

        #region CopyAll
        [Fact]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(10);
            writer.LongList.CopyTo(array);
            Assert.Equal(writer.LongList.Select(e => (long)e), array);
        });

        [Fact]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.LongList.CopyTo(array); });
        });
        #endregion

        #region CopyAllWithDstOffset
        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.LongList.CopyTo(array, 5);
            Assert.Equal(new long[5], array.Take(5));
            Assert.Equal(writer.LongList.Select(e => (long)e), array.Skip(5));
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.LongList.CopyTo(array, 3); });
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.LongList.CopyTo(array, 102); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.LongList.CopyTo(array, -20); });
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.LongList.CopyTo(2, array, 3, 5);
            Assert.Equal(Enumerable.Range(0, 3).Select(_ => default(long)), array.Take(3));
            Assert.Equal(writer.LongList.Skip(2).Take(5).Select(e => (long)e), array.Skip(3).Take(5));
            Assert.Equal(Enumerable.Range(0, 15 - 8).Select(_ => default(long)), array.Skip(8));
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.LongList.CopyTo(2, array, 3, 5); });
            Assert.Throws<ArgumentException>(() => { writer.LongList.CopyTo(0, array, 1, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.LongList.CopyTo(2, array, 102, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.LongList.CopyTo(2, array, -20, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.LongList.CopyTo(200, array, 0, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.LongList.CopyTo(-200, array, 0, 200); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.LongList.CopyTo(200, array, 0, -200); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.LongList.CopyTo(0, array, 0, -200); });
        });
        #endregion
    }


    public class StringListAccessorTests
    {
        #region Util

        private string[] GetArray(int size) => new string[size];

        #endregion

        #region CopyAll
        [Fact]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(10);
            writer.StringList.CopyTo(array);
            Assert.Equal(writer.StringList.Select(e => (string)e), array);
        });

        [Fact]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.StringList.CopyTo(array); });
        });
        #endregion

        #region CopyAllWithDstOffset
        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.StringList.CopyTo(array, 5);
            Assert.Equal(new string[5], array.Take(5));
            Assert.Equal(writer.StringList.Select(e => (string)e), array.Skip(5));
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.StringList.CopyTo(array, 3); });
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.StringList.CopyTo(array, 102); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.StringList.CopyTo(array, -20); });
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.StringList.CopyTo(2, array, 3, 5);
            Assert.Equal(Enumerable.Range(0, 3).Select(_ => default(string)), array.Take(3));
            Assert.Equal(writer.StringList.Skip(2).Take(5).Select(e => (string)e), array.Skip(3).Take(5));
            Assert.Equal(Enumerable.Range(0, 15 - 8).Select(_ => default(string)), array.Skip(8));
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.StringList.CopyTo(2, array, 3, 5); });
            Assert.Throws<ArgumentException>(() => { writer.StringList.CopyTo(0, array, 1, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.StringList.CopyTo(2, array, 102, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.StringList.CopyTo(2, array, -20, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.StringList.CopyTo(200, array, 0, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.StringList.CopyTo(-200, array, 0, 200); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.StringList.CopyTo(200, array, 0, -200); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.StringList.CopyTo(0, array, 0, -200); });
        });
        #endregion
    }


    public class FixedLengthStructListAccessorTests
    {
        #region Util

        private FixedLengthStruct[] GetArray(int size) => new FixedLengthStruct[size];

        #endregion

        #region CopyAll
        [Fact]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(10);
            writer.FixedLengthStructList.CopyTo(array);
            Assert.Equal(writer.FixedLengthStructList.Select(e => (FixedLengthStruct)e), array);
        });

        [Fact]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.FixedLengthStructList.CopyTo(array); });
        });
        #endregion

        #region CopyAllWithDstOffset
        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.FixedLengthStructList.CopyTo(array, 5);
            Assert.Equal(new FixedLengthStruct[5], array.Take(5));
            Assert.Equal(writer.FixedLengthStructList.Select(e => (FixedLengthStruct)e), array.Skip(5));
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.FixedLengthStructList.CopyTo(array, 3); });
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.FixedLengthStructList.CopyTo(array, 102); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.FixedLengthStructList.CopyTo(array, -20); });
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.FixedLengthStructList.CopyTo(2, array, 3, 5);
            Assert.Equal(Enumerable.Range(0, 3).Select(_ => default(FixedLengthStruct)), array.Take(3));
            Assert.Equal(writer.FixedLengthStructList.Skip(2).Take(5).Select(e => (FixedLengthStruct)e), array.Skip(3).Take(5));
            Assert.Equal(Enumerable.Range(0, 15 - 8).Select(_ => default(FixedLengthStruct)), array.Skip(8));
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.FixedLengthStructList.CopyTo(2, array, 3, 5); });
            Assert.Throws<ArgumentException>(() => { writer.FixedLengthStructList.CopyTo(0, array, 1, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.FixedLengthStructList.CopyTo(2, array, 102, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.FixedLengthStructList.CopyTo(2, array, -20, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.FixedLengthStructList.CopyTo(200, array, 0, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.FixedLengthStructList.CopyTo(-200, array, 0, 200); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.FixedLengthStructList.CopyTo(200, array, 0, -200); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.FixedLengthStructList.CopyTo(0, array, 0, -200); });
        });
        #endregion
    }


    public class VariableLengthStructListAccessorTests
    {
        #region Util

        private VariableLengthStruct[] GetArray(int size) => new VariableLengthStruct[size];

        #endregion

        #region CopyAll
        [Fact]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(10);
            writer.VariableLengthStructList.CopyTo(array);
            Assert.Equal(writer.VariableLengthStructList.Select(e => (VariableLengthStruct)e), array);
        });

        [Fact]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.VariableLengthStructList.CopyTo(array); });
        });
        #endregion

        #region CopyAllWithDstOffset
        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.VariableLengthStructList.CopyTo(array, 5);
            Assert.Equal(new VariableLengthStruct[5], array.Take(5));
            Assert.Equal(writer.VariableLengthStructList.Select(e => (VariableLengthStruct)e), array.Skip(5));
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.VariableLengthStructList.CopyTo(array, 3); });
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.VariableLengthStructList.CopyTo(array, 102); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.VariableLengthStructList.CopyTo(array, -20); });
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.VariableLengthStructList.CopyTo(2, array, 3, 5);
            Assert.Equal(Enumerable.Range(0, 3).Select(_ => default(VariableLengthStruct)), array.Take(3));
            Assert.Equal(writer.VariableLengthStructList.Skip(2).Take(5).Select(e => (VariableLengthStruct)e), array.Skip(3).Take(5));
            Assert.Equal(Enumerable.Range(0, 15 - 8).Select(_ => default(VariableLengthStruct)), array.Skip(8));
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.VariableLengthStructList.CopyTo(2, array, 3, 5); });
            Assert.Throws<ArgumentException>(() => { writer.VariableLengthStructList.CopyTo(0, array, 1, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.VariableLengthStructList.CopyTo(2, array, 102, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.VariableLengthStructList.CopyTo(2, array, -20, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.VariableLengthStructList.CopyTo(200, array, 0, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.VariableLengthStructList.CopyTo(-200, array, 0, 200); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.VariableLengthStructList.CopyTo(200, array, 0, -200); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.VariableLengthStructList.CopyTo(0, array, 0, -200); });
        });
        #endregion
    }


    public class IntArrayListAccessorTests
    {
        #region Util

        private int[][,] GetArray(int size)
            => new int[size][,];

        #endregion

        #region CopyAll
        [Fact]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(10);
            writer.IntArrayList.CopyTo(array);
            Assert.Equal(writer.IntArrayList.Select(e => (int[,])e), array);
        });

        [Fact]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.IntArrayList.CopyTo(array); });
        });
        #endregion

        #region CopyAllWithDstOffset
        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.IntArrayList.CopyTo(array, 5);
            Assert.Equal(Enumerable.Range(0, 5).Select(_ => new int[3,2]), array.Take(5));
            Assert.Equal(writer.IntArrayList.Select(e => (int[,])e), array.Skip(5));
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.IntArrayList.CopyTo(array, 3); });
        });

        [Fact]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.IntArrayList.CopyTo(array, 102); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.IntArrayList.CopyTo(array, -20); });
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = GetArray(15);
            writer.IntArrayList.CopyTo(2, array, 3, 5);
            Assert.Equal(Enumerable.Range(0, 3).Select(_ => default(int[,])), array.Take(3));
            Assert.Equal(writer.IntArrayList.Skip(2).Take(5).Select(e => (int[,])e), array.Skip(3).Take(5));
            Assert.Equal(Enumerable.Range(0, 15 - 8).Select(_ => default(int[,])), array.Skip(8));
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = GetArray(5);
            Assert.Throws<ArgumentException>(() => { writer.IntArrayList.CopyTo(2, array, 3, 5); });
            Assert.Throws<ArgumentException>(() => { writer.IntArrayList.CopyTo(0, array, 1, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.IntArrayList.CopyTo(2, array, 102, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.IntArrayList.CopyTo(2, array, -20, 5); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentException>(() => { writer.IntArrayList.CopyTo(200, array, 0, 5); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.IntArrayList.CopyTo(-200, array, 0, 200); });
        });

        [Fact]
        public void CopyToTest_CopyWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = GetArray(100);
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.IntArrayList.CopyTo(200, array, 0, -200); });
            Assert.Throws<ArgumentOutOfRangeException>(() => { writer.IntArrayList.CopyTo(0, array, 0, -200); });
        });
        #endregion
    }



}
