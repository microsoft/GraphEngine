using Prism.Regions;

namespace Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices
{
    public interface IContropolusScopedRegionManager : IRegionManager
    {
        IRegionManager ScopedRegionManager { get; set; }
    }
}