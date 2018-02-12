/* --------------------------------------------------------------------------------+
 * InKnowWorks Controplus: IKW.Contropolus.VKMC.UI.ProgramInfo.Module              *
 * Designed and Written by Tavi Truman                                             *
 * Version 1.0.0                                                                   *
 * InKnowWorks, Corp. proprietary/confidential. Use is subject to license terms.   *
 * Redistribution of this file for of an unauthorized byte-code version            *
 * of this file is strictly forbidden.                                             *
 * Copyright (c) 2009-2015 by InKnowWorks, Corp.                                   *
 * 2143 Willester Aave, San Jose, CA 95124. All rights reserved.                   *
 * --------------------------------------------------------------------------------*/

using System.Windows;
using System.Windows.Controls;
using Trinity.GE.WPF.TestFacility.UI.ProgramInfo.Module.ViewModels;

namespace Trinity.GE.WPF.TestFacility.UI.ProgramInfo.Module.Views
{
    /// <summary>
    /// Interaction logic for ProgramInfoView.xaml
    /// </summary>
    public partial class ProgramInfoView : UserControl
    {
        public ProgramInfoView()
        {
            InitializeComponent();
        }

        private ProgramInfoViewModel ViewModel
        {
            get { return (ProgramInfoViewModel)DataContext; }
            set { DataContext = value; }
        }

        public void ApplyViewModel(ProgramInfoViewModel uiViewModel)
        {
            ViewModel = uiViewModel as ProgramInfoViewModel;
            this.DataContext = ViewModel;
        }

        private void UIProgramInfoData_OnLoaded(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void FrameworkContentElement_OnLoaded(object sender, RoutedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
