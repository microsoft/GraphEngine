/* --------------------------------------------------------------------------------+
 * InKnowWorks Controplus: IKW.Contropolus.UI.ProgramInfo.Module                   *
 * Designed and Written by Tavi Truman                                             *
 * Version 1.0.0                                                                   *
 * InKnowWorks, Corp. proprietary/confidential. Use is subject to license terms.   *
 * Redistribution of this file for of an unauthorized byte-code version            *
 * of this file is strictly forbidden.                                             *
 * Copyright (c) 2009-2018 by InKnowWorks, Corp.                                   *
 * 2143 Willester Aave, San Jose, CA 95124. All rights reserved.                   *
 * --------------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using Prism.Mvvm;

namespace Trinity.GE.WPF.TestFacility.UI.ProgramInfo.Module.Data.Models
{
    public class ProgramInfoDataModel : BindableBase
    {
        private string _programName = string.Empty;
        private string _version = string.Empty;
        private string _release = string.Empty;
        private string _IPOwner = string.Empty;

        private Tuple<string, string, string, string, string, int> _contactInfo;
        private ObservableCollection<string> _shortProgramDescription = new ObservableCollection<string>();
        private Tuple<string, string> _originalAuthorInfo;
        private XElement _programInfoMetadata;

        private const string ikwNameSpaceUrl = @"http://www.inknowworks.com/program/metadata";
        private const string ikwPrefixOnNameSpace = "ikw";
        private const string ProgramInformationDataRootElement = "ProgramInformationData";
        private XNamespace SourceXmlNameSpace { get; set; }
        private XAttribute SourceNameSpacePrefix { get; set; }

        private List<string> MetadataSource { get; set; }
        private Uri DirPathToProgramInfoMetadata { get; set; }
        private string ProgramInfoMetadataFile { get; set; }

        private XNamespace OrgNameSpace
        {
            get
            {
                if (SourceXmlNameSpace == null)
                {
                    this.SourceXmlNameSpace = ikwNameSpaceUrl;
                    this.SourceNameSpacePrefix =
                        new XAttribute(XNamespace.Xmlns + ikwPrefixOnNameSpace, SourceXmlNameSpace);
                    return SourceXmlNameSpace;
                }
                else
                {
                    return SourceXmlNameSpace;
                }
            }
        }

        public string ProgramName
        {
            get { return _programName; }
            set
            {
                _programName = value;
                OnPropertyChanged("ProgramName");
            }
        }

        public string IPOwner
        {
            get { return _IPOwner; }
            set
            {
                _IPOwner = value;
            }
        }

        private Tuple<string, string, string, string, string, int> ContactInfo
        {
            get { return _contactInfo; }
            set { _contactInfo = value; }
        }

        public string Address1
        {
            get
            {
                return _contactInfo.Item1;
            }
        }

        public string Address2
        {
            get
            {
                return _contactInfo.Item2;
            }
        }

        public string Version
        {
            get { return _version; }
            set
            {
                _version = value;
                OnPropertyChanged("Version");
            }
        }

        public string City
        {
            get
            {
                return _contactInfo.Item3;
            }
        }

        public string PhoneNumber
        {
            get
            {
                return _contactInfo.Item5;
            }
        }

        public int ZipCode
        {
            get
            {
                return _contactInfo.Item6;
            }
        }

        public string State
        {
            get
            {
                return _contactInfo.Item4;
            }
        }

        public string Release
        {
            get { return _release; }
            set
            {
                _release = value;
                OnPropertyChanged("Release");
            }
        }

        public IEnumerable<string> ShortProgramDescription
        {
            get { return _shortProgramDescription; }
            set
            {
                _shortProgramDescription = value as ObservableCollection<string>;
                SetProperty<ObservableCollection<string>>(ref _shortProgramDescription, value as ObservableCollection<string>);
                OnPropertyChanged("ShortProgramDescription");
            }
        }

        private Tuple<string, string> OriginalAuthorInfo
        {
            get { return _originalAuthorInfo; }
            set
            {
                _originalAuthorInfo = value;
                SetProperty<string>(ref _author, value.Item1);
                SetProperty<string>(ref _professionalAssociation, value.Item2);
                OnPropertyChanged("OriginalAuthorInfo");
                OnPropertyChanged("ProfessionalAssociation");
            }
        }

        private string _author;
        private string _professionalAssociation;

        public string Author
        {
            get
            {
                return OriginalAuthorInfo.Item1 ?? null;
            }
            set { _author = value; }
        }

        public string ProfessionalAssociation
        {
            get { return OriginalAuthorInfo.Item2 ?? null; }
            set { _professionalAssociation = value; }
        }

        private XElement ProgramInfoMetadata
        {
            get { return _programInfoMetadata; }
            set { _programInfoMetadata = value; }
        }

        public ProgramInfoDataModel() 
        {
            LoadSourceFiles();
        }

        private XElement LoadSourceFiles()
        {
            DirPathToProgramInfoMetadata = new Uri(Path.GetFullPath(ConfigurationManager.AppSettings["UIProgramInfoDataRelativeDir"]));

            try
            {
                //DirPathToProgramInfoMetadata =
                //    new Uri(ConfigurationManager.AppSettings["UIProgramInfoDataAbsoluteDir"],
                //        UriKind.RelativeOrAbsolute);

                if (Directory.Exists(DirPathToProgramInfoMetadata.LocalPath))
                {
                    ProgramInfoMetadataFile = ConfigurationManager.AppSettings["UIProgramInfoDataFile"];

                    const string backSlash = @"\";

                    var exapandedFilePathInfo = $"{backSlash}{ProgramInfoMetadataFile}";

                    if (File.Exists(DirPathToProgramInfoMetadata.LocalPath.Insert(DirPathToProgramInfoMetadata.LocalPath.Length,
                        exapandedFilePathInfo)))
                    {
                        MetadataSource = Directory.GetFiles(DirPathToProgramInfoMetadata.LocalPath, "*.XML").AsQueryable()
                            .Select(xFiles => xFiles).ToList();

                        foreach (var xmlConfigFile in MetadataSource)
                        {
                            _programInfoMetadata = XElement.Load(xmlConfigFile, LoadOptions.PreserveWhitespace);

                            XNamespace sourceOrgNS = OrgNameSpace;

                            _programName = _programInfoMetadata.DescendantsAndSelf(sourceOrgNS + ProgramInformationDataRootElement)
                                .Elements<XElement>()
                                .Where(xElement => xElement.Name.LocalName.Equals("ProgramName"))
                                .Select(xValue => xValue.Value).First();

                            _version = _programInfoMetadata.DescendantsAndSelf(sourceOrgNS + ProgramInformationDataRootElement)
                                .Elements<XElement>()
                                .Where(xElement => xElement.Name.LocalName.Equals("Version"))
                                .Select(xValue => xValue.Value).First();

                            _release = _programInfoMetadata.DescendantsAndSelf(sourceOrgNS + ProgramInformationDataRootElement)
                                .Elements<XElement>()
                                .Where(xElement => xElement.Name.LocalName.Equals("Release"))
                                .Select(xValue => xValue.Value).First();

                            _IPOwner = _programInfoMetadata.DescendantsAndSelf(sourceOrgNS + ProgramInformationDataRootElement)
                                .Elements<XElement>()
                                .Where(xElement => xElement.Name.LocalName.Equals("IPOwner"))
                                .Select(xValue => xValue.Value).First();

                            // Populate the Contact Information

                            var xContactInfo = _programInfoMetadata.DescendantsAndSelf(sourceOrgNS +
                                                                                       ProgramInformationDataRootElement)
                                .Elements<XElement>(sourceOrgNS + "ContactInfo").DescendantsAndSelf()
                                .Where(xSource => xSource.Name.LocalName.Equals("ContactInfo"))
                                .Select(xContactMetadata => new
                                {
                                    address1 = xContactMetadata.Element(sourceOrgNS + "AddressLine1")?.Value,
                                    address2 = xContactMetadata.Element(sourceOrgNS + "AddressLine2")?.Value,
                                    city     = xContactMetadata.Element(sourceOrgNS + "City")?.Value,
                                    state    = xContactMetadata.Element(sourceOrgNS + "State")?.Value,
                                    phoneno  = xContactMetadata.Element(sourceOrgNS + "PhoneNumber")?.Value,
                                    zipcode  = Int32.Parse(xContactMetadata.Element(sourceOrgNS + "ZipCode")?.Value)
                                }).FirstOrDefault();

                            if (xContactInfo != null)
                                _contactInfo =
                                    new Tuple<string, string, string, string, string, int>(
                                        xContactInfo.address1,
                                        xContactInfo.address2,
                                        xContactInfo.city,
                                        xContactInfo.state,
                                        xContactInfo.phoneno,
                                        xContactInfo.zipcode);

                            var shortDesc = _programInfoMetadata.DescendantsAndSelf(sourceOrgNS +
                                                                                    ProgramInformationDataRootElement)
                                .Elements<XElement>(sourceOrgNS + "ShortProgramDescription").DescendantsAndSelf()
                                .Where(xElement => xElement.Name.LocalName == "ShortProgramDescription")
                                .Elements<XElement>(sourceOrgNS + "Para")
                                .Select(xText => xText.Value).ToList();

                            foreach (var lineOfText in shortDesc)
                            {
                                _shortProgramDescription.Add(lineOfText);
                            }

                            var xOrginialAuthorInfo = _programInfoMetadata.DescendantsAndSelf(sourceOrgNS +
                                                                                      ProgramInformationDataRootElement)
                               .Elements<XElement>(sourceOrgNS + "OrginialAuthorInfo").DescendantsAndSelf()
                               .Where(xSource => xSource.Name.LocalName.Equals("OrginialAuthorInfo"))
                               .Select(xOrginialAuthorMetaData => new
                               {
                                   author = xOrginialAuthorMetaData.Element(sourceOrgNS + "Author")?.Value,
                                   profAssoc = xOrginialAuthorMetaData.Element(sourceOrgNS + "ProfessionalAssociation")?.Value
                               }).FirstOrDefault();

                            if (xOrginialAuthorInfo != null)
                                _originalAuthorInfo =
                                    new Tuple<string, string>(
                                        xOrginialAuthorInfo.author,
                                        xOrginialAuthorInfo.profAssoc);
                        }
                    }
                    else
                    {
                        MessageBox.Show("An Unexpected Error has been detected: Unable to locate this file ->> " + DirPathToProgramInfoMetadata.LocalPath);
                        return null;
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("An Unexpected Error has been detected: " + ex.Message);
                return null;
            }

            return null;
        }
    }
}