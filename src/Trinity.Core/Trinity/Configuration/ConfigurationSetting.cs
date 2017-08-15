using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trinity.Configuration
{
    public class ConfigurationSetting
    {
        public ConfigurationSetting(System.Xml.Linq.XAttribute attribute)
            : this(attribute.Name.LocalName, attribute.Value) { }
        public ConfigurationSetting(string key, string value)
        {
            Key = key;
            Literal = value;
        }
        public string Key { get; private set; }
        public string Literal { get; private set; }
        public object GetValue(System.Type type)
        {
            if (type.IsEnum)
            {
                return Enum.Parse(type, Literal);
            }
            else if (type == typeof(Int32))
            {
                return Int32.Parse(Literal);
            }
            else if (type == typeof(Int64))
            {
                return Int64.Parse(Literal);
            }
            else if (type == typeof(string))
            {
                return Literal;
            }
            else if (type == typeof(bool))
            {
                return bool.Parse(Literal);
            }
            else if (type == typeof(float))
            {
                return float.Parse(Literal);
            }
            else if (type == typeof(double))
            {
                return double.Parse(Literal);
            }
            else
            {
                throw new ArgumentException(String.Format("Unsupported configuration value type: {0}", type.ToString()), "type");
            }
        }
    }
}
