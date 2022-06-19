using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Projection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.TSL;

namespace Trinity.VSExtension.EditorExtension.TSL
{
    class TSLNameDictionary
    {
        public HashSet<string> structs = new HashSet<string>();
        public HashSet<string> cells = new HashSet<string>();
        public HashSet<string> protocols = new HashSet<string>();
        public HashSet<string> enums = new HashSet<string>();
    }
    class TSLParser
    {
        #region Fields
        internal event EventHandler ParsingFinished;
        private ITextBuffer _textBuffer;
        private IBufferGraph _bufferGraph;
        private object lParserLock = new object();
        private bool _parserThreadWaiting = false;
        private List<ClassificationSpan> _highlightClassificationResults = new List<ClassificationSpan>();
        private List<TSLSyntaxSpan> _syntaxClassificationResults = new List<TSLSyntaxSpan>();
        private TSLNameDictionary _nameDictionary = new TSLNameDictionary();
        #endregion
        #region Static members
        #region Symbol maps
        //Note that elements in the symbol maps do not have to be the exact type that the map name indicates.
        //After all, those are just colors :-)
        static readonly HashSet<WrappedTokenType> StringTypes = new HashSet<WrappedTokenType>
        {
            WrappedTokenType.T_STRING,
            WrappedTokenType.T_STRING_UNCLOSED,
        };
        static readonly HashSet<WrappedTokenType> Modifiers = new HashSet<WrappedTokenType>
        {
            WrappedTokenType.T_OPTIONALMODIFIER,
            WrappedTokenType.T_REQUEST,
            WrappedTokenType.T_RESPONSE,
            WrappedTokenType.T_TYPE,
        };
        internal static readonly HashSet<WrappedTokenType> TopLevelKeywords = new HashSet<WrappedTokenType>
        {
            WrappedTokenType.T_TRINITY_SETTINGS,
            WrappedTokenType.T_CELL,
            WrappedTokenType.T_SERVER,
            WrappedTokenType.T_PROXY,
            WrappedTokenType.T_MODULE,
            WrappedTokenType.T_STRUCT,
            WrappedTokenType.T_INCLUDE,
            WrappedTokenType.T_PROTOCOL,
            WrappedTokenType.T_ENUM,
        };
        static readonly HashSet<WrappedTokenType> Containers = new HashSet<WrappedTokenType>
        {
            WrappedTokenType.T_ASYNCRPC,
            WrappedTokenType.T_SYNCRPC,
            WrappedTokenType.T_HTTP,
            WrappedTokenType.T_LISTTYPE,
            WrappedTokenType.T_GUIDTYPE,
            WrappedTokenType.T_DATETIMETYPE,
        };
        static readonly HashSet<WrappedTokenType> AtomTypes = new HashSet<WrappedTokenType>
        {
            WrappedTokenType.T_VOID,
            WrappedTokenType.T_STREAM,
            WrappedTokenType.T_BYTETYPE,
            WrappedTokenType.T_SBYTETYPE,
            WrappedTokenType.T_BOOLTYPE,
            WrappedTokenType.T_CHARTYPE,
            WrappedTokenType.T_SHORTTYPE,
            WrappedTokenType.T_USHORTTYPE,
            WrappedTokenType.T_INTTYPE,
            WrappedTokenType.T_UINTTYPE,
            WrappedTokenType.T_LONGTYPE,
            WrappedTokenType.T_ULONGTYPE,
            WrappedTokenType.T_FLOATTYPE,
            WrappedTokenType.T_DOUBLETYPE,
            WrappedTokenType.T_DECIMALTYPE,
            WrappedTokenType.T_STRINGTYPE,
            WrappedTokenType.T_U8STRINGTYPE,
        };
        static readonly HashSet<WrappedTokenType> CommentTypes = new HashSet<WrappedTokenType>
        {
            WrappedTokenType.T_COMMENT_BLOCK,
            WrappedTokenType.T_COMMENT_BLOCK_UNCLOSED,
            WrappedTokenType.T_COMMENT_LINE,
        };
        #endregion
        private static IClassificationType _atomType;
        private static IClassificationType _containerType;
        private static IClassificationType _keywordType;
        private static IClassificationType _ModifierType;
        private static IClassificationType _CommentType;
        private static IClassificationTypeRegistryService _classificationRegistry = null;
        private static object _staticMemberLock = new object();
        private static IBufferGraphFactoryService _bufferGraphFactory;
        private static object gParserLock = new object();
        #endregion
        internal static void SetClassificationTypeRegistryService(IClassificationTypeRegistryService registry)
        {
            lock (_staticMemberLock)
            {
                _classificationRegistry = registry;
                if (registry != null)
                {
                    _atomType = registry.GetClassificationType("TSLClassifierType_Atom");
                    _containerType = registry.GetClassificationType("TSLClassifierType_Container");
                    _keywordType = registry.GetClassificationType("TSLClassifierType_Keyword");
                    _ModifierType = registry.GetClassificationType("TSLClassifierType_Modifier");
                    _CommentType = registry.GetClassificationType("TSLClassifierType_Comment");
                }
            }
        }
        internal static void SetBufferGraphFactory(IBufferGraphFactoryService BufferGraphFactory)
        {
            lock (_staticMemberLock)
            {
                _bufferGraphFactory = BufferGraphFactory;
            }
        }
        internal static bool Ready
        {
            get
            {
                lock (_staticMemberLock)
                {
                    return (_bufferGraphFactory != null && _classificationRegistry != null);
                }
            }
        }
        internal static TSLParser GetParser(ITextBuffer buffer)
        {
            Debug.Assert(Ready);
            IBufferGraph bufferGraph = null;
            bufferGraph = buffer.Properties.GetOrCreateSingletonProperty<IBufferGraph>(() =>
                {
                    return _bufferGraphFactory.CreateBufferGraph(buffer);
                });
            var parser = buffer.Properties.GetOrCreateSingletonProperty<TSLParser>(() =>
            {
                return new TSLParser(buffer, bufferGraph);
            });

            return parser;
        }
        #region private implementations
        private TSLParser(ITextBuffer buffer, IBufferGraph bufferGraph)
        {
            _textBuffer = buffer;
            _bufferGraph = bufferGraph;
            buffer.Changed += this.BufferChangedEventHandler;
            TryTriggerGlobalParse();
        }
        private void BufferChangedEventHandler(object sender, Microsoft.VisualStudio.Text.TextContentChangedEventArgs e)
        {
            TryTriggerGlobalParse();
        }

