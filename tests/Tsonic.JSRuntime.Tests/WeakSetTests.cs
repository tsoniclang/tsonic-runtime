using System;
using Xunit;

namespace Tsonic.JSRuntime.Tests
{
    public class WeakSetTests
    {
        // ==================== Constructor Tests ====================

        [Fact]
        public void Constructor_CreatesEmptyWeakSet()
        {
            var set = new WeakSet<object>();
            var value = new object();
            Assert.False(set.has(value));
        }

        // ==================== add Tests ====================

        [Fact]
        public void add_NewValue_AddsToSet()
        {
            var set = new WeakSet<object>();
            var value = new object();
            set.add(value);
            Assert.True(set.has(value));
        }

        [Fact]
        public void add_DuplicateValue_DoesNotError()
        {
            var set = new WeakSet<object>();
            var value = new object();
            set.add(value);
            set.add(value);  // Should not throw
            Assert.True(set.has(value));
        }

        [Fact]
        public void add_ReturnsWeakSetForChaining()
        {
            var set = new WeakSet<object>();
            var v1 = new object();
            var v2 = new object();
            var result = set.add(v1).add(v2);
            Assert.Same(set, result);
        }

        // ==================== has Tests ====================

        [Fact]
        public void has_ExistingValue_ReturnsTrue()
        {
            var set = new WeakSet<object>();
            var value = new object();
            set.add(value);
            Assert.True(set.has(value));
        }

        [Fact]
        public void has_NonExistingValue_ReturnsFalse()
        {
            var set = new WeakSet<object>();
            var value = new object();
            Assert.False(set.has(value));
        }

        [Fact]
        public void has_AfterDelete_ReturnsFalse()
        {
            var set = new WeakSet<object>();
            var value = new object();
            set.add(value);
            set.delete(value);
            Assert.False(set.has(value));
        }

        // ==================== delete Tests ====================

        [Fact]
        public void delete_ExistingValue_ReturnsTrue()
        {
            var set = new WeakSet<object>();
            var value = new object();
            set.add(value);
            Assert.True(set.delete(value));
        }

        [Fact]
        public void delete_NonExistingValue_ReturnsFalse()
        {
            var set = new WeakSet<object>();
            var value = new object();
            Assert.False(set.delete(value));
        }

        [Fact]
        public void delete_RemovesValue()
        {
            var set = new WeakSet<object>();
            var value = new object();
            set.add(value);
            set.delete(value);
            Assert.False(set.has(value));
        }

        // ==================== Value Type Tests ====================

        [Fact]
        public void WeakSet_StringValues_WorksCorrectly()
        {
            var set = new WeakSet<string>();
            var value = "test-value";  // String interning may keep this alive
            set.add(value);
            Assert.True(set.has(value));
        }

        [Fact]
        public void WeakSet_ArrayValues_WorksCorrectly()
        {
            var set = new WeakSet<int[]>();
            var value = new int[] { 1, 2, 3 };
            set.add(value);
            Assert.True(set.has(value));
        }

        [Fact]
        public void WeakSet_DifferentValuesAreDifferent()
        {
            var set = new WeakSet<object>();
            var v1 = new object();
            var v2 = new object();
            set.add(v1);
            Assert.True(set.has(v1));
            Assert.False(set.has(v2));
        }

        [Fact]
        public void WeakSet_MultipleValues_AllPresent()
        {
            var set = new WeakSet<object>();
            var v1 = new object();
            var v2 = new object();
            var v3 = new object();
            set.add(v1).add(v2).add(v3);
            Assert.True(set.has(v1));
            Assert.True(set.has(v2));
            Assert.True(set.has(v3));
        }
    }
}
