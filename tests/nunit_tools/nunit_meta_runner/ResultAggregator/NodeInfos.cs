using System;
using System.Xml.Linq;

namespace NUnitLiteNetCoreTest.ResultAggregator
{
    internal interface INodeInfo
    {
        DateTime StartTime { get; }
        DateTime EndTime { get; }
        double Duration { get; }
        int WarningCount { get; }
        int AssertCount { get; }
        int TotalTestCount { get; }
        int PassedTestCount { get; }
        int InconclusiveTestCount { get; }
        int FailedTestCount { get; }
        int SkippedTestCount { get; }
    }

    internal class TestCaseInfo : INodeInfo
    {
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public double Duration { get; }
        public int WarningCount { get; }
        public int AssertCount { get; }
        public int TotalTestCount { get; }

        public int PassedTestCount { get; } = 0;
        public int InconclusiveTestCount { get; } = 0;
        public int FailedTestCount { get; } = 0;
        public int SkippedTestCount { get; } = 0;

        public TestCaseInfo(XElement element)
        {
            StartTime = DateTime.Parse(element.Attribute("start-time").Value);
            EndTime = DateTime.Parse(element.Attribute("end-time").Value);
            Duration = double.Parse(element.Attribute("duration").Value);
            //WarningCount = int.Parse(element.Attribute("warnings").Value);
            // TODO: how are warnings related to test-case?
            WarningCount = 0;
            AssertCount = int.Parse(element.Attribute("asserts").Value);
            TotalTestCount = 1;

            var result = element.Attribute("result").Value;
            switch (result)
            {
                case "Passed":
                    PassedTestCount = 1;
                    break;
                case "Inconclusive":
                    InconclusiveTestCount = 1;
                    break;
                case "Failed":
                    FailedTestCount = 1;
                    break;
                case "Skipped":
                    SkippedTestCount = 1;
                    break;
                default:
                    throw new NotSupportedException($"result = {result}");
            }
        }
    }

    internal class TestSuiteInfo : INodeInfo
    {
        public DateTime StartTime { get; }
        public DateTime EndTime { get; }
        public double Duration { get; }
        public int WarningCount { get; }
        public int AssertCount { get; }
        public int TotalTestCount { get; }
        public int PassedTestCount { get; }
        public int InconclusiveTestCount { get; }
        public int FailedTestCount { get; }
        public int SkippedTestCount { get; }

        public TestSuiteInfo(XElement element)
        {
            StartTime = DateTime.Parse(element.Attribute("start-time").Value);
            EndTime = DateTime.Parse(element.Attribute("end-time").Value);
            Duration = double.Parse(element.Attribute("duration").Value);
            WarningCount = int.Parse(element.Attribute("warnings").Value);
            AssertCount = int.Parse(element.Attribute("asserts").Value);
            TotalTestCount = int.Parse(element.Attribute("total").Value);
            PassedTestCount = int.Parse(element.Attribute("total").Value);
            InconclusiveTestCount = int.Parse(element.Attribute("inconclusive").Value);
            FailedTestCount = int.Parse(element.Attribute("failed").Value);
            SkippedTestCount = int.Parse(element.Attribute("skipped").Value);
        }
    }

    internal class NodeInfoFactory
    {
        public static INodeInfo GetInfoFor(XElement element)
        {
            if (element.Name.LocalName == "test-case")
                return new TestCaseInfo(element);
            if (element.Name.LocalName == "test-suite")
                return new TestSuiteInfo(element);
            throw new NotSupportedException($"Can't get info for an element of unknown type: {element.Name.LocalName}");
        }
    }
}