        private void TryTriggerGlobalParse()
        {
            lock (lParserLock)
                if (_parserThreadWaiting)
                    return;
            new Task(() =>
            {
                lock (lParserLock)
                {
                    if (_parserThreadWaiting)
                        return;
                    _parserThreadWaiting = true;
                }
                Thread.Sleep(500);/* Don't keep the editor too busy with this */
                GlobalParse();
                if (ParsingFinished != null)
                    ParsingFinished(this, null);
                lock (lParserLock)
                {
                    _parserThreadWaiting = false;
                }
            }).Start();
        }

        private struct ParseResult
        {
            public List<ClassificationSpan> highlightSpans;
            public List<TSLSyntaxSpan> syntaxSpans;
        }

        private ParseResult RetrieveParserResults(SnapshotSpan? _span)
        {
            ITextSnapshot currentSnapshot;
            bool is_globalParse;
            List<TSLTokenSpan> tokenSpanList;
            CallWrappedParser(_span, out currentSnapshot, out is_globalParse, out tokenSpanList);

            ParseResult result = new ParseResult();
            /* Note: Do not put BuildSyntaxSpans before BuildHighlightSpans,
             * Since it will remove items from tokenList..
             */
            result.highlightSpans = BuildHighlightSpans(tokenSpanList);
            if (is_globalParse)
                result.syntaxSpans = BuildSyntaxSpans(tokenSpanList, currentSnapshot);

            if (is_globalParse)
            {
                SetSyntaxSpans(result.syntaxSpans);
            }


            return result;
        }

