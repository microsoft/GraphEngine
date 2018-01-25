// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Utilities;

namespace Trinity.Configuration
{
    /// <summary>
    /// Contains settings for the configuration section "Extension".
    /// </summary>
    public sealed class ExtensionConfig
    {
        #region Singleton
        static ExtensionConfig s_instance = new ExtensionConfig();
        private ExtensionConfig() { Priority = new List<ExtensionPriority>(); }
        /// <summary>
        /// Gets the configuration entry singleton instance.
        /// </summary>
        [ConfigInstance]
        public static ExtensionConfig Instance { get { return s_instance; } }
        [ConfigEntryName]
        internal static string ConfigEntry { get { return ConfigurationConstants.Tags.EXTENSION.LocalName; } }
        #endregion

        /// <summary>
        /// Specifies the loading priority of extension classes
        /// </summary>
        [ConfigSetting(Optional: true)]
        public List<ExtensionPriority> Priority {get; set;}
    }

    /// <summary>
    /// Specifies the loading priority of an extension class
    /// </summary>
    public sealed class ExtensionPriority
    {
        /// <summary>
        /// The name of the extension class
        /// </summary>
        [ConfigSetting(Optional: false)]
        public string Name { get; set; }

        /// <summary>
        /// The priority of the extension class
        /// </summary>
        [ConfigSetting(Optional: false)]
        public int Priority { get; set; }
    }
}
