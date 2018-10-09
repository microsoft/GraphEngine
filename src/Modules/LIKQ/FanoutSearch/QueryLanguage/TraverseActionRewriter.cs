// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FanoutSearch
{
    /// <summary>
    /// Rewrites a traverse action expression tree, so that:
    ///   1. Variables captured in a closure are evaluated to constants.
    ///   2. Enumeration constants, if necessary, are converted.
    ///     - Currently Serialize.Linq has a limitation, that an enumeration type does
    ///       not accept a value that is not defined in the enumeration type, even if
    ///       the value is a combination of the flags defined in the type.
    /// </summary>
    class TraverseActionRewriter : ExpressionVisitor
    {
        private static readonly Type s_compiler_generated_attr = typeof(System.Runtime.CompilerServices.CompilerGeneratedAttribute);
        private HashSet<Expression>  m_expressions_evaluated_as_constant = new HashSet<Expression>();

        private Expression EvaluateToConstant(Expression expr)
        {
            Expression new_expr = null;
            object eval_result  = null;
            Type   eval_type    = null;
            var const_expr      = expr as ConstantExpression;

            if (const_expr != null)
            {
                eval_result = const_expr.Value;
                eval_type   = const_expr.Type;
            }
            else
            {
                var func    = Expression.Lambda(expr).Compile();
                eval_result = func.DynamicInvoke();
                eval_type   = eval_result.GetType();
            }

            if (eval_type.IsEnum)
            {
                //translate to a convert expression, when the value is
                //not a single value defined in the type.
                var enum_single_values = (IList)Enum.GetValues(eval_type);
                if (!enum_single_values.Contains(eval_result))
                {
                    new_expr = ExpressionBuilder.CompositeEnumConstantToConvertExpression(eval_type, eval_result);
                }
            }

            if (new_expr == null) new_expr = Expression.Constant(eval_result);

            m_expressions_evaluated_as_constant.Add(new_expr);

            return new_expr;
        }

        private bool IsEvaluatedAsConstant(Expression expr)
        {
            return m_expressions_evaluated_as_constant.Contains(expr);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            return EvaluateToConstant(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var arguments = node.Arguments.Select(arg => Visit(arg)).ToList();
            var obj       = Visit(node.Object);
            node          = node.Update(obj, arguments);

            if (arguments.Any(arg => !IsEvaluatedAsConstant(arg)))
                goto bypass;
            if (!IsEvaluatedAsConstant(obj))
                goto bypass;

            return EvaluateToConstant(node);

            bypass:
            return node;
        }

        protected override Expression VisitInvocation(InvocationExpression node)
        {
            var arguments  = node.Arguments.Select(arg => Visit(arg)).ToList();
            var expression = Visit(node.Expression);
            node           = node.Update(expression, arguments);

            if (arguments.Any(arg => !IsEvaluatedAsConstant(arg)))
                goto bypass;

            if (!IsEvaluatedAsConstant(expression))
                goto bypass;

            /* If all the arguments, and the lambda expression itself, are evaluated,
             * Then the invocation can also be evaluated.
             */
            return EvaluateToConstant(node);

            bypass:
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            /* This is a call to a static member.*/
            if (node.Expression == null)
                return EvaluateToConstant(node);

            var instance_expr = node.Expression;
            var instance_type = instance_expr.Type;

            if (instance_type.GetCustomAttributes(s_compiler_generated_attr, inherit: false).Any())
            {
                /* This is a member access of the compiler-generated anonymous class to hold closure variables. */
                return EvaluateToConstant(node);
            }
            else
            {
                /* If the expression is evaluated, then its member access can also be evaluated. */
                var new_expr = base.Visit(instance_expr);
                node         = node.Update(new_expr);

                if (IsEvaluatedAsConstant(new_expr))
                {
                    return EvaluateToConstant(node);
                }
                else
                {
                    return node;
                }
            }
        }
    }
}
