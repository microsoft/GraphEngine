#pragma warning disable 162,168,649,660,661,1522
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Trinity.TSL;
namespace Trinity.Extension.Linq
{
    /// <summary>
    /// Provides a wrapper that redirects CellAccessorSelector.AsParallel().
    /// </summary>
    public class PLINQWrapper<TSource>
    {
        private IEnumerable<TSource> m_accessor_selector;
        internal PLINQWrapper(IQueryable<TSource> accessor_selector)
        {
            m_accessor_selector = accessor_selector;
        }
        internal PLINQWrapper(IEnumerable<TSource> enumerable)
        {
            m_accessor_selector = enumerable;
        }
        /// <summary>
        ///     Applies an accumulator function over a sequence.
        /// </summary>
        /// 
        /// <param name="func">
        ///     An accumulator function to be invoked on each element.
        /// </param>
        /// 
        /// <returns>
        ///     The final accumulator value.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or func is null.
        /// </exception>
        ///
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        /// 
        public TSource Aggregate(Func<TSource, TSource, TSource> func) { return m_accessor_selector.Aggregate(func); }
        ///
        /// <summary>
        ///     Applies an accumulator function over a sequence. The specified seed value
        ///     is used as the initial accumulator value.
        /// </summary>
        /// 
        /// <param name="seed">
        ///     The initial accumulator value.
        /// </param>
        /// 
        /// <param name="func">
        ///     An accumulator function to be invoked on each element.
        /// </param>
        ///
        /// <typeparam name="TAccumulate">
        ///     The type of the accumulator value.
        /// </typeparam>
        /// <returns>
        ///     The final accumulator value.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or func is null.
        /// </exception>
        /// 
        public TAccumulate Aggregate<TAccumulate>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func)
        {
            return m_accessor_selector.Aggregate(seed, func);
        }
        ///
        /// <summary>
        ///     Applies an accumulator function over a sequence. The specified seed value
        ///     is used as the initial accumulator value, and the specified function is used
        ///     to select the result value.
        /// </summary>
        /// 
        /// <param name="seed">
        ///     The initial accumulator value.
        /// </param>
        /// 
        /// <param name="func">
        ///     An accumulator function to be invoked on each element.
        /// </param>
        ///
        /// <param name="resultSelector">
        ///     A function to transform the final accumulator value into the result value.
        /// </param>
        ///
        /// <typeparam name="TAccumulate">
        ///     The type of the accumulator value.
        /// </typeparam>
        /// 
        /// <typeparam name="TResult">
        ///     The type of the resulting value.
        /// </typeparam>
        /// 
        /// <returns>
        ///     The transformed final accumulator value.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or func or resultSelector is null.
        /// </exception>
        /// 
        public TResult Aggregate<TAccumulate, TResult>(TAccumulate seed, Func<TAccumulate, TSource, TAccumulate> func, Func<TAccumulate, TResult> resultSelector)
        {
            return m_accessor_selector.Aggregate(seed, func, resultSelector);
        }
        /// <summary>
        ///     Determines whether all elements of a sequence satisfy a condition.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A function to test each element for a condition.
        /// </param>
        /// 
        /// <returns>
        ///     true if every element of the source sequence passes the test in the specified
        ///     predicate, or if the sequence is empty; otherwise, false.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        /// 
        public bool All(Func<TSource, bool> predicate)
        {
            return m_accessor_selector.All(predicate);
        }
        /// <summary>
        ///     Determines whether a sequence contains any elements.
        /// </summary>
        /// <returns>
        ///     true if the source sequence contains any elements; otherwise, false.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        /// 
        public bool Any()
        {
            return m_accessor_selector.Any();
        }
        /// <summary>
        ///     Determines whether any element of a sequence satisfies a condition.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A function to test each element for a condition.
        /// </param>
        /// 
        /// <returns>
        ///     true if any elements in the source sequence pass the test in the specified
        ///     predicate; otherwise, false.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        /// 
        public bool Any(Func<TSource, bool> predicate)
        {
            return m_accessor_selector.Any(predicate);
        }
        /// <summary>
        ///     Returns the input typed as <see cref="System.Collections.Generic.IEnumerable{T}"/>.
        /// </summary>
        /// <returns>
        ///     The input sequence typed as <see cref="System.Collections.Generic.IEnumerable{T}"/>.
        /// </returns>
        public IEnumerable<TSource> AsEnumerable()
        {
            return m_accessor_selector.AsEnumerable();
        }
        /// <summary>
        ///     Computes the average of a sequence of nullable System.Decimal values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        ///      
        /// <returns>
        ///     The average of the sequence of values, or null if the source sequence is
        ///     empty or contains only values that are null.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// <exception cref="System.OverflowException">
        ///     The sum of the elements in the sequence is larger than System.Decimal.MaxValue.
        /// </exception>
        /// 
        public decimal? Average(Func<TSource, decimal?> selector)
        {
            return m_accessor_selector.Average(selector);
        }
        ///
        /// <summary>
        ///     Computes the average of a sequence of System.Decimal values that are obtained
        ///     by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The average of the sequence of values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        ///
        /// <exception cref="System.OverflowException">
        ///     The sum of the elements in the sequence is larger than System.Decimal.MaxValue.
        /// </exception>
        public decimal Average(Func<TSource, decimal> selector)
        {
            return m_accessor_selector.Average(selector);
        }
        ///
        /// <summary>
        ///     Computes the average of a sequence of nullable System.Double values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The average of the sequence of values, or null if the source sequence is
        ///     empty or contains only values that are null.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public double? Average(Func<TSource, double?> selector)
        {
            return m_accessor_selector.Average(selector);
        }
        /// 
        /// <summary>
        ///     Computes the average of a sequence of System.Double values that are obtained
        ///     by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The average of the sequence of values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        public double Average(Func<TSource, double> selector)
        {
            return m_accessor_selector.Average(selector);
        }
        ///
        /// <summary>
        ///     Computes the average of a sequence of nullable System.Single values that
        ///     are obtained by invoking a transform function on each element of the input
        ///     sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The average of the sequence of values, or null if the source sequence is
        ///     empty or contains only values that are null.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public float? Average(Func<TSource, float?> selector)
        {
            return m_accessor_selector.Average(selector);
        }
        /// 
        /// <summary>
        ///     Computes the average of a sequence of System.Single values that are obtained
        ///     by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The average of the sequence of values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        public float Average(Func<TSource, float> selector)
        {
            return m_accessor_selector.Average(selector);
        }
        ///
        /// <summary>
        ///     Computes the average of a sequence of nullable System.Int32 values that are
        ///     obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The average of the sequence of values, or null if the source sequence is
        ///     empty or contains only values that are null.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.OverflowException">
        ///     The sum of the elements in the sequence is larger than System.Int64.MaxValue.
        /// </exception>
        public double? Average(Func<TSource, int?> selector)
        {
            return m_accessor_selector.Average(selector);
        }
        /// <summary>
        ///     Computes the average of a sequence of System.Int32 values that are obtained
        ///     by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The average of the sequence of values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        ///
        /// <exception cref="System.OverflowException">
        ///     The sum of the elements in the sequence is larger than System.Int64.MaxValue.
        /// </exception>
        public double Average(Func<TSource, int> selector)
        {
            return m_accessor_selector.Average(selector);
        }
        ///
        /// <summary>
        ///     Computes the average of a sequence of nullable System.Int64 values that are
        ///     obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The average of the sequence of values, or null if the source sequence is
        ///     empty or contains only values that are null.
        /// </returns>
        public double? Average(Func<TSource, long?> selector)
        {
            return m_accessor_selector.Average(selector);
        }
        /// <summary>
        ///     Computes the average of a sequence of System.Int64 values that are obtained
        ///     by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The average of the sequence of values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        ///
        /// <exception cref="System.OverflowException">
        ///     The sum of the elements in the sequence is larger than System.Int64.MaxValue.
        /// </exception>
        public double Average(Func<TSource, long> selector)
        {
            return m_accessor_selector.Average(selector);
        }
        ///
        /// <summary>
        ///     Casts the elements of an <see cref="System.Linq.ParallelQuery"/> to the specified
        ///     type.
        /// </summary>
        /// <returns>
        ///     A System.Linq.ParallelQuery{T} that contains each element of
        ///     the source sequence cast to the specified type.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidCastException">
        ///     An element in the sequence cannot be cast to type TResult.
        /// </exception>
        public ParallelQuery<TResult> Cast<TResult>()
        {
            return m_accessor_selector.Cast<TResult>().AsParallel();
        }
        ///
        /// <summary>
        ///     Determines whether a sequence contains a specified element by using the default
        ///     equality comparer.
        /// </summary>
        /// 
        /// <param name="value">
        ///     The value to locate in the sequence.
        /// </param>
        /// 
        /// <returns>
        ///     true if the source sequence contains an element that has the specified value;
        ///     otherwise, false.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        public bool Contains(TSource value)
        {
            return m_accessor_selector.Contains(value);
        }
        /// 
        /// <summary>
        ///     Determines whether a sequence contains a specified element by using a specified
        ///     System.Collections.Generic.IEqualityComparer{T}.
        /// </summary>
        /// 
        /// <param name="value">
        ///     The value to locate in the sequence.
        /// </param>
        /// 
        /// <param name="comparer">
        ///     An equality comparer to compare values.
        /// </param>
        ///
        /// <returns>
        ///     true if the source sequence contains an element that has the specified value;
        ///     otherwise, false.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        public bool Contains(TSource value, IEqualityComparer<TSource> comparer)
        {
            return m_accessor_selector.Contains(value, comparer);
        }
        /// 
        /// <summary>
        ///     Returns the number of elements in a sequence.
        /// </summary>
        /// <returns>
        ///     The number of elements in the input sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        /// 
        /// <exception cref="System.OverflowException">
        ///     The number of elements in source is larger than System.Int32.MaxValue.
        /// </exception>
        public int Count()
        {
            return m_accessor_selector.Count();
        }
        ///
        /// <summary>
        ///     Returns a number that represents how many elements in the specified sequence
        ///     satisfy a condition.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A function to test each element for a condition.
        /// </param>
        /// 
        /// <returns>
        ///     A number that represents how many elements in the sequence satisfy the condition
        ///     in the predicate function.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        /// 
        /// <exception cref="System.OverflowException">
        ///     The number of elements in source is larger than System.Int32.MaxValue.
        /// </exception>
        public int Count(Func<TSource, bool> predicate) { return m_accessor_selector.Count(predicate); }
        ///
        /// <summary>
        ///     Returns the first element of a sequence.
        /// </summary>
        /// <returns>
        ///     The first element in the specified sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     The source sequence is empty.
        /// </exception>
        public TSource First()
        {
            return m_accessor_selector.First();
        }
        ///
        /// <summary>
        ///     Returns the first element in a sequence that satisfies a specified condition.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A function to test each element for a condition.
        /// </param>
        /// 
        /// <returns>
        ///     The first element in the sequence that passes the test in the specified predicate
        ///     function.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     No element satisfies the condition in predicate.-or-The source sequence is
        /// </exception>
        ///     empty.
        public TSource First(Func<TSource, bool> predicate)
        {
            return m_accessor_selector.First(predicate);
        }
        ///
        /// <summary>
        ///     Returns the first element of a sequence, or a default value if the sequence
        ///     contains no elements.
        /// </summary>
        /// <returns>
        ///     default(TSource) if source is empty; otherwise, the first element in source.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        public TSource FirstOrDefault()
        {
            return m_accessor_selector.FirstOrDefault();
        }
        /// 
        /// <summary>
        ///     Returns the first element of the sequence that satisfies a condition or a
        ///     default value if no such element is found.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A function to test each element for a condition.
        /// </param>
        /// 
        /// <returns>
        ///     default(TSource) if source is empty or if no element passes the test specified
        ///     by predicate; otherwise, the first element in source that passes the test
        ///     specified by predicate.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        public TSource FirstOrDefault(Func<TSource, bool> predicate)
        {
            return m_accessor_selector.FirstOrDefault(predicate);
        }
        /// 
        /// <summary>
        ///     Returns the last element of a sequence.
        /// </summary>
        /// <returns>
        ///     The value at the last position in the source sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     The source sequence is empty.
        /// </exception>
        public TSource Last() { return m_accessor_selector.Last(); }
        ///
        /// <summary>
        ///     Returns the last element of a sequence that satisfies a specified condition.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A function to test each element for a condition.
        /// </param>
        /// 
        /// <returns>
        ///     The last element in the sequence that passes the test in the specified predicate
        ///     function.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     No element satisfies the condition in predicate.-or-The source sequence is
        /// </exception>
        ///     empty.
        public TSource Last(Func<TSource, bool> predicate) { return m_accessor_selector.Last(predicate); }
        ///
        /// <summary>
        ///     Returns the last element of a sequence, or a default value if the sequence
        ///     contains no elements.
        /// </summary>
        /// <returns>
        ///     default(TSource) if the source sequence is empty; otherwise, the last element
        ///     in the Trinity.Index.LINQ.ParallelQuery{T}.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        public TSource LastOrDefault() { return m_accessor_selector.LastOrDefault(); }
        /// 
        /// <summary>
        ///     Returns the last element of a sequence that satisfies a condition or a default
        ///     value if no such element is found.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A function to test each element for a condition.
        /// </param>
        /// 
        /// <returns>
        ///     default(TSource) if the sequence is empty or if no elements pass the test
        ///     in the predicate function; otherwise, the last element that passes the test
        ///     in the predicate function.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        public TSource LastOrDefault(Func<TSource, bool> predicate)
        {
            return m_accessor_selector.LastOrDefault(predicate);
        }
        /// 
        /// <summary>
        ///     Returns an System.Int64 that represents the total number of elements in a
        ///     sequence.
        /// </summary>
        /// <returns>
        ///     The number of elements in the source sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        /// 
        /// <exception cref="System.OverflowException">
        ///     The number of elements exceeds System.Int64.MaxValue.
        /// </exception>
        public long LongCount()
        {
            return m_accessor_selector.LongCount();
        }
        ///
        /// <summary>
        ///     Returns an System.Int64 that represents how many elements in a sequence satisfy
        ///     a condition.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A function to test each element for a condition.
        /// </param>
        /// 
        /// <returns>
        ///     A number that represents how many elements in the sequence satisfy the condition
        ///     in the predicate function.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        /// 
        /// <exception cref="System.OverflowException">
        ///     The number of matching elements exceeds System.Int64.MaxValue.
        /// </exception>
        public long LongCount(Func<TSource, bool> predicate)
        {
            return m_accessor_selector.LongCount(predicate);
        }
        ///
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     maximum nullable System.Decimal value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The value of type Nullable{Decimal} in C# or Nullable(Of Decimal) in Visual
        ///     Basic that corresponds to the maximum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public decimal? Max(Func<TSource, decimal?> selector)
        {
            return m_accessor_selector.Max(selector);
        }
        /// 
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     maximum System.Decimal value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The maximum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        public decimal Max(Func<TSource, decimal> selector)
        {
            return m_accessor_selector.Max(selector);
        }
        ///
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     maximum nullable System.Double value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The value of type Nullable{Double} in C# or Nullable(Of Double) in Visual
        ///     Basic that corresponds to the maximum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public double? Max(Func<TSource, double?> selector)
        {
            return m_accessor_selector.Max(selector);
        }
        /// 
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     maximum System.Double value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The maximum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        public double Max(Func<TSource, double> selector)
        {
            return m_accessor_selector.Max(selector);
        }
        ///
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     maximum nullable System.Single value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The value of type Nullable{Single} in C# or Nullable(Of Single) in Visual
        ///     Basic that corresponds to the maximum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public float? Max(Func<TSource, float?> selector)
        {
            return m_accessor_selector.Max(selector);
        }
        /// 
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     maximum System.Single value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The maximum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        public float Max(Func<TSource, float> selector)
        {
            return m_accessor_selector.Max(selector);
        }
        ///
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     maximum nullable System.Int32 value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The value of type Nullable{Int32} in C# or Nullable(Of Int32) in Visual Basic
        ///     that corresponds to the maximum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public int? Max(Func<TSource, int?> selector)
        {
            return m_accessor_selector.Max(selector);
        }
        /// 
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     maximum System.Int32 value.
        /// </summary>
        /// 
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        ///
        /// <returns>
        ///     The maximum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        public int Max(Func<TSource, int> selector)
        {
            return m_accessor_selector.Max(selector);
        }
        ///
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     maximum nullable System.Int64 value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The value of type Nullable{Int64} in C# or Nullable(Of Int64) in Visual Basic
        ///     that corresponds to the maximum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public long? Max(Func<TSource, long?> selector)
        {
            return m_accessor_selector.Max(selector);
        }
        /// 
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     maximum System.Int64 value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The maximum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        public long Max(Func<TSource, long> selector)
        {
            return m_accessor_selector.Max(selector);
        }
        ///
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     minimum nullable System.Decimal value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The value of type Nullable{Decimal} in C# or Nullable(Of Decimal) in Visual
        ///     Basic that corresponds to the minimum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public decimal? Min(Func<TSource, decimal?> selector)
        {
            return m_accessor_selector.Min(selector);
        }
        /// 
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     minimum System.Decimal value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The minimum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        public decimal Min(Func<TSource, decimal> selector)
        {
            return m_accessor_selector.Min(selector);
        }
        ///
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     minimum nullable System.Double value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The value of type Nullable{Double} in C# or Nullable(Of Double) in Visual
        ///     Basic that corresponds to the minimum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public double? Min(Func<TSource, double?> selector)
        {
            return m_accessor_selector.Min(selector);
        }
        /// 
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     minimum System.Double value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The minimum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        public double Min(Func<TSource, double> selector)
        {
            return m_accessor_selector.Min(selector);
        }
        ///
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     minimum nullable System.Single value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The value of type Nullable{Single} in C# or Nullable(Of Single) in Visual
        ///     Basic that corresponds to the minimum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public float? Min(Func<TSource, float?> selector)
        {
            return m_accessor_selector.Min(selector);
        }
        /// 
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     minimum System.Single value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The minimum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        public float Min(Func<TSource, float> selector)
        {
            return m_accessor_selector.Min(selector);
        }
        ///
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     minimum nullable System.Int32 value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The value of type Nullable{Int32} in C# or Nullable(Of Int32) in Visual Basic
        ///     that corresponds to the minimum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public int? Min(Func<TSource, int?> selector)
        {
            return m_accessor_selector.Min(selector);
        }
        /// 
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     minimum System.Int32 value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The minimum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        public int Min(Func<TSource, int> selector)
        {
            return m_accessor_selector.Min(selector);
        }
        ///
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     minimum nullable System.Int64 value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The value of type Nullable{Int64} in C# or Nullable(Of Int64) in Visual Basic
        ///     that corresponds to the minimum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public long? Min(Func<TSource, long?> selector)
        {
            return m_accessor_selector.Min(selector);
        }
        /// 
        /// <summary>
        ///     Invokes a transform function on each element of a sequence and returns the
        ///     minimum System.Int64 value.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The minimum value in the sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     source contains no elements.
        /// </exception>
        public long Min(Func<TSource, long> selector)
        {
            return m_accessor_selector.Min(selector);
        }
        ///
        /// <summary>
        ///     Projects each element of a sequence into a new form by incorporating the
        ///     element's index.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each source element; the second parameter
        /// </param>
        ///     of the function represents the index of the source element.
        /// 
        /// <typeparam name="TResult">
        ///     The type of the value returned by selector.
        /// </typeparam>
        ///
        /// <returns>
        ///     A System.Linq.ParallelQuery{T} whose elements are the result
        ///     of invoking the transform function on each element of source.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public ParallelQuery<TResult> Select<TResult>(Func<TSource, int, TResult> selector)
        {
            return m_accessor_selector.Select(selector).AsParallel();
        }
        /// 
        /// <summary>
        ///     Projects each element of a sequence into a new form.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <typeparam name="TResult">
        ///     The type of the value returned by selector.
        /// </typeparam>
        ///
        /// <returns>
        ///     A System.Linq.ParallelQuery{T} whose elements are the result
        ///     of invoking the transform function on each element of source.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public ParallelQuery<TResult> Select<TResult>(Func<TSource, TResult> selector)
        {
            return m_accessor_selector.Select(selector).AsParallel();
        }
        /// 
        /// <summary>
        ///     Projects each element of a sequence to an Trinity.Index.LINQ.ParallelQuery{T}
        ///     and flattens the resulting sequences into one sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <typeparam name="TResult">
        ///     The type of the elements of the sequence returned by selector.
        /// </typeparam>
        ///
        /// <returns>
        ///     A System.Linq.ParallelQuery{T} whose elements are the result
        ///     of invoking the one-to-many transform function on each element of the input
        ///     sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public ParallelQuery<TResult> SelectMany<TResult>(Func<TSource, IEnumerable<TResult>> selector)
        {
            return m_accessor_selector.SelectMany(selector).AsParallel();
        }
        /// 
        /// <summary>
        ///     Projects each element of a sequence to an Trinity.Index.LINQ.ParallelQuery{T},
        ///     and flattens the resulting sequences into one sequence. The index of each
        ///     source element is used in the projected form of that element.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each source element; the second parameter
        /// </param>
        ///     of the function represents the index of the source element.
        /// 
        /// <typeparam name="TResult">
        ///     The type of the elements of the sequence returned by selector.
        /// </typeparam>
        ///
        /// <returns>
        ///     A System.Linq.ParallelQuery{T} whose elements are the result
        ///     of invoking the one-to-many transform function on each element of an input
        ///     sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public ParallelQuery<TResult> SelectMany<TResult>(Func<TSource, int, IEnumerable<TResult>> selector)
        {
            return m_accessor_selector.SelectMany(selector).AsParallel();
        }
        /// 
        /// <summary>
        ///     Projects each element of a sequence to an Trinity.Index.LINQ.ParallelQuery{T},
        ///     flattens the resulting sequences into one sequence, and invokes a result
        ///     selector function on each element therein.
        /// </summary>
        /// 
        /// 
        /// <param name="collectionSelector">
        ///     A transform function to apply to each element of the input sequence.
        /// </param>
        ///
        /// <param name="resultSelector">
        ///     A transform function to apply to each element of the intermediate sequence.
        /// </param>
        ///
        /// <typeparam name="TCollection">
        ///     The type of the intermediate elements collected by <paramref name="collectionSelector"/>.
        /// </typeparam>
        ///
        /// <typeparam name="TResult">
        ///     The type of the elements of the resulting sequence.
        /// </typeparam>
        ///
        /// <returns>
        ///     A System.Linq.ParallelQuery{T} whose elements are the result
        ///     of invoking the one-to-many transform function collectionSelector on each
        ///     element of source and then mapping each of those sequence elements and their
        ///     corresponding source element to a result element.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or collectionSelector or resultSelector is null.
        /// </exception>
        public ParallelQuery<TResult> SelectMany<TCollection, TResult>(Func<TSource, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return m_accessor_selector.SelectMany(collectionSelector, resultSelector).AsParallel();
        }
        /// 
        /// <summary>
        ///     Projects each element of a sequence to an Trinity.Index.LINQ.ParallelQuery{T},
        ///     flattens the resulting sequences into one sequence, and invokes a result
        ///     selector function on each element therein. The index of each source element
        ///     is used in the intermediate projected form of that element.
        /// </summary>
        /// 
        /// <param name="collectionSelector">
        ///     A transform function to apply to each source element; the second parameter
        /// </param>
        ///     of the function represents the index of the source element.
        /// 
        /// <param name="resultSelector">
        ///     A transform function to apply to each element of the intermediate sequence.
        /// </param>
        ///
        /// <typeparam name="TCollection">
        ///     The type of the intermediate elements collected by collectionSelector.
        /// </typeparam>
        ///
        /// <typeparam name="TResult">
        ///     The type of the elements of the resulting sequence.
        /// </typeparam>
        ///
        /// <returns>
        ///     A System.Linq.ParallelQuery{T} whose elements are the result
        ///     of invoking the one-to-many transform function collectionSelector on each
        ///     element of source and then mapping each of those sequence elements and their
        ///     corresponding source element to a result element.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or collectionSelector or resultSelector is null.
        /// </exception>
        public ParallelQuery<TResult> SelectMany<TCollection, TResult>(Func<TSource, int, IEnumerable<TCollection>> collectionSelector, Func<TSource, TCollection, TResult> resultSelector)
        {
            return m_accessor_selector.SelectMany(collectionSelector, resultSelector).AsParallel();
        }
        /// 
        /// <summary>
        ///     Determines whether two sequences are equal by comparing their elements by
        ///     using a specified System.Collections.Generic.IEqualityComparer{T}.
        /// </summary>
        public bool SequenceEqual(IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            return m_accessor_selector.SequenceEqual(second, comparer);
        }
        ///
        /// <summary>
        ///     Determines whether two sequences are equal by comparing the elements by using
        ///     the default equality comparer for their type.
        /// </summary>
        public bool SequenceEqual(IEnumerable<TSource> second)
        {
            return m_accessor_selector.SequenceEqual(second);
        }
        ///
        /// <summary>
        ///     Returns the only element of a sequence, and throws an exception if there
        ///     is not exactly one element in the sequence.
        /// </summary>
        /// <returns>
        ///     The single element of the input sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     The input sequence contains more than one element.-or-The input sequence
        /// </exception>
        ///     is empty.
        public TSource Single()
        {
            return m_accessor_selector.Single();
        }
        ///
        /// <summary>
        ///     Returns the only element of a sequence that satisfies a specified condition,
        ///     and throws an exception if more than one such element exists.
        /// </summary>
        /// 
        /// 
        /// <param name="predicate">
        ///     A function to test an element for a condition.
        /// </param>
        ///
        /// <returns>
        ///     The single element of the input sequence that satisfies a condition.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     No element satisfies the condition in predicate.-or-More than one element
        /// </exception>
        ///     satisfies the condition in predicate.-or-The source sequence is empty.
        public TSource Single(Func<TSource, bool> predicate)
        {
            return m_accessor_selector.Single(predicate);
        }
        ///
        /// <summary>
        ///     Returns the only element of a sequence, or a default value if the sequence
        ///     is empty; this method throws an exception if there is more than one element
        ///     in the sequence.
        /// </summary>
        /// <returns>
        ///     The single element of the input sequence, or default(TSource) if the sequence
        ///     contains no elements.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        /// 
        /// <exception cref="System.InvalidOperationException">
        ///     The input sequence contains more than one element.
        /// </exception>
        public TSource SingleOrDefault()
        {
            return m_accessor_selector.SingleOrDefault();
        }
        ///
        /// <summary>
        ///     Returns the only element of a sequence that satisfies a specified condition
        ///     or a default value if no such element exists; this method throws an exception
        ///     if more than one element satisfies the condition.
        /// </summary>
        /// 
        /// 
        /// <param name="predicate">
        ///     A function to test an element for a condition.
        /// </param>
        ///
        /// <returns>
        ///     The single element of the input sequence that satisfies the condition, or
        ///     default(TSource) if no such element is found.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        public TSource SingleOrDefault(Func<TSource, bool> predicate)
        {
            return m_accessor_selector.SingleOrDefault(predicate);
        }
        /// 
        /// <summary>
        ///     Bypasses a specified number of elements in a sequence and then returns the
        ///     remaining elements.
        /// </summary>
        /// 
        /// 
        /// <param name="count">
        ///     The number of elements to skip before returning the remaining elements.
        /// </param>
        ///
        /// <returns>
        ///     A Trinity.Index.LINQ.PLINQWrapper{T} that contains the elements that
        ///     occur after the specified index in the input sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        public PLINQWrapper<TSource> Skip(int count)
        {
            return new PLINQWrapper<TSource>(m_accessor_selector.Skip(count));
        }
        /// 
        /// <summary>
        ///     Bypasses elements in a sequence as long as a specified condition is true
        ///     and then returns the remaining elements.
        /// </summary>
        /// 
        /// 
        /// <param name="predicate">
        ///     A function to test each element for a condition.
        /// </param>
        ///
        ///
        /// <returns>
        ///     A Trinity.Index.LINQ.PLINQWrapper{T} that contains the elements from
        ///     the input sequence starting at the first element in the linear series that
        ///     does not pass the test specified by predicate.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        public PLINQWrapper<TSource> SkipWhile(Func<TSource, bool> predicate)
        {
            return new PLINQWrapper<TSource>(m_accessor_selector.SkipWhile(predicate));
        }
        /// 
        /// <summary>
        ///     Bypasses elements in a sequence as long as a specified condition is true
        ///     and then returns the remaining elements. The element's index is used in the
        ///     logic of the predicate function.
        /// </summary>
        /// 
        /// 
        /// <param name="predicate">
        ///     A function to test each source element for a condition; the second parameter
        /// </param>
        ///     of the function represents the index of the source element.
        ///
        /// <returns>
        ///     A Trinity.Index.LINQ.PLINQWrapper{T} that contains the elements from
        ///     the input sequence starting at the first element in the linear series that
        ///     does not pass the test specified by predicate.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        public PLINQWrapper<TSource> SkipWhile(Func<TSource, int, bool> predicate)
        {
            return new PLINQWrapper<TSource>(m_accessor_selector.SkipWhile(predicate));
        }
        /// 
        /// <summary>
        ///     Computes the sum of the sequence of nullable System.Decimal values that are
        ///     obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The sum of the projected values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.OverflowException">
        ///     The sum is larger than System.Decimal.MaxValue.
        /// </exception>
        public decimal? Sum(Func<TSource, decimal?> selector)
        {
            return m_accessor_selector.Sum(selector);
        }
        ///
        /// <summary>
        ///     Computes the sum of the sequence of System.Decimal values that are obtained
        ///     by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The sum of the projected values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.OverflowException">
        ///     The sum is larger than System.Decimal.MaxValue.
        /// </exception>
        public decimal Sum(Func<TSource, decimal> selector)
        {
            return m_accessor_selector.Sum(selector);
        }
        ///
        /// <summary>
        ///     Computes the sum of the sequence of nullable System.Double values that are
        ///     obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The sum of the projected values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public double? Sum(Func<TSource, double?> selector)
        {
            return m_accessor_selector.Sum(selector);
        }
        /// 
        /// <summary>
        ///     Computes the sum of the sequence of System.Double values that are obtained
        ///     by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The sum of the projected values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public double Sum(Func<TSource, double> selector)
        {
            return m_accessor_selector.Sum(selector);
        }
        /// 
        /// <summary>
        ///     Computes the sum of the sequence of nullable System.Single values that are
        ///     obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The sum of the projected values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public float? Sum(Func<TSource, float?> selector)
        {
            return m_accessor_selector.Sum(selector);
        }
        /// 
        /// <summary>
        ///     Computes the sum of the sequence of System.Single values that are obtained
        ///     by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The sum of the projected values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        public float Sum(Func<TSource, float> selector)
        {
            return m_accessor_selector.Sum(selector);
        }
        /// 
        /// <summary>
        ///     Computes the sum of the sequence of nullable System.Int32 values that are
        ///     obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The sum of the projected values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.OverflowException">
        ///     The sum is larger than System.Int32.MaxValue.
        /// </exception>
        public int? Sum(Func<TSource, int?> selector)
        {
            return m_accessor_selector.Sum(selector);
        }
        ///
        /// <summary>
        ///     Computes the sum of the sequence of System.Int32 values that are obtained
        ///     by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The sum of the projected values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.OverflowException">
        ///     The sum is larger than System.Int32.MaxValue.
        /// </exception>
        public int Sum(Func<TSource, int> selector)
        {
            return m_accessor_selector.Sum(selector);
        }
        ///
        /// <summary>
        ///     Computes the sum of the sequence of nullable System.Int64 values that are
        ///     obtained by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The sum of the projected values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.OverflowException">
        ///     The sum is larger than System.Int64.MaxValue.
        /// </exception>
        public long? Sum(Func<TSource, long?> selector)
        {
            return m_accessor_selector.Sum(selector);
        }
        ///
        /// <summary>
        ///     Computes the sum of the sequence of System.Int64 values that are obtained
        ///     by invoking a transform function on each element of the input sequence.
        /// </summary>
        /// 
        /// <param name="selector">
        ///     A transform function to apply to each element.
        /// </param>
        /// 
        /// <returns>
        ///     The sum of the projected values.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or selector is null.
        /// </exception>
        /// 
        /// <exception cref="System.OverflowException">
        ///     The sum is larger than System.Int64.MaxValue.
        /// </exception>
        public long Sum(Func<TSource, long> selector)
        {
            return m_accessor_selector.Sum(selector);
        }
        ///
        /// <summary>
        ///     Returns a specified number of contiguous elements from the start of a sequence.
        /// </summary>
        /// 
        /// <param name="count">
        ///     The number of elements to return.
        /// </param>
        /// 
        /// <returns>
        ///     A Trinity.Index.LINQ.PLINQWrapper{T} that contains the specified
        ///     number of elements from the start of the input sequence.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source is null.
        /// </exception>
        public PLINQWrapper<TSource> Take(int count)
        {
            return new PLINQWrapper<TSource>(m_accessor_selector.Take(count));
        }
        /// 
        /// <summary>
        ///     Returns elements from a sequence as long as a specified condition is true.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A function to test each element for a condition.
        /// </param>
        /// 
        /// <returns>
        ///     A Trinity.Index.LINQ.PLINQWrapper{T} that contains the elements from
        ///     the input sequence that occur before the element at which the test no longer
        ///     passes.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        public PLINQWrapper<TSource> TakeWhile(Func<TSource, bool> predicate)
        {
            return new PLINQWrapper<TSource>(m_accessor_selector.TakeWhile(predicate));
        }
        /// 
        /// <summary>
        ///     Returns elements from a sequence as long as a specified condition is true.
        ///     The element's index is used in the logic of the predicate function.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A function to test each source element for a condition; the second parameter
        /// </param>
        ///     of the function represents the index of the source element.
        /// 
        /// <returns>
        ///     A Trinity.Index.LINQ.PLINQWrapper{T} that contains elements from
        ///     the input sequence that occur before the element at which the test no longer
        ///     passes.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        public PLINQWrapper<TSource> TakeWhile(Func<TSource, int, bool> predicate)
        {
            return new PLINQWrapper<TSource>(m_accessor_selector.TakeWhile(predicate));
        }
        /// 
        /// <summary>
        ///     Filters a sequence of values based on a predicate.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A function to test each element for a condition.
        /// </param>
        /// 
        /// <returns>
        ///     A Trinity.Index.LINQ.PLINQWrapper{T} that contains elements from
        ///     the input sequence that satisfy the condition.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        public PLINQWrapper<TSource> Where(Func<TSource, bool> predicate)
        {
            return new PLINQWrapper<TSource>(m_accessor_selector.Where(predicate));
        }
        /// 
        /// <summary>
        ///     Filters a sequence of values based on a predicate. Each element's index is
        ///     used in the logic of the predicate function.
        /// </summary>
        /// 
        /// <param name="predicate">
        ///     A function to test each source element for a condition; the second parameter
        /// </param>
        ///     of the function represents the index of the source element.
        /// 
        /// <returns>
        ///     A Trinity.Index.LINQ.PLINQWrapper{T} that contains elements from
        ///     the input sequence that satisfy the condition.
        /// </returns>
        /// 
        /// <exception cref="System.ArgumentNullException">
        ///     source or predicate is null.
        /// </exception>
        public PLINQWrapper<TSource> Where(Func<TSource, int, bool> predicate)
        {
            return new PLINQWrapper<TSource>(m_accessor_selector.Where(predicate));
        }
        #region Not implemented
        /// 
        /// <summary>
        ///     Concatenates two sequences.
        /// </summary>
        /// !Not implemented
        public IEnumerable<TSource> Concat(IEnumerable<TSource> second)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Returns the elements of the specified sequence or the type parameter's default
        ///     value in a singleton collection if the sequence is empty.
        /// </summary>
        /// !NotImplemented
        public IEnumerable<TSource> DefaultIfEmpty() { throw new NotImplementedException(); }
        ///
        /// <summary>
        ///     Returns the elements of the specified sequence or the specified value in
        ///     a singleton collection if the sequence is empty.
        /// </summary>
        /// !NotImplemented
        public IEnumerable<TSource> DefaultIfEmpty(TSource defaultValue) { throw new NotImplementedException(); }
        ///
        /// <summary>
        ///     Filters the elements of an System.Collections.IEnumerable based on a specified
        ///     type.
        /// </summary>
        /// !Not implemented
        public IEnumerable<TResult> OfType<TResult>()
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Sorts the elements of a sequence in ascending order according to a key.
        /// </summary>
        /// !Not implemented
        public IOrderedEnumerable<TSource> OrderBy<TKey>(Func<TSource, TKey> keySelector)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Sorts the elements of a sequence in ascending order by using a specified
        ///     comparer.
        /// </summary>
        /// !Not implemented
        public IOrderedEnumerable<TSource> OrderBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Sorts the elements of a sequence in descending order according to a key.
        /// </summary>
        /// !Not implemented
        public IOrderedEnumerable<TSource> OrderByDescending<TKey>(Func<TSource, TKey> keySelector)
        {
            throw new NotFiniteNumberException();
        }
        ///
        /// <summary>
        ///     Sorts the elements of a sequence in descending order by using a specified
        ///     comparer.
        /// </summary>
        /// !Not implemented
        public IOrderedEnumerable<TSource> OrderByDescending<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Inverts the order of the elements in a sequence.
        /// </summary>
        /// !Not implemented
        public IEnumerable<TSource> Reverse()
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Returns distinct elements from a sequence by using the default equality comparer
        ///     to compare values.
        /// </summary>
        /// !Not implemented
        public IEnumerable<TSource> Distinct()
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Returns distinct elements from a sequence by using a specified System.Collections.Generic.IEqualityComparer{T}
        ///     to compare values.
        /// </summary>
        public IEnumerable<TSource> Distinct(IEqualityComparer<TSource> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Returns the element at a specified index in a sequence.
        /// </summary>
        public TSource ElementAt(int index)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Returns the element at a specified index in a sequence or a default value
        ///     if the index is out of range.
        /// </summary>
        public TSource ElementAtOrDefault(int index)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Produces the set difference of two sequences by using the default equality
        ///     comparer to compare values.
        /// </summary>
        public IEnumerable<TSource> Except(IEnumerable<TSource> second)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Produces the set difference of two sequences by using the specified System.Collections.Generic.IEqualityComparer{T}
        ///     to compare values.
        /// </summary>
        public IEnumerable<TSource> Except(IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Groups the elements of a sequence according to a specified key selector function.
        /// </summary>
        public IEnumerable<IGrouping<TKey, TSource>> GroupBy<TKey>(Func<TSource, TKey> keySelector)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Groups the elements of a sequence according to a specified key selector function
        ///     and creates a result value from each group and its key.
        /// </summary>
        public IEnumerable<TResult> GroupBy<TKey, TResult>(Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Groups the elements of a sequence according to a specified key selector function
        ///     and projects the elements for each group by using a specified function.
        /// </summary>
        public IEnumerable<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Groups the elements of a sequence according to a specified key selector function
        ///     and compares the keys by using a specified comparer.
        /// </summary>
        public IEnumerable<IGrouping<TKey, TSource>> GroupBy<TKey>(Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Groups the elements of a sequence according to a specified key selector function
        ///     and creates a result value from each group and its key. The keys are compared
        ///     by using a specified comparer.
        /// </summary>
        public IEnumerable<TResult> GroupBy<TKey, TResult>(Func<TSource, TKey> keySelector, Func<TKey, IEnumerable<TSource>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Groups the elements of a sequence according to a specified key selector function
        ///     and creates a result value from each group and its key. The elements of each
        ///     group are projected by using a specified function.
        /// </summary>
        public IEnumerable<TResult> GroupBy<TKey, TElement, TResult>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Groups the elements of a sequence according to a key selector function. The
        ///     keys are compared by using a comparer and each group's elements are projected
        ///     by using a specified function.
        /// </summary>
        public IEnumerable<IGrouping<TKey, TElement>> GroupBy<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Groups the elements of a sequence according to a specified key selector function
        ///     and creates a result value from each group and its key. Key values are compared
        ///     by using a specified comparer, and the elements of each group are projected
        ///     by using a specified function.
        /// </summary>
        public IEnumerable<TResult> GroupBy<TKey, TElement, TResult>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, Func<TKey, IEnumerable<TElement>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Correlates the elements of two sequences based on equality of keys and groups
        ///     the results. The default equality comparer is used to compare keys.
        /// </summary>
        public IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Correlates the elements of two sequences based on key equality and groups
        ///     the results. A specified System.Collections.Generic.IEqualityComparer{T}
        ///     is used to compare keys.
        /// </summary>
        public IEnumerable<TResult> GroupJoin<TOuter, TInner, TKey, TResult>(IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, IEnumerable<TInner>, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Produces the set intersection of two sequences by using the default equality
        ///     comparer to compare values.
        /// </summary>
        public IEnumerable<TSource> Intersect(IEnumerable<TSource> second)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        ///     Produces the set intersection of two sequences by using the specified System.Collections.Generic.IEqualityComparer{T}
        ///     to compare values.
        /// </summary>
        public IEnumerable<TSource> Intersect(IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Correlates the elements of two sequences based on matching keys. The default
        ///     equality comparer is used to compare keys.
        /// </summary>
        public IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Correlates the elements of two sequences based on matching keys. A specified
        ///     System.Collections.Generic.IEqualityComparer{T} is used to compare keys.
        /// </summary>
        public IEnumerable<TResult> Join<TOuter, TInner, TKey, TResult>(IEnumerable<TInner> inner, Func<TOuter, TKey> outerKeySelector, Func<TInner, TKey> innerKeySelector, Func<TOuter, TInner, TResult> resultSelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Performs a subsequent ordering of the elements in a sequence in ascending
        ///     order according to a key.
        /// </summary>
        /// !Not implemented
        public IOrderedEnumerable<TSource> ThenBy<TKey>(Func<TSource, TKey> keySelector)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Performs a subsequent ordering of the elements in a sequence in ascending
        ///     order by using a specified comparer.
        /// </summary>
        /// !Not implemented
        public IOrderedEnumerable<TSource> ThenBy<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Performs a subsequent ordering of the elements in a sequence in descending
        ///     order, according to a key.
        /// </summary>
        /// !Not implemented
        public IOrderedEnumerable<TSource> ThenByDescending<TKey>(Func<TSource, TKey> keySelector)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Performs a subsequent ordering of the elements in a sequence in descending
        ///     order by using a specified comparer.
        /// </summary>
        /// !Not implemented
        public IOrderedEnumerable<TSource> ThenByDescending<TKey>(Func<TSource, TKey> keySelector, IComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Creates an array from a System.Collections.Generic.IEnumerable{T}.
        /// </summary>
        /// !Not implemented
        public TSource[] ToArray()
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Creates a System.Collections.Generic.Dictionary{TKey,TValue} from an System.Collections.Generic.IEnumerable{T}
        ///     according to a specified key selector function.
        /// </summary>
        /// !Not implemented
        public Dictionary<TKey, TSource> ToDictionary<TKey>(Func<TSource, TKey> keySelector)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Creates a System.Collections.Generic.Dictionary{TKey,TValue} from an System.Collections.Generic.IEnumerable{T}
        ///     according to specified key selector and element selector functions.
        /// </summary>
        /// !Not implemented
        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Creates a System.Collections.Generic.Dictionary{TKey,TValue} from an System.Collections.Generic.IEnumerable{T}
        ///     according to a specified key selector function and key comparer.
        /// </summary>
        /// !Not implemented
        public Dictionary<TKey, TSource> ToDictionary<TKey>(Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Creates a System.Collections.Generic.Dictionary{TKey,TValue} from an System.Collections.Generic.IEnumerable{T}
        ///     according to a specified key selector function, a comparer, and an element
        ///     selector function.
        /// </summary>
        /// !Not implemented
        public Dictionary<TKey, TElement> ToDictionary<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Creates a System.Collections.Generic.List{T} from an System.Collections.Generic.IEnumerable{T}.
        /// </summary>
        /// !Not implemented
        public List<TSource> ToList()
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Creates a System.Linq.Lookup{TKey,TElement} from an System.Collections.Generic.IEnumerable{T}
        ///     according to a specified key selector function.
        /// </summary>
        /// !Not implemented
        public ILookup<TKey, TSource> ToLookup<TKey>(Func<TSource, TKey> keySelector)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Creates a System.Linq.Lookup{TKey,TElement} from an System.Collections.Generic.IEnumerable{T}
        ///     according to specified key selector and element selector functions.
        /// </summary>
        /// !Not implemented
        public ILookup<TKey, TElement> ToLookup<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Creates a System.Linq.Lookup{TKey,TElement} from an System.Collections.Generic.IEnumerable{T}
        ///     according to a specified key selector function and key comparer.
        /// </summary>
        /// !Not implemented
        public ILookup<TKey, TSource> ToLookup<TKey>(Func<TSource, TKey> keySelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Creates a System.Linq.Lookup{TKey,TElement} from an System.Collections.Generic.IEnumerable{T}
        ///     according to a specified key selector function, a comparer and an element
        ///     selector function.
        /// </summary>
        /// !Not implemented
        public ILookup<TKey, TElement> ToLookup<TKey, TElement>(Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Produces the set union of two sequences by using the default equality comparer.
        /// </summary>
        /// !Not implemented
        public IEnumerable<TSource> Union(IEnumerable<TSource> second)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Produces the set union of two sequences by using a specified System.Collections.Generic.IEqualityComparer{T}.
        /// </summary>
        /// !Not implemented
        public IEnumerable<TSource> Union(IEnumerable<TSource> second, IEqualityComparer<TSource> comparer)
        {
            throw new NotImplementedException();
        }
        ///
        /// <summary>
        ///     Merges two sequences by using the specified predicate function.
        /// </summary>
        /// !Not implemented
        public IEnumerable<TResult> Zip<TFirst, TSecond, TResult>(IEnumerable<TSecond> second, Func<TFirst, TSecond, TResult> resultSelector)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}

#pragma warning restore 162,168,649,660,661,1522
