#include "common.h"
#include <string>
#include <SyntaxNode.h>

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
Index(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(
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
)::");
if (Trinity::Codegen::contains_substring_index)
{
source->append(R"::(
using )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.InvertedIndex;
)::");
}
source->append(R"::(
using )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.Linq;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    /// <summary>
    /// Provides indexing capabilities on <see cref="Trinity.Storage.LocalMemoryStorage"/>.
    /// The target field to query on is specified with <paramref name="query"/>, <seealso cref=")::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.IndexIdentifier"/>.
    /// </summary>
    public class Index
    {
        static Index()
        {
            BackgroundIndexUpdater();
            BackgroundThread.AddBackgroundTask(new BackgroundTask(BackgroundIndexUpdater, s_BackgroundIndexUpdateInterval));
        }
        static          readonly    int                         s_BackgroundIndexUpdateInterval      = 60*1000*10;
        static          readonly    object                      s_IndexLock                          = new object();
        static internal readonly    Dictionary<string,string>   s_AccessorSubstringIndexAccessMethod = new Dictionary<string, string>
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexTargetVector)->size();++iterator_1)
{
source->append(R"::(
            { ")::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '.' + *(*(TSLIndexTargetVector))[iterator_1]->target));
source->append(R"::(", ")::");
source->append(Codegen::GetString((*(TSLIndexTargetVector))[iterator_1]->target_field->fieldType->is_string() ? "Contains":"ElementContainsSubstring"));
source->append(R"::("}
            )::");
if (iterator_1 < (TSLIndexTargetVector)->size() - 1)
source->append(",");
}
source->append(R"::(
        };
        static internal readonly    Dictionary<string,SubstringQueryDelegate>
                                                                s_AccessorSubstringQueryMethodTable = 
            new Dictionary<string, SubstringQueryDelegate>
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexTargetVector)->size();++iterator_1)
{
source->append(R"::(
            { ")::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '.' + *(*(TSLIndexTargetVector))[iterator_1]->target));
source->append(R"::(", )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_SubstringQuery }
            )::");
if (iterator_1 < (TSLIndexTargetVector)->size() - 1)
source->append(",");
}
source->append(R"::(
        };
        static internal readonly    Dictionary<string,SubstringWildcardQueryDelegate>
                                                                s_AccessorSubstringWildcardQueryMethodTable = 
            new Dictionary<string, SubstringWildcardQueryDelegate>
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexTargetVector)->size();++iterator_1)
{
source->append(R"::(
            { ")::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '.' + *(*(TSLIndexTargetVector))[iterator_1]->target));
source->append(R"::(", )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_SubstringQuery }
            )::");
if (iterator_1 < (TSLIndexTargetVector)->size() - 1)
source->append(",");
}
source->append(R"::(
        };
        static internal readonly    Dictionary<string,string>
                                                                s_CellSubstringIndexAccessMethod = 
            new Dictionary<string, string>
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexTargetVector)->size();++iterator_1)
{
source->append(R"::(
            { ")::");
source->append(Codegen::GetString(get__index_access_path_for_cell((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(", ")::");
source->append(Codegen::GetString((*(TSLIndexTargetVector))[iterator_1]->target_field->fieldType->is_string() ? "Contains":"ElementContainsSubstring"));
source->append(R"::("}
            )::");
if (iterator_1 < (TSLIndexTargetVector)->size() - 1)
source->append(",");
}
source->append(R"::(
        };
        static internal readonly    Dictionary<string,SubstringQueryDelegate>
                                                                s_CellSubstringQueryMethodTable = 
            new Dictionary<string, SubstringQueryDelegate>
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexTargetVector)->size();++iterator_1)
{
source->append(R"::(
            { ")::");
source->append(Codegen::GetString(get__index_access_path_for_cell((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(", )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_SubstringQuery }
            )::");
if (iterator_1 < (TSLIndexTargetVector)->size() - 1)
source->append(",");
}
source->append(R"::(
        };
        static internal readonly    Dictionary<string,SubstringWildcardQueryDelegate>
                                                                s_CellSubstringWildcardQueryMethodTable = 
            new Dictionary<string, SubstringWildcardQueryDelegate>
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexTargetVector)->size();++iterator_1)
{
source->append(R"::(
            { ")::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '.' + *(*(TSLIndexTargetVector))[iterator_1]->target));
source->append(R"::(", )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_SubstringQuery }
            )::");
if (iterator_1 < (TSLIndexTargetVector)->size() - 1)
source->append(",");
}
source->append(R"::(
        };
        )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexTargetVector)->size();++iterator_1)
{
source->append(R"::(
        /// <summary>
        /// Performs a substring search.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        public static List<long> )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_SubstringQuery(string value)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::( is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_searcher.SubstringSearch(value);
        }
        /// <summary>
        /// Performs a substring search for a list of keywords. The match pattern is:
        /// keywords[0]*keywords[1]..., where * is the wildcard symbol.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        public static List<long> )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_SubstringQuery(List<string> values)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::( is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_searcher.SubstringSearch(values);
        }
        /// <summary>
        /// Performs a substring search using the specified keywords. The match pattern is:
        /// keywords[0]*keywords[1]..., where * is the wildcard symbol.
        /// </summary>
        /// <param name="keywords">A list of keywords.</param>
        /// <returns>A list of matched cell Ids.</returns>
        public static List<long> )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_SubstringQuery(params string[] values)
        {
            InvertedBigramSearcher searcher;
            lock (s_IndexLock)
            {
                searcher = )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_searcher;
            }
            if (searcher == null)
            {
                Log.WriteLine(LogLevel.Warning, "Index: Substring index for )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::( is not yet ready. Returning an empty query result set.");
                return new List<long>();
            }
            return )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_searcher.SubstringSearch(values);
        }
        )::");
}
source->append(R"::(
        static int BackgroundIndexUpdater()
        {
            )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexTargetVector)->size();++iterator_1)
{
if ((*(TSLIndexTargetVector))[iterator_1]->type == IT_SUBSTRING)
{
source->append(R"::(
            {
                UpdateSubstringQueryIndex()::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '.' + *(*(TSLIndexTargetVector))[iterator_1]->target));
source->append(R"::();
            }
            )::");
}
}
source->append(R"::(
            return s_BackgroundIndexUpdateInterval;
        }
        #region Fields
        )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexTargetVector)->size();++iterator_1)
{
if ((*(TSLIndexTargetVector))[iterator_1]->type == IT_SUBSTRING)
{
source->append(R"::(
        internal static object                  )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_index_lock = new object();
        internal static InvertedBigramIndexer   )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_index;
        internal static InvertedBigramSearcher  )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_searcher;
        internal static ulong                   )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_index_version = 0;
        )::");
}
}
source->append(R"::(
        #endregion
        #region Index interfaces
        internal static List<long> SubstringQuery(string index_id, string query)
        {
            SubstringQueryDelegate query_method;
            if (!s_AccessorSubstringQueryMethodTable.TryGetValue(index_id, out query_method))
                throw new Exception("Unrecognized index id.");
            return query_method(query);
        }
        
        /// <summary>
        /// Performs a substring query on <see cref="Trinity.Global.LocalStorage"/>.
        /// The target field to query on is specified with <paramref name="query"/>, <seealso cref=")::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.IndexIdentifier"/>.
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
                )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexTargetVector)->size();++iterator_1)
{
if ((*(TSLIndexTargetVector))[iterator_1]->type == IT_SUBSTRING)
{
source->append(R"::(
                case )::");
source->append(Codegen::GetString((*TSLIndexIdMap)[(*(TSLIndexTargetVector))[iterator_1]]));
source->append(R"::(:
                    {
                        return )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_SubstringQuery(query);
                    }
                )::");
}
}
source->append(R"::(
                default:
                    throw new Exception("The target field does not have a substring index.");
            }
        }
        /// <summary>
        /// Performs a substring sequence query on <see cref="Trinity.Global.LocalStorage"/>. The semantics of
        /// this type of query is that, given a sequence of query strings <c>q_1, q_2,...,q_n</c> in 
        /// <paramref name="query"/>, the match condition on a target string is that all the strings in the sequence
        /// are the substrings of the target string, and the order of the substring occurances should correspond
        /// strictly to that given by <paramref name="query"/>. For example (let <c>S</c> denote the target string):
        /// <example>
        /// <list type="bullet">
        /// <item>
        /// <c>S=..q1....q2..q3{and so on}...qn...</c> is a match.
        /// </item>
        /// <item>
        /// <c>S=.....q1q2..q4{and there are missing substrings in the sequence}...qn...</c> is not m)::");
