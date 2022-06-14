using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.Composition;
using Microsoft.VisualStudio.Utilities;

namespace Trinity.VSExtension.EditorExtension
{
    #region Content type definition
    internal static class TrinitySpecificationLanguageContentTypeDefinition
    {
        [Export]
        [Name("TrinitySpecificationLanguage")]
        [BaseDefinition("code")]
        internal static ContentTypeDefinition TrinitySpecificationLanguageContentDefinition = null;
        [Export]
        [FileExtension(".tsl")]
        [ContentType("TrinitySpecificationLanguage")]
        internal static FileExtensionToContentTypeDefinition TrinitySpecificationLanguageFileExtensionDefinition = null;
    }
    #endregion
}
