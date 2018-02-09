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
using Prism.Events;
using Prism.Regions;
using Trinity.GE.WPF.UI.Infrastructure.Interfaces;

namespace Trinity.GE.WPF.UI.Infrastructure.Base.Module
{
    public abstract class ModuleBase : IModuleBase
    {
        private IRegionManager PrismRegionManager { get; set; }
        private IWindsorContainer WindsorContainer { get; set; }
        private IEventAggregator PrismEventAggregator { get; set; }

        protected ModuleBase(IRegionManager prismRegionManager,
                             IWindsorContainer windsorContainer,
                             IEventAggregator prismEventAggregator)
        {
            PrismEventAggregator = prismEventAggregator;
            WindsorContainer = windsorContainer;
            PrismRegionManager = prismRegionManager;
        }

        protected IRegionManager GlobalRegionManager
        {
            get
            {
                if (PrismRegionManager != null)
                    return PrismRegionManager;
                else
                {
                    throw new Exception("PRISM RegionManager can't be NULL");
                }
            }
        }

        protected IWindsorContainer GlobalContainer
        {
            get
            {
                if (WindsorContainer != null)
                    return WindsorContainer;
                else
                {
                    throw new Exception("PRISM Unity Container can't be NULL");
                }
            }
        }

        protected IEventAggregator GlobalEventAggregator
        {
            get
            {
                if (PrismEventAggregator != null)
                    return PrismEventAggregator;
                else
                {
                    throw new Exception("PRISM Event Aggregator can't be NULL");
                }
            }
        }

        public abstract void Initialize();
        public abstract void Initialize(IRegionManager theScopedRegionManager);
    }
}