using System;
using Trinity;
using Xunit;

namespace GraphEngine.Jit.UnitTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            IntPtr fn = IntPtr.Zero;
            Assert.Equal(0, Assembly.AsmJit.test(ref fn));
        }

        [Fact]
        public void Test2()
        {
            IntPtr fn = IntPtr.Zero;
            Assert.Equal(0, Assembly.AsmJit.test(ref fn));
            Assert.NotEqual(IntPtr.Zero, fn);
            Console.WriteLine(Assembly.AsmJit.test2(fn));
        }
    }
}
