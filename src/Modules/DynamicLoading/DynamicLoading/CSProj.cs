using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace DynamicLoading
{
    public static class CSProj
    {
        public static string Version = "1.0.9083";
        public static string NetFramework = "netstandard2.0";
        public static string Include;
        public static List<string> TemplateLine => new List<string>{
            "<Project Sdk = \"Microsoft.NET.Sdk\">",
            "   <PropertyGroup>",
            $"       <TargetFramework>{NetFramework}</TargetFramework>",
            "        <AllowUnsafeBlocks>true</AllowUnsafeBlocks>",
            "   </PropertyGroup>",
            "   <ItemGroup>",
            "       <Reference Include = \"GraphEngine.CoreCLR\">",
            $"           <HintPath>{Include}\\GraphEngine.CoreCLR.dll</HintPath>",
            "       </Reference>",
            "       <Reference Include = \"Newtonsoft.Json\">",
            $"            <HintPath>{Include}\\Newtonsoft.Json.dll</HintPath>",
            "       </Reference>",
            "   </ItemGroup>",
            "</Project>"};
    }
}
