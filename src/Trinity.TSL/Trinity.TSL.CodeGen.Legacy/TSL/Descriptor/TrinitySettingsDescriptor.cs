using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Trinity.TSL
{
    class TrinitySettingsDescriptor : AbstractStruct
    {
        internal RunningMode running_mode = RunningMode.Embedded;
        internal string index_server_conn_string = "";
        internal string ExtensionSuffixChar = "";
        internal Dictionary<string, string> KVTable = new Dictionary<string, string>();

        internal TrinitySettingsDescriptor(WrappedSyntaxNode node)
        {
            Name = node.name;
            foreach (var child in node.children)
            {
                //TrinitySettings: childrens are all NKVPairs
                if (child.type != "NKVPair")
                    CompilerError.Throw("TrinitySettings: non-NKVPair child");

                string key = child.data["key"];
                string value = child.data["value"];

                switch (key)
                {
                    case "RunningMode":
                        switch (value)
                        {
                            case "Embedded":
                                running_mode = RunningMode.Embedded;
                                break;
                            case "Distributed":
                                running_mode = RunningMode.Server;
                                break;
                        }
                        break;
                    case "IndexConnString":
                        index_server_conn_string = value.Trim();
                        break;
                    case "ExtensionSuffixChar":
                        string tmp_suffix = value.Trim();
                        if (tmp_suffix.Length > 2)
                            ExtensionSuffixChar = tmp_suffix.Substring(1, 1).ToUpper();

                        if ((ExtensionSuffixChar[0] >= '0' && ExtensionSuffixChar[0] <= '9') ||
(ExtensionSuffixChar[0] >= 'A' && ExtensionSuffixChar[0] <= 'Z')
                            )
                        {
                            //Valid 
                        }
                        else
                        {
                            CompilerError.Throw(CompilerErrorType.DigitalOrLetterExpected);
                        }
                        break;
                    default:
                        KVTable.Add(key, value);
                        break;
                }
            }
        }
    }
}
