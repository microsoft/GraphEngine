using System;
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

                UpdateProjectVersion(importerProjectPath, tslProjectPath, @"<Project ToolsVersion=(.*?\..*?)+>");
                UpdateNugetVersion(importerProjectPath, tslProjectPath, "GraphEngine.Core");
                UpdateNugetVersion(importerProjectPath, tslProjectPath, "Newtonsoft.Json");

                Process process = new Process();
                // Need to add the path of Msbuild.exe to PATH environment variable first.
                process.StartInfo = new ProcessStartInfo("MSBuild.exe", Path.Combine(exePath, "TSLCompiler.csproj") + " /p:TSLPath=" + tslFilePath + " /flp:logfile=" + Path.Combine(exePath, "errors.log") + ";errorsonly");
                process.Start();
                process.WaitForExit();

                if (File.Exists(Path.Combine(exePath, "errors.log")))
                {
                    string errorMessage = File.ReadAllText(Path.Combine(exePath, "errors.log"));
                    Console.WriteLine("TSL File Compile Error:"); 
                    ColoredConsoleWrite(ConsoleColor.Red , errorMessage);
                    File.Delete(Path.Combine(exePath, "errors.log"));
                    return null;
                }
                else
                {
                    return exePath + @"\bin\Release\TSLAssembly.dll";
                }
            }
            else if (frameWork.StartsWith(".NETCoreApp"))
            {
                string importerClrProjectPath = Path.Combine(exePath, @"..\..\..\GraphEngine.DataImporter.Clr.csproj");
                string tslClrProjectPath = Path.Combine(exePath, "TSLCompiler.Clr.csproj");

                UpdateProjectVersion(importerClrProjectPath, tslClrProjectPath, @"<TargetFramework>(.*?\..*?)+</TargetFramework>");
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

                if (File.Exists(exePath + @"\bin\Release\netcoreapp" + frameVersion + @"\TSLAssembly.CoreCLR.dll"))
                {
                    return exePath + @"\bin\Release\netcoreapp" + frameVersion + @"\TSLAssembly.CoreCLR.dll";
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void UpdateProjectVersion(string importerProjectPath, string tslProjectPath, string pattern)
        {
            Match newVersion = Regex.Match(File.ReadAllText(importerProjectPath), pattern);
            Match oldVersion = Regex.Match(File.ReadAllText(tslProjectPath), pattern);
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

        private void UpdateNugetVersionClr(string importerClrProjectPath, string tslClrProjectPath, string packageName)
        {
            Match newVersion = Regex.Match(File.ReadAllText(importerClrProjectPath), @"<PackageReference.*?" + packageName + "\" Version=\\S*? />");
            Match oldVersion = Regex.Match(File.ReadAllText(tslClrProjectPath), @"<PackageReference.*?" + packageName + "\" Version=\\S*? />");
            string text = File.ReadAllText(tslClrProjectPath);
            text = text.Replace(oldVersion.Value, newVersion.Value);
            File.WriteAllText(tslClrProjectPath, text);
        }
        private static void ColoredConsoleWrite(ConsoleColor color, string text)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = originalColor;
        }
    }
}
