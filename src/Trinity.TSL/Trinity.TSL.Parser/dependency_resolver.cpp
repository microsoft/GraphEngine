#include "SyntaxNode.h"
#include "dependency_resolver.h"

DependencyNode::DependencyNode(Node* node)
{
    this->node = node;
}
void DependencyNode::add_dependency(DependencyNode *from)
{
    dependsOn.insert(from);
}
bool DependencyNode::self_loop()
{
    return (dependsOn.end() != find(dependsOn.begin(), dependsOn.end(), this));
}

DependencyNode* DependencyResolver::declareNode(Node* node)
{
    auto iter = node_mapping.find(node);
    if (iter == node_mapping.end())
    {
        auto dnode = new DependencyNode(node);
        node_mapping[node] = dnode;
        return dnode;
    }
    return iter->second;
}
void DependencyResolver::declareDependency(Node* my, Node* dependency)
{
    auto dto = declareNode(my), dfrom = declareNode(dependency);
    dto->add_dependency(dfrom);
}
void DependencyResolver::get_traverse_sequence(vector<Node*>& traverse_sequence, vector<vector<Node*>>& recursive_dependencies)
{
    traverse_sequence = vector<Node*>();
    recursive_dependencies = vector<vector<Node*>>();

    vector<DependencyNode*> nodeList;

    for (auto &iter : node_mapping)
        nodeList.push_back(iter.second);
    //find recursive dependencies, and return the topologic sequence of the others
    Tarjan(nodeList, traverse_sequence);

    for (auto *scc : SCCs)
    {
        vector<Node*> dependency_ring;

        for (auto *dnode : *scc)
        {
            dependency_ring.push_back(dnode->node);
        }
        recursive_dependencies.push_back(dependency_ring);
    }
}
//  note that the scc routine will also output the topological sequence
void DependencyResolver::strongly_connected_components(DependencyNode* node, vector<Node*> &output)
{
    //called on each node exactly once
    node->index = node->low_index = node_index++;
    tarjan_stack.push_back(node);
    for (auto *it : node->dependsOn)
    {
        if (it->index == -1)
        {
            // the dependency is not reached before
            // here we preclude the case of self-loop,
            // as node->index is touched on method entry.
            strongly_connected_components(it, output);
            node->low_index = min(node->low_index, it->low_index);
        }
        else
        {
            if (find(tarjan_stack.begin(), tarjan_stack.end(), it) != tarjan_stack.end())
            {
                //it is on the stack, thus node is in the SCC
                node->low_index = min(node->low_index, it->low_index);
            }
        }
    }

    //if didn't reach anything earlier on stack, node is new
    if (node->index == node->low_index)
    {
        // node is the only one, and no self loop, then we can safely output the node
        // to the topological sequence
        if (tarjan_stack.back() == node && !node->self_loop())
        {
            tarjan_stack.pop_back();
            output.push_back(node->node);
            return;
        }
        d_vector* new_scc = new d_vector();
        for (;;)
        {
            auto *it = tarjan_stack.back();
            new_scc->push_back(it);
            tarjan_stack.pop_back();
            if (it == node)
                break;
        }
        SCCs.push_back(new_scc);
    }
}
void DependencyResolver::Tarjan(d_vector& nodes, vector<Node*> &output)
{
    tarjan_stack.clear();
    SCCs.clear();
    for (auto* it : nodes)
        if (it->index == -1)
            strongly_connected_components(it, output);
}

DependencyResolver::~DependencyResolver()
{
    for (auto pair : node_mapping)
    {
        delete pair.second;
    }
}
