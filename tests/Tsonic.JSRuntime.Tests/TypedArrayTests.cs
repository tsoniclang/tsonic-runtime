using System;
using System.Linq;
using Xunit;

namespace Tsonic.JSRuntime.Tests
{
    public class TypedArrayTests
    {
        // ==================== Int8Array Tests ====================

        [Fact]
        public void Int8Array_BYTES_PER_ELEMENT_Is1()
        {
            Assert.Equal(1, Int8Array.BYTES_PER_ELEMENT);
        }

        [Fact]
        public void Int8Array_Constructor_WithLength_CreatesArray()
        {
            var arr = new Int8Array(4);
            Assert.Equal(4, arr.length);
            Assert.Equal(4, arr.byteLength);
        }

        [Fact]
        public void Int8Array_Constructor_WithValues_CreatesArray()
        {
            var arr = new Int8Array(new sbyte[] { 1, 2, 3 });
            Assert.Equal(3, arr.length);
            Assert.Equal(1, arr[0]);
            Assert.Equal(2, arr[1]);
            Assert.Equal(3, arr[2]);
        }

        [Fact]
        public void Int8Array_Indexer_GetSet_Works()
        {
            var arr = new Int8Array(2);
            arr[0] = 42;
            arr[1] = -10;
            Assert.Equal(42, arr[0]);
            Assert.Equal(-10, arr[1]);
        }

        [Fact]
        public void Int8Array_at_NegativeIndex_CountsFromEnd()
        {
            var arr = new Int8Array(new sbyte[] { 1, 2, 3 });
            Assert.Equal((sbyte)3, arr.at(-1));
            Assert.Equal((sbyte)2, arr.at(-2));
        }

        [Fact]
        public void Int8Array_fill_FillsWithValue()
        {
            var arr = new Int8Array(4);
            arr.fill(42);
            Assert.All(arr, v => Assert.Equal(42, v));
        }

        [Fact]
        public void Int8Array_slice_CopiesPortion()
        {
            var arr = new Int8Array(new sbyte[] { 1, 2, 3, 4, 5 });
            var sliced = arr.slice(1, 4);
            Assert.Equal(3, sliced.length);
            Assert.Equal(2, sliced[0]);
            Assert.Equal(3, sliced[1]);
            Assert.Equal(4, sliced[2]);
        }

        [Fact]
        public void Int8Array_subarray_CopiesPortion()
        {
            var arr = new Int8Array(new sbyte[] { 1, 2, 3, 4, 5 });
            var sub = arr.subarray(1, 4);
            Assert.Equal(3, sub.length);
            Assert.Equal(2, sub[0]);
        }

        [Fact]
        public void Int8Array_indexOf_FindsValue()
        {
            var arr = new Int8Array(new sbyte[] { 1, 2, 3, 2, 1 });
            Assert.Equal(1, arr.indexOf(2));
            Assert.Equal(-1, arr.indexOf(99));
        }

        [Fact]
        public void Int8Array_includes_ChecksValue()
        {
            var arr = new Int8Array(new sbyte[] { 1, 2, 3 });
            Assert.True(arr.includes(2));
            Assert.False(arr.includes(99));
        }

        [Fact]
        public void Int8Array_join_JoinsElements()
        {
            var arr = new Int8Array(new sbyte[] { 1, 2, 3 });
            Assert.Equal("1,2,3", arr.join());
            Assert.Equal("1-2-3", arr.join("-"));
        }

        [Fact]
        public void Int8Array_reverse_ReversesInPlace()
        {
            var arr = new Int8Array(new sbyte[] { 1, 2, 3 });
            arr.reverse();
            Assert.Equal(3, arr[0]);
            Assert.Equal(2, arr[1]);
            Assert.Equal(1, arr[2]);
        }

        [Fact]
        public void Int8Array_sort_SortsInPlace()
        {
            var arr = new Int8Array(new sbyte[] { 3, 1, 2 });
            arr.sort();
            Assert.Equal(1, arr[0]);
            Assert.Equal(2, arr[1]);
            Assert.Equal(3, arr[2]);
        }

        // ==================== Uint8Array Tests ====================

        [Fact]
        public void Uint8Array_BYTES_PER_ELEMENT_Is1()
        {
            Assert.Equal(1, Uint8Array.BYTES_PER_ELEMENT);
        }

        [Fact]
        public void Uint8Array_StoresUnsignedValues()
        {
            var arr = new Uint8Array(new byte[] { 0, 128, 255 });
            Assert.Equal((byte)0, arr[0]);
            Assert.Equal((byte)128, arr[1]);
            Assert.Equal((byte)255, arr[2]);
        }

        // ==================== Uint8ClampedArray Tests ====================

        [Fact]
        public void Uint8ClampedArray_SetClamped_ClampsValues()
        {
            var arr = new Uint8ClampedArray(3);
            arr.SetClamped(0, -10);    // Should clamp to 0
            arr.SetClamped(1, 128);    // Should stay 128
            arr.SetClamped(2, 300);    // Should clamp to 255
            Assert.Equal((byte)0, arr[0]);
            Assert.Equal((byte)128, arr[1]);
            Assert.Equal((byte)255, arr[2]);
        }

        // ==================== Int16Array Tests ====================

