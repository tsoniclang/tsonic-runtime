/**
 * JavaScript Array extension methods
 * Operates on native .NET List<T> type
 */

using System;
using System.Collections.Generic;
using System.Linq;

namespace Tsonic.JSRuntime
{
    /// <summary>
    /// Extension methods for JavaScript array operations on List&lt;T&gt;
    /// </summary>
    public static class Array
    {
        // ==================== Index Access Methods ====================

        /// <summary>
        /// Get element at index (safe, returns default for out-of-bounds)
        /// </summary>
        public static T get<T>(this List<T> arr, int index)
        {
            if (index < 0 || index >= arr.Count)
            {
                return default(T)!;
            }
            return arr[index];
        }

        /// <summary>
        /// Set element at index (fills gaps with default for sparse arrays)
        /// </summary>
        public static void set<T>(this List<T> arr, int index, T value)
        {
            if (index < 0)
            {
                throw new ArgumentException("Array index cannot be negative", nameof(index));
            }

            // Fill gaps with default(T) if index is beyond current length
            while (arr.Count <= index)
            {
                arr.Add(default(T)!);
            }

            // Set the value
            arr[index] = value;
        }

        /// <summary>
        /// Get array length (JavaScript 'length' property)
        /// </summary>
        public static int length<T>(this List<T> arr)
        {
            return arr.Count;
        }

        /// <summary>
        /// Set array length (truncate or extend with defaults)
        /// </summary>
        public static void setLength<T>(this List<T> arr, int newLength)
        {
            if (newLength < 0)
            {
                throw new ArgumentException("Invalid array length", nameof(newLength));
            }

            if (newLength < arr.Count)
            {
                // Truncate
                arr.RemoveRange(newLength, arr.Count - newLength);
            }
            else if (newLength > arr.Count)
            {
                // Extend with default values
                int toAdd = newLength - arr.Count;
                for (int i = 0; i < toAdd; i++)
                {
                    arr.Add(default(T)!);
                }
            }
        }

        // ==================== Basic Mutation Methods ====================

        /// <summary>
        /// Add element to end of array
        /// </summary>
        public static void push<T>(this List<T> arr, T item)
        {
            arr.Add(item);
        }

        /// <summary>
        /// Remove and return last element
        /// </summary>
        public static T pop<T>(this List<T> arr)
        {
            if (arr.Count == 0)
            {
                return default(T)!;
            }

            T item = arr[arr.Count - 1];
            arr.RemoveAt(arr.Count - 1);
            return item;
        }

        /// <summary>
        /// Remove and return first element
        /// </summary>
        public static T shift<T>(this List<T> arr)
        {
            if (arr.Count == 0)
            {
                return default(T)!;
            }

            T item = arr[0];
            arr.RemoveAt(0);
            return item;
        }

        /// <summary>
        /// Add element to beginning of array
        /// </summary>
        public static void unshift<T>(this List<T> arr, T item)
        {
            arr.Insert(0, item);
        }

        // ==================== Slicing Methods ====================

        /// <summary>
        /// Return shallow copy of portion of array
        /// </summary>
        public static List<T> slice<T>(this List<T> arr, int start = 0, int? end = null)
        {
            int actualStart = start < 0 ? System.Math.Max(0, arr.Count + start) : start;
            int actualEnd = end.HasValue
                ? (end.Value < 0 ? System.Math.Max(0, arr.Count + end.Value) : end.Value)
                : arr.Count;

            actualStart = System.Math.Min(actualStart, arr.Count);
            actualEnd = System.Math.Min(actualEnd, arr.Count);

            if (actualStart >= actualEnd)
            {
                return new List<T>();
            }

            return arr.GetRange(actualStart, actualEnd - actualStart);
        }

        /// <summary>
        /// Add/remove elements at position
        /// </summary>
        public static List<T> splice<T>(this List<T> arr, int start, int? deleteCount = null, params T[] items)
        {
            int actualStart = start < 0 ? System.Math.Max(0, arr.Count + start) : System.Math.Min(start, arr.Count);
            int actualDeleteCount = deleteCount ?? (arr.Count - actualStart);
            actualDeleteCount = System.Math.Max(0, System.Math.Min(actualDeleteCount, arr.Count - actualStart));

            // Extract deleted elements
            var deleted = new List<T>();
            for (int i = 0; i < actualDeleteCount; i++)
            {
                deleted.Add(arr[actualStart]);
                arr.RemoveAt(actualStart);
            }

            // Insert new items
            for (int i = 0; i < items.Length; i++)
            {
                arr.Insert(actualStart + i, items[i]);
            }

            return deleted;
        }

