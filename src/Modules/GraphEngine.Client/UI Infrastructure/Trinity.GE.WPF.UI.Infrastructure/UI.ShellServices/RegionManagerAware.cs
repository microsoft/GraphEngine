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

using System.Windows;
using Prism.Regions;
using Trinity.GE.WPF.UI.Infrastructure.Prism.Interfaces;

namespace Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices
{
    public static class RegionManagerAware
    {
        public static void SetRegionManagerAware(object item, IRegionManager regionManager)
        {
            if (item is IRegionManagerAware rmAware)
                rmAware.RegionManagerAware = regionManager;

            var rmAwareFrameworkElement = item as FrameworkElement;
            if (rmAwareFrameworkElement?.DataContext is IRegionManagerAware rmAwareDataContext)
                rmAwareDataContext.RegionManagerAware = regionManager;
        }
    }
}