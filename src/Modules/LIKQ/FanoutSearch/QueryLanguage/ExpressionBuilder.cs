// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Trinity.Storage;

namespace FanoutSearch
{
    internal static class ExpressionBuilder
    {
        #region Method info objects
        private static readonly MemberInfo         s_icell_accessor_cell_id_member = typeof(ICell).GetMember("CellId").First();
        private static readonly MethodInfo         s_long_ienumerable_contains     = typeof(Enumerable).GetMethods(BindingFlags.Public | BindingFlags.Static).First(_ => _.Name == "Contains" && _.GetParameters().Count() == 2).MakeGenericMethod(typeof(long));
        private static readonly MethodInfo         s_string_contains               = typeof(string).GetMethod("Contains");
        private static readonly MethodInfo         s_icell_has                     = typeof(Verbs).GetMethod("has", new Type[] { typeof(ICell), typeof(string) });
        private static readonly MethodInfo         s_icell_has_value               = typeof(Verbs).GetMethod("has", new Type[] { typeof(ICell), typeof(string), typeof(string) });
        private static readonly MethodInfo         s_icell_get                     = typeof(Verbs).GetMethod("get", new Type[] { typeof(ICell), typeof(string) });
        private static readonly MethodInfo         s_icell_type                    = typeof(Verbs).GetMethod("type");
        private static readonly MethodInfo         s_icell_count                   = typeof(Verbs).GetMethod("count");
        #region numerical
        private static readonly MethodInfo         s_icell_gt_int                  = typeof(Verbs).GetMethod("greater_than", new Type[] {typeof(ICell), typeof(string), typeof(int) });
        private static readonly MethodInfo         s_icell_gt_double               = typeof(Verbs).GetMethod("greater_than", new Type[] {typeof(ICell), typeof(string), typeof(double) });
        private static readonly MethodInfo         s_icell_geq_int                 = typeof(Verbs).GetMethod("greater_than_or_equal", new Type[] {typeof(ICell), typeof(string), typeof(int) });
        private static readonly MethodInfo         s_icell_geq_double              = typeof(Verbs).GetMethod("greater_than_or_equal", new Type[] {typeof(ICell), typeof(string), typeof(double) });
        private static readonly MethodInfo         s_icell_lt_int                  = typeof(Verbs).GetMethod("less_than", new Type[] {typeof(ICell), typeof(string), typeof(int) });
        private static readonly MethodInfo         s_icell_lt_double               = typeof(Verbs).GetMethod("less_than", new Type[] {typeof(ICell), typeof(string), typeof(double) });
        private static readonly MethodInfo         s_icell_leq_int                 = typeof(Verbs).GetMethod("less_than_or_equal", new Type[] {typeof(ICell), typeof(string), typeof(int) });
        private static readonly MethodInfo         s_icell_leq_double              = typeof(Verbs).GetMethod("less_than_or_equal", new Type[] {typeof(ICell), typeof(string), typeof(double) });
        #endregion
        #endregion

        internal static Expression CompositeEnumConstantToConvertExpression(Type type, object value)
        {
            int action_int_val              = (int)value;
            ConstantExpression action_const = Expression.Constant(action_int_val);
            Expression action_converted     = Expression.Convert(action_const, type);

            return action_converted;
        }

        internal static Expression<Func<ICellAccessor, Action>> WrapAction(Action action)
        {
            return Expression.Lambda<Func<ICellAccessor, Action>>(
                Expression.Constant(action),
                Expression.Parameter(typeof(ICellAccessor)));
        }

