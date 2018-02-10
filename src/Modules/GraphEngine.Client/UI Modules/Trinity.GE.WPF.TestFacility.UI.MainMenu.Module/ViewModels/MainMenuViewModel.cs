/* --------------------------------------------------------------------------------+
 * InKnowWorks Controplus: IKW.Contropolus.VKMC.UI.MainMenu.Module                 *
 * Designed and Written by Tavi Truman                                             *
 * Version 1.0.0                                                                   *
 * InKnowWorks, Corp. proprietary/confidential. Use is subject to license terms.   *
 * Redistribution of this file for of an unauthorized byte-code version            *
 * of this file is strictly forbidden.                                             *
 * Copyright (c) 2009-2015 by InKnowWorks, Corp.                                   *
 * 2143 Willester Aave, San Jose, CA 95124. All rights reserved.                   *
 * --------------------------------------------------------------------------------*/

using System;
using System.Reactive.Linq;
using Castle.Windsor;
using IKW.Contropolus.VKMC.UI.MainMenu.Module.Views;
using Prism.Events;
using Prism.Regions;
using Reactive.Bindings;
using Trinity.GE.WPF.TestFacility.UI.MainPresentationHost.Module.Initialization;
using Trinity.GE.WPF.UI.Infrastructure.Base.Module;
using Trinity.GE.WPF.UI.Infrastructure.Base.ViewModel;
using Trinity.GE.WPF.UI.Infrastructure.Prism.Interfaces;
using Trinity.GE.WPF.UI.Infrastructure.UI.ModuleNames;
using Trinity.GE.WPF.UI.Infrastructure.UI.RegionNames;
using Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices;
using Trinity.GE.WPF.UI.Infrastructure.UI.ViewNames;
using MainMenuView = Trinity.GE.WPF.TestFacility.UI.MainMenu.Module.Views.MainMenuView;

namespace Trinity.GE.WPF.TestFacility.UI.MainMenu.Module.ViewModels
{
    public class MainMenuViewModel : ViewModelBase, IRegionManagerAware
    {
        private IUIShellService GlobalShellService { get; }
        private IRegionManager  ScopedRegionManager { get; set; }
        public ReactiveProperty<EventArgs> OpenAboutShellClickEvent { get; private set; }
        public ReactiveProperty<EventArgs> CloseAboutShellClickEvent { get; private set; }
        public ReactiveProperty<EventArgs> CloseAllAboutShellClickEvent { get; private set; }
        public ReactiveProperty<EventArgs> OpenWorkflowControlClickEvent { get; private set; }
        public ReactiveProperty<EventArgs> CloseWorkflowControlClickEvent { get; private set; }
        public ReactiveProperty<EventArgs> OpenWorkflowDisplayMonitorEvent { get; private set; }
        public ReactiveProperty<EventArgs> CloseWorkflowDisplayMonitorClickEvent { get; private set; }
        // ReloadAboutShellMenuControlClickEvent
        public ReactiveProperty<EventArgs> ReloadAboutShellMenuControlClickEvent { get; private set; }
        // Added Support for Trinity Graph Engine Test Facilty GE-Native Server
        public ReactiveProperty<EventArgs> OpenGEServerControlShellClickEvent { get; private set; }
        public ReactiveProperty<EventArgs> CloseGEServerControlShellClickEvent { get; private set; }

        private ReactiveProperty<int> ReturnCode { get; set; }

        private const ReactivePropertyMode AllMode = ReactivePropertyMode.None;
        public MainMenuView ActiveView { get; set; }

        public MainMenuViewModel(IRegionManager prismRegionManager, 
                                 IWindsorContainer windsorContainer, 
                                 IEventAggregator prismEventAggregator,
                                 IUIShellService theGlobalShellService) : base(prismRegionManager, windsorContainer, prismEventAggregator)
        {
            GlobalShellService = theGlobalShellService;

            IntializeReactiveProperties();

            LoadReactivePropertyCodeSegments();
        }

        /// <summary>
        /// 
        /// </summary>

        private void LoadReactivePropertyCodeSegments()
        {
            ReturnCode = OpenAboutShellClickEvent.Select((theSender, theEventArgs) =>
            {
                ScopedRegionManager =
                    GlobalShellService.ShowShell<MainMenuViewModel, MainMenuView>(typeof(MainMenuViewModel),
                        typeof(MainMenuView), UIViewNames.UIMainMenu);

                var presentationHostModule =
                    GlobalContainer.Resolve<PresentationHostModule>(UIModuleNames.UIPresentationHostModule) as ModuleBase;

                presentationHostModule?.Initialize(ScopedRegionManager);

                var programInfoModule =
                    GlobalContainer.Resolve<ProgramInfoModule>(UIModuleNames.UIProgramInfoModule) as ModuleBase;

                programInfoModule?.Initialize(ScopedRegionManager);

                return 0;

            }).ToReactiveProperty();

            ReturnCode = CloseAboutShellClickEvent.Select((theSender, theEventArgs) =>
            {
                GlobalShellService.UnloadRegion(UIRegionNames.MainPresentationHostRegion, typeof(MainMenuView), UIViewNames.UIPresentationHost);

                return 0;

            }).ToReactiveProperty();

        }

        /// <summary>
        /// 
        /// </summary>
        private void IntializeReactiveProperties()
        {
            OpenAboutShellClickEvent              = new ReactiveProperty<EventArgs>(mode: AllMode);
            CloseAboutShellClickEvent             = new ReactiveProperty<EventArgs>(mode: AllMode);
            CloseAllAboutShellClickEvent          = new ReactiveProperty<EventArgs>(mode: AllMode);
            OpenWorkflowControlClickEvent         = new ReactiveProperty<EventArgs>(mode: AllMode);
            CloseWorkflowControlClickEvent        = new ReactiveProperty<EventArgs>(mode: AllMode);
            OpenWorkflowDisplayMonitorEvent       = new ReactiveProperty<EventArgs>(mode: AllMode);
            CloseWorkflowDisplayMonitorClickEvent = new ReactiveProperty<EventArgs>(mode: AllMode);
            ReloadAboutShellMenuControlClickEvent = new ReactiveProperty<EventArgs>(mode: AllMode);
            OpenGEServerControlShellClickEvent    = new ReactiveProperty<EventArgs>(mode: AllMode);
            CloseGEServerControlShellClickEvent   = new ReactiveProperty<EventArgs>(mode: AllMode);
        }

        public IRegionManager RegionManagerAware { get; set; }
    }
}