using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Trinity.Configuration
{
    static class ConfigurationConstants
    {
        public static XNamespace NS = XNamespace.Get("https://www.graphengine.io/schema/config");
        public static class Tags
        {
            // legacy tags are namespace-agnostic
            public const string          LEGACY_ENTRY_LABEL         = "entry";
            public const string          LEGACY_SECTION_LABEL       = "section";
            public const string          LEGACY_NAME                = "name";

            public static readonly XName ROOT_NODE                  = NS + "Trinity";
            public static readonly XName LOCAL                      = NS + "Local";
            public static readonly XName IMPORT                     = NS + "Import";
            public static readonly XName CLUSTER                    = NS + "Cluster";
            public static readonly XName SERVER                     = NS + "Server";
            public static readonly XName PROXY                      = NS + "Proxy";
            public static readonly XName TEMPLATE                   = NS + "Template";
            public static readonly XName LOGGING                    = NS + "Logging";
            public static readonly XName EXTENSION                  = NS + "Extension";
            public static readonly XName STORAGE                    = NS + "Storage";
            public static readonly XName DEFAULT                    = NS + "Default";
            public static readonly XName NETWORK                    = NS + "Network";
        }
        public static class Values
        {
            public const int    DEFAULT_BACKGROUND_SENDING_INTERVAL = 10;//ms
            public const int    DEFAULT_HEARTBEAT_INTERVAL          = 1000;//ms
            public const int    DEFAULT_MAXSOCKET_RECONNECTNUM      = 8;//ms
            public const int    DEFAULT_INVALID_VALUE               = -1;
            public const int    DEFAULT_SERVER_PORT                 = 5304;
            public const int    DEFAULT_PROXY_PORT                  = 7304;
            public const int    MAX_TRUNK_COUNT                     = 256;
            public const bool   DEFAULT_VALUE_FALSE                 = false;
            public const bool   DEFAULT_VALUE_TRUE                  = true;
            public const ushort UNDEFINED_CELL_TYPE                 = 0;
            public const int    DEFAULT_DEFRAG_INTERVAL             = 600;
            public const int    DEFALUT_GC_PATRALLELISM             = 16;
            public const string BLANK                               = "";
            public const int    DEFAULT_HTTP_PORT                   = 80;
            public const int    DEFAULT_CLIENT_MAX_CONN             = 2;
            public const int    DEFAULT_CLIENT_SEND_RETRY           = 1;
            public const int    DEFAULT_CLIENT_RECONNECT_RETRY      = 1;
            public const string DEFAULT_CLUSTER                     = "";
            public const string DEFAULT_CONFIG_FILE                 = "trinity.xml";
            public const string LEGACYVER                           = "1.0";
            public const string CURRENTVER                          = "2.0";
        }
        public static class Attrs
        {
            public const string FILE                                = "File";
            public const string DIRECTORY                           = "Directory";
            public const string CONFIG_VERSION                      = "ConfigVersion";
            public const string ID                                  = "Id";
            public const string RUNNINGMODE                         = "RunningMode";
            public const string TEMPLATE                            = "Template";

            public const string LOGGING_LEVEL                       = "LoggingLevel";
            public const string LOGGING_DIRECTORY                   = "LogDirectory";
            public const string LOG_TO_FILE                         = "LogToFile";
            public const string ECHO_ON_CONSILE                     = "EchoOnConsole";
            public const string STORAGE_ROOT                        = "StorageRoot";
            public const string TRUNK_COUNT                         = "TrunkCount";
            public const string READ_ONLY                           = "ReadOnly";
            public const string STORAGE_CAPACITY                    = "StorageCapacity";
            public const string DEFRAG_INTERVAL                     = "DefragInterval";
            public const string ENDPOINT                            = "Endpoint";
            public const string HTTP_PORT                           = "HttpPort";
            public const string CLIENT_MAX_CONN                     = "ClientMaxConn";
            public const string ASSEMBLY_PATH                       = "AssemblyPath";
            public const string LEGACY_ASSEMBLY_PATH                = "WorkingDirectory";
            public const string AVAILABILITY_GROUP                  = "AvailabilityGroup";
        }
    }
}
