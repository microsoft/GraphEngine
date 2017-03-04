using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    enum TokenId
    {
        MapSymbol,
        Comma,
        Semicolon,
        BlockBegin,
        BlockEnd,
        TypeListBegin,
        TypeListEnd,
        LSquare,
        RSquare,
        Equal,
        Dot,
        Colon,
        Using,
        Include,

        ListType,
        SetType,
        DictionaryType,
        StringType,
        BitArrayType,
        BitListType,
        DateTimeType,
        GuidType,

        FixedModifier,
        ElasticModifier,
        ExternModifier,
        OptionalModifier,

        StructDefinition,
        CellDefinition,
        SlaveDefinition,
        ProxyDefinition,
        ProtocolDefinition,
        IndexDefinition,

        SyncRPC,
        AsyncRPC,
        Type,
        Request,
        Response,
        VOID,
        RAW,

        ByteType,
        SByteType,
        CharType,
        BoolType,
        ShortType,
        UShortType,
        IntType,
        UIntType,
        LongType,
        ULongType,
        FloatType,
        DoubleType,
        DecimalType,

        NameString,
        Integer,

        DoubleSlash,
        SlashStar,
        StarSlash,
        LF,
        Sharp,

        Layout,
        Seq_Layout,
        Auto_Layout,

        Enum,

        // Struct Attributes
        EntityList,
        DataSource,
        RelationalTable,
        PartitionBy,
        SpecFile,
        Entities,
        ConnectionString,
        SqlServer,

        // Cell Field Attributes
        PrimaryKey,
        ReferencedCell,
        Column,
        Association,
        Invisible,
        TrinitySettings,
        RunningMode,
        Embedded,
        Distributed,
        IndexConnString,
        TQL,
        OFF,
        ON,
        DataProvider,
        RDF,
        Freebase,
        TSLProfile,
        TrinityMM,
        ExtensionSuffixChar,
        Namespace,
    }

    internal class Token
    {
        internal string SrcString;
        internal int StartPosition;
        internal int Row;
        internal int Column;

        internal TokenId TokenID;
        internal object Value;

        internal Token(TokenId tokenID, int position, object value)
        {
            this.TokenID = tokenID;
            this.StartPosition = position;
            this.Value = value;
        }
    }

    internal class KeywordMap
    {
        Dictionary<string, Token> KeywordTokenPairs;
        public KeywordMap(Dictionary<string, Token> map)
        {
            KeywordTokenPairs = map;
        }
    }

    partial class Lexer
    {
        #region Symbol maps
        static List<string> SymbolStringList = new List<string>
        {
            "<-",
            "<",
            ">",
            "{",
            "}",
            ".",
            "[",
            "]",
            ",",
            ";",
            "=",
            ":",
            "//",
            "/*",
            "*/",
            "#",
            "\n",

            "List",
            "Set",
            "Dictionary",
            "String",
            "string",
            "BitArray",
            "BitList",
            "DateTime",
            "Guid",

            "Fixed",
            "fixed",
            "Elastic",
            "elastic",
            "Extern",
            "extern",
            "Optional",
            "optional",

            "syn",
            "Syn",
            "asyn",
            "Asyn",
            "type",
            "Type",
            "Request",
            "request",
            "Response",
            "response",
            "raw",
            "Raw",
            "RAW",
            "void",

            "Struct",
            "struct",
            "Cell",
            "cell",
            "Protocol",
            "protocol",
            "Slave",
            "slave",
            "Proxy",
            "proxy",
            "Index",
            "index",

            "byte",
            "sbyte",
            "bool",
            "char",
            "short",
            "ushort",
            "int",
            "uint",
            "long",
            "ulong",
            "float",
            "double",
            "decimal",

            "Using",
            "using",
            "Include",
            "include",

            "Layout",
            "Sequential",
            "Auto",
            "Enum",
            "enum",

            "EntityList",
            "DataSource",
            "RelationalTable",
            "PartitionBy",
            "SpecFile",
            "Entities",
            "ConnectionString",
            "SqlServer",
            "SQLServer",

            "PrimaryKey",
            "ReferencedCell",
            "Column",
            "association",
            "Association",
            "invisible",
            "Invisible",
            "TrinitySettings",
            "RunningMode",
            "Embedded",
            "Distributed",
            "IndexServerConnectionString",
            "IndexConnString",

            "TQL",
            "OFF",
            "Off",
            "ON",
            "On",
            "DataProvider",
            "RDF",
            "Freebase",
            "TSLProfile",
            "TrinityMM",
            "ExtensionSuffixChar",
            "namespace",
        };

        static List<TokenId> SymbolList = new List<TokenId>
        {
            TokenId.MapSymbol,
            TokenId.TypeListBegin,
            TokenId.TypeListEnd,
            TokenId.BlockBegin,
            TokenId.BlockEnd,
            TokenId.Dot,
            TokenId.LSquare,
            TokenId.RSquare,
            TokenId.Comma,
            TokenId.Semicolon,
            TokenId.Equal,
            TokenId.Colon,
            TokenId.DoubleSlash,
            TokenId.SlashStar,
            TokenId.StarSlash,
            TokenId.Sharp,
            TokenId.LF,

            TokenId.ListType,
            TokenId.SetType,
            TokenId.DictionaryType,
            TokenId.StringType,
            TokenId.StringType,
            TokenId.BitArrayType,
            TokenId.BitListType,
            TokenId.DateTimeType,
            TokenId.GuidType,

            TokenId.FixedModifier,
            TokenId.FixedModifier,
            TokenId.ElasticModifier,
            TokenId.ElasticModifier,
            TokenId.ExternModifier,
            TokenId.ExternModifier,
            TokenId.OptionalModifier,
            TokenId.OptionalModifier,

            TokenId.SyncRPC,
            TokenId.SyncRPC,
            TokenId.AsyncRPC,
            TokenId.AsyncRPC,
            TokenId.Type,
            TokenId.Type,
            TokenId.Request,
            TokenId.Request,
            TokenId.Response,
            TokenId.Response,
            TokenId.RAW,
            TokenId.RAW,
            TokenId.RAW,
            TokenId.VOID,

            TokenId.StructDefinition,
            TokenId.StructDefinition,
            TokenId.CellDefinition,
            TokenId.CellDefinition,
            TokenId.ProtocolDefinition,
            TokenId.ProtocolDefinition,
            TokenId.SlaveDefinition,
            TokenId.SlaveDefinition,
            TokenId.ProxyDefinition,
            TokenId.ProxyDefinition,
            TokenId.IndexDefinition,
            TokenId.IndexDefinition,

            TokenId.ByteType,
            TokenId.SByteType,
            TokenId.BoolType,
            TokenId.CharType,
            TokenId.ShortType,
            TokenId.UShortType,
            TokenId.IntType,
            TokenId.UIntType,
            TokenId.LongType,
            TokenId.ULongType,
            TokenId.FloatType,
            TokenId.DoubleType,
            TokenId.DecimalType,

            TokenId.Using,
            TokenId.Using,
            TokenId.Include,
            TokenId.Include,

            TokenId.Layout,
            TokenId.Seq_Layout,
            TokenId.Auto_Layout,

            TokenId.Enum,
            TokenId.Enum,

            TokenId.EntityList,
            TokenId.DataSource,
            TokenId.RelationalTable,
            TokenId.PartitionBy,
            TokenId.SpecFile,
            TokenId.Entities,
            TokenId.ConnectionString,
            TokenId.SqlServer,
            TokenId.SqlServer,

            TokenId.PrimaryKey,
            TokenId.ReferencedCell,
            TokenId.Column,
            TokenId.Association,
            TokenId.Association,
            TokenId.Invisible,
            TokenId.Invisible,
            TokenId.TrinitySettings,
            TokenId.RunningMode,
            TokenId.Embedded,
            TokenId.Distributed,
            TokenId.IndexConnString,
            TokenId.IndexConnString,
            TokenId.TQL,
            TokenId.OFF,
            TokenId.OFF,
            TokenId.ON,
            TokenId.ON,
            TokenId.DataProvider,
            TokenId.RDF,
            TokenId.Freebase,
            TokenId.TSLProfile,
            TokenId.TrinityMM,
            TokenId.ExtensionSuffixChar,
            TokenId.Namespace,
        };
        static List<TokenId> PunctationList = new List<TokenId>
        {
            TokenId.MapSymbol,
            TokenId.TypeListBegin,
            TokenId.TypeListEnd,
            TokenId.BlockBegin,
            TokenId.BlockEnd,
            TokenId.Dot,
            TokenId.LSquare,
            TokenId.RSquare,
            TokenId.Comma,
            TokenId.Semicolon,
            TokenId.Equal,
            TokenId.Colon,
            TokenId.LF,
            TokenId.DoubleSlash,
            TokenId.SlashStar,
            TokenId.StarSlash
        };
        static List<TokenId> ModifierList = new List<TokenId>
        {
            TokenId.FixedModifier,
            TokenId.ElasticModifier,
            TokenId.ExternModifier,
            TokenId.OptionalModifier,
        };
        #endregion

        internal static List<string> GetSymbolStrings(TokenId symbol)
        {
            List<string> results;
            if (TokenMap.TryGetValue(symbol, out results))
            {
                return results;
            }
            return new List<string>(0);
        }

        internal static bool Compare(TokenId symbol, string str)
        {
            return GetSymbolStrings(symbol).Contains(str);
        }
    }
}
