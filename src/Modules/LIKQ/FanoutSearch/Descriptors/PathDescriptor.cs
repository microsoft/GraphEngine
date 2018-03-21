// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using FanoutSearch.Protocols.TSL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanoutSearch
{
    [DebuggerDisplay("Path = {ToString()}")]
    public class PathDescriptor : IEnumerable<NodeDescriptor>
    {
        List<NodeDescriptor> m_nodes;

        public PathDescriptor(ResultPathDescriptor_Accessor pda, List<List<string>> selectFields)
        {
            m_nodes = new List<NodeDescriptor>();
            int idx = 0;
            List<string> empty_field_selections = new List<string>();
            foreach (var n in pda.nodes)
            {
                List<string> field_selections = n.Contains_field_selections ?
                    n.field_selections.Select(_ =>(string)_).ToList() :
                    empty_field_selections;

                m_nodes.Add(new NodeDescriptor(selectFields[idx++], field_selections, n.id));
            }
        }

        public NodeDescriptor this[int index] => m_nodes[index];

        public IEnumerator<NodeDescriptor> GetEnumerator()
        {
            return m_nodes.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            using (StringWriter sw = new StringWriter())
            {
                Serialize(sw);
                return sw.ToString();
            }
        }

        internal void Serialize(TextWriter writer)
        {
            bool first = true;
            writer.Write('[');
            foreach (var node in m_nodes)
            {
                if (first) { first = false; }
                else { writer.Write(','); }

                node.Serialize(writer);
            }
            writer.Write(']');
        }
    }
}
