using System;

namespace GE.ServiceFabric.Infrastructure.Module.Service.Interface
{
    public interface IGEModule : IDisposable
    {
        Guid ModuleID { get; }
        string ModuleVersion { get; }
        string ModuleName { get; }
    }
}