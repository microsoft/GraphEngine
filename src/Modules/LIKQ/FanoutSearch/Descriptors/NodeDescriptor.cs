// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL.Lib;

namespace FanoutSearch
{
    public class NodeDescriptor : IEnumerable<KeyValuePair<string, string>>
    {
        internal NodeDescriptor(List<string> keys, List<string> values, long id)
        {
            this.id     = id;
            this.keys   = keys;
            this.values = values;
        }

        internal List<string> keys;
        internal List<string> values;

        public long id
        {
            get;
            internal set;
        }

        public string this[string key]
        {
            get
            {
                return values[keys.IndexOf(key)];
            }
        }

        public bool Contains(string key)
        {
            return keys.Contains(key);
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            for (int i = 0, len = keys.Count; i < len; ++i)
            {
                yield return new KeyValuePair<string, string>(keys[i], values[i]);
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public override string ToString()
        {
            using (StringWriter sw = new StringWriter())
            {
                Serialize(sw);
                return sw.ToString();
            }
        }

        public void Serialize(TextWriter writer)
        {
            writer.Write('{');
            writer.Write("\"CellId\":");
            writer.Write(id);
            for (int i = 0, len = keys.Count; i < len; ++i)
            {
                writer.Write(',');
                writer.Write(JsonStringProcessor.escape(keys[i]));
                writer.Write(':');
                writer.Write(EncodeValue(values[i]));
            }
            writer.Write('}');
        }

        private string EncodeValue(string v)
        {
            if (v == string.Empty) return "\"\"";
            var fchar = v[0];

            if (char.IsLetter(fchar) || (char.Equals('0', fchar) && v.Length > 1)) // probably a string. don't parse
            {
                goto return_string;
            }

            try
            {
                JToken token = JToken.Parse(v);
                return v;
            }
            catch
            {
                goto return_string;
            }

return_string:
            return JsonStringProcessor.escape(v);
        }
    }


}
