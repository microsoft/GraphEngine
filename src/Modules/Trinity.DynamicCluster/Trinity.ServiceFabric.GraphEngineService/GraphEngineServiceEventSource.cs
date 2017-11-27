using System;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Fabric;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Runtime;
using Trinity.Diagnostics;

namespace Trinity.ServiceFabric
{
    [EventSource(Name = "MyCompany-Trinity.ServiceFabric.SmokeTest-Trinity.ServiceFabric.GraphEngineService")]
    internal sealed class GraphEngineServiceEventSource : EventSource
    {
        public static readonly GraphEngineServiceEventSource Current = new GraphEngineServiceEventSource();

        static GraphEngineServiceEventSource()
        {
            // A workaround for the problem where ETW activities do not get tracked until Tasks infrastructure is initialized.
            // This problem will be fixed in .NET Framework 4.6.2.
            Task.Run(() => { });
        }

        // Instance constructor is private to enforce singleton semantics
        private GraphEngineServiceEventSource() : base()
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

            TrinityConfig.LogToFile = false;
            Log.LogsWritten += GraphEngineLogsWritten;
            AppDomain.CurrentDomain.UnhandledException += OnUnhandledException;
        }

        [NonEvent]
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.WriteLine(LogLevel.Fatal, "{0}", $"Unhandled exception: {e.ExceptionObject.ToString()}.");
        }

        private Dictionary<LogLevel, Action<string>> m_logprocs = null;
        [NonEvent]
        private void GraphEngineLogsWritten(IList<LOG_ENTRY> logs)
        {
            foreach (var log in logs)
            {
                string msg = $"[{log.logTime}]\t{log.logMessage}";
                Action<string> logproc = _ => { };
                m_logprocs.TryGetValue(log.logLevel, out logproc);
                logproc(msg);
            }
        }

        #region Keywords
        // Event keywords can be used to categorize events. 
        // Each keyword is a bit flag. A single event can be associated with multiple keywords (via EventAttribute.Keywords property).
        // Keywords must be defined as a public class named 'Keywords' inside EventSource that uses them.
        public static class Keywords
        {
            public const EventKeywords Requests = (EventKeywords)0x1L;
            public const EventKeywords ServiceInitialization = (EventKeywords)0x2L;
            public const EventKeywords GraphEngineLog = (EventKeywords)0x4L;
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

        [NonEvent]
        public void Message(string message, params object[] args)
        {
            if (this.IsEnabled())
            {
                string finalMessage = string.Format(message, args);
                Message(finalMessage);
            }
        }

        private const int MessageEventId = 1;
        [Event(MessageEventId, Level = EventLevel.Informational, Message = "{0}")]
        public void Message(string message)
        {
            if (this.IsEnabled())
            {
                WriteEvent(MessageEventId, message);
            }
        }

        [NonEvent]
        public void ServiceMessage(StatefulServiceContext serviceContext, string message, params object[] args)
        {
            if (this.IsEnabled())
            {
                string finalMessage = string.Format(message, args);
                ServiceMessage(
                    serviceContext.ServiceName.ToString(),
                    serviceContext.ServiceTypeName,
                    serviceContext.ReplicaId,
                    serviceContext.PartitionId,
                    serviceContext.CodePackageActivationContext.ApplicationName,
                    serviceContext.CodePackageActivationContext.ApplicationTypeName,
                    serviceContext.NodeContext.NodeName,
                    finalMessage);
            }
        }

        // For very high-frequency events it might be advantageous to raise events using WriteEventCore API.
        // This results in more efficient parameter handling, but requires explicit allocation of EventData structure and unsafe code.
        // To enable this code path, define UNSAFE conditional compilation symbol and turn on unsafe code support in project properties.
        private const int ServiceMessageEventId = 2;
        [Event(ServiceMessageEventId, Level = EventLevel.Informational, Message = "{7}")]
        private
#if UNSAFE
        unsafe
#endif
        void ServiceMessage(
            string serviceName,
            string serviceTypeName,
            long replicaOrInstanceId,
            Guid partitionId,
            string applicationName,
            string applicationTypeName,
            string nodeName,
            string message)
        {
#if !UNSAFE
            WriteEvent(ServiceMessageEventId, serviceName, serviceTypeName, replicaOrInstanceId, partitionId, applicationName, applicationTypeName, nodeName, message);
#else
            const int numArgs = 8;
            fixed (char* pServiceName = serviceName, pServiceTypeName = serviceTypeName, pApplicationName = applicationName, pApplicationTypeName = applicationTypeName, pNodeName = nodeName, pMessage = message)
            {
                EventData* eventData = stackalloc EventData[numArgs];
                eventData[0] = new EventData { DataPointer = (IntPtr) pServiceName, Size = SizeInBytes(serviceName) };
                eventData[1] = new EventData { DataPointer = (IntPtr) pServiceTypeName, Size = SizeInBytes(serviceTypeName) };
                eventData[2] = new EventData { DataPointer = (IntPtr) (&replicaOrInstanceId), Size = sizeof(long) };
                eventData[3] = new EventData { DataPointer = (IntPtr) (&partitionId), Size = sizeof(Guid) };
                eventData[4] = new EventData { DataPointer = (IntPtr) pApplicationName, Size = SizeInBytes(applicationName) };
                eventData[5] = new EventData { DataPointer = (IntPtr) pApplicationTypeName, Size = SizeInBytes(applicationTypeName) };
                eventData[6] = new EventData { DataPointer = (IntPtr) pNodeName, Size = SizeInBytes(nodeName) };
                eventData[7] = new EventData { DataPointer = (IntPtr) pMessage, Size = SizeInBytes(message) };

                WriteEventCore(ServiceMessageEventId, numArgs, eventData);
            }
#endif
        }

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

        private const int GraphEngineLogInfoEventId = 7;
        [Event(GraphEngineLogInfoEventId, Level = EventLevel.Informational, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogInfo(string message)
        {
            WriteEvent(GraphEngineLogInfoEventId, message);
        }

        private const int GraphEngineLogWarnEventId = 8;
        [Event(GraphEngineLogWarnEventId, Level = EventLevel.Warning, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogWarn(string message)
        {
            WriteEvent(GraphEngineLogWarnEventId, message);
        }

        private const int GraphEngineLogErrEventId = 9;
        [Event(GraphEngineLogErrEventId, Level = EventLevel.Error, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogErr(string message)
        {
            WriteEvent(GraphEngineLogErrEventId, message);
        }

        private const int GraphEngineLogDebugEventId = 10;
        [Event(GraphEngineLogDebugEventId, Level = EventLevel.Verbose, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogDebug(string message)
        {
            WriteEvent(GraphEngineLogDebugEventId, message);
        }

        private const int GraphEngineLogFatalEventId = 11;
        [Event(GraphEngineLogFatalEventId, Level = EventLevel.Critical, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogFatal(string message)
        {
            WriteEvent(GraphEngineLogFatalEventId, message);
        }

        private const int GraphEngineLogVerboseEventId = 12;
        [Event(GraphEngineLogVerboseEventId, Level = EventLevel.Verbose, Message = "{0}", Keywords = Keywords.GraphEngineLog)]
        public void GraphEngineLogVerbose(string message)
        {
            WriteEvent(GraphEngineLogVerboseEventId, message);
        }
        #endregion

        #region Private methods
#if UNSAFE
        private int SizeInBytes(string s)
        {
            if (s == null)
            {
                return 0;
            }
            else
            {
                return (s.Length + 1) * sizeof(char);
            }
        }
#endif
        #endregion
    }
}
