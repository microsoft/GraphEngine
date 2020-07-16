using System;
using Xunit;
using Trinity;
using Trinity.Network;

namespace cloud1
{
    public class CloudTest
    {
        [Fact]
        public void issue_267()
        {
            Global.Initialize("trinity.xml");
            var server = new TrinityServer();
            server.Start();

            var contains = Global.CloudStorage.ContainsAsync(0).Result;

            Assert.False(contains);
        }
    }
}
