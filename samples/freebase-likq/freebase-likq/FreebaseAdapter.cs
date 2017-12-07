using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;

using Trinity;
using Trinity.Storage;
using Trinity.Core.Lib;
using Trinity.TSL.Lib;
using FanoutSearch;
using freebase_tsl;

namespace freebase_likq
{
    public unsafe class CellGroupUtil
    {
        // Resolve MID <---> CellID Mapping
        private static Dictionary<string, ushort> prop2Type       = new Dictionary<string, ushort>();
        private static Dictionary<ushort, string> CellTypeNames   = new Dictionary<ushort, string>();
        private static HashSet<string> singleValuedStrProps       = new HashSet<string>();
        private static Dictionary<string, string> midResolveTable = new Dictionary<string, string>();
        private static Dictionary<string, string> normp2Orignal   = new Dictionary<string, string>();

        private static bool init = false;

        static CellGroupUtil()
        {
            Init();
        }

        public static void Init()
        {
            if (init)
            {
                return;
            }
            init = true;

            #region Init Predicate to Type Mapping
            var ts = Assembly.GetAssembly(typeof(type_object)).GetTypes();
            foreach (var type in ts)
            {
                string tname = type.Name;
                if (type.IsValueType && !type.IsEnum && !type.IsPrimitive)
                {
                    foreach (var field in type.GetFields())
                    {
                        string fname = field.Name;
                        if (field.FieldType == typeof(string))
                        {
                            singleValuedStrProps.Add(fname);
                        }

                        if (fname == "CellID" || fname == "cellType")
                        {
                            continue;
                        }
                        if (prop2Type.ContainsKey(fname))
                        {
                            continue;
                        }
                        else
                        {
                            CellType ct;
                            if (Enum.TryParse<CellType>(tname, out ct))
                            {
                                prop2Type.Add(fname, (ushort)ct);
                            }
                        }
                    }
                }
            }
            #endregion

            #region resolve cell name -> type code mapping
            foreach (var cd in StorageSchema.CellDescriptors)
            {
                CellTypeNames.Add(cd.CellType, cd.TypeName);
            }
            #endregion
        }

        public static string GetTypeName(ushort typeCode)
        {
            string ret = "";
            CellTypeNames.TryGetValue(typeCode, out ret);
            return ret;
        }

        public static ushort GetTypeCode(string normalizedProp)
        {
            ushort ct = 1;
            prop2Type.TryGetValue(normalizedProp, out ct);
            return ct;
        }

        public static string ResolveMid(string originialMid)
        {
            string resolved;
            if (midResolveTable.TryGetValue(originialMid, out resolved))
            {
                return resolved;
            }
            return originialMid;
        }

        public static bool IsFreebaseEntity(string text)
        {
            return text.StartsWith("m.") || (text.StartsWith("<http://rdf.freebase.com/ns/m.") && text.EndsWith(">"));
        }

        public static long GetCellID(string mid, ushort cellType = 1)
        {
            long id = HashHelper.HashString2Int64(mid);
            ushort* sp = (ushort*)&id;
            *(sp + 2) = (ushort)(*(sp + 2) ^ *(sp + 3));
            *(sp + 3) = cellType;
            return id;
        }

        public static long GetCellID(long id, ushort cellType = 1)
        {
            ushort* sp = (ushort*)&id;
            *(sp + 3) = cellType;
            return id;
        }

        public static string Abbr(string uri)
        {
            string abbr = uri.TrimStart('<').TrimEnd('>').Replace("http://rdf.freebase.com/ns/", "");
            return abbr;
        }

        public static string Restore(string normp)
        {
            string orignal;
            if (!normp2Orignal.TryGetValue(normp, out orignal))
            {
                return normp;
            }
            return orignal;
        }

        public static string Normalize(string uri)
        {
            return uri.Replace(".", "_").Replace("/", "_").Replace(":", "_").Replace("-", "_").Replace("#", "_");
        }

