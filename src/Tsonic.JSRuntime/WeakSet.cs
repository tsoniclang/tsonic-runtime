/**
 * JavaScript WeakSet implementation
 * Wraps native .NET ConditionalWeakTable with JavaScript WeakSet semantics
 */

using System;
using System.Runtime.CompilerServices;

namespace Tsonic.JSRuntime
{
    /// <summary>
    /// JavaScript WeakSet - unique value collection with weakly-referenced values
    /// Values must be reference types and can be garbage collected
    /// </summary>
    public class WeakSet<T> where T : class
    {
        private readonly ConditionalWeakTable<T, object> _table = new();
        private static readonly object Marker = new();

        // ==================== Constructors ====================

        /// <summary>
        /// Create empty WeakSet
        /// </summary>
        public WeakSet() { }

        // ==================== Core Methods ====================

        /// <summary>
        /// Add value to WeakSet, returns the WeakSet for chaining
        /// </summary>
        public WeakSet<T> add(T value)
        {
            // Remove existing if present, then add new
            _table.Remove(value);
            _table.Add(value, Marker);
            return this;
        }

        /// <summary>
        /// Check if value exists in WeakSet
        /// </summary>
        public bool has(T value)
        {
            return _table.TryGetValue(value, out _);
        }

        /// <summary>
        /// Delete value from WeakSet, returns true if value existed
        /// </summary>
        public bool delete(T value)
        {
            return _table.Remove(value);
        }

        // Note: WeakSet is intentionally not iterable (matches JavaScript)
        // No keys(), values(), entries(), forEach(), size, or clear()
    }
}
