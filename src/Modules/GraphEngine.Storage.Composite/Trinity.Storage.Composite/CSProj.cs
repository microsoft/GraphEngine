using System.Linq;

namespace Trinity.Storage.Composite
{
    internal static class CSProj
    {
        private static string AssemblyReferenceDirectory;
        private static Package[] Packages = new Package[] { new Package("GraphEngine.Core", "1.0.9083"), new Package("Newtonsoft.Json", "9.0.1") };

        public static string _makeReference(string reference) => $@"
    <Reference Include = ""{reference}"">
      <HintPath>{AssemblyReferenceDirectory}\{reference}.dll</HintPath>
    </Reference>";
        public static string _makePackageDependency(Package package) => $@"<PackageReference Include=""{package.Name}"" Version=""{package.Version}"" />
    ";

        public static string TargetFramework => "netstandard2.0";

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
