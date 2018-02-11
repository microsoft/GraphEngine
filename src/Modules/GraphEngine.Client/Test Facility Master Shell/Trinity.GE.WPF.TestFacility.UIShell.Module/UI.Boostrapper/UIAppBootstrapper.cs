/* --------------------------------------------------------------------------------+
 * InKnowWorks Controplus: IKW.Contropolus.VKMC.UIShell.Module                     *
 * Designed and Written by Tavi Truman                                             *
 * Version 1.0.0                                                                   *
 * InKnowWorks, Corp. proprietary/confidential. Use is subject to license terms.   *
 * Redistribution of this file for of an unauthorized byte-code version            *
 * of this file is strictly forbidden.                                             *
 * Copyright (c) 2009-2015 by InKnowWorks, Corp.                                   *
 * 2143 Willester Aave, San Jose, CA 95124. All rights reserved.                   *
 * --------------------------------------------------------------------------------*/

using System.Windows;
using Castle.Core;
using Infragistics.Windows.DockManager;
using Prism.Modularity;
using Prism.Regions;
using PrismContrib.WindsorExtensions;
using Trinity.GE.WPF.TestFacility.UI.MainMenu.Module.Initialization;
using Trinity.GE.WPF.TestFacility.UI.MainMenu.Module.ViewModels;
using Trinity.GE.WPF.TestFacility.UI.MainMenu.Module.Views;
using Trinity.GE.WPF.TestFacility.UI.MainPresentationHost.Module.Initialization;
using Trinity.GE.WPF.TestFacility.UI.MainPresentationHost.Module.ViewModels;
using Trinity.GE.WPF.TestFacility.UI.MainPresentationHost.Module.Views;
using Trinity.GE.WPF.TestFacility.UIShell.Module.Shell.Module.Initialization;
using Trinity.GE.WPF.TestFacility.UIShell.Module.Shell.Services;
using Trinity.GE.WPF.TestFacility.UIShell.Module.UI.ViewModels;
using Trinity.GE.WPF.UI.Infrastructure.Prism.Region.Adapters;
using Trinity.GE.WPF.UI.Infrastructure.PubSub.EventNames;
using Trinity.GE.WPF.UI.Infrastructure.PubSub.Events;
using Trinity.GE.WPF.UI.Infrastructure.UI.ModuleNames;
using Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices;
using Trinity.GE.WPF.UI.Infrastructure.UI.ViewModelNames;
using Trinity.GE.WPF.UI.Infrastructure.UI.ViewNames;
using UI.CastleWindsorAdapter.ServiceLocator.Extensions;
using Component = Castle.MicroKernel.Registration.Component;
using MainShellWindow = Trinity.GE.WPF.TestFacility.UIShell.Module.Shell.MainShellWindow;
using RegionManagerAware = Trinity.GE.WPF.UI.Infrastructure.Prism.Region.Adapters.RegionManagerAware;
using RegionManagerAwareBehavior = Trinity.GE.WPF.UI.Infrastructure.Prism.Region.Adapters.RegionManagerAwareBehavior;

namespace Trinity.GE.WPF.TestFacility.UIShell.Module.UI.Boostrapper
{
    public class UIAppBootstrapper : WindsorBootstrapper
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override DependencyObject CreateShell()
        {
            return Container.Resolve<MainShellWindow>(UIViewNames.UIMainShellWindow);
        }

        /// <summary>
        /// 
        /// </summary>

        protected override void InitializeShell()
        {
            var regionManager = RegionManager.GetRegionManager((Shell));
            RegionManagerAware.SetRegionManagerAware(Shell, regionManager);

            base.InitializeShell();

            Application.Current.MainWindow = (Window) Shell;
            Application.Current.MainWindow?.Show();
        }

        /// <summary>
        /// 
        /// </summary>

