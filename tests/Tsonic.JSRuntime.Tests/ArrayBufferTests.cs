using System;
using Xunit;

namespace Tsonic.JSRuntime.Tests
{
    public class ArrayBufferTests
    {
        // ==================== Constructor Tests ====================

        [Fact]
        public void Constructor_WithLength_CreatesBuffer()
        {
            var buffer = new ArrayBuffer(16);
            Assert.Equal(16, buffer.byteLength);
        }

        [Fact]
        public void Constructor_ZeroLength_CreatesEmptyBuffer()
        {
            var buffer = new ArrayBuffer(0);
            Assert.Equal(0, buffer.byteLength);
        }

        [Fact]
        public void Constructor_NegativeLength_Throws()
        {
            Assert.Throws<ArgumentException>(() => new ArrayBuffer(-1));
        }

        // ==================== slice Tests ====================

        [Fact]
        public void slice_Default_CopiesEntireBuffer()
        {
            var buffer = new ArrayBuffer(8);
            var sliced = buffer.slice();
            Assert.Equal(8, sliced.byteLength);
        }

        [Fact]
        public void slice_WithBegin_CopiesFromBegin()
        {
            var buffer = new ArrayBuffer(8);
            var sliced = buffer.slice(2);
            Assert.Equal(6, sliced.byteLength);
        }

        [Fact]
        public void slice_WithBeginAndEnd_CopiesRange()
        {
            var buffer = new ArrayBuffer(8);
            var sliced = buffer.slice(2, 6);
            Assert.Equal(4, sliced.byteLength);
        }

        [Fact]
        public void slice_NegativeBegin_CountsFromEnd()
        {
            var buffer = new ArrayBuffer(8);
            var sliced = buffer.slice(-3);
            Assert.Equal(3, sliced.byteLength);
        }

        [Fact]
        public void slice_NegativeEnd_CountsFromEnd()
        {
            var buffer = new ArrayBuffer(8);
            var sliced = buffer.slice(0, -2);
            Assert.Equal(6, sliced.byteLength);
        }

        [Fact]
        public void slice_CreatesNewBuffer()
        {
            var buffer = new ArrayBuffer(8);
            var sliced = buffer.slice();
            Assert.NotSame(buffer, sliced);
        }

    }
}
