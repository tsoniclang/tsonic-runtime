/**
 * JavaScript Array implementation with full JS semantics
 * Use this when you need dynamic arrays with push/pop/splice etc.
 * For fixed-size arrays, use native T[] instead.
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tsonic.JSRuntime
{
    /// <summary>
    /// JavaScript-style dynamic array with full JS semantics.
    /// Backed by List&lt;T&gt; for dynamic sizing.
    /// </summary>
    public class JSArray<T> : IEnumerable<T>
    {
        private readonly List<T> _list;

        // ==================== Constructors ====================

        /// <summary>
        /// Create empty JSArray
        /// </summary>
        public JSArray()
        {
            _list = new List<T>();
        }

        /// <summary>
        /// Create JSArray with specified initial capacity
        /// </summary>
        public JSArray(int capacity)
        {
            _list = new List<T>(capacity);
        }

        /// <summary>
        /// Create JSArray from native array
        /// </summary>
        public JSArray(T[] source)
        {
            _list = new List<T>(source);
        }

        /// <summary>
        /// Create JSArray from List
        /// </summary>
        public JSArray(List<T> source)
        {
            _list = new List<T>(source);
        }

        /// <summary>
        /// Create JSArray from any enumerable
        /// </summary>
        public JSArray(IEnumerable<T> source)
        {
            _list = new List<T>(source);
        }

        // ==================== Properties ====================

        /// <summary>
        /// Get array length
        /// </summary>
        public int length => _list.Count;

        // ==================== Indexer ====================

        /// <summary>
        /// Get or set element at index
        /// </summary>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= _list.Count)
                {
                    return default(T)!;
                }
                return _list[index];
            }
            set
            {
                if (index < 0)
                {
                    throw new ArgumentException("Array index cannot be negative", nameof(index));
                }

                // Fill gaps with default(T) if index is beyond current length
                while (_list.Count <= index)
                {
                    _list.Add(default(T)!);
                }

                _list[index] = value;
            }
        }

        // ==================== Length Manipulation ====================

        /// <summary>
        /// Set array length (truncate or extend with defaults)
        /// </summary>
        public void setLength(int newLength)
        {
            if (newLength < 0)
            {
                throw new ArgumentException("Invalid array length", nameof(newLength));
            }

            if (newLength < _list.Count)
            {
                _list.RemoveRange(newLength, _list.Count - newLength);
            }
            else if (newLength > _list.Count)
            {
                int toAdd = newLength - _list.Count;
                for (int i = 0; i < toAdd; i++)
                {
                    _list.Add(default(T)!);
                }
            }
        }

        // ==================== Basic Mutation Methods ====================

        /// <summary>
        /// Add element to end of array and return new length
        /// </summary>
        public int push(T item)
        {
            _list.Add(item);
            return _list.Count;
        }

        /// <summary>
        /// Add multiple elements to end of array and return new length
        /// </summary>
        public int push(params T[] items)
        {
            _list.AddRange(items);
            return _list.Count;
        }

        /// <summary>
        /// Remove and return last element
        /// </summary>
        public T pop()
        {
            if (_list.Count == 0)
            {
                return default(T)!;
            }

            T item = _list[_list.Count - 1];
            _list.RemoveAt(_list.Count - 1);
            return item;
        }

        /// <summary>
        /// Remove and return first element
        /// </summary>
        public T shift()
        {
            if (_list.Count == 0)
            {
                return default(T)!;
            }

            T item = _list[0];
            _list.RemoveAt(0);
            return item;
        }

        /// <summary>
        /// Add element to beginning of array and return new length
        /// </summary>
        public int unshift(T item)
        {
            _list.Insert(0, item);
            return _list.Count;
        }

        /// <summary>
        /// Add multiple elements to beginning of array and return new length
        /// </summary>
        public int unshift(params T[] items)
        {
            _list.InsertRange(0, items);
            return _list.Count;
        }

        // ==================== Slicing Methods ====================

        /// <summary>
        /// Return shallow copy of portion of array
        /// </summary>
        public JSArray<T> slice(int start = 0, int? end = null)
        {
            int actualStart = start < 0 ? System.Math.Max(0, _list.Count + start) : start;
            int actualEnd = end.HasValue
                ? (end.Value < 0 ? System.Math.Max(0, _list.Count + end.Value) : end.Value)
                : _list.Count;

            actualStart = System.Math.Min(actualStart, _list.Count);
            actualEnd = System.Math.Min(actualEnd, _list.Count);

            if (actualStart >= actualEnd)
            {
                return new JSArray<T>();
            }

            return new JSArray<T>(_list.GetRange(actualStart, actualEnd - actualStart));
        }

        /// <summary>
        /// Add/remove elements at position
        /// </summary>
        public JSArray<T> splice(int start, int? deleteCount = null, params T[] items)
        {
            int actualStart = start < 0 ? System.Math.Max(0, _list.Count + start) : System.Math.Min(start, _list.Count);
            int actualDeleteCount = deleteCount ?? (_list.Count - actualStart);
            actualDeleteCount = System.Math.Max(0, System.Math.Min(actualDeleteCount, _list.Count - actualStart));

            var deleted = new JSArray<T>();
            for (int i = 0; i < actualDeleteCount; i++)
            {
                deleted.push(_list[actualStart]);
                _list.RemoveAt(actualStart);
            }

            for (int i = 0; i < items.Length; i++)
            {
                _list.Insert(actualStart + i, items[i]);
            }

            return deleted;
        }

        // ==================== Higher-Order Functions ====================

        /// <summary>
        /// Map array elements to new array (value only)
        /// </summary>
        public JSArray<TResult> map<TResult>(Func<T, TResult> callback)
        {
            var result = new JSArray<TResult>(_list.Count);
            for (int i = 0; i < _list.Count; i++)
            {
                result.push(callback(_list[i]));
            }
            return result;
        }

        /// <summary>
        /// Map array elements to new array (value, index)
        /// </summary>
        public JSArray<TResult> map<TResult>(Func<T, int, TResult> callback)
        {
            var result = new JSArray<TResult>(_list.Count);
            for (int i = 0; i < _list.Count; i++)
            {
                result.push(callback(_list[i], i));
            }
            return result;
        }

        /// <summary>
        /// Map array elements to new array (value, index, array)
        /// </summary>
        public JSArray<TResult> map<TResult>(Func<T, int, JSArray<T>, TResult> callback)
        {
            var result = new JSArray<TResult>(_list.Count);
            for (int i = 0; i < _list.Count; i++)
            {
                result.push(callback(_list[i], i, this));
            }
            return result;
        }

        /// <summary>
        /// Filter array elements (value only)
        /// </summary>
        public JSArray<T> filter(Func<T, bool> callback)
        {
            var result = new JSArray<T>();
            for (int i = 0; i < _list.Count; i++)
            {
                if (callback(_list[i]))
                {
                    result.push(_list[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// Filter array elements (value, index)
        /// </summary>
        public JSArray<T> filter(Func<T, int, bool> callback)
        {
            var result = new JSArray<T>();
            for (int i = 0; i < _list.Count; i++)
            {
                if (callback(_list[i], i))
                {
                    result.push(_list[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// Filter array elements (value, index, array)
        /// </summary>
        public JSArray<T> filter(Func<T, int, JSArray<T>, bool> callback)
        {
            var result = new JSArray<T>();
            for (int i = 0; i < _list.Count; i++)
            {
                if (callback(_list[i], i, this))
                {
                    result.push(_list[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// Reduce array to single value (accumulator, value)
        /// </summary>
        public TResult reduce<TResult>(Func<TResult, T, TResult> callback, TResult initialValue)
        {
            TResult accumulator = initialValue;
            for (int i = 0; i < _list.Count; i++)
            {
                accumulator = callback(accumulator, _list[i]);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array to single value (accumulator, value, index)
        /// </summary>
        public TResult reduce<TResult>(Func<TResult, T, int, TResult> callback, TResult initialValue)
        {
            TResult accumulator = initialValue;
            for (int i = 0; i < _list.Count; i++)
            {
                accumulator = callback(accumulator, _list[i], i);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array to single value (accumulator, value, index, array)
        /// </summary>
        public TResult reduce<TResult>(Func<TResult, T, int, JSArray<T>, TResult> callback, TResult initialValue)
        {
            TResult accumulator = initialValue;
            for (int i = 0; i < _list.Count; i++)
            {
                accumulator = callback(accumulator, _list[i], i, this);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array to single value (no initial value)
        /// </summary>
        public T reduce(Func<T, T, T> callback)
        {
            if (_list.Count == 0)
            {
                throw new InvalidOperationException("Reduce of empty array with no initial value");
            }

            T accumulator = _list[0];
            for (int i = 1; i < _list.Count; i++)
            {
                accumulator = callback(accumulator, _list[i]);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array from right to left
        /// </summary>
        public TResult reduceRight<TResult>(Func<TResult, T, TResult> callback, TResult initialValue)
        {
            TResult accumulator = initialValue;
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                accumulator = callback(accumulator, _list[i]);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array from right to left (accumulator, value, index)
        /// </summary>
        public TResult reduceRight<TResult>(Func<TResult, T, int, TResult> callback, TResult initialValue)
        {
            TResult accumulator = initialValue;
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                accumulator = callback(accumulator, _list[i], i);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array from right to left (accumulator, value, index, array)
        /// </summary>
        public TResult reduceRight<TResult>(Func<TResult, T, int, JSArray<T>, TResult> callback, TResult initialValue)
        {
            TResult accumulator = initialValue;
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                accumulator = callback(accumulator, _list[i], i, this);
            }
            return accumulator;
        }

        /// <summary>
        /// Execute callback for each element (value only)
        /// </summary>
        public void forEach(Action<T> callback)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                callback(_list[i]);
            }
        }

        /// <summary>
        /// Execute callback for each element (value, index)
        /// </summary>
        public void forEach(Action<T, int> callback)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                callback(_list[i], i);
            }
        }

        /// <summary>
        /// Execute callback for each element (value, index, array)
        /// </summary>
        public void forEach(Action<T, int, JSArray<T>> callback)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                callback(_list[i], i, this);
            }
        }

        // ==================== Search Methods ====================

        /// <summary>
        /// Find first element matching predicate
        /// </summary>
        public T find(Func<T, bool> callback)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (callback(_list[i]))
                {
                    return _list[i];
                }
            }
            return default(T)!;
        }

        /// <summary>
        /// Find first element matching predicate (value, index)
        /// </summary>
        public T find(Func<T, int, bool> callback)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (callback(_list[i], i))
                {
                    return _list[i];
                }
            }
            return default(T)!;
        }

        /// <summary>
        /// Find first element matching predicate (value, index, array)
        /// </summary>
        public T find(Func<T, int, JSArray<T>, bool> callback)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (callback(_list[i], i, this))
                {
                    return _list[i];
                }
            }
            return default(T)!;
        }

        /// <summary>
        /// Find index of first element matching predicate
        /// </summary>
        public int findIndex(Func<T, bool> callback)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (callback(_list[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find index of first element matching predicate (value, index)
        /// </summary>
        public int findIndex(Func<T, int, bool> callback)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (callback(_list[i], i))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find index of first element matching predicate (value, index, array)
        /// </summary>
        public int findIndex(Func<T, int, JSArray<T>, bool> callback)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (callback(_list[i], i, this))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find last element matching predicate
        /// </summary>
        public T findLast(Func<T, bool> callback)
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (callback(_list[i]))
                {
                    return _list[i];
                }
            }
            return default(T)!;
        }

        /// <summary>
        /// Find last element matching predicate (value, index)
        /// </summary>
        public T findLast(Func<T, int, bool> callback)
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (callback(_list[i], i))
                {
                    return _list[i];
                }
            }
            return default(T)!;
        }

        /// <summary>
        /// Find last element matching predicate (value, index, array)
        /// </summary>
        public T findLast(Func<T, int, JSArray<T>, bool> callback)
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (callback(_list[i], i, this))
                {
                    return _list[i];
                }
            }
            return default(T)!;
        }

        /// <summary>
        /// Find index of last element matching predicate
        /// </summary>
        public int findLastIndex(Func<T, bool> callback)
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (callback(_list[i]))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find index of last element matching predicate (value, index)
        /// </summary>
        public int findLastIndex(Func<T, int, bool> callback)
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (callback(_list[i], i))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find index of last element matching predicate (value, index, array)
        /// </summary>
        public int findLastIndex(Func<T, int, JSArray<T>, bool> callback)
        {
            for (int i = _list.Count - 1; i >= 0; i--)
            {
                if (callback(_list[i], i, this))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find first index of element
        /// </summary>
        public int indexOf(T searchElement, int fromIndex = 0)
        {
            for (int i = fromIndex; i < _list.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(_list[i], searchElement))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find last index of element
        /// </summary>
        public int lastIndexOf(T searchElement, int? fromIndex = null)
        {
            int startIndex = fromIndex ?? _list.Count - 1;
            if (startIndex < 0)
            {
                startIndex = _list.Count + startIndex;
            }
            startIndex = System.Math.Min(startIndex, _list.Count - 1);

            for (int i = startIndex; i >= 0; i--)
            {
                if (EqualityComparer<T>.Default.Equals(_list[i], searchElement))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Check if array includes element
        /// </summary>
        public bool includes(T searchElement)
        {
            return indexOf(searchElement) >= 0;
        }

        /// <summary>
        /// Test if every element matches predicate
        /// </summary>
        public bool every(Func<T, bool> callback)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (!callback(_list[i]))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Test if every element matches predicate (value, index, array)
        /// </summary>
        public bool every(Func<T, int, JSArray<T>, bool> callback)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (!callback(_list[i], i, this))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Test if any element matches predicate
        /// </summary>
        public bool some(Func<T, bool> callback)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (callback(_list[i]))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Test if any element matches predicate (value, index, array)
        /// </summary>
        public bool some(Func<T, int, JSArray<T>, bool> callback)
        {
            for (int i = 0; i < _list.Count; i++)
            {
                if (callback(_list[i], i, this))
                {
                    return true;
                }
            }
            return false;
        }

        // ==================== Sorting Methods ====================

        /// <summary>
        /// Sort array in place and return the array
        /// </summary>
        public JSArray<T> sort(Func<T, T, double>? compareFunc = null)
        {
            if (compareFunc != null)
            {
                _list.Sort((a, b) =>
                {
                    double result = compareFunc(a, b);
                    return result < 0 ? -1 : result > 0 ? 1 : 0;
                });
            }
            else
            {
                _list.Sort((a, b) =>
                {
                    string aStr = a?.ToString() ?? "";
                    string bStr = b?.ToString() ?? "";
                    return string.Compare(aStr, bStr, StringComparison.Ordinal);
                });
            }
            return this;
        }

        /// <summary>
        /// Reverse array in place and return the array
        /// </summary>
        public JSArray<T> reverse()
        {
            _list.Reverse();
            return this;
        }

        // ==================== Conversion Methods ====================

        /// <summary>
        /// Join array elements into string
        /// </summary>
        public string join(string separator = ",")
        {
            var parts = new List<string>();
            for (int i = 0; i < _list.Count; i++)
            {
                parts.Add(_list[i]?.ToString() ?? "");
            }
            return string.Join(separator, parts);
        }

        /// <summary>
        /// Convert to string
        /// </summary>
        public override string ToString()
        {
            return join(",");
        }

        /// <summary>
        /// Convert to locale string
        /// </summary>
        public string toLocaleString()
        {
            return join(",");
        }

        /// <summary>
        /// Concatenate arrays
        /// </summary>
        public JSArray<T> concat(params object[] items)
        {
            var result = new JSArray<T>(_list);

            foreach (var item in items)
            {
                if (item is JSArray<T> jsArr)
                {
                    foreach (var val in jsArr)
                    {
                        result.push(val);
                    }
                }
                else if (item is IEnumerable<T> enumerable)
                {
                    foreach (var val in enumerable)
                    {
                        result.push(val);
                    }
                }
                else if (item is T value)
                {
                    result.push(value);
                }
            }

            return result;
        }

        /// <summary>
        /// Convert to native array
        /// </summary>
        public T[] toArray()
        {
            return _list.ToArray();
        }

        /// <summary>
        /// Convert to List
        /// </summary>
        public List<T> toList()
        {
            return new List<T>(_list);
        }

        // ==================== Iterator Methods ====================

        /// <summary>
        /// Get iterator for [index, value] pairs
        /// </summary>
        public IEnumerable<(int index, T value)> entries()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                yield return (i, _list[i]);
            }
        }

        /// <summary>
        /// Get iterator for keys (indices)
        /// </summary>
        public IEnumerable<int> keys()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Get iterator for values
        /// </summary>
        public IEnumerable<T> values()
        {
            for (int i = 0; i < _list.Count; i++)
            {
                yield return _list[i];
            }
        }

        // ==================== Advanced Methods ====================

        /// <summary>
        /// Get element at index (supports negative indices)
        /// </summary>
        public T at(int index)
        {
            int actualIndex = index < 0 ? _list.Count + index : index;
            if (actualIndex < 0 || actualIndex >= _list.Count)
            {
                return default(T)!;
            }
            return _list[actualIndex];
        }

        /// <summary>
        /// Flatten nested arrays by specified depth
        /// </summary>
        public JSArray<object> flat(int depth = 1)
        {
            var result = new JSArray<object>();
            FlattenHelper(_list, result, depth);
            return result;
        }

        private static void FlattenHelper<TSource>(IEnumerable<TSource> source, JSArray<object> result, int depth)
        {
            foreach (var item in source)
            {
                if (depth > 0 && item != null && item is IEnumerable enumerable && !(item is string))
                {
                    foreach (var nestedItem in enumerable)
                    {
                        var tempList = new List<object> { nestedItem };
                        FlattenHelper(tempList, result, depth - 1);
                    }
                }
                else
                {
                    result.push(item!);
                }
            }
        }

        /// <summary>
        /// Map then flatten result
        /// </summary>
        public JSArray<TResult> flatMap<TResult>(Func<T, int, JSArray<T>, object> callback)
        {
            var result = new JSArray<TResult>();
            for (int i = 0; i < _list.Count; i++)
            {
                var mapped = callback(_list[i], i, this);

                if (mapped is JSArray<TResult> jsArr)
                {
                    foreach (var val in jsArr)
                    {
                        result.push(val);
                    }
                }
                else if (mapped is IEnumerable<TResult> enumerable)
                {
                    foreach (var val in enumerable)
                    {
                        result.push(val);
                    }
                }
                else if (mapped is TResult singleValue)
                {
                    result.push(singleValue);
                }
            }
            return result;
        }

        /// <summary>
        /// Fill array with value
        /// </summary>
        public JSArray<T> fill(T value, int start = 0, int? end = null)
        {
            int actualStart = start < 0 ? System.Math.Max(0, _list.Count + start) : start;
            int actualEnd = end.HasValue
                ? (end.Value < 0 ? _list.Count + end.Value : end.Value)
                : _list.Count;

            for (int i = actualStart; i < actualEnd && i < _list.Count; i++)
            {
                _list[i] = value;
            }
            return this;
        }

        /// <summary>
        /// Copy array section to another location
        /// </summary>
        public JSArray<T> copyWithin(int target, int start = 0, int? end = null)
        {
            int actualTarget = target < 0 ? System.Math.Max(0, _list.Count + target) : target;
            int actualStart = start < 0 ? System.Math.Max(0, _list.Count + start) : start;
            int actualEnd = end.HasValue
                ? (end.Value < 0 ? _list.Count + end.Value : end.Value)
                : _list.Count;

            int count = System.Math.Min(actualEnd - actualStart, _list.Count - actualTarget);

            var temp = new List<T>();
            for (int i = 0; i < count; i++)
            {
                temp.Add(_list[actualStart + i]);
            }

            for (int i = 0; i < count; i++)
            {
                _list[actualTarget + i] = temp[i];
            }

            return this;
        }

        // ==================== Immutable Variants ====================

        /// <summary>
        /// Create new array with element replaced (immutable)
        /// </summary>
        public JSArray<T> with(int index, T value)
        {
            int actualIndex = index < 0 ? _list.Count + index : index;
            if (actualIndex < 0 || actualIndex >= _list.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var result = new JSArray<T>(_list);
            result[actualIndex] = value;
            return result;
        }

        /// <summary>
        /// Create new reversed array (immutable)
        /// </summary>
        public JSArray<T> toReversed()
        {
            var result = new JSArray<T>(_list);
            result.reverse();
            return result;
        }

        /// <summary>
        /// Create new sorted array (immutable)
        /// </summary>
        public JSArray<T> toSorted(Func<T, T, double>? compareFunc = null)
        {
            var result = new JSArray<T>(_list);
            result.sort(compareFunc);
            return result;
        }

        /// <summary>
        /// Create new spliced array (immutable)
        /// </summary>
        public JSArray<T> toSpliced(int start, int? deleteCount = null, params T[] items)
        {
            var result = new JSArray<T>(_list);
            result.splice(start, deleteCount, items);
            return result;
        }

        // ==================== Static Factory Methods ====================

        /// <summary>
        /// Check if value is a JSArray
        /// </summary>
        public static bool isArray(object? value)
        {
            if (value == null) return false;
            Type type = value.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(JSArray<>);
        }

        /// <summary>
        /// Create array from iterable
        /// </summary>
        public static JSArray<T> from(IEnumerable<T> iterable)
        {
            return new JSArray<T>(iterable);
        }

        /// <summary>
        /// Create array from iterable with map function
        /// </summary>
        public static JSArray<TResult> from<TSource, TResult>(IEnumerable<TSource> iterable, Func<TSource, int, TResult> mapFunc)
        {
            var result = new JSArray<TResult>();
            int index = 0;
            foreach (var item in iterable)
            {
                result.push(mapFunc(item, index++));
            }
            return result;
        }

        /// <summary>
        /// Create array from arguments
        /// </summary>
        public static JSArray<T> of(params T[] items)
        {
            return new JSArray<T>(items);
        }

        // ==================== IEnumerable Implementation ====================

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
