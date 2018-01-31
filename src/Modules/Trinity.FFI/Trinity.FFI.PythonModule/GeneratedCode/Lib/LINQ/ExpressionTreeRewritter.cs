#pragma warning disable 162,168,649,660,661,1522
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Trinity.TSL;
namespace CellAssembly.Linq
{
    internal class RewrittableWhereCaluseVisitor<T> : ExpressionVisitor
    {
        private  List<MethodCallExpression> m_where_clauses         = new List<MethodCallExpression>();
        private  Expression                 m_expression;
        private  bool                       m_inject_enumerator     = false;
        private  static Type                m_type                  = typeof(T);
        private  IQueryable<T>              m_queryable;
        private  Type                       m_constant_type; /* The type of target constant expression to be injected */
        internal RewrittableWhereCaluseVisitor(Expression expression)
        {
            m_expression = expression;
        }
        /// <summary>
        /// Visits the expression and get a list of rewrittable where clauses.
        /// We say a clause is rewrittable, if there's no select clause before
        /// it in the call chain.
        /// !It is then the inner most where clauses that get packed into this list.
        /// </summary>
        internal List<LambdaExpression> RewrittableWhereClauses
        {
            get
            {
                m_where_clauses.Clear();
                Visit(m_expression);
                return m_where_clauses.Select(m => (LambdaExpression)((UnaryExpression)(m.Arguments[1])).Operand).ToList();
            }
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (!m_inject_enumerator)
            {
                if (node.Method.Name == "Where")
                    m_where_clauses.Add(node);
                else if (node.Method.Name == "Select")
                {
                    /*
                     * Check for [Select] clause validity
                     */
                    if (((LambdaExpression)((UnaryExpression)(node.Arguments[1])).Operand).Body.Type == m_type)
                        throw new Exception("Cannot select into accessors.");
                    m_where_clauses.Clear();
                }
                return base.VisitMethodCall(node);
            }
            else
            {
                if (m_where_clauses.Contains(node))
                    return m_queryable.Expression;
                return base.VisitMethodCall(node);
            }
        }
        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (m_inject_enumerator)
            {
                if (node.Value.GetType() == m_constant_type)
                    return m_queryable.Expression;
                else
                    return base.VisitConstant(node);
            }
            else
            {
                return base.VisitConstant(node);
            }
        }
        internal Expression InjectEnumerator(Expression expression, IQueryable<T> queryable, Type constantInjectionTargetType)
        {
            m_inject_enumerator = true;
            m_queryable         = queryable;
            m_constant_type     = constantInjectionTargetType;
            return Visit(expression);
        }
    }
    internal class PredicateSubjectRewritter<T> : ExpressionVisitor
    {
        private static readonly Type                s_type                  = typeof(T);
        private                 ParameterExpression m_parameter_expression  = null;
        internal ParameterExpression Parameter
        {
            get
            {
                if (m_parameter_expression == null)
                    m_parameter_expression = Expression.Parameter(s_type, "accessor");
                return m_parameter_expression;
            }
        }
        protected override Expression VisitParameter(ParameterExpression node)
        {
            if (node.Type == s_type)
            {
                base.VisitParameter(node);
                return Parameter;
            }
            return base.VisitParameter(node);
        }
    }
    internal class IndexQueryTreeGenerator<T> : ExpressionVisitor
    {
        private  static Type                        s_type                  = typeof(T);
        private  static Type                        s_bool_type             = typeof(Boolean);
        private  static Type                        s_string_type           = typeof(string);
        private         IndexQueryTreeNode          m_current_node          = null;
        private         string                      m_cell_name;
        private         bool                        m_is_cell;
        private         Dictionary<string,string>   m_index_access_table;
        internal IndexQueryTreeNode QueryTree
        {
            get
            {
                return m_current_node;
            }
        }
        internal IndexQueryTreeGenerator(string cell_name, Dictionary<string, string> index_table, bool is_cell)
        {
            m_cell_name          = cell_name;
            m_index_access_table = index_table;
            m_is_cell            = is_cell;
        }
        IndexQueryTreeNode VisitChild(Expression child)
        {
            IndexQueryTreeNode original_node = m_current_node;
            Visit(child);
            if (m_current_node != original_node)
            {
                var ret        = m_current_node;
                m_current_node = original_node;
                return ret;
            }
            else
            {
                return null;
            }
        }
        void VisitChildren(List<IndexQueryTreeNode> childrenIndexTreeNodeList, params Expression[] children)
        {
            foreach (var child in children)
            {
                var tree_node = VisitChild(child);
                if (tree_node != null)
                    childrenIndexTreeNodeList.Add(tree_node);
            }
        }
        void BinaryBuildTree(IndexQueryTreeNode.NodeType type, BinaryExpression node)
        {
            IndexQueryTreeNode query_node    = new IndexQueryTreeNode { type = type, children = new List<IndexQueryTreeNode>() };
            VisitChildren(query_node.children, node.Left, node.Right);
            if (query_node.children.Count != 0)
                m_current_node = query_node;
        }
        void UnaryBuildTree(IndexQueryTreeNode.NodeType type, UnaryExpression node)
        {
            IndexQueryTreeNode query_node    = new IndexQueryTreeNode { type = type, children = new List<IndexQueryTreeNode>() };
            VisitChildren(query_node.children, node.Operand);
            if (query_node.children.Count != 0)
                m_current_node = query_node;
        }
        private bool _is_accessor_exp(Expression exp)
        {
            switch (exp.NodeType)
            {
                case ExpressionType.MemberAccess:
                    return _is_accessor_exp((exp as MemberExpression).Expression);
                case ExpressionType.Parameter:
                    /* In our query predicate, accessor is the parameter */
                    return true;
                case ExpressionType.Call:
                    {
                        var call_exp = exp as MethodCallExpression;
                        if (_is_accessor_exp(call_exp.Object))
                            return true;
                        foreach (var argument in call_exp.Arguments)
                            if (_is_accessor_exp(argument))
                                return true;
                        return false;
                    }
                default:
                    return false;
            }
        }
        private bool? _constant_boolean_exp_eval(Expression exp)
        {
            if (exp.NodeType == ExpressionType.MemberAccess)
            {
                if (_is_accessor_exp(exp))
                    return null;
                Expression<Func<bool>> lambda = Expression.Lambda<Func<bool>>(exp);
                Func<bool> func = lambda.Compile();
                return func();
            }
            else if (exp.NodeType == ExpressionType.Constant)
            {
                return (bool)(exp as ConstantExpression).Value;
            }
            else
                return null;
        }
        private void _equality_build_tree(BinaryExpression node, bool is_equal)
        {
            if (node.Left.Type != s_bool_type || node.Right.Type != s_bool_type)
                return;
            bool? constant                         = null;
            List<Expression>        exp            = new List<Expression>();
            Func<bool, bool, bool>  _equal_helper  = (lhs, rhs) => is_equal? lhs == rhs : lhs != rhs;
            bool? lconstant = _constant_boolean_exp_eval(node.Left);
            bool? rconstant = _constant_boolean_exp_eval(node.Right);
            if (lconstant.HasValue)
                constant = lconstant;
            else
                exp.Add(node.Left);
            if (rconstant.HasValue)
                constant = rconstant;
            else
                exp.Add(node.Right);
            if (lconstant.HasValue && rconstant.HasValue)
            {
                m_current_node = new IndexQueryTreeNode(
                    _equal_helper(lconstant.Value, rconstant.Value)?
                        IndexQueryTreeNode.NodeType.UNIVERSE:IndexQueryTreeNode.NodeType.EMPTY);
                return;
            }
            IndexQueryTreeNode query_node;
            IndexQueryTreeNode child_node;
            if (constant.HasValue)
            {
                child_node = VisitChild(exp[0]);
                if (child_node == null)
                {
                    m_current_node = new IndexQueryTreeNode(
                        _equal_helper(constant.Value, true)?
                        IndexQueryTreeNode.NodeType.UNIVERSE: IndexQueryTreeNode.NodeType.EMPTY);
                }
                else
                {
                    if (_equal_helper(constant.Value, true))
                    { /* keep m_current_node as is */ }
                    else
                    {
                        query_node = new IndexQueryTreeNode(IndexQueryTreeNode.NodeType.NOT) { children = new List<IndexQueryTreeNode> { child_node } };
                        m_current_node = query_node;
                    }
                }
            }
            else
            {
                query_node = new IndexQueryTreeNode(
                    _equal_helper(true, true)? IndexQueryTreeNode.NodeType.XNOR:IndexQueryTreeNode.NodeType.XOR) { children = new List<IndexQueryTreeNode>() };
                VisitChildren(query_node.children, node.Left, node.Right);
                switch (query_node.children.Count)
                {
                    case 0:
                    case 1:
                        /* no index query actions, as we cannot analyse the semantics */
                        break;
                    case 2:
                        /* we have query actions on both sides */
                        m_current_node = query_node;
                        break;
                    default:
                        throw new Exception("Internal error T5010");
                }
            }
        }
        private void NotEqualBuildTree(BinaryExpression node)
        {
            _equality_build_tree(node, is_equal: false);
        }
        private void EqualBuildTree(BinaryExpression node)
        {
            _equality_build_tree(node, is_equal: true);
        }
        #region Overridden expression tree visiting methods
        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.AndAlso:
                    BinaryBuildTree(IndexQueryTreeNode.NodeType.AND, node);
                    break;
                case ExpressionType.OrElse:
                    BinaryBuildTree(IndexQueryTreeNode.NodeType.OR, node);
                    break;
                case ExpressionType.Equal:
                    EqualBuildTree(node);
                    break;
                case ExpressionType.NotEqual:
                    NotEqualBuildTree(node);
                    break;
            }
            return node;
        }
        protected override Expression VisitUnary(UnaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Not:
                    UnaryBuildTree(IndexQueryTreeNode.NodeType.NOT, node);
                    break;
            }
            return node;
        }
        protected override Expression VisitMember(MemberExpression node)
        {
            return base.VisitMember(node);
        }
        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            var     call_object         = (node.Object) as MemberExpression;
            if (call_object == null)
                goto method_call_cleanup;
            var     member_string       = _get_member_string(call_object);
            string  index_access_method;
            if (m_index_access_table.TryGetValue(member_string, out index_access_method) &&
                index_access_method == node.Method.Name)
            {
                if (node.Arguments.Count != 1)
                    goto method_call_cleanup;
                var    arg             = node.Arguments[0];
                string query_string    = null;
                if (arg.NodeType == ExpressionType.Constant)
                    query_string = (string)(arg as ConstantExpression).Value;
                else
                {
                    query_string = _evaluate_expression<string>(arg);
                    /* We have to update the expression to avoid re-entering the expression */
                    node = node.Update(
                        node.Object,
                        new List<Expression> { Expression.Constant(query_string, s_string_type) });
                }
                if (query_string == null)
                    goto method_call_cleanup;
                /* 
                 * @note    Gotcha. It's an index query method call!  
                 */
                m_current_node             = new IndexQueryTreeNode(IndexQueryTreeNode.NodeType.QUERY);
                m_current_node.queryTarget = member_string;
                m_current_node.queryString = query_string;
            }
            /*
             * @note    We don't have to go deeper into the rabbit hole,
             *          as we cannot evaluate the method call.
             */
        method_call_cleanup:
            return node;
        }
        #endregion
        private VType _evaluate_expression<VType>(Expression exp)
        {
            Expression<Func<VType>> lambda = Expression.Lambda<Func<VType>>(exp);
            return lambda.Compile()();
        }
        private string _get_member_string(MemberExpression exp)
        {
            var str = exp.ToString();
            if (str.StartsWith("accessor.", StringComparison.Ordinal))
            {
                str = m_cell_name + str.Substring("accessor".Length);
                return str;
            }
            else
                return "";
        }
    }
}

#pragma warning restore 162,168,649,660,661,1522
