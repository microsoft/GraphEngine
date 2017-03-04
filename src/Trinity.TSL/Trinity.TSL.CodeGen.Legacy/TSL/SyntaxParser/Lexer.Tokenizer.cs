using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

namespace Trinity.TSL
{
    partial class Lexer
    {
        internal string srcString;
        internal List<Token> tokenList;
        internal int lastPos = 0;

        internal int LastTokenCol
        {
            get
            {
                int ret = 1;
                for (int i = 0; i < lastPos; ++i)
                    if (src[i] == '\n') ret = 1;
                    else if (src[i] == '\t') ret += 4;
                    else ++ret;
                return ret;
            }
        }

        internal int LastTokenRow
        {
            get
            {
                int ret = 1;
                for (int i = 0; i < lastPos; ++i)
                    if (src[i] == '\n') ++ret;
                return ret;
            }
        }

        internal bool Tokenizer()
        {
            tokenList = new List<Token>();

            while (!EOF)
            {
                Token tempToken = new Token(ReadToken(), lastPos, srcString);
                tempToken.Column = LastTokenCol;
                tempToken.Row = LastTokenRow;
                tempToken.SrcString = srcString;
                tokenList.Add(tempToken);
            }

            return true;
        }
    }
}
