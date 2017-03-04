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
        internal string src;
        internal int src_offset = 0;

        internal int CurrentPos
        {
            get
            {
                return src_offset;
            }
        }

        private AttributeDescriptor last_attribute;
        internal AttributeDescriptor LastAttribute
        {
            get
            {
                AttributeDescriptor tmp = last_attribute;
                last_attribute = new AttributeDescriptor();
                if (tmp == null)
                    return new AttributeDescriptor();
                return tmp;
            }

            set
            {
                last_attribute = value;
            }
        }

        internal string GetSrcSpan(int start, int end)
        {
            return src.Substring(start, end - start);
        }

        internal string source_filename;
        internal Lexer(string source, string source_filename)
        {
            src = source;
            src_offset = 0;
            this.source_filename = source_filename;
            SkipSpaces();
        }

        private void SkipSpaces()
        {
            while (src_offset < src.Length && char.IsWhiteSpace(src[src_offset]))
                ++src_offset;
        }

        public TokenId ReadToken(bool only_peek = false)
        {
            lastPos = src_offset;

            for (int i = 0; i < SymbolStringList.Count; ++i)
            {
                if (src.Length - src_offset >= SymbolStringList[i].Length && src.Substring(src_offset, SymbolStringList[i].Length) == SymbolStringList[i])
                {
                    if (!PunctationList.Contains(SymbolList[i]) &&
                        src_offset + SymbolStringList[i].Length < src.Length)
                    {
                        //Check whether its storage a name string with a reserved keyword as prefix
                        if (char.IsLetterOrDigit(src[src_offset + SymbolStringList[i].Length]) ||
                            src[src_offset + SymbolStringList[i].Length] == '_')
                            continue;
                    }


                    if (!only_peek)
                    {
                        src_offset += SymbolStringList[i].Length;
                        if (SymbolList[i] == TokenId.Semicolon || SymbolList[i] == TokenId.BlockBegin)
                        {
                            SkipSpaces();
                            this.PushFieldStartPos();
                        }
                    }

                    srcString = SymbolStringList[i];
                    SkipSpaces();
                    if (IsModifier(SymbolList[i]))
                    {
                        switch (SymbolList[i])
                        {
                            case TokenId.FixedModifier:
                                LastModifier = Modifier.Fixed;
                                break;
                            case TokenId.ElasticModifier:
                                LastModifier = Modifier.Elastic;
                                break;
                            case TokenId.ExternModifier:
                                LastModifier = Modifier.Extern;
                                break;
                            case TokenId.OptionalModifier:
                                LastModifier = Modifier.Optional;
                                break;
                        }
                    }
                    LastToken = SymbolList[i];
                    if (LastToken == TokenId.DoubleSlash)
                    {
                        ReadUntil(TokenId.LF);
                        ReadToken();
                        return ReadToken(only_peek);
                    }
                    else if (LastToken == TokenId.SlashStar)
                    {
                        ReadUntil(TokenId.StarSlash);
                        ReadToken();
                        return ReadToken(only_peek);
                    }
                    return SymbolList[i];
                }
            }
            if (src.Length == src_offset)
                CompilerError.Throw(CompilerErrorType.UnexpectedEndOfFile, this);

            bool invalidAsNameString = false;

            if (src[src_offset] != '_' && !char.IsLetter(src[src_offset]) && !this.IsSatoriStringChar(src[src_offset]))
                invalidAsNameString = true;

            StringBuilder sb = new StringBuilder();

            while (src_offset != src.Length)
            {
                if (src[src_offset] != '_' && !char.IsLetterOrDigit(src[src_offset]) && !this.IsSatoriStringChar(src[src_offset]))
                    break;
                sb.Append(src[src_offset++]);
            }

            LastNameString = sb.ToString();
            srcString = LastNameString;
            SkipSpaces();
            if (int.TryParse(LastNameString, out LastInteger))//Wait, it'storage an integer!
            {
                LastToken = TokenId.Integer;
            }
            else//Just name string
            {
                if (invalidAsNameString)
                    CompilerError.Throw(CompilerErrorType.InvalidNameString, this);
                LastToken = TokenId.NameString;
            }
            return LastToken;
        }

        private bool IsSatoriStringChar(char c)
        {
            //if (c == '"' || c == '@' || c == '^' || c == ':' || c == ' ')
            if (c == '@')
                return true;
            else
                return false;
        }
        /// <summary>
        /// Insert a include file into current symbol stream.
        /// </summary>
        /// <param name="filename"></param>
        public void Include(string filename)
        {
            src = src.Insert(src_offset, File.ReadAllText(filename));
        }

        /// <summary>
        /// The boundary tokens are not included.
        /// </summary>
        public string ReadUntil(params TokenId[] tokens)
        {
            List<int> indexList = new List<int>();
            //int index = -1;
            for (int i = 0; i < SymbolList.Count; ++i)
                if (tokens.Contains(SymbolList[i]))
                {
                    indexList.Add(i);
                    //index = i;
                    //break;
                }
            if (indexList.Count == 0)
            {
                throw new Exception("Token not found!");
            }
            StringBuilder sb = new StringBuilder();
            while (src_offset != src.Length)
            {
                bool matched = false;
                foreach (int index in indexList)
                {
                    if (src.Length - src_offset >= SymbolStringList[index].Length && src.Substring(src_offset, SymbolStringList[index].Length) == SymbolStringList[index])
                    {
                        matched = true;
                        break;
                    }
                }
                if (matched)
                    break;
                else
                    sb.Append(src[src_offset++]);
            }
            return sb.ToString();
        }
        public string LastNameString = "";
        public int LastInteger = -1;
        public Modifier LastModifier;
        public TokenId LastToken;
        public bool EOF
        {
            get { SkipSpaces(); return src_offset == src.Length; }
        }
        public int CurrentLine
        {
            get
            {
                int ret = 0;
                for (int i = 0; i < src_offset; ++i)
                    if (src[i] == '\n') ++ret;
                return ret;
            }
        }
        public int CurrentCol
        {
            get
            {
                int ret = 1;
                for (int i = 0; i < src_offset; ++i)
                    if (src[i] == '\n') ret = 1;
                    else ++ret;
                return ret;
            }
        }
        public static bool IsAtomType(TokenId s)
        {
            return AtomType.NameTable.ContainsKey(s);
        }

        internal static bool IsModifier(TokenId s)
        {
            return ModifierList.Contains(s);
        }


        internal string ReadStringValue()
        {
            SkipSpaces();
            int start = 0;
            int length = 0;
            if (src[src_offset] == '\"')
            {
                start = src_offset;
                src_offset++;
            }
            else
            {
                CompilerError.Throw(CompilerErrorType.DoubleQuoteExpected, this);
            }



            while (true)
            {
                if (src[src_offset] == '\\')
                    src_offset += 2;
                else
                    src_offset++;
                if (src[src_offset] == '\"')
                {
                    src_offset++;
                    length = src_offset - start;
                    break;
                }
            }

            SkipSpaces();
            return src.Substring(start, length);

        }

        internal string ReadValueExpression()
        {
            int semicolon = SymbolList.FindIndex(x => x == TokenId.Semicolon);
            StringBuilder sb = new StringBuilder();
            bool InsideString = false;
            bool InsideSingleChar = false;
            bool Escape = false;
            while (src_offset != src.Length)
            {
                //deal with "xxx"
                if (InsideString)
                {
                    if (Escape)
                    {
                        Escape = false;
                    }
                    else if (src[src_offset] == '\\')
                        Escape = true;
                    else if (src[src_offset] == '\"')
                        InsideSingleChar = false;
                }
                else if (InsideSingleChar)
                {
                    if (Escape)
                    {
                        Escape = false;
                    }
                    else if (src[src_offset] == '\\')
                        Escape = true;
                    else if (src[src_offset] == '\'')
                        InsideSingleChar = false;
                }
                else
                {
                    if (src[src_offset] == ';')
                        break;
                    if (src[src_offset] == '\'')
                        InsideSingleChar = true;
                    if (src[src_offset] == '"')
                        InsideSingleChar = true;
                }
                sb.Append(src[src_offset++]);
            }
            if (ReadToken() != TokenId.Semicolon)
                CompilerError.Throw(CompilerErrorType.SemicolonExpected, this);
            return sb.ToString();
        }
    }
}