        internal static Expression<Func<ICellAccessor, Action>> GenerateTraverseActionFromCellIds(Action action, List<long> cellIds)
        {
            /***********************************************
             * The target expression:
             *
             * icell_accessor =>
             *   [cell_id_predicate] ?
             *     [given action] : ~0
             *
             * cell_id_predicate:
             *   single cell id:
             *     input_icell_param.CellId == cell_id_const
             *   multiple cell ids:
             *     cell_id_const.Contains(input_icell_param.CellId)
             *
             ***********************************************/

            /* 1. input params */

            ParameterExpression   input_icell_param = Expression.Parameter(typeof(ICellAccessor), "icell_accessor");
            bool                  single_cell_id    = cellIds.Count == 1;
            ConstantExpression    cell_id_const     = single_cell_id ? Expression.Constant(cellIds[0]) : Expression.Constant(cellIds.ToArray());
            ConstantExpression    action_const      = Expression.Constant(action);
            ConstantExpression    abort_const       = Expression.Constant(~(Action)0);
            MemberExpression      cell_id_expr      = Expression.MakeMemberAccess(input_icell_param, s_icell_accessor_cell_id_member);

            /* 2. cell_id_predicate */
            Expression            cell_id_predicate = single_cell_id ? Expression.Equal(cell_id_expr, cell_id_const) : Expression.Call(s_long_ienumerable_contains, cell_id_const, cell_id_expr) as Expression;

            /* 3. conditional tenary operator */
            ConditionalExpression cond_expr         = Expression.Condition(cell_id_predicate, action_const, abort_const);

            /* Final lambda expression */
            return Expression.Lambda<Func<ICellAccessor, Action>>(cond_expr, input_icell_param);
        }

        internal static Expression GenerateFieldOperatorExpression(string fieldname, string op, JToken op_obj, ParameterExpression icell)
        {
            ConstantExpression fieldname_expr = Expression.Constant(fieldname);
            switch (op)
            {
                case JsonDSL.Count:
                    return GenerateFieldOperatorCountExpression(fieldname_expr, op_obj, icell);
                case JsonDSL.Substring:
                    return GenerateFieldOperatorSubstringExpression(fieldname_expr, op_obj, icell);
                case JsonDSL.gt:
                case JsonDSL.sym_gt:
                    return GenerateFieldOperatorGreaterThanExpression(fieldname_expr, op_obj, icell);
                case JsonDSL.lt:
                case JsonDSL.sym_lt:
                    return GenerateFieldOperatorLessThanExpression(fieldname_expr, op_obj, icell);
                case JsonDSL.geq:
                case JsonDSL.sym_geq:
                    return GenerateFieldOperatorGreaterEqualExpression(fieldname_expr, op_obj, icell);
                case JsonDSL.leq:
                case JsonDSL.sym_leq:
                    return GenerateFieldOperatorLessEqualExpression(fieldname_expr, op_obj, icell);
                default:
                    //TODO ignore unrecognized opcode or throw error?
                    throw new FanoutSearchQueryException("Unrecognized operator " + op);
            }
        }

        #region Field operators
        #region Numerical
        internal static Expression GenerateFieldOperatorLessEqualExpression(Expression field, JToken op_obj, ParameterExpression icell)
        {
            if (op_obj.Type == JTokenType.Integer)
            {
                return Expression.Call(s_icell_leq_int, icell, field, Expression.Constant((int)op_obj));
            }
            else /* Assume Double */
            {
                return Expression.Call(s_icell_leq_double, icell, field, Expression.Constant((double)op_obj));
            }
        }

        internal static Expression GenerateFieldOperatorGreaterEqualExpression(Expression field, JToken op_obj, ParameterExpression icell)
        {
            if (op_obj.Type == JTokenType.Integer)
            {
                return Expression.Call(s_icell_geq_int, icell, field, Expression.Constant((int)op_obj));
            }
            else /* Assume Double */
            {
                return Expression.Call(s_icell_geq_double, icell, field, Expression.Constant((double)op_obj));
            }
        }

        internal static Expression GenerateFieldOperatorLessThanExpression(Expression field, JToken op_obj, ParameterExpression icell)
        {
            if (op_obj.Type == JTokenType.Integer)
            {
                return Expression.Call(s_icell_lt_int, icell, field, Expression.Constant((int)op_obj));
            }
            else /* Assume Double */
            {
                return Expression.Call(s_icell_lt_double, icell, field, Expression.Constant((double)op_obj));
            }
        }

        internal static Expression GenerateFieldOperatorGreaterThanExpression(Expression field, JToken op_obj, ParameterExpression icell)
        {
            try
            {
                if (op_obj.Type == JTokenType.Integer)
                {
                    return Expression.Call(s_icell_gt_int, icell, field, Expression.Constant((int)op_obj));
                }
                else /* Assume Double */
                {
                    return Expression.Call(s_icell_gt_double, icell, field, Expression.Constant((double)op_obj));
                }
            }
            catch (ArgumentException) { throw new FanoutSearchQueryException("Invalid comparand"); }
            catch (FormatException) { throw new FanoutSearchQueryException("Invalid comparand"); }
        }
        #endregion

