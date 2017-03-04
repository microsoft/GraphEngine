#pragma once
#include <vector>
#include <unordered_map>
#include <unordered_set>
#include <stack>
#include <algorithm>
#include "error.h"

using namespace std;

class Node;

class DependencyNode
{
public:
    DependencyNode(Node* node);
    void add_dependency(DependencyNode *from);
    bool self_loop();
    Node* node;
    unordered_set<DependencyNode*> dependsOn;

    //Tarjan
    int index = -1;
    int low_index = INT_MAX;
};

class DependencyResolver
{
public:
    ~DependencyResolver();

    DependencyNode* declareNode(Node* node);
    void declareDependency(Node* my, Node* dependency);
    void get_traverse_sequence(vector<Node*>& traverse_sequence, vector<vector<Node*>>& recursive_dependencies);
private:
    unordered_map<Node*, DependencyNode*> node_mapping;

    typedef stack<DependencyNode*> d_stack;
    typedef vector<DependencyNode*> d_vector;

    d_vector tarjan_stack;
    vector<d_vector*> SCCs;
    int node_index = 0;

    void strongly_connected_components(DependencyNode* node, vector<Node*> &output);
    void Tarjan(d_vector& nodes, vector<Node*> &output);
};
