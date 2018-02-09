using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using Infragistics.Windows.DockManager;
using Prism.Regions;
using Prism.Regions.Behaviors;

namespace Trinity.GE.WPF.UI.Infrastructure.Prism.Region.Adapters
{
    public class TabGroupPaneRegionBehavior : RegionBehavior, IHostAwareRegionBehavior
    {
        public const string BehaviorKey = "TabGroupPaneRegionBehavior";
        private bool _updatingActiveViewsInHostControlSelectionChanged;

        private static readonly DependencyProperty IsGeneratedProperty = DependencyProperty.RegisterAttached("IsGenerated", typeof(bool), typeof(TabGroupPaneRegionBehavior), null);

        TabGroupPane _hostControl;
        public System.Windows.DependencyObject HostControl
        {
            get { return _hostControl; }
            set { _hostControl = value as TabGroupPane; }
        }

        protected override void OnAttach()
        {
            if (_hostControl.ItemsSource != null)
                throw new InvalidOperationException("ItemsControl's ItemsSource property is not empty. This control is being associated with a region, but the control is already bound to something else. If you did not explicitly set the control's ItemSource property, this exception may be caused by a change in the value of the inherited RegionManager attached property.");

            SynchronizeItems();

            _hostControl.SelectionChanged += HostControl_SelectionChanged;
            Prism.Region.Views.CollectionChanged += Views_CollectionChanged;
        }

        void Views_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                int startIndex = e.NewStartingIndex;
                foreach (object newItem in e.NewItems)
                {
                    ContentPane contentPane = this.PrepareContainerForItem(newItem, _hostControl);
                    _hostControl.Items.Insert(startIndex++, contentPane);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (object oldItem in e.OldItems)
                {
                    ContentPane contentPane = GetContainerForItem(oldItem, _hostControl.Items);
                    if (contentPane != null)
                    {
                        _hostControl.Items.Remove(contentPane);
                        ClearContainerForItem(contentPane);
                    }
                }
            }
        }

        void HostControl_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            foreach (object item in e.RemovedItems)
            {
                //this first checks to see if we had any default views declared in XAML
                if (Prism.Region.Views.Contains(item) && Prism.Region.ActiveViews.Contains(item))
                {
                    Prism.Region.Deactivate(item);
                    continue;
                }

                //now check to see if we have any views that were injected
                var contentControl = item as ContentControl;
                if (contentControl != null)
                {
                    var injectedView = contentControl.Content;
                    if (Prism.Region.Views.Contains(injectedView) && Prism.Region.ActiveViews.Contains(injectedView))
                        Prism.Region.Deactivate(injectedView);
                }
            }

            foreach (object item in e.AddedItems)
            {
                //this first checks to see if we had any default views declared in XAML
                if (Prism.Region.Views.Contains(item))
                {
                    Prism.Region.Activate(item);
                    continue;
                }

                //now check to see if we have ay views that were injected
                var contentControl = item as ContentControl;
                if (contentControl != null)
                {
                    var injectedView = contentControl.Content;
                    if (Prism.Region.Views.Contains(injectedView) && !this.Region.ActiveViews.Contains(item))
                        Prism.Region.Activate(injectedView);
                }
            }
        }

        private void SynchronizeItems()
        {
            List<object> existingItems = new List<object>();
            if (_hostControl.Items.Count > 0)
            {
                foreach (object childItem in _hostControl.Items)
                {
                    existingItems.Add(childItem);
                }
            }

            foreach (object view in Prism.Region.Views)
            {
                var contentPane = PrepareContainerForItem(view, _hostControl);
                _hostControl.Items.Add(contentPane);
            }

            foreach (object existingItem in existingItems)
            {
                PrepareContainerForItem(existingItem, _hostControl);
                Prism.Region.Add(existingItem);
            }
        }

        private static object GetDataContext(object item)
        {
            FrameworkElement frameworkElement = item as FrameworkElement;
            return frameworkElement == null ? item : frameworkElement.DataContext;
        }

        protected virtual void ClearContainerForItem(ContentPane contentPane)
        {
            if (contentPane == null) throw new ArgumentNullException("contentPane");
            if ((bool)contentPane.GetValue(IsGeneratedProperty))
            {
                contentPane.Content = null;
            }
        }

        protected virtual ContentPane GetContainerForItem(object item, ItemCollection itemCollection)
        {
            if (itemCollection == null) throw new ArgumentNullException("itemCollection");
            ContentPane container = item as ContentPane;
            if (container != null && ((bool)container.GetValue(IsGeneratedProperty)) == false)
            {
                return container;
            }

            foreach (ContentPane tabItem in itemCollection)
            {
                if ((bool)tabItem.GetValue(IsGeneratedProperty))
                {
                    if (tabItem.Content == item)
                    {
                        return tabItem;
                    }
                }
            }

            return null;
        }

        protected virtual ContentPane PrepareContainerForItem(object item, DependencyObject parent)
        {
            ContentPane container = item as ContentPane;

            if (container == null)
            {
                object dataContext = GetDataContext(item);
                container = new ContentPane();
                container.Content = item;
                container.DataContext = dataContext;
                container.SetValue(IsGeneratedProperty, true);
            }

            container.Closed += Container_Closed;

            return container;
        }

        void Container_Closed(object sender, Infragistics.Windows.DockManager.Events.PaneClosedEventArgs e)
        {
            ContentPane container = sender as ContentPane;
            if (container != null)
            {
                container.Closed -= Container_Closed;

                if (Prism.Region.Views.Contains(container))
                    Prism.Region.Remove(container);

                var item = container.Content;
                if (item != null && Prism.Region.Views.Contains(item))
                    Prism.Region.Remove(item);
            }
        }
    }
}
