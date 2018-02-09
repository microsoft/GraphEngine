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
using System.Reactive.Linq;
using System.Windows;
using Castle.Windsor;
using Prism.Events;
using Prism.Interactivity.InteractionRequest;
using Prism.Regions;
using Reactive.Bindings;
using Trinity.GE.WPF.UI.Infrastructure.Base.ViewModel;
using Trinity.GE.WPF.UI.Infrastructure.Prism.Interfaces;
using Trinity.GE.WPF.UI.Infrastructure.UI.RegionNames;
using Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices;

namespace Trinity.GE.WPF.TestFacility.UIShell.Module.UI.ViewModels
{
    public class MainShellWindowViewModel : ViewModelBase, IRegionManagerAware, INavigationAware, IRegionMemberLifetime
    {
        private readonly IUIShellService _shellService;

        public ReactiveProperty<string> ProgramInfoTitle { get; private set; } // tow way

        public ReactiveProperty<string> Input { get; private set; }

        public ReactiveCommand AlertCommand { get; private set; }

        public ReactiveProperty<string> Output { get; private set; }

        public ReactiveProperty<int> ReturnCode { get; set; }

        public InteractionRequest<IConfirmation> ConfirmRequest { get; private set; }
        public ReactiveProperty<EventArgs> MainShellLoaded { get; private set; }
        public ReactiveProperty<RoutedEventArgs> ShellLoaded { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prismRegionManager"></param>
        /// <param name="windsorContainer"></param>
        /// <param name="prismEventAggregator"></param>
        /// <param name="shellService"></param>
        public MainShellWindowViewModel(IRegionManager    prismRegionManager, 
                                        IWindsorContainer windsorContainer, 
                                        IEventAggregator  prismEventAggregator,
                                        IUIShellService   shellService) : base(prismRegionManager, windsorContainer, prismEventAggregator)
        {
            // mode is Flags. (default is all)
            // DistinctUntilChanged is no push value if next value is same as current
            // RaiseLatestValueOnSubscribe is push value when subscribed
            const ReactivePropertyMode allMode = ReactivePropertyMode.DistinctUntilChanged |
                                                 ReactivePropertyMode.RaiseLatestValueOnSubscribe;

            const ReactivePropertyMode modeIsNone = ReactivePropertyMode.DistinctUntilChanged;

            this.ProgramInfoTitle =
                new ReactiveProperty<string>($"InKnowWorks Contropolus WPF Experimental Parallel-Multi-Shell (PMS) - Testing Contropolus Concepts");

            // Let's get the ProgramInformation Data Loaded

            MainShellLoaded = new ReactiveProperty<EventArgs>(mode: ReactivePropertyMode.None);
            ShellLoaded     = new ReactiveProperty<RoutedEventArgs>(mode: ReactivePropertyMode.None);

            this.ReturnCode = MainShellLoaded.Select((e, r) =>
            {
                _shellService.ShowShell(UIRegionNames.MainMenuRegion);

                return 0;
            }).ToReactiveProperty();

            this.ReturnCode = ShellLoaded.Select((e, r) =>
            {
                _shellService.ShowShell(UIRegionNames.MainMenuRegion);

                return 0;
            }).ToReactiveProperty();

            _shellService = shellService;

            //this.ConfirmRequest = new InteractionRequest<IConfirmation>();
            //this.Input = new ReactiveProperty<string>("");
            //this.Output = new ReactiveProperty<string>();

            //this.AlertCommand = new ReactiveCommand();
            //this.AlertCommand.SelectMany()
            //    // RaiseAsObservable method integrate IObservable method chain.
            //    .SelectMany(_ => this.ConfirmRequest.RaiseAsync(new Confirmation
            //    {
            //        Title = "Confirm",
            //        Content = "Convert OK?"
            //    }))
            //    .Where(c => c.Confirmed)
            //    .Select(_ => this.Input.Value)
            //    .Select(s => s.ToUpper())
            //    .Subscribe(s => this.Output.Value = s);
        }

        public IRegionManager RegionManagerAware { get; set; }
        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            throw new NotImplementedException();
        }

        public bool KeepAlive { get; }
    }
}