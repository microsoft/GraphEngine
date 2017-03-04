using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    internal class EntityListDescriptor: AbstractStruct
    {
        List<string> EntityList;
        internal EntityListDescriptor(Lexer l)
        {
            EntityList = new List<string>();

            if (l.ReadToken() != TokenId.NameString)
            {
                CompilerError.Throw(CompilerErrorType.UnexpectedSymbol, l);
            }
            this.Name = l.LastNameString;
            if (l.ReadToken() != TokenId.BlockBegin)
            {
                CompilerError.Throw(CompilerErrorType.UnexpectedSymbol, l);
            }

            while (true)
            {
                if (l.ReadToken(true) != TokenId.BlockEnd)
                {
                    l.ReadUntil(TokenId.Semicolon);
                    EntityList.Add(l.LastNameString.Trim());
                    l.ReadToken();
                }
                else
                {
                    l.ReadToken();
                    break;
                }
            }
        }

        internal static EntityListDescriptor ReadEntityList(Lexer l)
        {
            return new EntityListDescriptor(l);
        }
    }
}