// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.Extension;
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

        #region Fields
        private bool m_priority_updated = false;
        private Dictionary<Type, int> m_priority_dict = new Dictionary<Type, int>();
        private List<ExtensionPriority> m_priority_list = new List<ExtensionPriority>();
        #endregion

        /// <summary>
        /// Specifies the loading priority of extension classes
        /// </summary>
        [ConfigSetting(Optional: true)]
        public List<ExtensionPriority> Priority { get => m_priority_list; set { m_priority_list = value; m_priority_updated = true; } }

        /// <summary>
        /// Resolve priorities of types specified by this configuration entry.
        /// </summary>
        /// <returns></returns>
        public Dictionary<Type, int> ResolveTypePriorities()
        {
            if (!m_priority_updated) return m_priority_dict;

            Dictionary<Type, int> rank = new Dictionary<Type, int>();
            var defaults = AssemblyUtility.GetAllClassTypes()
                .Select(t => new { Type = t, PriorityAttr = t.GetCustomAttribute<ExtensionPriorityAttribute>(inherit: false) })
                .Where(t => t.PriorityAttr != null);

            foreach(var d in defaults)
            {
                rank.Add(d.Type, d.PriorityAttr.Priority);
            }

            List<ExtensionPriority> priorities = Priority;
            foreach(var p in priorities)
            {
                try { rank.Add(AssemblyUtility.GetType(p.Name), p.Priority); }
                catch { }
            }

            m_priority_dict = rank;
            m_priority_updated = false;
            return rank;
        }
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
