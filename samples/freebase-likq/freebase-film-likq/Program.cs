using FanoutSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using Trinity;
using Trinity.Network;
using Newtonsoft.Json.Linq;
using Trinity.Core.Lib;
using System.IO;
using Trinity.Diagnostics;
using Microsoft.Data.Sqlite;
using System.Net;
using System.Diagnostics;
using System.IO.Compression;
using freebase_film_tsl;

namespace freebase_likq
{
    class Program
    {
        private static SqliteConnection s_dbconn;
        private static string s_freebase_data_blobcontainer = "https://graphengine.blob.core.windows.net/public-data";
        //  !Note, different datasets are built with different TSL extensions,
        //  make sure you reference the correct TSL storage extension dll!
        private static string s_freebase_dataset = "freebase-film-dataset.zip";

        static void Main(string[] args)
        {
            Initialize();

            //  Spawn a 'stock' Trinity server
            TrinityServer server = new TrinityServer();
            //  Plug-in the LIKQ communication module.
            server.RegisterCommunicationModule<FanoutSearchModule>();
            //  Start the server
            server.Start();
        }

        private static void Initialize()
        {
            //  Setup LIKQ index service
            FanoutSearchModule.RegisterIndexService(IndexService);
            //  Set LIKQ starting node keyword to 'Freebase'
            LambdaDSL.SetDialect("Freebase", "StartFrom", "VisitNode", "FollowEdge", "Action");
            //  Plug-in Freebase ICell adapter
            FanoutSearchModule.RegisterUseICellOperationMethod(CellGroupAccessor.New);
            //  Configure LIKQ timeout
            FanoutSearchModule.SetQueryTimeout(1000000);

            string storage_path = Path.Combine(Global.MyAssemblyPath, "storage");
            if (Directory.Exists(storage_path) && !Directory.GetFileSystemEntries(storage_path).Any())
            {
                Directory.Delete(storage_path);
            }

            if (!Directory.Exists(storage_path))
            {
                DownloadDataFile();
            }

            Global.LocalStorage.LoadStorage();
            string sqlite_db_path = Path.Combine(storage_path, "freebase.sqlite");
            if (!File.Exists(sqlite_db_path))
            {
                BuildIndex(sqlite_db_path);
            }

            s_dbconn = new SqliteConnection($"Data Source={sqlite_db_path};Version=3;");
            s_dbconn.Open();

            return;
        }

        private static void BuildIndex(string sqlite_db_path)
        {
            s_dbconn = new SqliteConnection($"Data Source={sqlite_db_path};Version=3;");
            s_dbconn.Open();
            Log.WriteLine("Building Sqlite db to index type.object.name property...");
            long processed_count = 0;
            SqliteCommand cmd;
            cmd = new SqliteCommand("CREATE TABLE nameindex (name TEXT, id INTEGER)", s_dbconn);
            cmd.ExecuteNonQuery();
            cmd = new SqliteCommand("CREATE VIRTUAL TABLE fuzzynameindex using FTS3 (name TEXT, id INTEGER);", s_dbconn);
            cmd.ExecuteNonQuery();
            List<Tuple<string, long>> batch = new List<Tuple<string, long>>();
            foreach (var type_object in Global.LocalStorage.type_object_Accessor_Selector())
            {
                if (!type_object.Contains_type_object_name) continue;
                string name = type_object.type_object_name.ToString().Replace("'", "''");
                batch.Add(Tuple.Create(name, type_object.CellId));
                if (++processed_count % 100000 == 0)
                {
                    Log.WriteLine("{0} cells processed", processed_count);
                }
                if (batch.Count >= 500000)
                {
                    BatchInsert(batch);
                }
            }
            if (batch.Count > 0)
            {
                BatchInsert(batch);
            }

            Log.WriteLine("Successfully built the index.");
            s_dbconn.Dispose();
        }

