// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System.Net;
using Newtonsoft.Json;
using System.IO;
using Trinity.Modules.Spark.Protocols.TSL;

namespace Trinity.Modules.Spark
{
    public class SparkTrinityModule : SparkTrinityBase
    {
        public ISparkTrinityConnector SparkTrinityConnector { get; set; } = new DefaultSparkTrinityConnector();

        public override string GetModuleName()
        {
            return "Spark";
        }

        public override void GetSchemaHandler(RequestBodyStruct request, HttpListenerResponse response)
        {
            var schema = SparkTrinityConnector.GetSchema(request.jsonBody);
            var jsonResponse = JsonConvert.SerializeObject(schema);
            using (var sw = new StreamWriter(response.OutputStream))
            {
                sw.WriteLine(jsonResponse);
            }
        }

        public override void GetPartitionsHandler(RequestBodyStruct request, HttpListenerResponse response)
        {
            var partitions = SparkTrinityConnector.GetPartitions(request.jsonBody);
            var jsonResponse = JsonConvert.SerializeObject(partitions);
            using (var sw = new StreamWriter(response.OutputStream))
            {
                sw.WriteLine(jsonResponse);
            }
        }

        public override void GetPartitionHandler(RequestBodyStruct request, HttpListenerResponse response)
        {
            var cells = SparkTrinityConnector.GetPartition(request.jsonBody);
            var jsonResponse = JsonConvert.SerializeObject(cells);
            using (var sw = new StreamWriter(response.OutputStream))
            {
                sw.WriteLine(jsonResponse);
            }
        }
    }
}
