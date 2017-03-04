using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL.Test.TestModules
{
    partial class TestCodeTemplate
    {

        public static void GenerateTestCode(ListType type, List<string> readySents, string workingSent, int depth, bool hasListOfStruct,bool isFollowingListIndexer)
        {
            int branch = (int)(r.NextDouble() * 20);
            if (branch > 8)
                branch = 0;
            switch (branch)
            {
                //test indexer
                case 0:
                    readySents.Add(@"
                   int length_" + depth + "= " + workingSent + @".Count;
                   int index_" + depth + "= " + @"GetRandom(length_" + depth + ");");

                    workingSent += "[index_" + depth + "]  ";
                    //if there'storage a List<Struct>, then you can't invoke list[0].structfield = xxx in C# object
                    //but you can do it with our Accessor
                    if (type.ElementFieldType is StructFieldType)
                        GenerateTestCode(type.ElementFieldType, readySents, workingSent, depth + 1, true,true);
                    else
                        GenerateTestCode(type.ElementFieldType, readySents, workingSent, depth + 1, hasListOfStruct,true);


                   readySents[readySents.Count-1] = "if(length_"+depth+@">0)" + readySents[readySents.Count-1]; 
                    break;

                //test Add()
                case 1:
                    workingSent += ".Add(" + ObjectGenerator.generate(type.ElementFieldType) + ");";
                    readySents.Add(workingSent);
                    break;

                //test RemoveAT
                case 2:
                    readySents.Add(@"
                   int length_" + depth + "= " + workingSent + @".Count;
                   int index_" + depth + "= " + @"GetRandom(length_" + depth + ");\r\n");
                    workingSent += ".RemoveAt(index_" + depth + ");";
                    workingSent = "if(length_" + depth + " > 0)" + workingSent;
                    readySents.Add(workingSent);
                    break;

                //test Insert
                case 3:
                    readySents.Add(@"
                   int length_" + depth + "= " + workingSent + @".Count;
                   int index_" + depth + "= " + @"GetRandomIncludingLast(length_" + depth + ");\r\n");
                    workingSent += ".Insert(index_" + depth + "," + ObjectGenerator.generate(type.ElementFieldType) + ");";
                    readySents.Add(workingSent);
                    break;

                //test AddRange
                case 4:
                    workingSent += ".AddRange(" + ObjectGenerator.generate(type) + ");";
                    readySents.Add(workingSent);
                    break;

                //test Clear
                case 5:
                    workingSent += ".Clear();";
                    readySents.Add(workingSent);
                    break;

                //test InsertRange
                case 6:
                    readySents.Add(@"
                   int length_" + depth + "= " + workingSent + @".Count;
                   int index_" + depth + "= " + @"GetRandomIncludingLast(length_" + depth + ");\r\n");
                    workingSent += ".InsertRange(index_" + depth + "," + ObjectGenerator.generate(type) + ");";
                    readySents.Add(workingSent);
                    break;

                //test RemoveRange
                case 7:
                    readySents.Add(@"
                   int length_" + depth + "= " + workingSent + @".Count;
                    int index_" + depth + @"=0;
                    int count_" + depth + @"=0;
                    GetRandomRange(length_" + depth + ",ref index_" + depth + ",ref count_" + depth + @");
");
                    workingSent += ".RemoveRange(index_" + depth + ",count_" + depth + ");";
                    workingSent = "if(length_" + depth + " > 0)" + workingSent;
                    readySents.Add(workingSent);
                    break;

                    //test =
                case 8:
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

                default:
                    break;
            }
        }
    }
}


