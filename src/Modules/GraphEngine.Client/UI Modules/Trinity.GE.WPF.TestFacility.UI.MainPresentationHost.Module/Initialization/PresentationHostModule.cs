/* --------------------------------------------------------------------------------+
 * InKnowWorks Controplus: IKW.Contropolus.VKMC.UI.MainPresentationHost.Module     *
 * Designed and Written by Tavi Truman                                             *
 * Version 1.0.0                                                                   *
 * InKnowWorks, Corp. proprietary/confidential. Use is subject to license terms.   *
 * Redistribution of this file for of an unauthorized byte-code version            *
 * of this file is strictly forbidden.                                             *
 * Copyright (c) 2009-2015 by InKnowWorks, Corp.                                   *
 * 2143 Willester Aave, San Jose, CA 95124. All rights reserved.                   *
 * --------------------------------------------------------------------------------*/

using Castle.Windsor;
using IKW.Contropolus.WPF.UI.Infrastructure.Base.Module;
using IKW.Contropolus.WPF.UI.Infrastructure.UI.RegionNames;
using IKW.Contropolus.WPF.UI.Infrastructure.UI.ShellServices;
using IKW.Contropolus.WPF.UI.Infrastructure.UI.ViewNames;
using Prism.Events;
using Prism.Regions;
using Trinity.GE.WPF.TestFacility.UI.MainPresentationHost.Module.ViewModels;
using Trinity.GE.WPF.TestFacility.UI.MainPresentationHost.Module.Views;

namespace IKW.Contropolus.VKMC.UI.MainPresentationHost.Module.Initialization
{
    public class PresentationHostModule : ModuleBase
    {
        private IUIShellService TheShellService { get; set; }
        private PresentationHostViewModel UIViewModel { get; set; }
        private PresentationHostView UIViewInstance { get; set; }

        public PresentationHostModule(IRegionManager prismRegionManager,
                                      IWindsorContainer windsorContainer, 
                                      IEventAggregator prismEventAggregator,
                                      IUIShellService theShellService) : base(prismRegionManager, windsorContainer, prismEventAggregator)
        {
            TheShellService = theShellService;
        }

        /// <summary>
        /// 
        /// </summary>

        public override void Initialize()
        {
            // Step 1: New-up Singletons 

            UIViewInstance = GlobalContainer.Resolve<PresentationHostView>(UIViewNames.UIPresentationHost);

            // Okay this is a UIViewInstance-first execution model so let's fire-up the ViewModel

            UIViewModel = GlobalContainer.Resolve<PresentationHostViewModel>();

            UIViewModel.ActiveView = UIViewInstance;

            UIViewInstance.ApplyViewModel(UIViewModel);

            // Do the UIViewInstance Injection!

            PrismRegionServices.InjectUIViewIntoRegion<PresentationHostView>(
                GlobalRegionManager,
                UIRegionNames.MainPresentationHostRegion,
                UIViewNames.UIPresentationHost,
                UIViewInstance);

            // Go ahead and hookup the Data
            UIViewInstance.MainPresentationDocManager.DataContext = UIViewModel;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="theRegionManager"></param>

        public override void Initialize(IRegionManager theRegionManager)
        {
            // Step 1: New-up Singletons 

            UIViewInstance = GlobalContainer.Resolve<PresentationHostView>(UIViewNames.UIPresentationHost);

            // Okay this is a UIViewInstance-first execution model so let's fire-up the ViewModel

            UIViewModel = GlobalContainer.Resolve<PresentationHostViewModel>();

            UIViewModel.ActiveView = UIViewInstance;

            UIViewInstance.ApplyViewModel(UIViewModel);

            // Do the UIViewInstance Injection!

            PrismRegionServices.InjectUIViewIntoRegion<PresentationHostView>(
                theRegionManager,
                UIRegionNames.MainPresentationHostRegion,
                UIViewNames.UIPresentationHost,
                UIViewInstance);

            // Go ahead and hookup the Data
            UIViewInstance.MainPresentationDocManager.DataContext = UIViewModel;
        }
    }
}
