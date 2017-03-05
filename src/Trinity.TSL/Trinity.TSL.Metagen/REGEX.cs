using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Trinity.TSL.Metagen
{
    partial class Metagen
    {
        /** Most control tokens come in three flavors:
         *    1. \/\*CONTROL...\*\/
         *    2. [CONTROL...]
         *    3. CONTROL(...); (';' is optional, and parentheses matching is greedy!)
         *         Arbitrary number of underdash ('_') can be prepended to the call.
         *         This is for code indentation purpose.
         *  Pay attention that END has an extra form \/\*\*\/
         *  
         *  MODULE_CALL syntax:
         *    1. MODULE_CALL("MODULE_NAME" [, PARAMETERS]);
         *    2. [MODULE_CALL("MODULE_NAME" [, PARAMETER]")];
         *    3. \/\*MODULE_CALL("MODULE_NAME" [, PARAMETER])..
         *  
         *  META syntax:
         *    1. $identifier will be replaced by MAP_VAR targets
         *    2. %identifier will be replaced by META_VAR targets
         * 
         *  META_VAR(type,name_prefix):
         *    Declare a numbered variable of a particular type.
         *  
         *  META_OUTPUT(expression)
         *    Pull the value of a meta variable into template.
         */
        #region Helper functions
        static string         parameter_seq  = @"(?:[^""]|""(?:[^""\\]|\\.)*?"")";
        static REGEX_CAT_FUNC or             = (strs) => string.Join("|", strs);
        static REGEX_OP_FUNC  comment        = (str)  => @"\/\*\s*_*" + str + @"(?:[^\w](?:[^\/\*]|\/[^\*])*?)?\s*\*\/";
        static REGEX_OP_FUNC  square_bracket = (str)  => @"\[_*" + str + @"(?:[^\w\]]"+ parameter_seq + @"*?)?\]";
        static REGEX_OP_FUNC  func_call      = (str)  => @"_*" + str + @"\s*\(" + parameter_seq + @"*?\)[;]?";
        static REGEX_OP_FUNC  common_form    = (str)  => or(comment(str), square_bracket(str), func_call(str));
        #endregion

        #region REGEX
        static Regex FOREACH             = new Regex(common_form("FOREACH"));
        static Regex IF                  = new Regex(common_form("IF"));
        static Regex ELIF                = new Regex(common_form("ELIF"));
        static Regex ELSE                = new Regex(common_form("ELSE"));
        static Regex END                 = new Regex(or(common_form("END"), @"\/\*\*\/"));
        static Regex TARGET              = new Regex(common_form("TARGET"));
        static Regex MAP_VAR             = new Regex(common_form("MAP_VAR"));
        static Regex MAP_LIST            = new Regex(common_form("MAP_LIST"));
        static Regex USE_LIST            = new Regex(common_form("USE_LIST"));
        static Regex GET_ITERATOR_VALUE  = new Regex(common_form("GET_ITERATOR_VALUE"));
        static Regex GET_ITERATOR_LENGTH = new Regex(common_form("GET_ITERATOR_LENGTH"));
        static Regex META                = new Regex(common_form("META"));
        static Regex META_VAR            = new Regex(common_form("META_VAR"));
        static Regex META_OUTPUT         = new Regex(common_form("META_OUTPUT"));
        static Regex TEMPLATE            = new Regex(@"t_(\w|\d|_)*"); /** TEMPLATE: An identifier that start with t_*/
        static Regex LITERAL_OUTPUT      = new Regex(common_form("LITERAL_OUTPUT"));
        static Regex MODULE_CALL         = new Regex(common_form("MODULE_CALL"));
        static Regex MODULE_BEGIN        = new Regex(common_form("MODULE_BEGIN"));
        static Regex MODULE_END          = new Regex(common_form("MODULE_END"));
        static Regex MUTE                = new Regex(common_form("MUTE"));
        static Regex MUTE_END            = new Regex(common_form("MUTE_END"));
        #endregion


        private static string Preprocess(string literal)
        {
            //Use RegexOption.Multiline to allow regex to match against each line, not regarding the string as a whole.
            Regex __meta_remove              = new Regex(@"__meta\s*,\s*|\s*\:\s*__meta\s*$|__meta\s*\.", RegexOptions.Multiline);
            Regex line_comment_remove        = new Regex(@"(?<=^(?:[^""\\\/]|""(?:[^""\\]|\\.)*"")*)\/\/[^\/].*", RegexOptions.Multiline);
            Regex struct_indicator_operation = new Regex(@"(?<=\[STRUCT\]([^{])*)class", RegexOptions.Multiline);  /**Lookbehind and find [Struct], then find a class. If { is found, break.*/
            Regex struct_indicator_remove    = new Regex(@"\[STRUCT\]");
            Regex macro_remove               = new Regex(@"^\#.*", RegexOptions.Multiline);
            Regex collapse_lines             = new Regex(@"(^\s*$)+", RegexOptions.Multiline);

            literal = __meta_remove.Replace(literal, String.Empty);
            literal = line_comment_remove.Replace(literal, String.Empty);

            literal = struct_indicator_operation.Replace(literal, "struct");
            literal = struct_indicator_remove.Replace(literal, String.Empty);

            literal = macro_remove.Replace(literal, String.Empty);

            literal = collapse_lines.Replace(literal, String.Empty);

            return literal;
        }
    }
}
