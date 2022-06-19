using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using Trinity.VSExtension.EditorExtension.TSL;
using Trinity.TSL;

namespace Trinity.VSExtension.EditorExtension.AutoCompletion
{
    internal class CompletionSource : ICompletionSource
    {
        #region Fields
        private CompletionSourceProvider sourceProvider;
        private IGlyphService GlyphService;
        private ITextBuffer textBuffer;
        private List<Completion> completionList;
        private bool isDisposed = false;
        private TSLParser parser;
        #endregion

        #region Constructor
        public CompletionSource(CompletionSourceProvider completionSourceProvider, ITextBuffer iTextBuffer)
        {
            sourceProvider = completionSourceProvider;
            textBuffer = iTextBuffer;
            parser = TSLParser.GetParser(textBuffer);
            GlyphService = completionSourceProvider.GlyphService;
        }
        #endregion
        #region Implementation
        void ICompletionSource.AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            completionList = new List<Completion>();
            TSLCompletionState state = GetCompletionState(session);
            var nameDict = parser.GetNameDictionary();
            char punctationTriggerCompletion = char.MinValue;

            switch (state)
            {
                case TSLCompletionState.InAttributeSet:
                case TSLCompletionState.InEnum:
                case TSLCompletionState.InTrinitySetting:
                case TSLCompletionState.TSL_SpecifyIdentifier:
                    break;
                case TSLCompletionState.TSL_Default:
                    {
                        foreach (string str in TSLSyntaxDefinition.TopLevelKeywords)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForTopLevelKeywords(str), null));
                        break;
                    }
                #region protocol
                case TSLCompletionState.InProtocol_Default:
                    {
                        foreach (string str in TSLSyntaxDefinition.ProtocolDefaultKeywords)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForProtocolKeyword(str), null));
                        break;
                    }
                case TSLCompletionState.InProtocol_SpecifyProtocolData:
                    {
                        foreach (string str in TSLSyntaxDefinition.ProtocolDataKeywordsInProtocol)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForProtocolKeyword(str), null));
                        foreach (string str in nameDict.structs)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForStruct(), null));
                        foreach (string str in nameDict.cells)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForCell(), null));
                        punctationTriggerCompletion = ':';
                        break;
                    }
                case TSLCompletionState.InProtocol_SpecifyProtocolType:
                    {
                        foreach (string str in TSLSyntaxDefinition.ProtocolTypeKeywordsInProtocol)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForProtocolKeyword(str), null));
                        punctationTriggerCompletion = ':';
                        break;
                    }
                #endregion
                #region server/proxy/module
                case TSLCompletionState.InProtocolGroup_Default:
                    {
                        foreach (string str in TSLSyntaxDefinition.ProtocolGroupDefaultKeywords)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForProtocol(), null));
                        break;
                    }
                case TSLCompletionState.InProtocolGroup_SpecifyProtocol:
                    {
                        foreach (string str in nameDict.protocols)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForProtocol(), null));
                        punctationTriggerCompletion = ' ';
                        break;
                    }
                #endregion
                #region cell/struct
                case TSLCompletionState.InStruct_SpecifyFieldType:
                    {
                        foreach (string str in TSLSyntaxDefinition.DataTypeKeywords)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForDataTypeKeywords(str), null));
                        foreach (string str in nameDict.structs)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForStruct(), null));
                        foreach (string str in nameDict.enums)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForEnum(), null));
                        punctationTriggerCompletion = '<';
                        break;
                    }
                case TSLCompletionState.InStruct_SpecifyFieldTypeOrModifiers:
                    {
                        foreach (string str in TSLSyntaxDefinition.DataTypeKeywords)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForDataTypeKeywords(str), null));
                        foreach (string str in TSLSyntaxDefinition.ModifiersInStruct)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForModifiersInStruct(str), null));
                        foreach (string str in nameDict.structs)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForStruct(), null));
                        foreach (string str in nameDict.enums)
                            completionList.Add(new Completion(str, str, str, GetCompletionIconForEnum(), null));
                        punctationTriggerCompletion = '<';
                        break;
                    }
                case TSLCompletionState.InStruct_SpecifyFieldName:
                    break;
                #endregion
                default:
                    break;
            }

            completionSets.Add(new CompletionSet(
                "TSL Completion",    //the non-localized title of the tab
                "TSL Completion",    //the display title of the tab
                FindTokenSpanAtPosition(session.GetTriggerPoint(textBuffer), session, punctationTriggerCompletion),
                completionList,
                null));
        }

        private System.Windows.Media.ImageSource GetCompletionIconForTopLevelKeywords(string str)
        {
            return GlyphService.GetGlyph(TSLSyntaxDefinition.TopLevelGlyph(str), StandardGlyphItem.GlyphItemPublic);
        }

        private System.Windows.Media.ImageSource GetCompletionIconForProtocolKeyword(string str)
        {
            return GlyphService.GetGlyph(TSLSyntaxDefinition.ProtocolKeywordGlyph(str), StandardGlyphItem.GlyphItemPublic);
        }

        private System.Windows.Media.ImageSource GetCompletionIconForProtocol()
        {
            return GlyphService.GetGlyph(TSLSyntaxDefinition.ProtocolGroupGlyph(), StandardGlyphItem.GlyphItemPublic);
        }

        private System.Windows.Media.ImageSource GetCompletionIconForDataTypeKeywords(string str)
        {
            return GlyphService.GetGlyph(TSLSyntaxDefinition.DataTypeGlyph(str), StandardGlyphItem.GlyphItemPublic);
        }

        private System.Windows.Media.ImageSource GetCompletionIconForEnum()
        {
            return GlyphService.GetGlyph(TSLSyntaxDefinition.TopLevelGlyph("enum"), StandardGlyphItem.GlyphItemPublic);
        }

        private System.Windows.Media.ImageSource GetCompletionIconForStruct()
        {
            return GlyphService.GetGlyph(TSLSyntaxDefinition.TopLevelGlyph("struct"), StandardGlyphItem.GlyphItemPublic);
        }

        private System.Windows.Media.ImageSource GetCompletionIconForCell()
        {
            return GlyphService.GetGlyph(TSLSyntaxDefinition.TopLevelGlyph("cell"), StandardGlyphItem.GlyphItemPublic);
        }

        private System.Windows.Media.ImageSource GetCompletionIconForModifiersInStruct(string str)
        {
            return GlyphService.GetGlyph(TSLSyntaxDefinition.ModifierInStructGlyph(str), StandardGlyphItem.GlyphItemPublic);
        }

        private ITrackingSpan FindTokenSpanAtPosition(ITrackingPoint point, ICompletionSession session, char punctationTriggerCompletion)
        {
            SnapshotPoint currentPoint = (session.TextView.Caret.Position.BufferPosition) - 1;
            ITextStructureNavigator navigator = sourceProvider.NavigatorService.GetTextStructureNavigator(textBuffer);
            if (punctationTriggerCompletion == currentPoint.GetChar())
            {
                var trackingSpan = currentPoint.Snapshot.CreateTrackingSpan(new Span(currentPoint + 1, 0), SpanTrackingMode.EdgeInclusive);
                return trackingSpan;
            }
            else
            {
                TextExtent extent = navigator.GetExtentOfWord(currentPoint);
                var trackingSpan = currentPoint.Snapshot.CreateTrackingSpan(extent.Span, SpanTrackingMode.EdgeInclusive);
                return trackingSpan;
            }
        }

        private TSLCompletionState GetCompletionState(ICompletionSession session)
        {

            /* We decide to parse the current line in this routine,
             * since it will provide real-time token information
             * without which we would have to depend on the lagging
             * and unnecessary global parse results.
             */
            var caretSnapshotPoint = session.TextView.Caret.Position.BufferPosition;
            var tokenSpans = parser.ParseTokens(textBuffer.CurrentSnapshot.GetLineFromPosition(caretSnapshotPoint.Position).Extent);
            var syntaxSpanTypes = parser.GetSyntaxSpanTypes(caretSnapshotPoint);

            bool afterTypes = false;
            bool beforeRAngle = false;
            bool afterLAngle = false;
            bool afterProtocol = false;
            bool afterColon = false;
            bool afterRAngle = false;
            bool afterProtocolTypeSpecifier = false;
            bool afterProtocolDataSpecifier = false;

            bool afterTopLevelFinalBlockTypeToken = false;

            /* Compare the spans in the line with current caret */
            foreach (var tokenSpan in tokenSpans)
            {
                if (tokenSpan.span.End <= caretSnapshotPoint)
                /* caret after token */
                {
                    if (tokenSpan.token == WrappedTokenType.T_COLON)
                        afterColon = true;
                    if (TSLSyntaxDefinition.isTypeToken(tokenSpan.token))
                        afterTypes = true;
                    if (tokenSpan.token == WrappedTokenType.T_TYPE)
                        afterProtocolTypeSpecifier = true;
                    if (tokenSpan.token == WrappedTokenType.T_REQUEST || tokenSpan.token == WrappedTokenType.T_RESPONSE)
                        afterProtocolDataSpecifier = true;
                    if (tokenSpan.token == WrappedTokenType.T_LANGLE)
                        afterLAngle = true;
                    if (tokenSpan.token == WrappedTokenType.T_PROTOCOL)
                        afterProtocol = true;
                    if (tokenSpan.token == WrappedTokenType.T_RANGLE)
                        afterRAngle = true;
                    if (TSLSyntaxDefinition.isTopLevelFinalBlockTypeToken(tokenSpan.token))
                        afterTopLevelFinalBlockTypeToken = true;
                }
                else if (tokenSpan.span.Start >= caretSnapshotPoint)
                {
                    /* caret before token */
                    switch (tokenSpan.token)
                    {
                        case WrappedTokenType.T_RANGLE:
                            beforeRAngle = true;
                            break;
                    }
                }
            }

            /* Note that GetSyntaxSpanTypes might return multiple results,
             * so we define matching priorities here.
             * For example, Comments take the highest priority to capture State so that any completion will be
             * disabled.
             */

            if (syntaxSpanTypes.Contains(TSLSyntaxSpanType.AttributeSet))
                return TSLCompletionState.InAttributeSet;
            else if (syntaxSpanTypes.Contains(TSLSyntaxSpanType.Comments))
                return TSLCompletionState.InComment;

            if (syntaxSpanTypes.Count != 0)
                switch (syntaxSpanTypes.First())
                {
                    case TSLSyntaxSpanType.EnumBlock:
                        return TSLCompletionState.InEnum;
                    case TSLSyntaxSpanType.IncludeBlock:
                        return TSLCompletionState.InInclude;
                    case TSLSyntaxSpanType.ProtocolBlock:
                        if (afterProtocolTypeSpecifier && afterColon)
                            return TSLCompletionState.InProtocol_SpecifyProtocolType;
                        else if (afterProtocolDataSpecifier && afterColon)
                            return TSLCompletionState.InProtocol_SpecifyProtocolData;
                        else
                            return TSLCompletionState.InProtocol_Default;
                    case TSLSyntaxSpanType.ProtocolGroupBlock:
                        if (afterProtocol)
                            return TSLCompletionState.InProtocolGroup_SpecifyProtocol;
                        else
                            return TSLCompletionState.InProtocolGroup_Default;
                    case TSLSyntaxSpanType.StructBlock:
                        if (beforeRAngle && afterLAngle)
                            return TSLCompletionState.InStruct_SpecifyFieldType;
                        else /* The outmost Type token? */ if (!afterTypes)
                            return TSLCompletionState.InStruct_SpecifyFieldTypeOrModifiers;
                        else /* No RAngle, we're in the middle */if (afterLAngle && !afterRAngle)
                            return TSLCompletionState.InStruct_SpecifyFieldType;
                        else
                            return TSLCompletionState.InStruct_SpecifyFieldName;
                    case TSLSyntaxSpanType.TrinitySettingBlock:
                        return TSLCompletionState.InTrinitySetting;
                }

            /* Nothing matched. we say that it's in the top-level. */
            if (afterTopLevelFinalBlockTypeToken)
                return TSLCompletionState.TSL_SpecifyIdentifier;
            return TSLCompletionState.TSL_Default;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                GC.SuppressFinalize(this);
                isDisposed = true;
            }
        }
        #endregion
    }
}
