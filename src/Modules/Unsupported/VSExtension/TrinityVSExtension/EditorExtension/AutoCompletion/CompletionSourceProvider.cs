using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.Utilities;
using Trinity.VSExtension.EditorExtension.TSL;

namespace Trinity.VSExtension.EditorExtension.AutoCompletion
{
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("TrinitySpecificationLanguage")]
    [Name("TSLCompletionSourceProvider")]
    internal class CompletionSourceProvider : ICompletionSourceProvider
    {
        [Import]
        internal ITextStructureNavigatorSelectorService NavigatorService { get; set; }
        [Import]
        internal IGlyphService GlyphService { get; set; }

        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            //return new DumbCompletionSource(this, textBuffer);
            if (!TSLParser.Ready)
                return null;
            return new CompletionSource(this, textBuffer);
        }
    }
}
