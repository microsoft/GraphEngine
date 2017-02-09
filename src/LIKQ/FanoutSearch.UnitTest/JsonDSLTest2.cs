// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;

namespace FanoutSearch.UnitTest
{
    [TestClass]
    public class JsonDSLTest2
    {
        [ClassInitialize]
        public static void Init(TestContext ctx)
        {
            Initializer.Initialize();
        }

        [ClassCleanup]
        public static void Uninit()
        {
            Initializer.Uninitialize();
        }

        private void JsonQuery(string str)
        {
            var mod = Global.CommunicationInstance.GetCommunicationModule<FanoutSearchModule>();
            mod.JsonQuery(str);
        }

        [TestMethod]
        public void JsonDSLTest2_1()
        {
            Expect.FanoutSearchQueryException(JsonQuery,
@"
  'path': '/paper/AuthorIDs/author',
  'paper': {
    'type': 'Paper',
    'NormalizedTitle': 'graph engine',
    'select': [
      'OriginalTitle'
    ]
  },
  'author': {
    'return': {
      'type': 'Author',
      'Name': 'bin shao'
    }
  }
}
", "The input is not a valid Json object");
        }

        [TestMethod]
        public void JsonDSLTest2_2()
        {
            JsonQuery(@"
{
  'path': '/paper/AuthorIDs/author',
  'paper': {
    'type': 'Paper',
    'NormalizedTitle': 'graph engine',
    'select': [
      'OriginalTitle'
    ]
  },
  'author': {
    'return': {
      'type': 'Author',
      'Name': 'bin shao'
    }
  }
");
        }

        [TestMethod]
        public void JsonDSLTest2_3()
        {
            JsonQuery(@"
{
  'path': '/paper/AuthorIDs/author',
  'paper': {
    'type': 'Paper',
    'NormalizedTitle': 'graph engine',
    'select':
      'OriginalTitle'
  },
  'author': {
    'return': {
      'type': 'Author',
      'Name': 'bin shao'
    }
  }
}
");
        }

        [TestMethod]
        public void JsonDSLTest2_4()
        {
            JsonQuery(@"
{
  'path': '/paper/AuthorIDs/author',
  'paper': {
    'type': 'Paper',
    'NormalizedTitle': 'graph engine'
  },
  'author': {
    'return': {
      'type': 'Author',
      'Name': 'bin shao',
    },
    'select': [
      'Wife'
    ]
  }
}
");
        }

        [TestMethod]
        public void JsonDSLTest2_5()
        {
            JsonQuery(@"{
  'path': '/paper/AffiliationIDs/affiliation',
  'paper': {
    'type': 'Paper',
    'NormalizedTitle': 'graph engine',
    'select': [
      'OriginalTitle',
      'AffiliationIDs'
    ]
  },
  'author': {
    'return': {
      'type': 'Author',
      'Name': 'bin shao'
    }
  },
  'affiliation': {
    'return': {
      'type': 'Affiliation'
    },
    'select': [
      'Name',
      'Aliases'
    ]
  }
}
");
        }

        [TestMethod]
        public void JsonDSLTest2_6()
        {
            Expect.FanoutSearchQueryException(
            JsonQuery, @"
{
  'path': '/affiliation/AuthorIDs/author',
  'affiliation': {
    'id': [1290206253]
  },
  'author': {
    'return': {
        'select': [
            'Name'
        ]
    }
  }
}
", "Invalid property value");
        }

        [TestMethod]
        public void JsonDSLTest2_7()
        {
            JsonQuery(@"{
  'path': '/affiliation/AuthorIDs/author',
  'affiliation': {
    'id': [1290206253]
  },
  'author': {
    'return': {
    },
    'select': [
        'Name'
    ]
  }
}


"
);
        }

        [TestMethod]
        public void JsonDSLTest2_8()
        {
            JsonQuery(@"{
  'path': '/affiliation/AuthorIDs/author',
  'affiliation': {
    'type': 'Affiliation',
    'match': {
      'Name': 'micro rf'
    },
    'select': [
      'Name'
    ]
  },
  'author': {
    'return': {
    },
    'select': [
        'Name'
    ]
  }
}

"
);
        }

        [TestMethod]
        public void JsonDSLTest2_9()
        {
            JsonQuery(@"{
  'path': '/author/AffiliationIDs/affiliation',
  'author': {
    'type': 'Author',
    'Name': 'bin shao',
    'select': [
      'Name'
    ]
  },
  'affiliation': {
    'return': {
    },
    'select': [
        'Name'
    ]
  }
}

"
);
        }

        [TestMethod]
        public void JsonDSLTest2_10()
        {
            JsonQuery(@"{
  'path': '/author/PaperIDs/paper',
  'author': {
    'type': 'Author',
    'match': { 'Name': 'bin shao' },
    'select': [ 'DisplayAuthorName' ]
  },
  'paper': {
    'type': 'Paper',
    'has': 'SomeNonsenseField',
    'select': [ 'OriginalTitle', 'ConferenceID' ],
    'return': {}
  }
}

"
);
        }

        [TestMethod]
        public void JsonDSLTest2_11()
        {
            JsonQuery(@"{
  'path': '/author/PaperIDs/paper',
  'author': {
    'type': 'Author',
    'match': { 'Name': 'daniel' },
    'select': [ 'DisplayAuthorName' ]
  },
  'paper': {
    'type': 'Paper',
    'select': [ 'OriginalTitle', 'ConferneceID' ],
    'return': {
      'has': 'ConferneceID'
    }
  }
}

"
);
        }

