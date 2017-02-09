// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("Trinity.Core")]
[assembly: AssemblyDescription("Trinity.Core.dll")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany(ProductInfo.COMPANY_PLACEHOLDER)]
[assembly: AssemblyProduct(ProductInfo.PRODUCTNAME_PLACEHOLDER)]
[assembly: AssemblyCopyright(ProductInfo.COPYRIGHT_PLACEHOLDER)]
[assembly: AssemblyTrademark(ProductInfo.TRADEMARK_PLACEHOLDER)]
[assembly: AssemblyCulture(ProductInfo.CULTURE_PLACEHOLDER)]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("6a2d9bb4-48d8-4392-9850-dd616995a2a2")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// major number.minor number.build number.private part number
[assembly: AssemblyVersion(ProductInfo.VERSION_PLACEHOLDER)]
[assembly: AssemblyFileVersion(ProductInfo.FILEVERSION_PLACEHOLDER)]

#region System Component
[assembly: InternalsVisibleTo("Trinity.TSL.CodeTemplates" + TrinityPublicKey.MSPublicKey)]
[assembly: InternalsVisibleTo("Trinity.Azure.ManagementService.Core" + TrinityPublicKey.MSPublicKey)]
[assembly: InternalsVisibleTo("geconfig" + TrinityPublicKey.MSPublicKey)]
[assembly: InternalsVisibleTo("gediagnose" + TrinityPublicKey.MSPublicKey)]
#endregion

#region Trinity Generated Code
[assembly: InternalsVisibleTo("Trinity.Azure.ManagementService.Extension" + TrinityPublicKey.MSPublicKey)]
[assembly: InternalsVisibleTo("Trinity.Azure.ManagementService.Extension" + TrinityPublicKey.Key4GeneratedCode)]
[assembly: InternalsVisibleTo("Trinity.Extension.Accessor" + TrinityPublicKey.Key4GeneratedCode)]
[assembly: InternalsVisibleTo("Trinity.Extension.Storage" + TrinityPublicKey.Key4GeneratedCode)]
#endregion

#region Tools and Tests
[assembly: InternalsVisibleTo("TrinityService" + TrinityPublicKey.Value)]
[assembly: InternalsVisibleTo("Trinity.Math" + TrinityPublicKey.Value)]
[assembly: InternalsVisibleTo("Trinity.Shell" + TrinityPublicKey.Value)]
[assembly: InternalsVisibleTo("Trinity.Core.UnitTest" + TrinityPublicKey.Value)]
[assembly: InternalsVisibleTo("CloudStorage.Test" + TrinityPublicKey.Value)]
[assembly: InternalsVisibleTo("Trinity.Test" + TrinityPublicKey.Value)]
#endregion