        protected override void ConfigureContainer()
        {
            base.ConfigureContainer();

            // New Support for Multiple PRISM Shells

            // Make a signleton

            Container.Register(Component.For<SemanticEvent>()
                .ImplementedBy<SemanticEvent>()
                .Named(PubSubEventNames.SemanticEvent)
                .LifeStyle.Transient);

            Container.Register(Component.For<IUIShellService>()
                .ImplementedBy<ShellService>()
                .Named(@"Contropolus.MainRegion")
                .LifeStyle.Singleton);

            // Now start loading up the WPF User Controls

            Container.Register(Component.For<MainShellWindow>()
                .ImplementedBy<MainShellWindow>()
                .Named(UIViewNames.UIMainShellWindow)
                .LifeStyle.Transient);

            Container.RegisterType<MainMenuView, MainMenuView>(UIViewNames.UIMainMenu, LifestyleType.Transient);
            Container.RegisterType<PresentationHostView, PresentationHostView>(UIViewNames.UIPresentationHost, LifestyleType.Transient);
            Container.RegisterType<ProgramInfoView, ProgramInfoView>(UIViewNames.UIProgramInfo, LifestyleType.Transient);

            Container.Register(Component.For<MainShellWindowViewModel>()
                .ImplementedBy<MainShellWindowViewModel>()
                .Named(UIViewModelNames.UIMainShellWindowVM)
                .LifeStyle.Transient);

            Container.RegisterType<MainMenuViewModel, MainMenuViewModel>(UIViewModelNames.UIMainMenuVM, LifestyleType.Transient);
            Container.RegisterType<PresentationHostViewModel, PresentationHostViewModel>(UIViewModelNames.UIPresentationHostVM, LifestyleType.Transient);
            Container.RegisterType<ProgramInfoViewModel, ProgramInfoViewModel>(UIViewModelNames.UIProgramInfoVM, LifestyleType.Transient);
        }

        /// <summary>
        /// 
        /// </summary>

        protected override void InitializeModules()
        {
            // Let's get the modules ready for use

            // Register the main WPF UI Main Shell Windows

            Container.RegisterType<UIMainShellWindowModuleLoader, UIMainShellWindowModuleLoader>(UIModuleNames.UIConsoleShellModule,
                LifestyleType.Transient);

            // Register the Main Menu Module

            Container.RegisterType<MainMenuModule, MainMenuModule>(UIModuleNames.UIMainMenuModule, LifestyleType.Transient);

            // Register the Main Menu Module

            Container.RegisterType<PresentationHostModule, PresentationHostModule>(UIModuleNames.UIPresentationHostModule,
                LifestyleType.Transient);

            // Register the Main Menu Module

            Container.RegisterType<ProgramInfoModule, ProgramInfoModule>(UIModuleNames.UIProgramInfoModule, LifestyleType.Transient);

            base.InitializeModules();
        }

        protected override IModuleCatalog CreateModuleCatalog()
        {
            ModuleCatalog catalog = new ConfigurationModuleCatalog();

            // First, we start by loading the MainShellWindow

            catalog.AddModule(typeof (UIMainShellWindowModuleLoader)); // This will call the intitialization Module

            //catalog.AddModule(typeof(MainMenuModule));
            //catalog.AddModule(typeof(PresentationHostModule)).AddModule(typeof(ProgramInfoModule), InitializationMode.OnDemand);
            //catalog.AddModule(typeof(WorkflowMonitorModule))
            //catalog.AddModule(typeof(ProgramInfoModule));
            //catalog.AddModule(typeof(WorkflowControlProgramMonitorModule));

            return catalog;
        }

        /// <summary>
        /// Override of the IRegionBehaviorFactory method adding in RegionAwareBehavior; this behavior will 
        /// </summary>
        /// <returns></returns>

        protected override IRegionBehaviorFactory ConfigureDefaultRegionBehaviors()
        {
            IRegionBehaviorFactory behaviors = base.ConfigureDefaultRegionBehaviors();
            behaviors.AddIfMissing(RegionManagerAwareBehavior.BehaviorKey, typeof(RegionManagerAwareBehavior));
            return behaviors;
        }

        /// <summary>
        /// Override RegionAdapeterMappings 
        /// </summary>
        /// <returns></returns>
        protected override RegionAdapterMappings ConfigureRegionAdapterMappings()
        {
            var mappings      = base.ConfigureRegionAdapterMappings();
            var regionAdapter = Container.TryResolve<TabGroupPaneRegionAdapter>();

            if (regionAdapter == null)
                Container.Register(Component.For<TabGroupPaneRegionAdapter>()
                    .ImplementedBy<TabGroupPaneRegionAdapter>()
                    .Named(@"TabGroupPaneRegionAdapter")
                    .LifeStyle.Singleton);

            mappings.RegisterMapping(typeof(TabGroupPane), Container.Resolve<TabGroupPaneRegionAdapter>());
            return mappings;
        }

    }
}