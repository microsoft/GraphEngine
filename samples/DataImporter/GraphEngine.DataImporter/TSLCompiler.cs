using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Versioning;
using System.Text.RegularExpressions;

namespace GraphEngine.DataImporter
{
    class TSLCompiler
    {
        public string Compile(string tslFilePath)
        {
            var targetFrameWork = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(TargetFrameworkAttribute), false);
            string frameWork = ((TargetFrameworkAttribute)(targetFrameWork[0])).FrameworkName;
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string frameVersion = Regex.Match(frameWork, @"(\d+\.)+\d+").Value; 

            if (frameWork.StartsWith(".NETFramework"))
            {
                string importerProjectPath = Path.Combine(exePath, @"..\..\GraphEngine.DataImporter.csproj");
                string tslProjectPath = Path.Combine(exePath, "TSLCompiler.csproj");

                UpdateProjectVersion(importerProjectPath, tslProjectPath);
                UpdateNugetVersion(importerProjectPath, tslProjectPath, "GraphEngine.Core");
                UpdateNugetVersion(importerProjectPath, tslProjectPath, "Newtonsoft.Json");

                Process process = new Process();
                // Need to add the path of Msbuild.exe to PATH environment variable first.
                process.StartInfo = new ProcessStartInfo("MSBuild.exe", Path.Combine(exePath, "TSLCompiler.csproj") + " /p:TSLPath=" + tslFilePath);
                process.Start();
                process.WaitForExit();
                return exePath + @"\bin\Release\TSLAssembly.dll";
            }
            else if (frameWork.StartsWith(".NETCoreApp"))
            {
                string importerClrProjectPath = Path.Combine(exePath, @"..\..\..\GraphEngine.DataImporter.Clr.csproj");
                string tslClrProjectPath = Path.Combine(exePath, "TSLCompiler.Clr.csproj");

                UpdateProjectVersionClr(importerClrProjectPath, tslClrProjectPath);
                UpdateNugetVersionClr(importerClrProjectPath, tslClrProjectPath, "GraphEngine.CoreCLR");
                UpdateNugetVersionClr(importerClrProjectPath, tslClrProjectPath, "Newtonsoft.Json");

                Process restoreProcess = new Process();
                // Need to add the path of dotnet.exe to PATH environment variable first.
                restoreProcess.StartInfo = new ProcessStartInfo("dotnet.exe", " restore " + Path.Combine(exePath, "TSLcompiler.Clr.csproj"));
                restoreProcess.Start();
                restoreProcess.WaitForExit();

                Process buildProcess = new Process();
                buildProcess.StartInfo = new ProcessStartInfo("dotnet.exe", @" build " + Path.Combine(exePath, "TSLcompiler.Clr.csproj") + @" -c Release /p:TSLPath=" + tslFilePath);
                buildProcess.Start();
                buildProcess.WaitForExit();

                return exePath + @"\bin\Release\netcoreapp" + frameVersion + @"\TSLAssembly.CoreCLR.dll";
            }
            else
            {
                return null;
            }
        }

        private void UpdateProjectVersion(string importerProjectPath, string tslProjectPath)
        {
            Match newVersion = Regex.Match(File.ReadAllText(importerProjectPath), @"<Project ToolsVersion=.*?\..*?>");
            Match oldVersion = Regex.Match(File.ReadAllText(tslProjectPath), @"<Project ToolsVersion=.*?\..*?>");
            string text = File.ReadAllText(tslProjectPath);
            text = text.Replace(oldVersion.Value, newVersion.Value);
            File.WriteAllText(tslProjectPath, text);
        }

        private void UpdateNugetVersion(string importerProjectPath, string tslProjectPath, string packageName)
        {
            Match newVersion = Regex.Match(File.ReadAllText(importerProjectPath), @"\\" + packageName + @".*?\\");
            Match oldVersion = Regex.Match(File.ReadAllText(tslProjectPath), @"\\" + packageName + @".*?\\");
            string text = File.ReadAllText(tslProjectPath);
            text = text.Replace(oldVersion.Value, newVersion.Value);
            newVersion = Regex.Match(File.ReadAllText(importerProjectPath), packageName + @", Version=.*?>");

            if (newVersion.Value == "" && packageName == "GraphEngine.Core")
            {
                packageName = packageName.Replace("GraphEngine", "Trinity");
                newVersion = Regex.Match(File.ReadAllText(importerProjectPath), packageName + @", Version=.*?>");
            }

            oldVersion = Regex.Match(File.ReadAllText(tslProjectPath), packageName + @", Version=.*?>");
            text = text.Replace(oldVersion.Value, newVersion.Value);
            File.WriteAllText(tslProjectPath, text);
        }

        private void UpdateProjectVersionClr(string importerClrProjectPath, string tslClrProjectPath)
        {
            Match newVersion = Regex.Match(File.ReadAllText(importerClrProjectPath), @"<TargetFramework>.*?\..*?</TargetFramework>");
            Match oldVersion = Regex.Match(File.ReadAllText(tslClrProjectPath), @"<TargetFramework>.*?\..*?</TargetFramework>");
            string text = File.ReadAllText(tslClrProjectPath);
            text = text.Replace(oldVersion.Value, newVersion.Value);
            File.WriteAllText(tslClrProjectPath, text);
        }

        private void UpdateNugetVersionClr(string importerClrProjectPath, string tslClrProjectPath,string packageName)
        {
            Match newVersion = Regex.Match(File.ReadAllText(importerClrProjectPath), @"<PackageReference.*?" + packageName + "\" Version=\\S*? />");
            Match oldVersion = Regex.Match(File.ReadAllText(tslClrProjectPath), @"<PackageReference.*?" + packageName + "\" Version=\\S*? />");
            string text = File.ReadAllText(tslClrProjectPath);
            text = text.Replace(oldVersion.Value, newVersion.Value);
            File.WriteAllText(tslClrProjectPath, text);
        }
    }
}