        private void CallWrappedParser(SnapshotSpan? _span, out ITextSnapshot currentSnapshot, out bool is_globalParse, out List<TSLTokenSpan> tokenSpans)
        {
            string buffer;
            var bufferLines = new List<string>();
            var lineOffsets = new List<int>();
            currentSnapshot = null;
            is_globalParse = (_span == null);

            if (is_globalParse)
            {
                currentSnapshot = _textBuffer.CurrentSnapshot;
                buffer = currentSnapshot.GetText();
                foreach (var line in currentSnapshot.Lines)
                {
                    bufferLines.Add(line.GetText());
                    lineOffsets.Add(line.Start);
                }
            }
            else
            {
                SnapshotSpan span = _span.Value;
                buffer = span.GetText();
                currentSnapshot = span.Snapshot;
                foreach (var overlappedLine in
                    from snapshotLine in currentSnapshot.Lines.Select(line => new SnapshotSpan(line.Start, line.End))
                    where span.OverlapsWith(snapshotLine)
                    select span.Overlap(snapshotLine).Value)
                {
                    //TODO efficiency low
                    bufferLines.Add(overlappedLine.GetText());
                    lineOffsets.Add(overlappedLine.Start);
                }
            }

            WrappedTokenList tokenList = null;
            tokenSpans = new List<TSLTokenSpan>();

            lock (gParserLock)
            {
                //Trinity.TSL.Parser lib is not reentrant.
                try
                {
                    tokenList = new WrappedTokenList(
                        buffer,
                        bufferLines,
                        lineOffsets
                        );
                }
                catch (Exception)
                {
                    //TODO log
                    return;
                }
            }

            foreach (var token in tokenList.tokens)
            {
                try
                {
                    tokenSpans.Add(new TSLTokenSpan(new SnapshotSpan(currentSnapshot, new Span(token.FirstOffset, token.SecondOffset - token.FirstOffset + 1)), token.type));
                }
                catch (Exception)
                {
                    //TODO log
                }
            }
        }

        private void SetNameDictionary(TSLNameDictionary dict)
        {
            lock (lParserLock)
                _nameDictionary = dict;
        }

        private void SetSyntaxSpans(List<TSLSyntaxSpan> list)
        {
            lock (lParserLock)
            {
                _syntaxClassificationResults = list;
            }
        }
        private List<ClassificationSpan> BuildHighlightSpans(List<TSLTokenSpan> tokenSpanList)
        {
            lock (_staticMemberLock)
            {
                if (_classificationRegistry == null)
                    return null;
            }
            List<ClassificationSpan> classifications = new List<ClassificationSpan>();
            Func<TSLTokenSpan, IClassificationType, ClassificationSpan> makeSpan = (tokenSpan, classificationType) =>
            {
                return new ClassificationSpan(tokenSpan.span, classificationType);
            };

            foreach (var tokenSpan in tokenSpanList)
            {
                if (AtomTypes.Contains(tokenSpan.token))
                {
                    classifications.Add(makeSpan(tokenSpan, _atomType));
                }
                else if (Containers.Contains(tokenSpan.token))
                {
                    classifications.Add(makeSpan(tokenSpan, _containerType));
                }
                else if (Modifiers.Contains(tokenSpan.token))
                {
                    classifications.Add(makeSpan(tokenSpan, _ModifierType));
                }
                else if (TopLevelKeywords.Contains(tokenSpan.token))
                {
                    classifications.Add(makeSpan(tokenSpan, _keywordType));
                }
                else if (CommentTypes.Contains(tokenSpan.token))
                {
                    classifications.Add(makeSpan(tokenSpan, _CommentType));
                }
                else if (StringTypes.Contains(tokenSpan.token))
                {
                    classifications.Add(makeSpan(tokenSpan, _ModifierType));
                }
            }
            return classifications;
        }

