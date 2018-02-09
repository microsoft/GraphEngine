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
using System.Windows;
using Prism.Regions;

namespace Trinity.GE.WPF.UI.Infrastructure.Interfaces
{
    public interface IUIShellService : IScopedRegionManager
    {
        Window ShellWindow { get; set; }
        void ShowShell(string uri);
        IRegionManager ShowShell<TViewModel, TView>(Type theUIViewModel, Type theUIView, string theUIViewName);
        void UnloadRegion(string theRegionName, Type theUIView, string theUIViewName);
        void SetupandLoadViewViewModelInRegion<TViewModel, TView>(Type theUIViewModel, Type theUIView, string theUIViewName);
        bool TeardownRegionSetup();
    }
}