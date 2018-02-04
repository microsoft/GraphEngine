#pragma warning disable 162,168,649,660,661,1522
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
namespace Trinity.Extension.abc.Linq
{
    internal class IndexQueryTreeNode
    {
        internal enum NodeType
        {
            QUERY,
            AND,
            OR,
            NOT,
            XOR,
            XNOR,
            EMPTY,
            UNIVERSE,
        }
        internal IndexQueryTreeNode(NodeType type) { this.type = type; }
        internal IndexQueryTreeNode() { }
        internal NodeType                   type;
        internal List<IndexQueryTreeNode>   children            = null;
        internal String                     queryTarget         = null;
        internal String                     queryString         = null;
        /// <summary>
        /// See issue #155
        /// http://graph006:9009/redmine/issues/155
        /// </summary>
        internal List<String>               wildcard_query      = null;
        internal IndexQueryTreeNode Optimize()
        {
            if (children != null)
            {
                List<IndexQueryTreeNode> new_child_list = new List<IndexQueryTreeNode>();
                foreach (var child in children)
                {
                    var new_child = child.Optimize();
                    if (new_child != null)
                        new_child_list.Add(new_child);
                }
                this.children = new_child_list;
            }
            if (type == NodeType.QUERY || type == NodeType.UNIVERSE || type == NodeType.EMPTY)
                return this;
            if (children.Count == 0)
                return null;
            IndexQueryTreeNode universe = null;
            IndexQueryTreeNode empty    = null;
            IndexQueryTreeNode normal   = null;
            foreach (var child in children)
            {
                if (child.type == NodeType.EMPTY)
                    empty    = child;
                else if (child.type == NodeType.UNIVERSE)
                    universe = child;
                else
                    normal   = child;
            }
            switch (type)
            {
                case NodeType.NOT:
                    /* Drop NOT queries */
                    if (children[0].type == NodeType.NOT)
                        return children[0].children[0];
                    else
                        return null;
                case NodeType.XNOR:
                case NodeType.XOR:
                    /* Not implemented. */
                    return null;
                case NodeType.AND:
                    {
                        if (empty != null)
                            return empty;
                        if (universe != null)
                            return normal;
                        if (children.Count == 1)
                            return children[0];
                        else
                            return this;
                    }
                case NodeType.OR:
                    {
                        if (empty != null)
                            return normal;
                        if (universe != null)
                            return universe;
                        if (children.Count == 1)
                            return children[0];
                        else
                            return this;
                    }
                default:
                    throw new Exception("Internal error T5011.");
            }
        }
    }
}

#pragma warning restore 162,168,649,660,661,1522
