using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    internal enum DataSourceType : int
    {
        Unknown, SqlServer
    }
    internal class DataSourceDescriptor : AbstractStruct
    {
        internal DataSourceType Type;
        //internal string SpecName;
        internal string ConnectionString;
        //internal HashSet<string> EntityLists;
        internal DataSourceDescriptor(Lexer l)
        {
            Type = DataSourceType.Unknown;

            if (l.ReadToken() != TokenId.NameString)
            {
                CompilerError.Throw(CompilerErrorType.UnexpectedSymbol, l);
            }
            Name = l.LastNameString;
            if (l.ReadToken() != TokenId.BlockBegin)
            {
                CompilerError.Throw(CompilerErrorType.UnexpectedSymbol, l);
            }

            while (true)
            {
                TokenId s = l.ReadToken();
                if (s == TokenId.BlockEnd)
                    break;
                switch (s)
                {
                    case TokenId.Type:
                        if (l.ReadToken() != TokenId.Colon)
                            CompilerError.Throw(CompilerErrorType.UnexpectedSymbol, l);
                        s = l.ReadToken();

                        if (s == TokenId.SqlServer)
                            Type = DataSourceType.SqlServer;
                        break;

                    case TokenId.ConnectionString:
                        if (l.ReadToken() != TokenId.Colon)
                            CompilerError.Throw(CompilerErrorType.UnexpectedSymbol, l);
                        ConnectionString = l.ReadStringValue().Trim();
                        break;
                }
                if (l.ReadToken() != TokenId.Semicolon)
                    CompilerError.Throw(CompilerErrorType.UnexpectedSymbol, l);
            }
        }
        internal static DataSourceDescriptor ReadDataSource(Lexer l)
        {
            return new DataSourceDescriptor(l);
        }
    }
}
