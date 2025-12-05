/**
 * JavaScript Set implementation
 * Wraps native .NET HashSet<T> with JavaScript Set semantics
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tsonic.JSRuntime
{
    /// <summary>
    /// JavaScript Set - unique value collection with insertion order preservation
    /// </summary>
    public class Set<T> : IEnumerable<T>
    {
        private readonly HashSet<T> _set = new();

        // ==================== Constructors ====================

        /// <summary>
        /// Create empty Set
        /// </summary>
        public Set() { }

        /// <summary>
        /// Create Set from values
        /// </summary>
        public Set(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                _set.Add(value);
            }
        }

        // ==================== Properties ====================

        /// <summary>
        /// Number of unique values in the Set
        /// </summary>
        public int size => _set.Count;

        // ==================== Core Methods ====================

        /// <summary>
        /// Add value to Set, returns the Set for chaining
        /// </summary>
        public Set<T> add(T value)
        {
            _set.Add(value);
            return this;
        }

        /// <summary>
        /// Check if value exists in Set
        /// </summary>
        public bool has(T value)
        {
            return _set.Contains(value);
        }

        /// <summary>
        /// Delete value from Set, returns true if value existed
        /// </summary>
        public bool delete(T value)
        {
            return _set.Remove(value);
        }

        /// <summary>
        /// Remove all values from Set
        /// </summary>
        public void clear()
        {
            _set.Clear();
        }

        // ==================== Iteration Methods ====================

        /// <summary>
        /// Get all values (same as values() in JS Set)
        /// </summary>
        public IEnumerable<T> keys()
        {
            return _set;
        }

        /// <summary>
        /// Get all values
        /// </summary>
        public IEnumerable<T> values()
        {
            return _set;
        }

        /// <summary>
        /// Get all entries as (value, value) tuples (JS Set behavior)
        /// </summary>
        public IEnumerable<(T, T)> entries()
        {
            foreach (var value in _set)
            {
                yield return (value, value);
            }
        }

        /// <summary>
        /// Execute callback for each value
        /// </summary>
        public void forEach(Action<T, T, Set<T>> callback)
        {
            foreach (var value in _set)
            {
                callback(value, value, this);
            }
        }

        /// <summary>
        /// Execute callback for each value (value twice, matching JS)
        /// </summary>
        public void forEach(Action<T, T> callback)
        {
            foreach (var value in _set)
            {
                callback(value, value);
            }
        }

        /// <summary>
        /// Execute callback for each value
        /// </summary>
        public void forEach(Action<T> callback)
        {
            foreach (var value in _set)
            {
                callback(value);
            }
        }

        // ==================== Set Operations ====================

        /// <summary>
        /// Return new Set with values in this Set but not in other
        /// </summary>
        public Set<T> difference(Set<T> other)
        {
            var result = new Set<T>();
            foreach (var value in _set)
            {
                if (!other.has(value))
                {
                    result.add(value);
                }
            }
            return result;
        }

        /// <summary>
        /// Return new Set with values in both Sets
        /// </summary>
        public Set<T> intersection(Set<T> other)
        {
            var result = new Set<T>();
            foreach (var value in _set)
            {
                if (other.has(value))
                {
                    result.add(value);
                }
            }
            return result;
        }

        /// <summary>
        /// Return new Set with values in either Set
        /// </summary>
        public Set<T> union(Set<T> other)
        {
            var result = new Set<T>(_set);
            foreach (var value in other)
            {
                result.add(value);
            }
            return result;
        }

        /// <summary>
        /// Return new Set with values in either Set but not both
        /// </summary>
        public Set<T> symmetricDifference(Set<T> other)
        {
            var result = new Set<T>();
            foreach (var value in _set)
            {
                if (!other.has(value))
                {
                    result.add(value);
                }
            }
            foreach (var value in other)
            {
                if (!this.has(value))
                {
                    result.add(value);
                }
            }
            return result;
        }

        /// <summary>
        /// Check if this Set is a subset of other
        /// </summary>
        public bool isSubsetOf(Set<T> other)
        {
            foreach (var value in _set)
            {
                if (!other.has(value))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Check if this Set is a superset of other
        /// </summary>
        public bool isSupersetOf(Set<T> other)
        {
            return other.isSubsetOf(this);
        }

        /// <summary>
        /// Check if this Set has no values in common with other
        /// </summary>
        public bool isDisjointFrom(Set<T> other)
        {
            foreach (var value in _set)
            {
                if (other.has(value))
                {
                    return false;
                }
            }
            return true;
        }

        // ==================== IEnumerable Implementation ====================

        /// <summary>
        /// Get enumerator for iterating values
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return _set.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