        public static string GetPType(string predicate)
        {
            string original;
            if (!normp2Orignal.TryGetValue(predicate, out original))
            {
                original = predicate;
            }
            //string original = normp2Orignal.TryGetValue(predicate, out original) ? original : predicate;

            string type = predicate.Substring(0, original.LastIndexOf('.'));
            return Normalize(type);
        }
    }

    public class CellGroupAccessor : IFanoutSearchCellAccessor
    {
        #region Fields
        private long cell_id;
        private long category_cell_id;
        private List<long> child_ids = new List<long>();
        private bool child_ids_initialized = false;
        [ThreadStatic]
        private static CellGroupAccessor s_accessor = null;
        #endregion

        public CellGroupAccessor(long id)
        {
            CellID = id;
            category_cell_id = id;
        }

        public static CellGroupAccessor New(long id)
        {
            if (s_accessor == null)
            {
                return new CellGroupAccessor(id);
            }
            else
            {
                s_accessor.CellID = id;
                s_accessor.child_ids.Clear();
                s_accessor.child_ids_initialized = false;
                s_accessor.category_cell_id = id;
                return s_accessor;
            }
        }

        #region Private methods
        private long RouteToChildCell(string fieldName)
        {
            //string np = CellGroupUtil.Normalize(fieldName);
            ushort ct = CellGroupUtil.GetTypeCode(fieldName);
            return CellGroupUtil.GetCellID(CellID, ct);
        }

        private IEnumerable<long> ChildrenIDs()
        {
            using (var cell = Global.LocalStorage.Usetype_object(CellID))
            {
                foreach (var type in cell.types)
                {
                    yield return CellGroupUtil.GetCellID(CellID, type);
                }
            }
        }

        private IEnumerable<long> GetChildrenIDs()
        {
            if (!child_ids_initialized)
            {
                child_ids.AddRange(ChildrenIDs());
                child_ids_initialized = true;
            }
            return child_ids;
        }

        public void AppendToField<T>(string fieldName, T value)
        {
            long child_id = RouteToChildCell(fieldName);
            using (var cell = Global.LocalStorage.UseGenericCell(child_id, CellAccessOptions.CreateNewOnCellNotFound))
            {
                cell.AppendToField<T>(fieldName, value);
            }
        }


        #endregion

        #region ICell Members
        public long CellID
        {
            get
            {
                return cell_id;
            }
            set
            {
                cell_id = CellGroupUtil.GetCellID(value);
            }
        }

        public IEnumerable<T> EnumerateField<T>(string fieldName)
        {
            long child_id = RouteToChildCell(fieldName);
            using (var cell = Global.LocalStorage.UseGenericCell(child_id, CellAccessOptions.ReturnNullOnCellNotFound))
            {
                if (cell != null && cell.ContainsField(fieldName))
                {
                    foreach (var item in cell.EnumerateField<T>(fieldName))
                    {
                        yield return item;
                    }
                }
            }
        }

        public IEnumerable<T> EnumerateValues<T>(string attributeKey = null, string attributeValue = null)
        {
            foreach (var child in GetChildrenIDs())
            {
                using (var cell = Global.LocalStorage.UseGenericCell(child, CellAccessOptions.ReturnNullOnCellNotFound))
                {
                    if (cell != null)
                    {
                        foreach (var item in cell.EnumerateValues<T>(attributeKey, attributeValue))
                        {
                            yield return item;
                        }
                    }
                }
            }
        }

        public T GetField<T>(string fieldName)
        {
            if (fieldName.Equals("entity_ids"))
            {
                using (var cell = Global.LocalStorage.UseGenericCell(category_cell_id, CellAccessOptions.ReturnNullOnCellNotFound))
                {
                    if (cell == null)
                    {
                        return default(T);
                    }
                    return cell.GetField<T>(fieldName);
                }
            }

            if (fieldName.Equals("type_object_type"))
            {
                using (var cell = Global.LocalStorage.UseGenericCell(CellID, CellAccessOptions.ReturnNullOnCellNotFound))
                {
                    if (cell == null)
                    {
                        throw new ArgumentException("Undefined field.");
                    }

                    var type_names = cell.GetField<List<ushort>>("types").Select(tcode => CellGroupUtil.GetTypeName(tcode));
                    return _convert<T>(type_names);
                }
            }

            long child_id = RouteToChildCell(fieldName);
            using (var cell = Global.LocalStorage.UseGenericCell(child_id, CellAccessOptions.ReturnNullOnCellNotFound))
            {
                if (cell == null)
                {
                    return default(T);
                }
                return cell.GetField<T>(fieldName);
            }
        }

