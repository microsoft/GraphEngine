using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL.Test.TestModules
{
    partial class TestCodeTemplate
    {

        public static void GenerateTestCode(ArrayType type, List<string> readySents, string workingSent, int depth, bool hasListOfStruct,bool isFollowingListIndexer)
        {

            int branch = (int)(r.NextDouble() * 10 );
            if (branch > 2)
                branch = 0;// increase the possibility of testing the indexer, rather than the Clear() or =
            switch (branch)
            {
                //test indexer
                case 0:
                    string indexstring = GetRandomIndexerString(type); 
                    workingSent += indexstring; 
                    GenerateTestCode(type.ElementType, readySents, workingSent, depth + 1, hasListOfStruct,true);
                    break;

                //test clear
                case 1:
                    readySents.Add(@"
                   int length_" + depth + "= " + workingSent + @".Length;
                    int index_" + depth + @"=0;
                    int count_" + depth + @"=0;
                    GetRandomRange(length_" + depth + ",ref index_" + depth + ",ref count_" + depth + @");
");
                    workingSent += ".Clear(index_" + depth + ",count_" + depth + ");";
                    readySents.Add(workingSent);
                    break;

                    //test =
                case 2:
                    if (!hasListOfStruct||isFollowingListIndexer)
                    {
                        workingSent += "= " + ObjectGenerator.generate(type) + ";";
                        readySents.Add(workingSent);
                    }
                    else
                    {
                        workingSent = "DoNothing(" + workingSent + ");";
                        readySents.Add(workingSent);
                    }
                    break;
            }
        }

        public static string GetRandomIndexerString(ArrayType type)
        {
            string ret = "";
            for (int i = 0; i < type.lengths.Length; ++i)
            {
                ret += "[" + (int)(r.NextDouble() * type.lengths[i]) + "]";
            }
            return ret;
        }
    }
}
