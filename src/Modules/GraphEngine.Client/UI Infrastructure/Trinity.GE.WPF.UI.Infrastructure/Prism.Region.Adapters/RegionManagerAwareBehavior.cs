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
using System.Collections.Specialized;
using System.Windows;
using Prism.Regions;
using Trinity.GE.WPF.UI.Infrastructure.Prism.Interfaces;

namespace Trinity.GE.WPF.UI.Infrastructure.Prism.Region.Adapters
{
    public class RegionManagerAwareBehavior : RegionBehavior
    {
        public const string BehaviorKey = "RegionManagerAwareBehavior";

        /// <summary>
        /// 
        /// </summary>
        protected override void OnAttach()
        {
            Region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        void ActiveViews_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var item in e.NewItems)
                {
                    IRegionManager regionManager = Region.RegionManager;

                    FrameworkElement element = item as FrameworkElement;
                    IRegionManager scopedRegionManager =
                        element?.GetValue(RegionManager.RegionManagerProperty) as IRegionManager;

                    if (scopedRegionManager != null)
                        regionManager = scopedRegionManager;

                    InvokeOnRegionManagerAwareElement(item, x => x.RegionManagerAware = regionManager);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var item in e.OldItems)
                {
                    InvokeOnRegionManagerAwareElement(item, x => x.RegionManagerAware = null);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        /// <param name="invocation"></param>

        static void InvokeOnRegionManagerAwareElement(object item, Action<IRegionManagerAware> invocation)
        {
            var rmAwareItem = item as IRegionManagerAware;
            if (rmAwareItem != null)
                invocation(rmAwareItem);

            var frameworkElement = item as FrameworkElement;
            IRegionManagerAware rmAwareDataContext = frameworkElement?.DataContext as IRegionManagerAware;

            if (rmAwareDataContext != null)
            {
                var frameworkElementParent   = frameworkElement.Parent as FrameworkElement;
                var rmAwareDataContextParent = frameworkElementParent?.DataContext as IRegionManagerAware;

                if (rmAwareDataContextParent != null)
                {
                    if (rmAwareDataContext == rmAwareDataContextParent)
                    {
                        return;
                    }
                }

                invocation(rmAwareDataContext);
            }
        }
    }
}
