#pragma warning disable 0162 // disable the "unreachable code" warning
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
using Trinity.TSL.Lib;
using Trinity.Storage;
using Trinity.Daemon;
using Trinity.Diagnostics;

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
/*IF("Trinity::Codegen::contains_substring_index")*/
using t_Namespace.InvertedIndex;
/*END*/
using t_Namespace.Linq;
namespace t_Namespace
{
    [TARGET("NTSL")]
    [MAP_LIST("t_index", "TSLIndexTargetVector")]
    [MAP_VAR("t_index", "")]
    [MAP_VAR("t_index_id", "(*TSLIndexIdMap)[$$]")]
    [MAP_VAR("t_index_name", "*$$->cell->name + '_' + get__index_member_name($$)")]
    [MAP_VAR("t_index_access_path_for_accessor", "*$$->cell->name + '.' + *$$->target", MemberOf = "t_index")]
    [MAP_VAR("t_index_access_path_for_cell", "get__index_access_path_for_cell($$)")]
    [MAP_VAR("t_field_name", "*$$->cell->name + '.' + *$$->target", MemberOf = "t_index")]
    [MAP_VAR("t_index_access_method", "$$->target_field->fieldType->is_string() ? \"Contains\":\"ElementContainsSubstring\"", MemberOf = "t_index")]
    /// <summary>
    /// Provides indexing capabilities on <see cref="Trinity.Storage.LocalMemoryStorage"/>.
    /// The target field to query on is specified with <paramref name="query"/>, <seealso cref="t_Namespace.IndexIdentifier"/>.
    /// </summary>
    public class Index : __meta
    {

        static Index()
        {
            BackgroundIndexUpdater();
            BackgroundThread.AddBackgroundTask(new BackgroundTask(BackgroundIndexUpdater, s_BackgroundIndexUpdateInterval));
        }

        //XXX fixed at 10 mins
        static          readonly    int                         s_BackgroundIndexUpdateInterval      = 60*1000*10;
        static          readonly    object                      s_IndexLock                          = new object();
        static internal readonly    Dictionary<string,string>   s_AccessorSubstringIndexAccessMethod = new Dictionary<string, string>
        {
            /*FOREACH(",")*/
            { "t_index_access_path_for_accessor", "t_index_access_method"}
            /*END*/
        };
        static internal readonly    Dictionary<string,SubstringQueryDelegate>
                                                                s_AccessorSubstringQueryMethodTable = 
            new Dictionary<string, SubstringQueryDelegate>
        {
            /*FOREACH(",")*/
            { "t_index_access_path_for_accessor", t_index_name_SubstringQuery }
            /*END*/
        };
        static internal readonly    Dictionary<string,SubstringWildcardQueryDelegate>
                                                                s_AccessorSubstringWildcardQueryMethodTable = 
            new Dictionary<string, SubstringWildcardQueryDelegate>
        {
            /*FOREACH(",")*/
            { "t_index_access_path_for_accessor", t_index_name_SubstringQuery }
            /*END*/
        };
        static internal readonly    Dictionary<string,string>
                                                                s_CellSubstringIndexAccessMethod = 
            new Dictionary<string, string>
        {
            /*FOREACH(",")*/
            { "t_index_access_path_for_cell", "t_index_access_method"}
            /*END*/
        };
        static internal readonly    Dictionary<string,SubstringQueryDelegate>
                                                                s_CellSubstringQueryMethodTable = 
            new Dictionary<string, SubstringQueryDelegate>
        {
            /*FOREACH(",")*/
            { "t_index_access_path_for_cell", t_index_name_SubstringQuery }
            /*END*/
        };
        static internal readonly    Dictionary<string,SubstringWildcardQueryDelegate>
                                                                s_CellSubstringWildcardQueryMethodTable = 
            new Dictionary<string, SubstringWildcardQueryDelegate>
        {
            /*FOREACH(",")*/
            { "t_index_access_path_for_accessor", t_index_name_SubstringQuery }
            /*END*/
        };

        [FOREACH]
        /// <summary>
        /// Performs a substring search.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<long> t_index_name_SubstringQuery(string value)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = t_index_name_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for t_index_name is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return t_index_name_substring_searcher.SubstringSearch(value);
        }

