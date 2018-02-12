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

using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using Castle.Windsor;
using Infragistics.Windows.DataPresenter;
using Infragistics.Windows.DataPresenter.Events;
using Prism.Events;
using Prism.Regions;
using Reactive.Bindings;
using Trinity.GE.WPF.TestFacility.UI.ProgramInfo.Module.Data.Models;
using Trinity.GE.WPF.UI.Infrastructure.Base.ViewModel;
using Trinity.GE.WPF.UI.Infrastructure.Prism.Interfaces;
using Trinity.GE.WPF.UI.Infrastructure.UI.ShellServices;
using ProgramInfoView = Trinity.GE.WPF.TestFacility.UI.ProgramInfo.Module.Views.ProgramInfoView;

namespace Trinity.GE.WPF.TestFacility.UI.ProgramInfo.Module.ViewModels
{
    public class ProgramInfoViewModel : ViewModelBase, IRegionManagerAware
    {
        public IUIShellService TheGlobalShellService { get; set; }
        //private IRegionManager PrismRegionManager { get; set; }
        //private IWindsorContainer WindsorContainer { get; set; }
        //private IEventAggregator PrismEventAggregator { get; set; }
        private Guid _privateContentKey  = Guid.Empty;
        private Guid _sharedContentKey   = Guid.Empty;

        private XamDataPresenter DataPresenter { get; } = null;
        private FieldLayout _fieldLayout = new FieldLayout();

        // Setup Support for Reactive Processing using the ReactiveProperty Library

        public ReactiveProperty<string> InputText { get; private set; }
        public ReactiveProperty<string> DisplayText { get; private set; }
        public ReactiveCommand ReplaceTextCommand { get; private set; }

        public ReactiveProperty<RoutedEventArgs> ReactOnLoaded { get; set; }
        public ReactiveProperty<FieldLayoutInitializedEventArgs> ReactOnInitialized { get; private set; }
        public ReactiveProperty<FieldLayoutInitializingEventArgs> ReactOnInitializing { get; private set; }
        public ReactiveProperty<EventArgs> ReactOnControlInitalized { get; private set; }
        // FieldLayoutLoaded
        public ReactiveProperty<int> ReturnCode { get; set; }

        private Guid PrivateContentKey
        {
            get { return _privateContentKey; }
            set { _privateContentKey = value; }
        }

        private Guid SharedContentKey
        {
            get { return _sharedContentKey; }
            set { _sharedContentKey = value; }
        }

        public ProgramInfoView ActiveView { get; set; }

        public ProgramInfoDataModel ProgramInfoData { get; set; }

        private DataTable ProgramInfoDataDataTable
        {
            get { return _programInfoDataDataTable; }
            set { _programInfoDataDataTable = value; }
        }

