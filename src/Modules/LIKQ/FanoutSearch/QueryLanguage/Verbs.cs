// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trinity.Storage;

namespace FanoutSearch
{
    [Flags]
    [Serializable]
    public enum Action
    {
        Continue = ~1,
        Return = ~2,
    }

    public static class Verbs
    {
        public static Action continue_if(this ICell cell, bool pred)
        {
            return pred ? Action.Continue : ~(Action)0;
        }

        public static Action return_if(this ICell cell, bool pred)
        {
            return pred ? Action.Return : ~(Action)0;
        }

        public static bool has_cell_id(this ICell cell, params long[] cellIds)
        {
            return cellIds.Contains(cell.CellID);
        }

        public static bool type(this ICell cell, string type_name)
        {
            IFanoutSearchCellAccessor accessor = cell as IFanoutSearchCellAccessor;
            if (accessor == null)
                return cell.TypeName == type_name;
            return accessor.isOfType(type_name);

        }

        [ThreadStatic]
        private static Random s_rng = null;

        public static bool dice(this ICell cell, double p)
        {
            _ensure_random();
            return (s_rng.NextDouble() <= p);
        }

        private static void _ensure_random()
        {
            if (s_rng == null)
            {
                s_rng = new Random(Thread.CurrentThread.GetHashCode());
            }
        }

        public static int count(this ICell cell, string field)
        {
            try
            {
                if (!cell.ContainsField(field))
                    return 0;

                object field_obj = cell.GetField<object>(field);
                IEnumerable enumerable = field_obj as IEnumerable;

                if (enumerable != null)
                {
                    int cnt = 0;
                    var enumerator = enumerable.GetEnumerator();
                    while (enumerator.MoveNext())
                        ++cnt;

                    return cnt;
                }
                else
                {
                    return 1;
                }
            }
            catch { return 0; }
        }

        public static string get(this ICell cell, string field)
        {
            try
            {
                return cell.GetField<string>(field);
            }
            catch
            {
                return "";
            }
        }

        public static bool has(this ICell cell, string field)
        {
            return cell.ContainsField(field);
        }

        #region Numerical
        public static bool greater_than(this ICell cell, string field, double value)
        {
            if (!cell.ContainsField(field))
                return false;

            return cell.GetField<List<double>>(field).Any(_ => _ > value);
        }

        public static bool greater_than(this ICell cell, string field, int value)
        {
            if (!cell.ContainsField(field))
                return false;

            return cell.GetField<List<long>>(field).Any(_ => _ > value);
        }

        public static bool greater_than_or_equal(this ICell cell, string field, double value)
        {
            if (!cell.ContainsField(field))
                return false;

            return cell.GetField<List<double>>(field).Any(_ => _ >= value);
        }

        public static bool greater_than_or_equal(this ICell cell, string field, int value)
        {
            if (!cell.ContainsField(field))
                return false;

            return cell.GetField<List<long>>(field).Any(_ => _ >= value);
        }

        public static bool less_than(this ICell cell, string field, double value)
        {
            if (!cell.ContainsField(field))
                return false;

            return cell.GetField<List<double>>(field).Any(_ => _ < value);
        }

        public static bool less_than(this ICell cell, string field, int value)
        {
            if (!cell.ContainsField(field))
                return false;

            return cell.GetField<List<long>>(field).Any(_ => _ < value);
        }

        public static bool less_than_or_equal(this ICell cell, string field, double value)
        {
            if (!cell.ContainsField(field))
                return false;

            return cell.GetField<List<double>>(field).Any(_ => _ <= value);
        }

        public static bool less_than_or_equal(this ICell cell, string field, int value)
        {
            if (!cell.ContainsField(field))
                return false;

            return cell.GetField<List<long>>(field).Any(_ => _ <= value);
        }
        #endregion

        public static bool has(this ICell cell, string field, string value)
        {
            try
            {
                if (!cell.ContainsField(field))
                    return false;

                object field_obj = cell.GetField<object>(field);

                if(field_obj is string)
                {
                    return (field_obj as string == value);
                }

                IEnumerable enumerable = field_obj as IEnumerable;

                if (enumerable != null)
                {
                    foreach (object element in enumerable)
                    {
                        if (element.ToString() == value)
                            return true;
                    }

                    return false;
                }
                else
                {
                    return (field_obj.ToString() == value);
                }
            }
            catch { }
            return false;

        }

        [ThreadStatic]
        internal static unsafe long* m_path_ptr;
        [ThreadStatic]
        internal static unsafe int m_path_len;

        public static unsafe long get_id_in_path(this ICell cell, int index)
        {
            return m_path_ptr[index];
        }

        public static int get_path_length(this ICell cell)
        {
            return m_path_len;
        }

        public static TOut Let<TIn, TOut>(this ICellAccessor cell, TIn t, Func<TIn, TOut> closure)
        {
            return closure(t);
        }

        public static Action Switch<T>(this ICellAccessor cell, T val, Dictionary<T, Action> cases)
        {
            Action action = ~(Action)0;
            cases.TryGetValue(val, out action);
            return action;
        }

        public static Action Switch<T>(this ICellAccessor cell, T val, Dictionary<T, Func<Action>> cases)
        {
            Func<Action> func = null;
            if (cases.TryGetValue(val, out func))
                return func();
            else
                return ~(Action)0;
        }

        public static Action Do(this ICellAccessor cell, params System.Action[] actions)
        {
            foreach (var action in actions)
                action();

            return ~(Action)0;
        }

        public static Action ForEach<T>(this ICellAccessor cell, IEnumerable<T> collection, System.Action<T> action)
        {
            foreach (var t in collection)
                action(t);

            return ~(Action)0;
        }

        public static T Cast<T>(this ICellAccessor accessor) where T : ICellAccessor
        {
            var cell_group = accessor as IFanoutSearchCellAccessor;
            return cell_group.Cast<T>();
        }
    }
}
