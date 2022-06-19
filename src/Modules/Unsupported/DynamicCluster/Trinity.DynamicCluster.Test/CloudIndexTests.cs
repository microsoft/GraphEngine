using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.DynamicCluster.Consensus;
using Trinity.DynamicCluster.Storage;

namespace Trinity.DynamicCluster.Test
{
    [TestClass]
    public class CloudIndexTests
    {
        [TestMethod]
        public async Task CloudIndexProbesNameservice()
        {
            CancellationTokenSource tsrc = new CancellationTokenSource();
            var namesvc = Mock.Of<INameService>();
            var ctable  = Mock.Of<IChunkTable>();

            Guid id = Guid.NewGuid();

            Mock.Get(namesvc).Setup(svc => svc.IsMaster).Returns(true);
            Mock.Get(namesvc).Setup(svc => svc.PartitionCount).Returns(1);
            Mock.Get(namesvc).Setup(svc => svc.InstanceId).Returns(id);
            Mock.Get(namesvc).Setup(svc => svc.ResolvePartition(0))
                .ReturnsAsync(new List<ReplicaInformation> { new ReplicaInformation("localhost", 9999, id, 0) });

            CloudIndex ci = new CloudIndex(tsrc.Token, namesvc, ctable, null, "myname", _ => null);
            await Task.Delay(1000);
            Mock.Get(namesvc).Verify(_ => _.PartitionCount, Times.AtLeastOnce);
            Mock.Get(namesvc).Verify(_ => _.ResolvePartition(It.IsAny<int>()), Times.AtLeastOnce);
        }
    }
}
