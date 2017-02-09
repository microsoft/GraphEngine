// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
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
    class TraverseActionSecurityChecker : ExpressionVisitor
    {
        private static readonly HashSet<Type> s_WhitelistTypes = new HashSet<Type>
        {
            typeof(byte),
            typeof(sbyte),
            typeof(char),
            typeof(short),
            typeof(ushort),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(ulong),

            typeof(float),
            typeof(double),
            typeof(decimal),

            typeof(string),
            typeof(Guid),
            typeof(DateTime),
            typeof(ICell),
            typeof(ICellAccessor),
            typeof(Verbs),
            typeof(FanoutSearch.Action),

            typeof(System.Action),
            typeof(System.Action<>),
            typeof(System.Action<,>),
            typeof(System.Action<,,>),
            typeof(System.Action<,,,>),
            typeof(System.Action<,,,,>),
            typeof(System.Action<,,,,,>),
            typeof(System.Action<,,,,,,>),
            typeof(System.Action<,,,,,,,>),
            typeof(System.Action<,,,,,,,,>),
            typeof(System.Action<,,,,,,,,,>),
            typeof(System.Action<,,,,,,,,,,>),
            typeof(System.Action<,,,,,,,,,,,>),
            typeof(System.Action<,,,,,,,,,,,,>),
            typeof(System.Action<,,,,,,,,,,,,,>),
            typeof(System.Action<,,,,,,,,,,,,,,>),
            typeof(System.Action<,,,,,,,,,,,,,,,>),
            typeof(System.Func<>),
            typeof(System.Func<,>),
            typeof(System.Func<,,>),
            typeof(System.Func<,,,>),
            typeof(System.Func<,,,,>),
            typeof(System.Func<,,,,,>),
            typeof(System.Func<,,,,,,>),
            typeof(System.Func<,,,,,,,>),
            typeof(System.Func<,,,,,,,,>),
            typeof(System.Func<,,,,,,,,,>),
            typeof(System.Func<,,,,,,,,,,>),
            typeof(System.Func<,,,,,,,,,,,>),
            typeof(System.Func<,,,,,,,,,,,,>),
            typeof(System.Func<,,,,,,,,,,,,,>),
            typeof(System.Func<,,,,,,,,,,,,,,>),
            typeof(System.Func<,,,,,,,,,,,,,,,>),

            typeof(List<>),
            typeof(Enumerable),
            typeof(IEnumerable<>),
            typeof(HashSet<>),
            typeof(Array),
        };

        private static readonly HashSet<MethodInfo> s_BlacklistMethods = new HashSet<MethodInfo>
        {
            typeof(ICell).GetMethod("SetField"),
            typeof(ICell).GetMethod("AppendToField"),
            typeof(ICellAccessor).GetMethod("SetField"),
            typeof(ICellAccessor).GetMethod("AppendToField"),
        };

        private void CheckBlacklistMethod(MethodInfo method)
        {
            if (method.IsGenericMethod) method = method.GetGenericMethodDefinition();
            if (s_BlacklistMethods.Contains(method))
            {
                // omit details from a user, but tells about the method name.
                throw new FanoutSearchQueryException("Syntax error: " + method.Name);
            }
        }

        private void CheckWhitelistType(Type type)
        {
            if (type.IsGenericType)
            {
                Type invalid_generic_type_arg = type.GetGenericArguments().FirstOrDefault(_ => !s_WhitelistTypes.Contains(_));
                if (invalid_generic_type_arg != null)
                {
                    type = invalid_generic_type_arg;
                    goto invalid_type;
                }
                // all type args good. abstract them away.
                type = type.GetGenericTypeDefinition();
            }

            if (!s_WhitelistTypes.Contains(type))
            {
                goto invalid_type;
            }

            return;

            invalid_type:
            throw new FanoutSearchQueryException("Referencing a type not allowed: " + type.Name);

        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            CheckWhitelistType(node.Method.ReflectedType);
            CheckBlacklistMethod(node.Method);

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            return base.VisitMember(node);
        }

        protected override Expression VisitNew(NewExpression node)
        {
            CheckWhitelistType(node.Type);

            return base.VisitNew(node);
        }

        protected override Expression VisitNewArray(NewArrayExpression node)
        {
            CheckWhitelistType(node.Type.GetElementType());

            return base.VisitNewArray(node);
        }

        protected override Expression VisitExtension(Expression node)
        {
            Console.WriteLine(node.ToString());
            return base.VisitExtension(node);
        }
    }
}
