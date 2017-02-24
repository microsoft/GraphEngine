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
using Trinity.Core.Lib;
using Trinity.Storage;
using Trinity.Utilities;


namespace Trinity.Configuration
{
    internal sealed class LocalConfig
    {
        static LocalConfig localConfig = new LocalConfig();
        private LocalConfig() { template = TemplateConfig.Instance.Id; }
        [ConfigInstance]
        internal static LocalConfig Instance { get { return localConfig; } }
        [ConfigEntryName]
        internal static string ConfigEntry { get { return "Local"; } }

        private int template = -1;

        [ConfigSetting(Optional: true)]
        public int Template
        {
            get { return template; }
            set { TemplateConfig.Instance.Id = value; }
        }
    }
}
