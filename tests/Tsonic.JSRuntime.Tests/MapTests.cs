using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tsonic.JSRuntime.Tests
{
    public class MapTests
    {
        // ==================== Constructor Tests ====================

        [Fact]
        public void Constructor_Empty_CreatesEmptyMap()
        {
            var map = new Map<string, int>();
            Assert.Equal(0, map.size);
        }

        [Fact]
        public void Constructor_WithTuples_CreatesMapWithEntries()
        {
            var map = new Map<string, int>(new[] { ("a", 1), ("b", 2) });
            Assert.Equal(2, map.size);
            Assert.Equal(1, map.get("a"));
            Assert.Equal(2, map.get("b"));
        }

        [Fact]
        public void Constructor_WithKeyValuePairs_CreatesMapWithEntries()
        {
            var entries = new List<KeyValuePair<string, int>>
            {
                new("a", 1),
                new("b", 2)
            };
            var map = new Map<string, int>(entries);
            Assert.Equal(2, map.size);
        }

        // ==================== size Property Tests ====================

        [Fact]
        public void size_EmptyMap_ReturnsZero()
        {
            var map = new Map<string, int>();
            Assert.Equal(0, map.size);
        }

        [Fact]
        public void size_AfterAdding_ReturnsCorrectCount()
        {
            var map = new Map<string, int>();
            map.set("a", 1);
            map.set("b", 2);
            Assert.Equal(2, map.size);
        }

        // ==================== get/set Tests ====================

        [Fact]
        public void set_NewKey_AddsEntry()
        {
            var map = new Map<string, int>();
            map.set("key", 42);
            Assert.Equal(42, map.get("key"));
        }

        [Fact]
        public void set_ExistingKey_UpdatesValue()
        {
            var map = new Map<string, int>();
            map.set("key", 1);
            map.set("key", 2);
            Assert.Equal(2, map.get("key"));
            Assert.Equal(1, map.size);
        }

        [Fact]
        public void set_ReturnsMapForChaining()
        {
            var map = new Map<string, int>();
            var result = map.set("a", 1).set("b", 2).set("c", 3);
            Assert.Same(map, result);
            Assert.Equal(3, map.size);
        }

        [Fact]
        public void get_ExistingKey_ReturnsValue()
        {
            var map = new Map<string, int>();
            map.set("key", 42);
            Assert.Equal(42, map.get("key"));
        }

        [Fact]
        public void get_NonExistingKey_ReturnsDefault()
        {
            var map = new Map<string, int>();
            Assert.Equal(0, map.get("missing"));
        }

        [Fact]
        public void get_NonExistingKey_ReferenceType_ReturnsNull()
        {
            var map = new Map<string, string>();
            Assert.Null(map.get("missing"));
        }

        // ==================== has Tests ====================

        [Fact]
        public void has_ExistingKey_ReturnsTrue()
        {
            var map = new Map<string, int>();
            map.set("key", 42);
            Assert.True(map.has("key"));
        }

        [Fact]
        public void has_NonExistingKey_ReturnsFalse()
        {
            var map = new Map<string, int>();
            Assert.False(map.has("missing"));
        }

        [Fact]
        public void has_AfterDelete_ReturnsFalse()
        {
            var map = new Map<string, int>();
            map.set("key", 42);
            map.delete("key");
            Assert.False(map.has("key"));
        }

        // ==================== delete Tests ====================

        [Fact]
        public void delete_ExistingKey_ReturnsTrue()
        {
            var map = new Map<string, int>();
            map.set("key", 42);
            Assert.True(map.delete("key"));
        }

        [Fact]
        public void delete_NonExistingKey_ReturnsFalse()
        {
            var map = new Map<string, int>();
            Assert.False(map.delete("missing"));
        }

        [Fact]
        public void delete_RemovesEntry()
        {
            var map = new Map<string, int>();
            map.set("key", 42);
            map.delete("key");
            Assert.Equal(0, map.size);
            Assert.False(map.has("key"));
        }

        // ==================== clear Tests ====================

        [Fact]
        public void clear_RemovesAllEntries()
        {
            var map = new Map<string, int>();
            map.set("a", 1).set("b", 2).set("c", 3);
            map.clear();
            Assert.Equal(0, map.size);
        }

        [Fact]
        public void clear_EmptyMap_DoesNothing()
        {
            var map = new Map<string, int>();
            map.clear();
            Assert.Equal(0, map.size);
        }

        // ==================== keys Tests ====================

        [Fact]
        public void keys_ReturnsAllKeys()
        {
            var map = new Map<string, int>();
            map.set("a", 1).set("b", 2).set("c", 3);
            var keys = map.keys().ToList();
            Assert.Equal(3, keys.Count);
            Assert.Contains("a", keys);
            Assert.Contains("b", keys);
            Assert.Contains("c", keys);
        }

        [Fact]
        public void keys_EmptyMap_ReturnsEmpty()
        {
            var map = new Map<string, int>();
            Assert.Empty(map.keys());
        }

        // ==================== values Tests ====================

        [Fact]
        public void values_ReturnsAllValues()
        {
            var map = new Map<string, int>();
            map.set("a", 1).set("b", 2).set("c", 3);
            var values = map.values().ToList();
            Assert.Equal(3, values.Count);
            Assert.Contains(1, values);
            Assert.Contains(2, values);
            Assert.Contains(3, values);
        }

        [Fact]
        public void values_EmptyMap_ReturnsEmpty()
        {
            var map = new Map<string, int>();
            Assert.Empty(map.values());
        }

        // ==================== entries Tests ====================

        [Fact]
        public void entries_ReturnsAllEntries()
        {
            var map = new Map<string, int>();
            map.set("a", 1).set("b", 2);
            var entries = map.entries().ToList();
            Assert.Equal(2, entries.Count);
            Assert.Contains(("a", 1), entries);
            Assert.Contains(("b", 2), entries);
        }

        [Fact]
        public void entries_EmptyMap_ReturnsEmpty()
        {
            var map = new Map<string, int>();
            Assert.Empty(map.entries());
        }

        // ==================== forEach Tests ====================

        [Fact]
        public void forEach_CallsCallbackForEachEntry()
        {
            var map = new Map<string, int>();
            map.set("a", 1).set("b", 2);
            var visited = new List<(int value, string key)>();
            map.forEach((v, k) => visited.Add((v, k)));
            Assert.Equal(2, visited.Count);
            Assert.Contains((1, "a"), visited);
            Assert.Contains((2, "b"), visited);
        }

        [Fact]
        public void forEach_WithMapArg_ReceivesMapReference()
        {
            var map = new Map<string, int>();
            map.set("a", 1);
            Map<string, int>? receivedMap = null;
            map.forEach((v, k, m) => receivedMap = m);
            Assert.Same(map, receivedMap);
        }

        [Fact]
        public void forEach_ValueOnly_CallsWithValue()
        {
            var map = new Map<string, int>();
            map.set("a", 1).set("b", 2);
            var values = new List<int>();
            map.forEach(v => values.Add(v));
            Assert.Equal(2, values.Count);
            Assert.Contains(1, values);
            Assert.Contains(2, values);
        }

        // ==================== IEnumerable Tests ====================

        [Fact]
        public void GetEnumerator_AllowsForeach()
        {
            var map = new Map<string, int>();
            map.set("a", 1).set("b", 2);
            var count = 0;
            foreach (var kvp in map)
            {
                count++;
                Assert.True(kvp.Key == "a" || kvp.Key == "b");
            }
            Assert.Equal(2, count);
        }

        // ==================== Edge Cases ====================

        [Fact]
        public void Map_WithNullValue_StoresNull()
        {
            var map = new Map<string, string?>();
            map.set("key", null);
            Assert.True(map.has("key"));
            Assert.Null(map.get("key"));
        }

        [Fact]
        public void Map_IntKeys_WorksCorrectly()
        {
            var map = new Map<int, string>();
            map.set(1, "one").set(2, "two");
            Assert.Equal("one", map.get(1));
            Assert.Equal("two", map.get(2));
        }

        [Fact]
        public void Map_ObjectKeys_WorksCorrectly()
        {
            var key1 = new object();
            var key2 = new object();
            var map = new Map<object, string>();
            map.set(key1, "first").set(key2, "second");
            Assert.Equal("first", map.get(key1));
            Assert.Equal("second", map.get(key2));
        }
    }
}
