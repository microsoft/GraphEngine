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

using System.Collections.Specialized;
using Prism.Common;
using Prism.Regions;
using Trinity.GE.WPF.UI.Infrastructure.Interfaces;

namespace Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices
{
    public class RegionControllerAwareBehavior : RegionBehavior
    {
        private ListDictionary<string, IViewModel> _viewCache;

        public const string BehaviorKey = "RegionControllerAwareBehavior";
        protected override void OnAttach()
        {
            Region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
        }

        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
        }
    }
}