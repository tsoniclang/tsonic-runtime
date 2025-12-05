using System;
using Xunit;

namespace Tsonic.JSRuntime.Tests
{
    public class WeakMapTests
    {
        // ==================== Constructor Tests ====================

        [Fact]
        public void Constructor_CreatesEmptyWeakMap()
        {
            var map = new WeakMap<object, int>();
            var key = new object();
            Assert.False(map.has(key));
        }

        // ==================== get/set Tests ====================

        [Fact]
        public void set_NewKey_AddsEntry()
        {
            var map = new WeakMap<object, int>();
            var key = new object();
            map.set(key, 42);
            Assert.Equal(42, map.get(key));
        }

        [Fact]
        public void set_ExistingKey_UpdatesValue()
        {
            var map = new WeakMap<object, int>();
            var key = new object();
            map.set(key, 1);
            map.set(key, 2);
            Assert.Equal(2, map.get(key));
        }

        [Fact]
        public void set_ReturnsWeakMapForChaining()
        {
            var map = new WeakMap<object, int>();
            var key1 = new object();
            var key2 = new object();
            var result = map.set(key1, 1).set(key2, 2);
            Assert.Same(map, result);
        }

        [Fact]
        public void get_ExistingKey_ReturnsValue()
        {
            var map = new WeakMap<object, string>();
            var key = new object();
            map.set(key, "hello");
            Assert.Equal("hello", map.get(key));
        }

        [Fact]
        public void get_NonExistingKey_ReturnsDefault()
        {
            var map = new WeakMap<object, int>();
            var key = new object();
            Assert.Equal(0, map.get(key));
        }

        [Fact]
        public void get_NonExistingKey_ReferenceType_ReturnsNull()
        {
            var map = new WeakMap<object, string>();
            var key = new object();
            Assert.Null(map.get(key));
        }

        // ==================== has Tests ====================

        [Fact]
        public void has_ExistingKey_ReturnsTrue()
        {
            var map = new WeakMap<object, int>();
            var key = new object();
            map.set(key, 42);
            Assert.True(map.has(key));
        }

        [Fact]
        public void has_NonExistingKey_ReturnsFalse()
        {
            var map = new WeakMap<object, int>();
            var key = new object();
            Assert.False(map.has(key));
        }

        [Fact]
        public void has_AfterDelete_ReturnsFalse()
        {
            var map = new WeakMap<object, int>();
            var key = new object();
            map.set(key, 42);
            map.delete(key);
            Assert.False(map.has(key));
        }

        // ==================== delete Tests ====================

        [Fact]
        public void delete_ExistingKey_ReturnsTrue()
        {
            var map = new WeakMap<object, int>();
            var key = new object();
            map.set(key, 42);
            Assert.True(map.delete(key));
        }

        [Fact]
        public void delete_NonExistingKey_ReturnsFalse()
        {
            var map = new WeakMap<object, int>();
            var key = new object();
            Assert.False(map.delete(key));
        }

        [Fact]
        public void delete_RemovesEntry()
        {
            var map = new WeakMap<object, int>();
            var key = new object();
            map.set(key, 42);
            map.delete(key);
            Assert.False(map.has(key));
        }

        // ==================== Key Type Tests ====================

        [Fact]
        public void WeakMap_StringKeys_WorksCorrectly()
        {
            var map = new WeakMap<string, int>();
            var key = "test-key";  // String interning may keep this alive
            map.set(key, 42);
            Assert.Equal(42, map.get(key));
        }

        [Fact]
        public void WeakMap_ArrayKeys_WorksCorrectly()
        {
            var map = new WeakMap<int[], string>();
            var key = new int[] { 1, 2, 3 };
            map.set(key, "array");
            Assert.Equal("array", map.get(key));
        }

        [Fact]
        public void WeakMap_DifferentKeysAreDifferent()
        {
            var map = new WeakMap<object, int>();
            var key1 = new object();
            var key2 = new object();
            map.set(key1, 1);
            map.set(key2, 2);
            Assert.Equal(1, map.get(key1));
            Assert.Equal(2, map.get(key2));
        }

        // ==================== Value Type Tests ====================

        [Fact]
        public void WeakMap_NullValue_StoresNull()
        {
            var map = new WeakMap<object, string?>();
            var key = new object();
            map.set(key, null);
            Assert.True(map.has(key));
            Assert.Null(map.get(key));
        }

        [Fact]
        public void WeakMap_ObjectValue_WorksCorrectly()
        {
            var map = new WeakMap<object, object>();
            var key = new object();
            var value = new object();
            map.set(key, value);
            Assert.Same(value, map.get(key));
        }
    }
}
