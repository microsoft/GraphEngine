using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Trinity.Utilities;

namespace Trinity.TSL
{
    partial class Lexer
    {
        public const string tslcomment = "/*The layout is optimized. Add [Layout:Sequential] attribute to suppress the optimization.*/\r\n\t";

        private static Stack<int> FieldStartPos = new Stack<int>();
        private static List<Tuple<int, int>> FieldPosRange = new List<Tuple<int, int>>();

        public void UpdateTSLSrc(List<int> newOrder)
        {
            string modified_src = string.Copy(src);
            List<int> fieldIndexesOnShrinkedSrc = new List<int>();
            List<string> fieldStrings = new List<string>();
            for (int i = FieldPosRange.Count - 1; i >= 0; --i)
            {
                fieldStrings.Insert(0, modified_src.Substring(FieldPosRange[i].Item1, FieldPosRange[i].Item2));
                modified_src = modified_src.Substring(0, FieldPosRange[i].Item1) +
                    modified_src.Substring(FieldPosRange[i].Item1 + FieldPosRange[i].Item2);
                fieldIndexesOnShrinkedSrc.Insert(0, FieldPosRange[i].Item1);
                for (int j = 1; j < fieldIndexesOnShrinkedSrc.Count; ++j)
                {
                    fieldIndexesOnShrinkedSrc[j] = fieldIndexesOnShrinkedSrc[j] - FieldPosRange[i].Item2;
                }
            }

            //insert in the new order
            for (int i = fieldIndexesOnShrinkedSrc.Count - 1; i >= 0; --i)
            {
                modified_src = modified_src.Insert(fieldIndexesOnShrinkedSrc[i], fieldStrings[newOrder[i]]);
            }

            //insert a comment
            modified_src = modified_src.Insert(fieldIndexesOnShrinkedSrc[0], tslcomment);
            src_offset += tslcomment.Length;

            src = modified_src;

            FieldStartPos.Clear();
            FieldPosRange.Clear();
        }

        public void ClearReorderInfo()
        {
            FieldStartPos.Clear();
            FieldPosRange.Clear();
        }

        internal void ClearFieldStartMarks()
        {
            FieldStartPos.Clear();
        }

        internal List<Tuple<int, int>> CollectFieldPosRange()
        {
            return new List<Tuple<int, int>>(FieldPosRange);
        }

        public void PopFieldStart()
        {
            if (FieldStartPos.Count > 0)
                FieldStartPos.Pop();
        }

        public void PrintModifiedTSL_reorderuse()
        {
            string modified_src = string.Copy(src);
            while (FieldStartPos.Count != 0)
            {
                int index = FieldStartPos.Pop();
                modified_src = modified_src.Insert(index, "*(" + src[index] + ")");
            }
            Console.WriteLine(modified_src);
            File.WriteAllLines("test.txt", new string[] { modified_src });
        }

        public void PushFieldStartPos()
        {
            if (FieldStartPos.Count == 1)
            {
                int temp = FieldStartPos.Pop();
                FieldPosRange.Add(Tuple.Create(temp, src_offset - temp));
                FieldStartPos.Push(src_offset);
            }
            else if (FieldStartPos.Count == 0)
            {
                FieldStartPos.Push(src_offset);
            }
        }

        internal void WriteBackReorderedTSL()
        {
            string trinity_home = TrinityEnvironment.ResolveTrinityHome();

            if (trinity_home == null)
            {
                return;
            }

            string filepath = trinity_home + "bin\\_reordered_tsl_temp_file.dat";
            File.WriteAllText(filepath, src);
        }
    }
}