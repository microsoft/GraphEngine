using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows.Media;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;
using System.Diagnostics;
using System.Threading.Tasks;
using Trinity.VSExtension.EditorExtension.TSL;


namespace Trinity.VSExtension.EditorExtension.SyntaxHighlighting
{


    #region Classifier
    /// <summary>
    /// Classifier that classifies all text as an instance of the OrdinaryClassifierType
    /// </summary>
    class SyntaxHighlightClassifier : IClassifier
    {


        private TSLParser parser;
        private ITextBuffer textBuffer;

        private SyntaxHighlightClassifier(TSLParser parser, ITextBuffer textBuffer)
        {
            this.parser = parser;
            this.textBuffer = textBuffer;
            parser.ParsingFinished += this.ParsingFinishedHandler;
        }
        private void ParsingFinishedHandler(object sender, EventArgs e)
        {
            if (ClassificationChanged != null)
                ClassificationChanged(this, new ClassificationChangedEventArgs(new SnapshotSpan(textBuffer.CurrentSnapshot, new Span(0, textBuffer.CurrentSnapshot.Length))));
        }
        internal static SyntaxHighlightClassifier GetSyntaxHighlightClassifier(TSLParser parser, ITextBuffer textBuffer)
        {
            return textBuffer.Properties.GetOrCreateSingletonProperty<SyntaxHighlightClassifier>(delegate
            {
                return new SyntaxHighlightClassifier(parser, textBuffer);
            });
        }

        //Main classification routine
        public IList<ClassificationSpan> GetClassificationSpans(SnapshotSpan span)
        {
            return parser.GetSyntaxHighlightSpans(span);
        }

#pragma warning disable 67
        // This event gets raised if a non-text change would affect the classification in some way,
        // for example typing /* would cause the classification to change in C# without directly
        // affecting the span.
        public event EventHandler<ClassificationChangedEventArgs> ClassificationChanged;
#pragma warning restore 67

    }
    #endregion //Classifier
}
