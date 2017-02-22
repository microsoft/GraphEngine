using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Configuration
{
    static class ConfigurationConstants
    {
        public static class Tags
        {
            public const string ROOT_NODE  = "Trinity";
            public const string LOCAL    = "Local";
            public const string IMPORT   = "Import";
            public const string CLUSTER  = "Cluster";
            public const string SERVER   = "Server";
            public const string PROXY    = "Proxy";
            public const string TEMPLATE = "Template";
            public const string LOGGING  = "Logging";
            public const string STORAGE  = "Storage";
            public const string DEFAULT  = "Default";
            public const string NETWORK  = "Network";
            public const string LEGACYVER       = "1.0";
            public const string CURRENTVER      = "2.0";
            public const string DEFAULT_CLUSTER = "";
        }

        public static class Attrs
        {
            public const string FILE               = "File";
            public const string DIRECTORY          = "Directory";
            public const string CONFIG_VERSION     = "ConfigVersion";
            public const string ID                 = "Id";
            public const string TEMPLATE           = "Template";

            public const string LOGGING_LEVEL      = "LoggingLevel";
            public const string LOGGING_DIRECTLY   = "LogDirectory";
            public const string LOG_TO_FILE        = "LogToFile";
            public const string ECHO_ON_CONSILE    = "EchoOnConsole";
            public const string STORAGE_ROOT       = "StorageRoot";
            public const string TRUNK_COUNT = "TrunkCount";
            public const string READ_ONLY = "ReadOnly";
            public const string STORAGE_CAPACITY = "StorageCapacity";
            public const string DEFRAG_INTERVAL = "DefragInterval";
            public const string ENDPOINT           = "Endpoint";
            public const string HTTP_PORT          = "HttpPort";
            public const string CLIENT_MAX_CONN    = "ClientMaxConn";
            public const string ASSEMBLY_PATH      = "AssemblyPath";
            public const string AVAILABILITY_GROUP = "AvailabilityGroup";
        }
    }
}