        public IEnumerable<KeyValuePair<string, T>> SelectFields<T>(string attributeKey = null, string attributeValue = null)
        {
            foreach (var child in GetChildrenIDs())
            {
                using (var cell = Global.LocalStorage.UseGenericCell(child, CellAccessOptions.ReturnNullOnCellNotFound))
                {
                    if (cell != null)
                    {
                        if (!_check_attribute(cell, attributeKey, attributeValue))
                            continue;

                        IEnumerable<KeyValuePair<string, T>> sub_cell_enum_results = null;
                        try
                        {
                            sub_cell_enum_results = cell.SelectFields<T>(attributeKey, attributeValue).ToList();
                        }
                        catch { }

                        if (sub_cell_enum_results != null)
                        {
                            foreach (var item in sub_cell_enum_results)
                            {
                                yield return item;
                            }
                        }
                    }
                }
            }
        }

        private bool _check_attribute(ICellAccessor cell, string attributeKey, string attributeValue)
        {

            if (attributeKey == null)
                return true;

            if (attributeValue == null)
            {
                return (cell.GetFieldDescriptors().Any(
                    fd =>
                    fd.Attributes.ContainsKey(attributeKey)));
            }
            else
            {
                return (cell.GetFieldDescriptors().Any(
                    fd =>
                    {
                        string val;
                        return (fd.Attributes.TryGetValue(attributeKey, out val) && attributeValue == val);
                    }));
            }
        }

        public void SetField<T>(string fieldName, T value)
        {
            long child_id = RouteToChildCell(fieldName);
            using (var cell = Global.LocalStorage.UseGenericCell(child_id, CellAccessOptions.CreateNewOnCellNotFound))
            {
                cell.SetField<T>(fieldName, value);
            }
        }

        public bool ContainsField(string fieldName)
        {
            long child_id = RouteToChildCell(fieldName);
            using (var cell = Global.LocalStorage.UseGenericCell(child_id, CellAccessOptions.ReturnNullOnCellNotFound))
            {
                return cell.ContainsField(fieldName);
            }
        }
        #endregion

        #region ICellDescriptor Members

        public IAttributeCollection GetFieldAttributes(string fieldName)
        {
            long child_id = RouteToChildCell(fieldName);
            using (var cell = Global.LocalStorage.UseGenericCell(child_id, CellAccessOptions.CreateNewOnCellNotFound))
            {
                return cell.GetFieldAttributes(fieldName);
            }
        }

        public IEnumerable<IFieldDescriptor> GetFieldDescriptors()
        {
            foreach (var child in GetChildrenIDs())
            {
                using (var cell = Global.LocalStorage.UseGenericCell(child, CellAccessOptions.CreateNewOnCellNotFound))
                {
                    foreach (var fd in cell.GetFieldDescriptors())
                    {
                        yield return fd;
                    }
                }
            }
        }

        public IEnumerable<string> GetFieldNames()
        {
            foreach (var child in GetChildrenIDs())
            {
                using (var cell = Global.LocalStorage.UseGenericCell(child, CellAccessOptions.ReturnNullOnCellNotFound))
                {
                    if (cell != null)
                    {
                        foreach (var field_name in cell.GetFieldNames())
                        {
                            yield return field_name;
                        }
                    }
                }
            }
        }

        #endregion

        #region ITypeDescriptor Members

        public Type Type
        {
            get { return this.GetType(); }
        }

        public string TypeName
        {
            get { return this.GetType().Name; }
        }

        #endregion

        #region IAttributeCollection Members

        public IReadOnlyDictionary<string, string> Attributes
        {
            get { throw new NotImplementedException(); }
        }

