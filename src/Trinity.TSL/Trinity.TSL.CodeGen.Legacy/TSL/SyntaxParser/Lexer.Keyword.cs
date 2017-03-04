using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Trinity.TSL
{
    internal class Keyword
    {
        internal TokenId Token;
        internal object Info;

        internal Keyword(TokenId token)
            : this(token, null)
        {
        }

        internal Keyword(TokenId token, object tokenValue)
        {
            this.Token = token;
            this.Info = tokenValue;
        }
    }

    partial class Lexer
    {
        static Lexer()
        {
            KeywordMap = new Dictionary<string, Keyword>();
            #region Initialize Keyword Map
            KeywordMap.Add("<-", new Keyword(TokenId.MapSymbol, null));
            KeywordMap.Add("<", new Keyword(TokenId.TypeListBegin, null));
            KeywordMap.Add(">", new Keyword(TokenId.TypeListEnd, null));
            KeywordMap.Add("{", new Keyword(TokenId.BlockBegin, null));
            KeywordMap.Add("}", new Keyword(TokenId.BlockEnd, null));
            KeywordMap.Add(".", new Keyword(TokenId.Dot, null));
            KeywordMap.Add("[", new Keyword(TokenId.LSquare, null));
            KeywordMap.Add("]", new Keyword(TokenId.RSquare, null));
            KeywordMap.Add(",", new Keyword(TokenId.Comma, null));
            KeywordMap.Add(";", new Keyword(TokenId.Semicolon, null));
            KeywordMap.Add("=", new Keyword(TokenId.Equal, null));
            KeywordMap.Add(":", new Keyword(TokenId.Colon, null));
            KeywordMap.Add("//", new Keyword(TokenId.DoubleSlash, null));
            KeywordMap.Add("/*", new Keyword(TokenId.SlashStar, null));
            KeywordMap.Add("*/", new Keyword(TokenId.StarSlash, null));
            KeywordMap.Add("#", new Keyword(TokenId.Sharp, null));
            KeywordMap.Add("\n", new Keyword(TokenId.LF, null));
            KeywordMap.Add("List", new Keyword(TokenId.ListType, null));
            KeywordMap.Add("Set", new Keyword(TokenId.SetType, null));
            KeywordMap.Add("Dictionary", new Keyword(TokenId.DictionaryType, null));
            KeywordMap.Add("String", new Keyword(TokenId.StringType, null));
            KeywordMap.Add("string", new Keyword(TokenId.StringType, null));
            KeywordMap.Add("BitArray", new Keyword(TokenId.BitArrayType, null));
            KeywordMap.Add("BitList", new Keyword(TokenId.BitListType, null));
            KeywordMap.Add("DateTime", new Keyword(TokenId.DateTimeType, null));
            KeywordMap.Add("Guid", new Keyword(TokenId.GuidType, null));
            KeywordMap.Add("Fixed", new Keyword(TokenId.FixedModifier, null));
            KeywordMap.Add("fixed", new Keyword(TokenId.FixedModifier, null));
            KeywordMap.Add("Elastic", new Keyword(TokenId.ElasticModifier, null));
            KeywordMap.Add("elastic", new Keyword(TokenId.ElasticModifier, null));
            KeywordMap.Add("Extern", new Keyword(TokenId.ExternModifier, null));
            KeywordMap.Add("extern", new Keyword(TokenId.ExternModifier, null));
            KeywordMap.Add("Optional", new Keyword(TokenId.OptionalModifier, null));
            KeywordMap.Add("optional", new Keyword(TokenId.OptionalModifier, null));
            KeywordMap.Add("syn", new Keyword(TokenId.SyncRPC, null));
            KeywordMap.Add("Syn", new Keyword(TokenId.SyncRPC, null));
            KeywordMap.Add("asyn", new Keyword(TokenId.AsyncRPC, null));
            KeywordMap.Add("Asyn", new Keyword(TokenId.AsyncRPC, null));
            KeywordMap.Add("type", new Keyword(TokenId.Type, null));
            KeywordMap.Add("Type", new Keyword(TokenId.Type, null));
            KeywordMap.Add("Request", new Keyword(TokenId.Request, null));
            KeywordMap.Add("request", new Keyword(TokenId.Request, null));
            KeywordMap.Add("Response", new Keyword(TokenId.Response, null));
            KeywordMap.Add("response", new Keyword(TokenId.Response, null));
            KeywordMap.Add("raw", new Keyword(TokenId.RAW, null));
            KeywordMap.Add("Raw", new Keyword(TokenId.RAW, null));
            KeywordMap.Add("RAW", new Keyword(TokenId.RAW, null));
            KeywordMap.Add("void", new Keyword(TokenId.VOID, null));
            KeywordMap.Add("Struct", new Keyword(TokenId.StructDefinition, null));
            KeywordMap.Add("struct", new Keyword(TokenId.StructDefinition, null));
            KeywordMap.Add("Cell", new Keyword(TokenId.CellDefinition, null));
            KeywordMap.Add("cell", new Keyword(TokenId.CellDefinition, null));
            KeywordMap.Add("Protocol", new Keyword(TokenId.ProtocolDefinition, null));
            KeywordMap.Add("protocol", new Keyword(TokenId.ProtocolDefinition, null));
            KeywordMap.Add("Slave", new Keyword(TokenId.SlaveDefinition, null));
            KeywordMap.Add("slave", new Keyword(TokenId.SlaveDefinition, null));
            KeywordMap.Add("Proxy", new Keyword(TokenId.ProxyDefinition, null));
            KeywordMap.Add("proxy", new Keyword(TokenId.ProxyDefinition, null));
            KeywordMap.Add("Index", new Keyword(TokenId.IndexDefinition, null));
            KeywordMap.Add("index", new Keyword(TokenId.IndexDefinition, null));
            KeywordMap.Add("byte", new Keyword(TokenId.ByteType, null));
            KeywordMap.Add("sbyte", new Keyword(TokenId.SByteType, null));
            KeywordMap.Add("bool", new Keyword(TokenId.BoolType, null));
            KeywordMap.Add("char", new Keyword(TokenId.CharType, null));
            KeywordMap.Add("short", new Keyword(TokenId.ShortType, null));
            KeywordMap.Add("ushort", new Keyword(TokenId.UShortType, null));
            KeywordMap.Add("int", new Keyword(TokenId.IntType, null));
            KeywordMap.Add("uint", new Keyword(TokenId.UIntType, null));
            KeywordMap.Add("long", new Keyword(TokenId.LongType, null));
            KeywordMap.Add("ulong", new Keyword(TokenId.ULongType, null));
            KeywordMap.Add("float", new Keyword(TokenId.FloatType, null));
            KeywordMap.Add("double", new Keyword(TokenId.DoubleType, null));
            KeywordMap.Add("decimal", new Keyword(TokenId.DecimalType, null));
            KeywordMap.Add("Using", new Keyword(TokenId.Using, null));
            KeywordMap.Add("using", new Keyword(TokenId.Using, null));
            KeywordMap.Add("Include", new Keyword(TokenId.Include, null));
            KeywordMap.Add("include", new Keyword(TokenId.Include, null));
            KeywordMap.Add("Layout", new Keyword(TokenId.Layout, null));
            KeywordMap.Add("Sequential", new Keyword(TokenId.Seq_Layout, null));
            KeywordMap.Add("Auto", new Keyword(TokenId.Auto_Layout, null));
            KeywordMap.Add("Enum", new Keyword(TokenId.Enum, null));
            KeywordMap.Add("enum", new Keyword(TokenId.Enum, null));
            KeywordMap.Add("EntityList", new Keyword(TokenId.EntityList, null));
            KeywordMap.Add("DataSource", new Keyword(TokenId.DataSource, null));
            KeywordMap.Add("RelationalTable", new Keyword(TokenId.RelationalTable, null));
            KeywordMap.Add("PartitionBy", new Keyword(TokenId.PartitionBy, null));
            KeywordMap.Add("SpecFile", new Keyword(TokenId.SpecFile, null));
            KeywordMap.Add("Entities", new Keyword(TokenId.Entities, null));
            KeywordMap.Add("ConnectionString", new Keyword(TokenId.ConnectionString, null));
            KeywordMap.Add("SqlServer", new Keyword(TokenId.SqlServer, null));
            KeywordMap.Add("SQLServer", new Keyword(TokenId.SqlServer, null));
            KeywordMap.Add("PrimaryKey", new Keyword(TokenId.PrimaryKey, null));
            KeywordMap.Add("ReferencedCell", new Keyword(TokenId.ReferencedCell, null));
            KeywordMap.Add("Column", new Keyword(TokenId.Column, null));
            KeywordMap.Add("association", new Keyword(TokenId.Association, null));
            KeywordMap.Add("Association", new Keyword(TokenId.Association, null));
            KeywordMap.Add("invisible", new Keyword(TokenId.Invisible, null));
            KeywordMap.Add("Invisible", new Keyword(TokenId.Invisible, null));
            KeywordMap.Add("TrinitySettings", new Keyword(TokenId.TrinitySettings, null));
            KeywordMap.Add("RunningMode", new Keyword(TokenId.RunningMode, null));
            KeywordMap.Add("Embedded", new Keyword(TokenId.Embedded, null));
            KeywordMap.Add("Distributed", new Keyword(TokenId.Distributed, null));
            KeywordMap.Add("IndexServerConnectionString", new Keyword(TokenId.IndexConnString, null));
            KeywordMap.Add("IndexConnString", new Keyword(TokenId.IndexConnString, null));
            KeywordMap.Add("TQL", new Keyword(TokenId.TQL, null));
            KeywordMap.Add("OFF", new Keyword(TokenId.OFF, null));
            KeywordMap.Add("Off", new Keyword(TokenId.OFF, null));
            KeywordMap.Add("ON", new Keyword(TokenId.ON, null));
            KeywordMap.Add("On", new Keyword(TokenId.ON, null));
            KeywordMap.Add("DataProvider", new Keyword(TokenId.DataProvider, null));
            KeywordMap.Add("RDF", new Keyword(TokenId.RDF, null));
            KeywordMap.Add("Freebase", new Keyword(TokenId.Freebase, null));
            KeywordMap.Add("TSLProfile", new Keyword(TokenId.TSLProfile, null));
            KeywordMap.Add("TrinityMM", new Keyword(TokenId.TrinityMM, null));
            KeywordMap.Add("ExtensionSuffixChar", new Keyword(TokenId.ExtensionSuffixChar, null));
            KeywordMap.Add("namespace", new Keyword(TokenId.Namespace, null));
            #endregion

            TokenMap = new Dictionary<TokenId, List<string>>();
            #region Initialize Token Map
            foreach (var kv in KeywordMap)
            {
                var token_id = kv.Value.Token;
                List<string> keyword_list;
                if(!TokenMap.TryGetValue(token_id, out keyword_list))
                {
                    keyword_list = new List<string>();
                    TokenMap.Add(token_id, keyword_list);
                }
                keyword_list.Add(kv.Key);
            }
            #endregion
        }
        static Dictionary<string, Keyword> KeywordMap;
        static Dictionary<TokenId, List<string>> TokenMap;
    }
}
