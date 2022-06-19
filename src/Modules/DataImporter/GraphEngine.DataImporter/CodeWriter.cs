using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace GraphEngine.Utilities
{
    internal class CodeWriter
    {
        private StringBuilder sb;
        private int indent = 0;

        int spaces_per_tab = 4;
        string space_tab = "    ";

        bool use_spaces = true;

        public bool UseSpaces
        {
            get
            {
                return use_spaces;
            }

            set
            {
                use_spaces = value;
            }
        }

        public int SpacesPerTab
        {
            get
            {
                return spaces_per_tab;
            }

            set
            {
                spaces_per_tab = value;
                space_tab = "";
                for (int i = 0; i < spaces_per_tab; i++)
                    space_tab += ' ';
            }
        }

        string T1
        {
            get
            {
                if (use_spaces)
                    return space_tab;
                else
                    return "\t";
            }
        }
        string T2
        {
            get
            {
                if (use_spaces)
                    return space_tab + space_tab;
                else
                    return "\t\t";
            }
        }
        string T3
        {
            get
            {
                if (use_spaces)
                    return space_tab + space_tab + space_tab;
                else
                    return "\t\t\t";
            }
        }

        string T4
        {
            get
            {
                if (use_spaces)
                    return space_tab + space_tab + space_tab + space_tab;
                else
                    return "\t\t\t\t";
            }
        }

        string T5
        {
            get
            {
                if (use_spaces)
                    return space_tab + space_tab + space_tab + space_tab + space_tab;
                else
                    return "\t\t\t\t\t";
            }
        }

        string T6
        {
            get
            {
                if (use_spaces)
                    return space_tab + space_tab + space_tab + space_tab + space_tab + space_tab;
                else
                    return "\t\t\t\t\t\t";
            }
        }

        public CodeWriter(int indent = 0)
        {
            this.indent = indent;
            sb = new StringBuilder();
        }
        public CodeWriter(string value, int indent = 0)
        {
            sb = new StringBuilder(value);
            this.indent = indent;
        }

        public int Indent
        {
            get
            {
                return indent;
            }
            set
            {
                indent = value;
            }
        }

        internal string S
        {
            set
            {
                AddDefaultIndent();
                sb.Append(value);
            }
        }

        public static CodeWriter operator +(CodeWriter cw, string value)
        {
            cw.sb.Append(value);
            return cw;
        }

        public static CodeWriter operator +(CodeWriter cw1, CodeWriter cw2)
        {
            cw1.sb.Append(cw2.sb.ToString());
            return cw1;
        }

        private void AddDefaultIndent()
        {
            for (int i = 0; i < indent; i++)
            {
                sb.Append(T1);
            }
        }

        internal string S1
        {
            set
            {
                AddDefaultIndent();
                sb.Append(T1);
                sb.Append(value);
            }
        }

        internal string S2
        {
            set
            {
                AddDefaultIndent();
                sb.Append(T2);
                sb.Append(value);
            }
        }

        internal string S3
        {
            set
            {
                AddDefaultIndent();
                sb.Append(T3);
                sb.Append(value);
            }
        }

        internal string S4
        {
            set
            {
                AddDefaultIndent();
                sb.Append(T4);
                sb.Append(value);
            }
        }

        internal string S5
        {
            set
            {
                AddDefaultIndent();
                sb.Append(T5);
                sb.Append(value);
            }
        }

        internal string S6
        {
            set
            {
                AddDefaultIndent();
                sb.Append(T6);
                sb.Append(value);
            }
        }

        internal void Left()
        {
            AddDefaultIndent();
            sb.AppendLine("{");
            indent++;
        }

        internal void Right()
        {
            indent--;
            AddDefaultIndent();
            sb.AppendLine("}");
        }

        internal string SL
        {
            set
            {
                AddDefaultIndent();
                sb.AppendLine(value);
            }
        }

        internal string SL1
        {
            set
            {
                AddDefaultIndent();
                sb.Append(T1);
                sb.AppendLine(value);
            }
        }

        internal string SL2
        {
            set
            {
                AddDefaultIndent();
                sb.Append(T2);
                sb.AppendLine(value);
            }
        }

        internal string SL3
        {
            set
            {
                AddDefaultIndent();
                sb.Append(T3);
                sb.AppendLine(value);
            }
        }

        internal string SL4
        {
            set
            {
                AddDefaultIndent();
                sb.Append(T4);
                sb.AppendLine(value);
            }
        }

        internal string SL5
        {
            set
            {
                AddDefaultIndent();
                sb.Append(T5);
                sb.AppendLine(value);
            }
        }

        internal string SL6
        {
            set
            {
                AddDefaultIndent();
                sb.Append(T6);
                sb.AppendLine(value);
            }
        }

        internal CodeWriter Append(string s)
        {
            AddDefaultIndent();
            sb.Append(s);
            return this;
        }

        internal CodeWriter Append(char c)
        {
            AddDefaultIndent();
            sb.Append(c);
            return this;
        }

        internal void TemplateWrite(string tmpl, params object[] args)
        {
            string msg = tmpl;
            for (int i = 0; i < args.Length; i++)
            {
                msg = msg.Replace("{" + i + "}", args[i].ToString());
            }
            sb.Append(msg);
        }

        internal void W()
        {
            AddDefaultIndent();
        }

        internal void W(string space, int count, string message)
        {
            for (int i = 0; i < count; i++)
                sb.Append(space);
            if (message != null)
                sb.Append(message);
        }

        internal void W(string format, params object[] args)
        {
            AddDefaultIndent();
            if (args == null || args.Length == 0)
                sb.Append(format);
            else
                sb.AppendFormat(CultureInfo.InvariantCulture, format, args);
        }

        internal void W(int indent, string format, params object[] args)
        {
            for (int i = 0; i < indent; i++)
                sb.Append(T1);
            if (args == null || args.Length == 0)
                sb.Append(format);
            else
                sb.AppendFormat(CultureInfo.InvariantCulture, format, args);
        }

        internal void W1(string format, params object[] args)
        {
            sb.Append(T1);
            W(format, args);
        }

        internal void W2(string format, params object[] args)
        {
            sb.Append(T2);
            W(format, args);
        }

        internal void W3(string format, params object[] args)
        {
            sb.Append(T3);
            W(format, args);
        }

        internal void WL()
        {
            sb.AppendLine();
        }

        internal void WL(string space, int count, string message)
        {
            for (int i = 0; i < count; i++)
                sb.Append(space);
            if (message == null)
                sb.AppendLine(message);
        }

        internal void WL(string format, params object[] args)
        {
            AddDefaultIndent();
            if (args == null || args.Length == 0)
                sb.AppendLine(format);
            else
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        internal void WL(int indent, string format, params object[] args)
        {
            for (int i = 0; i < indent; i++)
                sb.Append(T1);
            if (args == null || args.Length == 0)
                sb.AppendLine(format);
            else
                sb.AppendLine(string.Format(CultureInfo.InvariantCulture, format, args));
        }

        internal void WL1(string format, params object[] args)
        {
            sb.Append(T1);
            WL(format, args);
        }

        internal void WL2(string format, params object[] args)
        {
            sb.Append(T2);
            WL(format, args);
        }

        internal void WL3(string format, params object[] args)
        {
            sb.Append(T3);
            WL(format, args);
        }
        internal void WL4(string format, params object[] args)
        {
            sb.Append(T4);
            WL(format, args);
        }

        internal void WL5(string format, params object[] args)
        {
            sb.Append(T5);
            WL(format, args);
        }

        public CodeWriter Remove(int startIndex)
        {
            this.sb.Remove(startIndex, sb.Length - startIndex);
            return this;
        }

        public int Length
        {
            get
            {
                return sb.Length;
            }
        }

        public CodeWriter TrimEnd(params char[] trimChars)
        {
            while (sb.Length > 0)
            {
                int index = sb.Length - 1;
                if (trimChars.Contains(sb[index]))
                {
                    sb.Remove(index, 1);
                }
                else
                    break;
            }
            return this;
        }

        public CodeWriter TrimStart(params char[] trimChars)
        {
            while (sb.Length > 0)
            {
                if (trimChars.Contains(sb[0]))
                {
                    sb.Remove(0, 1);
                }
                else
                    break;
            }
            return this;
        }

        public CodeWriter Trim(params char[] trimChars)
        {
            TrimStart(trimChars);
            TrimEnd(trimChars);
            return this;
        }

        public char Last()
        {
            if (sb.Length > 0)
                return sb[sb.Length - 1];
            else
                return char.MinValue;
        }

        internal string Value
        {
            get
            {
                return sb.ToString();
            }
        }

        public override string ToString()
        {
            return sb.ToString();
        }

        public static implicit operator CodeWriter(string value)
        {
            return new CodeWriter(value);
        }
        public static implicit operator string(CodeWriter cw)
        {
            return cw.sb.ToString();
        }
    }
}
