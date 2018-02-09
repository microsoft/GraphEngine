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

using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Prism.Events;
using Prism.Regions;
using Reactive.Bindings;
using Trinity.GE.WPF.TestFacility.UI.MainPresentationHost.Module.Views;
using Trinity.GE.WPF.UI.Infrastructure.Base.ViewModel;
using Trinity.GE.WPF.UI.Infrastructure.Prism.Interfaces;
using Trinity.GE.WPF.UI.Infrastructure.UI.DocAware.ViewModelNames;
using Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices;

namespace Trinity.GE.WPF.TestFacility.UI.MainPresentationHost.Module.ViewModels
{
    public class PresentationHostViewModel : ViewModelBase, IRegionManagerAware
    {
        public IUIShellService TheGlobalShellService { get; set; }
        private ReactiveProperty<int> ReturnCode { get; set; }

        private const ReactivePropertyMode AllMode = ReactivePropertyMode.None;
        public PresentationHostView ActiveView { get; set; }

        public IRegionManager RegionManagerAware { get; set; }

        public PresentationHostViewModel(IRegionManager    prismRegionManager,
                                         IWindsorContainer windsorContainer,
                                         IEventAggregator  prismEventAggregator,
                                         IUIShellService   theGlobalShellService) : base(prismRegionManager, windsorContainer, prismEventAggregator)
        {
            TheGlobalShellService = theGlobalShellService;

            var viewModelIsAlreadyRegistered =
                GlobalContainer.Kernel.HasComponent(UIDocAwareViewModelNames.UIPresentationHostDocAwareVM);

            if (!viewModelIsAlreadyRegistered)
            {
                GlobalContainer.Register(
                    Component.For<IDockAware>()
                        .Instance(this)
                        .Named(UIDocAwareViewModelNames.UIPresentationHostDocAwareVM)
                        .LifeStyle.Singleton);
            }

            Header = "Not Loaded Yet";
        }
    }
}