        private List<TSLSyntaxSpan> BuildSyntaxSpans(List<TSLTokenSpan> tokenSpanList, ITextSnapshot snapshot)
        {
            List<TSLSyntaxSpan> results = new List<TSLSyntaxSpan>();
            int cursor = 0;
            int length = tokenSpanList.Count;

            TSLNameDictionary dict = new TSLNameDictionary();

            TSLTokenSpan cTokenSpan;

            #region helper routines
            Action<TSLTokenSpan, TSLSyntaxSpanType> makeSingleTokenSpan = (token, syntaxSpanType) =>
                {
                    results.Add(new TSLSyntaxSpan(token.span, syntaxSpanType));
                };
            Action<TSLTokenSpan, TSLTokenSpan, TSLSyntaxSpanType> makeSpan = (token, endToken, syntaxSpanType) =>
                {
                    var rangeSpan = new SnapshotSpan(token.span.Start, endToken.span.End);
                    results.Add(new TSLSyntaxSpan(rangeSpan, syntaxSpanType));
                };
            Func<WrappedTokenType, bool> findSymbol = (type) => { while (++cursor < length) if (tokenSpanList[cursor].token == type) return true; return false; };
            Func<TSLTokenSpan> nextSymbol = () => { if (cursor + 1 < length)return tokenSpanList[cursor + 1]; else return null; };
            Action<TSLSyntaxSpanType> captureBlock = (type) =>
            {
                if (!findSymbol(WrappedTokenType.T_LCURLY))
                    return;
                var start = tokenSpanList[cursor];
                if (!findSymbol(WrappedTokenType.T_RCURLY))
                    return;
                var end = tokenSpanList[cursor];
                makeSpan(start, end, type);
            };
            Func<String> captureName = () =>
                {
                    var nToken = nextSymbol();
                    if (nToken == null || nToken.token != WrappedTokenType.T_IDENTIFIER)
                    {
                        return null;
                    }

                    return nToken.span.GetText();
                };
            Func<String> captureCellName = () =>
                {
                    var nToken = nextSymbol();
                    if (nToken != null && nToken.token == WrappedTokenType.T_STRUCT)
                    {
                        ++cursor;
                        nToken = nextSymbol();
                    }
                    if (nToken == null || nToken.token != WrappedTokenType.T_IDENTIFIER)
                    {
                        return null;
                    }

                    return nToken.span.GetText();
                };
            Action<string> addStruct = (name) =>
                {
                    if (name == null)
                        return;
                    dict.structs.Add(name);
                };
            Action<string> addCell = (name) =>
                {
                    if (name == null)
                        return;
                    dict.cells.Add(name);
                };
            Action<string> addProtocol = (name) =>
                {
                    if (name == null)
                        return;
                    dict.protocols.Add(name);
                };
            Action<string> addEnum = (name) =>
                {
                    if (name == null)
                        return;
                    dict.enums.Add(name);
                };
            Action captureAttributeSet = () =>
            {
                var start = tokenSpanList[cursor];
                if (!findSymbol(WrappedTokenType.T_RSQUARE))
                    return;
                var end = tokenSpanList[cursor];
                makeSpan(start, end, TSLSyntaxSpanType.AttributeSet);
            };

            tokenSpanList.RemoveAll(tokenSpan =>
                {
                    if (TSLSyntaxDefinition.isComment(tokenSpan.token))
                    {
                        makeSingleTokenSpan(tokenSpan, TSLSyntaxSpanType.Comments);
                        --length;
                        return true;
                    }
                    return false;
                });
            #endregion

            //TODO hierarchical scanning, and formal description of rules

            /* First round, scan {} blocks */
            for (cursor = 0; cursor < length; ++cursor)
            {
                cTokenSpan = tokenSpanList[cursor];
                switch (cTokenSpan.token)
                {
                    case WrappedTokenType.T_INCLUDE:
                        if (findSymbol(WrappedTokenType.T_SEMICOLON))
                            makeSpan(cTokenSpan, tokenSpanList[cursor], TSLSyntaxSpanType.IncludeBlock);
                        break;
                    case WrappedTokenType.T_TRINITY_SETTINGS:
                        captureBlock(TSLSyntaxSpanType.TrinitySettingBlock);
                        break;
                    case WrappedTokenType.T_CELL:
                        addCell(captureCellName());
                        captureBlock(TSLSyntaxSpanType.StructBlock);
                        break;
                    case WrappedTokenType.T_PROXY:
                    case WrappedTokenType.T_SERVER:
                    case WrappedTokenType.T_MODULE:
                        captureBlock(TSLSyntaxSpanType.ProtocolGroupBlock);
                        break;
                    case WrappedTokenType.T_STRUCT:
                        addStruct(captureName());
                        captureBlock(TSLSyntaxSpanType.StructBlock);
                        break;
                    case WrappedTokenType.T_PROTOCOL:
                        addProtocol(captureName());
                        captureBlock(TSLSyntaxSpanType.ProtocolBlock);
                        break;
                    case WrappedTokenType.T_ENUM:
                        addEnum(captureName());
                        captureBlock(TSLSyntaxSpanType.EnumBlock);
                        //TODO enum span
                        break;
                }
            }

            /* Second round, scan [] blocks */
            for (cursor = 0; cursor < length; ++cursor)
            {
                cTokenSpan = tokenSpanList[cursor];
                if (cTokenSpan.token == WrappedTokenType.T_LSQUARE)
                {
                    captureAttributeSet();
                }
            }

            SetNameDictionary(dict);
            return results;
        }