        [TestMethod]
        public void JsonDSLTest2_12()
        {
            JsonQuery(@"{
  'path': '/series/ConferenceInstanceIDs/conference',
  'series': {
    'type': 'ConferenceSeries',
    'FullName': 'graph',
    'select': [ 'FullName', 'ShortName' ]
  },
  'conference': {
    'type': 'ConferenceInstance',
    'select': [ 'FullName' ],
    'return': {}
  }
}");
        }


        [TestMethod]
        public void JsonDSLTest2_13()
        {
            JsonQuery(@"{
  'path': '/series/ConferenceInstanceIDs/conference/FieldOfStudyIDs/field',
  'series': {
    'type': 'ConferenceSeries',
    'FullName': 'graph',
    'select': [ 'FullName', 'ShortName' ]
  },
  'conference': {
    'type': 'ConferenceInstance',
    'select': [ 'FullName' ]
  },
  'field': {
    'type': 'FieldOfStudy',
    'select': [ 'Name' ],
    'return': { 'Name': { 'substring' : 'World Wide Web' } }
  }
}

"
);
        }

        [TestMethod]
        public void JsonDSLTest2_14()
        {
            JsonQuery(@"{
  'path': '/series/ConferenceInstanceIDs/conference/FieldOfStudyIDs/field',
  'series': {
    'type': 'ConferenceSeries',
    'FullName': 'graph',
    'select': [ 'FullName', 'ShortName' ]
    },
  'conference': {
    'type': 'ConferenceInstance',
    'select': [ 'FullName', 'StartDate' ],
    'return': { 'StartDate': { 'substring': '2014' } }
  }
}

");
        }
        [TestMethod]
        public void JsonDSLTest2_15()
        {
            JsonQuery(@"{
  'path': '/author/PaperIDs/paper',
  'author': {
    'type': 'Author',
    'Name': 'bin shao',
    'select': [ 'DisplayAuthorName' ]
  },
  'paper': {
    'type': 'Paper',
    'select': [ 'OriginalTitle', 'ConferneceID' ],
    'return': { 'has': 'ConferneceID' }
  }
}

");
        }

        [TestMethod]
        public void JsonDSLTest2_16()
        {
            JsonQuery(@"{
  'path': '/author/PaperIDs/paper/JournalID/journal',
  'author': {
    'type': 'Author',
    'Name': 'bin shao',
    'select': [ 'DisplayAuthorName' ]
  },
  'paper': {
    'type': 'Paper',
    'select': [ 'OriginalTitle', 'JournalID' ],
    'continue': { 'JournalID': { 'gt': 0 } }
  },
  'journal': {
    'type': 'Journal',
    'select': [ 'Name' ],
    'return': {}
  }
}

");
        }

        [TestMethod]
        public void JsonDSLTest2_17()
        {
            JsonQuery(@"{
  'path': '/author/PaperIDs/paper',
  'author': {
    'type': 'Author',
    'select': [ 'DisplayAuthorName' ],
    'match': { 'Name': 'bin shao' }
  },
  'paper': {
    'type': 'Paper',
    'select': [ 'OriginalTitle', 'CitationCount' ],
    'return': { 'CitationCount': { 'gt': 1 } }
  }
}

");
        }


        [TestMethod]
        public void JsonDSLTest2_18()
        {
            Expect.FanoutSearchQueryException(JsonQuery, @"{
  'path': '/affiliation/AuthorIDs/author',
  'affiliation': {
    'type': 'Affiliation',
    'select': [ 'Name' ],
    'match': { 'Name': 'microsoft' }
  },
  'author': {
    'type': 'Author',
    'select': [ 'DisplayAuthorName' ],
    'return': { 'Aliases': { 'contains': 'Bin Shao' } }
  }
}

", "Unrecognized operator contains");
        }

        [TestMethod]
        public void JsonDSLTest2_19()
        {
            JsonQuery(@"{
  'path': '/affiliation/PaperIDs/paper',
  'affiliation': {
    'type': 'Affiliation',
    'select': [ 'Name' ],
    'match': { 'Name': 'microsoft' }
  },
  'paper': {
    'type': 'Paper',
    'select': [ 'OriginalTitle' ],
    'return': { 'NormalizedTitle': { 'substring': 'graph' } }
  }
}

");
        }


        [TestMethod]
        public void JsonDSLTest2_20()
        {
            JsonQuery(@"{
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

        [TestMethod]
        public void JsonDSLTest2_21()
        {
            JsonQuery(@"
{
  'path': '/paper/AuthorIDs/author',
  'paper': {
    'type': 'Paper',
    'NormalizedTitle':'trinity: a distributed graph engine on a memory cloud'
  },
  'author': {
    'return': {
        'id': [2093502026]
    }
  }
}
");
        }
    }
}
