using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;

namespace Trinity.VSExtension.EditorExtension.TSL
{
    internal enum TSLCompletionState
    {
        TSL_Default,
        InAttributeSet,
        InStruct_SpecifyFieldType,
        InProtocol_Default,
        InProtocolGroup_Default,
        InTrinitySetting,
        InComment,
        InInclude,
        InEnum,
        InProtocol_SpecifyProtocolType,
        InProtocol_SpecifyProtocolData,
        InProtocolGroup_SpecifyProtocol,
        InStruct_SpecifyFieldName,
        InStruct_SpecifyFieldTypeOrModifiers,
        TSL_SpecifyIdentifier,
    }
    enum TSLSyntaxSpanType
    {
        TrinitySettingBlock,
        StructBlock,
        ProtocolBlock,
        ProtocolGroupBlock, //Server, Proxy are both Protocol groups
        AttributeSet,
        //Protocol_TypeBlock,  /* currently lower level spans are handled in CompletionSource directly through some dirty stuff..
        //Protocol_FormatBlock,
        SyntaxError, /* Only when there're no (obvious) SyntaxError spans, we send our buffer to Syntax/Semantics Checker module */
        Comments,
        Unknown, //When doing a short scan, we do not know the scope
        IncludeBlock,
        EnumBlock,
    }

    //    class TSLSyntaxPattern
    //    {
    //        public TSLSyntaxPattern(TSLSyntaxSpanType type, List<WrappedTokenType> start, List<WrappedTokenType> end, TSLPointState state, string errorMessage = null)
    //        {
    //            this.type = type;
    //            this.start = start;
    //            this.end = end;
    //            this.transistedPointState = state;
    //            this.syntaxErrorMessage = errorMessage;
    //        }
    //        public TSLSyntaxSpanType type;
    //        public List<WrappedTokenType> start;
    //        public List<WrappedTokenType> end;
    //        public TSLPointState transistedPointState;
    //        public string syntaxErrorMessage;
    //    }

    class TSLSyntaxDefinition
    {
        internal static StandardGlyphGroup DataTypeGlyph(string key)
        {
            key = key.ToLowerInvariant();
            if (key == "list")
                return StandardGlyphGroup.GlyphGroupTemplate;
            return StandardGlyphGroup.GlyphGroupStruct;
        }
        internal static readonly List<string> DataTypeKeywords = new List<string>
        {
            /* "bit", */"byte","sbyte","bool","char","short","ushort","int","uint",
            "long","ulong","float","double","decimal","DateTime","Guid","string","u8string","List","CellId"
        };
        internal static StandardGlyphGroup ModifierInStructGlyph(string key)
        {
            key = key.ToLowerInvariant();
            switch (key)
            {
                case "optional":
                    return (StandardGlyphGroup)ObjectBrowserIcon.OptionalModifier;
                default:
                    return StandardGlyphGroup.GlyphGroupEnum;
            }
        }
        internal static readonly List<string> ModifiersInStruct = new List<string>
        {
            "optional",
        };
        #region protocol keywords
        internal static StandardGlyphGroup ProtocolKeywordGlyph(string key)
        {
            key = key.ToLowerInvariant();
            switch (key)
            {
                case "protocol":
                    return StandardGlyphGroup.GlyphGroupEvent;
                case "type":
                    return StandardGlyphGroup.GlyphGroupType;
                case "request":
                    return (StandardGlyphGroup)ObjectBrowserIcon.ForwardTelephone;
                case "response":
                    return (StandardGlyphGroup)ObjectBrowserIcon.BackwardTelephone;
                case "syn":
                    return (StandardGlyphGroup)ObjectBrowserIcon.Syn;
                case "asyn":
                    return (StandardGlyphGroup)ObjectBrowserIcon.Asyn;
                case "http":
                    return StandardGlyphGroup.GlyphXmlAttribute;
                //TODO what about void?
                default:
                    return StandardGlyphGroup.GlyphGroupType;
            }
        }
        internal static readonly List<string> ProtocolDefaultKeywords = new List<string>
        {
            "Type", "Request", "Response", 
        };
        internal static readonly List<string> ProtocolTypeKeywordsInProtocol = new List<string>
        {
            "Syn", "Asyn", "HTTP", 
        };
        internal static readonly List<string> ProtocolDataKeywordsInProtocol = new List<string>
        {
            "stream", "void",
        };
        #endregion
        #region protocol group keywords
        internal static StandardGlyphGroup ProtocolGroupGlyph()
        {
            return StandardGlyphGroup.GlyphGroupInterface;
        }
        internal static readonly List<string> ProtocolGroupDefaultKeywords = new List<string>
        {
            "protocol", 
        };
        #endregion
        internal static StandardGlyphGroup TopLevelGlyph(string key)
        {
            key = key.ToLowerInvariant();
            switch (key)
            {
                case "struct":
                    return (StandardGlyphGroup)ObjectBrowserIcon.TrinityStruct;
                case "cell":
                    return StandardGlyphGroup.GlyphGroupModule;
                case "protocol":
                    return StandardGlyphGroup.GlyphGroupInterface;
                case "server":
                case "proxy":
                case "module":
                    return StandardGlyphGroup.GlyphGroupDelegate;
                case "enum":
                    return StandardGlyphGroup.GlyphGroupEnum;
                case "trinitysettings":
                    return StandardGlyphGroup.GlyphGroupProperty;
                default:
                    return StandardGlyphGroup.GlyphGroupUnknown;
            }
        }
        internal static readonly List<string> TopLevelKeywords = new List<string>
        {
            "struct", "cell", "protocol", 
            "server", "proxy", "module", "enum", "TrinitySettings"
        };


