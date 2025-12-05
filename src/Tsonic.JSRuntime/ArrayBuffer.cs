/**
 * JavaScript ArrayBuffer implementation
 * Fixed-length binary data buffer backed by native byte[]
 */

using System;
using SysMath = System.Math;
using SysArray = System.Array;

namespace Tsonic.JSRuntime
{
    /// <summary>
    /// JavaScript ArrayBuffer - fixed-length raw binary data buffer
    /// </summary>
    public class ArrayBuffer
    {
        private readonly byte[] _buffer;

        /// <summary>
        /// Create ArrayBuffer with specified byte length
        /// </summary>
        public ArrayBuffer(int byteLength)
        {
            if (byteLength < 0)
                throw new ArgumentException("byteLength must be non-negative", nameof(byteLength));
            _buffer = new byte[byteLength];
        }

        /// <summary>
        /// Length of the buffer in bytes
        /// </summary>
        public int byteLength => _buffer.Length;

        /// <summary>
        /// Create new ArrayBuffer containing a copy of bytes from begin to end
        /// </summary>
        public ArrayBuffer slice(int begin = 0, int? end = null)
        {
            int actualEnd = end ?? _buffer.Length;

            if (begin < 0) begin = SysMath.Max(0, _buffer.Length + begin);
            if (actualEnd < 0) actualEnd = SysMath.Max(0, _buffer.Length + actualEnd);

            begin = SysMath.Min(begin, _buffer.Length);
            actualEnd = SysMath.Min(actualEnd, _buffer.Length);

            int length = SysMath.Max(0, actualEnd - begin);
            var result = new ArrayBuffer(length);
            if (length > 0)
                SysArray.Copy(_buffer, begin, result._buffer, 0, length);
            return result;
        }
    }
}
