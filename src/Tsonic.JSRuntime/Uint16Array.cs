/**
 * JavaScript Uint16Array implementation
 * Typed array of 16-bit unsigned integers backed by native ushort[]
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
    /// JavaScript Uint16Array - typed array of 16-bit unsigned integers
    /// </summary>
    public class Uint16Array : IEnumerable<ushort>
    {
        private readonly ushort[] _array;

        public static int BYTES_PER_ELEMENT => 2;

        public Uint16Array(int length)
        {
            _array = new ushort[length];
        }

        public Uint16Array(IEnumerable<ushort> values)
        {
            _array = values.ToArray();
        }

        public Uint16Array(ushort[] values)
        {
            _array = (ushort[])values.Clone();
        }

        public int length => _array.Length;
        public int byteLength => _array.Length * BYTES_PER_ELEMENT;

        public ushort this[int index]
        {
            get => (index < 0 || index >= _array.Length) ? (ushort)0 : _array[index];
            set { if (index >= 0 && index < _array.Length) _array[index] = value; }
        }

        public ushort? at(int index)
        {
            if (index < 0) index = _array.Length + index;
            if (index < 0 || index >= _array.Length) return null;
            return _array[index];
        }

        public Uint16Array fill(ushort value, int start = 0, int? end = null)
        {
            int actualEnd = end ?? _array.Length;
            if (start < 0) start = SysMath.Max(0, _array.Length + start);
            if (actualEnd < 0) actualEnd = SysMath.Max(0, _array.Length + actualEnd);
            start = SysMath.Min(start, _array.Length);
            actualEnd = SysMath.Min(actualEnd, _array.Length);
            for (int i = start; i < actualEnd; i++) _array[i] = value;
            return this;
        }

        public void set(IEnumerable<ushort> array, int offset = 0)
        {
            int i = offset;
            foreach (var value in array)
            {
                if (i >= _array.Length) break;
                _array[i++] = value;
            }
        }

        public Uint16Array subarray(int begin = 0, int? end = null)
        {
            int actualEnd = end ?? _array.Length;
            if (begin < 0) begin = SysMath.Max(0, _array.Length + begin);
            if (actualEnd < 0) actualEnd = SysMath.Max(0, _array.Length + actualEnd);
            begin = SysMath.Min(begin, _array.Length);
            actualEnd = SysMath.Min(actualEnd, _array.Length);
            int newLength = SysMath.Max(0, actualEnd - begin);
            var result = new Uint16Array(newLength);
            SysArray.Copy(_array, begin, result._array, 0, newLength);
            return result;
        }

        public Uint16Array slice(int begin = 0, int? end = null) => subarray(begin, end);

        public int indexOf(ushort value, int fromIndex = 0)
        {
            if (fromIndex < 0) fromIndex = SysMath.Max(0, _array.Length + fromIndex);
            return SysArray.IndexOf(_array, value, fromIndex);
        }

        public bool includes(ushort value, int fromIndex = 0) => indexOf(value, fromIndex) >= 0;

        public string join(string separator = ",") => string.Join(separator, _array);

        public Uint16Array reverse()
        {
            SysArray.Reverse(_array);
            return this;
        }

        public Uint16Array sort(Comparison<ushort>? compareFn = null)
        {
            if (compareFn != null) SysArray.Sort(_array, compareFn);
            else SysArray.Sort(_array);
            return this;
        }

        public IEnumerator<ushort> GetEnumerator() => ((IEnumerable<ushort>)_array).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _array.GetEnumerator();
    }
}
