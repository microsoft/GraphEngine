// Copyright (c) 2011 rubicon IT GmbH
using System.Windows.Controls;
using Trinity.GE.WPF.TestFacility.UI.MainMenu.Module.ViewModels;

namespace Trinity.GE.WPF.TestFacility.UI.MainMenu.Module.Views
{
    /// <summary>
    /// Interaction logic for MainMenuView.xaml
    /// </summary>
    public partial class MainMenuView : UserControl
    {
        public MainMenuView()
        {
            InitializeComponent();
        }

        private MainMenuViewModel ViewModel
        {
            get { return (MainMenuViewModel)DataContext; }
            set { DataContext = value; }
        }

        public void ApplyViewModel(MainMenuViewModel uiViewModel)
        {
            ViewModel = uiViewModel as MainMenuViewModel;
            this.DataContext = ViewModel;
        }
    }
}
