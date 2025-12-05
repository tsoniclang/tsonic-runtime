/**
 * JavaScript Map implementation
 * Wraps native .NET Dictionary<K,V> with JavaScript Map semantics
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tsonic.JSRuntime
{
    /// <summary>
    /// JavaScript Map - key-value collection with insertion order preservation
    /// </summary>
    public class Map<K, V> : IEnumerable<KeyValuePair<K, V>> where K : notnull
    {
        private readonly Dictionary<K, V> _dict = new();

        // ==================== Constructors ====================

        /// <summary>
        /// Create empty Map
        /// </summary>
        public Map() { }

        /// <summary>
        /// Create Map from key-value pairs
        /// </summary>
        public Map(IEnumerable<(K key, V value)> entries)
        {
            foreach (var (key, value) in entries)
            {
                _dict[key] = value;
            }
        }

        /// <summary>
        /// Create Map from KeyValuePairs
        /// </summary>
        public Map(IEnumerable<KeyValuePair<K, V>> entries)
        {
            foreach (var kvp in entries)
            {
                _dict[kvp.Key] = kvp.Value;
            }
        }

        // ==================== Properties ====================

        /// <summary>
        /// Number of key-value pairs in the Map
        /// </summary>
        public int size => _dict.Count;

        // ==================== Core Methods ====================

        /// <summary>
        /// Get value for key, or default if not found
        /// </summary>
        public V? get(K key)
        {
            return _dict.TryGetValue(key, out var value) ? value : default;
        }

        /// <summary>
        /// Set value for key, returns the Map for chaining
        /// </summary>
        public Map<K, V> set(K key, V value)
        {
            _dict[key] = value;
            return this;
        }

        /// <summary>
        /// Check if key exists in Map
        /// </summary>
        public bool has(K key)
        {
            return _dict.ContainsKey(key);
        }

        /// <summary>
        /// Delete key from Map, returns true if key existed
        /// </summary>
        public bool delete(K key)
        {
            return _dict.Remove(key);
        }

        /// <summary>
        /// Remove all key-value pairs from Map
        /// </summary>
        public void clear()
        {
            _dict.Clear();
        }

        // ==================== Iteration Methods ====================

        /// <summary>
        /// Get all keys in insertion order
        /// </summary>
        public IEnumerable<K> keys()
        {
            return _dict.Keys;
        }

        /// <summary>
        /// Get all values in insertion order
        /// </summary>
        public IEnumerable<V> values()
        {
            return _dict.Values;
        }

        /// <summary>
        /// Get all key-value pairs as tuples in insertion order
        /// </summary>
        public IEnumerable<(K key, V value)> entries()
        {
            foreach (var kvp in _dict)
            {
                yield return (kvp.Key, kvp.Value);
            }
        }

        /// <summary>
        /// Execute callback for each key-value pair
        /// </summary>
        public void forEach(Action<V, K, Map<K, V>> callback)
        {
            foreach (var kvp in _dict)
            {
                callback(kvp.Value, kvp.Key, this);
            }
        }

        /// <summary>
        /// Execute callback for each key-value pair (value and key only)
        /// </summary>
        public void forEach(Action<V, K> callback)
        {
            foreach (var kvp in _dict)
            {
                callback(kvp.Value, kvp.Key);
            }
        }

        /// <summary>
        /// Execute callback for each value
        /// </summary>
        public void forEach(Action<V> callback)
        {
            foreach (var kvp in _dict)
            {
                callback(kvp.Value);
            }
        }

        // ==================== IEnumerable Implementation ====================

        /// <summary>
        /// Get enumerator for iterating key-value pairs
        /// </summary>
        public IEnumerator<KeyValuePair<K, V>> GetEnumerator()
        {
            return _dict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
