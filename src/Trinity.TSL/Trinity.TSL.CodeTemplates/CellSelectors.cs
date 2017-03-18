#pragma warning disable 0162 // disable the "unreachable code" warning
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

/*MAP_VAR("t_Namespace", "Trinity::Codegen::GetNamespace()")*/
using t_Namespace.Linq;
namespace t_Namespace
{
    [TARGET("NTSL")]
    [MAP_LIST("t_cell", "node->cellList")]
    [MAP_VAR("t_cell_name", "name")]
    [FOREACH]
    #region Internal
    /**
     * <summary>
     * Accepts transformation from t_cell_name_Accessor to T.
     * </summary>
     */
    internal class t_cell_name_Accessor_local_projector<T> : __meta, IQueryable<T>
    {
        private         Expression                                   query_expression;
        private         t_cell_name_Accessor_local_query_provider    query_provider;

        internal t_cell_name_Accessor_local_projector(t_cell_name_Accessor_local_query_provider provider, Expression expression)
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
     * Accepts transformation from t_cell_name to T.
     */
    internal class t_cell_name_local_projector<T> : __meta, IQueryable<T>
    {
        private         Expression                                   query_expression;
        private         t_cell_name_local_query_provider             query_provider;

        internal t_cell_name_local_projector(t_cell_name_local_query_provider provider, Expression expression)
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

    internal class t_cell_name_AccessorEnumerable : IEnumerable<t_cell_name_Accessor>
    {
        private     LocalMemoryStorage              m_storage;
        private     HashSet<long>                   m_filter_set;
        private     bool                            m_is_positive_filtering;
        private     Func<t_cell_name_Accessor,bool> m_filter_predicate;

        internal t_cell_name_AccessorEnumerable(LocalMemoryStorage storage)
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

