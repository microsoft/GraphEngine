using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Trinity;
using Trinity.Storage;

namespace tsl4
{
    public static class Utils
    {
        public static TestStructWriter GetWriter()
            => new TestStructWriter
            {
                IntList = Enumerable.Range(0, 10).ToList(),
                ByteList = Enumerable.Range(0, 10).Select(i => (byte)i).ToList(),
                LongList = Enumerable.Range(0, 10).Select(i => (long)i).ToList(),
                DoubleList = Enumerable.Range(0, 10).Select(i => (double)i).ToList(),
                StringList = Enumerable.Range(0, 10).Select(i => i.ToString()).ToList(),
                FixedLengthStructList = Enumerable.Range(0, 10).Select(i => new FixedLengthStruct(i, i)).ToList(),
                VariableLengthStructList = Enumerable.Range(0, 10).Select(i => new VariableLengthStruct(i.ToString())).ToList(),
            };

        public static void WithWriter(Action<TestStructWriter> action)
        {
            using (var writer = GetWriter())
                action(writer);
        }
    }
}
