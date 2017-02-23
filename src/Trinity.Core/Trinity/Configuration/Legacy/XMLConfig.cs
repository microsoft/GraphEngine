// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Net;
using System.Globalization;

namespace Trinity.Configuration
{
    internal class XMLConfig
    {
        private const string section_label = "section";
        private const string entry_label = "entry";
        private string config_file;
        private XElement root_xelement;
        
        public XMLConfig(string xml_file_name)
        {
            config_file = xml_file_name;
            if (File.Exists(xml_file_name))
            {
                root_xelement = XElement.Load(config_file);
            }
            else
            {
                root_xelement = new XElement("Trinity");
            }
        }

        public void Save()
        {
            root_xelement.Save(config_file);
        }

        public double GetEntryValue(string section, string entry, double defaultValue)
        {
            string entry_value = GetEntryValue(section, entry);
            if (entry_value == null)
                return defaultValue;

            try
            {
                return Double.Parse(entry_value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        public bool GetEntryValue(string section, string entry, bool defaultValue)
        {
            string entry_value = GetEntryValue(section, entry);
            if (entry_value == null)
                return defaultValue;

            try
            {
                return Boolean.Parse(entry_value);
            }
            catch
            {
                return false;
            }
        }

        public string GetEntryValue(string section, string entry, string defaultValue)
        {
            string entry_value = GetEntryValue(section, entry);
            return (entry_value == null ? defaultValue : entry_value);
        }

        public IPAddress GetEntryValue(string section, string entry, IPAddress defaultValue)
        {
            string entry_value = GetEntryValue(section, entry);
            return (entry_value == null ? defaultValue : Utilities.NetworkUtility.Hostname2IPv4Address(entry_value));
        }

        public int GetEntryValue(string section, string entry, int defaultValue)
        {
            string entry_value = GetEntryValue(section, entry);
            if (entry_value == null)
                return defaultValue;
            try
            {
                return Int32.Parse(entry_value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        public long GetEntryValue(string section, string entry, long defaultValue)
        {
            string entry_value = GetEntryValue(section, entry);
            if (entry_value == null)
                return defaultValue;

            try
            {
                return Int64.Parse(entry_value, CultureInfo.InvariantCulture);
            }
            catch
            {
                return 0;
            }
        }

        public string GetEntryValue(string section_name, string entry_name)
		{
            try
            {
                var matched_sections = from section in root_xelement.Elements(section_label)
                                       where ((string)section.Attribute("name")).Equals(section_name)
                                       select section;

                return (from entry in matched_sections.Elements(entry_label)
                        where ((string)entry.Attribute("name")).Equals(entry_name)
                        select entry.Value).ToArray<string>()[0];
            }
            catch (Exception)
            {
                return null;
            }
		}

        public string GetEntryProperty(string section_name, string entry_name, string entry_value, string property_name)
        {
            try
            {
                var matched_sections = from section in root_xelement.Elements(section_label)
                                       where ((string)section.Attribute("name")).Equals(section_name)
                                       select section;

                  var property_values = from entry in matched_sections.Elements(entry_label)
                        where ((string)entry.Attribute("name")).Equals(entry_name) && entry.Value.Equals(entry_value)
                          select ((string)entry.Attribute(property_name));

                  return property_values.ToArray<string>()[0];
            }
            catch (Exception)
            {
                return null;
            }
        }

        public Dictionary<string,string> GetEntryProperties(string section_name, string entry_name, string entry_value)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            try
            {
                var matched_sections = from section in root_xelement.Elements(section_label)
                                       where ((string)section.Attribute("name")).Equals(section_name)
                                       select section;

                var attributes = (from entry in matched_sections.Elements(entry_label)
                                      where ((string)entry.Attribute("name")).Equals(entry_name) && entry.Value.Equals(entry_value)
                                      select entry.Attributes()).ToArray()[0];
                foreach (var attribute in attributes)
                {
                    dic.Add(attribute.Name.ToString(), attribute.Value);
                }
                
            }
            catch (Exception)
            {
                return new Dictionary<string,string>();
            }
            return dic;
        }

        public List<string> GetEntryValues(string section_name, string entry_name)
        {
            try
            {
                var current_section = from section in root_xelement.Elements("section")
                                      where ((string)section.Attribute("name")).Equals(section_name)
                                      select section;
                return (from entry in current_section.Elements("entry")
                        where ((string)entry.Attribute("name")).Equals(entry_name)
                        select entry.Value).ToList<string>();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }

        public List<XElement> GetEntries(string section_name, string entry_name)
        {
            try
            {
                var sections = from section in root_xelement.Elements("section")
                               where ((string)section.Attribute("name")).Equals(section_name)
                               select section;
                return (from entry in sections.Elements("entry")
                        where ((string)entry.Attribute("name")).Equals(entry_name)
                        select entry).ToList();
            }
            catch
            {
                return new List<XElement>();
            }
        }

        public void SetEntryValue(string section_name, string entry_name, object entry_value)
        {
            var matched_sections = from section in root_xelement.Elements(section_label)
                                  where ((string)section.Attribute("name")).Equals(section_name)
                                  select section;

            XElement selected_section;
            
            if (matched_sections.Count<object>() == 0)
            {
                //selected section doesn't exist
                selected_section = new XElement(section_label, new XAttribute("name", section_name));
                root_xelement.Add(selected_section);
            }
            else
            {
                selected_section = matched_sections.First<XElement>();
            }

            var matched_entries = from entry in selected_section.Elements(entry_label)
                                  where ((string)entry.Attribute("name")).Equals(entry_name)
                                  select entry;

            XElement selected_entry;

            if (matched_entries.Count<object>() == 0)
            {
                //selected entry doesn't exist
                selected_entry = new XElement(entry_label, new XAttribute("name", entry_name));
                selected_section.Add(selected_entry);
            }
            else
            {
                selected_entry = matched_entries.First<XElement>();
            }

            selected_entry.SetValue(entry_value);
        }

        public void SetEntryProperty(string section_name, string entry_name, object entry_value, string property_name, object property_value)
        {
            var matched_sections = from section in root_xelement.Elements(section_label)
                                   where ((string)section.Attribute("name")).Equals(section_name)
                                   select section;

            XElement selected_section;

            if (matched_sections.Count<object>() == 0)
            {
                //selected section doesn't exist
                selected_section = new XElement(section_label, new XAttribute("name", section_name));
                root_xelement.Add(selected_section);
            }
            else
            {
                selected_section = matched_sections.First<XElement>();
            }

            var matched_entries = from entry in selected_section.Elements(entry_label)
                                  where ((string)entry.Attribute("name")).Equals(entry_name) && entry.Value.Equals(entry_value)
                                  select entry;

            XElement selected_entry;

            if (matched_entries.Count<object>() == 0)
            {
                //selected entry doesn't exist
                selected_entry = new XElement(entry_label, new XAttribute("name", entry_name));
                selected_section.Add(selected_entry);
                selected_entry.SetValue(entry_value);
            }
            else
            {
                selected_entry = matched_entries.First<XElement>();
            }

            selected_entry.SetAttributeValue(property_name, property_value);
        }

        public void SetEntryValues(string section_name, string entry_name, List<object> entry_value_list)
        {
            var matched_sections = from section in root_xelement.Elements(section_label)
                                   where ((string)section.Attribute("name")).Equals(section_name)
                                   select section;

            XElement selected_section;

            if (matched_sections.Count<object>() == 0)
            {
                //selected section doesn't exist
                selected_section = new XElement(section_label, new XAttribute("name", section_name));
                root_xelement.Add(selected_section);
            }
            else
            {
                selected_section = matched_sections.First<XElement>();
            }

            var matched_entries = from entry in selected_section.Elements(entry_label)
                                  where ((string)entry.Attribute("name")).Equals(entry_name)
                                  select entry;

            foreach (XElement entry in matched_entries)
            {
                entry.Remove();
            }

            for(int i=0;i<entry_value_list.Count;i++)
            {
                XElement new_entry = new XElement(entry_label, new XAttribute("name", entry_name));
                new_entry.SetValue(entry_value_list[i]);
                selected_section.Add(new_entry);
            }
        }

        public void SetEntryProperties(string section_name, string entry_name,  object entry_value, Dictionary<string,object> property_values)
        {
            var matched_sections = from section in root_xelement.Elements(section_label)
                                   where ((string)section.Attribute("name")).Equals(section_name)
                                   select section;

            XElement selected_section;

            if (matched_sections.Count<object>() == 0)
            {
                //selected section doesn't exist
                selected_section = new XElement(section_label, new XAttribute("name", section_name));
                root_xelement.Add(selected_section);
            }
            else
            {
                selected_section = matched_sections.First<XElement>();
            }

            var matched_entries = from entry in selected_section.Elements(entry_label)
                                  where ((string)entry.Attribute("name")).Equals(entry_name) && entry.Value.Equals(entry_value)
                                  select entry;

            XElement selected_entry;

            if (matched_entries.Count<object>() == 0)
            {
                //selected entry doesn't exist
                selected_entry = new XElement(entry_label, new XAttribute("name", entry_name));
                selected_section.Add(selected_entry);
                selected_entry.SetValue(entry_value);
            }
            else
            {
                selected_entry = matched_entries.First<XElement>();
            }

            foreach (KeyValuePair<string, object> pair in property_values)
            {
                selected_entry.SetAttributeValue(pair.Key, pair.Value);
            }
        }

        public void ClearEntryValues(string section_name, string entry_name)
        {
            var matched_sections = from section in root_xelement.Elements(section_label)
                                   where ((string)section.Attribute("name")).Equals(section_name)
                                   select section;

            XElement selected_section;

            if (matched_sections.Count<object>() == 0)
            {
                //selected section doesn't exist
                selected_section = new XElement(section_label, new XAttribute("name", section_name));
                root_xelement.Add(selected_section);
            }
            else
            {
                selected_section = matched_sections.First<XElement>();
            }

            var matched_entries = from entry in selected_section.Elements(entry_label)
                                  where ((string)entry.Attribute("name")).Equals(entry_name)
                                  select entry;

            foreach (XElement entry in matched_entries)
            {
                entry.Remove();
            }
        }
        
    }
}
