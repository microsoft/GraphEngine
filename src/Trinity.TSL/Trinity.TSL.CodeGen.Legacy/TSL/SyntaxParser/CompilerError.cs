using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.CodeDom.Compiler;

namespace Trinity.TSL
{
    enum CompilerErrorType
    {
        UnexpectedEndOfFile,
        UnexpectedSymbol,
        InvalidNameString,
        RecursiveDefinition,
        UndefinedStruct,
        FixedFormatExpected,
        DynamicFormatExpected,
        FieldTypeListBeginExpected,
        FieldTypeListEndExpected,
        CommaExpected,
        SemicolonExpected,
        EmptyFormat,
        DuplicatedFieldName,
        ColonExpected,
        IntegerExpected,
        FieldNameSameAsStructName,
        TooManyOptionalFields,
        InternalError,
        MissingProtocolTypeDefinition,
        MissingProtocolResponseDefinition,
        MissingProtocolRequestDefinition,
        DuplicateProtocolRequestDefinition,
        DuplicateProtocolResponseDefinition,
        DuplicateProtocolTypeDefinition,
        UndefinedProtocol,
        TooManyCellDescriptors,
        IndexEntryHeaderMustBeCellType,
        IndexSyntaxNotValid,
        IndexMustBeCreatedOnFieldsNotCells,
        IndexDeclarationsMushBeInOneBlock,
        DoubleQuoteExpected,
        DigitalOrLetterExpected
    }
    [Serializable]
    class CompilerErrorException : Exception
    {
        public CompilerErrorException(CompilerError error)
        {
            this.error = error;
        }
        public CompilerError error;
    }
    [Serializable]
    class CompilerInternalErrorException : Exception
    {
        public CompilerResults cr;
        public string OutputDir;
    }
    class CompilerError
    {
        public string ErrorMessage;
        public CompilerErrorType ErrorType;
        public static void Throw(CompilerErrorType type)
        {
            CompilerError err = new CompilerError
            {
                ErrorType = type,
            };
            throw new CompilerErrorException(err);
        }
        public static void Throw(string Message)
        {
            CompilerError err = new CompilerError
            {
                ErrorMessage = Message
            };
            throw new CompilerErrorException(err);
        }
        public static void ThrowInternalError(CompilerResults cr, string outputDir)
        {
            CompilerInternalErrorException e = new CompilerInternalErrorException();
            e.cr = cr;
            e.OutputDir = outputDir;
            throw e;
        }

        public static void Parse(CompilerErrorException compilerErrorException)
        {
            //Very well, MSBuild and Visual studio is aware of this type of error message.
            //Automatically integrated into VS!
            //See http://blogs.msdn.com/b/msbuild/archive/2006/11/03/msbuild-visual-studio-aware-error-messages-and-message-formats.aspx
            try
            {
                Console.WriteLine("error : {0}", compilerErrorException.error.ErrorMessage);
            }
            catch (Exception)
            {
            }
        }

        internal static void ParseInternalError(CompilerInternalErrorException cie)
        {
            foreach (System.CodeDom.Compiler.CompilerError error in cie.cr.Errors)
            {
                Console.WriteLine("{0}({1},{2}): error {3}: {4}", error.FileName, error.Line, error.Column, error.ErrorNumber, error.ErrorText);
            }
        }
    }
}