        // ==================== Higher-Order Functions ====================

        /// <summary>
        /// Map array elements to new array (value only)
        /// </summary>
        public static List<TResult> map<T, TResult>(this List<T> arr, Func<T, TResult> callback)
        {
            var result = new List<TResult>(arr.Count);
            for (int i = 0; i < arr.Count; i++)
            {
                result.Add(callback(arr[i]));
            }
            return result;
        }

        /// <summary>
        /// Map array elements to new array (value, index)
        /// </summary>
        public static List<TResult> map<T, TResult>(this List<T> arr, Func<T, int, TResult> callback)
        {
            var result = new List<TResult>(arr.Count);
            for (int i = 0; i < arr.Count; i++)
            {
                result.Add(callback(arr[i], i));
            }
            return result;
        }

        /// <summary>
        /// Map array elements to new array (value, index, array)
        /// </summary>
        public static List<TResult> map<T, TResult>(this List<T> arr, Func<T, int, List<T>, TResult> callback)
        {
            var result = new List<TResult>(arr.Count);
            for (int i = 0; i < arr.Count; i++)
            {
                result.Add(callback(arr[i], i, arr));
            }
            return result;
        }

        /// <summary>
        /// Filter array elements (value only)
        /// </summary>
        public static List<T> filter<T>(this List<T> arr, Func<T, bool> callback)
        {
            var result = new List<T>();
            for (int i = 0; i < arr.Count; i++)
            {
                if (callback(arr[i]))
                {
                    result.Add(arr[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// Filter array elements (value, index)
        /// </summary>
        public static List<T> filter<T>(this List<T> arr, Func<T, int, bool> callback)
        {
            var result = new List<T>();
            for (int i = 0; i < arr.Count; i++)
            {
                if (callback(arr[i], i))
                {
                    result.Add(arr[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// Filter array elements (value, index, array)
        /// </summary>
        public static List<T> filter<T>(this List<T> arr, Func<T, int, List<T>, bool> callback)
        {
            var result = new List<T>();
            for (int i = 0; i < arr.Count; i++)
            {
                if (callback(arr[i], i, arr))
                {
                    result.Add(arr[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// Reduce array to single value (accumulator, value)
        /// </summary>
        public static TResult reduce<T, TResult>(this List<T> arr, Func<TResult, T, TResult> callback, TResult initialValue)
        {
            TResult accumulator = initialValue;
            for (int i = 0; i < arr.Count; i++)
            {
                accumulator = callback(accumulator, arr[i]);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array to single value (accumulator, value, index)
        /// </summary>
        public static TResult reduce<T, TResult>(this List<T> arr, Func<TResult, T, int, TResult> callback, TResult initialValue)
        {
            TResult accumulator = initialValue;
            for (int i = 0; i < arr.Count; i++)
            {
                accumulator = callback(accumulator, arr[i], i);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array to single value (accumulator, value, index, array)
        /// </summary>
        public static TResult reduce<T, TResult>(this List<T> arr, Func<TResult, T, int, List<T>, TResult> callback, TResult initialValue)
        {
            TResult accumulator = initialValue;
            for (int i = 0; i < arr.Count; i++)
            {
                accumulator = callback(accumulator, arr[i], i, arr);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array to single value (no initial value, accumulator, value)
        /// </summary>
        public static T reduce<T>(this List<T> arr, Func<T, T, T> callback)
        {
            if (arr.Count == 0)
            {
                throw new InvalidOperationException("Reduce of empty array with no initial value");
            }

            T accumulator = arr[0];
            for (int i = 1; i < arr.Count; i++)
            {
                accumulator = callback(accumulator, arr[i]);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array to single value (no initial value, accumulator, value, index)
        /// </summary>
        public static T reduce<T>(this List<T> arr, Func<T, T, int, T> callback)
        {
            if (arr.Count == 0)
            {
                throw new InvalidOperationException("Reduce of empty array with no initial value");
            }

            T accumulator = arr[0];
            for (int i = 1; i < arr.Count; i++)
            {
                accumulator = callback(accumulator, arr[i], i);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array to single value (no initial value, accumulator, value, index, array)
        /// </summary>
        public static T reduce<T>(this List<T> arr, Func<T, T, int, List<T>, T> callback)
        {
            if (arr.Count == 0)
            {
                throw new InvalidOperationException("Reduce of empty array with no initial value");
            }

            T accumulator = arr[0];
            for (int i = 1; i < arr.Count; i++)
            {
                accumulator = callback(accumulator, arr[i], i, arr);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array from right to left (accumulator, value)
        /// </summary>
        public static TResult reduceRight<T, TResult>(this List<T> arr, Func<TResult, T, TResult> callback, TResult initialValue)
        {
            TResult accumulator = initialValue;
            for (int i = arr.Count - 1; i >= 0; i--)
            {
                accumulator = callback(accumulator, arr[i]);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array from right to left (accumulator, value, index)
        /// </summary>
        public static TResult reduceRight<T, TResult>(this List<T> arr, Func<TResult, T, int, TResult> callback, TResult initialValue)
        {
            TResult accumulator = initialValue;
            for (int i = arr.Count - 1; i >= 0; i--)
            {
                accumulator = callback(accumulator, arr[i], i);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array from right to left (accumulator, value, index, array)
        /// </summary>
        public static TResult reduceRight<T, TResult>(this List<T> arr, Func<TResult, T, int, List<T>, TResult> callback, TResult initialValue)
        {
            TResult accumulator = initialValue;
            for (int i = arr.Count - 1; i >= 0; i--)
            {
                accumulator = callback(accumulator, arr[i], i, arr);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array from right to left (no initial value, accumulator, value)
        /// </summary>
        public static T reduceRight<T>(this List<T> arr, Func<T, T, T> callback)
        {
            if (arr.Count == 0)
            {
                throw new InvalidOperationException("Reduce of empty array with no initial value");
            }

            T accumulator = arr[arr.Count - 1];
            for (int i = arr.Count - 2; i >= 0; i--)
            {
                accumulator = callback(accumulator, arr[i]);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array from right to left (no initial value, accumulator, value, index)
        /// </summary>
        public static T reduceRight<T>(this List<T> arr, Func<T, T, int, T> callback)
        {
            if (arr.Count == 0)
            {
                throw new InvalidOperationException("Reduce of empty array with no initial value");
            }

            T accumulator = arr[arr.Count - 1];
            for (int i = arr.Count - 2; i >= 0; i--)
            {
                accumulator = callback(accumulator, arr[i], i);
            }
            return accumulator;
        }

        /// <summary>
        /// Reduce array from right to left (no initial value, accumulator, value, index, array)
        /// </summary>
        public static T reduceRight<T>(this List<T> arr, Func<T, T, int, List<T>, T> callback)
        {
            if (arr.Count == 0)
            {
                throw new InvalidOperationException("Reduce of empty array with no initial value");
            }

            T accumulator = arr[arr.Count - 1];
            for (int i = arr.Count - 2; i >= 0; i--)
            {
                accumulator = callback(accumulator, arr[i], i, arr);
            }
            return accumulator;
        }

        /// <summary>
        /// Execute callback for each element (value only)
        /// </summary>
        public static void forEach<T>(this List<T> arr, Action<T> callback)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                callback(arr[i]);
            }
        }

        /// <summary>
        /// Execute callback for each element (value, index)
        /// </summary>
        public static void forEach<T>(this List<T> arr, Action<T, int> callback)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                callback(arr[i], i);
            }
        }

        /// <summary>
        /// Execute callback for each element (value, index, array)
        /// </summary>
        public static void forEach<T>(this List<T> arr, Action<T, int, List<T>> callback)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                callback(arr[i], i, arr);
            }
        }

        // ==================== Search Methods ====================

        /// <summary>
        /// Find first element matching predicate
        /// </summary>
        public static T find<T>(this List<T> arr, Func<T, int, List<T>, bool> callback)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                if (callback(arr[i], i, arr))
                {
                    return arr[i];
                }
            }
            return default(T)!;
        }

        /// <summary>
        /// Find index of first element matching predicate
        /// </summary>
        public static int findIndex<T>(this List<T> arr, Func<T, int, List<T>, bool> callback)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                if (callback(arr[i], i, arr))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find last element matching predicate
        /// </summary>
        public static T findLast<T>(this List<T> arr, Func<T, int, List<T>, bool> callback)
        {
            for (int i = arr.Count - 1; i >= 0; i--)
            {
                if (callback(arr[i], i, arr))
                {
                    return arr[i];
                }
            }
            return default(T)!;
        }

        /// <summary>
        /// Find index of last element matching predicate
        /// </summary>
        public static int findLastIndex<T>(this List<T> arr, Func<T, int, List<T>, bool> callback)
        {
            for (int i = arr.Count - 1; i >= 0; i--)
            {
                if (callback(arr[i], i, arr))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find first index of element
        /// </summary>
        public static int indexOf<T>(this List<T> arr, T searchElement, int fromIndex = 0)
        {
            for (int i = fromIndex; i < arr.Count; i++)
            {
                if (EqualityComparer<T>.Default.Equals(arr[i], searchElement))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Find last index of element
        /// </summary>
        public static int lastIndexOf<T>(this List<T> arr, T searchElement, int? fromIndex = null)
        {
            int startIndex = fromIndex ?? arr.Count - 1;
            if (startIndex < 0)
            {
                startIndex = arr.Count + startIndex;
            }
            startIndex = System.Math.Min(startIndex, arr.Count - 1);

            for (int i = startIndex; i >= 0; i--)
            {
                if (EqualityComparer<T>.Default.Equals(arr[i], searchElement))
                {
                    return i;
                }
            }
            return -1;
        }

        /// <summary>
        /// Check if array includes element
        /// </summary>
        public static bool includes<T>(this List<T> arr, T searchElement)
        {
            return arr.indexOf(searchElement) >= 0;
        }

        /// <summary>
        /// Test if every element matches predicate
        /// </summary>
        public static bool every<T>(this List<T> arr, Func<T, int, List<T>, bool> callback)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                if (!callback(arr[i], i, arr))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Test if any element matches predicate
        /// </summary>
        public static bool some<T>(this List<T> arr, Func<T, int, List<T>, bool> callback)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                if (callback(arr[i], i, arr))
                {
                    return true;
                }
            }
            return false;
        }

        // ==================== Sorting Methods ====================

        /// <summary>
        /// Sort array in place and return the array (matches JavaScript behavior)
        /// </summary>
        public static List<T> sort<T>(this List<T> arr, Func<T, T, double>? compareFunc = null)
        {
            if (compareFunc != null)
            {
                arr.Sort((a, b) =>
                {
                    double result = compareFunc(a, b);
                    return result < 0 ? -1 : result > 0 ? 1 : 0;
                });
            }
            else
            {
                arr.Sort((a, b) =>
                {
                    string aStr = a?.ToString() ?? "";
                    string bStr = b?.ToString() ?? "";
                    return string.Compare(aStr, bStr, StringComparison.Ordinal);
                });
            }
            return arr;
        }

        /// <summary>
        /// Reverse array in place and return the array (matches JavaScript behavior)
        /// </summary>
        public static List<T> reverse<T>(this List<T> arr)
        {
            arr.Reverse();
            return arr;
        }

        // ==================== Conversion Methods ====================

        /// <summary>
        /// Join array elements into string
        /// </summary>
        public static string join<T>(this List<T> arr, string separator = ",")
        {
            var parts = new List<string>();
            for (int i = 0; i < arr.Count; i++)
            {
                parts.Add(arr[i]?.ToString() ?? "");
            }
            return string.Join(separator, parts);
        }

        /// <summary>
        /// Convert to string
        /// </summary>
        public static string toString<T>(this List<T> arr)
        {
            return arr.join(",");
        }

        /// <summary>
        /// Convert to locale string
        /// </summary>
        public static string toLocaleString<T>(this List<T> arr)
        {
            return arr.join(",");
        }

        /// <summary>
        /// Concatenate arrays
        /// </summary>
        public static List<T> concat<T>(this List<T> arr, params object[] items)
        {
            var result = new List<T>(arr);

            foreach (var item in items)
            {
                if (item is List<T> list)
                {
                    result.AddRange(list);
                }
                else if (item is T value)
                {
                    result.Add(value);
                }
            }

            return result;
        }

        // ==================== Iterator Methods ====================

        /// <summary>
        /// Get iterator for [index, value] pairs
        /// </summary>
        public static IEnumerable<(int index, T value)> entries<T>(this List<T> arr)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                yield return (i, arr[i]);
            }
        }

        /// <summary>
        /// Get iterator for keys (indices)
        /// </summary>
        public static IEnumerable<int> keys<T>(this List<T> arr)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                yield return i;
            }
        }

        /// <summary>
        /// Get iterator for values
        /// </summary>
        public static IEnumerable<T> values<T>(this List<T> arr)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                yield return arr[i];
            }
        }

        // ==================== Advanced Methods ====================

        /// <summary>
        /// Get element at index (supports negative indices)
        /// </summary>
        public static T at<T>(this List<T> arr, int index)
        {
            int actualIndex = index < 0 ? arr.Count + index : index;
            if (actualIndex < 0 || actualIndex >= arr.Count)
            {
                return default(T)!;
            }
            return arr[actualIndex];
        }

        /// <summary>
        /// Flatten nested arrays by specified depth
        /// </summary>
        public static List<object> flat<T>(this List<T> arr, int depth = 1)
        {
            var result = new List<object>();
            FlattenHelper(arr, result, depth);
            return result;
        }

        private static void FlattenHelper<T>(List<T> arr, List<object> result, int depth)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                var item = arr[i];
                if (depth > 0 && item != null && item is System.Collections.IEnumerable enumerable && !(item is string))
                {
                    // Recursive flatten for nested enumerables
                    foreach (var nestedItem in enumerable)
                    {
                        var tempList = new List<object> { nestedItem };
                        FlattenHelper(tempList, result, depth - 1);
                    }
                }
                else
                {
                    result.Add(item!);
                }
            }
        }

        /// <summary>
        /// Map then flatten result
        /// </summary>
        public static List<TResult> flatMap<T, TResult>(this List<T> arr, Func<T, int, List<T>, object> callback)
        {
            var result = new List<TResult>();
            for (int i = 0; i < arr.Count; i++)
            {
                var mapped = callback(arr[i], i, arr);

                if (mapped is List<TResult> list)
                {
                    result.AddRange(list);
                }
                else if (mapped is TResult singleValue)
                {
                    result.Add(singleValue);
                }
            }
            return result;
        }

        /// <summary>
        /// Fill array with value
        /// </summary>
        public static List<T> fill<T>(this List<T> arr, T value, int start = 0, int? end = null)
        {
            int actualStart = start < 0 ? System.Math.Max(0, arr.Count + start) : start;
            int actualEnd = end.HasValue
                ? (end.Value < 0 ? arr.Count + end.Value : end.Value)
                : arr.Count;

            for (int i = actualStart; i < actualEnd && i < arr.Count; i++)
            {
                arr[i] = value;
            }
            return arr;
        }

        /// <summary>
        /// Copy array section to another location
        /// </summary>
        public static List<T> copyWithin<T>(this List<T> arr, int target, int start = 0, int? end = null)
        {
            int actualTarget = target < 0 ? System.Math.Max(0, arr.Count + target) : target;
            int actualStart = start < 0 ? System.Math.Max(0, arr.Count + start) : start;
            int actualEnd = end.HasValue
                ? (end.Value < 0 ? arr.Count + end.Value : end.Value)
                : arr.Count;

            int count = System.Math.Min(actualEnd - actualStart, arr.Count - actualTarget);

            // Copy to temporary list to handle overlapping ranges
            var temp = new List<T>();
            for (int i = 0; i < count; i++)
            {
                temp.Add(arr[actualStart + i]);
            }

            // Copy back to target location
            for (int i = 0; i < count; i++)
            {
                arr[actualTarget + i] = temp[i];
            }

            return arr;
        }

        // ==================== Immutable Variants ====================

        /// <summary>
        /// Create new array with element replaced (immutable)
        /// </summary>
        public static List<T> @with<T>(this List<T> arr, int index, T value)
        {
            int actualIndex = index < 0 ? arr.Count + index : index;
            if (actualIndex < 0 || actualIndex >= arr.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            var result = new List<T>(arr);
            result[actualIndex] = value;
            return result;
        }

        /// <summary>
        /// Create new reversed array (immutable)
        /// </summary>
        public static List<T> toReversed<T>(this List<T> arr)
        {
            var result = new List<T>(arr);
            result.Reverse();
            return result;
        }

        /// <summary>
        /// Create new sorted array (immutable)
        /// </summary>
        public static List<T> toSorted<T>(this List<T> arr, Func<T, T, double>? compareFunc = null)
        {
            var result = new List<T>(arr);
            result.sort(compareFunc);
            return result;
        }

        /// <summary>
        /// Create new spliced array (immutable)
        /// </summary>
        public static List<T> toSpliced<T>(this List<T> arr, int start, int? deleteCount = null, params T[] items)
        {
            var result = new List<T>(arr);
            result.splice(start, deleteCount, items);
            return result;
        }

        // ==================== Static Factory Methods ====================

        /// <summary>
        /// Check if value is an array
        /// </summary>
        public static bool isArray(object? value)
        {
            if (value == null) return false;
            Type type = value.GetType();
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>);
        }

        /// <summary>
        /// Create array from iterable
        /// </summary>
        public static List<T> from<T>(IEnumerable<T> iterable)
        {
            return new List<T>(iterable);
        }

        /// <summary>
        /// Create array from iterable with map function
        /// </summary>
        public static List<TResult> from<TSource, TResult>(IEnumerable<TSource> iterable, Func<TSource, int, TResult> mapFunc)
        {
            var result = new List<TResult>();
            int index = 0;
            foreach (var item in iterable)
            {
                result.Add(mapFunc(item, index++));
            }
            return result;
        }

        /// <summary>
        /// Create array from arguments
        /// </summary>
        public static List<T> of<T>(params T[] items)
        {
            return new List<T>(items);
        }
    }
}
