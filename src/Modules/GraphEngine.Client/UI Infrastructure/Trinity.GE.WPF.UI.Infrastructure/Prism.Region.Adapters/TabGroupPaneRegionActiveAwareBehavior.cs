/* Written by Brian Lagunas
 * Blog: http://brianlagunas.com
 * Twitter: @brianlagunas
 * Email: blagunas@infragistics.com 
 */

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
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using Infragistics.Windows.DockManager;
using Prism.Regions;
using Prism.Regions.Behaviors;

namespace Trinity.GE.WPF.UI.Infrastructure.Prism.Region.Adapters
{
    public class TabGroupPaneRegionActiveAwareBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        public const string BehaviorKey = "TabGroupPaneRegionActiveAwareBehavior";

        XamDockManager _parentDockManager;

        TabGroupPane _hostControl;
        public DependencyObject HostControl
        {
            get { return _hostControl; }
            set { _hostControl = value as TabGroupPane; }
        }

        protected override void OnAttach()
        {
            if (_hostControl != null) _parentDockManager = XamDockManager.GetDockManager(_hostControl);
            if (_parentDockManager != null)
                _parentDockManager.ActivePaneChanged += DockManager_ActivePaneChanged;

            Region.ActiveViews.CollectionChanged += ActiveViews_CollectionChanged;
        }

        private void DockManager_ActivePaneChanged(object sender, RoutedPropertyChangedEventArgs<ContentPane> e)
        {
            if (e.OldValue != null)
            {
                var item = e.OldValue;

                //are we dealing with a ContentPane directly
                if (Region.Views.Contains(item) && Region.ActiveViews.Contains(item))
                {
                    Region.Deactivate(item);
                }
                else
                {
                    //now check to see if we have any views that were injected
                    if (item is ContentControl contentControl)
                    {
                        var injectedView = contentControl.Content;
                        if (Region.Views.Contains(injectedView) && Region.ActiveViews.Contains(injectedView))
                            Region.Deactivate(injectedView);
                    }
                }
            }

            if (e.NewValue != null)
            {
                var item = e.NewValue;

                //are we dealing with a ContentPane directly
                if (Region.Views.Contains(item) && !Region.ActiveViews.Contains(item))
                {
                    Region.Activate(item);
                }
                else
                {
                    //now check to see if we have any views that were injected
                    if (item is ContentControl contentControl)
                    {
                        var injectedView = contentControl.Content;
                        if (Region.Views.Contains(injectedView) && !Region.ActiveViews.Contains(injectedView))
                            Region.Activate(injectedView);
                    }
                }
            }
        }

        private void ActiveViews_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                //are we dealing with a view
                if (e.NewItems[0] is FrameworkElement frameworkElement)
                {
                    ContentPane contentPane = frameworkElement as ContentPane;
                    if (contentPane == null)
                        contentPane = frameworkElement.Parent as ContentPane;

                    if (contentPane != null && !contentPane.IsActivePane)
                        contentPane.Activate();
                }
                else
                {
                    //must be a viewmodel
                    object viewModel = e.NewItems[0];
                    var contentPane = GetContentPaneFromFromViewModel(viewModel);
                    if (contentPane != null)
                        contentPane.Activate();
                }
            }
        }

        private ContentPane GetContentPaneFromFromViewModel(object viewModel)
        {
            var panes = XamDockManager.GetDockManager(_hostControl).GetPanes(PaneNavigationOrder.VisibleOrder);
            return panes.FirstOrDefault(contentPane => contentPane.DataContext == viewModel);
        }
    }
}
