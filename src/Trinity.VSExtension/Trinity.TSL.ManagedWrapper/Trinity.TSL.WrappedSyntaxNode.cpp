#include <msclr/marshal_cppstd.h>
#include <string>
#include "Trinity.TSL.WrappedSyntaxNode.h"
#pragma make_public(SerializedNode)

System::String^ convert_string(const std::string& str)
{
    return msclr::interop::marshal_as<System::String^>(str);
}

namespace Trinity
{
    namespace TSL
    {
        ref class WrappedSyntaxNode;
        WrappedSyntaxNode::WrappedSyntaxNode(){}
        WrappedSyntaxNode::WrappedSyntaxNode(SerializedNode* node)
        {
            this->name = convert_string(node->name);
            this->type = convert_string(node->type);
            for (auto c : node->children)
                this->children->Add(gcnew WrappedSyntaxNode(c));
            for (auto &kvp : node->data)
                this->data[convert_string(kvp.first)] = convert_string(kvp.second);
            for (auto d : node->arrayDimensionList)
                this->arrayDimensionList->Add(d);
            for (auto &mod : node->modifierList)
                this->modifierList->Add(convert_string(mod));
            this->top_level_index = node->top_level_index;
        }
        bool WrappedSyntaxNode::is_http_protocol()
        {
            if (this->type != "NProtocol")
                return false;
            return (this->data["protocolType"] == "http");
        }
    }
}

