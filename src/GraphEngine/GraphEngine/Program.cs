using GraphEngine.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Trinity;
using Trinity.Diagnostics;
using Trinity.Network;
using Trinity.Utilities;

namespace GraphEngine
{
    //https://daveaglick.com/posts/exploring-the-nuget-v3-libraries-part-3
    class Program
    {
        private static void LoadPlugin(Plugin plugin)
        {
            if (plugin.Local != null)
            {
                LoadLocal(plugin.Local);
            }

            if (plugin.Package != null)
            {
                LoadNugetPackage(plugin.Package, plugin.Prerelease);
            }
        }

        private static void LoadLocal(string local)
        {
            if (!Path.IsPathRooted(local))
            {
                local = Path.Combine(PluginConfig.Instance.PluginInstallDirectory, local);
            }
            Log.WriteLine("Loading plugin '{0}'...", Path.GetFileName(local));
            Assembly.LoadFile(local);
        }

        private static void LoadNugetPackage(string packageId, bool prerelease)
        {
            string repo_url = PluginConfig.Instance.PackageRepository;
            Log.WriteLine("Resolving plugin package '{0}' from repository {1}", packageId, repo_url);

            throw new NotImplementedException();
            Log.WriteLine("Installing package {0}", packageId);
        }

        private static void LoadPlugins(PluginConfig instance)
        {
            Log.WriteLine("Scanning for plugins.");
            foreach (var plugin in instance.Plugin)
            {
                LoadPlugin(plugin);
            }
        }

        static void Main(string[] args)
        {
            LoadPlugins(PluginConfig.Instance);
            Global.Initialize();
            TrinityServer server = new TrinityServer();
            server.Start();

            while (true)
            {
                System.Threading.Thread.Sleep(10000);
            }
        }
    }
}
