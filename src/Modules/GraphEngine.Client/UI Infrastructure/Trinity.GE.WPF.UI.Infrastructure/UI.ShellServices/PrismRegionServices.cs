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

using System.Linq;
using Castle.Windsor;
using Prism.Events;
using Prism.Regions;
using Trinity.GE.WPF.UI.Infrastructure.Interfaces;

namespace Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices
{
    public class PrismRegionServices : IPrismRegionManagerServices
    {
        private static IRegionManager LocalRegionManager { get; set; }
        private static IWindsorContainer WindsorContainer { get; set; }
        private static IEventAggregator EventAggregator { get; set; }

        // Tavi Truman - 07/28/2015; added Scoped Region Support

        public static bool InjectUIViewInNewShell<THostShellView>(IRegionManager theGlobalRegionManager,
                                                                  IWindsorContainer theGlobalContainer,
                                                                  IEventAggregator theGlobalEventAggregator,
                                                                  string theRegionName,
                                                                  string theViewName,
                                                                  THostShellView theHostShellView)
        {
            LocalRegionManager = theGlobalRegionManager;
            WindsorContainer   = theGlobalContainer;
            EventAggregator    = theGlobalEventAggregator;

            var shell = WindsorContainer.Resolve<THostShellView>();

            var scopedRegion = LocalRegionManager.CreateRegionManager();
            // RegionManager.SetRegionManager(shell,scopedRegion);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TUIView"></typeparam>
        /// <param name="prismRegionManager"></param>
        /// <param name="theRegionName"></param>
        /// <param name="theViewName"></param>
        /// <param name="theView"></param>
        /// <returns></returns>

        public static bool InjectUIViewIntoRegion<TUIView>(IRegionManager prismRegionManager,
                                                           string theRegionName,
                                                           string theViewName,
                                                           TUIView theView)
        {
            LocalRegionManager = prismRegionManager;

            var theRegion = LocalRegionManager.Regions[theRegionName];

            // Test to Remove the Active View and to Inject the new view

            var activeViews = LocalRegionManager.Regions[theRegionName].Views;

            var regions = LocalRegionManager.Regions;

            object activeView;

            foreach (var view in activeViews.Where(view => view.ToString().Contains(theViewName)))
            {
                activeView = theRegion.GetView(theViewName);

                if (activeView != null)
                {
                    foreach (var region in regions)
                    {
                        if (region.Views.Contains(activeView))
                        {
                            if (activeView == region.GetView(theViewName))
                            {
                                theRegion.Deactivate(activeView);
                                theRegion.Remove(activeView);
                            }
                        }
                    }
                }
            }

            // Do PRISM View Injection!

            if (!LocalRegionManager.Regions[theRegionName].Views.Contains(theViewName))
            {
                LocalRegionManager.Regions[theRegionName].Add(theView, theViewName);
            }

            theRegion.Activate(theView);

            activeView = theRegion.GetView(theViewName);

            return activeView != null;
        }

        public static bool RemoveUIViewFromRegion<TUIView>(IRegionManager prismRegionManager,
                                                           string theRegionName,
                                                           string theViewName,
                                                           TUIView theView)
        {
            LocalRegionManager = prismRegionManager;

            var theRegion = LocalRegionManager.Regions[theRegionName];

            // Test to Remove the Active View and to Inject the new view

            var activeViews = LocalRegionManager.Regions[theRegionName].Views;

            var regions = LocalRegionManager.Regions;

            object activeView;

            foreach (var view in activeViews.Where(view => view.ToString().Contains(theViewName)))
            {
                activeView = theRegion.GetView(theViewName);

                if (activeView != null)
                {
                    foreach (var region in regions)
                    {
                        if (region.Views.Contains(activeView))
                        {
                            if (activeView == region.GetView(theViewName))
                            {
                                theRegion.Deactivate(activeView);
                                theRegion.Remove(activeView);
                            }
                        }
                    }
                }
            }

            activeView = theRegion.GetView(theViewName);

            return activeView != null;
        }
    }
}