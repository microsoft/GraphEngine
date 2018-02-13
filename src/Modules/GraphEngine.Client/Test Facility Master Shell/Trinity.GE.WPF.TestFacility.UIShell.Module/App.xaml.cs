using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Trinity.GE.WPF.TestFacility.UIShell.Module.UI.Boostrapper;

namespace Trinity.GE.WPF.TestFacility.UIShell.Module
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            var bootstrapper = new UIAppBootstrapper();
            bootstrapper.Run();
        }
    }
}
