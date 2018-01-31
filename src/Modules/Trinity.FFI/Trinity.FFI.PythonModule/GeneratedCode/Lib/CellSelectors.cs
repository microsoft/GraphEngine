#pragma warning disable 162,168,649,660,661,1522

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
using CellAssembly.Linq;
namespace CellAssembly
{
    
    #region Internal
    /**
     * <summary>
     * Accepts transformation from C1_Accessor to T.
     * </summary>
     */
    internal class C1_Accessor_local_projector<T> : IQueryable<T>
    {
        private         Expression                                   query_expression;
        private         C1_Accessor_local_query_provider    query_provider;
        internal C1_Accessor_local_projector(C1_Accessor_local_query_provider provider, Expression expression)
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
     * Accepts transformation from C1 to T.
     */
    internal class C1_local_projector<T> : IQueryable<T>
    {
        private         Expression                                   query_expression;
        private         C1_local_query_provider             query_provider;
        internal C1_local_projector(C1_local_query_provider provider, Expression expression)
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
    internal class C1_AccessorEnumerable : IEnumerable<C1_Accessor>
    {
        private     LocalMemoryStorage              m_storage;
        private     HashSet<long>                   m_filter_set;
        private     bool                            m_is_positive_filtering;
        private     Func<C1_Accessor,bool> m_filter_predicate;
        internal C1_AccessorEnumerable(LocalMemoryStorage storage)
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
        public IEnumerator<C1_Accessor> GetEnumerator()
        {
            if (m_filter_set == null)
            {
                if (m_filter_predicate == null)
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.C1)
                        {
                            var accessor = C1_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return accessor;
                            accessor.Dispose();
                        }
                    }
                else
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.C1)
                        {
                            var accessor = C1_Accessor.AllocIterativeAccessor(cellInfo);
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
                        using (var accessor = m_storage.UseC1(cellID))
                        {
                            yield return accessor;
                        }
                    }
                else
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.UseC1(cellID))
                        {
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                        }
                    }
            }
            else
            {
                throw new NotImplementedException("Negative filtering not supported.");
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        internal void SetPredicate(Expression aggregated_where_clause, ParameterExpression parameter)
        {
            m_filter_predicate = Expression.Lambda<Func<C1_Accessor, bool>>(
                aggregated_where_clause,
                parameter
                ).Compile();
        }
    }
    internal class C1_Enumerable : IEnumerable<C1>
    {
        private     LocalMemoryStorage              m_storage;
        private     HashSet<long>                   m_filter_set;
        private     bool                            m_is_positive_filtering;
        private     Func<C1,bool>          m_filter_predicate;
        private     static Type                     m_cell_type = typeof(C1);
        internal C1_Enumerable(LocalMemoryStorage storage)
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
        public IEnumerator<C1> GetEnumerator()
        {
            if (m_filter_set == null)
            {
                if (m_filter_predicate == null)
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.C1)
                        {
                            var accessor = C1_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return accessor;
                            accessor.Dispose();
                        }
                    }
                else
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.C1)
                        {
                            var accessor = C1_Accessor.AllocIterativeAccessor(cellInfo);
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
                        using (var accessor = m_storage.UseC1(cellID))
                        {
                            yield return accessor;
                        }
                    }
                else
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.UseC1(cellID))
                        {
                            if (m_filter_predicate(accessor))
                                yield return accessor;
                        }
                    }
            }
            else
            {
                throw new NotImplementedException("Negative filtering not supported.");
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        internal void SetPredicate(Expression aggregated_where_clause, ParameterExpression parameter)
        {
            m_filter_predicate = Expression.Lambda<Func<C1, bool>>(
                aggregated_where_clause,
                parameter
                ).Compile();
        }
    }
    internal class C1_Accessor_local_query_provider : IQueryProvider
    {
        private static  Type                             s_accessor_type    = typeof(C1_Accessor);
        private static  Type                             s_cell_type        = typeof(C1);
        private static  Type                             s_ienumerable_type = typeof(IEnumerable<>);
        private         C1_AccessorEnumerable   m_accessor_enumerable;
        internal C1_Accessor_local_query_provider(LocalMemoryStorage storage)
        {
            m_accessor_enumerable = new C1_AccessorEnumerable(storage);
        }
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == s_accessor_type)
            {
                return (IQueryable<TElement>)new C1_Accessor_local_selector(this, expression);
            }
            else
            {
                return new C1_Accessor_local_projector<TElement>(this, expression);
            }
        }
        public TResult Execute<TResult>(Expression expression)
        {
            var  visitor              = new RewrittableWhereCaluseVisitor<C1_Accessor>(expression);
            var  where_clauses        = visitor.RewrittableWhereClauses;
            var  queryable            = m_accessor_enumerable.AsQueryable<C1_Accessor>();
            var  trimmed_expression   = visitor.InjectEnumerator(expression, queryable, typeof(C1_Accessor_local_selector));
            if (where_clauses.Count != 0)
            {
                var subject_rewritter           = new PredicateSubjectRewritter<C1_Accessor>();
                Expression aggregated_predicate = subject_rewritter.Visit(where_clauses.First().Body);
                foreach (var where_clause in where_clauses.Skip(1))
                {
                    Expression predicate = where_clause.Body;
                    aggregated_predicate = Expression.AndAlso(aggregated_predicate, subject_rewritter.Visit(predicate));
                }
                IndexQueryTreeGenerator<C1_Accessor> query_tree_gen       = new IndexQueryTreeGenerator<C1_Accessor>("C1", Index.s_AccessorSubstringIndexAccessMethod, is_cell: false);
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
            Type result_type          = typeof(TResult);
            bool result_is_enumerable = (result_type.GenericTypeArguments.Count() == 1);
            Type element_type         = result_is_enumerable ? result_type.GenericTypeArguments[0] : result_type;
            if (result_is_enumerable)
            {
                var  enumerator_type      = s_ienumerable_type.MakeGenericType(element_type);
                var  enumerator_extractor = Expression.Call(trimmed_expression, enumerator_type.GetMethod("GetEnumerator"));
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
    internal class C1_local_query_provider : IQueryProvider
    {
        private static  Type                             s_cell_type        = typeof(C1);
        private static  Type                             s_ienumerable_type = typeof(IEnumerable<>);
        private         C1_Enumerable           s_cell_enumerable;
        internal C1_local_query_provider(LocalMemoryStorage storage)
        {
            s_cell_enumerable = new C1_Enumerable(storage);
        }
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == s_cell_type)
            {
                return (IQueryable<TElement>)new C1_local_selector(this, expression);
            }
            else
            {
                return new C1_local_projector<TElement>(this, expression);
            }
        }
        public TResult Execute<TResult>(Expression expression)
        {
            var  visitor              = new RewrittableWhereCaluseVisitor<C1>(expression);
            var  where_clauses        = visitor.RewrittableWhereClauses;
            var  queryable            = s_cell_enumerable.AsQueryable<C1>();
            var  trimmed_expression   = visitor.InjectEnumerator(expression, queryable, typeof(C1_local_selector));
            if (where_clauses.Count != 0)
            {
                var subject_rewritter           = new PredicateSubjectRewritter<C1>();
                Expression aggregated_predicate = subject_rewritter.Visit(where_clauses.First().Body);
                foreach (var where_clause in where_clauses.Skip(1))
                {
                    Expression predicate = where_clause.Body;
                    aggregated_predicate = Expression.AndAlso(aggregated_predicate, subject_rewritter.Visit(predicate));
                }
                IndexQueryTreeGenerator<C1> query_tree_gen       = new IndexQueryTreeGenerator<C1>("C1", Index.s_CellSubstringIndexAccessMethod, is_cell: true);
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
            bool result_is_enumerable = (result_type.GenericTypeArguments.Count() == 1);
            Type element_type         = result_is_enumerable ? result_type.GenericTypeArguments[0] : result_type;
            if (result_is_enumerable)
            {
                var  enumerator_type      = s_ienumerable_type.MakeGenericType(element_type);
                var  enumerator_extractor = Expression.Call(trimmed_expression, enumerator_type.GetMethod("GetEnumerator"));
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
    #endregion
    #region Public
    /// <summary>
    /// Implements System.Linq.IQueryable{C1_Accessor} and accepts LINQ
    /// queries on <see cref="Trinity.Global.LocalStorage"/>.
    /// </summary>
    public class C1_Accessor_local_selector : IQueryable<C1_Accessor>
    {
        private         Expression                                   query_expression;
        private         C1_Accessor_local_query_provider    query_provider;
        private C1_Accessor_local_selector() { /* nobody should reach this method */ }
        internal C1_Accessor_local_selector(Trinity.Storage.LocalMemoryStorage storage)
        {
            this.query_expression              = Expression.Constant(this);
            this.query_provider                = new C1_Accessor_local_query_provider(storage);
        }
        internal unsafe C1_Accessor_local_selector(C1_Accessor_local_query_provider query_provider, Expression query_expression)
        {
            this.query_expression              = query_expression;
            this.query_provider                = query_provider;
        }
        #region IQueryable<CellAccessor> interfaces
        public IEnumerator<C1_Accessor> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<C1_Accessor>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(C1_Accessor); }
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
        public PLINQWrapper<C1_Accessor> AsParallel()
        {
            return new PLINQWrapper<C1_Accessor>(this);
        }
        #endregion
    }
    /// <summary>
    /// Implements System.Linq.IQueryable{C1} and accepts LINQ
    /// queries on <see cref="Trinity.Global.LocalStorage"/>.
    /// </summary>
    public class C1_local_selector : IQueryable<C1>, IOrderedQueryable<C1>
    {
        private         Expression                                   query_expression;
        private         C1_local_query_provider             query_provider;
        private C1_local_selector() { /* nobody should reach this method */ }
        internal C1_local_selector(Trinity.Storage.LocalMemoryStorage storage)
        {
            this.query_expression              = Expression.Constant(this);
            this.query_provider                = new C1_local_query_provider(storage);
        }
        internal unsafe C1_local_selector(C1_local_query_provider query_provider, Expression query_expression)
        {
            this.query_expression              = query_expression;
            this.query_provider                = query_provider;
        }
        #region IQueryable<Cell> interfaces
        public IEnumerator<C1> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<C1>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator<C1>)this.GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof(C1); }
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
    
    public static class LocalStorageCellSelectorExternsion
    {
        
        /// <summary>
        /// Enumerates all the C1 within the local memory storage.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>All the C1 within the local memory storage.</returns>
        public static C1_local_selector C1_Selector(this LocalMemoryStorage storage)
        {
            return new C1_local_selector(storage);
        }
        /// <summary>
        /// Enumerates all the C1_Accessor within the local memory storage.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>All the C1_Accessor within the local memory storage.</returns>
        public static C1_Accessor_local_selector C1_Accessor_Selector(this LocalMemoryStorage storage)
        {
            return new C1_Accessor_local_selector(storage);
        }
        
    }
}

#pragma warning restore 162,168,649,660,661,1522
