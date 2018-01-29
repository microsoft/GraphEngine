using System;
using System.Collections.Generic;
using System.Text;
using System.IO;


namespace TslAssembly
{
    static class SymTable 
    {
        // TODO: Dear Yatli, I need the very first information of cell types :)
        // Key-Value Pair is okay.
        static List<string> Content;
        /* Example:
         * 
         * ["{"name": "C1", 
         *     "attrs":
         *          {"foo": "int", 
         *           "baz": "optional int"},
         *           "bar": "string"
         *   }",
         *  ...
         * ]
         * 
         * Maybe the following way is more efficient
         * because it's faster to check whether a cell_type is in current symbol table(
         * the name of cell_type must be unique so it doesn't hurt.)
         * 
         * {"C1":[["foo", "int"], 
         *        ["baz", "optional int"],
         *        ["bar", "string"],
         *  ...
         *  }
         *  
         * It's not necessary for us to use hash to check attributes  
         * because the number of a cell's attributes could be just constant.
         *  
        */
    }
}
