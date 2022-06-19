#pragma once

#include <Trinity.TSL.Parser.h>
#include <SyntaxNode.h>
/* See http://stackoverflow.com/questions/947213/c-cli-error-c3767-candidate-functions-not-accessible */
#pragma make_public(SerializedNode)
class SerializedNode;

namespace Trinity
{
    namespace TSL
    {
        public ref class WrappedSyntaxNode
        {
        public:
            WrappedSyntaxNode(SerializedNode*);
            WrappedSyntaxNode();

            bool is_http_protocol();

            System::String^ name;
            System::String^ type;
            System::Collections::Generic::List<WrappedSyntaxNode^>
                ^children = gcnew System::Collections::Generic::List<WrappedSyntaxNode^>();
            System::Collections::Generic::Dictionary<System::String^, System::String^>
                ^data = gcnew System::Collections::Generic::Dictionary<System::String^, System::String^>();
            System::Collections::Generic::List<int>
                ^arrayDimensionList = gcnew System::Collections::Generic::List<int>();
            System::Collections::Generic::List<System::String^>
                ^modifierList = gcnew System::Collections::Generic::List<System::String^>();
            int top_level_index;

        };
    }
}
