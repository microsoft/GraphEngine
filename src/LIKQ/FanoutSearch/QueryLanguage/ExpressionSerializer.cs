// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using Serialize.Linq.Factories;
using Serialize.Linq.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serialize.Linq.Extensions;
using Serialize.Linq.Nodes;
using Trinity.Storage;

namespace FanoutSearch
{
    class ExpressionSerializer
    {
        [ThreadStatic]
        private static XmlSerializer s_serializer                  = null;
        [ThreadStatic]
        private static NodeFactory s_factory                       = null;

        private static void EnsureSerializer()
        {
            if (s_serializer == null)
            {
                s_serializer = new XmlSerializer();
                s_serializer.AddKnownType(typeof(Action));
                s_factory = new NodeFactory();
            }
        }

        internal static string Serialize(System.Linq.Expressions.Expression pred)
        {
            EnsureSerializer();
            return pred.ToXml(s_factory, s_serializer);
        }

        internal static Func<ICellAccessor, Action> DeserializeTraverseAction(string pred)
        {
            EnsureSerializer();
            var func_exp = s_serializer.Deserialize<LambdaExpressionNode>(pred).ToExpression<Func<ICellAccessor, Action>>();
            return func_exp.Compile();
        }

        internal static Func<ICellAccessor, bool> DeserializeOriginPredicate(string pred)
        {
            EnsureSerializer();
            var func_exp = s_serializer.Deserialize<LambdaExpressionNode>(pred).ToExpression<Func<ICellAccessor, bool>>();
            return func_exp.Compile();
        }
    }
}
