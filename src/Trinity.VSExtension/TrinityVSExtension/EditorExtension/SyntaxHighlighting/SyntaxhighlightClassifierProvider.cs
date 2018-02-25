using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Projection;
using Microsoft.VisualStudio.Utilities;
using Trinity.VSExtension.EditorExtension.TSL;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.VSExtension.EditorExtension.SyntaxHighlighting
{
    #region Provider definition
    [Export(typeof(IClassifierProvider))]
    [ContentType("TrinitySpecificationLanguage")]
    internal class SyntaxhighlightClassifierProvider : IClassifierProvider
    {
        /// <summary>
        /// Import the classification registry to be used for getting a reference
        /// to the custom classification type later.
        /// The ClassificationRegistry cannot be refactored into other classes, since
        /// this KeywordClassifierProvider is our only exported IClassifierProvider of
        /// type TrinitySpecificationLanguage, hence MEF will only set the registry 
        /// for this particular class.
        /// </summary>
        [Import]
        internal IClassificationTypeRegistryService ClassificationRegistry = null; // Set via MEF
        [Import]
        internal IBufferGraphFactoryService BufferGraphFactory = null;//Set via MEF
        public IClassifier GetClassifier(ITextBuffer buffer)
        {
            TSLParser.SetClassificationTypeRegistryService(ClassificationRegistry);
            TSLParser.SetBufferGraphFactory(BufferGraphFactory);
            var parser     = TSLParser.GetParser(buffer);
            var classifier = SyntaxHighlightClassifier.GetSyntaxHighlightClassifier(parser, buffer);
            return classifier;
        }

    }
    #endregion //provider def
}