        internal static Expression GenerateFieldOperatorSubstringExpression(Expression field, JToken op_obj, ParameterExpression icell)
        {
            ConstantExpression str_expr       = Expression.Constant((string)op_obj);
            MethodCallExpression get_call     = Expression.Call(s_icell_get, icell, field);

            return Expression.Call(get_call, s_string_contains, str_expr);
        }

        internal static Expression GenerateFieldOperatorCountExpression(Expression field, JToken op_obj, ParameterExpression icell)
        {
            MethodCallExpression count_call   = Expression.Call(s_icell_count, icell, field);
            if (op_obj.Type == JTokenType.Integer)
            {
                return Expression.Equal(count_call, Expression.Constant((int)op_obj));
            }
            else /* Assume Json struct, extract 1st child */
            {
                IEnumerable<KeyValuePair<string, JToken>> dict = op_obj as JObject;
                if (dict == null) { throw new FanoutSearchQueryException("Invalid count operand"); }
                var key  = dict.First().Key;
                var val  = dict.First().Value;
                if(val.Type != JTokenType.Integer) { throw new FanoutSearchQueryException("Invalid count value"); }
                var count_cnt_expr = Expression.Constant((int)val);
                switch (key)
                {
                    case JsonDSL.gt:
                    case JsonDSL.sym_gt:
                        return Expression.GreaterThan(count_call, count_cnt_expr);
                    case JsonDSL.lt:
                    case JsonDSL.sym_lt:
                        return Expression.LessThan(count_call, count_cnt_expr);
                    case JsonDSL.geq:
                    case JsonDSL.sym_geq:
                        return Expression.GreaterThanOrEqual(count_call, count_cnt_expr);
                    case JsonDSL.leq:
                    case JsonDSL.sym_leq:
                        return Expression.LessThanOrEqual(count_call, count_cnt_expr);
                    default:
                        throw new FanoutSearchQueryException("Unrecognized comparator " + key);
                }
            }
        }
        #endregion
        /// <summary>
        /// When the field key is a DSL keyword, it may also represents a predicate of:
        ///    - type query
        ///    - cell id query
        ///    - has query
        /// </summary>
        internal static Expression GenerateFieldPredicateExpression(string pred_key, JToken pred_obj, ParameterExpression icell)
        {
            ConstantExpression       key_expr      = Expression.Constant(pred_key);
            Lazy<ConstantExpression> pred_str_expr = new Lazy<ConstantExpression>(() => Expression.Constant((string)pred_obj));

            switch (pred_key)
            {
                case JsonDSL.Id:
                    {
                        ConstantExpression id_list  = Expression.Constant((pred_obj as JArray).Select(_ => (long)_).ToArray());
                        MemberExpression   icell_id = Expression.MakeMemberAccess(icell, s_icell_accessor_cell_id_member);
                        return Expression.Call(s_long_ienumerable_contains, id_list, icell_id);
                    }

                case JsonDSL.Type:
                    {
                        return Expression.Call(s_icell_type, icell, pred_str_expr.Value);
                    }

                case JsonDSL.Has:
                    {
                        if(pred_obj.Type != JTokenType.String) { throw new FanoutSearchQueryException("Invalid has operand"); }
                        return Expression.Call(s_icell_has, icell, pred_str_expr.Value);
                    }
            }

            switch (pred_obj.Type)
            {
                case JTokenType.String:
                    return Expression.IsTrue(Expression.Call(s_icell_has_value, icell, key_expr, pred_str_expr.Value));
                case JTokenType.Object:
                    {
                        IEnumerable<KeyValuePair<string, JToken>> child_tokens = pred_obj as JObject;

                        if (child_tokens == null || child_tokens.Count() == 0)
                        {
                            /* If no conditions are specified, return true */
                            return Expression.Constant(true);
                        }

                        Expression field_pred_exp = GenerateFieldOperatorExpression(pred_key, child_tokens.First().Key, child_tokens.First().Value, icell);
                        foreach (var kvp in child_tokens.Skip(1))
                        {
                            field_pred_exp = Expression.AndAlso(field_pred_exp, GenerateFieldOperatorExpression(pred_key, kvp.Key, kvp.Value, icell));
                        }
                        return field_pred_exp;
                    }
                default:
                    throw new FanoutSearchQueryException("Invalid property value");
            }
        }

