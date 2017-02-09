// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace FanoutSearch
{
    internal class JsonDSL
    {
        //  Query object level keywords
        internal const string From      = "from";
        internal const string Size      = "size";
        internal const string Path      = "path";

        //  Constraint object level keywords
        internal const string Continue  = "continue";
        internal const string Return    = "return";
        internal const string Select    = "select";
        internal const string Match     = "match";

        //  Predicate object level keywords
        internal const string Id        = "id";
        internal const string Type      = "type";
        internal const string Has       = "has";

        //  Field level keywords
        internal const string Substring = "substring";
        internal const string Count     = "count";

        internal const string gt        = "gt";
        internal const string sym_gt    = ">";
        internal const string lt        = "lt";
        internal const string sym_lt    = "<";
        internal const string geq       = "geq";
        internal const string sym_geq   = ">=";
        internal const string leq       = "leq";
        internal const string sym_leq   = "<=";
        internal const string or        = "or";
        internal const string not       = "not";

        //  Special select operators
        internal const string graph_outlink_type = "graph_outlink_type";

        private static readonly char[]  s_url_path_separator            = "/".ToCharArray();

        private static bool TryParseLongList(string element_str, out List<long> cell_ids)
        {
            string[] ids_str = element_str.Split(',');
            cell_ids = new List<long>(ids_str.Length);

            foreach (var str in ids_str)
            {
                long id;
                if (long.TryParse(str, out id))
                {
                    cell_ids.Add(id);
                }
                else
                {
                    return false;
                }
            }

            return cell_ids.Count > 0;
        }

        internal static void ProcessJsonQueryObject(FanoutSearchDescriptor fs_desc, string queryPath, JObject queryObject)
        {
            string[] path_elements    = queryPath.Split(s_url_path_separator, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0, len = path_elements.Length; i != len; ++i)
            {
                var element_str = path_elements[i];

                if (0 == (i % 2)) /* Node */
                {
                    ProcessJsonTraverseAction(fs_desc, queryObject, i, len, element_str);
                }
                else /* Edge */
                {
                    ProcessJsonEdgeTypeDescriptor(fs_desc, element_str);
                }
            }

            if (0 == path_elements.Length % 2) /* There's no final node desc. Add a return command. */
            {
                fs_desc.AddTraverseStep(_ => Action.Return, null);
            }
        }

        private static void ProcessJsonEdgeTypeDescriptor(FanoutSearchDescriptor fs_desc, string edge_desc)
        {
            if (edge_desc == "*" || edge_desc == "")
            {
                fs_desc.m_edgeTypes.Add(new EdgeTypeDescriptor(fs_desc));
            }
            else
            {
                fs_desc.m_edgeTypes.Add(new EdgeTypeDescriptor(fs_desc, edge_desc.Split(',')));
            }
        }

        internal static void ProcessJsonTraverseAction(FanoutSearchDescriptor fs_desc, JObject queryObject, int i, int len, string element_str)
        {
            List<long> cell_ids = null;
            bool is_cell_id = TryParseLongList(element_str, out cell_ids);
            bool is_origin  = (i == 0);
            bool is_last = (i == len -1 || i == len - 2);
            List<string> selectedFields = new List<string>();

            if (is_cell_id)
            {
                if (is_origin)
                {
                    fs_desc.m_origin = cell_ids;
                }
                else
                {
                    if (is_last) /* The last node, action set to return */
                    {
                        fs_desc.AddTraverseStep(ExpressionBuilder.GenerateTraverseActionFromCellIds(Action.Return, cell_ids), null);
                    }
                    else /* Intermediate node, action set to continue */
                    {
                        fs_desc.AddTraverseStep(ExpressionBuilder.GenerateTraverseActionFromCellIds(Action.Continue, cell_ids), null);
                    }
                }
            }
            else
            {
                dynamic action_object = queryObject[element_str];
                ProcessJsonSelectField(selectedFields, action_object);

                if (is_origin)
                {
                    ProcessJsonOriginNodeObject(fs_desc, action_object);
                }
                else /* not origin */
                {
                    ProcessJsonNonOriginNodeObject(fs_desc, is_last, action_object);
                }
            }

            fs_desc.m_selectFields.Add(selectedFields);
        }

        internal static void ProcessJsonSelectField(List<string> selectedFields, dynamic action_object)
        {
            dynamic select = null;

            if (action_object != null && (select = action_object[JsonDSL.Select]) != null)
            {
                if (select is JArray)
                {
                    selectedFields.AddRange((select as JArray).Select(_ => (string)_));
                }
                else if (select is String)
                {
                    selectedFields.Add(select);
                }
                else if (select is JToken && select.Type == JTokenType.String)
                {
                    selectedFields.Add((string)select);
                }
                else
                {
                    throw new FanoutSearchQueryException("Invalid select operand");
                }

                action_object.Remove(JsonDSL.Select);
            }
        }

        private static void ProcessJsonNonOriginNodeObject(FanoutSearchDescriptor fs_desc, bool is_last, dynamic action_object)
        {
            Expression<Func<ICellAccessor, Action>> traverse_action = null;
            Action default_action = is_last ? Action.Return : Action.Continue;
            traverse_action = ExpressionBuilder.GenerateTraverseActionFromQueryObject(action_object, default_action);

            if (traverse_action != null)
            {
                fs_desc.AddTraverseStep(traverse_action, null);
            }
            else
            {
                /* either user didn't specify node desc, or node desc doesn't imply traverse action */
                if (is_last)
                {
                    fs_desc.AddTraverseStep(_ => Action.Return, null);
                }
                else
                {
                    fs_desc.AddTraverseStep(_ => Action.Continue, null);
                }
            }
        }

        internal static void ProcessJsonOriginNodeObject(FanoutSearchDescriptor fs_desc, dynamic action_object)
        {
            if (action_object == null)
            {
                throw new FanoutSearchQueryException("The starting node descriptor cannot be null");
            }
            JArray id_array   = action_object[JsonDSL.Id];
            string type_str   = action_object[JsonDSL.Type];
            JObject match_obj = action_object[JsonDSL.Match];

            if (id_array != null)
            {
                fs_desc.m_origin = id_array.Select(_ => (long)_).ToList();
            }
            else
            {
                /* construct origin query object */
                JObject query_object = new JObject();
                /* if match_obj not found, use the action object itself (remove query operators). */
                if (match_obj == null)
                {
                    match_obj = new JObject(action_object);
                    match_obj.Remove(JsonDSL.Id);
                    match_obj.Remove(JsonDSL.Type);
                    match_obj.Remove(JsonDSL.Match);
                    if (match_obj.Properties().Count() == 0)
                    {
                        throw new FanoutSearchQueryException("No match conditions for starting node");
                    }
                }

                query_object[JsonDSL.Match] = match_obj;
                query_object[JsonDSL.Type] = type_str;
                fs_desc.m_origin_query = query_object.ToString();
            }
        }
    }
}
