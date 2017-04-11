// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

        public static Expression BuildExpression(IQueryable<ICell> cells, ICellDescriptor cellDesc, IEnumerable<JObject> filters)
        {
            var icell = Expression.Parameter(typeof(ICell));
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

            return Expression.Call(typeof(Queryable), "Where", new Type[] { typeof(ICell) }, cells.Expression,
                Expression.Lambda<Func<ICell, bool>>(expr, new ParameterExpression[] { icell }));
        }

        internal static Expression ICellGetFieldExpression(ParameterExpression icell, string fieldName, Type fieldType)
        {
            var method = cellGetField.MakeGenericMethod(fieldType);
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

            Expression fieldExpr = null;
            Expression valueExpr = null;
            var fieldName = filter["attr"].Value<string>();
            if (fieldName == "CellID")
            {
                fieldExpr = Expression.Property(icell, fieldName);
                valueExpr = Expression.Constant(TypeDescriptor.GetConverter(typeof(long)).ConvertFromString(filter["value"].Value<string>()));
            }
            else
            {
                var fieldDesc = cellDesc.GetFieldDescriptors().FirstOrDefault(f => f.Name == fieldName);
                if (fieldDesc == null)
                    return null;
                fieldExpr = ICellGetFieldExpression(icell, fieldName, fieldDesc.Type);
                valueExpr = Expression.Constant(TypeDescriptor.GetConverter(fieldDesc.Type).ConvertFromString(filter["value"].Value<string>()));
            }

            var opt = filter["operator"].ToString();
            switch (opt)
            {
                case "EqualTo":
                    return Expression.Equal(fieldExpr, valueExpr);
                case "GreaterThan":
                    return Expression.GreaterThan(fieldExpr, valueExpr);
                case "LessThan":
                    return Expression.LessThan(fieldExpr, valueExpr);
                case "StringStartsWith":
                case "StringEndsWith":
                case "StringContains":
                    return Expression.Call(fieldExpr, stringMethods[opt], valueExpr);
                default:
                    return null;
            }
        }
    }
}