        internal static readonly HashSet<WrappedTokenType> TypeTokens = new HashSet<WrappedTokenType>
        {
            WrappedTokenType.T_BYTETYPE,WrappedTokenType.T_SBYTETYPE,WrappedTokenType.T_BOOLTYPE,WrappedTokenType.T_CHARTYPE,WrappedTokenType.T_SHORTTYPE,WrappedTokenType.T_USHORTTYPE,WrappedTokenType.T_INTTYPE,WrappedTokenType.T_UINTTYPE,
            WrappedTokenType.T_LONGTYPE,WrappedTokenType.T_ULONGTYPE,WrappedTokenType.T_FLOATTYPE,WrappedTokenType.T_DOUBLETYPE,WrappedTokenType.T_DECIMALTYPE,WrappedTokenType.T_DATETIMETYPE,WrappedTokenType.T_GUIDTYPE,WrappedTokenType.T_STRINGTYPE, WrappedTokenType.T_U8STRINGTYPE, WrappedTokenType.T_LISTTYPE
        };

        internal static bool isComment(WrappedTokenType token)
        {
            return token == WrappedTokenType.T_COMMENT_BLOCK || token == WrappedTokenType.T_COMMENT_BLOCK_UNCLOSED || token == WrappedTokenType.T_COMMENT_LINE;
        }

        internal static bool isTypeToken(WrappedTokenType token)
        {
            return TypeTokens.Contains(token);
        }

        internal static bool isTopLevelFinalBlockTypeToken(WrappedTokenType token)
        {
            return TSLParser.TopLevelKeywords.Contains(token) && token != WrappedTokenType.T_CELL;
        }
    }

    class TSLSyntaxSpan
    {
        public TSLSyntaxSpan(SnapshotSpan span, TSLSyntaxSpanType type)
        {
            this.span = span;
            this.type = type;
        }
        public SnapshotSpan span;
        public TSLSyntaxSpanType type;
    }

    class TSLTokenSpan
    {
        public TSLTokenSpan(SnapshotSpan span, WrappedTokenType token)
        {
            this.span = span;
            this.token = token;
        }
        public SnapshotSpan span;
        public WrappedTokenType token;
    }
}
