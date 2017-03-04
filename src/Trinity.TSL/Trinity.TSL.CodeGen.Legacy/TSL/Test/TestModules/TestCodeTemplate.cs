using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL.Test.TestModules
{
    partial class TestCodeTemplate
    {
        static Random r = new Random();

        public static void GenerateTestCode(FieldType type, List<string> readySents, string workingSent, int depth,bool hasListOfStruct,bool isFollowingListIndexer)
        {
            if (type is AtomType)
                 GenerateTestCode((AtomType)type,readySents,workingSent,depth,hasListOfStruct,isFollowingListIndexer);
            else if (type is StructFieldType)
                 GenerateTestCode((StructFieldType)type,readySents,workingSent,depth,hasListOfStruct,isFollowingListIndexer);
            else if (type is ListType)
                GenerateTestCode((ListType)type,readySents,workingSent,depth,hasListOfStruct,isFollowingListIndexer);
            else if (type is StringType)
                GenerateTestCode((StringType)type,readySents,workingSent,depth,hasListOfStruct,isFollowingListIndexer);
            else if (type is ArrayType)
                GenerateTestCode((ArrayType)type,readySents,workingSent,depth,hasListOfStruct,isFollowingListIndexer);
        }
    }
}
