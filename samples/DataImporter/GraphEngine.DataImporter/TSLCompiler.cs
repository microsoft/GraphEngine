using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;

namespace GraphEngine.DataImporter
{
    class TSLCompiler
    {
        public string Compile(string tslPath)
        {
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo("MSBuild.exe", Path.Combine(exePath, "TSLCompiler.csproj") + " /p:TSLPath=" + tslPath);
            process.Start();
            process.WaitForExit();
            return exePath + "\\bin\\Release\\TSLAssembly.dll";
        }
    }
}
