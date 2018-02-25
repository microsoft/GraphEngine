#pragma once
#include <string>
#include <vector>
#include <map>

class SerializedNode
{

public:
    std::string name;
    std::string type;
    std::vector<SerializedNode*> children;
    std::map<std::string, std::string> data;
    std::vector<int> arrayDimensionList;
    std::vector<std::string> modifierList;
    static SerializedNode* parse(std::string filename);
    int top_level_index;

    ~SerializedNode()
    {
        for (auto c : children)
            delete c;
    }
};

