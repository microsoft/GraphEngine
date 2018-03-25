#include "common.h"
#include <string>
#include "SyntaxNode.h"

using std::string;

namespace Trinity
{
    namespace Codegen
    {
        string* 
CellSelectors(
NTSL* node)
        {
            string* source = new string();
            
source->append(R"::(
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
using )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(.Linq;
using Trinity.Storage.Transaction;
namespace )::");
source->append(Codegen::GetString(Trinity::Codegen::GetNamespace()));
source->append(R"::(
{
    )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
    #region Internal
    /**
     * <summary>
     * Accepts transformation from )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor to T.
     * </summary>
     */
    internal class )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_projector<T> : IQueryable<T>
    {
        private         Expression                                   query_expression;
        private         )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_query_provider    query_provider;
        internal )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_projector()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_query_provider provider, Expression expression)
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
     * Accepts transformation from )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::( to T.
     */
    internal class )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_projector<T> : IQueryable<T>
    {
        private         Expression                                   query_expression;
        private         )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_query_provider             query_provider;
        internal )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_projector()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_query_provider provider, Expression expression)
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
    internal class )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_AccessorEnumerable : IEnumerable<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor>
    {
        private     LocalMemoryStorage              m_storage;
        private     LocalTransactionContext         m_tx;
        private     HashSet<long>                   m_filter_set;
        private     bool                            m_is_positive_filtering;
        private     Func<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor,bool> m_filter_predicate;
        internal )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_AccessorEnumerable(LocalMemoryStorage storage, LocalTransactionContext tx)
        {
            this.m_storage     = storage;
            m_filter_set       = null;
            m_filter_predicate = null;
            m_tx               = tx;
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
        public IEnumerator<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor> GetEnumerator()
        {
            if (m_filter_set == null)
            {
                if (m_filter_predicate == null)
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::()
                        {
                            var accessor = )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor.AllocIterativeAccessor(cellInfo, m_tx);
                            yield return accessor;
                            accessor.Dispose();
                        }
                    }
                else
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::()
                        {
                            var accessor = )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor.AllocIterativeAccessor(cellInfo, m_tx);
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
                        using (var accessor = m_storage.Use)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::((cellID))
                        {
                            yield return accessor;
                        }
                    }
                else
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.Use)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::((cellID))
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
            m_filter_predicate = Expression.Lambda<Func<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor, bool>>(
                aggregated_where_clause,
                parameter
                ).Compile();
        }
    }
    internal class )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Enumerable : IEnumerable<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(>
    {
        private LocalMemoryStorage      m_storage;
        private HashSet<long>           m_filter_set;
        private bool                    m_is_positive_filtering;
        private Func<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(,bool>  m_filter_predicate;
        private static Type             m_cell_type = typeof()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::();
        private LocalTransactionContext m_tx;
        internal )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Enumerable(LocalMemoryStorage storage, LocalTransactionContext tx)
        {
            m_storage          = storage;
            m_filter_set       = null;
            m_filter_predicate = null;
            m_tx               = tx;
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
        public IEnumerator<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(> GetEnumerator()
        {
            if (m_filter_set == null)
            {
                if (m_filter_predicate == null)
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::()
                        {
                            var accessor = )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor.AllocIterativeAccessor(cellInfo, m_tx);
                            yield return accessor;
                            accessor.Dispose();
                        }
                    }
                else
                    foreach (var cellInfo in m_storage)
                    {
                        if (cellInfo.CellType == (ushort)CellType.)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::()
                        {
                            var accessor = )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor.AllocIterativeAccessor(cellInfo, m_tx);
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
                        using (var accessor = m_storage.Use)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::((cellID))
                        {
                            yield return accessor;
                        }
                    }
                else
                    foreach (var cellID in m_filter_set)
                    {
                        using (var accessor = m_storage.Use)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::((cellID))
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
            m_filter_predicate = Expression.Lambda<Func<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(, bool>>(
                aggregated_where_clause,
                parameter
                ).Compile();
        }
    }
    internal class )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_query_provider : IQueryProvider
    {
        private static  Type                             s_accessor_type    = typeof()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor);
        private static  Type                             s_cell_type        = typeof()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::();
        private static  Type                             s_ienumerable_type = typeof(IEnumerable<>);
        private         )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_AccessorEnumerable   m_accessor_enumerable;
        internal )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_query_provider(LocalMemoryStorage storage, LocalTransactionContext tx)
        {
            m_accessor_enumerable = new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_AccessorEnumerable(storage, tx);
        }
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == s_accessor_type)
            {
                return (IQueryable<TElement>)new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_selector(this, expression);
            }
            else
            {
                return new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_projector<TElement>(this, expression);
            }
        }
        public TResult Execute<TResult>(Expression expression)
        {
            var  visitor              = new RewrittableWhereCaluseVisitor<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor>(expression);
            var  where_clauses        = visitor.RewrittableWhereClauses;
            var  queryable            = m_accessor_enumerable.AsQueryable<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor>();
            var  trimmed_expression   = visitor.InjectEnumerator(expression, queryable, typeof()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_selector));
            if (where_clauses.Count != 0)
            {
                var subject_rewritter           = new PredicateSubjectRewritter<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor>();
                Expression aggregated_predicate = subject_rewritter.Visit(where_clauses.First().Body);
                foreach (var where_clause in where_clauses.Skip(1))
                {
                    Expression predicate = where_clause.Body;
                    aggregated_predicate = Expression.AndAlso(aggregated_predicate, subject_rewritter.Visit(predicate));
                }
                IndexQueryTreeGenerator<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor> query_tree_gen       = new IndexQueryTreeGenerator<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor>(")::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(", Index.s_AccessorSubstringIndexAccessMethod, is_cell: false);
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
            Type res)::");
