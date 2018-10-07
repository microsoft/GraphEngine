// Graph Engine
// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE.md file in the project root for full license information.
//
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.CodeDom.Compiler;
using System.IO;
using System.Security.Permissions;
using System.Reflection;
using Trinity;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using FanoutSearch.LIKQ;
using System.Linq.Expressions;
using Trinity.Storage;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace FanoutSearch
{
    internal class LambdaDSLSyntaxErrorException : Exception
    {
        private SyntaxNode m_errorSyntaxNode;
        private static string FormatMessage(string message, SyntaxNode node)
        {
            if (message == null) message = "Syntax error";
            return message + ": " + node.GetText().ToString();
        }

        public LambdaDSLSyntaxErrorException(string message, SyntaxNode node) :
            base(FormatMessage(message, node))
        {
            m_errorSyntaxNode = node;
        }

        public LambdaDSLSyntaxErrorException(SyntaxNode node) :
            base(FormatMessage(null, node))
        {
            m_errorSyntaxNode = node;
        }
    }

    //TODO JsonDSL and LambdaDSL could share ExpressionBuilder.
    //Each DSL should provide ExpressionBuilder-compatible interfaces
    //that report the purpose of the syntax node, thus hiding the
    //details of a DSL's syntax.
    public static class LambdaDSL
    {
        private class FSDescCallchainElement
        {
            public string Method;
            public List<ArgumentSyntax> Arguments;
            public InvocationExpressionSyntax SyntaxNode;
        }

        #region constants
        private const string c_code_header = @"
public static class FanoutSearchDescriptorEvaluator
{
    public static void _evaluate()
    {";
        private const string c_code_footer = @"
    }
}
";
        private static string s_LIKQ_Prolog       = "MAG";
        private static string s_LIKQ_StartFrom    = "StartFrom";
        private static string s_LIKQ_VisitNode    = "VisitNode";
        private static string s_LIKQ_FollowEdge   = "FollowEdge";
        private static string s_LIKQ_Action       = "Action";
        private static string s_LIKQ_FanoutSearch = "FanoutSearch";
        #endregion

        #region helpers
        private static void ThrowIf(bool condition, string message, SyntaxNode syntaxNode)
        {
            if (condition) throw new LambdaDSLSyntaxErrorException(message, syntaxNode);
        }

        private static T Get<T>(object obj, string message = null) where T : class
        {
            var ret = obj as T;
            ThrowIf(ret == null, message, obj as SyntaxNode);
            return ret;
        }

        private static T TryGet<T>(object obj) where T : class
        {
            return obj as T;
        }

        private class FanoutSearchActionRewritter : CSharpSyntaxRewriter
        {
            public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
            {
                do
                {
                    var expr = TryGet<IdentifierNameSyntax>(node.Expression);
                    if (expr == null) break;
                    if (expr.GetText().ToString() != s_LIKQ_Action) break;

                    //  Action.[expr] => FanoutSearch.Action.[expr]

                    return SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                              SyntaxFactory.IdentifierName(s_LIKQ_FanoutSearch),
                              SyntaxFactory.IdentifierName(s_LIKQ_Action)),
                            node.Name);
                } while (false);
                return base.VisitMemberAccessExpression(node);
            }
        }

        #endregion

        /// <summary>
        /// The prolog is "___".StartFrom(...)....
        /// </summary>
        private static void CheckProlog(IdentifierNameSyntax identifierNameSyntax)
        {
            bool prolog_valid = identifierNameSyntax.Identifier.Text == s_LIKQ_Prolog;

            ThrowIf(!prolog_valid, "Invalid prolog", identifierNameSyntax);
        }

        private static List<FSDescCallchainElement> GetFanoutSearchDescriptorCallChain(InvocationExpressionSyntax callchain_root)
        {
            List<FSDescCallchainElement> callchain = new List<FSDescCallchainElement>();
            InvocationExpressionSyntax invocation = callchain_root;

            while (invocation != null)
            {
                // Each invocation MUST be a member access, so there aren't dangling invocations like 'StartFrom(0)', without an identifier as prolog.
                MemberAccessExpressionSyntax member = Get<MemberAccessExpressionSyntax>(invocation.Expression);
                callchain.Add(new FSDescCallchainElement
                {
                    Arguments  = invocation.ArgumentList.Arguments.ToList(),
                    Method     = member.Name.Identifier.Text,
                    SyntaxNode = invocation
                });

                invocation = member.Expression as InvocationExpressionSyntax;

                if (member.Expression is IdentifierNameSyntax)
                {
                    CheckProlog(member.Expression as IdentifierNameSyntax);
                }
            }

            ThrowIf(callchain.Count == 0, "The query string does not contain an expression.", callchain_root);
            callchain.Reverse();

            return callchain;
        }

        private static FanoutSearchDescriptor ConstructFanoutSearchDescriptor(List<FSDescCallchainElement> fs_callchain)
        {
            FanoutSearchDescriptor fs_desc = ConstructFanoutSearchOrigin(fs_callchain.First());
            for (int idx = 1; idx < fs_callchain.Count; ++idx)
            {
                FSDescCallchainElement visitNode     = fs_callchain[idx];
                FSDescCallchainElement followEdge    = null;
                FanoutSearch.Action    defaultAction = Action.Continue;

                if (visitNode.Method == s_LIKQ_FollowEdge)
                {
                    ThrowIf(++idx >= fs_callchain.Count, "The query expression cannot end with '" + s_LIKQ_FollowEdge + "'", visitNode.SyntaxNode);
                    followEdge = visitNode;
                    visitNode  = fs_callchain[idx];
                    if (visitNode.Method == s_LIKQ_FollowEdge)
                    {
                        // two consecutive FollowEdge, use default traverse action.
                        --idx;
                        visitNode = null;
                    }
                }

                if (idx + 1 >= fs_callchain.Count)
                {
                    // The last element should have default action Action.Return
                    defaultAction = Action.Return;
                }

                ThrowIf(visitNode.Method != s_LIKQ_VisitNode, "Expecting '" + s_LIKQ_VisitNode + "'", visitNode.SyntaxNode);
                fs_desc = ConstructFanoutSearchBody(fs_desc, visitNode, followEdge, defaultAction);
            }

            return fs_desc;
        }

        private static FanoutSearchDescriptor ConstructFanoutSearchBody(FanoutSearchDescriptor current, FSDescCallchainElement visitNode, FSDescCallchainElement followEdge, Action defaultTraverseAction)
        {
            List<string> follow_edge_set = ConstructFollowEdgeSet(followEdge);
            EdgeTypeDescriptor ets       = current.FollowEdge(follow_edge_set.ToArray());

            if (visitNode == null)
            {
                return ets.VisitNode(defaultTraverseAction);
            }
            else if (visitNode.Arguments.Count == 1)
            {
                return ets.VisitNode(ConstructVisitNodeAction(visitNode.Arguments[0].Expression));
            }
            else /*if (visitNode.Arguments.Count > 1)*/
            {
                return ets.VisitNode(
                    ConstructVisitNodeAction(visitNode.Arguments[0].Expression),
                    ConstructCollection<string>(visitNode.Arguments[1].Expression) /*select*/);
            }
        }

        /// <summary>
        /// Analyse expressions of form new[] { .., .., ....}, or new List[T] { .., .., .. }, or new T[] {..., ...}
        /// Support string collection, or numerical collection
        /// </summary>
        /// <typeparam name="T">
        /// Only supports string and long
        /// </typeparam>
        private static IEnumerable<T> ConstructCollection<T>(ExpressionSyntax collectionSyntax)
        {
            InitializerExpressionSyntax initializer = null;
            SyntaxKind element_type_kind = SyntaxKind.VoidKeyword;
            SyntaxKind element_literal_kind = SyntaxKind.VoidKeyword;
            if (typeof(T) == typeof(long))
            {
                element_type_kind    = SyntaxKind.LongKeyword;
                element_literal_kind = SyntaxKind.NumericLiteralExpression;
            }
            else if (typeof(T) == typeof(string))
            {
                element_type_kind    = SyntaxKind.StringKeyword;
                element_literal_kind = SyntaxKind.StringLiteralExpression;
            }
            else
            {
                throw new LambdaDSLSyntaxErrorException("Invalid collection element type", collectionSyntax);
            }

            switch (collectionSyntax.Kind())
            {
                case SyntaxKind.ObjectCreationExpression:
                    /*List<T>*/
                    var list_expr     = Get<ObjectCreationExpressionSyntax>(collectionSyntax);
                    var list_type     = Get<GenericNameSyntax>(list_expr.Type);
                    ThrowIf(list_type.Arity != 1, null, syntaxNode: list_type);
                    var list_type_arg = Get<PredefinedTypeSyntax>(list_type.TypeArgumentList.Arguments[0]);
                    ThrowIf(!list_type_arg.Keyword.IsKind(element_type_kind), "Invalid collection element type", syntaxNode: list_type_arg);
                    initializer       = list_expr.Initializer;
                    break;
                case SyntaxKind.ImplicitArrayCreationExpression:
                    /*Implicit array*/
                    initializer = Get<InitializerExpressionSyntax>((collectionSyntax as ImplicitArrayCreationExpressionSyntax).Initializer);
                    break;
                case SyntaxKind.ArrayCreationExpression:
                    /*Array*/
                    var array_expr = Get<ArrayCreationExpressionSyntax>(collectionSyntax);
                    initializer    = Get<InitializerExpressionSyntax>(array_expr.Initializer);
                    var array_type = Get<PredefinedTypeSyntax>(array_expr.Type.ElementType);
                    ThrowIf(!array_type.Keyword.IsKind(element_type_kind), "Invalid collection element type", syntaxNode: array_type);
                    break;
                default:
                    throw new LambdaDSLSyntaxErrorException(collectionSyntax);
            }

            foreach (var element in initializer.Expressions)
            {
                switch (element)
                {
                    case LiteralExpressionSyntax literal:
                    ThrowIf(!literal.IsKind(element_literal_kind), "Invalid collection element type", literal);
                    yield return (T)(dynamic)literal.Token.Value;
                    break;
                    case PrefixUnaryExpressionSyntax puliteral:
                    ThrowIf(!puliteral.IsKind(SyntaxKind.UnaryMinusExpression), "Invalid unary expression", puliteral);
                    ThrowIf(!puliteral.OperatorToken.IsKind(SyntaxKind.MinusToken), "Invalid unary prefix operator", puliteral);
                    ThrowIf(!puliteral.Operand.IsKind(element_literal_kind), "Invalid unary operand", puliteral);
                    yield return (T)(dynamic)Get<LiteralExpressionSyntax>(puliteral.Operand).Token.Value;
                    break;
                    default:
                    throw new LambdaDSLSyntaxErrorException("Invalid collection element type", collectionSyntax);
                }
            }
            yield break;
        }

        private static Expression<Func<ICellAccessor, Action>> ConstructVisitNodeAction(ExpressionSyntax traverseAction)
        {
            /***********************************************
             * Syntax: (VISITNODE)
             *
             * 1. VisitNode(FanoutSearch.Action action, IEnumerable<string> select = null)
             * 2. VisitNode(Expression<Func<ICellAccessor, FanoutSearch.Action>> action, IEnumerable<string> select = null)
             * 
             * The select part is handled by the caller.
             ***********************************************/

            var action_expr                             = TryGet<MemberAccessExpressionSyntax>(traverseAction);
            var lambda_expression                       = TryGet<LambdaExpressionSyntax>(traverseAction);
            Expression<Func<ICellAccessor, Action>> ret = null;
            if (action_expr != null)
            {
                // Action enum
                var id_expr = Get<IdentifierNameSyntax>(action_expr.Expression);
                if (id_expr.ToString() != s_LIKQ_Action) goto throw_badtype;

                Action result_action;
                ThrowIf(!Enum.TryParse(action_expr.Name.ToString(), out result_action), "Invalid traverse action", action_expr);

                return ExpressionBuilder.WrapAction(result_action);
            }
            else
            {
                if (lambda_expression == null) goto throw_badtype;

                // FanoutSearch.Action is ambiguous with with System.Action
                var action_visitor = new FanoutSearchActionRewritter();
                lambda_expression = action_visitor.Visit(lambda_expression) as LambdaExpressionSyntax;

                ScriptOptions scriptOptions = ScriptOptions.Default;
                var mscorlib = typeof(System.Object).Assembly;
                var systemCore = typeof(System.Linq.Enumerable).Assembly;
                var expression = typeof(Expression).Assembly;
                var fanout     = typeof(FanoutSearchModule).Assembly;
                var trinity    = typeof(Trinity.Global).Assembly;

                scriptOptions = scriptOptions.AddReferences(mscorlib, systemCore, expression, fanout, trinity);

                scriptOptions = scriptOptions.AddImports(
                    "System",
                    "System.Linq",
                    "System.Linq.Expressions",
                    "System.Collections.Generic",
                    "FanoutSearch",
                    "FanoutSearch.LIKQ",
                    "Trinity",
                    "Trinity.Storage");

                try
                {
                    //  Allocate a cancellation token source, which signals after our timeout setting (if we do have timeout setting)
                    CancellationToken cancel_token = default(CancellationToken);
                    if (FanoutSearchModule._QueryTimeoutEnabled())
                    {
                        checked
                        {
                            cancel_token = new CancellationTokenSource((int)FanoutSearchModule.GetQueryTimeout()).Token;
                        }
                    }
                    //  It is guaranteed that the lambda_expression is really a lambda.
                    //  Evaluating a lambda and expecting an expression tree to be obtained now.
                    using (var eval_task = CSharpScript.EvaluateAsync<Expression<Func<ICellAccessor, Action>>>(lambda_expression.ToString(), scriptOptions, cancellationToken: cancel_token))
                    {
                        eval_task.Wait(cancel_token);
                        ret = eval_task.Result;
                    }
                }
                catch (ArithmeticException) { /* that's a fault not an error */ throw; }
                catch { /*swallow roslyn scripting engine exceptions.*/ }

                ThrowIf(null == ret, "Invalid lambda expression.", traverseAction);
                return ret;
            }

            throw_badtype:
            ThrowIf(true, "Expecting an argument of type FanoutSearch.Action, or a lambda expression.", traverseAction);
            return null;//not going to happen
        }

        private static List<string> ConstructFollowEdgeSet(FSDescCallchainElement followEdge)
        {
            /***********************************************
             * Syntax:
             *
             *   FollowEdge(params string[] edgeTypes)
             ***********************************************/
            var ret = new List<string>();
            if (followEdge == null) return ret;

            foreach (var arg in followEdge.Arguments)
            {
                var edge = Get<LiteralExpressionSyntax>(arg.Expression, "Expecting a string");
                ThrowIf(!edge.IsKind(SyntaxKind.StringLiteralExpression), "Expecting a string", arg);
                ret.Add(edge.Token.ValueText);
            }

            // * means wildcard.
            if (ret.Any(_ => _ == "*")) { ret.Clear(); }
            return ret;
        }

        private static FanoutSearchDescriptor ConstructFanoutSearchOrigin(FSDescCallchainElement origin)
        {
            /***********************************************
             * Syntax:
             *
             * 1. StartFrom(long cellid, IEnumerable<string> select = null)
             * 2. StartFrom(IEnumerable<long> cellid, IEnumerable<string> select = null)
             * 3. StartFrom(string queryObject, IEnumerable<string> select = null)
             ***********************************************/

            ThrowIf(origin.Method != s_LIKQ_StartFrom, "Expecting " + s_LIKQ_StartFrom, origin.SyntaxNode);
            FanoutSearchDescriptor fs_desc = new FanoutSearchDescriptor("");
            fs_desc.m_origin_query = null;
            ThrowIf(origin.Arguments.Count < 1 || origin.Arguments.Count > 2, "Expecting 1 or 2 arguments", origin.SyntaxNode);

            ExpressionSyntax origin_cell_query = origin.Arguments[0].Expression;
            ExpressionSyntax selection         = origin.Arguments.Count < 2 ? null : origin.Arguments[1].Expression;

            switch (origin_cell_query.Kind())
            {
                case SyntaxKind.ObjectCreationExpression:
                case SyntaxKind.ImplicitArrayCreationExpression:
                case SyntaxKind.ArrayCreationExpression:
                    fs_desc.m_origin = ConstructCollection<long>(origin_cell_query).ToList();
                    break;
                case SyntaxKind.NumericLiteralExpression:
                case SyntaxKind.UnaryMinusExpression:
                case SyntaxKind.UnaryPlusExpression:
                    {
                        long cell_id = 0;
                        var parse_result = long.TryParse(origin_cell_query.ToString(), out cell_id);
                        ThrowIf(!parse_result, "Expecting a cell id", origin_cell_query);
                        fs_desc.m_origin = new List<long> { cell_id };
                        break;
                    }
                case SyntaxKind.StringLiteralExpression:
                    fs_desc.m_origin_query = (origin_cell_query as LiteralExpressionSyntax).Token.ValueText;
                    break;
                default:
                    throw new LambdaDSLSyntaxErrorException("Invalid starting node argument", origin_cell_query);
            }

            if (selection != null)
            {
                fs_desc.m_selectFields.Add(ConstructCollection<string>(selection).ToList());
            }

            return fs_desc;
        }

        private static InvocationExpressionSyntax GetFanoutSearchDescriptorExpression(string expr)
        {
            var tree               = Get<SyntaxTree>(CSharpSyntaxTree.ParseText(c_code_header + expr + c_code_footer));
            var root               = Get<CompilationUnitSyntax>(tree.GetRoot());
            ThrowIf(root.ChildNodes().Count() != 1, null, root);
            var class_declaration  = Get<ClassDeclarationSyntax>(root.ChildNodes().First());
            ThrowIf(class_declaration.ChildNodes().Count() != 1, null, class_declaration);
            var method_declaration = Get<MethodDeclarationSyntax>(class_declaration.Members.FirstOrDefault());
            ThrowIf(method_declaration.ExpressionBody != null, null, method_declaration);
            var method_body        = Get<BlockSyntax>(method_declaration.Body);
            ThrowIf(method_body.Statements.Count != 1, "The query string contains too many statements.", method_body);
            var fs_desc_stmt       = Get<StatementSyntax>(method_body.Statements.First());
            ThrowIf(fs_desc_stmt.ChildNodes().Count() != 1, "The query string contains too many expressions.", fs_desc_stmt);
            var invocation         = Get<InvocationExpressionSyntax>(fs_desc_stmt.ChildNodes().First());

            return invocation;

        }

        public static FanoutSearchDescriptor Evaluate(string expr)
        {
            try
            {
                var fs_invocation_expr = GetFanoutSearchDescriptorExpression(expr);

                //if we could use LIKQ to match such patterns...
                var fs_callchain = GetFanoutSearchDescriptorCallChain(fs_invocation_expr);

                var fs_desc = ConstructFanoutSearchDescriptor(fs_callchain);
                var checker = new TraverseActionSecurityChecker();

                fs_desc.m_traverseActions.ForEach(_ => checker.Visit(_));

                return fs_desc;
            }
            catch (LambdaDSLSyntaxErrorException ex)
            {
                throw new FanoutSearchQueryException(ex.Message);
            }
        }

        public static void SetDialect(string prolog, string startFrom, string visitNode, string followEdge, string action)
        {
            s_LIKQ_Prolog     = prolog;
            s_LIKQ_StartFrom  = startFrom;
            s_LIKQ_VisitNode  = visitNode;
            s_LIKQ_FollowEdge = followEdge;
            s_LIKQ_Action     = action;
        }
    }
}
