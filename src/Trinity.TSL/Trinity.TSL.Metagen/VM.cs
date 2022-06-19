using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Trinity.TSL.Metagen
{
    class VMState
    {
        internal int    if_depth         = 0;
        internal int    foreach_depth    = 0;
        internal int    ip               = 0;
        internal string target           = null;
        internal bool   muted            = false;

        public VMState()
        {
            if_depth      = 0;
            foreach_depth = 0;
            ip            = 0;
            target        = null;
            muted         = false;
        }

        public VMState(VMState vMState)
        {
            if_depth      = vMState.if_depth;
            foreach_depth = vMState.foreach_depth;
            ip            = 0;
            target        = vMState.target;
            muted         = vMState.muted;
        }

    }

    class VM
    {
        private StringBuilder                     source;
        private List<MetaToken>                   instructions;
        private string                            name;
        private Stack<MetaToken>                  stack;
        private Stack<string>                     separator_stack;
        private VMState                           state;
        private Dictionary<string,string>         d_var;
        private Dictionary<string,string>         d_var_explicit_host;
        private Dictionary<string,string>         d_list;
        private Dictionary<string,int>            d_list_iterator_depth;
        private Dictionary<string,string>         d_list_explicit_host;
        private Dictionary<string,int>            d_metavar_depth;
        private Dictionary<MetaToken,MetaToken>   d_iteratable_target;
        private List<string>                      l_literal_buffer;
        private bool                              b_is_sub_vm;
        private bool                              b_is_module;

        private delegate void InstructionDelegate();
        private Dictionary<string, InstructionDelegate> d_instruction_method_table;
        private static int                              s_MAX_LITERAL_LENGTH = 1024;


        private MetaToken current_instruction
        {
            get { return instructions[state.ip]; }
        }

        public VM(StringBuilder src, List<MetaToken> insts, string name)
        {
            source        = src;
            instructions  = insts;
            this.name     = name;
        }

        public VM(VM vM, StringBuilder sub_vm_literal, List<MetaToken> meta_tokens)
        {
            source                = sub_vm_literal;
            instructions          = meta_tokens;
            name         = vM.name;
            stack                 = new Stack<MetaToken>();
            separator_stack       = new Stack<string>(vM.separator_stack);
            state                 = new VMState(vM.state);
            d_var                 = new Dictionary<string, string>(vM.d_var);
            d_var_explicit_host   = new Dictionary<string, string>(vM.d_var_explicit_host);
            d_list                = new Dictionary<string, string>(vM.d_list);
            d_list_iterator_depth = new Dictionary<string, int>(vM.d_list_iterator_depth);
            d_list_explicit_host  = new Dictionary<string, string>(vM.d_list_explicit_host);
            d_metavar_depth       = new Dictionary<string, int>(vM.d_metavar_depth);
            d_iteratable_target   = new Dictionary<MetaToken, MetaToken>(vM.d_iteratable_target);
            l_literal_buffer      = new List<string>();
            b_is_sub_vm           = true;
        }

        public void LITERAL()
        {
            OutputLiteralToTemplate(arg(0));
        }

        public void FOREACH()
        {
            Push();

            string sep = "";
            if (argc() > 0)
                sep = arg(0);

            separator_stack.Push(sep);

            int end_idx                              = Find_END();
            var iteratable                           = Find_LISTVAR();
            d_iteratable_target[current_instruction] = iteratable;

            //if (iteratable.text[0].Contains("array"))
            //    Console.WriteLine("Hit");

            META_SetListDepth(iteratable);

            OutputToMeta(
                "for (size_t " + META_ForeachIteratorName() + " = 0; " + 
                META_ForeachIteratorName() + " < " + META_GetListSize(iteratable) + ";" +
                "++" + META_ForeachIteratorName() + ")\r\n{");
        }


        public void IF()
        {
            Push();
            ++state.if_depth;

            OutputToMeta("if (" + META_Translate(arg(0)) + ")\r\n{");
        }


        public void ELIF()
        {
            OutputToMeta("}\r\nelse if (" + META_Translate(arg(0)) + ")\r\n{");
        }

        public void ELSE()
        {
            OutputToMeta("}\r\nelse\r\n{");
        }

        public void END()
        {
            Pop();
            OutputToMeta("}");
        }

        public void TARGET()
        {
            state.target = arg(0);
        }

        private string MAP_FIND_EXPLICIT_HOST()
        {
            if (argc() > 2)//Find explicit host
            {
                string explicit_host        = arg(2);
                Regex explicit_host_rgx     = new Regex(@"MemberOf\s*=\s*""(?<host>.*)""");
                Match explicit_host_match   = explicit_host_rgx.Match(explicit_host);
                if (!explicit_host_match.Success)
                    throw new Exception("MAP_VAR syntax error");
                return explicit_host_match.Groups["host"].ToString();
            }
            return null;
        }

        public void MAP_VAR()
        {
            d_var[arg(0)] = arg(1);
            var explicit_host = MAP_FIND_EXPLICIT_HOST();
            if (explicit_host != null)
                d_var_explicit_host[arg(0)] = explicit_host;
            else
                d_var_explicit_host.Remove(arg(0));
        }

        public void MAP_LIST()
        {
            d_list[arg(0)] = arg(1);
            var explicit_host = MAP_FIND_EXPLICIT_HOST();
            if (explicit_host != null)
                d_list_explicit_host[arg(0)] = explicit_host;
            else
                d_var_explicit_host.Remove(arg(0));
        }

        public void USE_LIST()
        {
            /* Do nothing when this token is actually hit */
        }

        public void GET_ITERATOR_VALUE()
        {
            string meta_code = META_Translate(META_ForeachIteratorName());

            if (b_is_sub_vm)
            {
                OutputMetaStringToTemplate(meta_code);
            }
            else
            {
                meta_code = META_GetString(meta_code);
                OutputMetaStringToTemplate(meta_code);
            }

        }

        public void GET_ITERATOR_LENGTH()
        {
            string meta_code = META_Translate(META_GetListSize(META_GetIteratable()));

            if (b_is_sub_vm)
            {
                OutputMetaStringToTemplate(meta_code);
            }
            else
            {
                meta_code = META_GetString(meta_code);
                OutputMetaStringToTemplate(meta_code);
            }

        }

        public void META()
        {
            string meta_code = META_Translate(arg(0));
            OutputToMeta(meta_code);
        }

        private string META_Translate(string meta_code)
        {
            meta_code = META_TranslateMapVar(meta_code);
            meta_code = META_TranslateMetaVar(meta_code);
            return meta_code;
        }

        private string META_TranslateMapVar(string meta_code)
        {
            Regex map_var_rgx  = new Regex(@"\$[\w|\d|_]+");
            foreach (Match m in map_var_rgx.Matches(meta_code))
            {
                var name = m.Value.Substring(1);
                var substitution = META_GetVar(name);
                Regex substitute_rgx = new Regex(@"(?<![\w|\d|_])\" + m.Value + @"(?![\w|\d|_])");
                meta_code = substitute_rgx.Replace(meta_code, substitution);
            }
            return meta_code;
        }

        private string META_TranslateMetaVar(string meta_code)
        {
            Regex meta_var_rgx = new Regex(@"\%[\w|\d|_]+");
            foreach (Match m in meta_var_rgx.Matches(meta_code))
            {
                var name  = m.Value.Substring(1);
                name      = name + "_" + d_metavar_depth[name];
                meta_code = meta_code.Replace(m.Value, name);
            }
            return meta_code;
        }

        /**
         * MapVar source syntax:
         *  1. Direct map; When the template name does not contain a prefix
         *     of another list, the MetaForm is directly expanded.
         *  2. Prefix map; When the template name contains a list var as
         *     a prefix, the list var is first accessed with proper offset, 
         *     then the meta form of the variable is appended as a 
         *     pointer dereference. Note that, if it is desired to use the
         *     elements in a list directly, a variable with the same name of
         *     the list should be declared.
         *  3. MemberOf map; When explicitly specified with 
         *     MemberOf = "template", list expansion is enforced, even
         *     if not a prefix.
         * MapVar target syntax:
         *  1. Direct map; When the template target does not contain '$$', 
         *     and is not a member of a list, it will be translated directly.
         *  2. Member map; When the template target does not contain '$$',
         *     and is a member of a list, it will be translated to:
         *     get_list(..)[item_offset]->member
         *  3. Function map; When the template target contains '$$', and
         *     is a member of a list, it will be translated to:
         *     function(get_list(..)[item_offset])
         *  4. Additionally, the MapVar target string will run through VM
         *     again so that arbitrary META command can be executed.
         */
        private string META_GetVar(string var_name)
        {
            string list_prefix = LISTVAR_GetListName(var_name);
            string result;

            if(!d_var.TryGetValue(var_name, out result))
            {
                throw new Exception("META_GetVar: undefined variable " + var_name);
            }

            /* Interpret META commands (if any) in the MAP_VAR target string */
            List<MetaToken> meta_tokens = Metagen.GetTokens(ref result);
            StringBuilder sub_vm_literal = new StringBuilder();
            VM sub_vm = new VM(this, sub_vm_literal, meta_tokens);

            sub_vm.Execute_Core();

            /* Translate variables in the target representation. */
            result = META_Translate(sub_vm_literal.ToString());

            if (list_prefix != null)
            {
                result = META_GetListItem(list_prefix, result);
            }
            return result;
        }

        private string META_GetList(string list_name)
        {
            string host = null;
            if (!d_list_explicit_host.TryGetValue(list_name, out host))
                /* No host found. It's a standalone list. */
                return META_Translate(d_list[list_name]);
            //TODO $$
            try
            {
                return "(*(" + META_GetList(host) + "))[" + META_ForeachIteratorName(META_GetListDepth(host)) + "]->" + d_list[list_name];
            }
            catch /* the host is standalone. */
            {
                return META_GetVar(host) + "->" + d_list[list_name];
            }
        }

        private string META_GetListItem(string list_name, string element_name)
        {
            string result = "(*(" + META_GetList(list_name) + "))[" + META_ForeachIteratorName(META_GetListDepth(list_name)) + "]";
            if (element_name != String.Empty)
            {
                if (element_name.Contains("$$"))
                {
                    result = element_name.Replace("$$", result);
                }
                else
                {
                    result += "->" + element_name;
                }
            }
            return result;
        }

        private string META_GetString(string expr)
        {
            return "Codegen::GetString(" + expr + ")";
        }

        private string META_GetListSize(MetaToken listvar)
        {
            var list_prefix = LISTVAR_GetListName(listvar.text[0]);
            var list_var    = META_GetList(list_prefix);
            return "("+list_var+")->size()";
        }

        private MetaToken META_GetIteratable()
        {
            for (int idx = stack.Count - 1; idx >= 0; --idx)
                if (stack.ElementAt(idx).type == MetaType.FOREACH)
                    return d_iteratable_target[stack.ElementAt(idx)];
            return null;
        }

        private void META_SetListDepth(MetaToken listvar)
        {
            //XXX currently nested iteration over a same list is impossible,
            //as we only have a flat dict string->int for the iteration depth,
            //and inner depth will overwrite the outer one.
            var list_prefix = LISTVAR_GetListName(listvar.text[0]);
            d_list_iterator_depth[list_prefix] = state.foreach_depth;
        }

        private int META_GetListDepth(string listvar)
        {
            return d_list_iterator_depth[listvar];
        }

        private string META_ForeachIteratorName(int depth = -1)
        {
            if (depth == -1)
                depth = state.foreach_depth;
            return "iterator_" + depth;
        }

        public void META_VAR()
        {
            string s_type = arg(0);
            string s_name = arg(1);
            int depth = 0;

            if (d_metavar_depth.ContainsKey(s_name))
                depth = ++d_metavar_depth[s_name];
            else
                depth = d_metavar_depth[s_name] = 1;

            if (argc() == 2)
                OutputToMeta(s_type + " " + s_name + "_" + depth + ";");
            else if (argc() == 3)
                OutputToMeta(s_type + " " + s_name + "_" + depth + " = " + META_Translate(arg(2)) + ";");
            else throw new Exception("META_VAR syntax error");
        }

        public void META_OUTPUT()
        {
            string meta_code = META_Translate(arg(0));

            meta_code = META_GetString(meta_code);
            OutputMetaStringToTemplate(meta_code);
        }

        /**
         * Template substitution rule:
         * If there is a list prefix, regard this template as a list item.
         * Else, if there is a var prefix, regard this template as a var plus
         * literal suffix.
         * Otherwise, regard this template as a var.
         */

        public void TEMPLATE()
        {
            var template    = arg(0);
            var var_prefix  = VAR_prefix(template);
            var list_prefix = LISTVAR_GetListName(template);
            try
            {
                if (var_prefix == null)
                    OutputMetaStringToTemplate(META_GetString(META_GetVar(template)));
                else
                {
                    OutputMetaStringToTemplate(META_GetString(META_GetVar(var_prefix)));
                    OutputLiteralToTemplate(template.Substring(var_prefix.Length));
                }
            }
            catch (Exception)
            {
                Console.Write("\nWarning: Unrecognized template '{0}', output literal instead.", template);
                OutputLiteralToTemplate(template);
            }
        }

        public void LITERAL_OUTPUT()
        {
            var template    = META_Translate(arg(0));
            OutputLiteralToTemplate(template);
        }

        public void MODULE_CALL()
        {
            string module_call_code;
            if (b_is_module)
            {
                module_call_code = @"
{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = context->m_stack_depth + 1;
";
            }
            else
            {
                module_call_code = @"
{
    ModuleContext module_ctx;
    module_ctx.m_stack_depth = 0;
";
            }

            for (int i = 2; i < argc(); ++i)
            {
                module_call_code += @"module_ctx.m_arguments.push_back(" + META_GetString(META_Translate(arg(i))) + @");
";
            }

            module_call_code += "std::string* module_content = Modules::" + arg(0) + "(" + META_Translate(arg(1)) + @", &module_ctx);
    source->append(*module_content);
    delete module_content;
}";

            OutputToMeta(module_call_code);
        }

        public void MODULE_BEGIN()
        {
            // MODULE_BEGIN is ignored.
        }

        public void MODULE_END()
        {
            // MODULE_END is ignored.
        }

        public void MUTE()
        {
            state.muted = true;
        }

        public void MUTE_END()
        {
            state.muted = false;
        }

        private void GenerateTemplatePrologue()
        {
            OutputToMeta(
@"#include ""common.h""
#include <string>
#include ""SyntaxNode.h""

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* "); OutputToMeta(name + "("); OutputToMeta(FindTarget() + @"* node)
        {
            string* source = new string();
            ");
        }


        private void GenerateTemplateEpilogue()
        {
            OutputToMeta(
@"
            return source;
        }
    }
}");
        }

        private void GenerateModulePrologue()
        {
            OutputToMeta(
@"#include ""common.h""
#include <string>
#include ""SyntaxNode.h""

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        namespace Modules
        {
            string* "); OutputToMeta(name + "("); OutputToMeta(FindTarget() + @"* node, ModuleContext* context)
            {
                string* source = new string();
                ");
        }

        private void GenerateModuleEpilogue()
        {
            OutputToMeta(
@"
                return source;
            }
        }
    }
}");
        }


        private bool IsModule()
        {
            return instructions.Any(inst => inst.type == MetaType.MODULE_BEGIN);
        }

        private string FindTarget()
        {
            foreach (var token in instructions)
                if (token.type == MetaType.TARGET)
                    return token.text[0];
            throw new Exception("No target found.");
        }

        private void InitializeMethodTable()
        {
            d_instruction_method_table = new Dictionary<string, InstructionDelegate>();
            Type vm_type = typeof(VM);

            foreach (var method_desc in vm_type.GetMethods())
            {
                if (method_desc.Name == method_desc.Name.ToUpperInvariant())
                {
                    d_instruction_method_table[method_desc.Name] = (InstructionDelegate)Delegate.CreateDelegate(typeof(InstructionDelegate), this, method_desc);
                }
            }
        }

        public void Execute()
        {
            b_is_module = IsModule();
            var target  = FindTarget();

            if (b_is_module)
            {
                GenerateModulePrologue();
                instructions.FocusOnModule();
            }
            else
            {
                GenerateTemplatePrologue();
            }

            Reset();

            Execute_Core();

            if (b_is_module)
            {
                GenerateModuleEpilogue();
            }
            else
            {
                GenerateTemplateEpilogue();
            }
        }

        private void Execute_Core()
        {
            InitializeMethodTable();

            for (; state.ip < instructions.Count; ++state.ip)
            {
                if (state.muted)
                {
                    if (current_instruction.type == MetaType.MUTE_END)
                        state.muted = false;
                }
                else
                {
                    //var func = typeof(VM).GetMethod(current_instruction.type.ToString());
                    try
                    {
                        //func.Invoke(this, null);
                        d_instruction_method_table[current_instruction.type.ToString()]();
                    }
                    catch 
                    {
                        Console.WriteLine("Current instruction:");
                        Console.WriteLine(current_instruction);
                        throw;
                    }
                }
            }
            FlushLiteralBuffer();

            if (stack.Count != 0)
            {
                Console.WriteLine("\nStack not empty after executing all instructions.");
                throw new Exception();
            }
        }


        private string arg(int idx)
        {
            return current_instruction.text[idx];
        }

        private int argc()
        {
            return current_instruction.text.Count;
        }

        private void Push()
        {
            stack.Push(current_instruction);
            switch (current_instruction.type)
            {
                case MetaType.FOREACH:
                    ++state.foreach_depth;
                    break;
                case MetaType.IF:
                    ++state.if_depth;
                    break;
            }
        }

        private void Pop()
        {
            var inst = stack.Pop();
            switch (inst.type)
            {
                case MetaType.FOREACH:
                    string sep = separator_stack.Pop();
                    if (sep != String.Empty)
                    {
                        OutputToMeta("if (" + 
                        META_ForeachIteratorName() + " < " + 
                        META_GetListSize(d_iteratable_target[inst]) + " - 1)");
                        OutputMetaStringToTemplate('"'+sep+'"');
                    }
                    --state.foreach_depth;
                    break;
                case MetaType.IF:
                    --state.if_depth;
                    break;
            }
        }

        private MetaToken Find_LISTVAR()
        {
            //XXX only scans for the first LIST, when nested and inner loop has list in the front
            //it will be picked up by outer loop
            //Workaround: introduce USE_LIST
            return instructions[instructions.FindIndex(state.ip, i => Is_LISTVAR(i))];
        }

        private bool Is_LISTVAR(MetaToken i)
        {
            if (i.type == MetaType.USE_LIST)
                return true;
            if (i.type != MetaType.TEMPLATE)
                return false;
            var template_name = i.text[0];
            if (null != LISTVAR_GetListName(template_name))
                return true;
            return false;
        }

        private string LISTVAR_GetListName(string str)
        {
            if (d_var_explicit_host.ContainsKey(str))
                return d_var_explicit_host[str];
            string list_name = null;
            foreach (var kvp in d_list)
                if (str.StartsWith(kvp.Key, StringComparison.Ordinal))
                    if (list_name == null || list_name.Length < kvp.Key.Length)
                        list_name = kvp.Key;
            if (list_name != null)
                return list_name;
            return null;
        }

        /**
         * The rule of matching VAR prefix:
         *   1. There is a MAP_VAR which is a proper prefix of the string
         *     *AND*
         *   2. If multiple prefixes exist, choose the longest one.
         *   3. If str is a defined variable, drop any var prefix.
         *   
         *   (A proper prefix is a strict prefix, that is, the length is smaller)
         */
        private string VAR_prefix(string str)
        {
            if (d_var.ContainsKey(str))
                return null;
            string var_prefix = null;
            foreach (var kvp in d_var)
                if (str.StartsWith(kvp.Key, StringComparison.Ordinal) && str.Length != kvp.Key.Length)
                {
                    if (var_prefix == null || var_prefix.Length < kvp.Key.Length)
                        var_prefix = kvp.Key;
                }
            return var_prefix;
        }

        private int Find_END()
        {
            int required_end = 1;
            for (int idx = state.ip + 1; idx < instructions.Count; ++idx)
            {
                switch (instructions[idx].type)
                {
                    case MetaType.FOREACH:
                    case MetaType.IF:
                        required_end++;
                        break;
                    case MetaType.END:
                        required_end--;
                        if (required_end == 0)
                            return idx;
                        break;
                }
            }
            throw new Exception("Unmatched END");
        }

        private void Reset()
        {
            stack                     = new Stack<MetaToken>();
            separator_stack           = new Stack<string>();
            state                     = new VMState();
            d_var                     = new Dictionary<string, string>();
            d_var_explicit_host       = new Dictionary<string, string>();
            d_list                    = new Dictionary<string, string>();
            d_list_iterator_depth     = new Dictionary<string, int>();
            d_list_explicit_host      = new Dictionary<string, string>();
            d_metavar_depth           = new Dictionary<string, int>();//TODO should be able to pop meta var?
            d_iteratable_target       = new Dictionary<MetaToken, MetaToken>();
            l_literal_buffer          = new List<string>();
            b_is_sub_vm               = false;
        }

        private void FlushLiteralBuffer()
        {
            if (b_is_sub_vm)
            {
                source.Append(string.Join("", l_literal_buffer));
                l_literal_buffer.Clear();
                return;
            }

            if (l_literal_buffer == null || l_literal_buffer.Count == 0)
                return;

            /**
             * Format logic:
             *  1. Replace continuous <CR><CR> with a single <CR>
             *  2. Then, replace <CR><spaces><CR> with a single<CR>,
             *     unless it follows a '}', or #region, #endregion.
             */
            var text = string.Join("", l_literal_buffer);
            var rgx  = new Regex(@"(?:\r?\n){2,}");
            text     = rgx.Replace(text, "\r\n");
            rgx      = new Regex(@"(?<!\}|\#.*)\r\n\s*\r\n");
            text     = rgx.Replace(text, "\r\n");

            /**
             * We chop literals into chunks of max size s_MAX_LITERAL_LENGTH.
             * This is due to that MSVC has a maximum string length restriction.
             */
            int len = text.Length;
            int pos = 0;

            while (len > 0)
            {
                source.Append("source->append(R\"::(");

                if (len <= s_MAX_LITERAL_LENGTH)
                {
                    source.Append(text.Substring(pos, len));
                    len = 0;
                }
                else
                {
                    source.Append(text.Substring(pos, s_MAX_LITERAL_LENGTH));
                    len -= s_MAX_LITERAL_LENGTH;
                    pos += s_MAX_LITERAL_LENGTH;
                }

                source.Append(")::\");\r\n");
            }

            l_literal_buffer.Clear();
        }

        private void OutputLiteralToTemplate(string text)
        {
            if (0 == text.Count(x => !char.IsWhiteSpace(x)))
            {
                if(text.Contains('\n'))
                    return;
            }
            l_literal_buffer.Add(text);
        }

        /**
         * Output directly into codegen, rather than template.
         */
        private void OutputToMeta(string meta_code)
        {
            FlushLiteralBuffer();
            source.Append(meta_code);
            if (!b_is_sub_vm)
                source.Append("\r\n");
        }

        private void OutputMetaStringToTemplate(string meta_expr)
        {
            FlushLiteralBuffer();
            if (b_is_sub_vm)
                OutputToMeta(meta_expr);
            else
                OutputToMeta("source->append(" + meta_expr + ");");
        }
    }
}