source->append(R"::(ult_type          = typeof(TResult);
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
                var  result               =)::");
source->append(R"::( func();
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
    internal class )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_query_provider : IQueryProvider
    {
        private static  Type                             s_cell_type        = typeof()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::();
        private static  Type                             s_ienumerable_type = typeof(IEnumerable<>);
        private         )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Enumerable           s_cell_enumerable;
        internal )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_query_provider(LocalMemoryStorage storage, LocalTransactionContext tx)
        {
            s_cell_enumerable = new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Enumerable(storage, tx);
        }
        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            if (typeof(TElement) == s_cell_type)
            {
                return (IQueryable<TElement>)new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_selector(this, expression);
            }
            else
            {
                return new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_projector<TElement>(this, expression);
            }
        }
        public TResult Execute<TResult>(Expression expression)
        {
            var  visitor              = new RewrittableWhereCaluseVisitor<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(>(expression);
            var  where_clauses        = visitor.RewrittableWhereClauses;
            var  queryable            = s_cell_enumerable.AsQueryable<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(>();
            var  trimmed_expression   = visitor.InjectEnumerator(expression, queryable, typeof()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_selector));
            if (where_clauses.Count != 0)
            {
                var subject_rewritter           = new PredicateSubjectRewritter<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(>();
                Expression aggregated_predicate = subject_rewritter.Visit(where_clauses.First().Body);
                foreach (var where_clause in where_clauses.Skip(1))
                {
                    Expression predicate = where_clause.Body;
                    aggregated_predicate = Expression.AndAlso(aggregated_predicate, subject_rewritter.Visit(predicate));
                }
                IndexQueryTreeGenerator<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(> query_tree_gen       = new IndexQueryTreeGenerator<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(>(")::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(", Index.s_CellSubstringIndexAccessMethod, is_cell: true);
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
     )::");
source->append(R"::(       bool result_is_enumerable = (result_type.GenericTypeArguments.Count() == 1);
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
 )::");
source->append(R"::(           }
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
    /// Implements System.Linq.IQueryable{)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor} and accepts LINQ
    /// queries on <see cref="Trinity.Global.LocalStorage"/>.
    /// </summary>
    public class )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_selector : IQueryable<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor>
    {
        private         Expression                                   query_expression;
        private         )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_query_provider    query_provider;
        private )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_selector() { /* nobody should reach this method */ }
        internal )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_selector(Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx)
        {
            this.query_expression              = Expression.Constant(this);
            this.query_provider                = new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_query_provider(storage, tx);
        }
        internal unsafe )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_selector()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_query_provider query_provider, Expression query_expression)
        {
            this.query_expression              = query_expression;
            this.query_provider                = query_provider;
        }
        #region IQueryable<CellAccessor> interfaces
        public IEnumerator<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor); }
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
        public PLINQWrapper<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor> AsParallel()
        {
            return new PLINQWrapper<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor>(this);
        }
        #endregion
    }
    /// <summary>
    /// Implements System.Linq.IQueryable{)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(} and accepts LINQ
    /// queries on <see cref="Trinity.Global.LocalStorage"/>.
    /// </summary>
    public class )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_selector : IQueryable<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(>, IOrderedQueryable<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(>
    {
        private         Expression                                   query_expression;
        private         )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_query_provider             query_provider;
        private )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_selector() { /* nobody should reach this method */ }
        internal )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_selector(Trinity.Storage.LocalMemoryStorage storage, LocalTransactionContext tx)
        {
            this.query_expression              = Expression.Constant(this);
            this.query_provider                = new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_query_provider(storage, tx);
        }
        internal unsafe )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_selector()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_query_provider query_provider, Expression query_expression)
        {
            this.query_expression              = query_expression;
            this.query_provider                = query_provider;
        }
        #region IQueryable<Cell> interfaces
        public IEnumerator<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(> GetEnumerator()
        {
            return Provider.Execute<IEnumerator<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(>>(query_expression);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator<)::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(>)this.GetEnumerator();
        }
        public Type ElementType
        {
            get { return typeof()::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(); }
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
    )::");
}
source->append(R"::(
    public static class LocalStorageCellSelectorExternsion
    {
        )::");
for (size_t iterator_1 = 0; iterator_1 < (node->cellList)->size();++iterator_1)
{
source->append(R"::(
        /// <summary>
        /// Enumerates all the )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::( within the local memory storage.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>All the )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::( within the local memory storage.</returns>
        public static )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_selector )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Selector(this LocalMemoryStorage storage)
        {
            return new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_selector(storage, null);
        }
        /// <summary>
        /// Enumerates all the )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor within the local memory storage.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>All the )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor within the local memory storage.</returns>
        public static )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_selector )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_Selector(this LocalMemoryStorage storage)
        {
            return new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_selector(storage, null);
        }
        /// <summary>
        /// Enumerates all the )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::( within the local memory storage.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>All the )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::( within the local memory storage.</returns>
        public static )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_selector )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Selector(this LocalMemoryStorage storage, LocalTransactionContext tx)
        {
            return new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_local_selector(storage, tx);
        }
        /// <summary>
        /// Enumerates all the )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor within the local memory storage.
        /// </summary>
        /// <param name="storage">A <see cref="Trinity.Storage.LocalMemoryStorage"/> object.</param>
        /// <returns>All the )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor within the local memory storage.</returns>
        public static )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_selector )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_Selector(this LocalMemoryStorage storage, LocalTransactionContext tx)
        {
            return new )::");
source->append(Codegen::GetString((*(node->cellList))[iterator_1]->name));
source->append(R"::(_Accessor_local_selector(storage, tx);
        }
        )::");
}
source->append(R"::(
    }
}
)::");

            return source;
        }
    }
}
