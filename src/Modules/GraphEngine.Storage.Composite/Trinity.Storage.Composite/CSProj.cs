using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Trinity.Storage.CompositeExtension
{
    public static class CSProj
    {
        //TODO multi-targeting
        public static string Version = "1.0.9083";
        public static string NetFramework = "netstandard2.0";
        public static string IncludeDirectory; // Need to be configured.
        public static class Reference
        {
            public static string[] Names = new string[] {
                "Trinity.Core",
                "Newtonsoft.Json"
            };
        }

        public static string _makeReference(string reference) => string.Join("\n",
            $"       <Reference Include = \"{reference}\">",
            $"           <HintPath>{IncludeDirectory}\\{reference}.dll</HintPath>",
            "       </Reference>"
        );

        public static string Template => string.Join("\n",
            "<Project Sdk = \"Microsoft.NET.Sdk\">",
            "   <PropertyGroup>",
            $"       <TargetFramework>{NetFramework}</TargetFramework>",
            "        <AllowUnsafeBlocks>true</AllowUnsafeBlocks>",
            "   </PropertyGroup>",
            "   <ItemGroup>",
            string.Join("\n", Reference.Names.Select(_makeReference)),
            "   </ItemGroup>",
            "</Project>");
    }
}
