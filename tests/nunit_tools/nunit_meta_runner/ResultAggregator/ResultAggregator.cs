using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;

namespace NUnitMetaRunner
{
    public static class ResultAggregator
    {
        public static XDocument Aggregate(IEnumerable<XDocument> resDocs)
        {
            if (resDocs == null) throw new ArgumentNullException(nameof(resDocs));
            var documentList = resDocs.ToList();
            if (!documentList.Any()) throw new ArgumentException("Argument is empty", nameof(resDocs));
            
            var pivot = new XDocument(documentList.First());
            var suites = documentList.SelectMany(a => a.Elements("test-run"))
                .GroupBy(e => e.Attribute("fullname").Value)
                .Select(g => AggregateElements(g, "TestRun"))
                .ToList();
            pivot.Elements("test-run").Remove();
            foreach (var e in suites)
                pivot.Add(e);
            return pivot;
        }

        class NodeType
        {
            public string TypeName { get; set; }
            public string[] ChildNodeTypes { get; set; }
            public bool HasChildTestCases { get; set; }
            public Action<XElement> Hook { get; set; } = null;
        }

        private static readonly Dictionary<string, NodeType> NodeTypes = new[] {
            new NodeType { TypeName = "TestRun", HasChildTestCases = false, ChildNodeTypes = new [] { "Assembly" },
                           Hook = e => e.Elements("filter").Elements().Remove()},
            new NodeType { TypeName = "Assembly", HasChildTestCases = false, ChildNodeTypes = new [] { "TestSuite" }},
            new NodeType { TypeName = "TestSuite", HasChildTestCases = false, ChildNodeTypes = new [] { "TestFixture" }},
            new NodeType { TypeName = "TestFixture", HasChildTestCases = true, ChildNodeTypes = new [] { "ParameterizedMethod", "Theory" }},
            new NodeType { TypeName = "ParameterizedMethod", HasChildTestCases = true, ChildNodeTypes = new string[] {}},
            new NodeType { TypeName = "Theory", HasChildTestCases = true, ChildNodeTypes = new string[] {}},
        }.ToDictionary(nt => nt.TypeName);

        private static XElement AggregateElements(IEnumerable<XElement> elements, string elementType)
        {
            var elementList = elements.ToList();
            var pivot = new XElement(elementList.First());
            Debug.Assert(elementList.All(a => a.Name == pivot.Name));

            var nodeType = NodeTypes[elementType];

            if (nodeType.ChildNodeTypes.Length > 0)
            {
                var newChildSuites = nodeType.ChildNodeTypes
                    .SelectMany(tn => GetChildSuites(tn, elementList))
                    .ToList();
                pivot.Elements("test-suite").Remove();
                foreach (var suite in newChildSuites)
                    pivot.Add(suite);
            }

            if (nodeType.HasChildTestCases)
            {
                var testcases = elementList.SelectMany(f => f.Elements("test-case")).ToList();
                pivot.Elements("test-case").Remove();
                foreach (var e in testcases)
                    pivot.Add(e);
            }

            AggregateAttributes(pivot);
            nodeType.Hook?.Invoke(pivot);
            return pivot;
        }

        private static void AggregateAttributes(XElement parent)
        {
            var children = parent.Elements("test-suite").Concat(parent.Elements("test-case"));
            var infos = children.Select(NodeInfoFactory.GetInfoFor).ToList();
            int total, failed, passed, skipped;
            parent.SetAttributeValue("total", total = infos.Sum(i => i.TotalTestCount));
            parent.SetAttributeValue("passed", passed = infos.Sum(i => i.PassedTestCount));
            parent.SetAttributeValue("warnings", infos.Sum(i => i.WarningCount));
            parent.SetAttributeValue("inconclusive", infos.Sum(i => i.InconclusiveTestCount));
            parent.SetAttributeValue("skipped", skipped = infos.Sum(i => i.SkippedTestCount));
            parent.SetAttributeValue("failed", failed = infos.Sum(i => i.FailedTestCount));
            parent.SetAttributeValue("asserts", infos.Sum(i => i.AssertCount));
            parent.SetAttributeValue("duration", infos.Sum(i => i.Duration));
            parent.SetAttributeValue("start-time", infos.Min(i => i.StartTime).ToString("u"));
            parent.SetAttributeValue("end-time", infos.Max(i => i.EndTime).ToString("u"));

            // TODO(leasunhy): aggregate <label> as well
            string result = "Inconclusive";
            if (failed != 0)
                result = "Failed";
            else if (skipped != 0)
                result = "Skipped";
            else if (passed == total)
                result = "Passed";

            if (result == "Failed")
                parent.Add(GetFailureElement());

            //Debug.Assert(total == int.Parse(parent.Attribute("testcasecount").Value));
        }

        private static XElement GetFailureElement()
        {
            return new XElement(XName.Get("failure"), new XElement("message", new XCData("One or more child tests had errors")));
        }

        private static IEnumerable<XElement> GetChildSuites(string suiteType, IEnumerable<XElement> sources)
        {
            return sources.SelectMany(a => a.Elements("test-suite"))
                .Where(e => e.Attribute("type").Value == suiteType)
                .GroupBy(e => e.Attribute("fullname").Value)
                .Select(e => AggregateElements(e, suiteType))
                .ToList();
        }
    }
}
