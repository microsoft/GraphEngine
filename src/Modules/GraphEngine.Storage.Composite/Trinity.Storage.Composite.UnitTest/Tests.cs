using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace Trinity.Storage.Composite.UnitTest
{
    public class Tests
    {
        [Fact]
        public void FindTslCodegen()
        {
            Assert.True(Commands.TSLCodeGenCmd(""));
        }

        [Fact]
        public void FindDotnet()
        {
            Assert.True(Commands.DotNetBuildCmd(""));
        }
    }
}
