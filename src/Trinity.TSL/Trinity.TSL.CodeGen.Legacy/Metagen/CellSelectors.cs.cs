using System.Text;
using System.Collections.Generic;

namespace Trinity.TSL.CodeTemplates
{
    internal partial class SourceFiles
    {
        internal static string 
CellSelectors(
NTSL node)
        {
            StringBuilder source = new StringBuilder();
            
source.Append(@"
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Trinity;
using Trinity.TSL;
using Trinity.TSL.Lib;
using Trinity.Storage;
using System.Linq.Expressions;
using ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@".Linq;
namespace ");
source.Append(Codegen.GetString(Trinity::Codegen::GetNamespace()));
source.Append(@"
{
    ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
    #region Internal
    /**
     * <summary>
     * Accepts transformation from ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor to T.
     * </summary>
     */
    internal class ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_projector<T> : IQueryable<T>
    {
        private         Expression                                   query_expression;
        private         ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_query_provider    query_provider;
        internal ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_projector(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_query_provider provider, Expression expression)
        {
            this.query_expression              = expression;
            this.query_provider                = provider;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<T>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(T); }
        }
        public Expression Expression
        {
            get { return query_expression; }
        }
        public IQueryProvider Provider
        {
            get { return query_provider; }
        }
    }
    /**
     * Accepts transformation from ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@" to T.
     */
    internal class ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_projector<T> : IQueryable<T>
    {
        private         Expression                                   query_expression;
        private         ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_query_provider             query_provider;
        internal ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_projector(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_query_provider provider, Expression expression)
        {
            this.query_expression              = expression;
            this.query_provider                = provider;
        }
        public IEnumerator<T> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<T>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(T); }
        }
        public Expression Expression
        {
            get { return query_expression; }
        }
        public IQueryProvider Provider
        {
            get { return query_provider; }
        }
    }
    internal class ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_AccessorEnumerable : IEnumerable<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor>
    {
        private     LocalMemoryStorage              m_storage;
        private     HashSet<long>                   m_filter_set;
        private     bool                            m_is_positive_filtering;
        private     Func<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor,bool> m_filter_predicate;
        internal ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_AccessorEnumerable(LocalMemoryStorage storage)
        {
            this.m_storage     = storage;
            m_filter_set       = null;
            m_filter_predicate = null;
        }
        internal void SetPositiveFiltering(HashSet<long> set)
        {
            this.m_filter_set       = set;
            m_is_positive_filtering = true;
        }
        internal void SetNegativeFiltering(HashSet<long> set)
        {
            this.m_filter_set       = set;
            m_is_positive_filtering = false;
        }
        public IEnumerator<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor> GetEnumerator()
        {
            if (m_filter_set == null)
            {
                if (m_filter_predicate == null)
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@")
                        {
                            var accessor = ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return accessor;
                            accessor.Dispose();
                        }
                    }
                else
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@")
                        {
                            var accessor = ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor.AllocIterativeAccessor(cellInfo);
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                            accessor.Dispose();
                        }
                    }
            }
            else if (m_is_positive_filtering)
            {
                if (m_filter_predicate == null)
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.Use");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"(cellID))
                        {
                            yield return accessor;
                        }
                    }
                else
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.Use");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"(cellID))
                        {
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                        }
                    }
            }
            else
            {
                throw new NotImplementedException(""Negative filtering not supported."");
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        internal void SetPredicate(Expression aggregated_where_clause, ParameterExpression parameter)
        {
            m_filter_predicate = Expression.Lambda<Func<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor, bool>>(
                aggregated_where_clause,
                parameter
                ).Compile();
        }
    }
    internal class ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Enumerable : IEnumerable<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@">
    {
        private     LocalMemoryStorage              m_storage;
        private     HashSet<long>                   m_filter_set;
        private     bool                            m_is_positive_filtering;
        private     Func<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@",bool>          m_filter_predicate;
        private     static Type                     m_cell_type = typeof(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@");
        internal ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Enumerable(LocalMemoryStorage storage)
        {
            this.m_storage     = storage;
            m_filter_set       = null;
            m_filter_predicate = null;
        }
        internal void SetPositiveFiltering(HashSet<long> set)
        {
            this.m_filter_set       = set;
            m_is_positive_filtering = true;
        }
        internal void SetNegativeFiltering(HashSet<long> set)
        {
            this.m_filter_set       = set;
            m_is_positive_filtering = false;
        }
        public IEnumerator<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"> GetEnumerator()
        {
            if (m_filter_set == null)
            {
                if (m_filter_predicate == null)
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@")
                        {
                            var accessor = ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return accessor;
                            accessor.Dispose();
                        }
                    }
                else
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@")
                        {
                            var accessor = ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor.AllocIterativeAccessor(cellInfo);
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                            accessor.Dispose();
                        }
                    }
            }
            else if (m_is_positive_filtering)
            {
                if (m_filter_predicate == null)
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.Use");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"(cellID))
                        {
                            yield return accessor;
                        }
                    }
                else
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.Use");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"(cellID))
                        {
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                        }
                    }
            }
            else
            {
                throw new NotImplementedException(""Negative filtering not supported."");
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        internal void SetPredicate(Expression aggregated_where_clause, ParameterExpression parameter)
        {
            m_filter_predicate = Expression.Lambda<Func<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@", bool>>(
                aggregated_where_clause,
                parameter
                ).Compile();
        }
    }
    internal class ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_query_provider : IQueryProvider
    {
        private static  Type                             s_accessor_type    = typeof(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor);
        private static  Type                             s_cell_type        = typeof(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@");
        private static  Type                             s_ienumerable_type = typeof(IEnumerable<>);
        private         ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_AccessorEnumerable   m_accessor_enumerable;
        internal ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_query_provider(LocalMemoryStorage storage)
        {
            m_accessor_enumerable = new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_AccessorEnumerable(storage);
        }
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == s_accessor_type)
            {
                return (IQueryable<TElement>)new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_selector(this, expression);
            }
            else
            {
                return new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_projector<TElement>(this, expression);
            }
        }
        public TResult Execute<TResult>(Expression expression)
        {
            var  visitor              = new RewrittableWhereCaluseVisitor<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor>(expression);
            var  where_clauses        = visitor.RewrittableWhereClauses;
            var  queryable            = m_accessor_enumerable.AsQueryable<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor>();
            var  trimmed_expression   = visitor.InjectEnumerator(expression, queryable, typeof(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_selector));
            if (where_clauses.Count != 0)
            {
                var subject_rewritter           = new PredicateSubjectRewritter<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor>();
                Expression aggregated_predicate = subject_rewritter.Visit(where_clauses.First().Body);
                foreach (var where_clause in where_clauses.Skip(1))
                {
                    Expression predicate = where_clause.Body;
                    aggregated_predicate = Expression.AndAlso(aggregated_predicate, subject_rewritter.Visit(predicate));
                }
                IndexQueryTreeGenerator<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor> query_tree_gen       = new IndexQueryTreeGenerator<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor>(""");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@""", Index.s_AccessorSubstringIndexAccessMethod, is_cell: false);
                aggregated_predicate                                               = query_tree_gen.Visit(aggregated_predicate);
                var query_tree                                                     = query_tree_gen.QueryTree;
                if (query_tree != null)
                {
                    query_tree = query_tree.Optimize();
                    var query_tree_exec = new IndexQueryTreeExecutor(Index.s_AccessorSubstringQueryMethodTable, Index.s_AccessorSubstringWildcardQueryMethodTable);
                    m_accessor_enumerable.SetPositiveFiltering(query_tree_exec.Execute(query_tree));
                }
                m_accessor_enumerable.SetPredicate(aggregated_predicate, subject_rewritter.Parameter);
            }
            if (trimmed_expression.NodeType == ExpressionType.Constant)
            {
                return (TResult)m_accessor_enumerable.GetEnumerator();
            }
            Type res");
source.Append(@"ult_type          = typeof(TResult);
            bool result_is_enumerable = (result_type.GenericTypeArguments.Count() == 1);
            Type element_type         = result_is_enumerable ? result_type.GenericTypeArguments[0] : result_type;
            if (result_is_enumerable)
            {
                var  enumerator_type      = s_ienumerable_type.MakeGenericType(element_type);
                var  enumerator_extractor = Expression.Call(trimmed_expression, enumerator_type.GetMethod(""GetEnumerator""));
                var  lambda               = Expression.Lambda<Func<TResult>>(enumerator_extractor);
                var  func                 = (lambda).Compile();
                var  result               = func();
                return result;
            }
            else
            {
                var  lambda               = Expression.Lambda<Func<TResult>>(trimmed_expression);
                var  func                 = (lambda).Compile();
                var  result               =");
source.Append(@" func();
                return result;
            }
        }
        #region Not implemented
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }
        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    internal class ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_query_provider : IQueryProvider
    {
        private static  Type                             s_cell_type        = typeof(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@");
        private static  Type                             s_ienumerable_type = typeof(IEnumerable<>);
        private         ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Enumerable           s_cell_enumerable;
        internal ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_query_provider(LocalMemoryStorage storage)
        {
            s_cell_enumerable = new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Enumerable(storage);
        }
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == s_cell_type)
            {
                return (IQueryable<TElement>)new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_selector(this, expression);
            }
            else
            {
                return new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_projector<TElement>(this, expression);
            }
        }
        public TResult Execute<TResult>(Expression expression)
        {
            var  visitor              = new RewrittableWhereCaluseVisitor<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@">(expression);
            var  where_clauses        = visitor.RewrittableWhereClauses;
            var  queryable            = s_cell_enumerable.AsQueryable<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@">();
            var  trimmed_expression   = visitor.InjectEnumerator(expression, queryable, typeof(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_selector));
            if (where_clauses.Count != 0)
            {
                var subject_rewritter           = new PredicateSubjectRewritter<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@">();
                Expression aggregated_predicate = subject_rewritter.Visit(where_clauses.First().Body);
                foreach (var where_clause in where_clauses.Skip(1))
                {
                    Expression predicate = where_clause.Body;
                    aggregated_predicate = Expression.AndAlso(aggregated_predicate, subject_rewritter.Visit(predicate));
                }
                IndexQueryTreeGenerator<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"> query_tree_gen       = new IndexQueryTreeGenerator<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@">(""");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@""", Index.s_CellSubstringIndexAccessMethod, is_cell: true);
                aggregated_predicate                                      = query_tree_gen.Visit(aggregated_predicate);
                var query_tree                                            = query_tree_gen.QueryTree;
                if (query_tree != null)
                {
                    query_tree = query_tree.Optimize();
                    var query_tree_exec = new IndexQueryTreeExecutor(Index.s_CellSubstringQueryMethodTable, Index.s_CellSubstringWildcardQueryMethodTable);
                    s_cell_enumerable.SetPositiveFiltering(query_tree_exec.Execute(query_tree));
                }
                s_cell_enumerable.SetPredicate(aggregated_predicate, subject_rewritter.Parameter);
            }
            if (trimmed_expression.NodeType == ExpressionType.Constant)
            {
                return (TResult)s_cell_enumerable.GetEnumerator();
            }
            Type result_type          = typeof(TResult);
     ");
source.Append(@"       bool result_is_enumerable = (result_type.GenericTypeArguments.Count() == 1);
            Type element_type         = result_is_enumerable ? result_type.GenericTypeArguments[0] : result_type;
            if (result_is_enumerable)
            {
                var  enumerator_type      = s_ienumerable_type.MakeGenericType(element_type);
                var  enumerator_extractor = Expression.Call(trimmed_expression, enumerator_type.GetMethod(""GetEnumerator""));
                var  lambda               = Expression.Lambda<Func<TResult>>(enumerator_extractor);
                var  func                 = (lambda).Compile();
                var  result               = func();
                return result;
            }
            else
            {
                var  lambda               = Expression.Lambda<Func<TResult>>(trimmed_expression);
                var  func                 = (lambda).Compile();
                var  result               = func();
                return result;
 ");
source.Append(@"           }
        }
        #region Not implemented
        public IQueryable CreateQuery(Expression expression)
        {
            throw new NotImplementedException();
        }
        public object Execute(Expression expression)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
    #endregion
    #region Public
    /// <summary>
    /// Implements System.Linq.IQueryable{");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor} and accepts LINQ
    /// queries on <see cref=""Trinity.Global.LocalStorage""/>.
    /// </summary>
    public class ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_selector : IQueryable<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor>
    {
        private         Expression                                   query_expression;
        private         ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_query_provider    query_provider;
        private ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_selector() { /* nobody should reach this method */ }
        internal ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_selector(Trinity.Storage.LocalMemoryStorage storage)
        {
            this.query_expression              = Expression.Constant(this);
            this.query_provider                = new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_query_provider(storage);
        }
        internal unsafe ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_selector(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_query_provider query_provider, Expression query_expression)
        {
            this.query_expression              = query_expression;
            this.query_provider                = query_provider;
        }
        #region IQueryable<CellAccessor> interfaces
        public IEnumerator<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor); }
        }
        public Expression Expression
        {
            get { return this.query_expression; }
        }
        public IQueryProvider Provider
        {
            get { return query_provider; }
        }
        #endregion
        #region PLINQ Wrapper
        public PLINQWrapper<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor> AsParallel()
        {
            return new PLINQWrapper<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor>(this);
        }
        #endregion
    }
    /// <summary>
    /// Implements System.Linq.IQueryable{");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"} and accepts LINQ
    /// queries on <see cref=""Trinity.Global.LocalStorage""/>.
    /// </summary>
    public class ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_selector : IQueryable<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@">, IOrderedQueryable<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@">
    {
        private         Expression                                   query_expression;
        private         ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_query_provider             query_provider;
        private ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_selector() { /* nobody should reach this method */ }
        internal ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_selector(Trinity.Storage.LocalMemoryStorage storage)
        {
            this.query_expression              = Expression.Constant(this);
            this.query_provider                = new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_query_provider(storage);
        }
        internal unsafe ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_selector(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_query_provider query_provider, Expression query_expression)
        {
            this.query_expression              = query_expression;
            this.query_provider                = query_provider;
        }
        #region IQueryable<Cell> interfaces
        public IEnumerator<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@">>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator<");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@">)this.GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"); }
        }
        public Expression Expression
        {
            get { return this.query_expression; }
        }
        public IQueryProvider Provider
        {
            get { return query_provider; }
        }
        #endregion
    }
    #endregion
    ");
}
source.Append(@"
    public static class LocalStorageCellSelectorExternsion
    {
        ");
for (int iterator_1 = 0; iterator_1 < (node->cellList).Count;++iterator_1)
{
source.Append(@"
        /// <summary>
        /// Enumerates all the ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@" within the local memory storage.
        /// </summary>
        /// <param name=""storage"">A <see cref=""Trinity.Storage.LocalMemoryStorage""/> object.</param>
        /// <returns>All the ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@" within the local memory storage.</returns>
        public static ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_selector/*_*/");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Selector(this LocalMemoryStorage storage)
        {
            return new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_local_selector(storage);
        }
        /// <summary>
        /// Enumerates all the ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor within the local memory storage.
        /// </summary>
        /// <param name=""storage"">A <see cref=""Trinity.Storage.LocalMemoryStorage""/> object.</param>
        /// <returns>All the ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor within the local memory storage.</returns>
        public static ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_selector/*_*/");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_Selector(this LocalMemoryStorage storage)
        {
            return new ");
source.Append(Codegen.GetString((node->cellList)[iterator_1].name));
source.Append(@"_Accessor_local_selector(storage);
        }
        ");
}
source.Append(@"
    }
}
");

            return source.ToString();
        }
    }
}