        public IEnumerator<t_cell_name_Accessor> GetEnumerator()
        {
            if (m_filter_set == null)
            {
                if (m_filter_predicate == null)
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.t_cell_name)
                        {
                            var accessor = t_cell_name_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return accessor;
                            accessor.Dispose();
                        }
                    }
                else
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.t_cell_name)
                        {
                            var accessor = t_cell_name_Accessor.AllocIterativeAccessor(cellInfo);
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
                        using (var accessor = m_storage.Uset_cell_name(cellID))
                        {
                            yield return accessor;
                        }
                    }
                else
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.Uset_cell_name(cellID))
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
            m_filter_predicate = Expression.Lambda<Func<t_cell_name_Accessor, bool>>(
                aggregated_where_clause,
                parameter
                ).Compile();
        }
    }

    internal class t_cell_name_Enumerable : IEnumerable<t_cell_name>
    {
        private     LocalMemoryStorage              m_storage;
        private     HashSet<long>                   m_filter_set;
        private     bool                            m_is_positive_filtering;
        private     Func<t_cell_name,bool>          m_filter_predicate;
        private     static Type                     m_cell_type = typeof(t_cell_name);

        internal t_cell_name_Enumerable(LocalMemoryStorage storage)
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

        public IEnumerator<t_cell_name> GetEnumerator()
        {
            if (m_filter_set == null)
            {
                if (m_filter_predicate == null)
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.t_cell_name)
                        {
                            var accessor = t_cell_name_Accessor.AllocIterativeAccessor(cellInfo);
                            yield return accessor;
                            accessor.Dispose();
                        }
                    }
                else
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.t_cell_name)
                        {
                            var accessor = t_cell_name_Accessor.AllocIterativeAccessor(cellInfo);
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
                        using (var accessor = m_storage.Uset_cell_name(cellID))
                        {
                            yield return accessor;
                        }
                    }
                else
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.Uset_cell_name(cellID))
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
            m_filter_predicate = Expression.Lambda<Func<t_cell_name, bool>>(
                aggregated_where_clause,
                parameter
                ).Compile();
        }
    }

    internal class t_cell_name_Accessor_local_query_provider : __meta, IQueryProvider
    {
        private static  Type                             s_accessor_type    = typeof(t_cell_name_Accessor);
        private static  Type                             s_cell_type        = typeof(t_cell_name);
        private static  Type                             s_ienumerable_type = typeof(IEnumerable<>);
        private         t_cell_name_AccessorEnumerable   m_accessor_enumerable;

        internal t_cell_name_Accessor_local_query_provider(LocalMemoryStorage storage)
        {
            m_accessor_enumerable = new t_cell_name_AccessorEnumerable(storage);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == s_accessor_type)
            {
                return (IQueryable<TElement>)new t_cell_name_Accessor_local_selector(this, expression);
            }
            else
            {
                return new t_cell_name_Accessor_local_projector<TElement>(this, expression);
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var  visitor              = new RewrittableWhereCaluseVisitor<t_cell_name_Accessor>(expression);
            var  where_clauses        = visitor.RewrittableWhereClauses;
            var  queryable            = m_accessor_enumerable.AsQueryable<t_cell_name_Accessor>();
            var  trimmed_expression   = visitor.InjectEnumerator(expression, queryable, typeof(t_cell_name_Accessor_local_selector));

            if (where_clauses.Count != 0)
            {
                var subject_rewritter           = new PredicateSubjectRewritter<t_cell_name_Accessor>();
                Expression aggregated_predicate = subject_rewritter.Visit(where_clauses.First().Body);

                foreach (var where_clause in where_clauses.Skip(1))
                {
                    Expression predicate = where_clause.Body;
                    aggregated_predicate = Expression.AndAlso(aggregated_predicate, subject_rewritter.Visit(predicate));
                }

                IndexQueryTreeGenerator<t_cell_name_Accessor> query_tree_gen       = new IndexQueryTreeGenerator<t_cell_name_Accessor>("t_cell_name", Index.s_AccessorSubstringIndexAccessMethod, is_cell: false);
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
                // Nothing left in the expression tree.
                return (TResult)m_accessor_enumerable.GetEnumerator();
            }

            // We get IQueryable<ElementType> from trimmed_expression,
            // and we have to wrap it to IEnumerator<ElementType>

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

    internal class t_cell_name_local_query_provider : __meta, IQueryProvider
    {
        private static  Type                             s_cell_type        = typeof(t_cell_name);
        private static  Type                             s_ienumerable_type = typeof(IEnumerable<>);
        private         t_cell_name_Enumerable           s_cell_enumerable;
        internal t_cell_name_local_query_provider(LocalMemoryStorage storage)
        {
            s_cell_enumerable = new t_cell_name_Enumerable(storage);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == s_cell_type)
            {
                return (IQueryable<TElement>)new t_cell_name_local_selector(this, expression);
            }
            else
            {
                return new t_cell_name_local_projector<TElement>(this, expression);
            }
        }

        public TResult Execute<TResult>(Expression expression)
        {
            var  visitor              = new RewrittableWhereCaluseVisitor<t_cell_name>(expression);
            var  where_clauses        = visitor.RewrittableWhereClauses;
            var  queryable            = s_cell_enumerable.AsQueryable<t_cell_name>();
            var  trimmed_expression   = visitor.InjectEnumerator(expression, queryable, typeof(t_cell_name_local_selector));

            if (where_clauses.Count != 0)
            {
                var subject_rewritter           = new PredicateSubjectRewritter<t_cell_name>();
                Expression aggregated_predicate = subject_rewritter.Visit(where_clauses.First().Body);

                foreach (var where_clause in where_clauses.Skip(1))
                {
                    Expression predicate = where_clause.Body;
                    aggregated_predicate = Expression.AndAlso(aggregated_predicate, subject_rewritter.Visit(predicate));
                }

                IndexQueryTreeGenerator<t_cell_name> query_tree_gen       = new IndexQueryTreeGenerator<t_cell_name>("t_cell_name", Index.s_CellSubstringIndexAccessMethod, is_cell: true);
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
                // Nothing left in the expression tree.
                return (TResult)s_cell_enumerable.GetEnumerator();
            }

            // We get IQueryable<ElementType> from trimmed_expression,
            // and we have to wrap it to IEnumerator<ElementType>

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


    #endregion//Internal

    #region Public
    /// <summary>
    /// Implements System.Linq.IQueryable{t_cell_name_Accessor} and accepts LINQ
    /// queries on <see cref="Trinity.Global.LocalStorage"/>.
    /// </summary>
    public class t_cell_name_Accessor_local_selector : __meta, IQueryable<t_cell_name_Accessor>
    {
        private         Expression                                   query_expression;
        private         t_cell_name_Accessor_local_query_provider    query_provider;

        private t_cell_name_Accessor_local_selector() { /* nobody should reach this method */ }

        internal t_cell_name_Accessor_local_selector(Trinity.Storage.LocalMemoryStorage storage)
        {
            this.query_expression              = Expression.Constant(this);
            this.query_provider                = new t_cell_name_Accessor_local_query_provider(storage);
        }

        internal unsafe t_cell_name_Accessor_local_selector(t_cell_name_Accessor_local_query_provider query_provider, Expression query_expression)
        {
            this.query_expression              = query_expression;
            this.query_provider                = query_provider;
        }

        #region IQueryable<CellAccessor> interfaces
        public IEnumerator<t_cell_name_Accessor> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<t_cell_name_Accessor>>(query_expression);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(t_cell_name_Accessor); }
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
        public PLINQWrapper<t_cell_name_Accessor> AsParallel()
        {
            return new PLINQWrapper<t_cell_name_Accessor>(this);
        }

        #endregion
    }

    /// <summary>
    /// Implements System.Linq.IQueryable{t_cell_name} and accepts LINQ
    /// queries on <see cref="Trinity.Global.LocalStorage"/>.
    /// </summary>
    public class t_cell_name_local_selector : __meta, IQueryable<t_cell_name>, IOrderedQueryable<t_cell_name>
    {
        private         Expression                                   query_expression;
        private         t_cell_name_local_query_provider             query_provider;

        private t_cell_name_local_selector() { /* nobody should reach this method */ }

        internal t_cell_name_local_selector(Trinity.Storage.LocalMemoryStorage storage)
        {
            this.query_expression              = Expression.Constant(this);
            this.query_provider                = new t_cell_name_local_query_provider(storage);
        }

        internal unsafe t_cell_name_local_selector(t_cell_name_local_query_provider query_provider, Expression query_expression)
        {
            this.query_expression              = query_expression;
            this.query_provider                = query_provider;
        }

        #region IQueryable<Cell> interfaces
        public IEnumerator<t_cell_name> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<t_cell_name>>(query_expression);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator<t_cell_name>)this.GetEnumerator();
        }

        public Type ElementType
        {
            get { return typeof(t_cell_name); }
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
    #endregion//Public
    [END]

    public static class LocalStorageCellSelectorExternsion
    {
        [FOREACH]
        /// <summary>
        /// Enumerates all the t_cell_name within the local memory storage.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>All the t_cell_name within the local memory storage.</returns>
        public static t_cell_name_local_selector t_cell_name_Selector(this LocalMemoryStorage storage)
        {
            return new t_cell_name_local_selector(storage);
        }

        /// <summary>
        /// Enumerates all the t_cell_name_Accessor within the local memory storage.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>All the t_cell_name_Accessor within the local memory storage.</returns>
        public static t_cell_name_Accessor_local_selector t_cell_name_Accessor_Selector(this LocalMemoryStorage storage)
        {
            return new t_cell_name_Accessor_local_selector(storage);
        }
        /**/
    }
}