        /// <summary>
        /// Generates boolean expressions from the Json DSL predicate object.
        /// </summary>
        /// <param name="pred_object">Caller guarantees that pred_object is not null</param>
        /// <param name="icell"></param>
        /// <returns></returns>
        internal static Expression GenerateBooleanPredicateExpression(JObject pred_object, ParameterExpression icell)
        {
            JToken or_token = null;
            JToken not_token = null;

            JObject or_obj = null;
            JObject not_obj = null;

            if(pred_object.TryGetValue(JsonDSL.or, out or_token))
            {
                or_obj = or_token as JObject;
            }

            if(pred_object.TryGetValue(JsonDSL.not, out not_token))
            {
                not_obj = not_token as JObject;
            }

            if (or_obj != null && not_obj != null)
            {
                throw new FanoutSearchQueryException("Cannot specify not/or conditions together.");
            }

            if (or_obj != null)
            {
                if (or_obj.Count == 0)
                {
                    throw new FanoutSearchQueryException("No predicates found in OR expression.");
                }

                IEnumerable<KeyValuePair<string, JToken>> enumerable = or_obj;
                Expression or_pred = GenerateFieldPredicateExpression(enumerable.First().Key, enumerable.First().Value, icell);
                foreach (var or_other_conditions in enumerable.Skip(1))
                {
                    or_pred = Expression.OrElse(or_pred, GenerateFieldPredicateExpression(or_other_conditions.Key, or_other_conditions.Value, icell));
                }
                return or_pred;
            }
            else if (not_obj != null)
            {
                return Expression.IsFalse(GenerateBooleanPredicateExpression(not_obj, icell));
            }
            else /* and also expr */
            {
                IEnumerable<KeyValuePair<string, JToken>> enumerable = pred_object;

                if (pred_object.Count == 0)
                {
                    throw new FanoutSearchQueryException("No predicates found.");
                }

                Expression and_pred = GenerateFieldPredicateExpression(enumerable.First().Key, enumerable.First().Value, icell);
                foreach (var and_other_conditions in enumerable.Skip(1))
                {
                    and_pred = Expression.AndAlso(and_pred, GenerateFieldPredicateExpression(and_other_conditions.Key, and_other_conditions.Value, icell));
                }
                return and_pred;
            }
        }

        /// <summary>
        /// Generates conditional expressions of form (pred) ? action: noaction
        /// When pred is empty, or always evaluates to true, generate Action.pred_action.
        /// When pred is false, generate NOACTION
        /// When pred is null, return null.
        /// </summary>
        internal static Expression GenerateConditionalPredicateExpression(JObject pred_object, Action pred_action, ParameterExpression icell)
        {
            ConstantExpression    action_const   = Expression.Constant(pred_action);
            Expression            noaction_const = Expression.Convert(Expression.Constant(~(int)0), typeof(FanoutSearch.Action));

            if (pred_object == null)
                return null;

            if ((pred_object.Type == JTokenType.Boolean && ((bool)pred_object) == true) || pred_object.Count == 0)
                return action_const;

            if (pred_object.Type == JTokenType.Boolean && (bool)pred_object == false)
                return noaction_const;

            return Expression.Condition(GenerateBooleanPredicateExpression(pred_object, icell), action_const, noaction_const);
        }

