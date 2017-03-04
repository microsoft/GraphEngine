using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL.Test.TestModules
{
    partial class TestCodeTemplate
    {
        public static void GenerateTestCode(StructFieldType type, List<string> readySents, string workingSent, int depth,bool hasListOfStruct, bool isFollowingListIndexer)
        {
            int branch = (int)(r.NextDouble() * (type.descriptor.Fields.Count+1));

            if (branch == type.descriptor.Fields.Count)
            {
                if(!hasListOfStruct||isFollowingListIndexer)
                {
                    workingSent += " = " + ObjectGenerator.generate(type) +";";
                    readySents.Add(workingSent);
                    return;
                }
                else
                    branch = (int)(r.NextDouble() * (type.descriptor.Fields.Count));
            }

            workingSent += "." + type.descriptor.Fields[branch].Name;
            GenerateTestCode(type.descriptor.Fields[branch].Type, readySents, workingSent, depth + 1,hasListOfStruct,false);

        }
    }
}
