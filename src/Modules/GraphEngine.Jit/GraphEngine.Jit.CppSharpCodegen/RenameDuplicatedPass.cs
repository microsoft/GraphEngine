using CppSharp.AST;
using CppSharp.Passes;
using System.Collections.Generic;

namespace GraphEngine.Jit.CppSharpCodegen
{
    internal class RenameDuplicatedPass : TranslationUnitPass
    {
        public override bool VisitFieldDecl(Field field)
        {
            ASTContext.ExcludeFromPass(field.QualifiedLogicalOriginalName, typeof(FieldToPropertyPass));
            return base.VisitFieldDecl(field);
        }

        public override bool VisitClassDecl(Class @class)
        {
            ASTContext.ExcludeFromPass(@class.QualifiedLogicalOriginalName, typeof(FieldToPropertyPass));
            return base.VisitClassDecl(@class);
        }
    }
}
