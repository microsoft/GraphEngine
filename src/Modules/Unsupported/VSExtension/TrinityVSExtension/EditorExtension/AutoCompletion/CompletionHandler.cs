using System;
using System.ComponentModel.Composition;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;
using System.Collections.Generic;

namespace Trinity.VSExtension.EditorExtension.AutoCompletion
{

    internal class TSLCompletionHandler : IOleCommandTarget
    {
        #region field
        private IOleCommandTarget nextCommandHandler;
        private ITextView textView;
        private TSLCompletionHandlerProvider provider;
        private ICompletionSession session;
        private CompletionBufferEditor bufferEditor;
        #endregion

        #region Constructor
        internal TSLCompletionHandler(IVsTextView textViewAdapter, ITextView iTextView, TSLCompletionHandlerProvider handlerProvider)
        {
            textView = iTextView;
            provider = handlerProvider;
            bufferEditor = new CompletionBufferEditor(iTextView);
            textViewAdapter.AddCommandFilter(this, out nextCommandHandler);
        }
        #endregion

        #region Public Implementation
        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            //if (pguidCmdGroup == VSConstants.VSStd2K)
            //{
            //    switch ((VSConstants.VSStd2KCmdID)prgCmds[0].cmdID)
            //    {
            //        case VSConstants.VSStd2KCmdID.AUTOCOMPLETE:
            //        case VSConstants.VSStd2KCmdID.COMPLETEWORD:
            //            prgCmds[0].cmdf = (uint)OLECMDF.OLECMDF_ENABLED | (uint)OLECMDF.OLECMDF_SUPPORTED;
            //            return VSConstants.S_OK;
            //    }
            //}
            return nextCommandHandler.QueryStatus(pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }
        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            /* This is a chain-of-responsibility pattern.
             * If we don't handle the command, we shall
             * pass it on to our next handler. Also, if
             * we expect later handlers to do something,
             * pass the execution control and wait for
             * it to return. A straightforward example
             * is that when TYPECHAR occur, we want our
             * kind neighbor handlers to insert that char
             * into the text buffer(we don't want to do
             * that by ourselves), we call them.
             */
            int retVal = VSConstants.S_OK;
            try
            {
                char? typedChar;
                foreach (var cmd in GetExecutionCommands(pguidCmdGroup, nCmdID, pvaIn, out typedChar))
                    switch (cmd)
                    {
                        case ExecutionCommand.CMD_Commit:
                            session.Commit();
                            break;
                        case ExecutionCommand.CMD_Dismiss:
                            session.Dismiss();
                            break;
                        case ExecutionCommand.CMD_Filter:
                            session.Filter();
                            break;
                        case ExecutionCommand.CMD_StartSession:
                            StartSession();
                            break;
                        case ExecutionCommand.CMD_CheckInputChar:
                            CheckInputChar(typedChar.Value);
                            break;
                        case ExecutionCommand.CMD_PassToNextHandler:
                        default:
                            retVal = nextCommandHandler.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
                            if (!ErrorHandler.Succeeded(retVal))
                                goto end;
                            break;
                    }
            }
            catch (Exception) { }
        end:
            return retVal;
        }

        #endregion

        #region Private Implementation

        private enum ExecutionCommand
        {
            CMD_PassToNextHandler,
            CMD_Commit,
            CMD_Filter,
            CMD_Dismiss,
            CMD_StartSession,
            CMD_CheckInputChar,
        }