        /// <summary>
        /// Performs a substring search for a list of keywords. The match pattern is:
        /// keywords[0]*keywords[1]..., where * is the wildcard symbol.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<long> t_index_name_SubstringQuery(List<string> values)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = t_index_name_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for t_index_name is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return t_index_name_substring_searcher.SubstringSearch(values);
        }

        /// <summary>
        /// Performs a substring search using the specified keywords. The match pattern is:
        /// keywords[0]*keywords[1]..., where * is the wildcard symbol.
        /// </summary>
        /// <param name="keywords">A list of keywords.</param>
        /// <returns>A list of matched cell Ids.</returns>
        public static List<long> t_index_name_SubstringQuery(params string[] values)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = t_index_name_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for t_index_name is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return t_index_name_substring_searcher.SubstringSearch(values);
        }
        [END]

        static int BackgroundIndexUpdater()
        {
            FOREACH();
            IF("$t_index->type == IT_SUBSTRING");
            {
                UpdateSubstringQueryIndex(t_field_name);
            }
            END();
            END();
            return s_BackgroundIndexUpdateInterval;
        }

        #region Fields
        [FOREACH]
        [IF("$t_index->type == IT_SUBSTRING")]
        internal static object                  t_index_name_substring_index_lock = new object();
        internal static InvertedBigramIndexer   t_index_name_substring_index;
        internal static InvertedBigramSearcher  t_index_name_substring_searcher;
        internal static ulong                   t_index_name_substring_index_version = 0;
        [END]//IF
        [END]//FOREACH
        #endregion

        #region Index interfaces
        internal static List<long> SubstringQuery(string index_id, string query)
        {
            SubstringQueryDelegate query_method;
            if (!s_AccessorSubstringQueryMethodTable.TryGetValue(index_id, out query_method))
                throw new Exception("Unrecognized index id.");
            return query_method(query);
        }

        [MAP_VAR("t_field_name", "target", MemberOf = "t_index")]
        /// <summary>
        /// Performs a substring query on <see cref="Trinity.Global.LocalStorage"/>.
        /// The target field to query on is specified with <paramref name="query"/>, <seealso cref="t_Namespace.IndexIdentifier"/>.
        /// </summary>
        /// <param name="index_id">The identifier of the field that the query should be performed on.</param>
        /// <param name="query">The query string.</param>
        /// <returns>
        /// A list of cell ids, of which the given query string is a substring of the field, or a substring of
        /// an element if the target field is a container of strings.
        /// </returns>
        public static List<long> SubstringQuery(IndexIdentifier index_id, string query)
        {
            switch (index_id.id)
            {
                /*FOREACH*/
                /*IF("$t_index->type == IT_SUBSTRING")*/
                case t_index_id:
                    {
                        return t_index_name_SubstringQuery(query);
                    }
                /*END*/
                /*END*/
                default:
                    throw new Exception("The target field does not have a substring index.");
            }
        }

        /// <summary>
        /// Performs a substring sequence query on <see cref="Trinity.Global.LocalStorage"/>. The semantics of
        /// this type of query is that, given a sequence of query strings <c>q_1, q_2,...,q_n</c> in 
        /// <paramref name="query"/>, the match condition on a target string is that all the strings in the sequence
        /// are the substrings of the target string, and the order of the substring occurrences should correspond
        /// strictly to that given by <paramref name="query"/>. For example (let <c>S</c> denote the target string):
        /// <example>
        /// <list type="bullet">
        /// <item>
        /// <c>S=..q1....q2..q3{and so on}...qn...</c> is a match.
        /// </item>
        /// <item>
        /// <c>S=.....q1q2..q4{and there are missing substrings in the sequence}...qn...</c> is not match.
        /// </item>
        /// <item>
        /// <c>S=..q5..q3.q1{all the substrings in the sequence are present, but never in the corresponding order}...qn...</c> is not match.
        /// </item>
        /// </list>
        /// </example>
        /// The target field to query on is specified with <paramref name="query"/>, <seealso cref="t_Namespace.IndexIdentifier"/>.
        /// </summary>
        /// <param name="index_id">The identifier of the field that the query should be performed on.</param>
        /// <param name="query">The sequence of query strings.</param>
        /// <returns>
        /// A list of cell ids, of which the given query string sequence is matched on the target field, or
        /// an element it if it is a container of strings.
        /// </returns>
        public static List<long> SubstringQuery(IndexIdentifier index_id, List<string> query)
        {
            switch (index_id.id)
            {
                /*FOREACH*/
                /*IF("$t_index->type == IT_SUBSTRING")*/
                case t_index_id:
                    {
                        return t_index_name_SubstringQuery(query);
                    }
                /*END*/
                /*END*/
                default:
                    throw new Exception("The target field does not have a substring index.");
            }
        }

        /// <summary>
        /// Performs a substring sequence query on <see cref="Trinity.Global.LocalStorage"/>. The semantics of
        /// this type of query is that, given a sequence of query strings <c>q_1, q_2,...,q_n</c> in 
        /// <paramref name="query"/>, the match condition on a target string is that all the strings in the sequence
        /// are the substrings of the target string, and the order of the substring occurrences should correspond
        /// strictly to that given by <paramref name="query"/>. For example (let <c>S</c> denote the target string):
        /// <example>
        /// <list type="bullet">
        /// <item>
        /// <c>S=..q1....q2..q3{and so on}...qn...</c> is a match.
        /// </item>
        /// <item>
        /// <c>S=.....q1q2..q4{and there are missing substrings in the sequence}...qn...</c> is not match.
        /// </item>
        /// <item>
        /// <c>S=..q5..q3.q1{all the substrings in the sequence are present, but never in the corresponding order}...qn...</c> is not match.
        /// </item>
        /// </list>
        /// </example>
        /// The target field to query on is specified with <paramref name="query"/>, <seealso cref="t_Namespace.IndexIdentifier"/>.
        /// </summary>
        /// <param name="index_id">The identifier of the field that the query should be performed on.</param>
        /// <param name="query">The sequence of query strings.</param>
        /// <returns>
        /// A list of cell ids, of which the given query string sequence is matched on the target field, or
        /// an element it if it is a container of strings.
        /// </returns>
        public static List<long> SubstringQuery(IndexIdentifier index_id, params string[] query)
        {
            switch (index_id.id)
            {
                /*FOREACH*/
                /*IF("$t_index->type == IT_SUBSTRING")*/
                case t_index_id:
                    {
                        return t_index_name_SubstringQuery(query);
                    }
                /*END*/
                /*END*/
                default:
                    throw new Exception("The target field does not have a substring index.");
            }
        }

        [MAP_VAR("t_cell_name", "cell->name", MemberOf = "t_index")]
        /// <summary>
        /// Updates the index on the given field.
        /// </summary>
        /// <param name="index_id">The identifier of the field whose index should be rebuilt.</param>
        public static void UpdateSubstringQueryIndex(IndexIdentifier index_id)
        {
            switch (index_id.id)
            {
                /*FOREACH*/
                /*IF("$t_index->type == IT_SUBSTRING")*/
                case t_index_id:
                    {
                        Log.WriteLine(LogLevel.Info, "Index: updating substring index of t_index_name");
                        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();

                        InvertedBigramIndexer  new_index;
                        InvertedBigramSearcher new_searcher;
                        ulong                  old_version = t_index_name_substring_index_version;
                        /**
                         *  Lock the original index to prevent multiple updates
                         *  to be executed simultaneously.
                         */

                        lock (t_index_name_substring_index_lock)
                        {
                            /*
                             * If the index version has changed, it means that someone else
                             * has updated the index while we're blocked on the old index object.
                             */
                            if (t_index_name_substring_index_version != old_version)
                            {
                                Log.WriteLine(LogLevel.Info, "Index: Substring index of t_index_name is already updated, stopping current action.");
                                break;
                            }

                            new_index = new InvertedBigramIndexer("t_index_name");
                            foreach (var accessor in Global.LocalStorage.t_cell_name_Accessor_Selector())
                            {
                                bool optional_not_exist = true;
                                META_VAR("auto", "access_chain", "$t_index->resolve_target()");
                                META_VAR("std::string", "field_name");
                                META_VAR("bool", "has_optional", "false");
                                META("for(auto *field : *%access_chain){");
                                MAP_VAR("t_field_name", "%field_name + \"Contains_\" + *field->name");
                                IF("field->is_optional()");
                                META("%has_optional = true;");
                                if (accessor.t_field_name)
                                    END();
                                META("%field_name.append(*field->name).append(\".\");}");
                                IF("%has_optional");
                                {
                                    optional_not_exist = false;
                                }
                                if (optional_not_exist)
                                    break;
                                END();
                                IF("$t_index->target_field->fieldType->is_string()");
                                {
                                    MAP_VAR("t_field_name", "target", MemberOf = "t_index");
                                    new_index.AddItem(accessor.t_field_name, accessor.CellId);
                                }
                                ELSE();
                                {
                                    META_VAR("std::vector<NFieldType*>*", "container_chain");
                                    META("%container_chain = $t_index->target_field->fieldType->resolve_container_chain();");
                                    META("%container_chain->pop_back();");
                                    MAP_LIST("t_container_chain", "%container_chain");

                                    META_VAR("std::string", "field_name");
                                    META("%field_name = \"accessor.\" + *$t_index->target;");
                                    MAP_VAR("t_field_name", "%field_name");

                                    MAP_VAR("t_element", "std::string(\"element_\") + GetString(GET_ITERATOR_VALUE())");
                                    META_VAR("int", "final_element", "-1");
                                    FOREACH(); USE_LIST("t_container_chain");
                                    foreach (var t_element in t_field_name)
                                    /*META("%field_name = $t_element; ++%final_element;")*/
                                    /*END*/
                                    {
                                        MAP_VAR("t_element", "std::string(\"element_\") + GetString(%final_element)");
                                        new_index.AddItem((string)t_element, accessor.CellId);
                                    }

                                    META("delete %container_chain;");
                                }
                                END();
                            }
                            new_index.BuildIndex();
                            new_searcher = new InvertedBigramSearcher(true, "t_index_name");

                            ++t_index_name_substring_index_version;
                        }
                        /*  Update the index objects now.  */
                        lock (s_IndexLock)
                        {
                            t_index_name_substring_index    = new_index;
                            t_index_name_substring_searcher = new_searcher;
                        }

                        sw.Stop();
                        Log.WriteLine(LogLevel.Info, "Index: Finished updating the substring index of t_index_name. Time = {0}ms.", sw.ElapsedMilliseconds);
                        break;
                    }
                /*END*/
                /*END*/
                default:
                    throw new Exception("The target field does not have a substring index.");
            }
        }
        #endregion

        #region Index identifiers

        [MAP_LIST("t_cell", "TSLIndexIdentifierCellVector")]
        [MAP_VAR("t_cell", "")]
        [MAP_VAR("t_cell_name", "name")]
        [MAP_LIST("t_struct", "TSLIndexIdentifierStructVector")]
        [MAP_VAR("t_struct", "")]
        [MAP_VAR("t_struct_name", "name")]
        [MAP_VAR("t_struct_2_name", "fieldType->referencedNStruct->name")]
        [MAP_VAR("t_field_name", "name", MemberOf = "t_struct_2")]
        [MAP_VAR("t_target_name", "name", MemberOf = "t_target")]
        [MAP_VAR("t_struct_2", "")]
        [MAP_VAR("t_target", "")]
        [MAP_VAR("t_target_id", "(*TSLIndexFieldIdMap)[$$]")]
        [FOREACH]
        /// <summary>
        /// The index identifier representing <see cref="t_Namespace.t_cell_name"/>.
        /// </summary>
        public class t_cell_name : IndexIdentifier
        {
            [MAP_LIST("t_struct_2", "(*TSLIndexIdentifierSubstructureMap)[$t_cell]")]
            [IF("TSLIndexIdentifierSubstructureMap->count($t_cell)")]
            [FOREACH]
            [USE_LIST("t_struct_2")]
#pragma warning disable
            /// <summary>
            /// The index identifier representing <see cref="t_Namespace.t_cell_name.t_field_name"/>.
            /// </summary>
            public static readonly t_struct_2_name t_field_name = new t_struct_2_name();
#pragma warning enable
            [END]
            [END]
            [MAP_LIST("t_target", "(*TSLIndexIdentifierTargetMap)[$t_cell]")]
            [IF("TSLIndexIdentifierTargetMap->count($t_cell)")]
            [FOREACH]
            [USE_LIST("t_target")]
            /// <summary>
            /// The index identifier representing <see cref="t_Namespace.t_cell_name.t_target_name"/>.
            /// </summary>
            public static readonly IndexIdentifier t_target_name = t_target_id;
            /*END()*/
            /*END()*/
        }
        [END]

        [FOREACH]
        /// <summary>
        /// The index identifier representing <see cref="t_Namespace.t_struct_name"/>.
        /// </summary>
        public class t_struct_name : IndexIdentifier
        {
            [MAP_LIST("t_struct_2", "(*TSLIndexIdentifierSubstructureMap)[$t_struct]")]
            [IF("TSLIndexIdentifierSubstructureMap->count($t_struct)")]
            [FOREACH]
            [USE_LIST("t_struct_2")]
            /// <summary>
            /// The index identifier representing <see cref="t_Namespace.t_struct_name.t_field_name"/>
            /// </summary>
            public t_struct_2_name t_field_name = new t_struct_2_name();
            /**/
            /**/
            [MAP_LIST("t_target", "(*TSLIndexIdentifierTargetMap)[$t_struct]")]
            [IF("TSLIndexIdentifierTargetMap->count($t_struct)")]
            [FOREACH]
            [USE_LIST("t_target")]
            /// <summary>
            /// The index identifier representing <see cref="t_Namespace.t_struct_name.t_target_name"/>.
            /// </summary>
            public IndexIdentifier t_target_name = t_target_id;
            /**/
            /**/
        }
        [END]

        [MUTE]
        public class t_struct_2_name : IndexIdentifier
        {

        }

        private const uint t_index_id = 0;
        private const uint t_target_id = 0;
