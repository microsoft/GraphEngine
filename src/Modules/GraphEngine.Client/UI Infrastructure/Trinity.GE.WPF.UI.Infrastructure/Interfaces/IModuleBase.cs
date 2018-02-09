using Prism.Modularity;
using Prism.Regions;

namespace Trinity.GE.WPF.UI.Infrastructure.Interfaces
{
    public interface IModuleBase : IModule
    {
        void Initialize(IRegionManager theScopedRegionManager);
    }
}