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
            Assert.Equal(TrinityErrorCode.E_SUCCESS, Assembly.AsmJit.test(ref fn));
        }

        [Fact]
        public void Test2()
        {
            IntPtr fn = IntPtr.Zero;
            Assert.Equal(TrinityErrorCode.E_SUCCESS, Assembly.AsmJit.test(ref fn));
            Assert.NotEqual(IntPtr.Zero, fn);
            Assembly.AsmJit.test2(fn);
        }
    }
}
