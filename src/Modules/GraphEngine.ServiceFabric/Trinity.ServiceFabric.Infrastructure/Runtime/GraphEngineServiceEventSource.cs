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

        //static GraphEngineStatefulServiceEventSource()
        //{
        //    // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
        //    // This problem will be fixed in .NET Framework 4.6.2.
        //    Task.Run(() => { });
        //}

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
            public const EventKeywords GraphEngineLog        = (EventKeywords)0x1L;
            public const EventKeywords Requests              = (EventKeywords)0x4L;
            public const EventKeywords ServiceInitialization = (EventKeywords)0x2L;
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

        private const int ServiceTypeRegisteredEventId = 3;
        [Event(ServiceTypeRegisteredEventId, Level = EventLevel.Informational, Message = "Service host process {0} registered service type {1}", Keywords = Keywords.ServiceInitialization)]
        public void ServiceTypeRegistered(int hostProcessId, string serviceType)
        {
            WriteEvent(ServiceTypeRegisteredEventId, hostProcessId, serviceType);
        }

        private const int ServiceHostInitializationFailedEventId = 4;
        [Event(ServiceHostInitializationFailedEventId, Level = EventLevel.Error, Message = "Service host initialization failed", Keywords = Keywords.ServiceInitialization)]
        public void ServiceHostInitializationFailed(string exception)
        {
            WriteEvent(ServiceHostInitializationFailedEventId, exception);
        }

        // A pair of events sharing the same name prefix with a "Start"/"Stop" suffix implicitly marks boundaries of an event tracing activity.
        // These activities can be automatically picked up by debugging and profiling tools, which can compute their execution time, child activities,
        // and other statistics.
        private const int ServiceRequestStartEventId = 5;
        [Event(ServiceRequestStartEventId, Level = EventLevel.Informational, Message = "Service request '{0}' started", Keywords = Keywords.Requests)]
        public void ServiceRequestStart(string requestTypeName)
        {
            WriteEvent(ServiceRequestStartEventId, requestTypeName);
        }

        private const int ServiceRequestStopEventId = 6;
        [Event(ServiceRequestStopEventId, Level = EventLevel.Informational, Message = "Service request '{0}' finished", Keywords = Keywords.Requests)]
        public void ServiceRequestStop(string requestTypeName, string exception = "")
        {
            WriteEvent(ServiceRequestStopEventId, requestTypeName, exception);
        }

        private const int GraphEngineLogInfoEventId = 8;
        [Event(GraphEngineLogInfoEventId, Level = EventLevel.Informational, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogInfo(string message)
        {
            WriteEvent(GraphEngineLogInfoEventId, message);
        }

        private const int GraphEngineLogWarnEventId = 9;
        [Event(GraphEngineLogWarnEventId, Level = EventLevel.Warning, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogWarn(string message)
        {
            WriteEvent(GraphEngineLogWarnEventId, message);
        }

        private const int GraphEngineLogErrEventId = 10;
        [Event(GraphEngineLogErrEventId, Level = EventLevel.Error, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogErr(string message)
        {
            WriteEvent(GraphEngineLogErrEventId, message);
        }

        private const int GraphEngineLogDebugEventId = 11;
        [Event(GraphEngineLogDebugEventId, Level = EventLevel.Verbose, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogDebug(string message)
        {
            WriteEvent(GraphEngineLogDebugEventId, message);
        }

        private const int GraphEngineLogFatalEventId = 12;
        [Event(GraphEngineLogFatalEventId, Level = EventLevel.Critical, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogFatal(string message)
        {
            WriteEvent(GraphEngineLogFatalEventId, message);
        }

        private const int GraphEngineLogVerboseEventId = 13;
        [Event(GraphEngineLogVerboseEventId, Level = EventLevel.Verbose, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogVerbose(string message)
        {
            WriteEvent(GraphEngineLogVerboseEventId, message);
        }
        #endregion
    }
}
