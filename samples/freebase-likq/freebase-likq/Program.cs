using FanoutSearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.Network;
using freebase_tsl;
using Newtonsoft.Json.Linq;
using Trinity.Core.Lib;
using System.IO;
using Trinity.Diagnostics;
using System.Data.SQLite;
using Trinity.TSL.Lib;

namespace freebase_likq
{
    class Program
    {
        static SQLiteConnection s_dbconn;

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

            string storage_path = Path.Combine(Global.MyAssemblyPath, "storage");
            if (!Directory.Exists(storage_path))
            {
                Log.WriteLine("The storage folder is not found. Please download the data and place it next to the executable.");
                Environment.Exit(-1);
            }

            Global.LocalStorage.LoadStorage();
            string sqlite_db_path = Path.Combine(storage_path, "freebase.sqlite");
            if (File.Exists(sqlite_db_path))
            {
                s_dbconn = new SQLiteConnection(String.Format("Data Source={0};Version=3;", sqlite_db_path));
                s_dbconn.Open();
                return;
            }

            SQLiteConnection.CreateFile(sqlite_db_path);
            s_dbconn = new SQLiteConnection(String.Format("Data Source={0};Version=3;", sqlite_db_path));
            s_dbconn.Open();
            Log.WriteLine("Building SQLite db to index type.object.name property...");
            long processed_count = 0;
            SQLiteCommand cmd;
            cmd = new SQLiteCommand("CREATE TABLE nameindex (name TEXT, id INTEGER)", s_dbconn);
            cmd.ExecuteNonQuery();
            cmd = new SQLiteCommand("CREATE VIRTUAL TABLE fuzzynameindex using FTS3 (name TEXT, id INTEGER);", s_dbconn);
            cmd.ExecuteNonQuery();
            List<Tuple<string, long>> batch = new List<Tuple<string, long>>();
            foreach (var type_object in Global.LocalStorage.type_object_Accessor_Selector())
            {
                if (!type_object.Contains_type_object_name) continue;
                string name = type_object.type_object_name.ToString().Replace("'", "''");
                batch.Add(Tuple.Create( name , type_object.CellID.Value));
                if (++processed_count % 100000 == 0)
                {
                    Log.WriteLine("{0} cells processed", processed_count);
                }
                if (batch.Count >= 500000)
                {
                    BatchInsert(batch);
                }
            }
            if(batch.Count > 0)
            {
                BatchInsert(batch);
            }

            Log.WriteLine("Successfully built the index.");
        }

        private static void BatchInsert(List<Tuple<string, long>> batch)
        {
            try
            {
                SQLiteCommand cmd;
                cmd = new SQLiteCommand(String.Format("INSERT INTO nameindex (name, id) VALUES {0};",
                    string.Join(",", batch.Select(_ => String.Format("('{0}', {1})", _.Item1, _.Item2)))
                    ), s_dbconn);
                cmd.ExecuteNonQuery();

                cmd = new SQLiteCommand(String.Format("INSERT INTO fuzzynameindex (name, id) VALUES {0};",
                    string.Join(",", batch.Select(_ => String.Format("('{0}', {1})", _.Item1, _.Item2)))
                    ), s_dbconn);
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
                return SQLiteNameIndex(name.ToString());
            }
            else if (fuzzy_name != null)
            {
                return SQLiteNameIndex(fuzzy_name.ToString(), fuzzy: true);
            }
            else
            {
                return Enumerable.Empty<long>();
            }
        }

        private static IEnumerable<long> SQLiteNameIndex(string name, bool fuzzy = false)
        {
            name = name.Replace("'", "''");
            string op = fuzzy ? "MATCH" : "=";
            string table = fuzzy ? "fuzzynameindex" : "nameindex";

            SQLiteCommand cmd = new SQLiteCommand(String.Format("SELECT id FROM {2} WHERE name {0} '{1}'", op, name, table), s_dbconn);
            SQLiteDataReader results = cmd.ExecuteReader();
            while (results.Read())
            {
                long id = long.Parse(results["id"].ToString());
                yield return id;
            }
        }
    }
}
