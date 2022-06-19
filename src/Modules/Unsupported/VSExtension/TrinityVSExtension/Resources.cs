/***************************************************************************

Copyright (c) Microsoft Corporation. All rights reserved.
This code is licensed under the Visual Studio SDK license terms.
THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.

***************************************************************************/

using System;
using System.Reflection;
using System.Globalization;
using System.Resources;
using System.Threading;

namespace Trinity.VSExtension
{
    /// <summary>
    /// This class represent resource storage and management functionality.
    /// </summary>
    internal sealed class Resources
    {
        #region Constants
        //internal const string MsgFailedToLoadTemplateFile = "Failed to add template file to project";
        internal const string Application                          = "Application";
        internal const string ApplicationCaption                   = "ApplicationCaption";
        internal const string ApplicationIcon                      = "ApplicationIcon";
        internal const string ApplicationIconDescription           = "ApplicationIconDescription";
        internal const string AssemblyName                         = "AssemblyName";
        internal const string AssemblyNameDescription              = "AssemblyNameDescription";
        internal const string CleanSourceCodeAfterBuild            = "CleanSourceCodeAfterBuild";
        internal const string CleanSourceCodeAfterBuildDescription = "CleanSourceCodeAfterBuildDescription";
        internal const string GenerateDebugSymbols                 = "GenerateDebugSymbols";
        internal const string GenerateDebugSymbolsDescription      = "GenerateDebugSymbolsDescription";
        internal const string DefaultNamespace                     = "DefaultNamespace";
        internal const string DefaultNamespaceDescription          = "DefaultNamespaceDescription";
        internal const string GeneralCaption                       = "General Properties";
        internal const string NestedProjectFileAssemblyFilter      = "NestedProjectFileAssemblyFilter";
        internal const string OutputFile                           = "OutputFile";
        internal const string OutputFileDescription                = "OutputFileDescription";
        internal const string OutputType                           = "OutputType";
        internal const string OutputTypeDescription                = "OutputTypeDescription";
        internal const string Project                              = "Project";
        internal const string ProjectFile                          = "ProjectFile";
        internal const string ProjectFileDescription               = "ProjectFileDescription";
        internal const string ProjectFolder                        = "ProjectFolder";
        internal const string ProjectFolderDescription             = "ProjectFolderDescription";
        internal const string RootNamespace                        = "RootNamespace";
        internal const string RootNamespaceDescription             = "RootNamespaceDescription";
        internal const string StartupObject                        = "StartupObject";
        internal const string StartupObjectDescription             = "StartupObjectDescription";
        internal const string TargetFrameworkMoniker               = "TargetFrameworkMoniker";
        internal const string TargetFrameworkMonikerDescription    = "TargetFrameworkMonikerDescription";
        #endregion Constants

        #region Fields
        private static Resources loader;
        private ResourceManager resourceManager;
        private static Object internalSyncObjectInstance;
        #endregion Fields

        #region Constructors
        /// <summary>
        /// Internal explicitly defined default constructor.
        /// </summary>
        internal Resources()
        {
            resourceManager = new System.Resources.ResourceManager("Trinity.VSExtension.TrinityVSExtensionResources",
                Assembly.GetExecutingAssembly());
        }
        #endregion Constructors

        #region Properties
        /// <summary>
        /// Gets the internal sync. object.
        /// </summary>
        private static Object InternalSyncObject
        {
            get
            {
                if (internalSyncObjectInstance == null)
                {
                    Object o = new Object();
                    Interlocked.CompareExchange(ref internalSyncObjectInstance, o, null);
                }
                return internalSyncObjectInstance;
            }
        }
        /// <summary>
        /// Gets information about a specific culture.
        /// </summary>
        private static CultureInfo Culture
        {
            get { return CultureInfo.CurrentUICulture/*null*//*use ResourceManager default, CultureInfo.CurrentUICulture*/; }
        }

        /// <summary>
        /// Gets convenient access to culture-specific resources at runtime.
        /// </summary>
        public static ResourceManager ResourceManager
        {
            get
            {
                return GetLoader().resourceManager;
            }
        }
        #endregion Properties

        #region Public Implementation
        /// <summary>
        /// Provide access to resource string value.
        /// </summary>
        /// <param name="name">Received string name.</param>
        /// <param name="args">Arguments for the String.Format method.</param>
        /// <returns>Returns resources string value or null if error occurred.</returns>
        public static string GetString(string name, params object[] args)
        {
            Resources resourcesInstance = GetLoader();
            if (resourcesInstance == null)
            {
                return null;
            }
            string res = resourcesInstance.resourceManager.GetString(name, Resources.Culture);

            if (args != null && args.Length > 0)
            {
                return String.Format(CultureInfo.CurrentCulture, res, args);
            }
            else
            {
                return res;
            }
        }
        /// <summary>
        /// Provide access to resource string value.
        /// </summary>
        /// <param name="name">Received string name.</param>
        /// <returns>Returns resources string value or null if error occurred.</returns>
        public static string GetString(string name)
        {
            Resources resourcesInstance = GetLoader();

            if (resourcesInstance == null)
            {
                return null;
            }

            try
            {
                return resourcesInstance.resourceManager.GetString(name, Resources.Culture);
            }
            catch (Exception)
            {
                return name;
            }
        }

        /// <summary>
        /// Provide access to resource object value.
        /// </summary>
        /// <param name="name">Received object name.</param>
        /// <returns>Returns resources object value or null if error occurred.</returns>
        public static object GetObject(string name)
        {
            Resources resourcesInstance = GetLoader();

            if (resourcesInstance == null)
            {
                return null;
            }
            return resourcesInstance.resourceManager.GetObject(name, Resources.Culture);
        }
        #endregion Methods

        #region Private Implementation
        private static Resources GetLoader()
        {
            if (loader == null)
            {
                lock (InternalSyncObject)
                {
                    if (loader == null)
                    {
                        loader = new Resources();
                    }
                }
            }
            return loader;
        }
        #endregion
    }
}
