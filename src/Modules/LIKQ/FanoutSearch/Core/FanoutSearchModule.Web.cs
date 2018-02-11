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
using Trinity.Network.Messaging;
using System.Threading;

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
        /// <exception cref="Trinity.Network.Messaging.MessageTooLongException">Throws if the query response is too big, and the server is set to not respond with partial result.</exception>
        public string JsonQuery(string queryString, string queryPath = "")
        {
            FanoutSearchDescriptor fanoutSearch_desc = _JsonQuery_impl(queryString, queryPath);
            StringWriter sw = new StringWriter();
            _SerializePaths(fanoutSearch_desc, sw);
            return sw.GetStringBuilder().ToString();
        }

        /// <summary>
        /// Perform a Lambda-syntax query
        /// </summary>
        /// <param name="lambda">The query string, in LIKQ syntax.</param>
        /// <param name="getEdgeType">Get edge types, and interleave nodes and edge type arrays in the query result.</param>
        /// <returns>A json string representing query results.</returns>
        /// <exception cref="FanoutSearchQueryException">Throws if the query string is invalid.</exception>
        /// <exception cref="FanoutSearchQueryTimeoutException">Throws if the query timed out, and the server is set to not respond with partial result.</exception>"
        /// <exception cref="Trinity.Network.Messaging.MessageTooLongException">Throws if the query response is too big, and the server is set to not respond with partial result.</exception>
        public string LambdaQuery(string lambda)
        {
            FanoutSearchDescriptor fanoutSearch_desc = _LambdaQuery_impl(lambda);
            StringWriter sw = new StringWriter();
            _SerializePaths(fanoutSearch_desc, sw);
            return sw.GetStringBuilder().ToString();
        }

        private static FanoutSearchDescriptor _JsonQuery_impl(string queryString, string queryPath)
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
            Log.WriteLine(LogLevel.Debug, "{0}", $"Json query: {queryString} \r\n QueryPath = {queryPath}");

            try
            {
                FanoutSearchDescriptor fanoutSearch_desc = new FanoutSearchDescriptor(queryPath, queryObject);
                return fanoutSearch_desc;
            }
            catch (Exception ex)
            {
                throw new FanoutSearchQueryException("Error parsing the json query object", ex);
            }
        }

        private static FanoutSearchDescriptor _LambdaQuery_impl(string lambda)
        {
            Log.WriteLine(LogLevel.Debug, "{0}", $"Lambda query: {lambda}");
            try
            {
                return LambdaDSL.Evaluate(lambda);
            }
            catch (Exception ex)
            {
                throw new FanoutSearchQueryException("Error parsing the lambda query object", ex);
            }
        }

        #region Handlers
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

        public override void JsonQueryHandler(HttpListenerRequest request, HttpListenerResponse response)
        {
            const string queryPath_prefix = "/JsonQuery";
            string       queryPath;
            FanoutSearchDescriptor queryResultPaths;

            queryPath = request.RawUrl;
            queryPath = queryPath.Substring(queryPath.IndexOf(queryPath_prefix, StringComparison.Ordinal) + queryPath_prefix.Length);
            try
            {
                using (var sr = new StreamReader(request.InputStream, request.ContentEncoding))
                {
                    string input_stream_content = sr.ReadToEnd();
                    queryResultPaths = _JsonQuery_impl(input_stream_content, queryPath);
                }

                _WriteResults(response, queryResultPaths);
            }
            catch (FanoutSearch.FanoutSearchQueryTimeoutException timeout)
            {
                //  TODO TimeoutException in GraphEngine.Core
                throw new BadRequestException("Timeout", timeout.Message);
            }
            catch (FanoutSearch.FanoutSearchQueryException badRequest)
            {
                throw new BadRequestException("BadArgument", badRequest.Message);
            }
            catch (MessageTooLongException msgex)
            {
                throw new BadRequestException("MessageTooLong", msgex.Message);
            }
        }

        public override void LambdaQueryHandler(LambdaQueryInput request, HttpListenerResponse response)
        {
            Log.WriteLine(LogLevel.Info, "Processing lambda query.");

            if (!s_enable_external_query)
            {
                throw new Exception("Lambda queray not enabled.");
            }

            try
            {
                var queryResultPaths = _LambdaQuery_impl(request.lambda);
                _WriteResults(response, queryResultPaths);
            }
            catch (FanoutSearch.FanoutSearchQueryTimeoutException timeout)
            {
                throw new BadRequestException("Timeout", timeout.Message);
            }
            catch (FanoutSearch.FanoutSearchQueryException badRequest)
            {
                throw new BadRequestException("BadArgument", badRequest.Message);
            }
            catch (MessageTooLongException msgex)
            {
                throw new BadRequestException("MessageTooLong", msgex.Message);
            }
        }
        #endregion // Handlers

        #region helpers
        // queryResultPaths represents a serialized json array, so we just concatenate it with
        // a wrapper
        private static void _WriteResults(HttpListenerResponse response, FanoutSearchDescriptor queryResultPaths)
        {
            using (var sw = new StreamWriter(response.OutputStream))
            {
                sw.WriteLine(@"{
    ""Results"":");
                _SerializePaths(queryResultPaths, sw);
                sw.WriteLine(@"
}");
            }
        }

        private static void _SerializePaths(FanoutSearchDescriptor search, TextWriter writer)
        {
            long len = 0;
            try
            {
                var paths = search.ToList().AsParallel().Select(p =>
                {
                    StringBuilder builder = new StringBuilder();
                    p.Serialize(builder);
                    var newlen = Interlocked.Add(ref len, builder.Length);
                    if (newlen > s_max_rsp_size) throw new MessageTooLongException();
                    return builder.ToString();
                });
                bool first = true;
                writer.Write('[');


                foreach (var path in paths)
                {
                    if (first) { first = false; }
                    else { writer.Write(','); }

                    writer.Write(path);
                }
                writer.Write(']');
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Any(_ => _ is MessageTooLongException || _ is OutOfMemoryException))
            {
                throw new MessageTooLongException();
            }
            catch (OutOfMemoryException)
            {
                throw new MessageTooLongException();
            }
        }

        private static bool QueryPathInvalid(string queryPath)
        {
            return queryPath == null || queryPath == "/" || queryPath == String.Empty;
        }
        #endregion
    }
}
