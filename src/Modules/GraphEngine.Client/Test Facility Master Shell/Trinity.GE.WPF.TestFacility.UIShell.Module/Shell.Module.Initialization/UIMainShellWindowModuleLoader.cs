/* --------------------------------------------------------------------------------+
 * InKnowWorks Controplus: IKW.Contropolus.VKMC.UIShell.Module                     *
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
using Prism.Modularity;
using Prism.Regions;
using Trinity.GE.WPF.TestFacility.UI.MainMenu.Module.Initialization;
using Trinity.GE.WPF.UI.Infrastructure.Base.Module;
using Trinity.GE.WPF.UI.Infrastructure.UI.ModuleNames;

namespace Trinity.GE.WPF.TestFacility.UIShell.Module.Shell.Module.Initialization
{
    public class UIMainShellWindowModuleLoader : ModuleBase
    {
        public MainShellWindow UIConsoleShellView { get; set; }
        //public MainShellWindowViewModel ShellWindowViewModel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prismRegionManager"></param>
        /// <param name="windsorContainer"></param>
        /// <param name="prismEventAggregator"></param>
        public UIMainShellWindowModuleLoader(IRegionManager prismRegionManager,
                                             IWindsorContainer windsorContainer, 
                                             IEventAggregator prismEventAggregator) : base(prismRegionManager,windsorContainer,prismEventAggregator)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Initialize()
        {
            // Okay Let's bring up the UI Modules one Region at a time.

            // XamMenu Control in the MainMenu Region

            var mainMenuControl = GlobalContainer.Resolve<MainMenuModule>(UIModuleNames.UIMainMenuModule) as IModule;
            mainMenuControl.Initialize();

            // XmDocManager in the SideDocManager Region

            //var sideDocManagerModule = GlobalContainer.Resolve<PresentationHostModule>(UIModuleNames.UISideDocManagerModule) as IModule;
            //sideDocManagerModule.Initialize();

            // XmDocManager in the SideDocManager Region

            //var workflowControlProgramModule = GlobalContainer.Resolve<WorkflowControlProgramMonitorModule>(UIModuleNames.UIWorkflowControlProgramMonitorModule) as IModule;
            //workflowControlProgramModule.Initialize();

            // XmDocManager in the SideDocManager Region

            //var workflowMonitorModule = GlobalContainer.Resolve<WorkflowMonitorModule>(UIModuleNames.UIWorkflowMonitorModule) as IModule;
            //workflowMonitorModule.Initialize();

            // XamMenu Control in the MainMenu Region

            //var programInfoModule = GlobalContainer.Resolve<ProgramInfoModule>(UIModuleNames.UIProgramInfoModule) as IModule;
            //programInfoModule.Initialize();
        }

        public override void Initialize(IRegionManager theScopedRegionManager)
        {
            throw new NotImplementedException();
        }
    }
}