        private List<ExecutionCommand> GetExecutionCommands(Guid cmdGroup, uint nCmdID, IntPtr pvaIn, out char? typedChar)
        {
            typedChar = null;
            List<ExecutionCommand> ret = new List<ExecutionCommand>();
            /* If there's automation, or we can't recognize cmdGroup, just pass on. */
            if (VsShellUtilities.IsInAutomationFunction(provider.ServiceProvider) ||
               (cmdGroup != VSConstants.VSStd2K))
            {
                ret.Add(ExecutionCommand.CMD_PassToNextHandler);
                return ret;
            }
            bool sessionActive = (session != null && !session.IsDismissed);
            bool sessionSelectionActive = (sessionActive && session.SelectedCompletionSet.SelectionStatus.IsSelected);
            switch ((VSConstants.VSStd2KCmdID)nCmdID)
            {
                case VSConstants.VSStd2KCmdID.AUTOCOMPLETE:
                case VSConstants.VSStd2KCmdID.COMPLETEWORD:
                    ret.Add(sessionActive ? ExecutionCommand.CMD_PassToNextHandler : ExecutionCommand.CMD_StartSession);
                    break;
                case VSConstants.VSStd2KCmdID.TYPECHAR:
                    typedChar = (char)(ushort)Marshal.GetObjectForNativeVariant(pvaIn);
                    bool commitChr = TypedCharLeadsToCommit(typedChar.Value);
                    bool triggerChr = TypedCharLeadsToTrigger(typedChar.Value);
                    if (commitChr && sessionActive)
                    {
                        /* Ready to commit the completion.
                         * We first check if we can commit something meaningful.
                         * If so, we then check if this commit will lead to another
                         * completion trigger.
                         * */
                        if (sessionSelectionActive)
                        {
                            ret.Add(ExecutionCommand.CMD_Commit);
                            ret.Add(ExecutionCommand.CMD_PassToNextHandler);
                            if (triggerChr)
                                ret.Add(ExecutionCommand.CMD_StartSession);
                        }
                        else
                        {
                            ret.Add(ExecutionCommand.CMD_Dismiss);
                            ret.Add(ExecutionCommand.CMD_PassToNextHandler);
                        }
                    }
                    else if (sessionActive)
                    {
                        /* Typed char doesn't lead to a commit. It leads to filter of completion results. */
                        ret.Add(ExecutionCommand.CMD_PassToNextHandler);
                        ret.Add(ExecutionCommand.CMD_Filter);
                    }
                    else
                    {
                        ret.Add(ExecutionCommand.CMD_PassToNextHandler);
                        /* Session not active, check if we have a trigger*/
                        if (triggerChr)
                        {
                            ret.Add(ExecutionCommand.CMD_StartSession);
                        }
                    }

                    /* At last, we issue a CheckInputChar command to see if we
                     * could do something about auto brackets completion, indention.
                     */
                    ret.Add(ExecutionCommand.CMD_CheckInputChar);
                    break;
                case VSConstants.VSStd2KCmdID.RETURN:
                case VSConstants.VSStd2KCmdID.TAB:
                    if (sessionActive)
                    {
                        if(!sessionSelectionActive)
                        {
                            session.SelectedCompletionSet.SelectBestMatch();
                        }
                        ret.Add(ExecutionCommand.CMD_Commit);
                    }
                    else
                    {
                        ret.Add(ExecutionCommand.CMD_PassToNextHandler);
                        if (nCmdID == (uint)(VSConstants.VSStd2KCmdID.RETURN))
                        {
                            typedChar = '\n';
                            ret.Add(ExecutionCommand.CMD_CheckInputChar);
                        }
                    }
                    break;
                case VSConstants.VSStd2KCmdID.BACKSPACE:
                case VSConstants.VSStd2KCmdID.DELETE:
                    ret.Add(ExecutionCommand.CMD_PassToNextHandler);
                    if (sessionActive)
                        ret.Add(ExecutionCommand.CMD_Filter);
                    break;
                case VSConstants.VSStd2KCmdID.CANCEL:
                    ret.Add(sessionActive ? ExecutionCommand.CMD_Dismiss : ExecutionCommand.CMD_PassToNextHandler);
                    break;
                default:
                    ret.Add(ExecutionCommand.CMD_PassToNextHandler);
                    break;
            }
            return ret;
        }
        private void CheckInputChar(char c)
        {
            switch (c)
            {
                case '{':
                    bufferEditor.insertAfterCaret("}");
                    break;
                case '}':
                    bufferEditor.insertOrOverwrite('}');
                    bufferEditor.tryUnindent();
                    break;
                case '[':
                    bufferEditor.insertAfterCaret("]");
                    break;
                case ']':
                    bufferEditor.insertOrOverwrite(']');
                    break;
                case '<':
                    bufferEditor.insertAfterCaret(">");
                    break;
                case '>':
                    bufferEditor.insertOrOverwrite('>');
                    break;
                case '"':
                    bufferEditor.insertPairOrOverwrite('"');
                    break;
                case '\n':
                    bufferEditor.tryIndent();
                    break;
            }
        }

        private bool TypedCharLeadsToCommit(char typedChar)
        {
            if (typedChar == '_')
                return false;
            return (char.IsWhiteSpace(typedChar) || char.IsPunctuation(typedChar) || char.IsSymbol(typedChar));
        }

        private bool TypedCharLeadsToTrigger(char typedChar)
        {
            return (!TypedCharLeadsToCommit(typedChar) || (typedChar == ':') || (typedChar == '<') || (typedChar == ' '));
        }

        private bool StartSession()
        {
            SnapshotPoint? caretPoint = textView.Caret.Position.Point.GetPoint(
                textBuffer => (!textBuffer.ContentType.IsOfType("projection")),
                PositionAffinity.Predecessor);

            if (!caretPoint.HasValue)
            {
                return false;
            }

            session = provider.CompletionBroker.CreateCompletionSession(
                textView,
                caretPoint.Value.Snapshot.CreateTrackingPoint(caretPoint.Value.Position, PointTrackingMode.Positive),
                true);

            session.Dismissed += this.OnSessionDismissed;
            session.Start();
            if (session != null)
                session.Filter();
            return true;
        }

        private void OnSessionDismissed(object sender, EventArgs e)
        {
            session.Dismissed -= this.OnSessionDismissed;
            session = null;
        }
        #endregion

    }
}
