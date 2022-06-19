using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.TSL.Metagen
{
    partial class Metagen
    {
        internal static List<MetaToken> GetTokens(ref string literal)
        {
            literal = Preprocess(literal);

            List<MetaToken> tokens = new List<MetaToken>();
            AddControlTokens(literal, tokens);
            AddLiteralTokens(literal, tokens);

            return tokens;
        }

        internal delegate string REGEX_CAT_FUNC(params string[] strs);
        internal delegate string REGEX_OP_FUNC(string str);

        internal static void AddControlTokens(string literal, List<MetaToken> tokens)
        {
            tokens.InsertFromRegex(literal, () => FOREACH);
            tokens.InsertFromRegex(literal, () => IF);
            tokens.InsertFromRegex(literal, () => ELIF);
            tokens.InsertFromRegex(literal, () => ELSE);
            tokens.InsertFromRegex(literal, () => END);
            tokens.InsertFromRegex(literal, () => TARGET);
            tokens.InsertFromRegex(literal, () => MAP_VAR);
            tokens.InsertFromRegex(literal, () => MAP_LIST);
            tokens.InsertFromRegex(literal, () => USE_LIST);
            tokens.InsertFromRegex(literal, () => GET_ITERATOR_VALUE);
            tokens.InsertFromRegex(literal, () => GET_ITERATOR_LENGTH);
            tokens.InsertFromRegex(literal, () => META);
            tokens.InsertFromRegex(literal, () => META_VAR);
            tokens.InsertFromRegex(literal, () => META_OUTPUT);
            tokens.InsertFromRegex(literal, () => TEMPLATE);
            tokens.InsertFromRegex(literal, () => LITERAL_OUTPUT);
            tokens.InsertFromRegex(literal, () => MODULE_CALL);
            tokens.InsertFromRegex(literal, () => MODULE_BEGIN);
            tokens.InsertFromRegex(literal, () => MODULE_END);
            tokens.InsertFromRegex(literal, () => MUTE);
            tokens.InsertFromRegex(literal, () => MUTE_END);

            tokens.SortAndReduce();
        }

        /**
         * Executed after all other tokens are added. Literal tokens are
         * segmented from the original source code, separated by the ranges
         * of the other tokens.
         */
        internal static void AddLiteralTokens(string literal, List<MetaToken> tokens)
        {
            for (int i = 0, size = tokens.Count; i <= size; ++i)
            {
                int startIndex;
                int length = 0;

                if (i == 0)
                    startIndex = 0;
                else
                    startIndex = tokens[i - 1].startIndex + tokens[i - 1].length;

                if (i < size)
                    length = tokens[i].startIndex - startIndex;
                else
                    length = literal.Length - startIndex;

                if (length > 0)
                    tokens.Add(MetaToken.LITERAL(literal, startIndex, length));
            }

            tokens.SortAndReduce();
        }
    }
}