        private static void DownloadDataFile()
        {
            Log.WriteLine($"The storage folder is not found. Downloading the data from {s_freebase_data_blobcontainer}/{s_freebase_dataset} now...");
            WebClient download_client = new WebClient();
            Stopwatch download_timer  = Stopwatch.StartNew();
            download_client.DownloadProgressChanged += (sender, e) =>
            {
                lock (download_client)
                {
                    Console.CursorLeft = 0;
                    Console.Write($"[{e.ProgressPercentage}%] {e.BytesReceived} / {e.TotalBytesToReceive} bytes downloaded. {e.BytesReceived / (download_timer.ElapsedMilliseconds + 1)} KiB/s".PadRight(Console.BufferWidth - 1));
                }
            };
            download_client.DownloadFileTaskAsync($"{s_freebase_data_blobcontainer}/{s_freebase_dataset}", s_freebase_dataset).Wait();
            Console.WriteLine();
            Log.WriteLine("Download complete. Unarchiving storage folder...");
            ZipFile.ExtractToDirectory(s_freebase_dataset, Global.MyAssemblyPath);
            Log.WriteLine("Successfully unarchived the data files.");
        }

        private static void BatchInsert(List<Tuple<string, long>> batch)
        {
            try
            {
                string content = string.Join(",", batch.Select(_ => $"('{_.Item1}', {_.Item2})"));

                SqliteCommand cmd;
                cmd = new SqliteCommand($"INSERT INTO nameindex (name, id) VALUES {content}", s_dbconn);
                cmd.ExecuteNonQuery();

                cmd = new SqliteCommand($"INSERT INTO fuzzynameindex (name, id) VALUES {content}", s_dbconn);
                cmd.ExecuteNonQuery();
                batch.Clear();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public static unsafe long GetCellID(string mid, ushort cellType = 1)
        {
            long id = HashHelper.HashString2Int64(mid);
            ushort* sp = (ushort*)&id;
            *(sp + 2) = (ushort)(*(sp + 2) ^ *(sp + 3));
            *(sp + 3) = cellType;
            return id;
        }

        /// <summary>
        /// This is a sample index service that interfaces with the LIKQ module.
        /// When query constraints are specified in a query, the LIKQ module calls
        /// a registered index service to retrieve vertices satisfying the constraints.
        /// The constraints are specified in the match object, which is a json object.
        /// LIKQ itself does not specify the DSL syntax for the match object so here
        /// we establish a simple one, which accepts three types of queries:
        ///
        ///   1. Query by Freebase MID.
        ///   2. Query by type_object_name.
        ///   3. Query by type_object_name, and perform full-text-search (fuzzy match).
        ///
        /// To query by MID we simply hash the MID string to a cell id. To query by
        /// entity name, we route the query into our Sqlite database. In this way one
        /// can develop his/her own index query logic and dispatch the query to various
        /// query backends (A full RDBMS, ElasticSearch etc.).
        ///
        /// </summary>
        /// <param name="matchObject">The match object passed from the query client.</param>
        /// <param name="typeString">The type of the entity. For brevity we ignore this parameter now.</param>
        /// <returns></returns>
        private static IEnumerable<long> IndexService(object matchObject, string typeString)
        {
            JObject match_obj = (JObject)matchObject;

            var mid = match_obj["mid"];
            var name = match_obj["name"];
            var fuzzy_name = match_obj["fuzzy_name"];

            if (mid != null)
            {
                return new List<long> { (GetCellID(mid.ToString())) };
            }
            else if (name != null)
            {
                return SqliteNameIndex(name.ToString());
            }
            else if (fuzzy_name != null)
            {
                return SqliteNameIndex(fuzzy_name.ToString(), fuzzy: true);
            }
            else
            {
                return Enumerable.Empty<long>();
            }
        }

        private static IEnumerable<long> SqliteNameIndex(string name, bool fuzzy = false)
        {
            name = name.Replace("'", "''");
            string op = fuzzy ? "MATCH" : "=";
            string table = fuzzy ? "fuzzynameindex" : "nameindex";

            SqliteCommand cmd = new SqliteCommand($"SELECT id FROM {table} WHERE name {op} '{name}'", s_dbconn);
            SqliteDataReader results = cmd.ExecuteReader();
            while (results.Read())
            {
                long id = long.Parse(results["id"].ToString());
                yield return id;
            }
        }
    }
}
