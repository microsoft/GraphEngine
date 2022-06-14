// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Trinity.Modules.Spark
{
    internal class Utilities
    {
        public static bool TryDeserializeObject(string jsonstr, out JObject jobj)
        {
            try
            {
                jobj = JsonConvert.DeserializeObject(jsonstr) as JObject;
                return jobj != null;
            }
            catch
            {
                jobj = null;
                return false;
            }
        }

        public static bool TryGetValue<T>(JObject jobj, string propName, out T value)
        {
            JToken token;
            if (!jobj.TryGetValue(propName, out token))
            {
                value = default(T);
                return false;
            }
            else
            {
                try
                {
                    value = token.Value<T>();
                    return true;
                }
                catch
                {
                    value = default(T);
                    return false;
                }
            }
        }

        public static bool TryGetList<T>(JObject jobj, string propName, out List<T> list)
        {
            JToken token;
            if (!jobj.TryGetValue(propName, out token))
            {
                list = null;
                return false;
            }
            else
            {
                try
                {
                    list = (token as JArray).ToObject<List<T>>();
                    return true;
                }
                catch
                {
                    list = null;
                    return false;
                }
            }
        }
    }
}