        public string GetAttributeValue(string attributeKey)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Members
        public void Dispose()
        {
            if (s_accessor == null)
            {
                s_accessor = this;
            }
        }

        #endregion

        public ushort CellType
        {
            get { return 0; }
        }

        private void BuildCellGroupContent(ref bool empty, ICell sub_cell, StringBuilder sb, bool include_cell_id)
        {
            if (sub_cell != null)
            {
                List<string> available_field_names = sub_cell.GetFieldNames().ToList();

                foreach (var field_desc in sub_cell.GetFieldDescriptors())
                {
                    if (field_desc.Attributes.ContainsKey("GraphEdge"))
                        continue;

                    if (!available_field_names.Contains(field_desc.Name))
                        continue;

                    string field_value = sub_cell.GetField<string>(field_desc.Name);

                    if (field_desc.Type == typeof(string))
                    {
                        field_value = JsonStringProcessor.escape(field_value);
                    }

                    if (empty)
                    {
                        empty = false;
                    }
                    else
                    {
                        sb.Append(',');
                    }
                    sb.Append(String.Format(@"""{0}"":{1}", field_desc.Name, field_value));
                }

                if (include_cell_id)
                {
                    if (empty)
                    {
                        empty = false;
                    }
                    else
                    {
                        sb.Append(',');
                    }
                    sb.Append(String.Format("\"CellID\":{0}", sub_cell.CellID));
                }
            }
        }

        public override string ToString()
        {
            bool empty = true;
            StringBuilder sb = new StringBuilder();
            sb.Append('{');

            using (var cell = Global.LocalStorage.Usetype_object(cell_id, CellAccessOptions.ReturnNullOnCellNotFound))
            {
                BuildCellGroupContent(ref empty, cell, sb, include_cell_id: true);
            }

            foreach (var child in GetChildrenIDs())
            {
                using (var cell = Global.LocalStorage.UseGenericCell(child, CellAccessOptions.ReturnNullOnCellNotFound))
                {
                    BuildCellGroupContent(ref empty, cell, sb, include_cell_id: false);
                }
            }

            sb.Append('}');
            return sb.ToString();
        }

        private struct CellTypeLookup<T> where T : ICell, new()
        {
            public static ushort cellId = new T().CellType;
        }

        #region FanoutSearch extension
        public T Cast<T>() where T : ICellAccessor
        {
            ushort cell_type;
            string cell_type_name = typeof(T).Name;
            cell_type_name = cell_type_name.Substring(0, cell_type_name.Length - 9);
            cell_type = (ushort)StorageSchema.GetCellType(cell_type_name);

            long cid = CellGroupUtil.GetCellID(cell_id, cell_type);

            var generic_child = Global.LocalStorage.UseGenericCell(cid, CellAccessOptions.ReturnNullOnCellNotFound);

            if (generic_child == null)
                return default(T);

            return (T)generic_child;
        }

        public bool isOfType(string cell_type_name)
        {
            ushort cell_type = (ushort)StorageSchema.GetCellType(cell_type_name);
            long cid = CellGroupUtil.GetCellID(cell_id, cell_type);

            var generic_child = Global.LocalStorage.UseGenericCell(cid, CellAccessOptions.ReturnNullOnCellNotFound);

            if (generic_child == null)
                return false;

            generic_child.Dispose();
            return true;
        }
        #endregion

        public bool IsOfType<T>()
        {
            return false;
        }

        public bool IsList()
        {
            return false;
        }

        /// <summary>
        /// Tries to serialize the string collection to a string, then convert to T.
        /// Only works when T is string/object
        /// </summary>
        private static T _convert<T>(IEnumerable<string> values)
        {
            return (T)(object)FanoutSearchModule.ToJsonArray(values);
        }

        #region IAccessor
        public unsafe byte* GetUnderlyingBufferPointer()
        {
            throw new NotImplementedException();
        }

        public byte[] ToByteArray()
        {
            throw new NotImplementedException();
        }

        public int GetBufferLength()
        {
            throw new NotImplementedException();
        }

        public ResizeFunctionDelegate ResizeFunction
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }
        #endregion

    }
}