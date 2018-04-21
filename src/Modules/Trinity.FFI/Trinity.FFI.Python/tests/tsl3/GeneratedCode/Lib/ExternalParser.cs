#pragma warning disable 162,168,649,660,661,1522

using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
using Trinity.TSL.Lib;
namespace Trinity.Extension
{
    internal class ExternalParser
    {
        
        internal static unsafe bool TryParse_List_int(string s, out List<int> value)
        {
            List<int> value_type_value;
            JArray jarray;
            
            try
            {
                value = new List<int>();
                jarray = JArray.Parse(s);
                foreach (var jarray_element in jarray)
                {
                    int element;
                    
                    if (!int.TryParse((string)jarray_element, out element))
                    {
                        continue;
                    }
                    value.Add(element);
                    
                }
                return true;
            }
            catch
            {
                value = default(List<int>);
                return false;
            }
            
        }
        
        internal static unsafe bool TryParse_List_string(string s, out List<string> value)
        {
            List<string> value_type_value;
            JArray jarray;
            
            try
            {
                value = new List<string>();
                jarray = JArray.Parse(s);
                foreach (var jarray_element in jarray)
                {
                    string element;
                    
                    value.Add((string)jarray_element);
                    
                }
                return true;
            }
            catch
            {
                value = default(List<string>);
                return false;
            }
            
        }
        
        #region Mute
        
        #endregion
    }
}

#pragma warning restore 162,168,649,660,661,1522
