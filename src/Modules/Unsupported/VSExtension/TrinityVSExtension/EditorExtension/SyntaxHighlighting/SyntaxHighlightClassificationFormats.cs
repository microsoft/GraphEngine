using System.ComponentModel.Composition;
using System.Windows.Media;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Utilities;

namespace Trinity.VSExtension.EditorExtension.SyntaxHighlighting
{
    #region Format definition
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TSLClassifierType_Atom")]
    [Name("TSLClassifierFormat_Atom")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class TSLKeywordClassifierFormat_Atom : ClassificationFormatDefinition
    {
        public TSLKeywordClassifierFormat_Atom ()
        {
            this.DisplayName = "TSL Atom"; //human readable version of the name
            this.ForegroundColor = Color.FromRgb(107, 117, 233);
            this.IsBold = false;
            //this.BackgroundColor = Colors.Transparent;
            this.BackgroundOpacity = 0;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TSLClassifierType_Container")]
    [Name("TSLClassifierFormat_Container")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class TSLKeywordClassifierFormat_Container : ClassificationFormatDefinition
    {
        public TSLKeywordClassifierFormat_Container()
        {
            this.DisplayName = "TSL Container"; //human readable version of the name
            this.ForegroundColor = Color.FromRgb(79, 150, 61);
            this.IsBold = false;
            //this.BackgroundColor = Colors.Transparent;
            this.BackgroundOpacity = 0;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TSLClassifierType_Keyword")]
    [Name("TSLClassifierFormat_Keyword")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class TSLKeywordClassifierFormat_Keyword : ClassificationFormatDefinition
    {
        public TSLKeywordClassifierFormat_Keyword()
        {
            this.DisplayName = "TSL Keyword"; //human readable version of the name
            this.ForegroundColor = Color.FromRgb(159, 86, 184);
            this.IsBold = false;
            this.BackgroundOpacity = 0;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TSLClassifierType_Modifier")]
    [Name("TSLClassifierFormat_Modifier")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class TSLKeywordClassifierFormat_Modifier : ClassificationFormatDefinition
    {
        public TSLKeywordClassifierFormat_Modifier()
        {
            this.DisplayName = "TSL Modifier"; //human readable version of the name
            this.ForegroundColor = Color.FromRgb(222,137,61);
            this.IsBold = false;
            this.BackgroundOpacity = 0;
            //this.BackgroundColor = Colors.White;
            //this.TextDecorations = System.Windows.TextDecorations.Underline;
        }
    }
    [Export(typeof(EditorFormatDefinition))]
    [ClassificationType(ClassificationTypeNames = "TSLClassifierType_Comment")]
    [Name("TSLClassifierFormat_Comment")]
    [UserVisible(true)] //this should be visible to the end user
    [Order(Before = Priority.Default)] //set the priority to be after the default classifiers
    internal sealed class TSLKeywordClassifierFormat_Comment : ClassificationFormatDefinition
    {
        public TSLKeywordClassifierFormat_Comment()
        {
            this.DisplayName = "TSL Comment"; //human readable version of the name
            this.ForegroundColor = Colors.Gray;
            this.BackgroundOpacity = 0;
        }
    }
    #endregion //Format definition
}
