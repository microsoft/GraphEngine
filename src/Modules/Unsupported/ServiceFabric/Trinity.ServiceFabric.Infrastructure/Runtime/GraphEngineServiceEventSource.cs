// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Threading.Tasks;
using Trinity.Diagnostics;
using Trinity.DynamicCluster;

namespace Trinity.ServiceFabric.Infrastructure
{
    [EventSource(Name = "Microsoft-ServiceFabric.Trinity.StatefulService")]
    public sealed class GraphEngineStatefulServiceEventSource : EventSource
    {
        public static readonly GraphEngineStatefulServiceEventSource Current = new GraphEngineStatefulServiceEventSource();

        static GraphEngineStatefulServiceEventSource()
        {
            // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
            // This problem will be fixed in .NET Framework 4.6.2.
            Task.Run(() => { });
        }

        private Dictionary<LogLevel, Action<string>> m_logprocs = null;
        private string m_logheader = null;
        // Instance constructor is private to enforce singleton semantics
        private GraphEngineStatefulServiceEventSource() : base()
        {
            m_logprocs = new Dictionary<LogLevel, Action<string>>
            {
                { LogLevel.Debug, GraphEngineLogDebug },
                { LogLevel.Error, GraphEngineLogErr },
                { LogLevel.Fatal, GraphEngineLogFatal },
                { LogLevel.Info, GraphEngineLogInfo },
                { LogLevel.Verbose, GraphEngineLogVerbose },
                { LogLevel.Warning, GraphEngineLogWarn },
            };

            TrinityConfig.LogEchoOnConsole = false;
            TrinityConfig.LogToFile = false;
            Log.LogsWritten += GraphEngineLogsWritten;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        [NonEvent]
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            GraphEngineLogFatal($"Unhandled exception: {e.ExceptionObject.ToString()}.");
        }

        [NonEvent]
        private void GraphEngineLogsWritten(IList<LOG_ENTRY> logs)
        {
            var header = EnsureLogHeader();
            foreach (var log in logs)
            {
                string msg = $"{header}{log.logMessage}";
                Action<string> logproc = _ => { };
                m_logprocs.TryGetValue(log.logLevel, out logproc);
                logproc(msg);
            }
        }

        [NonEvent]
        private string EnsureLogHeader()
        {
            if (m_logheader != null) return m_logheader;
            GraphEngineStatefulServiceRuntime rt = null;
            try { rt = GraphEngineStatefulServiceRuntime.Instance; } catch { }
            if (rt == null) return string.Empty;

            m_logheader = $"[P{rt.PartitionId}-{Utils.GenerateNickName(rt.GetInstanceId())}]\t";
            return m_logheader;
        }

        #region Keywords
        // Event keywords can be used to categorize events. 
        // Each keyword is a bit flag. A single event can be associated with multiple keywords (via EventAttribute.Keywords property).
        // Keywords must be defined as a public class named 'Keywords' inside EventSource that uses them.
        public static class Keywords
        {
            public const EventKeywords GraphEngineLog = (EventKeywords)0x1L;
        }
        #endregion

        #region Events
        // Define an instance method for each event you want to record and apply an [Event] attribute to it.
        // The method name is the name of the event.
        // Pass any parameters you want to record with the event (only primitive integer types, DateTime, Guid & string are allowed).
        // Each event method implementation should check whether the event source is enabled, and if it is, call WriteEvent() method to raise the event.
        // The number and types of arguments passed to every event method must exactly match what is passed to WriteEvent().
        // Put [NonEvent] attribute on all methods that do not define an event.
        // For more information see https://msdn.microsoft.com/en-us/library/system.diagnostics.tracing.eventsource.aspx

        private const int GraphEngineLogInfoEventId = 1;
        [Event(GraphEngineLogInfoEventId, Level = EventLevel.Informational, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogInfo(string message)
        {
            WriteEvent(GraphEngineLogInfoEventId, message);
        }

        private const int GraphEngineLogWarnEventId = 2;
        [Event(GraphEngineLogWarnEventId, Level = EventLevel.Warning, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogWarn(string message)
        {
            WriteEvent(GraphEngineLogWarnEventId, message);
        }

        private const int GraphEngineLogErrEventId = 3;
        [Event(GraphEngineLogErrEventId, Level = EventLevel.Error, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogErr(string message)
        {
            WriteEvent(GraphEngineLogErrEventId, message);
        }

        private const int GraphEngineLogDebugEventId = 4;
        [Event(GraphEngineLogDebugEventId, Level = EventLevel.Verbose, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogDebug(string message)
        {
            WriteEvent(GraphEngineLogDebugEventId, message);
        }

        private const int GraphEngineLogFatalEventId = 5;
        [Event(GraphEngineLogFatalEventId, Level = EventLevel.Critical, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogFatal(string message)
        {
            WriteEvent(GraphEngineLogFatalEventId, message);
        }

        private const int GraphEngineLogVerboseEventId = 6;
        [Event(GraphEngineLogVerboseEventId, Level = EventLevel.Verbose, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogVerbose(string message)
        {
            WriteEvent(GraphEngineLogVerboseEventId, message);
        }
        #endregion
    }
}