#pragma warning disable
        private static IndexIdentifier t_field_name;
#pragma warning enable
        [MUTE_END]

        /// <summary>
        /// The base class of index identifiers. When performing index queries, an index identifier should be provided
        /// to the query interface. All the indexed fields defined in the TSL have a corresponding static index identifier
        /// instance, accessible through t_Namespace.Index.Target_Cell_Name.Target_Field_Name.
        /// </summary>
        public class IndexIdentifier /*MUTE*/ : t_field_type /*MUTE_END*/
        {
            /// <summary>
            /// For internal use only.
            /// </summary>
            /// <param name="value">An 32-bit unsigned integer that is assigned to the target index identifier. </param>
            /// <returns></returns>
            public static implicit operator IndexIdentifier(uint value)
            {
                return new IndexIdentifier(value);
            }
            protected IndexIdentifier(uint value)
            {
                this.id = value;
            }
            protected IndexIdentifier()
            {
                this.id = uint.MaxValue;
            }
            internal uint id;
        }
        #endregion
    }

    #region Index extension methods
    [MAP_LIST("t_substring_indexed_list_type", "TSLSubstringIndexedListTypes")]
    [MAP_VAR("t_substring_indexed_list_type", "")]
    [MAP_VAR("t_data_type", "", MemberOf = "t_substring_indexed_list_type")]
    [MAP_VAR("t_field_type", "data_type_get_accessor_name($$.get())", MemberOf = "t_substring_indexed_list_type")]
    /// <summary>
    /// Provides interfaces to be translated to index queries in Linq expressions.
    /// </summary>
    public static class SubstringQueryExtension
    {
        [IF("contains_substring_index")]
        public static bool Contains(this string @string, IEnumerable<string> substrings)
        {
            if (substrings == null || substrings.Count() == 0)
                return true;
            if (@string == null)
                throw new ArgumentNullException("string");

            int idx = 0;
            foreach (var substr in substrings)
            {
                if (substr == null)
                    continue;
                idx = @string.IndexOf(substr, idx, StringComparison.Ordinal);
                if (idx == -1)
                {
                    return false;
                }
            }
            return true;
        }
        public static bool Contains(this string @string, params string[] substrings)
        {
            return Contains(@string, substrings as IEnumerable<string>);
        }

        public static bool Contains(this StringAccessor @string, IEnumerable<string> substrings)
        {
            if (substrings == null || substrings.Count() == 0)
                return true;
            if (@string == (object)null)
                throw new ArgumentNullException("string");

            return Contains((string)@string, substrings);
        }
        public static bool Contains(this StringAccessor @string, params string[] substrings)
        {
            return Contains((string)@string, substrings as IEnumerable<string>);
        }
        [END]

        [FOREACH]
        [MAP_VAR("t_list", "\"list\"")]
        public static bool ElementContainsSubstring(this t_data_type t_list, IEnumerable<string> substrings)
        {
            if (substrings == null || substrings.Count() == 0)
                return true;
            if (t_list == null)
                throw new ArgumentNullException("list");

            __meta.META_VAR("std::vector<NFieldType*>*", "container_chain", "$t_substring_indexed_list_type->resolve_container_chain()");
            __meta.META_VAR("std::string", "list_name", "\"list\"");
            __meta.META_VAR("int", "iter_value", "0");
            __meta.META("%container_chain->pop_back();");
            __meta.META("%container_chain->pop_back();");
            __meta.MAP_LIST("t_chain", "%container_chain");
            __meta.MAP_VAR("t_list", "%list_name");
            __meta.MAP_VAR("t_element", "\"sublist_\" + GetString(%iter_value)");
            __meta.FOREACH();
            __meta.USE_LIST("t_chain");
            foreach (var t_element in t_list)
            {
                if (t_element == null)
                    continue;
                __meta.META("%list_name = std::string(\"sublist_\") + GetString(%iter_value);");
                __meta.META("++%iter_value;");
                __meta.END();
                foreach (string str in t_list)
                {
                    if (str.Contains(substrings))
                        return true;
                }
                __meta.FOREACH();
                __meta.USE_LIST("t_chain");
            }
            __meta.END();
            return false;
        }
        public static bool ElementContainsSubstring(this t_data_type list, params string[] substrings)
        {
            return ElementContainsSubstring(list, substrings as IEnumerable<string>);
        }
        /*END*/

        [FOREACH]
        [MAP_VAR("t_list", "\"list\"")]
        public static bool ElementContainsSubstring(this t_field_type t_list, IEnumerable<string> substrings)
        {
            if (substrings == null || substrings.Count() == 0)
                return true;
            if (t_list == null)
                throw new ArgumentNullException("list");

            __meta.META_VAR("std::vector<NFieldType*>*", "container_chain", "$t_substring_indexed_list_type->resolve_container_chain()");
            __meta.META_VAR("std::string", "list_name", "\"list\"");
            __meta.META_VAR("int", "iter_value", "0");
            __meta.META("%container_chain->pop_back();");
            __meta.META("%container_chain->pop_back();");
            __meta.MAP_LIST("t_chain", "%container_chain");
            __meta.MAP_VAR("t_list", "%list_name");
            __meta.MAP_VAR("t_element", "\"sublist_\" + GetString(%iter_value)");
            __meta.FOREACH();
            __meta.USE_LIST("t_chain");
            foreach (var t_element in t_list)
            {
                if (t_element == null)
                    continue;
                __meta.META("%list_name = std::string(\"sublist_\") + GetString(%iter_value);");
                __meta.META("++%iter_value;");
                __meta.END();
                foreach (string str in t_list)
                {
                    if (str.Contains(substrings))
                        return true;
                }
                __meta.FOREACH();
                __meta.USE_LIST("t_chain");
            }
            __meta.END();
            return false;
        }
        public static bool ElementContainsSubstring(this t_field_type list, params string[] substrings)
        {
            return ElementContainsSubstring(list, substrings as IEnumerable<string>);
        }
        /*END*/
    }
    #endregion
}
