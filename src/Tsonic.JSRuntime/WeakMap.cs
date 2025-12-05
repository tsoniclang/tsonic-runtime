/**
 * JavaScript WeakMap implementation
 * Wraps native .NET ConditionalWeakTable<K,V> with JavaScript WeakMap semantics
 */

using System;
using System.Runtime.CompilerServices;

namespace Tsonic.JSRuntime
{
    /// <summary>
    /// JavaScript WeakMap - key-value collection with weakly-referenced keys
    /// Keys must be reference types and can be garbage collected
    /// </summary>
    public class WeakMap<K, V> where K : class
    {
        private readonly ConditionalWeakTable<K, StrongBox<V>> _table = new();

        // ==================== Constructors ====================

        /// <summary>
        /// Create empty WeakMap
        /// </summary>
        public WeakMap() { }

        // ==================== Core Methods ====================

        /// <summary>
        /// Get value for key, or default if not found
        /// </summary>
        public V? get(K key)
        {
            if (_table.TryGetValue(key, out var box))
            {
                return box.Value;
            }
            return default;
        }

        /// <summary>
        /// Set value for key, returns the WeakMap for chaining
        /// </summary>
        public WeakMap<K, V> set(K key, V value)
        {
            // Remove existing if present, then add new
            _table.Remove(key);
            _table.Add(key, new StrongBox<V>(value));
            return this;
        }

        /// <summary>
        /// Check if key exists in WeakMap
        /// </summary>
        public bool has(K key)
        {
            return _table.TryGetValue(key, out _);
        }

        /// <summary>
        /// Delete key from WeakMap, returns true if key existed
        /// </summary>
        public bool delete(K key)
        {
            return _table.Remove(key);
        }

        // Note: WeakMap is intentionally not iterable (matches JavaScript)
        // No keys(), values(), entries(), forEach(), size, or clear()
    }
}
