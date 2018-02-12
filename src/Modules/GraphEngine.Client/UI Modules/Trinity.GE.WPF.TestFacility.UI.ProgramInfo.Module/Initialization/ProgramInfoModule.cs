/* --------------------------------------------------------------------------------+
 * InKnowWorks Controplus: IKW.Contropolus.VKMC.UI.ProgramInfo.Module              *
 * Designed and Written by Tavi Truman                                             *
 * Version 1.0.0                                                                   *
 * InKnowWorks, Corp. proprietary/confidential. Use is subject to license terms.   *
 * Redistribution of this file for of an unauthorized byte-code version            *
 * of this file is strictly forbidden.                                             *
 * Copyright (c) 2009-2015 by InKnowWorks, Corp.                                   *
 * 2143 Willester Aave, San Jose, CA 95124. All rights reserved.                   *
 * --------------------------------------------------------------------------------*/

using Castle.Windsor;
using Prism.Events;
using Prism.Regions;
using Trinity.GE.WPF.TestFacility.UI.ProgramInfo.Module.ViewModels;
using Trinity.GE.WPF.TestFacility.UI.ProgramInfo.Module.Views;
using Trinity.GE.WPF.UI.Infrastructure.Base.Module;
using Trinity.GE.WPF.UI.Infrastructure.Prism.Interfaces;
using Trinity.GE.WPF.UI.Infrastructure.UI.DocAware.ViewModelNames;
using Trinity.GE.WPF.UI.Infrastructure.UI.RegionNames;
using Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices;
using Trinity.GE.WPF.UI.Infrastructure.UI.ViewNames;

namespace Trinity.GE.WPF.TestFacility.UI.ProgramInfo.Module.Initialization
{
    public class ProgramInfoModule : ModuleBase
    {
        public IUIShellService TheUIShellService { get; set; }
        private ProgramInfoViewModel UIViewModel { get; set; }
        private ProgramInfoView UIViewInstance { get; set; }

        public ProgramInfoModule(IRegionManager prismRegionManager,
                                 IWindsorContainer windsorContainer,
                                 IEventAggregator prismEventAggregator,
                                 IUIShellService theUIShellService) : base(prismRegionManager, windsorContainer, prismEventAggregator)
        {
            TheUIShellService = theUIShellService;
        }

        /// <summary>
        /// Standard Controplous UI Module Initialization 
        /// </summary>

        public override void Initialize()
        {
            // Step 1: New-up Singletons 

            UIViewInstance = GlobalContainer.Resolve<ProgramInfoView>(UIViewNames.UIProgramInfo);

            // Okay this is a UIViewInstance-first execution model so let's fire-up the ViewModel

            UIViewModel = GlobalContainer.Resolve<ProgramInfoViewModel>();

            UIViewModel.ActiveView = UIViewInstance;

            UIViewInstance.ApplyViewModel(UIViewModel);

            //UIViewInstance.UIProgramInfoData.FieldLayoutInitialized += UIViewModel._xamDataCards_FieldLayoutInitialized;

            // Do the UIViewInstance Injection!

            PrismRegionServices.InjectUIViewIntoRegion(GlobalRegionManager,
                UIRegionNames.ProgramAboutInfoRegion,
                UIViewNames.UIProgramInfo,
                UIViewInstance);

            // Go ahead and hookup the Data
            UIViewInstance.UIProgramInfoData.DataSource = UIViewModel.ProgramInfoDataDataView;
        }

        /// <summary>
        /// Extended Controplous UI Module Initialization where the Scopped Region Manager is passed in.
        /// </summary>
        /// <param name="theScopedRegionManager"></param>

        public override void Initialize(IRegionManager theScopedRegionManager)
        {
            IDockAware presentationHostDockAware =
                GlobalContainer.Resolve<IDockAware>(UIDocAwareViewModelNames.UIPresentationHostDocAwareVM);

            // Step 1: New-up Singletons 

            UIViewInstance = GlobalContainer.Resolve<ProgramInfoView>(UIViewNames.UIProgramInfo);

            // Okay this is a UIViewInstance-first execution model so let's fire-up the ViewModel

            UIViewModel = GlobalContainer.Resolve<ProgramInfoViewModel>();

            presentationHostDockAware.Header = UIViewModel.Header;

            UIViewModel.ActiveView = UIViewInstance;

            UIViewInstance.ApplyViewModel(UIViewModel);

            //UIViewInstance.UIProgramInfoData.FieldLayoutInitialized += UIViewModel._xamDataCards_FieldLayoutInitialized;

            // Do the UIViewInstance Injection!

            PrismRegionServices.InjectUIViewIntoRegion(
                theScopedRegionManager,
                UIRegionNames.ProgramAboutInfoRegion,
                UIViewNames.UIProgramInfo,
                UIViewInstance);

            // Go ahead and hookup the Data
            UIViewInstance.UIProgramInfoData.DataSource = UIViewModel.ProgramInfoDataDataView;
        }
    }
}
