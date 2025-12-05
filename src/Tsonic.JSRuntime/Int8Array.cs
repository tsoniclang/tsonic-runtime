/**
 * JavaScript Int8Array implementation
 * Typed array of 8-bit signed integers backed by native sbyte[]
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
    /// JavaScript Int8Array - typed array of 8-bit signed integers
    /// </summary>
    public class Int8Array : IEnumerable<sbyte>
    {
        private readonly sbyte[] _array;

        public static int BYTES_PER_ELEMENT => 1;

        public Int8Array(int length)
        {
            _array = new sbyte[length];
        }

        public Int8Array(IEnumerable<sbyte> values)
        {
            _array = values.ToArray();
        }

        public Int8Array(sbyte[] values)
        {
            _array = (sbyte[])values.Clone();
        }

        public int length => _array.Length;
        public int byteLength => _array.Length * BYTES_PER_ELEMENT;

        public sbyte this[int index]
        {
            get => (index < 0 || index >= _array.Length) ? (sbyte)0 : _array[index];
            set { if (index >= 0 && index < _array.Length) _array[index] = value; }
        }

        public sbyte? at(int index)
        {
            if (index < 0) index = _array.Length + index;
            if (index < 0 || index >= _array.Length) return null;
            return _array[index];
        }

        public Int8Array fill(sbyte value, int start = 0, int? end = null)
        {
            int actualEnd = end ?? _array.Length;
            if (start < 0) start = SysMath.Max(0, _array.Length + start);
            if (actualEnd < 0) actualEnd = SysMath.Max(0, _array.Length + actualEnd);
            start = SysMath.Min(start, _array.Length);
            actualEnd = SysMath.Min(actualEnd, _array.Length);
            for (int i = start; i < actualEnd; i++) _array[i] = value;
            return this;
        }

        public void set(IEnumerable<sbyte> array, int offset = 0)
        {
            int i = offset;
            foreach (var value in array)
            {
                if (i >= _array.Length) break;
                _array[i++] = value;
            }
        }

        public Int8Array subarray(int begin = 0, int? end = null)
        {
            int actualEnd = end ?? _array.Length;
            if (begin < 0) begin = SysMath.Max(0, _array.Length + begin);
            if (actualEnd < 0) actualEnd = SysMath.Max(0, _array.Length + actualEnd);
            begin = SysMath.Min(begin, _array.Length);
            actualEnd = SysMath.Min(actualEnd, _array.Length);
            int newLength = SysMath.Max(0, actualEnd - begin);
            var result = new Int8Array(newLength);
            SysArray.Copy(_array, begin, result._array, 0, newLength);
            return result;
        }

        public Int8Array slice(int begin = 0, int? end = null) => subarray(begin, end);

        public int indexOf(sbyte value, int fromIndex = 0)
        {
            if (fromIndex < 0) fromIndex = SysMath.Max(0, _array.Length + fromIndex);
            return SysArray.IndexOf(_array, value, fromIndex);
        }

        public bool includes(sbyte value, int fromIndex = 0) => indexOf(value, fromIndex) >= 0;

        public string join(string separator = ",") => string.Join(separator, _array);

        public Int8Array reverse()
        {
            SysArray.Reverse(_array);
            return this;
        }

        public Int8Array sort(Comparison<sbyte>? compareFn = null)
        {
            if (compareFn != null) SysArray.Sort(_array, compareFn);
            else SysArray.Sort(_array);
            return this;
        }

        public IEnumerator<sbyte> GetEnumerator() => ((IEnumerable<sbyte>)_array).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _array.GetEnumerator();
    }
}
