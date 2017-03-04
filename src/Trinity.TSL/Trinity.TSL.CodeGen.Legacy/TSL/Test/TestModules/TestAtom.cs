using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL.Test.TestModules
{
    partial class TestCodeTemplate
    {

        public static void GenerateTestCode(AtomType type, List<string> readySents, string workingSent, int depth,bool hasListOfStruct,bool isFollowingListIndexer)
        {
            if (!hasListOfStruct)
            {
                workingSent += " = " + ObjectGenerator.generate(type) + ";";
                readySents.Add(workingSent);
            }
            else
            {
                if (isFollowingListIndexer)
                {
                    workingSent += " = " + ObjectGenerator.generate(type) + ";";
                    readySents.Add(workingSent);
                }
                else
                {
                    workingSent = "DoNothing(" + workingSent + ");";
                    readySents.Add(workingSent);
                }
            }
        }
    }
}
