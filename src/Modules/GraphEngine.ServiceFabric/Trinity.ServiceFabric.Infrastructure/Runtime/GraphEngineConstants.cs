// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
namespace Trinity.ServiceFabric.Infrastructure
{
    public static class GraphEngineConstants
    {
        public const string TrinityProtocolEndpoint        = @"TrinityProtocolEndpoint";
        public const string TrinityHttpProtocolEndpoint    = @"TrinityHttpEndpoint";
        public const string TrinityWCFProtocolEndpoint     = @"TrinityWCFEndpoint";
        public const string TrinitygRPCProtocolEndpoint    = @"TrinitygRPCEndpoint";
        public const string TrinityOdataProtocolEndpoint   = @"TrinityODataEndpoint";
        public const string TrinityWebApiProtocolEndpoint  = @"TrinityWebApiEndpoint";
        public const string LocalAvailabilityGroup         = @"LOCAL";
        public const string RemoteAvailabilityGroup        = @"REMOTE";
        public const string AvailabilityGroupLocalHost     = @"localhost";
        public const string GraphEngineHttpListenerName    = @"GraphEngineHttpListener";
        public const string GraphEngineListenerName        = @"GraphEngineListener";
        public const string GraphEngineWCFListenerName     = @"GraphEngineWCFListener";
        public const string GraphEnginegRPCListenerName    = @"GraphEnginegRPCListener";
        public const string GraphEngineODataListenerName   = @"GraphEngineODataListener";
        public const string ServiceFabricConfigParameter   = @"ConfigFile";
        public const string ServiceFabricConfigSection     = @"TrinityConfig";
    }
}