        public DataView ProgramInfoDataDataView
        {
            get { return _programInfoDataDataView; }
            set
            {
                _programInfoDataDataView = value;
                SetProperty(ref _programInfoDataDataView, value);              
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        public object GetViewFromRegionFromViewModel(object viewModel, IRegion region)
        {
            return region.Views.Cast<FrameworkElement>().FirstOrDefault(view => view.DataContext == viewModel);
        }

        // Declare local variables for backing storage
        // Fist let's new-up the model for the UI

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prismRegionManager"></param>
        /// <param name="windsorContainer"></param>
        /// <param name="prismEventAggregator"></param>
        /// <param name="theGlobalShellService"></param>
        public ProgramInfoViewModel(IRegionManager prismRegionManager, 
                                    IWindsorContainer windsorContainer, 
                                    IEventAggregator prismEventAggregator,
                                    IUIShellService theGlobalShellService) : base(prismRegionManager, windsorContainer, prismEventAggregator)
        {
            TheGlobalShellService = theGlobalShellService;

            var fs = _fieldLayout.FieldSettings = new FieldSettings();

            Header = "About this Program";

            // mode is Flags. (default is all)
            // DistinctUntilChanged is no push value if next value is same as current
            // RaiseLatestValueOnSubscribe is push value when subscribed
            const ReactivePropertyMode allMode = ReactivePropertyMode.DistinctUntilChanged |
                                                 ReactivePropertyMode.RaiseLatestValueOnSubscribe;

            const ReactivePropertyMode modeIsNone = ReactivePropertyMode.DistinctUntilChanged;

            // Let's get the ProgramInformation Data Loaded

            ReactOnLoaded            = new ReactiveProperty<RoutedEventArgs>(mode: ReactivePropertyMode.None);
            ReactOnInitialized       = new ReactiveProperty<FieldLayoutInitializedEventArgs>(mode: allMode);
            ReactOnInitializing      = new ReactiveProperty<FieldLayoutInitializingEventArgs>();
            ReactOnControlInitalized = new ReactiveProperty<EventArgs>();

            ProgramInfoData = new ProgramInfoDataModel();

            ConfigureDataCardView();

            this.ReturnCode = ReactOnLoaded.Select((e, r) =>
            {
                var xamDataCards = e.OriginalSource as XamDataCards;

                //if (_xamDataCards != null)
                //{
                //    var x = _xamDataCards.FieldLayouts[0];
                //    ConfigureFieldsOnDataCard(x);
                //}

                return 0;
            }).ToReactiveProperty();

            // Make Async Reactive Call
            this.ReturnCode = ReactOnInitialized.Select((e, r) =>
            {
                if (e != null)
                {
                    //var dataCardVioew = e as XamDataCards;
                    ConfigureFieldsOnDataCard(e.FieldLayout);
                    e.Handled = true;
                }

                return (0);
            }).ToReactiveProperty();

            // Make Async Reactive Call
            this.ReturnCode = ReactOnInitializing.Select((e, r) =>
            {
                if (e != null)
                {
                    ConfigureFieldsOnDataCard(e.FieldLayout);

                    e.Handled = true;
                }

                return (0);
            }).ToReactiveProperty();

            // Make Async Reactive Call
            this.ReturnCode = ReactOnControlInitalized.Select((e, r) =>
            {
                var t = string.Empty;

                return (0);
            }).ToReactiveProperty();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void _xamDataCards_FieldLayoutInitialized(object sender, FieldLayoutInitializedEventArgs e)
        {
            ConfigureFieldsOnDataCard(e.FieldLayout);
        }

        private Style GenerateProgramDescriptionStyle()
        {
            //    < Style x: Key = "ProgramShortDescStyle" TargetType = "{x:Type igDP:CellValuePresenter}" >
    
            //        < Setter Property = "Template" >
     
            //             < Setter.Value >
     
            //                 < ControlTemplate TargetType = "{x:Type igDP:CellValuePresenter}" >
      
            //                      < Grid Width = "{TemplateBinding Width}" Height = "{TemplateBinding Height}" >
         
            //                             < TextBlock Width = "Auto"
            //                               Height = "Auto"
            //                               Margin = "{TemplateBinding Padding}"
            //                               Text = "{Binding RelativeSource={RelativeSource TemplatedParent}, Path=Value}"
            //                               LineStackingStrategy = "BlockLineHeight"
            //                               TextTrimming = "None"
            //                               HorizontalAlignment = "Stretch"
            //                               TextAlignment = "Left"
            //                               TextWrapping = "Wrap" />
            //                </ Grid >
            //            </ ControlTemplate >
            //        </ Setter.Value >
            //    </ Setter >
            //</ Style >

            Style programDescStyle = new Style(typeof(CellValuePresenter));
            programDescStyle.RegisterName("ProgramShortDescStyle", null);
            programDescStyle.TargetType = typeof(CellValuePresenter);
            //programDescStyle.


            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fieldLayout"></param>
        private void ConfigureFieldsOnDataCard(FieldLayout fieldLayout)
        {

            //fieldLayout.Settings.AutoArrangeCells = AutoArrangeCells.LeftToRight;

            var pgmName     = new Field { Name = "ProgramName", Label = "Program Name"};
            var pgmVersion  = new Field { Name = "Version", Label = "Version"};
            var pgmRelease  = new Field { Name = "Release", Label = "Release" };
            var ipOwner     = new Field { Name = "IPOwner", Label = "IP Owner" };
            var address1    = new Field { Name = "Address1", Label = "Address" };
            var address2    = new Field { Name = "Address2", Label = "Address 2" };
            var city        = new Field { Name = "City", Label = "City" }; 
            var state       = new Field { Name = "State", Label = "State" };
            var phoneNumber = new Field { Name = "PhoneNumber", Label = "Phone Number" };
            var zipCode     = new Field { Name = "ZipCode", Label = "Zip Code" };
            var synopsisContentData = new Field { Name = "SynopsisContentData", Label = "Short Description" };
            var pgmAuthor   = new Field { Name = "Author", Label = "Author" };
            var profAssoc   = new Field { Name = "ProfessionalAssociation", Label = "Professional Association" };

            // First, let's remove whatever fields are there

            //var fieldSettingsforShortDesc = new FieldSettings
            //{
            //    CellMinHeight = 100,
            //    CellMinWidth  = 200,
            //    CellContentAlignment = CellContentAlignment.LabelAboveValueAlignLeft,
            //    AutoSizeOptions = FieldAutoSizeOptions.DataCells,
            //    CellValuePresenterStyle = ActiveView.Resources["ProgramShortDescStyle"] as Style
            //};

            fieldLayout.Fields.Clear();

            fieldLayout.Fields.Add(pgmName);           
            fieldLayout.Fields.Add(pgmVersion);
            fieldLayout.Fields.Add(pgmRelease);
            fieldLayout.Fields.Add(ipOwner);
            fieldLayout.Fields.Add(address1);
            fieldLayout.Fields.Add(address2);
            fieldLayout.Fields.Add(city);
            fieldLayout.Fields.Add(state);
            fieldLayout.Fields.Add(phoneNumber);
            fieldLayout.Fields.Add(zipCode);
            fieldLayout.Fields.Add(synopsisContentData);
            fieldLayout.Fields.Add(pgmAuthor);
            fieldLayout.Fields.Add(profAssoc);

            var lenghtOfProgramTitle = fieldLayout.Fields["ProgramName"].LabelWidthResolved;

            //fieldLayout.FieldSettings = fieldSettingsforShortDesc;

            foreach (var synopsisContainer in fieldLayout.Fields.Where(summaryDataContainer => summaryDataContainer.Name.Equals("SynopsisContentData")))
            {
                synopsisContainer.Height = new FieldLength(205);
                synopsisContainer.Width  = new FieldLength(synopsisContainer.Height.Value.Value*2);
                synopsisContainer.Settings.CellContentAlignment = CellContentAlignment.LabelAboveValueAlignLeft;
                synopsisContainer.Settings.AllowResize   = true;
                synopsisContainer.Settings.CellMinHeight = 150;
                synopsisContainer.Settings.CellMinWidth  = 450;
                synopsisContainer.Settings.CellHeight    = lenghtOfProgramTitle;
                synopsisContainer.Settings.CellWidth     = lenghtOfProgramTitle*0.5;
                synopsisContainer.Settings.CellValuePresenterStyle = ActiveView.Resources["ProgramShortDescStyle"] as Style;
            }

            // Now we need to recalculate the size of the Datacard!

            var totalFieldHeight = 0.0;
            var totalFieldWidth = 0.0;

            foreach (var field in fieldLayout.Fields)
            {
                totalFieldHeight = totalFieldHeight + field.LabelHeightResolved;
                totalFieldWidth = totalFieldWidth + field.LabelWidthResolved;
            }

            //var dataCardActualHeight = ActiveView.UIProgramInfoData.ActualHeight;
            var dataCardActualWidth = ActiveView.UIProgramInfoData.ActualWidth;

            //ActiveView.UIProgramInfoData.ViewSettings.CardHeight = (dataCardActualHeight + (dataCardActualHeight % 5.5));
            ActiveView.UIProgramInfoData.ViewSettings.CardWidth = (dataCardActualWidth + (dataCardActualWidth % 0.5));
        }

        private DataTable _programInfoDataDataTable;
        private DataView  _programInfoDataDataView;

        private void ConfigureDataCardView()
        {
            // We have to new up and DataTable so that we can bind to the XamDataCard
            // Step 1: New-up and DataTable
            _programInfoDataDataTable = new DataTable("ProgramInformationData");
            // Step 2: New-up the Columns
            var dcProgramName = new DataColumn("ProgramName", typeof (string));
            var dcVersion     = new DataColumn("Version", typeof (string));
            var dcRelease     = new DataColumn("Release", typeof(string));
            var dcIPOwner     = new DataColumn("IPOwner", typeof(string));
            // Contact Information
            var dcAddress1    = new DataColumn("Address1", typeof(string));
            var dcAddress2    = new DataColumn("Address2", typeof(string));
            var dcCity        = new DataColumn("City", typeof(string));
            var dcState       = new DataColumn("State", typeof(string));
            var dcPhoneNumber = new DataColumn("PhoneNumber", typeof(string));
            var dcZipCode     = new DataColumn("ZipCode", typeof (string));
            // Short Program Description
            var dcSynopsisContentData = new DataColumn("SynopsisContentData", typeof(string));
            // AOriginal Author Information
            var dcAuthor      = new DataColumn("Author", typeof(string));
            var dcProfessionalAssociation = new DataColumn("ProfessionalAssociation", typeof(string));

            // Set 3: Let's add the column to the in-memory data table

            _programInfoDataDataTable.Columns.Add(dcProgramName);
            _programInfoDataDataTable.Columns.Add(dcVersion);
            _programInfoDataDataTable.Columns.Add(dcRelease);
            _programInfoDataDataTable.Columns.Add(dcIPOwner);
            _programInfoDataDataTable.Columns.Add(dcAddress1);
            _programInfoDataDataTable.Columns.Add(dcAddress2);
            _programInfoDataDataTable.Columns.Add(dcCity);
            _programInfoDataDataTable.Columns.Add(dcState);
            _programInfoDataDataTable.Columns.Add(dcPhoneNumber);
            _programInfoDataDataTable.Columns.Add(dcZipCode);
            _programInfoDataDataTable.Columns.Add(dcSynopsisContentData);
            _programInfoDataDataTable.Columns.Add(dcAuthor);
            _programInfoDataDataTable.Columns.Add(dcProfessionalAssociation);

            // Step 4: Begin loading data into the table

            _programInfoDataDataTable.BeginLoadData();

            var pgmShortDesc = string.Empty;

            foreach (var lineOfText in ProgramInfoData.ShortProgramDescription)
            {
                pgmShortDesc = pgmShortDesc + " " + lineOfText;
                var trimEnd = pgmShortDesc.TrimEnd(' ').TrimStart(' ');
            }

            var drPrgramData = _programInfoDataDataTable.NewRow();

            drPrgramData[dcProgramName] = ProgramInfoData.ProgramName;
            drPrgramData[dcVersion]     = ProgramInfoData.Version;
            drPrgramData[dcRelease]     = ProgramInfoData.Release;
            drPrgramData[dcIPOwner]     = ProgramInfoData.IPOwner;
            drPrgramData[dcAddress1]    = ProgramInfoData.Address1;
            drPrgramData[dcAddress2]    = ProgramInfoData.Address2;
            drPrgramData[dcCity]        = ProgramInfoData.City;
            drPrgramData[dcState]       = ProgramInfoData.State;
            drPrgramData[dcPhoneNumber] = ProgramInfoData.PhoneNumber;
            drPrgramData[dcZipCode]     = ProgramInfoData.ZipCode.ToString(CultureInfo.InvariantCulture);
            drPrgramData[dcSynopsisContentData] = pgmShortDesc;
            drPrgramData[dcAuthor]      = ProgramInfoData.Author;
            drPrgramData[dcProfessionalAssociation] = ProgramInfoData.ProfessionalAssociation;

            // Step 5: Add the row of data to the table

            _programInfoDataDataTable.Rows.Add(drPrgramData);
            _programInfoDataDataTable.AcceptChanges();
            _programInfoDataDataTable.EndLoadData();

            _programInfoDataDataView = _programInfoDataDataTable.DefaultView;

            SetProperty(ref _programInfoDataDataView, _programInfoDataDataTable.DefaultView,
                $@"ProgramInfoDataDataView");
        }

        #region Implementation of IRegionManagerAware

        public IRegionManager RegionManagerAware { get; set; }

        #endregion
    }
}