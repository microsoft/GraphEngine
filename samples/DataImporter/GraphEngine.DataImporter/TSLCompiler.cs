using System;
using System.Collections.Generic;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;

namespace GraphEngine.DataImporter
{

    class TSLCompiler
    {
        public string Compile(string fpath)
        {
            string path = new DirectoryInfo("../../../").FullName;
            Process process = new Process();
            process.StartInfo = new ProcessStartInfo("MSBuild.exe", path+"TSLCompiler\\TSLCompiler.csproj /p:TSLRoot=" + fpath);
            process.Start();
            process.WaitForExit();
            return path + "TSLCompiler\\bin\\Debug\\GraphEngine.DataImporter.dll";
        }
    }
}
