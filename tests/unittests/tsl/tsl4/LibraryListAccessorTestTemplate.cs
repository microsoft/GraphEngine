using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace tsl4
{
    public class IntListAccessorTests
    {
        #region CopyAll
        [Test]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = new int[10];
            writer.IntList.CopyTo(array);
            Assert.That(array, Is.EqualTo(writer.IntList.Select(e => (int)e)));
        });

        [Test]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new int[5];
            Assert.That(() => { writer.IntList.CopyTo(array); }, Throws.ArgumentException);
        });
        #endregion

        #region CopyAllWithDstOffset
        [Test]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = new int[15];
            writer.IntList.CopyTo(array, 5);
            Assert.That(array.Take(5), Has.Exactly(5).EqualTo(default(int)));
            Assert.That(array.Skip(5), Is.EqualTo(writer.IntList.Select(e => (int)e)));
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new int[5];
            Assert.That(() => { writer.IntList.CopyTo(array, 3); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new int[100];
            Assert.That(() => { writer.IntList.CopyTo(array, 102); }, Throws.ArgumentException);
            Assert.That(() => { writer.IntList.CopyTo(array, -20); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = new int[15];
            writer.IntList.CopyTo(2, array, 3, 5);
            Assert.That(array.Take(3), Has.Exactly(3).EqualTo(default(int)));
            Assert.That(array.Skip(3).Take(5), Is.EqualTo(writer.IntList.Skip(2).Take(5).Select(e => (int)e)));
            Assert.That(array.Skip(8), Has.Exactly(15 - 8).EqualTo(default(int)));
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new int[5];
            Assert.That(() => { writer.IntList.CopyTo(2, array, 3, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.IntList.CopyTo(0, array, 1, 5); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new int[100];
            Assert.That(() => { writer.IntList.CopyTo(2, array, 102, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.IntList.CopyTo(2, array, -20, 5); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new int[100];
            Assert.That(() => { writer.IntList.CopyTo(200, array, 0, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.IntList.CopyTo(-200, array, 0, 200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new int[100];
            Assert.That(() => { writer.IntList.CopyTo(200, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => { writer.IntList.CopyTo(0, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion
    }

    public class ByteListAccessorTests
    {
        #region CopyAll
        [Test]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = new byte[10];
            writer.ByteList.CopyTo(array);
            Assert.That(array, Is.EqualTo(writer.ByteList.Select(e => (byte)e)));
        });

        [Test]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new byte[5];
            Assert.That(() => { writer.ByteList.CopyTo(array); }, Throws.ArgumentException);
        });
        #endregion

        #region CopyAllWithDstOffset
        [Test]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = new byte[15];
            writer.ByteList.CopyTo(array, 5);
            Assert.That(array.Take(5), Has.Exactly(5).EqualTo(default(byte)));
            Assert.That(array.Skip(5), Is.EqualTo(writer.ByteList.Select(e => (byte)e)));
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new byte[5];
            Assert.That(() => { writer.ByteList.CopyTo(array, 3); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new byte[100];
            Assert.That(() => { writer.ByteList.CopyTo(array, 102); }, Throws.ArgumentException);
            Assert.That(() => { writer.ByteList.CopyTo(array, -20); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = new byte[15];
            writer.ByteList.CopyTo(2, array, 3, 5);
            Assert.That(array.Take(3), Has.Exactly(3).EqualTo(default(byte)));
            Assert.That(array.Skip(3).Take(5), Is.EqualTo(writer.ByteList.Skip(2).Take(5).Select(e => (byte)e)));
            Assert.That(array.Skip(8), Has.Exactly(15 - 8).EqualTo(default(byte)));
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new byte[5];
            Assert.That(() => { writer.ByteList.CopyTo(2, array, 3, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.ByteList.CopyTo(0, array, 1, 5); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new byte[100];
            Assert.That(() => { writer.ByteList.CopyTo(2, array, 102, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.ByteList.CopyTo(2, array, -20, 5); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new byte[100];
            Assert.That(() => { writer.ByteList.CopyTo(200, array, 0, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.ByteList.CopyTo(-200, array, 0, 200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new byte[100];
            Assert.That(() => { writer.ByteList.CopyTo(200, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => { writer.ByteList.CopyTo(0, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion
    }

    public class DoubleListAccessorTests
    {
        #region CopyAll
        [Test]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = new double[10];
            writer.DoubleList.CopyTo(array);
            Assert.That(array, Is.EqualTo(writer.DoubleList.Select(e => (double)e)));
        });

        [Test]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new double[5];
            Assert.That(() => { writer.DoubleList.CopyTo(array); }, Throws.ArgumentException);
        });
        #endregion

        #region CopyAllWithDstOffset
        [Test]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = new double[15];
            writer.DoubleList.CopyTo(array, 5);
            Assert.That(array.Take(5), Has.Exactly(5).EqualTo(default(double)));
            Assert.That(array.Skip(5), Is.EqualTo(writer.DoubleList.Select(e => (double)e)));
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new double[5];
            Assert.That(() => { writer.DoubleList.CopyTo(array, 3); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new double[100];
            Assert.That(() => { writer.DoubleList.CopyTo(array, 102); }, Throws.ArgumentException);
            Assert.That(() => { writer.DoubleList.CopyTo(array, -20); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = new double[15];
            writer.DoubleList.CopyTo(2, array, 3, 5);
            Assert.That(array.Take(3), Has.Exactly(3).EqualTo(default(double)));
            Assert.That(array.Skip(3).Take(5), Is.EqualTo(writer.DoubleList.Skip(2).Take(5).Select(e => (double)e)));
            Assert.That(array.Skip(8), Has.Exactly(15 - 8).EqualTo(default(double)));
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new double[5];
            Assert.That(() => { writer.DoubleList.CopyTo(2, array, 3, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.DoubleList.CopyTo(0, array, 1, 5); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new double[100];
            Assert.That(() => { writer.DoubleList.CopyTo(2, array, 102, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.DoubleList.CopyTo(2, array, -20, 5); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new double[100];
            Assert.That(() => { writer.DoubleList.CopyTo(200, array, 0, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.DoubleList.CopyTo(-200, array, 0, 200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new double[100];
            Assert.That(() => { writer.DoubleList.CopyTo(200, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => { writer.DoubleList.CopyTo(0, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion
    }

    public class LongListAccessorTests
    {
        #region CopyAll
        [Test]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = new long[10];
            writer.LongList.CopyTo(array);
            Assert.That(array, Is.EqualTo(writer.LongList.Select(e => (long)e)));
        });

        [Test]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new long[5];
            Assert.That(() => { writer.LongList.CopyTo(array); }, Throws.ArgumentException);
        });
        #endregion

        #region CopyAllWithDstOffset
        [Test]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = new long[15];
            writer.LongList.CopyTo(array, 5);
            Assert.That(array.Take(5), Has.Exactly(5).EqualTo(default(long)));
            Assert.That(array.Skip(5), Is.EqualTo(writer.LongList.Select(e => (long)e)));
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new long[5];
            Assert.That(() => { writer.LongList.CopyTo(array, 3); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new long[100];
            Assert.That(() => { writer.LongList.CopyTo(array, 102); }, Throws.ArgumentException);
            Assert.That(() => { writer.LongList.CopyTo(array, -20); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = new long[15];
            writer.LongList.CopyTo(2, array, 3, 5);
            Assert.That(array.Take(3), Has.Exactly(3).EqualTo(default(long)));
            Assert.That(array.Skip(3).Take(5), Is.EqualTo(writer.LongList.Skip(2).Take(5).Select(e => (long)e)));
            Assert.That(array.Skip(8), Has.Exactly(15 - 8).EqualTo(default(long)));
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new long[5];
            Assert.That(() => { writer.LongList.CopyTo(2, array, 3, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.LongList.CopyTo(0, array, 1, 5); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new long[100];
            Assert.That(() => { writer.LongList.CopyTo(2, array, 102, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.LongList.CopyTo(2, array, -20, 5); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new long[100];
            Assert.That(() => { writer.LongList.CopyTo(200, array, 0, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.LongList.CopyTo(-200, array, 0, 200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new long[100];
            Assert.That(() => { writer.LongList.CopyTo(200, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => { writer.LongList.CopyTo(0, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion
    }

    public class StringListAccessorTests
    {
        #region CopyAll
        [Test]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = new string[10];
            writer.StringList.CopyTo(array);
            Assert.That(array, Is.EqualTo(writer.StringList.Select(e => (string)e)));
        });

        [Test]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new string[5];
            Assert.That(() => { writer.StringList.CopyTo(array); }, Throws.ArgumentException);
        });
        #endregion

        #region CopyAllWithDstOffset
        [Test]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = new string[15];
            writer.StringList.CopyTo(array, 5);
            Assert.That(array.Take(5), Has.Exactly(5).EqualTo(default(string)));
            Assert.That(array.Skip(5), Is.EqualTo(writer.StringList.Select(e => (string)e)));
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new string[5];
            Assert.That(() => { writer.StringList.CopyTo(array, 3); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new string[100];
            Assert.That(() => { writer.StringList.CopyTo(array, 102); }, Throws.ArgumentException);
            Assert.That(() => { writer.StringList.CopyTo(array, -20); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = new string[15];
            writer.StringList.CopyTo(2, array, 3, 5);
            Assert.That(array.Take(3), Has.Exactly(3).EqualTo(default(string)));
            Assert.That(array.Skip(3).Take(5), Is.EqualTo(writer.StringList.Skip(2).Take(5).Select(e => (string)e)));
            Assert.That(array.Skip(8), Has.Exactly(15 - 8).EqualTo(default(string)));
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new string[5];
            Assert.That(() => { writer.StringList.CopyTo(2, array, 3, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.StringList.CopyTo(0, array, 1, 5); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new string[100];
            Assert.That(() => { writer.StringList.CopyTo(2, array, 102, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.StringList.CopyTo(2, array, -20, 5); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new string[100];
            Assert.That(() => { writer.StringList.CopyTo(200, array, 0, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.StringList.CopyTo(-200, array, 0, 200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new string[100];
            Assert.That(() => { writer.StringList.CopyTo(200, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => { writer.StringList.CopyTo(0, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion
    }

    public class FixedLengthStructListAccessorTests
    {
        #region CopyAll
        [Test]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = new FixedLengthStruct[10];
            writer.FixedLengthStructList.CopyTo(array);
            Assert.That(array, Is.EqualTo(writer.FixedLengthStructList.Select(e => (FixedLengthStruct)e)));
        });

        [Test]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new FixedLengthStruct[5];
            Assert.That(() => { writer.FixedLengthStructList.CopyTo(array); }, Throws.ArgumentException);
        });
        #endregion

        #region CopyAllWithDstOffset
        [Test]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = new FixedLengthStruct[15];
            writer.FixedLengthStructList.CopyTo(array, 5);
            Assert.That(array.Take(5), Has.Exactly(5).EqualTo(default(FixedLengthStruct)));
            Assert.That(array.Skip(5), Is.EqualTo(writer.FixedLengthStructList.Select(e => (FixedLengthStruct)e)));
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new FixedLengthStruct[5];
            Assert.That(() => { writer.FixedLengthStructList.CopyTo(array, 3); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new FixedLengthStruct[100];
            Assert.That(() => { writer.FixedLengthStructList.CopyTo(array, 102); }, Throws.ArgumentException);
            Assert.That(() => { writer.FixedLengthStructList.CopyTo(array, -20); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = new FixedLengthStruct[15];
            writer.FixedLengthStructList.CopyTo(2, array, 3, 5);
            Assert.That(array.Take(3), Has.Exactly(3).EqualTo(default(FixedLengthStruct)));
            Assert.That(array.Skip(3).Take(5), Is.EqualTo(writer.FixedLengthStructList.Skip(2).Take(5).Select(e => (FixedLengthStruct)e)));
            Assert.That(array.Skip(8), Has.Exactly(15 - 8).EqualTo(default(FixedLengthStruct)));
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new FixedLengthStruct[5];
            Assert.That(() => { writer.FixedLengthStructList.CopyTo(2, array, 3, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.FixedLengthStructList.CopyTo(0, array, 1, 5); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new FixedLengthStruct[100];
            Assert.That(() => { writer.FixedLengthStructList.CopyTo(2, array, 102, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.FixedLengthStructList.CopyTo(2, array, -20, 5); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new FixedLengthStruct[100];
            Assert.That(() => { writer.FixedLengthStructList.CopyTo(200, array, 0, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.FixedLengthStructList.CopyTo(-200, array, 0, 200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new FixedLengthStruct[100];
            Assert.That(() => { writer.FixedLengthStructList.CopyTo(200, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => { writer.FixedLengthStructList.CopyTo(0, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion
    }

    public class VariableLengthStructListAccessorTests
    {
        #region CopyAll
        [Test]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = new VariableLengthStruct[10];
            writer.VariableLengthStructList.CopyTo(array);
            Assert.That(array, Is.EqualTo(writer.VariableLengthStructList.Select(e => (VariableLengthStruct)e)));
        });

        [Test]
        public void CopyToTest_CopyAll_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new VariableLengthStruct[5];
            Assert.That(() => { writer.VariableLengthStructList.CopyTo(array); }, Throws.ArgumentException);
        });
        #endregion

        #region CopyAllWithDstOffset
        [Test]
        public void CopyToTest_CopyAllWithDstOffset_Success() => Utils.WithWriter(writer =>
        {
            var array = new VariableLengthStruct[15];
            writer.VariableLengthStructList.CopyTo(array, 5);
            Assert.That(array.Take(5), Has.Exactly(5).EqualTo(default(VariableLengthStruct)));
            Assert.That(array.Skip(5), Is.EqualTo(writer.VariableLengthStructList.Select(e => (VariableLengthStruct)e)));
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new VariableLengthStruct[5];
            Assert.That(() => { writer.VariableLengthStructList.CopyTo(array, 3); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithDstOffset_OffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new VariableLengthStruct[100];
            Assert.That(() => { writer.VariableLengthStructList.CopyTo(array, 102); }, Throws.ArgumentException);
            Assert.That(() => { writer.VariableLengthStructList.CopyTo(array, -20); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion

        #region CopyWithOffsetsAndCount
        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_Success() => Utils.WithWriter(writer =>
        {
            var array = new VariableLengthStruct[15];
            writer.VariableLengthStructList.CopyTo(2, array, 3, 5);
            Assert.That(array.Take(3), Has.Exactly(3).EqualTo(default(VariableLengthStruct)));
            Assert.That(array.Skip(3).Take(5), Is.EqualTo(writer.VariableLengthStructList.Skip(2).Take(5).Select(e => (VariableLengthStruct)e)));
            Assert.That(array.Skip(8), Has.Exactly(15 - 8).EqualTo(default(VariableLengthStruct)));
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstNoSpace() => Utils.WithWriter(writer =>
        {
            var array = new VariableLengthStruct[5];
            Assert.That(() => { writer.VariableLengthStructList.CopyTo(2, array, 3, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.VariableLengthStructList.CopyTo(0, array, 1, 5); }, Throws.ArgumentException);
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_DstOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new VariableLengthStruct[100];
            Assert.That(() => { writer.VariableLengthStructList.CopyTo(2, array, 102, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.VariableLengthStructList.CopyTo(2, array, -20, 5); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_SrcOffsetOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new VariableLengthStruct[100];
            Assert.That(() => { writer.VariableLengthStructList.CopyTo(200, array, 0, 5); }, Throws.ArgumentException);
            Assert.That(() => { writer.VariableLengthStructList.CopyTo(-200, array, 0, 200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new VariableLengthStruct[100];
            Assert.That(() => { writer.VariableLengthStructList.CopyTo(200, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => { writer.VariableLengthStructList.CopyTo(0, array, 0, -200); }, Throws.TypeOf<ArgumentOutOfRangeException>());
        });
        #endregion
    }


}
