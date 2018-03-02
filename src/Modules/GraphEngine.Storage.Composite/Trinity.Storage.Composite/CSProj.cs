using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Trinity.Storage.CompositeExtension
{
    public static class CSProj
    {
        private static string IncludeDirectory;
        private static string _makeReference(string reference) => $@"
    <Reference Include = ""{reference}"">
      <HintPath>{IncludeDirectory}\{reference}.dll</HintPath>
    </Reference>";

        private static Package[] Packages = new Package[] { new Package("GraphEngine.Core", "1.0.9083"), new Package("Newtonsoft.Json", "8.0.3") };
        public static string _makePackageDependency(Package package) => $@"<PackageReference Include=""{package.Name}"" Version=""{package.Version}"" />
    ";

        public static string TargetFramework = "netstandard2.0";
        public static string Template => $@"
<Project Sdk = ""Microsoft.NET.Sdk"">
  <PropertyGroup>
    <TargetFramework>{TargetFramework}</TargetFramework>
    <AllowUnsafeBlocks>true</AllowUnsafeBlocks>
  </PropertyGroup>
  <ItemGroup>
    {string.Concat(Packages.Select(_makePackageDependency))}
  </ItemGroup>
</Project>";
    }
}
