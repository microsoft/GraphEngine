using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Globalization;

namespace Trinity.TSL.Test
{
    class ObjectGenerator
    {
            static Random r = new Random();

        public static string generate(AtomType type)
        {
            switch (type.Name)
            {
                case "byte":
                    byte _byte = (byte)(10 * r.NextDouble());
                    return _byte.ToString(CultureInfo.InvariantCulture);
                case "sbyte":
                    sbyte _sbyte = (sbyte)(10 * r.NextDouble());
                    return _sbyte.ToString(CultureInfo.InvariantCulture);
                case "bool":
                    if (r.NextDouble() > 0.5)
                        return "true";
                    else
                        return "false";
                case "char":
                    char _char = (char)(10 * r.NextDouble());
                    return _char.ToString(CultureInfo.InvariantCulture);
                case "short":
                    short _short = (short)(10 * r.NextDouble());
                    return _short.ToString(CultureInfo.InvariantCulture);
                case "ushort":
                    ushort _ushort = (ushort)(10 * r.NextDouble());
                    return _ushort.ToString(CultureInfo.InvariantCulture);
                case "int":
                    int _int = (int)(10 * r.NextDouble());
                    return _int.ToString(CultureInfo.InvariantCulture);
                case "uint":
                    uint _uint = (uint)(10 * r.NextDouble());
                    return _uint.ToString(CultureInfo.InvariantCulture);
                case "long":
                    long _long = (long)(10 * r.NextDouble());
                    return _long.ToString(CultureInfo.InvariantCulture);
                case "ulong":
                    ulong _ulong = (ulong)(10 * r.NextDouble());
                    return _ulong.ToString(CultureInfo.InvariantCulture);
                case "float":
                    float _float = (float)(10 * r.NextDouble());
                    return _float.ToString(CultureInfo.InvariantCulture);
                case "double":
                    double _double = 10*r.NextDouble();
                    return _double.ToString(CultureInfo.InvariantCulture);
                case "decimal":
                    decimal _decimal = (decimal)(10*r.NextDouble());
                    return _decimal.ToString(CultureInfo.InvariantCulture);
                default:
                    return "1";
            }

        }

        public static string generate(ListType type)
        {
            //maybe a list of 0 elemets
            int cnt = (int)(r.NextDouble() * 5);
            if (cnt > 0)
            {
                string ret = "new List<" + type.ElementFieldType.CSharpName + ">(new " + type.ElementFieldType.CSharpName + "[]{";
                for (int i = 0; i < cnt; ++i)
                {
                    ret += generate(type.ElementFieldType);
                    ret += ",";
                }
                if (ret[ret.Length - 1] == ',')
                    ret = ret.Remove(ret.Length - 1);
                ret += "})";
                return ret;
            }
            else
                return "new List<" + type.ElementFieldType.CSharpName + ">()";
        }
        public static string generate(StructFieldType type)
        {
            string ret = "new "+type.CSharpName+"(";
            foreach(Field field in type.descriptor.Fields)
            {
                ret+=generate(field.Type)+",";
            }

            if (ret[ret.Length - 1] != '(')
            {
                ret = ret.Remove(ret.Length - 1);
            }
            ret += ")";
            return ret;
        }

        public static string generate(StringType type)
        {
            string ret = "\"";
            //maybe 0 chars
            int cnt = (int)(r.NextDouble() * 10) ;
            for (int i = 0; i < cnt; ++i)
            {
                ret += "abc";
            }
            ret += "\"";
            return ret;
        }

        public static string generate(ArrayType type)
        {
            string ret = "";
            ret += "new " + type.ElementType.Name + "[";
            if(type.lengths.Length>0)
                foreach(int length in type.lengths)
                    ret += length.ToString(CultureInfo.InvariantCulture)+",";
            ret = ret.TrimEnd(",".ToCharArray());
            ret += "]";
            return ret;
        }

        public static string generate(FieldType type)
        {
            if (type is AtomType)
                return generate((AtomType)type);
            else if (type is StructFieldType)
                return generate((StructFieldType)type);
            else if (type is ListType)
                return generate((ListType)type);
            else if (type is StringType)
                return generate((StringType)type);
            else if (type is ArrayType)
                return generate((ArrayType)type);

            return "";
        }
    }

}