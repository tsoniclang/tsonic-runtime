/**
 * JavaScript Int32Array implementation
 * Typed array of 32-bit signed integers backed by native int[]
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
    /// JavaScript Int32Array - typed array of 32-bit signed integers
    /// </summary>
    public class Int32Array : IEnumerable<int>
    {
        private readonly int[] _array;

        public static int BYTES_PER_ELEMENT => 4;

        public Int32Array(int length)
        {
            _array = new int[length];
        }

        public Int32Array(IEnumerable<int> values)
        {
            _array = values.ToArray();
        }

        public Int32Array(int[] values)
        {
            _array = (int[])values.Clone();
        }

        public int length => _array.Length;
        public int byteLength => _array.Length * BYTES_PER_ELEMENT;

        public int this[int index]
        {
            get => (index < 0 || index >= _array.Length) ? 0 : _array[index];
            set { if (index >= 0 && index < _array.Length) _array[index] = value; }
        }

        public int? at(int index)
        {
            if (index < 0) index = _array.Length + index;
            if (index < 0 || index >= _array.Length) return null;
            return _array[index];
        }

        public Int32Array fill(int value, int start = 0, int? end = null)
        {
            int actualEnd = end ?? _array.Length;
            if (start < 0) start = SysMath.Max(0, _array.Length + start);
            if (actualEnd < 0) actualEnd = SysMath.Max(0, _array.Length + actualEnd);
            start = SysMath.Min(start, _array.Length);
            actualEnd = SysMath.Min(actualEnd, _array.Length);
            for (int i = start; i < actualEnd; i++) _array[i] = value;
            return this;
        }

        public void set(IEnumerable<int> array, int offset = 0)
        {
            int i = offset;
            foreach (var value in array)
            {
                if (i >= _array.Length) break;
                _array[i++] = value;
            }
        }

        public Int32Array subarray(int begin = 0, int? end = null)
        {
            int actualEnd = end ?? _array.Length;
            if (begin < 0) begin = SysMath.Max(0, _array.Length + begin);
            if (actualEnd < 0) actualEnd = SysMath.Max(0, _array.Length + actualEnd);
            begin = SysMath.Min(begin, _array.Length);
            actualEnd = SysMath.Min(actualEnd, _array.Length);
            int newLength = SysMath.Max(0, actualEnd - begin);
            var result = new Int32Array(newLength);
            SysArray.Copy(_array, begin, result._array, 0, newLength);
            return result;
        }

        public Int32Array slice(int begin = 0, int? end = null) => subarray(begin, end);

        public int indexOf(int value, int fromIndex = 0)
        {
            if (fromIndex < 0) fromIndex = SysMath.Max(0, _array.Length + fromIndex);
            return SysArray.IndexOf(_array, value, fromIndex);
        }

        public bool includes(int value, int fromIndex = 0) => indexOf(value, fromIndex) >= 0;

        public string join(string separator = ",") => string.Join(separator, _array);

        public Int32Array reverse()
        {
            SysArray.Reverse(_array);
            return this;
        }

        public Int32Array sort(Comparison<int>? compareFn = null)
        {
            if (compareFn != null) SysArray.Sort(_array, compareFn);
            else SysArray.Sort(_array);
            return this;
        }

        public IEnumerator<int> GetEnumerator() => ((IEnumerable<int>)_array).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _array.GetEnumerator();
    }
}
