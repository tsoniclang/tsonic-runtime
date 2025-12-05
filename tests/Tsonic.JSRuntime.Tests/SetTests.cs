using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Tsonic.JSRuntime.Tests
{
    public class SetTests
    {
        // ==================== Constructor Tests ====================

        [Fact]
        public void Constructor_Empty_CreatesEmptySet()
        {
            var set = new Set<int>();
            Assert.Equal(0, set.size);
        }

        [Fact]
        public void Constructor_WithValues_CreatesSetWithValues()
        {
            var set = new Set<int>(new[] { 1, 2, 3 });
            Assert.Equal(3, set.size);
            Assert.True(set.has(1));
            Assert.True(set.has(2));
            Assert.True(set.has(3));
        }

        [Fact]
        public void Constructor_WithDuplicates_KeepsUnique()
        {
            var set = new Set<int>(new[] { 1, 2, 2, 3, 3, 3 });
            Assert.Equal(3, set.size);
        }

        // ==================== size Property Tests ====================

        [Fact]
        public void size_EmptySet_ReturnsZero()
        {
            var set = new Set<int>();
            Assert.Equal(0, set.size);
        }

        [Fact]
        public void size_AfterAdding_ReturnsCorrectCount()
        {
            var set = new Set<int>();
            set.add(1).add(2).add(3);
            Assert.Equal(3, set.size);
        }

        // ==================== add Tests ====================

        [Fact]
        public void add_NewValue_AddsToSet()
        {
            var set = new Set<int>();
            set.add(42);
            Assert.True(set.has(42));
        }

        [Fact]
        public void add_DuplicateValue_DoesNotIncrementSize()
        {
            var set = new Set<int>();
            set.add(1);
            set.add(1);
            Assert.Equal(1, set.size);
        }

        [Fact]
        public void add_ReturnsSetForChaining()
        {
            var set = new Set<int>();
            var result = set.add(1).add(2).add(3);
            Assert.Same(set, result);
            Assert.Equal(3, set.size);
        }

        // ==================== has Tests ====================

        [Fact]
        public void has_ExistingValue_ReturnsTrue()
        {
            var set = new Set<int>();
            set.add(42);
            Assert.True(set.has(42));
        }

        [Fact]
        public void has_NonExistingValue_ReturnsFalse()
        {
            var set = new Set<int>();
            Assert.False(set.has(42));
        }

        [Fact]
        public void has_AfterDelete_ReturnsFalse()
        {
            var set = new Set<int>();
            set.add(42);
            set.delete(42);
            Assert.False(set.has(42));
        }

        // ==================== delete Tests ====================

        [Fact]
        public void delete_ExistingValue_ReturnsTrue()
        {
            var set = new Set<int>();
            set.add(42);
            Assert.True(set.delete(42));
        }

        [Fact]
        public void delete_NonExistingValue_ReturnsFalse()
        {
            var set = new Set<int>();
            Assert.False(set.delete(42));
        }

        [Fact]
        public void delete_RemovesValue()
        {
            var set = new Set<int>();
            set.add(42);
            set.delete(42);
            Assert.Equal(0, set.size);
            Assert.False(set.has(42));
        }

        // ==================== clear Tests ====================

        [Fact]
        public void clear_RemovesAllValues()
        {
            var set = new Set<int>();
            set.add(1).add(2).add(3);
            set.clear();
            Assert.Equal(0, set.size);
        }

        [Fact]
        public void clear_EmptySet_DoesNothing()
        {
            var set = new Set<int>();
            set.clear();
            Assert.Equal(0, set.size);
        }

        // ==================== keys Tests ====================

        [Fact]
        public void keys_ReturnsAllValues()
        {
            var set = new Set<int>();
            set.add(1).add(2).add(3);
            var keys = set.keys().ToList();
            Assert.Equal(3, keys.Count);
            Assert.Contains(1, keys);
            Assert.Contains(2, keys);
            Assert.Contains(3, keys);
        }

        [Fact]
        public void keys_EmptySet_ReturnsEmpty()
        {
            var set = new Set<int>();
            Assert.Empty(set.keys());
        }

        // ==================== values Tests ====================

        [Fact]
        public void values_ReturnsAllValues()
        {
            var set = new Set<int>();
            set.add(1).add(2).add(3);
            var values = set.values().ToList();
            Assert.Equal(3, values.Count);
        }

        [Fact]
        public void values_EmptySet_ReturnsEmpty()
        {
            var set = new Set<int>();
            Assert.Empty(set.values());
        }

        // ==================== entries Tests ====================

        [Fact]
        public void entries_ReturnsTuples()
        {
            var set = new Set<int>();
            set.add(1).add(2);
            var entries = set.entries().ToList();
            Assert.Equal(2, entries.Count);
            Assert.Contains((1, 1), entries);
            Assert.Contains((2, 2), entries);
        }

        [Fact]
        public void entries_EmptySet_ReturnsEmpty()
        {
            var set = new Set<int>();
            Assert.Empty(set.entries());
        }

        // ==================== forEach Tests ====================

        [Fact]
        public void forEach_CallsCallbackForEachValue()
        {
            var set = new Set<int>();
            set.add(1).add(2);
            var visited = new List<int>();
            set.forEach(v => visited.Add(v));
            Assert.Equal(2, visited.Count);
            Assert.Contains(1, visited);
            Assert.Contains(2, visited);
        }

        [Fact]
        public void forEach_WithSetArg_ReceivesSetReference()
        {
            var set = new Set<int>();
            set.add(1);
            Set<int>? receivedSet = null;
            set.forEach((v1, v2, s) => receivedSet = s);
            Assert.Same(set, receivedSet);
        }

        [Fact]
        public void forEach_TwoArgs_ReceivesSameValueTwice()
        {
            var set = new Set<int>();
            set.add(42);
            int? first = null, second = null;
            set.forEach((v1, v2) => { first = v1; second = v2; });
            Assert.Equal(42, first);
            Assert.Equal(42, second);
        }

        // ==================== Set Operations Tests ====================

        [Fact]
        public void difference_ReturnsValuesNotInOther()
        {
            var set1 = new Set<int>(new[] { 1, 2, 3 });
            var set2 = new Set<int>(new[] { 2, 3, 4 });
            var result = set1.difference(set2);
            Assert.Equal(1, result.size);
            Assert.True(result.has(1));
        }

        [Fact]
        public void intersection_ReturnsCommonValues()
        {
            var set1 = new Set<int>(new[] { 1, 2, 3 });
            var set2 = new Set<int>(new[] { 2, 3, 4 });
            var result = set1.intersection(set2);
            Assert.Equal(2, result.size);
            Assert.True(result.has(2));
            Assert.True(result.has(3));
        }

        [Fact]
        public void union_ReturnsCombinedValues()
        {
            var set1 = new Set<int>(new[] { 1, 2 });
            var set2 = new Set<int>(new[] { 3, 4 });
            var result = set1.union(set2);
            Assert.Equal(4, result.size);
        }

        [Fact]
        public void symmetricDifference_ReturnsExclusiveValues()
        {
            var set1 = new Set<int>(new[] { 1, 2, 3 });
            var set2 = new Set<int>(new[] { 2, 3, 4 });
            var result = set1.symmetricDifference(set2);
            Assert.Equal(2, result.size);
            Assert.True(result.has(1));
            Assert.True(result.has(4));
        }

        [Fact]
        public void isSubsetOf_TrueWhenSubset()
        {
            var set1 = new Set<int>(new[] { 1, 2 });
            var set2 = new Set<int>(new[] { 1, 2, 3 });
            Assert.True(set1.isSubsetOf(set2));
        }

        [Fact]
        public void isSubsetOf_FalseWhenNotSubset()
        {
            var set1 = new Set<int>(new[] { 1, 2, 4 });
            var set2 = new Set<int>(new[] { 1, 2, 3 });
            Assert.False(set1.isSubsetOf(set2));
        }

        [Fact]
        public void isSupersetOf_TrueWhenSuperset()
        {
            var set1 = new Set<int>(new[] { 1, 2, 3 });
            var set2 = new Set<int>(new[] { 1, 2 });
            Assert.True(set1.isSupersetOf(set2));
        }

        [Fact]
        public void isDisjointFrom_TrueWhenNoCommon()
        {
            var set1 = new Set<int>(new[] { 1, 2 });
            var set2 = new Set<int>(new[] { 3, 4 });
            Assert.True(set1.isDisjointFrom(set2));
        }

        [Fact]
        public void isDisjointFrom_FalseWhenCommon()
        {
            var set1 = new Set<int>(new[] { 1, 2 });
            var set2 = new Set<int>(new[] { 2, 3 });
            Assert.False(set1.isDisjointFrom(set2));
        }

        // ==================== IEnumerable Tests ====================

        [Fact]
        public void GetEnumerator_AllowsForeach()
        {
            var set = new Set<int>();
            set.add(1).add(2);
            var count = 0;
            foreach (var value in set)
            {
                count++;
            }
            Assert.Equal(2, count);
        }

        // ==================== Edge Cases ====================

        [Fact]
        public void Set_WithNullValue_StoresNull()
        {
            var set = new Set<string?>();
            set.add(null);
            Assert.True(set.has(null));
        }

        [Fact]
        public void Set_StringValues_WorksCorrectly()
        {
            var set = new Set<string>();
            set.add("a").add("b").add("c");
            Assert.Equal(3, set.size);
            Assert.True(set.has("b"));
        }

        [Fact]
        public void Set_ObjectValues_UsesReferenceEquality()
        {
            var obj1 = new object();
            var obj2 = new object();
            var set = new Set<object>();
            set.add(obj1).add(obj2);
            Assert.Equal(2, set.size);
            Assert.True(set.has(obj1));
            Assert.True(set.has(obj2));
        }
    }
}
