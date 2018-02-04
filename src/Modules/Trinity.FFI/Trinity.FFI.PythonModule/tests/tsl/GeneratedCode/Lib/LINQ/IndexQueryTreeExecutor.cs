#pragma warning disable 162,168,649,660,661,1522
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
namespace Trinity.Extension.abc.Linq
{
    internal delegate List<long> SubstringQueryDelegate(string value);
    internal delegate List<long> SubstringWildcardQueryDelegate(List<string> value);
    internal class IndexQueryTreeExecutor
    {
        Dictionary<string, SubstringQueryDelegate>          m_substring_query;
        Dictionary<string, SubstringWildcardQueryDelegate>  m_substring_query_wildcard;
        internal IndexQueryTreeExecutor(Dictionary<string, SubstringQueryDelegate>          substring_index_query_method_table,
                                        Dictionary<string, SubstringWildcardQueryDelegate>  substring_index_wildcard_query_method_table)
        {
            m_substring_query          = substring_index_query_method_table;
            m_substring_query_wildcard = substring_index_wildcard_query_method_table;
        }
        internal HashSet<long> Execute(IndexQueryTreeNode node)
        {
            if (node == null)
                return null;
            HashSet<long> ret;
            switch (node.type)
            {
                case IndexQueryTreeNode.NodeType.AND:
                    ret = Execute(node.children[0]);
                    foreach (var child in node.children.Skip(1))
                        ret.IntersectWith(Execute(child));
                    break;
                case IndexQueryTreeNode.NodeType.OR:
                    ret = Execute(node.children[0]);
                    foreach (var child in node.children.Skip(1))
                        foreach (var id in Execute(child))
                            ret.Add(id);
                    break;
                case IndexQueryTreeNode.NodeType.QUERY:
                    if (node.queryString != null)
                        ret = new HashSet<long>(m_substring_query[node.queryTarget](node.queryString));
                    else
                        ret = new HashSet<long>(m_substring_query_wildcard[node.queryTarget](node.wildcard_query));
                    break;
                default:
                    throw new NotImplementedException("Internal error T5012.");
            }
            return ret;
        }
    }
}

#pragma warning restore 162,168,649,660,661,1522
