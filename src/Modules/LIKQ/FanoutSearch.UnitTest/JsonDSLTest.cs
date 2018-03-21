// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using Trinity.Network;
using Trinity;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Trinity.Network.Http;
using System.Collections.Generic;
using Xunit;

namespace FanoutSearch.UnitTest
{
    [Collection("All")]
    public class JsonDSLTest
    {
        private void JsonQuery(string str)
        {
            var mod = Global.CommunicationInstance.GetCommunicationModule<FanoutSearchModule>();
            mod.JsonQuery(str);
        }

        [Fact]
        public void JsonDSLTest_2hop()
        {
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {

    }
}");
        }

        [Fact]
        public void JsonDSLTest_field_substring()
        {
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : {
            ""substring"" : ""bin""
        }
    }
}");
        }

        [Fact]
        public void JsonDSLTest_field_count_eq()
        {
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : {
            ""count"" : 3
        }
    }
}");
        }

        [Fact]
        public void JsonDSLTest_field_count_gt()
        {
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : {
            ""count"" : {
                ""gt"" : 3
            }
        }
    }
}");
        }

        [Fact]
        public void JsonDSLTest_field_eq()
        {
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : ""bin shao""
    }
}");
        }

        [Fact]
        public void JsonDSLTest_field_has()
        {
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""has"" : ""Name""
    }
}");
        }

        [Fact]
        public void JsonDSLTest_field_continue()
        {
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""continue"" : {
            ""Name"" : ""bin shao""
        }
    }
}");
        }

        [Fact]
        public void JsonDSLTest_field_return()
        {
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""return"" : {
            ""Name"" : ""bin shao""
        }
    }
}");
        }

        [Fact]
        public void JsonDSLTest_field_continue_return()
        {
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""return"" : {
            ""Name"" : ""bin shao""
        },
        ""continue"" : {
            ""Name"" : ""bin shao""
        }
    }
}");
        }


        [Fact]
        public void JsonDSLTest_field_gt()
        {
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : {
            ""gt"" : 3
        }
    }
}");
        }

        [Fact]
        public void JsonDSLTest_OR()
        {
            JsonQuery(@"
{
  'path': '/affiliation/PaperIDs/paper',
  'affiliation': {
    'type': 'Affiliation',
    'select': [ 'Name' ],
    'match': { 'Name': 'microsoft' }
  },
  'paper': {
    'type': 'Paper',
    'select': [ 'OriginalTitle' ],
    'return': {
      'or': {
        'NormalizedTitle': { 'substring': 'graph' },
        'CitationCount': { 'gt': 100 }
      }
    }
  }
}
");
        }

        [Fact]
        public void JsonDSLTest_nomatchcondition()
        {
            Expect.FanoutSearchQueryException(() =>
                JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""type""   : ""Paper"",
    },
    author: {

    }
}"), "No match conditions");
        }

        [Fact]
        public void JsonDSLTest_field_count_eq_str()
        {
            Expect.FanoutSearchQueryException(() =>
                JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : {
            ""count"" : ""3""
        }
    }
}"), "Invalid count operand");
        }


        [Fact]
        public void JsonDSLTest_field_count_eq_str_2()
        {
            Expect.FanoutSearchQueryException(() =>
                JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : {
            ""count"" : ""hey""
        }
    }
}"), "Invalid count operand");
        }

        [Fact]
        public void JsonDSLTest_field_count_eq_float()
        {
            Expect.FanoutSearchQueryException(() =>
                JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : {
            ""count"" : ""3.14""
        }
    }
}"), "Invalid count operand");
        }


        [Fact]
        public void JsonDSLTest_field_invalid_op()
        {
            Expect.FanoutSearchQueryException(() =>
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : {
            ""hey"" : ""you""
        }
    }
}"), "Unrecognized operator");
        }

        [Fact]
        public void JsonDSLTest_field_invalid_property_integer()
        {
            Expect.FanoutSearchQueryException(() =>
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : 123
    }
}"), "Invalid property value");
        }

        [Fact]
        public void JsonDSLTest_field_invalid_property_array()
        {
            Expect.FanoutSearchQueryException(() =>
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : [""you""]
    }
}"), "Invalid property value");
        }



        [Fact]
        public void JsonDSLTest_field_gt_throw()
        {
            Expect.FanoutSearchQueryException(() =>
                JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : {
            ""gt"" : ""hey""
        }
    }
}"), "Invalid comparand");
        }

        [Fact]
        public void JsonDSLTest_field_gt_throw_2()
        {
            Expect.FanoutSearchQueryException(() =>
                JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : {
            ""gt"" : {
                ""hey"" : ""you""
            }
        }
    }
}"), "Invalid comparand");
        }


        [Fact]
        public void JsonDSLTest_field_count_invalid()
        {
            Expect.FanoutSearchQueryException(() =>
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""Name"" : {
            ""count"" : {
                ""awef"" : []
            }
        }
    }
}"), "Invalid count value");
        }

        [Fact]
        public void JsonDSLTest_field_return_invalid()
        {
            Expect.FanoutSearchQueryException(() =>
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""return"" : {
            ""Name"" : ""bin shao"",
            ""select"" : [""*""]
        }
    }
}"), "Invalid property");
        }

        [Fact]
        public void JsonDSLTest_field_return_invalid_value()
        {
            Expect.FanoutSearchQueryException(() =>
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""return"" : []
    }
}"), "Invalid return expression");
        }

        [Fact]
        public void JsonDSLTest_field_return_invalid_value_2()
        {
            Expect.FanoutSearchQueryException(() =>
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""return"" : 123
    }
}"), "Invalid return expression");
        }

        [Fact]
        public void JsonDSLTest_field_continue_invalid_value()
        {
            Expect.FanoutSearchQueryException(() =>
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""continue"" : 123
    }
}"), "Invalid continue expression");
        }

        [Fact]
        public void JsonDSLTest_field_select_invalid()
        {
            Expect.FanoutSearchQueryException(() =>
            JsonQuery(@"
{
    path: ""paper"",
    paper: {
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine"",
        ""select"" : 123
    }
}"), "Invalid select operand");
        }

        [Fact]
        public void JsonDSLTest_field_has_invalid_operand_array()
        {
            Expect.FanoutSearchQueryException(()=>
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""has"" : [""Name""]
    }
}"), "Invalid has operand");
        }

        [Fact]
        public void JsonDSLTest_field_has_invalid_operand_int()
        {
            Expect.FanoutSearchQueryException(()=>
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        ""has"" : 123
    }
}"), "Invalid has operand");
        }

        [Fact]
        public void JsonDSLTest_cannot_specify_not_or_together()
        {
            Expect.FanoutSearchQueryException(()=>
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        or : {
        },
        not : {
        }
    }
}"), "Cannot specify not/or conditions together");
        }

        [Fact]
        public void JsonDSLTest_no_predicates_found_in_or()
        {
            Expect.FanoutSearchQueryException(()=>
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    paper: {
        ""select"" : [""*""],
        ""type""   : ""Paper"",
        ""NormalizedTitle"" : ""graph engine""
    },
    author: {
        or : {
        }
    }
}"), "No predicates found in OR expression");
        }

        [Fact]
        public void JsonDSLTest_the_origin_descriptor_cannot_be_null()
        {
            Expect.FanoutSearchQueryException(()=>
            JsonQuery(@"
{
    path: ""paper/AuthorIDs/author"",
    author: {
    }
}"), "The starting node descriptor cannot be null");
        }

    }
}
