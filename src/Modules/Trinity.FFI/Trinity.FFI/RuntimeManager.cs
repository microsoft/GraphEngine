using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Trinity.Diagnostics;
using Trinity.Extension;
using Trinity.Network;
using Trinity.Utilities;

[assembly: GraphEngineExtension]

namespace Trinity.FFI
{
    public class RuntimeManager : IStartupTask
    {
        #region Fields
        private List<ILanguageRuntimeProvider> m_providers = new List<ILanguageRuntimeProvider>();
        private Dictionary<string, ProgramRunner> m_runners = new Dictionary<string, ProgramRunner>();
        private FFIModule m_module = null;

        #endregion

        public void Run()
        {
            Global.Uninitialized += _OnGlobalUninitialized;
            Global.CommunicationInstanceStarted += _OnCommunicationInstanceStart;
            Log.WriteLine("Trinity.FFI loaded.");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void _OnCommunicationInstanceStart()
        {
            m_module = Global.CommunicationInstance.GetCommunicationModule<FFIModule>();
            Log.WriteLine("Scanning for foreign runtime providers.");
            m_providers = AssemblyUtility.GetAllClassInstances(t => t.GetConstructor(new Type[] { }).Invoke(new object[] { }) as ILanguageRuntimeProvider);
            foreach (var runtime_provider in m_providers)
            {
                try
                {
                    ProgramRunner runner = new ProgramRunner(runtime_provider, m_module);
                    Log.WriteLine("Discovered foreign runtime provider '{0}'.", runtime_provider.Name);
                    foreach (var format in runtime_provider.SupportedSuffix)
                    {
                        m_runners[format] = runner;
                        Log.WriteLine(LogLevel.Debug, "Use {0} to load *.{1}.", runtime_provider.Name, format);
                    }
                }
                catch (Exception ex)
                {
                    Log.WriteLine(LogLevel.Error, "Failed to load foreign runtime provider '{0}':{1}", runtime_provider.Name, ex.ToString());
                }
            }

            Log.WriteLine("Scanning for FFI Programs.");

            string dir;
            string[] files;

            dir = FFIConfig.Instance.ProgramDirectory;
            dir = FileUtility.CompletePath(dir, create_nonexistent: true);
            files = Directory.GetFiles(dir);

            foreach (var file in files)
            {
                try
                {
                    var suffix = Path.GetExtension(file);
                    if (m_runners.TryGetValue(suffix, out var runner))
                    {
                        Log.WriteLine("Loading program {0} with {1}.", Path.GetFileName(file), runner.RuntimeName);
                        runner.LoadProgram(file);
                    }
                }
                catch { }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        private void _OnGlobalUninitialized()
        {
            foreach (var runner in m_runners)
            {
                runner.Value.Dispose();
            }

            m_runners.Clear();
        }
    }
}
