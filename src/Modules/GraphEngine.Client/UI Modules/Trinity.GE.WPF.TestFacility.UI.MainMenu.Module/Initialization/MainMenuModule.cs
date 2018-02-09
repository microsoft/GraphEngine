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
using Castle.Windsor;
using IKW.Contropolus.VKMC.UI.MainMenu.Module.Views;
using Prism.Events;
using Prism.Regions;
using Trinity.GE.WPF.TestFacility.UI.MainMenu.Module.ViewModels;
using Trinity.GE.WPF.UI.Infrastructure.Base.Module;
using Trinity.GE.WPF.UI.Infrastructure.UI.RegionNames;
using Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices;
using Trinity.GE.WPF.UI.Infrastructure.UI.ViewNames;
using MainMenuView = Trinity.GE.WPF.TestFacility.UI.MainMenu.Module.Views.MainMenuView;

namespace Trinity.GE.WPF.TestFacility.UI.MainMenu.Module.Initialization
{
    public class MainMenuModule : ModuleBase
    {
        private MainMenuViewModel UIViewModel { get; set; }
        private MainMenuView UIViewInstance { get; set; }

        public MainMenuModule(IRegionManager prismRegionManager, 
                              IWindsorContainer windsorContainer, 
                              IEventAggregator prismEventAggregator) : base(prismRegionManager, windsorContainer, prismEventAggregator)
        {
        }

        public override void Initialize()
        {
            // Step 1: New-up Singletons 

            UIViewInstance = GlobalContainer.Resolve<MainMenuView>(UIViewNames.UIMainMenu);

            // Okay this is a UIViewInstance-first execution model so let's fire-up the ViewModel

            UIViewModel = GlobalContainer.Resolve<MainMenuViewModel>();

            UIViewModel.ActiveView = UIViewInstance;

            UIViewInstance.ApplyViewModel(UIViewModel);

            // Do the UIViewInstance Injection!

            PrismRegionServices.InjectUIViewIntoRegion<MainMenuView>(
                GlobalRegionManager,
                UIRegionNames.MainMenuControlRegion,
                UIViewNames.UIMainMenu,
                UIViewInstance);

            // Go ahead and hookup the Data
            UIViewInstance.MainMenuControl.DataContext = UIViewModel;
        }

        public override void Initialize(IRegionManager theScopedRegionManager)
        {
            throw new NotImplementedException();
        }
    }
}
