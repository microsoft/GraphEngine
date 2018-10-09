using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Trinity.Core.Lib;
using Trinity.Diagnostics;

namespace GraphEngine.DataImporter
{
    enum ValueType
    {
        vt_datetime,
        vt_literal,
        vt_graphedge,
        vt_others,
        vt_guid,
    }

    struct Triple : IComparable<Triple>
    {
        public string Subject { get; set; }
        public string Predicate { set; get; }
        public string Object { set; get; }
        public ValueType ValueType { get; set; }
        public int CompareTo(Triple other)
        {
            if (Predicate == other.Predicate)
            {
                Object.CompareTo(other.Object);
            }
            return Predicate.CompareTo(other.Predicate);
        }
    }

    class RDFEntity
    {
        public string MID;
        public List<Triple> PropertyValues;
    }

    class RDFUtils
    {
        internal static char c_separator = '\t';
        private const string c_freebasens_pfx = "<http://rdf.freebase.com/ns/";
        private const string c_gYear = "^^<http://www.w3.org/2001/XMLSchema#gYear>";
        private const string c_date = "^^<http://www.w3.org/2001/XMLSchema#date>";
        private const string c_gYearMonth = "^^<http://www.w3.org/2001/XMLSchema#gYearMonth>";
        private const string c_datetime = "^^<http://www.w3.org/2001/XMLSchema#dateTime>";
        private const int c_freebasens_pfxlen = 28;


        internal static Triple ParseTriple(string line)
        {
            string[] splits = line.Split(c_separator);
            if (splits.Length < 3)
            {
                Log.WriteLine("Skip: {0}", line);
                return new Triple();
            }

            bool is_literal;

            string S = NormalizeLiteral(splits[0], out is_literal);
            if (S == null) return new Triple();
            if (!IsMID(S)) return new Triple();

            string P = NormalizeLiteral(splits[1], out is_literal);
            if (P == null) return new Triple();

            if (!IsNotBaseOrUser(P)) return new Triple(); //TODO hard-coded freebase rule

            string O = NormalizeLiteral(splits[2], out is_literal);
            if (O == null) return new Triple();

            Triple triple = new Triple();
            triple.Subject = S;
            triple.Predicate = P;

            if (IsMID(O))
            {
                triple.Object = MID2CellId(O).ToString();
                triple.ValueType = ValueType.vt_graphedge;
            }
            else
            {
                triple.Object = O;
                if (is_literal)
                {
                    triple.ValueType = ValueType.vt_literal;
                }
                else if (O.EndsWith(c_gYear) || O.EndsWith(c_gYearMonth) || O.EndsWith(c_date) || O.EndsWith(c_datetime))
                {
                    O = O.Substring(1, O.IndexOf("\"^^"));
                    triple.ValueType = ValueType.vt_datetime;
                }
                //TODO guid?
                else
                {
                    //vt_others means that we are not sure about the type at this moment.
                    triple.ValueType = ValueType.vt_others;
                }
            }

            return triple;
        }

        //  is_literal: strong evidence that the string is a literal string.
        //  currently we detect this by examine the @en suffix
        internal static string NormalizeLiteral(string s, out bool is_literal)
        {
            is_literal = false;
            if (s.StartsWith(c_freebasens_pfx) && s.EndsWith(">"))
            {
                return s.Substring(c_freebasens_pfxlen, s.Length - c_freebasens_pfxlen - 1);
            }

            if (s.StartsWith("\"") && s.EndsWith("\""))
            {
                return s.Substring(1, s.Length - 2);
            }

            if (s.StartsWith("\"") && s.EndsWith("\"@en"))
            {
                is_literal = true;
                return s.Substring(1, s.Length - 5);
            }

            return null;
        }