source->append(R"::(atch.
        /// </item>
        /// <item>
        /// <c>S=..q5..q3.q1{all the substrings in the sequence are present, but never in the corresponding order}...qn...</c> is not match.
        /// </item>
        /// </list>
        /// </example>
        /// The target field to query on is specified with <paramref name="query"/>, <seealso cref=")::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.IndexIdentifier"/>.
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
                )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexTargetVector)->size();++iterator_1)
{
if ((*(TSLIndexTargetVector))[iterator_1]->type == IT_SUBSTRING)
{
source->append(R"::(
                case )::");
source->append(Codegen::GetString((*TSLIndexIdMap)[(*(TSLIndexTargetVector))[iterator_1]]));
source->append(R"::(:
                    {
                        return )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_SubstringQuery(query);
                    }
                )::");
}
}
source->append(R"::(
                default:
                    throw new Exception("The target field does not have a substring index.");
            }
        }
        /// <summary>
        /// Performs a substring sequence query on <see cref="Trinity.Global.LocalStorage"/>. The semantics of
        /// this type of query is that, given a sequence of query strings <c>q_1, q_2,...,q_n</c> in 
        /// <paramref name="query"/>, the match condition on a target string is that all the strings in the sequence
        /// are the substrings of the target string, and the order of the substring occurances should correspond
        /// strictly to that given by <paramref name="query"/>. For example (let <c>S</c> denote the target string):
        /// <example>
        /// <list type="bullet">
        /// <item>
        /// <c>S=..q1....q2..q3{and so on}...qn...</c> is a match.
        /// </item>
        /// <item>
        /// <c>S=.....q1q2..q4{and there are missing substrings in the sequence}...qn...</c> is not m)::");
