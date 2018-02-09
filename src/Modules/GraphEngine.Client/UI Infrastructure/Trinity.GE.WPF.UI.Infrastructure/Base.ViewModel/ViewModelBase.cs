/* --------------------------------------------------------------------------------+
 * InKnowWorks Controplus: IKW.Contropolus.WPF.UI.Infrastructure                   *
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
using Prism;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Trinity.GE.WPF.UI.Infrastructure.Interfaces;
using Trinity.GE.WPF.UI.Infrastructure.Prism.Interfaces;

namespace Trinity.GE.WPF.UI.Infrastructure.Base.ViewModel
{
    public class ViewModelBase : BindableBase, IActiveAware, IViewModel, IDockAware
    {
        private IRegionManager PrismRegionManager { get; set; }
        private IWindsorContainer WindsorContainer { get; set; }
        private IEventAggregator PrismEventAggregator { get; set; }
        public ViewModelBase(IRegionManager prismRegionManager,
                             IWindsorContainer windsorContainer, 
                             IEventAggregator prismEventAggregator)
        {
            if (prismRegionManager != null) PrismRegionManager     = prismRegionManager;
            if (windsorContainer != null) WindsorContainer         = windsorContainer;
            if (prismEventAggregator != null) PrismEventAggregator = prismEventAggregator;
        }

        public IRegionManager GlobalRegionManager
        {
            get
            {
                if (PrismRegionManager != null)
                    return PrismRegionManager;
                return null;
            }
        }

        public IWindsorContainer GlobalContainer
        {
            get
            {
                if (WindsorContainer != null)
                    return WindsorContainer;
                return null;
            }
        }

        public IEventAggregator GlobalEventAggregator
        {
            get
            {
                if (PrismEventAggregator != null)
                    return PrismEventAggregator;
                return null;
            }
        }

        public bool IsActive { get; set; }
        public event EventHandler IsActiveChanged;
        public string Header { get; set; }
    }
}