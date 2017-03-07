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
using Trinity.Diagnostics;
using Trinity.Storage;

namespace Trinity.Modules.Spark
{
    public class FilterExpressionBuilder
    {
        private static readonly PropertyInfo cellTypeName = typeof(ITypeDescriptor).GetProperty("TypeName");
        private static readonly MethodInfo cellGetField = typeof(ICell).GetMethod("GetField", new Type[] { typeof(string) });
        private static readonly Dictionary<string, MethodInfo> stringMethods = new Dictionary<string, MethodInfo>()
        {
            { "StringStartsWith", typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }) },
            { "StringEndsWith", typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }) },
            { "StringContains", typeof(string).GetMethod("Contains", new Type[] { typeof(string) }) },
        };

        public static Expression BuildExpression(IQueryable<ICellAccessor> cells, ICellDescriptor cellDesc, IEnumerable<JObject> filters)
        {
            var icell = Expression.Parameter(typeof(ICellAccessor));
            var expr = Expression.Equal(Expression.Property(icell, cellTypeName), Expression.Constant(cellDesc.TypeName));

            if (filters != null)
            {
                try
                {
                    var filtersExpr = BuildExpression(icell, cellDesc, filters);
                    if (filtersExpr != null)
                        expr = Expression.AndAlso(expr, filtersExpr);
                }
                catch (Exception e)
                {
                    Log.WriteLine(LogLevel.Error, $"Build filter expression exception: {e}");
                }
            }

            return Expression.Call(typeof(Queryable), "Where", new Type[] { typeof(ICellAccessor) }, cells.Expression,
                Expression.Lambda<Func<ICellAccessor, bool>>(expr, new ParameterExpression[] { icell }));
        }

        internal static Expression ICellGetFieldExpression<T>(ParameterExpression icell, string fieldName)
        {
            var method = cellGetField.MakeGenericMethod(typeof(T));
            return Expression.Call(icell, method, Expression.Constant(fieldName));
        }

        internal static Expression BuildExpression(ParameterExpression icell, ICellDescriptor cellDesc, IEnumerable<JObject> filters)
        {
            if (icell == null || cellDesc == null || filters == null || filters.Count() == 0)
                return null;

            var left = BuildExpression(icell, cellDesc, filters.First());
            var right = BuildExpression(icell, cellDesc, filters.Skip(1));

            if (left == null)
                return right;

            if (right == null)
                return left;

            return Expression.AndAlso(left, right);
        }

        internal static Expression BuildExpression(ParameterExpression icell, ICellDescriptor cellDesc, JObject filter)
        {
            if (icell == null || cellDesc == null || filter == null)
                return null;

            var opt = filter["operator"].ToString();
            switch (opt)
            {
                case "StringStartsWith":
                case "StringEndsWith":
                case "StringContains":
                    return BuildFieldFilterExpression<string, string>(icell, filter["attr"].ToString(), opt, filter["value"].ToString());
                case "GreaterThan":
                case "LessThan":
                    return BuildFieldFilterExpression<double, double>(icell, filter["attr"].ToString(), opt, filter["value"].ToString());
                default:
                    return null;
            }
        }

        internal static Expression BuildFieldFilterExpression<TField, TValue>(ParameterExpression icell, string fieldName, string opt, string value)
        {
            var field = ICellGetFieldExpression<TField>(icell, fieldName);
            var val = BuildConstantExpression<TValue>(value);
            switch (opt)
            {
                case "StringStartsWith":
                case "StringEndsWith":
                case "StringContains":
                    return Expression.Call(field, stringMethods[opt], val);
                case "GreaterThan":
                    return Expression.GreaterThan(field, val);
                case "LessThan":
                    return Expression.LessThan(field, val);
                default:
                    return null;
            }
        }

        internal static ConstantExpression BuildConstantExpression<T>(string value)
        {
            if (typeof(T) == typeof(string))
                return Expression.Constant(value);
            if (typeof(T) == typeof(double))
                return Expression.Constant(double.Parse(value));

            return Expression.Constant(value);
        }
    }
}