        [Fact]
        public void Int16Array_BYTES_PER_ELEMENT_Is2()
        {
            Assert.Equal(2, Int16Array.BYTES_PER_ELEMENT);
        }

        [Fact]
        public void Int16Array_Stores16BitValues()
        {
            var arr = new Int16Array(new short[] { -32768, 0, 32767 });
            Assert.Equal(-32768, arr[0]);
            Assert.Equal(0, arr[1]);
            Assert.Equal(32767, arr[2]);
        }

        [Fact]
        public void Int16Array_byteLength_IsDoubleLength()
        {
            var arr = new Int16Array(4);
            Assert.Equal(4, arr.length);
            Assert.Equal(8, arr.byteLength);
        }

        // ==================== Uint16Array Tests ====================

        [Fact]
        public void Uint16Array_BYTES_PER_ELEMENT_Is2()
        {
            Assert.Equal(2, Uint16Array.BYTES_PER_ELEMENT);
        }

        [Fact]
        public void Uint16Array_StoresUnsigned16BitValues()
        {
            var arr = new Uint16Array(new ushort[] { 0, 32768, 65535 });
            Assert.Equal((ushort)0, arr[0]);
            Assert.Equal((ushort)32768, arr[1]);
            Assert.Equal((ushort)65535, arr[2]);
        }

        // ==================== Int32Array Tests ====================

        [Fact]
        public void Int32Array_BYTES_PER_ELEMENT_Is4()
        {
            Assert.Equal(4, Int32Array.BYTES_PER_ELEMENT);
        }

        [Fact]
        public void Int32Array_Stores32BitValues()
        {
            var arr = new Int32Array(new int[] { int.MinValue, 0, int.MaxValue });
            Assert.Equal(int.MinValue, arr[0]);
            Assert.Equal(0, arr[1]);
            Assert.Equal(int.MaxValue, arr[2]);
        }

        [Fact]
        public void Int32Array_byteLength_IsQuadrupleLength()
        {
            var arr = new Int32Array(4);
            Assert.Equal(4, arr.length);
            Assert.Equal(16, arr.byteLength);
        }

        // ==================== Uint32Array Tests ====================

        [Fact]
        public void Uint32Array_BYTES_PER_ELEMENT_Is4()
        {
            Assert.Equal(4, Uint32Array.BYTES_PER_ELEMENT);
        }

        [Fact]
        public void Uint32Array_StoresUnsigned32BitValues()
        {
            var arr = new Uint32Array(new uint[] { 0, 2147483648, uint.MaxValue });
            Assert.Equal(0u, arr[0]);
            Assert.Equal(2147483648u, arr[1]);
            Assert.Equal(uint.MaxValue, arr[2]);
        }

        // ==================== Float32Array Tests ====================

        [Fact]
        public void Float32Array_BYTES_PER_ELEMENT_Is4()
        {
            Assert.Equal(4, Float32Array.BYTES_PER_ELEMENT);
        }

        [Fact]
        public void Float32Array_StoresFloatValues()
        {
            var arr = new Float32Array(new float[] { 1.5f, -2.5f, 3.14159f });
            Assert.Equal(1.5f, arr[0]);
            Assert.Equal(-2.5f, arr[1]);
            Assert.Equal(3.14159f, arr[2], 5);
        }

        // ==================== Float64Array Tests ====================

        [Fact]
        public void Float64Array_BYTES_PER_ELEMENT_Is8()
        {
            Assert.Equal(8, Float64Array.BYTES_PER_ELEMENT);
        }

        [Fact]
        public void Float64Array_StoresDoubleValues()
        {
            var arr = new Float64Array(new double[] { 1.5, -2.5, System.Math.PI });
            Assert.Equal(1.5, arr[0]);
            Assert.Equal(-2.5, arr[1]);
            Assert.Equal(System.Math.PI, arr[2], 10);
        }

        [Fact]
        public void Float64Array_byteLength_IsOctupleLength()
        {
            var arr = new Float64Array(4);
            Assert.Equal(4, arr.length);
            Assert.Equal(32, arr.byteLength);
        }

        // ==================== IEnumerable Tests ====================

        [Fact]
        public void TypedArray_IsEnumerable()
        {
            var arr = new Int32Array(new int[] { 1, 2, 3 });
            var sum = arr.Sum();
            Assert.Equal(6, sum);
        }

        [Fact]
        public void TypedArray_ForEach_Works()
        {
            var arr = new Int32Array(new int[] { 1, 2, 3 });
            var count = 0;
            foreach (var v in arr) count++;
            Assert.Equal(3, count);
        }

        // ==================== Edge Cases ====================

        [Fact]
        public void TypedArray_OutOfBoundsGet_ReturnsZero()
        {
            var arr = new Int32Array(2);
            Assert.Equal(0, arr[-1]);
            Assert.Equal(0, arr[100]);
        }

        [Fact]
        public void TypedArray_OutOfBoundsSet_DoesNothing()
        {
            var arr = new Int32Array(2);
            arr[-1] = 99;  // Should not throw
            arr[100] = 99; // Should not throw
            Assert.Equal(0, arr[0]);
            Assert.Equal(0, arr[1]);
        }

        [Fact]
        public void TypedArray_at_OutOfBounds_ReturnsNull()
        {
            var arr = new Int32Array(2);
            Assert.Null(arr.at(100));
            Assert.Null(arr.at(-100));
        }
    }
}