source->append(R"::(atch.
        /// </item>
        /// <item>
        /// <c>S=..q5..q3.q1{all the substrings in the sequence are present, but never in the corresponding order}...qn...</c> is not match.
        /// </item>
        /// </list>
        /// </example>
        /// The target field to query on is specified with <paramref name="query"/>, <seealso cref=")::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.IndexIdentifier"/>.
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
                )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexTargetVector)->size();++iterator_1)
{
if ((*(TSLIndexTargetVector))[iterator_1]->type == IT_SUBSTRING)
{
source->append(R"::(
                case )::");
source->append(Codegen::GetString((*TSLIndexIdMap)[(*(TSLIndexTargetVector))[iterator_1]]));
source->append(R"::(:
                    {
                        return )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_SubstringQuery(query);
                    }
                )::");
}
}
source->append(R"::(
                default:
                    throw new Exception("The target field does not have a substring index.");
            }
        }
        
        /// <summary>
        /// Updates the index on the given field.
        /// </summary>
        /// <param name="index_id">The identifier of the field whose index should be rebuilt.</param>
        public static void UpdateSubstringQueryIndex(IndexIdentifier index_id)
        {
            switch (index_id.id)
            {
                )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexTargetVector)->size();++iterator_1)
{
if ((*(TSLIndexTargetVector))[iterator_1]->type == IT_SUBSTRING)
{
source->append(R"::(
                case )::");
source->append(Codegen::GetString((*TSLIndexIdMap)[(*(TSLIndexTargetVector))[iterator_1]]));
source->append(R"::(:
                    {
                        Log.WriteLine(LogLevel.Info, "Index: updating substring index of )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(");
                        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
                        InvertedBigramIndexer  new_index;
                        InvertedBigramSearcher new_searcher;
                        ulong                  old_version = )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_index_version;
                        /**
                         *  Lock the original index to prevent multiple updates
                         *  to be executed simultaneously.
                         */
                        lock ()::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_index_lock)
                        {
                            /*
                             * If the index version has changed, it means that someone else
                             * has updated the index while we're blocked on the old index object.
                             */
                            if ()::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_index_version != old_version)
                            {
                                Log.WriteLine(LogLevel.Info, "Index: Substring index of )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::( is already updated, stopping current action.");
                                break;
                            }
                            new_index = new InvertedBigramIndexer(")::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(");
                            foreach (var accessor in Global.LocalStorage.)::");
source->append(Codegen::GetString((*(TSLIndexTargetVector))[iterator_1]->cell->name));
source->append(R"::(_Accessor_Selector())
                            {
                                bool optional_not_exist = true;
                                )::");
auto access_chain_1 = (*(TSLIndexTargetVector))[iterator_1]->resolve_target();
std::string field_name_1;
bool has_optional_1 = false;
for(auto *field : *access_chain_1){
if (field->is_optional())
{
has_optional_1 = true;
source->append(R"::(
                                if (accessor.)::");
source->append(Codegen::GetString(field_name_1 + "Contains_" + *field->name));
source->append(R"::()
                                    )::");
}
field_name_1.append(*field->name).append(".");}
if (has_optional_1)
{
source->append(R"::(
                                {
                                    optional_not_exist = false;
                                }
                                if (optional_not_exist)
                                    break;
                                )::");
}
if ((*(TSLIndexTargetVector))[iterator_1]->target_field->fieldType->is_string())
{
source->append(R"::(
                                {
                                    new_index.AddItem(accessor.)::");
source->append(Codegen::GetString((*(TSLIndexTargetVector))[iterator_1]->target));
source->append(R"::(, accessor.CellID.Value);
                                }
                                )::");
}
else
{
source->append(R"::(
                                {
                                    )::");
std::vector<NFieldType*>* container_chain_1;
container_chain_1 = (*(TSLIndexTargetVector))[iterator_1]->target_field->fieldType->resolve_container_chain();
container_chain_1->pop_back();
std::string field_name_2;
field_name_2 = "accessor." + *(*(TSLIndexTargetVector))[iterator_1]->target;
int final_element_1 = -1;
for (size_t iterator_2 = 0; iterator_2 < (container_chain_1)->size();++iterator_2)
{
source->append(R"::(
                                    foreach (var )::");
source->append(Codegen::GetString(std::string("element_") + GetString(iterator_2)));
source->append(R"::( in )::");
source->append(Codegen::GetString(field_name_2));
source->append(R"::()
                                    )::");
field_name_2 = std::string("element_") + GetString(iterator_2); ++final_element_1;
}
source->append(R"::(
                                    {
                                        new_index.AddItem((string))::");
source->append(Codegen::GetString(std::string("element_") + GetString(final_element_1)));
source->append(R"::(, accessor.CellID.Value);
                                    }
                                    )::");
delete container_chain_1;
source->append(R"::(
                                }
                                )::");
}
source->append(R"::(
                            }
                            new_index.BuildIndex();
                            new_searcher = new InvertedBigramSearcher(true, ")::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(");
                            ++)::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_index_version;
                        }
                        /*  Update the index objects now.  */
                        lock (s_IndexLock)
                        {
                            )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_index    = new_index;
                            )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(_substring_searcher = new_searcher;
                        }
                        sw.Stop();
                        Log.WriteLine(LogLevel.Info, "Index: Finished updating the substring index of )::");
source->append(Codegen::GetString(*(*(TSLIndexTargetVector))[iterator_1]->cell->name + '_' + get__index_member_name((*(TSLIndexTargetVector))[iterator_1])));
source->append(R"::(. Time = {0}ms.", sw.ElapsedMilliseconds);
                        break;
                    }
                )::");
}
}
source->append(R"::(
                default:
                    throw new Exception("The target field does not have a substring index.");
            }
        }
        #endregion
        #region Index identifiers
        )::");
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexIdentifierCellVector)->size();++iterator_1)
{
source->append(R"::(
        /// <summary>
        /// The index identifier representing <see cref=")::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.)::");
source->append(Codegen::GetString((*(TSLIndexIdentifierCellVector))[iterator_1]->name));
source->append(R"::("/>.
        /// </summary>
        public class )::");
source->append(Codegen::GetString((*(TSLIndexIdentifierCellVector))[iterator_1]->name));
source->append(R"::( : IndexIdentifier
        {
            )::");
if (TSLIndexIdentifierSubstructureMap->count((*(TSLIndexIdentifierCellVector))[iterator_1]))
{
for (size_t iterator_2 = 0; iterator_2 < ((*TSLIndexIdentifierSubstructureMap)[(*(TSLIndexIdentifierCellVector))[iterator_1]])->size();++iterator_2)
{
source->append(R"::(
            /// <summary>
            /// The index identifier representing <see cref=")::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.)::");
source->append(Codegen::GetString((*(TSLIndexIdentifierCellVector))[iterator_1]->name));
source->append(R"::(.)::");
source->append(Codegen::GetString((*((*TSLIndexIdentifierSubstructureMap)[(*(TSLIndexIdentifierCellVector))[iterator_1]]))[iterator_2]->name));
source->append(R"::("/>.
            /// </summary>
            public static readonly )::");
source->append(Codegen::GetString((*((*TSLIndexIdentifierSubstructureMap)[(*(TSLIndexIdentifierCellVector))[iterator_1]]))[iterator_2]->fieldType->referencedNStruct->name));
source->append(R"::(/*_*/)::");
source->append(Codegen::GetString((*((*TSLIndexIdentifierSubstructureMap)[(*(TSLIndexIdentifierCellVector))[iterator_1]]))[iterator_2]->name));
source->append(R"::( = new )::");
source->append(Codegen::GetString((*((*TSLIndexIdentifierSubstructureMap)[(*(TSLIndexIdentifierCellVector))[iterator_1]]))[iterator_2]->fieldType->referencedNStruct->name));
source->append(R"::(();
            )::");
}
}
if (TSLIndexIdentifierTargetMap->count((*(TSLIndexIdentifierCellVector))[iterator_1]))
{
for (size_t iterator_2 = 0; iterator_2 < ((*TSLIndexIdentifierTargetMap)[(*(TSLIndexIdentifierCellVector))[iterator_1]])->size();++iterator_2)
{
source->append(R"::(
            /// <summary>
            /// The index identifier representing <see cref=")::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.)::");
source->append(Codegen::GetString((*(TSLIndexIdentifierCellVector))[iterator_1]->name));
source->append(R"::(.)::");
source->append(Codegen::GetString((*((*TSLIndexIdentifierTargetMap)[(*(TSLIndexIdentifierCellVector))[iterator_1]]))[iterator_2]->name));
source->append(R"::("/>.
            /// </summary>
            public static readonly IndexIdentifier )::");
source->append(Codegen::GetString((*((*TSLIndexIdentifierTargetMap)[(*(TSLIndexIdentifierCellVector))[iterator_1]]))[iterator_2]->name));
source->append(R"::( = )::");
source->append(Codegen::GetString((*TSLIndexFieldIdMap)[(*((*TSLIndexIdentifierTargetMap)[(*(TSLIndexIdentifierCellVector))[iterator_1]]))[iterator_2]]));
source->append(R"::(;
            )::");
}
}
source->append(R"::(
        }
        )::");
}
for (size_t iterator_1 = 0; iterator_1 < (TSLIndexIdentifierStructVector)->size();++iterator_1)
{
source->append(R"::(
        /// <summary>
        /// The index identifier representing <see cref=")::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.)::");
source->append(Codegen::GetString((*(TSLIndexIdentifierStructVector))[iterator_1]->name));
source->append(R"::("/>.
        /// </summary>
        public class )::");
source->append(Codegen::GetString((*(TSLIndexIdentifierStructVector))[iterator_1]->name));
source->append(R"::( : IndexIdentifier
        {
            )::");
if (TSLIndexIdentifierSubstructureMap->count((*(TSLIndexIdentifierStructVector))[iterator_1]))
{
for (size_t iterator_2 = 0; iterator_2 < ((*TSLIndexIdentifierSubstructureMap)[(*(TSLIndexIdentifierStructVector))[iterator_1]])->size();++iterator_2)
{
source->append(R"::(
            /// <summary>
            /// The index identifier representing <see cref=")::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.)::");
source->append(Codegen::GetString((*(TSLIndexIdentifierStructVector))[iterator_1]->name));
source->append(R"::(.)::");
source->append(Codegen::GetString((*((*TSLIndexIdentifierSubstructureMap)[(*(TSLIndexIdentifierStructVector))[iterator_1]]))[iterator_2]->name));
source->append(R"::("/>
            /// </summary>
            public )::");
source->append(Codegen::GetString((*((*TSLIndexIdentifierSubstructureMap)[(*(TSLIndexIdentifierStructVector))[iterator_1]]))[iterator_2]->fieldType->referencedNStruct->name));
source->append(R"::(/*_*/)::");
source->append(Codegen::GetString((*((*TSLIndexIdentifierSubstructureMap)[(*(TSLIndexIdentifierStructVector))[iterator_1]]))[iterator_2]->name));
source->append(R"::( = new )::");
source->append(Codegen::GetString((*((*TSLIndexIdentifierSubstructureMap)[(*(TSLIndexIdentifierStructVector))[iterator_1]]))[iterator_2]->fieldType->referencedNStruct->name));
source->append(R"::(();
            )::");
}
}
if (TSLIndexIdentifierTargetMap->count((*(TSLIndexIdentifierStructVector))[iterator_1]))
{
for (size_t iterator_2 = 0; iterator_2 < ((*TSLIndexIdentifierTargetMap)[(*(TSLIndexIdentifierStructVector))[iterator_1]])->size();++iterator_2)
{
source->append(R"::(
            /// <summary>
            /// The index identifier representing <see cref=")::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.)::");
source->append(Codegen::GetString((*(TSLIndexIdentifierStructVector))[iterator_1]->name));
source->append(R"::(.)::");
source->append(Codegen::GetString((*((*TSLIndexIdentifierTargetMap)[(*(TSLIndexIdentifierStructVector))[iterator_1]]))[iterator_2]->name));
source->append(R"::("/>.
            /// </summary>
            public IndexIdentifier )::");
source->append(Codegen::GetString((*((*TSLIndexIdentifierTargetMap)[(*(TSLIndexIdentifierStructVector))[iterator_1]]))[iterator_2]->name));
source->append(R"::( = )::");
source->append(Codegen::GetString((*TSLIndexFieldIdMap)[(*((*TSLIndexIdentifierTargetMap)[(*(TSLIndexIdentifierStructVector))[iterator_1]]))[iterator_2]]));
source->append(R"::(;
            )::");
}
}
source->append(R"::(
        }
        )::");
}
source->append(R"::(
        /// <summary>
        /// The base class of index identifiers. When performing index queries, an index identifier should be provided
        /// to the query interface. All the indexed fields defined in the TSL have a corresponding static index identifier
        /// instance, accessible through )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.Index.Target_Cell_Name.Target_Field_Name.
        /// </summary>
        public class IndexIdentifier 
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
    
    /// <summary>
    /// Provides interfaces to be translated to index queires in Linq expressions.
    /// </summary>
    public static class SubstringQ)::");
source->append(R"::(ueryExtension
    {
        )::");
if (contains_substring_index)
{
source->append(R"::(
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
            if (substrings == null || substrings.)::");
source->append(R"::(Count() == 0)
                return true;
            if (@string == (object)null)
                throw new ArgumentNullException("string");
            return Contains((string)@string, substrings);
        }
        public static bool Contains(this StringAccessor @string, params string[] substrings)
        {
            return Contains((string)@string, substrings as IEnumerable<string>);
        }
        )::");
}
for (size_t iterator_1 = 0; iterator_1 < (TSLSubstringIndexedListTypes)->size();++iterator_1)
{
source->append(R"::(
        public static bool ElementContainsSubstring(this )::");
source->append(Codegen::GetString((*(TSLSubstringIndexedListTypes))[iterator_1]));
source->append(Codegen::GetString("list"));
source->append(R"::(, IEnumerable<string> substrings)
        {
            if (substrings == null || substrings.Count() == 0)
                return true;
            if ()::");
source->append(Codegen::GetString("list"));
source->append(R"::( == null)
                throw new ArgumentNullException("list");
            )::");
std::vector<NFieldType*>* container_chain_2 = (*(TSLSubstringIndexedListTypes))[iterator_1]->resolve_container_chain();
std::string list_name_1 = "list";
int iter_value_1 = 0;
container_chain_2->pop_back();
container_chain_2->pop_back();
for (size_t iterator_2 = 0; iterator_2 < (container_chain_2)->size();++iterator_2)
{
source->append(R"::(
            foreach (var )::");
source->append(Codegen::GetString("sublist_" + GetString(iter_value_1)));
source->append(R"::( in )::");
source->append(Codegen::GetString(list_name_1));
source->append(R"::()
            {
                if ()::");
source->append(Codegen::GetString("sublist_" + GetString(iter_value_1)));
source->append(R"::( == null)
                    continue;
                )::");
list_name_1 = std::string("sublist_") + GetString(iter_value_1);
++iter_value_1;
}
source->append(R"::(
                foreach (string str in )::");
source->append(Codegen::GetString(list_name_1));
source->append(R"::()
                {
                    if (str.Contains(substrings))
                        return true;
                }
                )::");
for (size_t iterator_2 = 0; iterator_2 < (container_chain_2)->size();++iterator_2)
{
source->append(R"::(
            }
            )::");
}
source->append(R"::(
            return false;
        }
        public static bool ElementContainsSubstring(this )::");
source->append(Codegen::GetString((*(TSLSubstringIndexedListTypes))[iterator_1]));
source->append(R"::( list, params string[] substrings)
        {
            return ElementContainsSubstring(list, substrings as IEnumerable<string>);
        }
        )::");
}
for (size_t iterator_1 = 0; iterator_1 < (TSLSubstringIndexedListTypes)->size();++iterator_1)
{
source->append(R"::(
        public static bool ElementContainsSubstring(this )::");
source->append(Codegen::GetString(data_type_get_accessor_name((*(TSLSubstringIndexedListTypes))[iterator_1].get())));
source->append(R"::(/*_*/)::");
source->append(Codegen::GetString("list"));
source->append(R"::(, IEnumerable<string> substrings)
        {
            if (substrings == null || substrings.Count() == 0)
                return true;
            if ()::");
source->append(Codegen::GetString("list"));
source->append(R"::( == null)
                throw new ArgumentNullException("list");
            )::");
std::vector<NFieldType*>* container_chain_3 = (*(TSLSubstringIndexedListTypes))[iterator_1]->resolve_container_chain();
std::string list_name_2 = "list";
int iter_value_2 = 0;
container_chain_3->pop_back();
container_chain_3->pop_back();
for (size_t iterator_2 = 0; iterator_2 < (container_chain_3)->size();++iterator_2)
{
source->append(R"::(
            foreach (var )::");
source->append(Codegen::GetString("sublist_" + GetString(iter_value_2)));
source->append(R"::( in )::");
source->append(Codegen::GetString(list_name_2));
source->append(R"::()
            {
                if ()::");
source->append(Codegen::GetString("sublist_" + GetString(iter_value_2)));
source->append(R"::( == null)
                    continue;
                )::");
list_name_2 = std::string("sublist_") + GetString(iter_value_2);
++iter_value_2;
}
source->append(R"::(
                foreach (string str in )::");
source->append(Codegen::GetString(list_name_2));
source->append(R"::()
                {
                    if (str.Contains(substrings))
                        return true;
                }
                )::");
for (size_t iterator_2 = 0; iterator_2 < (container_chain_3)->size();++iterator_2)
{
source->append(R"::(
            }
            )::");
}
source->append(R"::(
            return false;
        }
        public static bool ElementContainsSubstring(this )::");
source->append(Codegen::GetString(data_type_get_accessor_name((*(TSLSubstringIndexedListTypes))[iterator_1].get())));
source->append(R"::(/*_*/list, params string[] substrings)
        {
            return ElementContainsSubstring(list, substrings as IEnumerable<string>);
        }
        )::");
}
source->append(R"::(
    }
    #endregion
}
)::");

            return source;
        }
    }
}
