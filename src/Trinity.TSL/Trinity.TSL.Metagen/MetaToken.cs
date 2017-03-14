using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Trinity.TSL.Metagen
{
    [AttributeUsage(AttributeTargets.All)]
    class PARAMAttribute : Attribute
    {
        public int param_count;
        public PARAMAttribute(int param_count = 0) { this.param_count = param_count; }
    }
    static class MetaTokenExtension
    {
        internal delegate MetaToken META_TOKEN_GEN_FUNC(string literal, int startIdx, int length);
        internal static void InsertFromRegex(this List<MetaToken> tokens, string literal, Expression<Func<Regex>> rgx_func)
        {
            var rgx      = rgx_func.Compile()();
            var rgx_name = rgx_func.ToString();
            int idx      = rgx_name.LastIndexOf('.') + 1;
            int len      = rgx_name.Length - idx;
            rgx_name = rgx_name.Substring(idx, len);

            foreach (var attr in typeof(MetaType).GetMember(rgx_name)[0].GetCustomAttributes(false))
            {
                var param_attr = (PARAMAttribute)attr;
                if (param_attr != null)
                {
                    tokens.AddRange(from Match m in rgx.Matches(literal) select MetaToken.GeneralFactory(literal, m.Index, m.Length, rgx_name, param_attr.param_count));
                    return;
                }
            }

            var meta_gen = typeof(MetaToken).GetMethod(rgx_name).CreateDelegate(typeof(META_TOKEN_GEN_FUNC)) as META_TOKEN_GEN_FUNC;

            tokens.AddRange(from Match m in rgx.Matches(literal) select meta_gen(literal, m.Index, m.Length));
        }

        /**
         * Removes all tokens that doesn't belong to a module.
         * Note that, if there are multiple modules defined in a
         * module template, only the first one will be retained.
         */
        internal static void FocusOnModule(this List<MetaToken> tokens)
        {
            int begin_idx      = tokens.FindIndex(t => t.type == MetaType.MODULE_BEGIN);
            int end_idx        = tokens.FindIndex(t => t.type == MetaType.MODULE_END);

            int module_size    = end_idx - begin_idx + 1;
            int end_slice_size = tokens.Count - module_size - begin_idx + 1;

            tokens.RemoveRange(0, begin_idx);
            tokens.RemoveRange(end_idx - begin_idx, end_slice_size);
        }

        internal static void SortAndReduce(this List<MetaToken> tokens)
        {
            tokens.Sort((a, b) =>
            {
                var ret = a.startIndex - b.startIndex;
                if (ret == 0)
                {
                    ret = a.length - b.length;
                    if (ret == 0)
                        if (a.type != b.type)
                        {
                            Console.WriteLine(a.ToString());
                            Console.WriteLine(b.ToString());
                            MetaToken.Throw("Different tokens with same range");
                        }
                }
                return ret;
            });
            for (int i=1; i<tokens.Count; ++i)
            {
                if (tokens[i-1].startIndex + tokens[i-1].length >= tokens[i].startIndex + tokens[i].length)
                {
                    tokens.RemoveAt(i);
                    --i;
                }
            }
        }
    }

    public enum MetaType
    {
        /*MANUAL*/
        LITERAL,
        [PARAM(-1)]
        FOREACH,
        [PARAM(1)]
        IF,
        [PARAM(1)]
        ELIF,
        [PARAM]
        ELSE,
        /*MANUAL*/
        END,

        [PARAM(1)]
        TARGET,

        [PARAM(-3)]
        MAP_VAR,
        [PARAM(-3)]
        MAP_LIST,
        [PARAM(1)]
        USE_LIST,
        [PARAM]
        GET_ITERATOR_VALUE,
        [PARAM]
        GET_ITERATOR_LENGTH,

        [PARAM(1)]
        META,
        [PARAM(-3)]
        META_VAR,
        [PARAM(1)]
        META_OUTPUT,

        /*MANUAL*/
        TEMPLATE,
        [PARAM(1)]
        LITERAL_OUTPUT,

        [PARAM(-100000)]
        MODULE_CALL,
        [PARAM]
        MODULE_BEGIN,
        [PARAM]
        MODULE_END,

        [PARAM]
        MUTE,
        [PARAM]
        MUTE_END,
    }

    class MetaToken
    {

        #region Fields
        public MetaType type;
        public string original_text;
        public List<string> text = new List<string>();
        public int startIndex;
        public int length;
        #endregion

        #region Helper functions
        static void CheckParamCount(List<string> list, int count, string memberName = "")
        {
            if (count >= 0 && list.Count != count)
            {
                Throw(memberName + " require " + count + " parameters, got " + list.Count);
            }
            else if (count < 0 && list.Count + count > 0)
            {
                Throw(memberName + ": too many parameters, at most " + (-count) + " got " + list.Count);
            }
        }

        public static void Throw(string msg)
        {
            Console.WriteLine(msg);
            Console.WriteLine(Environment.StackTrace);
            throw new Exception(msg);
        }

        static List<string> CaptureArguments(string text, int startIdx, int len)
        {

            List<string> ret     = new List<string>();
            text                 = text.Substring(startIdx, len);

            /**
             * The meta call is in the form of [HEAD]METHOD_IDENTIFIER(...)[TAIL]
             * [HEAD] is one of the common forms, [, /*, space(\s), or nothing
             * METHOD_IDENTIFIER is an identifier (\w\d_)
             * [TAIL] is what corresponds to [HEAD], or ';' (usually in a statement)
             * Remove head: remove [HEAD] and METHOD_IDENTIFIER, also the preamble '('
             * Remove tail: remove [TAIL], also the postfix ')'
             */

            Regex head_rgx       = new Regex(@"^[\/\*_\w\d\s\[]*\(?");
            Regex tail_rgx       = new Regex(@"(\)?(\]|\*\/|;)|\))$");
            text                 = head_rgx.Replace(text, "");
            text                 = tail_rgx.Replace(text, "");

            /**
             * Parameter syntax:
             *      1. "param surrounded by quotes, can have ',' inside "
             *      2. param_outside_quote
             */

            Regex param_regex  = new Regex(@"\s*(?:(?<param>(?:""(?:[^""\\]|\\.)*?"")|[^,""]+(?:""(?:[^""\\]|\\.)*?"")?)\s*[,]?\s*)*");
            Match param_match  = param_regex.Match(text);
            Group param_group  = param_match.Groups["param"];

            foreach (Capture param_capture in param_group.Captures)
            {
                string val = param_capture.Value;
                if (val.StartsWith("\"", StringComparison.Ordinal) && val.EndsWith("\"", StringComparison.Ordinal))
                    val = val.Substring(1, val.Length - 2);
                val = val.Replace("\\\"", "\"");
                ret.Add(val);
            }

            return ret;
        }
        #endregion

        #region Factory
        public static MetaToken LITERAL(string text, int startIdx, int len)
        {
            var meta           = new MetaToken();
            meta.startIndex    = startIdx;
            meta.length        = len;
            meta.type          = MetaType.LITERAL;
            meta.original_text = text;
            meta.text.Add(text.Substring(startIdx, len));

            return meta;
        }

        public static MetaToken END(string text, int startIdx, int len)
        {
            var meta           = new MetaToken();
            meta.startIndex    = startIdx;
            meta.length        = len;
            meta.type          = MetaType.END;
            meta.original_text = text;

            return meta;
        }

        public static MetaToken TEMPLATE(string text, int startIdx, int len)
        {
            var meta           = new MetaToken();
            meta.startIndex    = startIdx;
            meta.length        = len;
            meta.type          = MetaType.TEMPLATE;
            meta.original_text = text;
            meta.text.Add(text.Substring(startIdx, len));

            return meta;
        }

        public static MetaToken GeneralFactory(string text, int startIdx, int len, string type_str, int param_count)
        {
            var meta            = new MetaToken();
            var type            = (MetaType)Enum.Parse(typeof(MetaType), type_str);
            meta.original_text  = text;
            try
            {
                meta.startIndex = startIdx;
                meta.length     = len;
                meta.type       = type;
                meta.text       = CaptureArguments(text, startIdx, len);

                CheckParamCount(meta.text, param_count, type_str);
            }
            catch (Exception e)
            {
                Console.WriteLine("Current token:");
                Console.WriteLine(meta);
                Console.WriteLine("Text:");
                Console.WriteLine(text.Substring(startIdx, len));

                throw;
            }

            return meta;
        }

        #endregion

        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, @"
start = {0}  length = {1}
line = {2}
type  = {3}
args  = {4}
", startIndex, length, getLine(), type, string.Join(" ; ", text));
        }

        private int getLine()
        {
            return original_text.Take(startIndex).Count(chr => chr == '\n') + 1;
        }
    }
}
