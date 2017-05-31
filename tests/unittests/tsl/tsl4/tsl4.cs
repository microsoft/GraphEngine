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
    public static class Utils
    {
        public static TestStructWriter GetWriter()
            => new TestStructWriter
            {
                IntList = Enumerable.Range(0, 10).ToList(),
                ByteList = Enumerable.Range(0, 10).Select(i => (byte)i).ToList(),
                LongList = Enumerable.Range(0, 10).Select(i => (long)i).ToList(),
                DoubleList = Enumerable.Range(0, 10).Select(i => (double)i).ToList(),
            };

        public static void WithWriter(Action<TestStructWriter> action)
        {
            using (var writer = GetWriter())
                action(writer);
        }
    }

    public class IntListAccessorTests
    {
        #region CopyAll
        [Test]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = new int[10];
            writer.IntList.CopyTo(array);
            Assert.That(array, Is.EquivalentTo(writer.IntList));
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
            Assert.That(array.Take(5), Has.Exactly(5).Zero);
            Assert.That(array.Skip(5), Is.EquivalentTo(writer.IntList));
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
            Assert.That(array.Take(3), Has.Exactly(3).Zero);
            Assert.That(array.Skip(3).Take(5), Is.EquivalentTo(writer.IntList.Skip(2).Take(5)));
            Assert.That(array.Skip(8), Has.Exactly(15 - 8).Zero);
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

    public class LongListAccessorTests
    {
        #region CopyAll

        [Test]
        public void CopyToTest_CopyAll_Success() => Utils.WithWriter(writer =>
        {
            var array = new long[10];
            writer.LongList.CopyTo(array);
            Assert.That(array, Is.EquivalentTo(writer.LongList));
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
            Assert.That(array.Take(5), Has.Exactly(5).Zero);
            Assert.That(array.Skip(5), Is.EquivalentTo(writer.LongList));
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
            Assert.That(array.Take(3), Has.Exactly(3).Zero);
            Assert.That(array.Skip(3).Take(5), Is.EquivalentTo(writer.LongList.Skip(2).Take(5)));
            Assert.That(array.Skip(8), Has.Exactly(15 - 8).Zero);
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
            Assert.That(() => { writer.LongList.CopyTo(-200, array, 0, 200); },
                Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new long[100];
            Assert.That(() => { writer.LongList.CopyTo(200, array, 0, -200); },
                Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => { writer.LongList.CopyTo(0, array, 0, -200); },
                Throws.TypeOf<ArgumentOutOfRangeException>());
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
            Assert.That(array, Is.EquivalentTo(writer.DoubleList));
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
            Assert.That(array.Take(5), Has.Exactly(5).Zero);
            Assert.That(array.Skip(5), Is.EquivalentTo(writer.DoubleList));
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
            Assert.That(array.Take(3), Has.Exactly(3).Zero);
            Assert.That(array.Skip(3).Take(5), Is.EquivalentTo(writer.DoubleList.Skip(2).Take(5)));
            Assert.That(array.Skip(8), Has.Exactly(15 - 8).Zero);
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
            Assert.That(() => { writer.DoubleList.CopyTo(-200, array, 0, 200); },
                Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new double[100];
            Assert.That(() => { writer.DoubleList.CopyTo(200, array, 0, -200); },
                Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => { writer.DoubleList.CopyTo(0, array, 0, -200); },
                Throws.TypeOf<ArgumentOutOfRangeException>());
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
            Assert.That(array, Is.EquivalentTo(writer.ByteList));
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
            Assert.That(array.Take(5), Has.Exactly(5).Zero);
            Assert.That(array.Skip(5), Is.EquivalentTo(writer.ByteList));
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
            Assert.That(array.Take(3), Has.Exactly(3).Zero);
            Assert.That(array.Skip(3).Take(5), Is.EquivalentTo(writer.ByteList.Skip(2).Take(5)));
            Assert.That(array.Skip(8), Has.Exactly(15 - 8).Zero);
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
            Assert.That(() => { writer.ByteList.CopyTo(-200, array, 0, 200); },
                Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        [Test]
        public void CopyToTest_CopyAllWithOffsetsAndCount_CountOutOfRange() => Utils.WithWriter(writer =>
        {
            var array = new byte[100];
            Assert.That(() => { writer.ByteList.CopyTo(200, array, 0, -200); },
                Throws.TypeOf<ArgumentOutOfRangeException>());
            Assert.That(() => { writer.ByteList.CopyTo(0, array, 0, -200); },
                Throws.TypeOf<ArgumentOutOfRangeException>());
        });

        #endregion
    }
}