        /// <summary>
        /// If both return & continue predicates are not given, the action_object itself will be parsed
        /// as a conditional predicate expression, defaulting to default_action.
        /// If action_object is null, returns a pass-through default action (_ => default_action).
        /// 
        /// Valid action_object examples:
        /// 
        /// 1.
        /// {
        ///     return: { ... }
        /// }
        /// 
        /// 2.
        /// {
        ///     continue: { ... }
        /// }
        /// 
        /// 3.
        /// {
        ///     return: { ... }
        ///     continue: { ... }
        /// }
        /// 
        /// 4.
        /// {
        ///     /* no return, no continue, but can be directly parsed as traverse conditions */
        /// }
        /// 
        /// </summary>
        internal static Expression<Func<ICellAccessor, Action>> GenerateTraverseActionFromQueryObject(JObject action_object, Action default_action)
        {
            ParameterExpression icell_param = Expression.Parameter(typeof(ICellAccessor), "icell_accessor");

            if (action_object == null)
            {
                return WrapAction(default_action);
            }

            JToken    continue_obj         = action_object[JsonDSL.Continue];
            JToken    return_obj           = action_object[JsonDSL.Return];
            Expression continue_pred       = GenerateConditionalPredicateExpression(continue_obj as JObject, Action.Continue, icell_param);
            Expression return_pred         = GenerateConditionalPredicateExpression(return_obj as JObject, Action.Return, icell_param);
            Expression lambda_body;

            if(continue_pred == null && continue_obj != null) { throw new FanoutSearchQueryException("Invalid continue expression"); }
            if(return_pred == null && return_obj != null) { throw new FanoutSearchQueryException("Invalid return expression"); }

            if (continue_pred != null && return_pred != null)
            {
                //  Bitwise 'AND' operator does not work with flag enums.
                //  According to system-generated expression trees, they
                //  are translated to integers, bitwise-and, and then
                //  converted back.

                lambda_body =
                    Expression.Convert(
                        Expression.And(
                            Expression.Convert(continue_pred, typeof(Int32)),
                            Expression.Convert(return_pred, typeof(Int32))),
                        typeof(FanoutSearch.Action));
            }
            else if (continue_pred != null || return_pred != null)
            {
                lambda_body = continue_pred ?? return_pred;
            }
            else /* continue_pred == null && return_pred == null */
            {
                lambda_body = GenerateConditionalPredicateExpression(action_object, default_action, icell_param);
            }

            return Expression.Lambda<Func<ICellAccessor, Action>>(lambda_body, icell_param);
        }

        internal static Expression<Func<ICellAccessor, Action>> GenerateStronglyTypedTraverseAction<T>(Expression<Func<T, Action>> action) where T : ICellAccessor
        {
            /***********************************************
             * The target expression:
             * 
             * icell_accessor => 
             *   icell_accessor.let(
             *      icell.Cast<T>(), 
             *      (_, strongly_typed_accessor) =>
             *          action(strongly_typed_accessor));
             ***********************************************/

            /***********************************************
             Example:
             
            Func<T, TraverseAction> action_func = t => TraverseAction.Stop;
            Func<ICellAccessor, TraverseAction> target_func =
                icell_accessor =>
                    icell_accessor.let(
                        icell_accessor.Cast<T>(),
                        (_, strongly_typed_accessor) =>
                            action_func(strongly_typed_accessor));

            ************************************************/

            /* 1. input params */

            ParameterExpression input_icell_param = Expression.Parameter(typeof(ICellAccessor), "icell_accessor");
            ParameterExpression _param = Expression.Parameter(typeof(ICellAccessor), "_");
            ParameterExpression s_typed_param = Expression.Parameter(typeof(T), "strongly_typed_accessor");

            /* 2. The invocation of the strongly-typed action */
            InvocationExpression action_invoke = Expression.Invoke(action, s_typed_param);

            /* 3. inner lambda expression */
            LambdaExpression inner_lambda = Expression.Lambda<Func<ICellAccessor, T, Action>>(action_invoke, _param, s_typed_param);

            /* 4. let call */
            MethodCallExpression cast_call = Expression.Call(typeof(Verbs), "Cast", new Type[] { typeof(T) }, new Expression[] { input_icell_param });
            MethodCallExpression let_call = Expression.Call(typeof(Verbs), "let", new Type[] { typeof(T) }, new Expression[] { input_icell_param, cast_call, inner_lambda });

            /* 5. final lambda expression */
            Expression<Func<ICellAccessor, Action>> wrapped_action = Expression.Lambda<Func<ICellAccessor, Action>>(let_call, input_icell_param);

            return wrapped_action;
        }
    }
}
