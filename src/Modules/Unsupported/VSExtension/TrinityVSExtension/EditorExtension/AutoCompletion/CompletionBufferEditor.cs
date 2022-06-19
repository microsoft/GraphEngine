using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.VSExtension.EditorExtension.AutoCompletion
{
    internal class CompletionBufferEditor
    {
        internal CompletionBufferEditor(ITextView textView)
        {
            try
            {
                useTabToIndent = textView.Options.GetOptionValue(DefaultOptions.ConvertTabsToSpacesOptionId);
                spaceIndent = textView.Options.GetOptionValue(DefaultOptions.IndentSizeOptionId);
            }
            catch (Exception) { }
            this.textView = textView;
        }

        internal bool caretAtLineStart()
        {
            return textView.Caret.Position.BufferPosition == textView.GetTextViewLineContainingBufferPosition(textView.Caret.Position.BufferPosition).Start;
        }

        internal void insertAfterCaret(string str)
        {
            textView.TextBuffer.Insert(textView.Caret.Position.BufferPosition, str);
            textView.Caret.MoveToPreviousCaretPosition();
        }

        internal void insertOrOverwrite(char p)
        {
            char? n = peekNextChar();
            if (n != null && n.Value == p)
            {
                textView.TextBuffer.Delete(new Span(textView.Caret.Position.BufferPosition.Position, 1));
            }
        }

        internal void insertPairOrOverwrite(char p)
        {
            char? n = peekNextChar();
            if (n != null && n.Value == p)
            {
                textView.TextBuffer.Delete(new Span(textView.Caret.Position.BufferPosition.Position, 1));
            }
            else
            {
                insertAfterCaret(p.ToString());
            }
        }

        private void insertBeforeCaret(string str)
        {
            textView.TextBuffer.Insert(textView.Caret.Position.BufferPosition, str);
        }

        internal void tryIndent()
        {
            if (!caretAtLineStart())
                return;
            int search_range = 256;
            var snapshot = textView.TextBuffer.CurrentSnapshot;
            int cur_pos = textView.Caret.Position.BufferPosition.Position;
            /* Try to find a '{' before caret, as far as search_range characters are searched */
            for (int search_idx = cur_pos - 1; search_idx >= 0 && search_range-- >= 0; --search_idx)
            {
                if (snapshot[search_idx] == '}')
                    return;
                if (snapshot[search_idx] == '{')
                {
                    if ((peekNextChar() ?? '.') == '}')
                    {
                        insertAfterCaret("\r\n");
                    }
                    //if (useTabToIndent)
                    insertAfterCaret("\t");
                    //insertBeforeCaret("\t");
                    //else
                    //    insertBeforeCaret(new string(' ', spaceIndent));
                    break;
                }
            }
        }
        internal void tryUnindent()
        {
            if (caretAtLineStart())
                return;
            //TODO
        }

        private char? peekNextChar()
        {
            char? ret = null;
            try { ret = textView.Caret.Position.BufferPosition.GetChar(); }
            catch (Exception) { }
            return ret;
        }

        #region fields
        bool useTabToIndent = true;
        ITextView textView;
        int spaceIndent = 4;
        #endregion

    }
}
