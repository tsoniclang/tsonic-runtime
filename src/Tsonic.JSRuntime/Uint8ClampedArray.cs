/**
 * JavaScript Uint8ClampedArray implementation
 * Typed array of 8-bit unsigned integers with clamping, backed by native byte[]
 */

using System;
using SysMath = System.Math;
using SysArray = System.Array;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Tsonic.JSRuntime
{
    /// <summary>
    /// JavaScript Uint8ClampedArray - typed array with values clamped to 0-255
    /// </summary>
    public class Uint8ClampedArray : IEnumerable<byte>
    {
        private readonly byte[] _array;

        public static int BYTES_PER_ELEMENT => 1;

        public Uint8ClampedArray(int length)
        {
            _array = new byte[length];
        }

        public Uint8ClampedArray(IEnumerable<byte> values)
        {
            _array = values.ToArray();
        }

        public Uint8ClampedArray(byte[] values)
        {
            _array = (byte[])values.Clone();
        }

        public int length => _array.Length;
        public int byteLength => _array.Length * BYTES_PER_ELEMENT;

        public byte this[int index]
        {
            get => (index < 0 || index >= _array.Length) ? (byte)0 : _array[index];
            set { if (index >= 0 && index < _array.Length) _array[index] = value; }
        }

        /// <summary>
        /// Set value with clamping (for values outside 0-255 range)
        /// </summary>
        public void SetClamped(int index, int value)
        {
            if (index >= 0 && index < _array.Length)
                _array[index] = (byte)SysMath.Clamp(value, 0, 255);
        }

        public byte? at(int index)
        {
            if (index < 0) index = _array.Length + index;
            if (index < 0 || index >= _array.Length) return null;
            return _array[index];
        }

        public Uint8ClampedArray fill(byte value, int start = 0, int? end = null)
        {
            int actualEnd = end ?? _array.Length;
            if (start < 0) start = SysMath.Max(0, _array.Length + start);
            if (actualEnd < 0) actualEnd = SysMath.Max(0, _array.Length + actualEnd);
            start = SysMath.Min(start, _array.Length);
            actualEnd = SysMath.Min(actualEnd, _array.Length);
            for (int i = start; i < actualEnd; i++) _array[i] = value;
            return this;
        }

        public void set(IEnumerable<byte> array, int offset = 0)
        {
            int i = offset;
            foreach (var value in array)
            {
                if (i >= _array.Length) break;
                _array[i++] = value;
            }
        }

        public Uint8ClampedArray subarray(int begin = 0, int? end = null)
        {
            int actualEnd = end ?? _array.Length;
            if (begin < 0) begin = SysMath.Max(0, _array.Length + begin);
            if (actualEnd < 0) actualEnd = SysMath.Max(0, _array.Length + actualEnd);
            begin = SysMath.Min(begin, _array.Length);
            actualEnd = SysMath.Min(actualEnd, _array.Length);
            int newLength = SysMath.Max(0, actualEnd - begin);
            var result = new Uint8ClampedArray(newLength);
            SysArray.Copy(_array, begin, result._array, 0, newLength);
            return result;
        }

        public Uint8ClampedArray slice(int begin = 0, int? end = null) => subarray(begin, end);

        public int indexOf(byte value, int fromIndex = 0)
        {
            if (fromIndex < 0) fromIndex = SysMath.Max(0, _array.Length + fromIndex);
            return SysArray.IndexOf(_array, value, fromIndex);
        }

        public bool includes(byte value, int fromIndex = 0) => indexOf(value, fromIndex) >= 0;

        public string join(string separator = ",") => string.Join(separator, _array);

        public Uint8ClampedArray reverse()
        {
            SysArray.Reverse(_array);
            return this;
        }

        public Uint8ClampedArray sort(Comparison<byte>? compareFn = null)
        {
            if (compareFn != null) SysArray.Sort(_array, compareFn);
            else SysArray.Sort(_array);
            return this;
        }

        public IEnumerator<byte> GetEnumerator() => ((IEnumerable<byte>)_array).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _array.GetEnumerator();
    }
}
