// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using FanoutSearch.Protocols.TSL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL.Lib;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Trinity.Diagnostics;
using Trinity.Network.Http;
using System.Globalization;

namespace FanoutSearch
{
    public partial class FanoutSearchModule : FanoutSearchBase
    {
        /// <summary>
        /// Perform a JSON-syntax query
        /// </summary>
        /// <param name="queryString">The Json string representing the query object, which contains the query path, and traverse action objects.</param>
        /// <param name="queryPath">When provided, overrides the query path in the query object.</param>
        /// <returns>A json string representing query results.</returns>
        /// <exception cref="FanoutSearchQueryException">Throws if the query string is invalid.</exception>
        /// <exception cref="FanoutSearchQueryTimeoutException">Throws if the query timed out, and the server is set to not respond with partial result.</exception>"
        public string JsonQuery(string queryString, string queryPath = "")
        {
            JObject queryObject = null;

            try { queryObject = JsonConvert.DeserializeObject<JObject>(queryString); } catch { }

            if (queryObject == null) { throw new FanoutSearchQueryException("The input is not a valid Json object."); }

            if (QueryPathInvalid(queryPath))
            {
                JToken queryPath_token = null;
                if (!queryObject.TryGetValue(JsonDSL.Path, out queryPath_token)) { throw new FanoutSearchQueryException("Query path not found"); }
                queryPath = queryPath_token.ToString();
                if (QueryPathInvalid(queryPath)) { throw new FanoutSearchQueryException("Invalid query path"); }
            }

            FanoutSearchDescriptor fanoutSearch_desc;
            fanoutSearch_desc = new FanoutSearchDescriptor(queryPath, queryObject);
            return PathsToJsonArray(fanoutSearch_desc, getEdgeType: false);
        }

        /// <summary>
        /// Perform a Lambda-syntax query
        /// </summary>
        /// <param name="queryString">The query string, in LIKQ syntax.</param>
        /// <param name="getEdgeType">Get edge types, and interleave nodes and edge type arrays in the query result.</param>
        /// <returns>A json string representing query results.</returns>
        /// <exception cref="FanoutSearchQueryException">Throws if the query string is invalid.</exception>
        /// <exception cref="FanoutSearchQueryTimeoutException">Throws if the query timed out, and the server is set to not respond with partial result.</exception>"
        public string LambdaQuery(string queryString, bool getEdgeType = false)
        {
            FanoutSearchDescriptor query_object;

            query_object = LambdaDSL.Evaluate(queryString);

            return PathsToJsonArray(query_object, getEdgeType);
        }

        #region Handlers
        public override void JsonQueryHandler(HttpListenerRequest request, HttpListenerResponse response)
        {
            const string queryPath_prefix = "/JsonQuery";
            string       queryPath;
            string       queryResultPaths;

            queryPath = request.RawUrl;
            queryPath = queryPath.Substring(queryPath.IndexOf(queryPath_prefix, StringComparison.Ordinal) + queryPath_prefix.Length);

            using (var sr = new StreamReader(request.InputStream, request.ContentEncoding))
            {
                string input_stream_content = sr.ReadToEnd();
                queryResultPaths = JsonQuery(input_stream_content, queryPath);
            }

            using (var sw = new StreamWriter(response.OutputStream))
            {
                sw.Write(queryResultPaths);
            }
        }

        public override void GetNodesInfoHandler(GetNodesInfoRequest request, System.Net.HttpListenerResponse response)
        {
            var writers = Enumerable.Range(0, Global.CloudStorage.ServerCount).Select(i => new GetNodesInfoRequestWriter(fields: new List<string> {  "type_object_name" })).ToArray();

            foreach (var id in request.ids)
                writers[Global.CloudStorage.GetServerIdByCellId(id)].ids.Add(id);

            var readers = writers.Select((writer, server) => _GetNodesInfo_impl(server, writer)).ToArray();

            var results = readers.Aggregate(
                (IEnumerable<NodeInfo>)new List<NodeInfo>(),
                (_, reader) =>
                    Enumerable.Concat(_, reader.infoList.Select(infoAccessor => (NodeInfo)infoAccessor)));

            string result_string = "[" + string.Join(",", results.Select(_ =>
                                            string.Format(CultureInfo.InvariantCulture, @"{{""CellID"": {0}, ""type_object_name"": {1}}}",
                                                _.id,
                                                JsonStringProcessor.escape(_.values.First())))) + "]";

            using (var sw = new StreamWriter(response.OutputStream))
            {
                sw.Write(result_string);
            }
        }

        public override void ExternalQueryHandler(StringStruct request, out ExternalQueryResponse response)
        {
            Log.WriteLine(LogLevel.Info, "Processing external query.");

            if (!s_enable_external_query)
            {
                throw new Exception("External queray not enabled.");
            }

            response.result = LambdaQuery(request.queryString, true);
        }
        #endregion // Handlers

        #region helpers
        private string PathsToJsonArray(FanoutSearchDescriptor search, bool getEdgeType)
        {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            sb.Append('[');
            foreach (var path in search)
            {
                if (first)
                    first = false;
                else
                    sb.Append(',');
                OutputPath(path, sb, getEdgeType);
            }
            sb.Append(']');
            return sb.ToString();
        }

        private void OutputPath(PathDescriptor path, StringBuilder sb, bool getEdgeType)
        {
            if (!getEdgeType)
            {
                path.Serialize(sb);
                return;
            }

            bool first = true;
            sb.Append('[');
            long last_id = 0;
            using (var edgeWriter = new EdgeStructWriter(0, 0))
            {
                foreach (var node in path)
                {
                    if (first)
                        first = false;
                    else
                    {
                        sb.Append(',');
                        edgeWriter.from = last_id;
                        edgeWriter.to = node.id;
                        using (var type_reader = GetEdgeType(Global.CloudStorage.GetServerIdByCellId(last_id), edgeWriter))
                        {
                            sb.Append(type_reader.queryString);
                        }
                        sb.Append(',');
                    }

                    sb.Append(node);
                    last_id = node.id;
                }
            }
            sb.Append(']');
        }

        private static bool QueryPathInvalid(string queryPath)
        {
            return queryPath == null || queryPath == "/" || queryPath == String.Empty;
        }
        #endregion
    }
}
