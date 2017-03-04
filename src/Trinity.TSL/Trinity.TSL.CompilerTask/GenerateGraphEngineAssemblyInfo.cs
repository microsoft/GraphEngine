using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Trinity.TSL
{
    [Serializable]
    class GraphEngineAssemblyInfoGenerator
    {
        string[] m_scan_targets;
        Dictionary<string, Assembly> m_loaded_assemblies;
        string   m_output;
        public string Output { get { return m_output; } }

        public GraphEngineAssemblyInfoGenerator(string[] references)
        {
            m_scan_targets = references;
            m_output = "";
        }

        private Assembly CurrentDomain_ReflectionOnlyAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return Assembly.ReflectionOnlyLoad(args.Name);
        }

        public void GenerateGraphEngineAssemblyInfo()
        {
            AppDomainSetup setup     = new AppDomainSetup();
            setup.SetConfigurationBytes(AppDomain.CurrentDomain.SetupInformation.GetConfigurationBytes());
            setup.ApplicationBase    = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDomain domain         = AppDomain.CreateDomain("__GraphEngineAssemblyInfoSandBox__", AppDomain.CurrentDomain.Evidence, setup);
            domain.DoCallBack(GenerateGraphEngineAssemblyInfo_impl);
            m_output = (string)domain.GetData("result");
            AppDomain.Unload(domain);
        }

        private void LoadAssemblies()
        {
            List<string> targets = m_scan_targets.ToList();
            int h_cnt = 0;

            while (targets.Count != h_cnt)
            {
                h_cnt = targets.Count;
                foreach (var target in targets.ToList())
                {
                    try
                    {
                        var asm = Assembly.ReflectionOnlyLoadFrom(target);
                        m_loaded_assemblies[target] = asm;
                        targets.Remove(target);
                    }
                    catch { }
                }
            }
        }

        private void GenerateGraphEngineAssemblyInfo_impl()
        {
            m_loaded_assemblies = new Dictionary<string, Assembly>();
            LoadAssemblies();
            AppDomain.CurrentDomain.ReflectionOnlyAssemblyResolve += CurrentDomain_ReflectionOnlyAssemblyResolve;
            StringBuilder writer = new StringBuilder();
            foreach (var str in m_scan_targets)
            {
                try
                {
                    Assembly asm = m_loaded_assemblies[str];
                    Type import_extension_attr_type = asm.GetTypes().FirstOrDefault(t => t.Name == "ImportGraphEngineExtensionAttribute");
                    if (import_extension_attr_type != null)
                        writer.AppendLine(String.Format(CultureInfo.InvariantCulture, "[assembly: {0}.ImportGraphEngineExtension]", import_extension_attr_type.Namespace));
                }
                catch { }
            }
            AppDomain.CurrentDomain.SetData("result", writer.ToString());
        }
    }

    public class GenerateGraphEngineAssemblyInfo : Task
    {
        [Required]
        public string[] ReferencedAssemblies { get; set; }
        [Required]
        public string Output { get; set; }

        public override bool Execute()
        {
            Log.LogMessage("Generating GraphEngine-related assembly information...");
            GraphEngineAssemblyInfoGenerator generator = new GraphEngineAssemblyInfoGenerator(ReferencedAssemblies);
            generator.GenerateGraphEngineAssemblyInfo();
            File.WriteAllText(Path.GetFullPath(Output), generator.Output);

            return true;
        }


    }
}
