using Prism.Regions;

namespace Trinity.GE.WPF.UI.Infrastructure.Interfaces
{
    public interface IScopedRegionManager : IRegionManager
    {
        IRegionManager ScopedRegionManager { get; set; }
    }
}