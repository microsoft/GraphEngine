using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GE.ServiceFabric.Infrastructure.Module.Service.Interface;

namespace GE.ServiceFabric.Infrastructure.Module
{
    // We interject the IGEModule in support for Dependency Injection
    public class GEServiceFabricInfrastructureModule : IGEModule
    {
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Guid ModuleID { get; private set; }
        public string ModuleVersion { get; private set; }
        public string ModuleName { get; private set; }
    }
}