        internal static FieldType GetFieldType(ref Triple triple)
        {
            if (triple.ValueType == ValueType.vt_graphedge) return FieldType.ft_graphedge;
            if (triple.ValueType == ValueType.vt_literal) return FieldType.ft_string;
            if (triple.ValueType == ValueType.vt_datetime) return FieldType.ft_datetime;
            if (triple.ValueType == ValueType.vt_guid) return FieldType.ft_guid;
            var value = triple.Object;


            if (value.Contains(".") || value.Contains("E") || value.Contains("e")) // floating point value
            {
                float f; double d; decimal de;
                if (float.TryParse(value, out f))
                    return FieldType.ft_float;
                if (double.TryParse(value, out d))
                    return FieldType.ft_double;
                if (decimal.TryParse(value, out de))
                    return FieldType.ft_decimal;
            }
            else // integer
            {
                if (value.StartsWith("-")) // signed value
                {
                    sbyte s; Int16 i16; Int32 i32; Int64 i64;
                    if (sbyte.TryParse(value, out s))
                        return FieldType.ft_sbyte;
                    if (Int16.TryParse(value, out i16))
                        return FieldType.ft_short;
                    if (Int32.TryParse(value, out i32))
                        return FieldType.ft_int;
                    if (Int64.TryParse(value, out i64))
                        return FieldType.ft_long;
                }
                else // unsigned value
                {
                    byte b; UInt16 u16; UInt32 u32; UInt64 u64;
                    if (byte.TryParse(value, out b))
                        return FieldType.ft_byte;
                    if (UInt16.TryParse(value, out u16))
                        return FieldType.ft_ushort;
                    if (UInt32.TryParse(value, out u32))
                        return FieldType.ft_uint;
                    if (UInt64.TryParse(value, out u64))
                        return FieldType.ft_ulong;
                }
            }

            {
                bool b; DateTime dt; Guid guid;

                if (Boolean.TryParse(value, out b)) return FieldType.ft_bool;
                if (DateTime.TryParse(value, out dt)) return FieldType.ft_datetime;
                if (Guid.TryParse(value, out guid)) return FieldType.ft_guid;
            }

            return FieldType.ft_string;
        }

        internal static bool IsNotBaseOrUser(string p)
        {
            if (p.StartsWith("base.") || p.StartsWith("user.") || p.StartsWith("freebase."))
                return false;
            return true;
        }

        internal static bool IsMID(string candidate)
        {
            return candidate.StartsWith("m.");
        }

        internal static string GetTslName(string name)
        {
            name = name.Replace("zzzz", "type_object_type").Replace("zz", "type_object");
            return name.Replace(".", "_").Replace("/", "_").Replace(":", "_").Replace("-", "_");
        }

        internal static string GetTypeName(string predicate)
        {
            if (predicate.StartsWith("zz"))
            {
                return "type_object";
            }

            int index = predicate.LastIndexOf('.');
            if (index < 0)
            {
                return GetTslName(predicate);
            }
            string prefix = predicate.Substring(0, index);
            return GetTslName(prefix);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe long RootCellIdToTypedCellId(long cellid, ushort cellType)
        {
            ushort* sp = (ushort*)&cellid;
            *(sp + 3) = cellType;
            return cellid;
        }

        public static unsafe long MID2CellId(string mid, ushort cellType = 1)
        {
            //string resolveId;
            //if (!conflictResolveTable.TryGetValue(mid, out resolveId))
            //{
            //    resolveId = mid;
            //}
            long id = HashHelper.HashString2Int64(mid);
            ushort* sp = (ushort*)&id;
            *(sp + 2) = (ushort)(*(sp + 2) ^ *(sp + 3));
            *(sp + 3) = cellType;
            return id;
        }


        public static IEnumerable<List<string>> GroupSortedLinesBySubject(IEnumerable<string> input)
        {
            List<string> entity = new List<string>();
            int idx;
            string entity_s = "";
            string s = "";
            foreach (var line in input)
            {
                try
                {
                    idx = line.IndexOf('\t');
                    s = line.Substring(0, idx);
                }
                catch { continue; }
                if (s != entity_s)
                {
                    entity_s = s;
                    if (entity.Count != 0) { yield return entity; }
                    entity = new List<string> { line };
                }
                else
                {
                    entity.Add(line);
                }
            }
            if (entity.Count != 0) { yield return entity; }
        }
    }
}