        private void GlobalParse()
        {
            var result = RetrieveParserResults(null);
            lock (lParserLock)
                _highlightClassificationResults = result.highlightSpans;
        }

        private List<SnapshotSpan> GetAlignedSpans(ITextSnapshot historySnapshot, SnapshotSpan span)
        {
            var results = _bufferGraph.MapDownToSnapshot(span, SpanTrackingMode.EdgeExclusive, historySnapshot);
            return results.ToList();
        }
        #endregion

        #region Parsing result retrieval
        internal List<TSLSyntaxSpan> GetSyntaxSpans()
        {
            lock (lParserLock)
            {
                return _syntaxClassificationResults;
            }
        }

        internal TSLNameDictionary GetNameDictionary()
        {
            lock (lParserLock)
                return _nameDictionary;
        }

        internal List<ClassificationSpan> GetSyntaxHighlightSpans(SnapshotSpan span)
        {
            List<ClassificationSpan> ret = new List<ClassificationSpan>();
            try
            {
                List<ClassificationSpan> cresults;
                lock (lParserLock)
                    cresults = _highlightClassificationResults;

                if (cresults.Count > 0)
                {
                    var alignedSpans = GetAlignedSpans(cresults[0].Span.Snapshot, span);
                    foreach (var cspan in cresults)
                    {
                        foreach (var alignedSpan in alignedSpans)
                            if (cspan.Span.IntersectsWith(alignedSpan))
                                ret.Add(new ClassificationSpan(cspan.Span.Intersection(alignedSpan).Value, cspan.ClassificationType));
                    }
                }
                var localResult = RetrieveParserResults(span);
                ret.AddRange(localResult.highlightSpans);
            }
            catch (Exception) { }

            return ret;
        }

        internal HashSet<TSLSyntaxSpanType> GetSyntaxSpanTypes(SnapshotPoint caretSnapshotPoint)
        {
            HashSet<TSLSyntaxSpanType> ret = new HashSet<TSLSyntaxSpanType>();
            try
            {
                var syntaxSpans = GetSyntaxSpans();
                if (syntaxSpans.Count == 0)
                    return ret;

                var historySnapshot = syntaxSpans[0].span.Snapshot;
                var alignedPoint = _bufferGraph.MapDownToSnapshot(caretSnapshotPoint, PointTrackingMode.Positive, historySnapshot, PositionAffinity.Predecessor);

                if (!alignedPoint.HasValue)
                    return ret;

                var alignedSpan = new SnapshotSpan(historySnapshot, new Span(alignedPoint.Value.Position, 0));

                foreach (var span in syntaxSpans)
                {
                    //TODO better sort the spans so that we can break more quickly here
                    if (span.span.IntersectsWith(alignedSpan))
                        ret.Add(span.type);
                }
            }
            catch (Exception) { }

            return ret;
        }

        /// <summary>
        /// Token-only parsing.
        /// </summary>
        /// <param name="snapshotSpan">Target region to parse</param>
        /// <returns></returns>
        internal List<TSLTokenSpan> ParseTokens(SnapshotSpan snapshotSpan)
        {
            ITextSnapshot currentSnapshot;
            bool _unused;
            List<TSLTokenSpan> ret = null;
            try { CallWrappedParser(snapshotSpan, out currentSnapshot, out _unused, out ret); }
            catch (Exception) { }
            return ret;
        }

        #endregion

    }
}
