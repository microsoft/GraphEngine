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

using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Castle.Windsor;
using Prism.Events;
using Prism.Regions;
using Trinity.GE.WPF.TestFacility.UIShell.Module.UI.ViewModels;

namespace Trinity.GE.WPF.TestFacility.UIShell.Module.Shell
{
    /// <summary>
    /// Interaction logic for MainShellWindow.xaml
    /// </summary>
    public partial class MainShellWindow : Window
    {
        private IRegionManager PrismRegionManager { get; set; }
        private IWindsorContainer WindsorContainer { get; set; }
        private IEventAggregator PrismEventAggregator { get; set; }

        public MainShellWindow(IRegionManager theGlobalRegionManager,
                               IWindsorContainer theGlobalWindsorContainer,
                               IEventAggregator theGlobalEventAggregator,
                               MainShellWindowViewModel theShellViewModel)
        {
            if (theShellViewModel != null) this.DataContext = theShellViewModel;

            InitializeComponent();

            PrismRegionManager   = theGlobalRegionManager;
            WindsorContainer     = theGlobalWindsorContainer;
            PrismEventAggregator = theGlobalEventAggregator;

            ViewModel = theShellViewModel;
        }

        public MainShellWindowViewModel ViewModel
        {
            get { return (MainShellWindowViewModel)DataContext; }
            set { DataContext = value; }
        }

        public void ApplyViewModel(MainShellWindowViewModel theShellViewModel)
        {
            if (theShellViewModel != null) ViewModel = theShellViewModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="availableSize"></param>
        /// <returns></returns>

        protected override Size MeasureOverride(Size availableSize)
        {
            var mySize = new Size();

            var co = FindVisualChild<Panel>(this);

            var lc = GetLogicalChildCollection<Control>(this);

            var lccanvas = GetLogicalChildCollection<Grid>(this);


            foreach (UIElement child in co.Children)
            {
                child.Measure(availableSize);
                mySize.Width += child.DesiredSize.Width;
                mySize.Height += child.DesiredSize.Height;
            }

            //return mySize;


            return base.MeasureOverride(availableSize);
        }

        private TChildItem FindVisualChild<TChildItem>(DependencyObject obj)
            where TChildItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child is TChildItem item)
                    return item;
                else
                {
                    TChildItem childOfChild = FindVisualChild<TChildItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        public static List<T> GetLogicalChildCollection<T>(object parent) where T : DependencyObject
        {
            List<T> logicalCollection = new List<T>();
            GetLogicalChildCollection(parent as DependencyObject, logicalCollection);
            return logicalCollection;
        }

        private static void GetLogicalChildCollection<T>(DependencyObject parent, List<T> logicalCollection) where T : DependencyObject
        {
            IEnumerable children = LogicalTreeHelper.GetChildren(parent);
            foreach (object child in children)
            {
                if (child is DependencyObject)
                {
                    DependencyObject depChild = child as DependencyObject;
                    if (child is T)
                    {
                        logicalCollection.Add(child as T);
                    }
                    GetLogicalChildCollection(depChild, logicalCollection);
                }
            }
        }

        private void MainShellWindowHost_Closed(object sender, System.EventArgs e)
        {
            ;
        }

        private void MainShellWindowHost_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            ;
        }
    }
}
