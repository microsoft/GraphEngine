using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
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
        public string Compile(string fpath)
        {
            string exePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string s = Path.Combine(exePath, "TSLCompiler.csproj");
            Console.WriteLine(s);
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo("MSBuild.exe", Path.Combine(exePath, "TSLCompiler.csproj") + " /p:TSLRoot=" + fpath);
            process.Start();
            process.WaitForExit();
            return exePath + "\\bin\\Release\\TSLAssembly.dll";
        }
    }
}
