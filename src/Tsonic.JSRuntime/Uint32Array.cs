/**
 * JavaScript Uint32Array implementation
 * Typed array of 32-bit unsigned integers backed by native uint[]
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
    /// JavaScript Uint32Array - typed array of 32-bit unsigned integers
    /// </summary>
    public class Uint32Array : IEnumerable<uint>
    {
        private readonly uint[] _array;

        public static int BYTES_PER_ELEMENT => 4;

        public Uint32Array(int length)
        {
            _array = new uint[length];
        }

        public Uint32Array(IEnumerable<uint> values)
        {
            _array = values.ToArray();
        }

        public Uint32Array(uint[] values)
        {
            _array = (uint[])values.Clone();
        }

        public int length => _array.Length;
        public int byteLength => _array.Length * BYTES_PER_ELEMENT;

        public uint this[int index]
        {
            get => (index < 0 || index >= _array.Length) ? 0u : _array[index];
            set { if (index >= 0 && index < _array.Length) _array[index] = value; }
        }

        public uint? at(int index)
        {
            if (index < 0) index = _array.Length + index;
            if (index < 0 || index >= _array.Length) return null;
            return _array[index];
        }

        public Uint32Array fill(uint value, int start = 0, int? end = null)
        {
            int actualEnd = end ?? _array.Length;
            if (start < 0) start = SysMath.Max(0, _array.Length + start);
            if (actualEnd < 0) actualEnd = SysMath.Max(0, _array.Length + actualEnd);
            start = SysMath.Min(start, _array.Length);
            actualEnd = SysMath.Min(actualEnd, _array.Length);
            for (int i = start; i < actualEnd; i++) _array[i] = value;
            return this;
        }

        public void set(IEnumerable<uint> array, int offset = 0)
        {
            int i = offset;
            foreach (var value in array)
            {
                if (i >= _array.Length) break;
                _array[i++] = value;
            }
        }

        public Uint32Array subarray(int begin = 0, int? end = null)
        {
            int actualEnd = end ?? _array.Length;
            if (begin < 0) begin = SysMath.Max(0, _array.Length + begin);
            if (actualEnd < 0) actualEnd = SysMath.Max(0, _array.Length + actualEnd);
            begin = SysMath.Min(begin, _array.Length);
            actualEnd = SysMath.Min(actualEnd, _array.Length);
            int newLength = SysMath.Max(0, actualEnd - begin);
            var result = new Uint32Array(newLength);
            SysArray.Copy(_array, begin, result._array, 0, newLength);
            return result;
        }

        public Uint32Array slice(int begin = 0, int? end = null) => subarray(begin, end);

        public int indexOf(uint value, int fromIndex = 0)
        {
            if (fromIndex < 0) fromIndex = SysMath.Max(0, _array.Length + fromIndex);
            return SysArray.IndexOf(_array, value, fromIndex);
        }

        public bool includes(uint value, int fromIndex = 0) => indexOf(value, fromIndex) >= 0;

        public string join(string separator = ",") => string.Join(separator, _array);

        public Uint32Array reverse()
        {
            SysArray.Reverse(_array);
            return this;
        }

        public Uint32Array sort(Comparison<uint>? compareFn = null)
        {
            if (compareFn != null) SysArray.Sort(_array, compareFn);
            else SysArray.Sort(_array);
            return this;
        }

        public IEnumerator<uint> GetEnumerator() => ((IEnumerable<uint>)_array).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _array.GetEnumerator();
    }
}
