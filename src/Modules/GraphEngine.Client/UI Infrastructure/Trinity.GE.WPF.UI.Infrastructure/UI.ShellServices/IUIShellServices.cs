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
using System.Threading.Tasks;
using System.Windows;
using Prism.Regions;

namespace Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices
{
    public enum ContropolusShellType
    {
        MainShell,
        ChildShell,
        UIRespositoryShell,
        MasterHiddenShell,
        ChildHiddenShell
    }
    public interface IUIShellService : IContropolusScopedRegionManager
    {
        Window ShellWindow { get; set; }
        void ShowShell(string uri);
        IRegionManager ShowShell<TViewModel, TView>(Type theUIViewModel, Type theUIView, string theUIViewName);
        void UnloadRegion(string theRegionName, Type theUIView, string theUIViewName);
        void SetupandLoadViewViewModelInRegion<TViewModel, TView>(Type theUIViewModel, Type theUIView, string theUIViewName);
        bool TeardownRegionSetup();

        Task<(int ReturnCode, int ReasonCode)> RegisterShellAsync<TShellType>(ContropolusShellType theShellRegistrationType, 
                                                                              TShellType theShellViewType, 
                                                                              DependencyObject theShellInstance);
        Task<(int ReturnCode, int ReasonCode)> RegisterShellAsync<TShellType>(ContropolusShellType theShellRegistrationType,
                                                                              Type theShellViewType,
                                                                              DependencyObject theShellInstance);

        Task<(DependencyObject ShellWindowInstance, Type ShellType)> RegisterShellInstanceAsync(ContropolusShellType theShellRegistrationType,
                                                                                                DependencyObject theShellInstance,
                                                                                                Type theShellInstanceType);

        Task<(DependencyObject ShellWindowInstance, Type ShellType)> RegisterShellInstanceAsync<TShellViewType, TShellViewModelType>(ContropolusShellType theShellRegistrationType,
                                                                                                                                     DependencyObject theShellInstance,
                                                                                                                                     Type theShellInstanceType);
    }
}