/**
 * JavaScript Float64Array implementation
 * Typed array of 64-bit floating point numbers backed by native double[]
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
    /// JavaScript Float64Array - typed array of 64-bit floating point numbers
    /// </summary>
    public class Float64Array : IEnumerable<double>
    {
        private readonly double[] _array;

        public static int BYTES_PER_ELEMENT => 8;

        public Float64Array(int length)
        {
            _array = new double[length];
        }

        public Float64Array(IEnumerable<double> values)
        {
            _array = values.ToArray();
        }

        public Float64Array(double[] values)
        {
            _array = (double[])values.Clone();
        }

        public int length => _array.Length;
        public int byteLength => _array.Length * BYTES_PER_ELEMENT;

        public double this[int index]
        {
            get => (index < 0 || index >= _array.Length) ? 0.0 : _array[index];
            set { if (index >= 0 && index < _array.Length) _array[index] = value; }
        }

        public double? at(int index)
        {
            if (index < 0) index = _array.Length + index;
            if (index < 0 || index >= _array.Length) return null;
            return _array[index];
        }

        public Float64Array fill(double value, int start = 0, int? end = null)
        {
            int actualEnd = end ?? _array.Length;
            if (start < 0) start = SysMath.Max(0, _array.Length + start);
            if (actualEnd < 0) actualEnd = SysMath.Max(0, _array.Length + actualEnd);
            start = SysMath.Min(start, _array.Length);
            actualEnd = SysMath.Min(actualEnd, _array.Length);
            for (int i = start; i < actualEnd; i++) _array[i] = value;
            return this;
        }

        public void set(IEnumerable<double> array, int offset = 0)
        {
            int i = offset;
            foreach (var value in array)
            {
                if (i >= _array.Length) break;
                _array[i++] = value;
            }
        }

        public Float64Array subarray(int begin = 0, int? end = null)
        {
            int actualEnd = end ?? _array.Length;
            if (begin < 0) begin = SysMath.Max(0, _array.Length + begin);
            if (actualEnd < 0) actualEnd = SysMath.Max(0, _array.Length + actualEnd);
            begin = SysMath.Min(begin, _array.Length);
            actualEnd = SysMath.Min(actualEnd, _array.Length);
            int newLength = SysMath.Max(0, actualEnd - begin);
            var result = new Float64Array(newLength);
            SysArray.Copy(_array, begin, result._array, 0, newLength);
            return result;
        }

        public Float64Array slice(int begin = 0, int? end = null) => subarray(begin, end);

        public int indexOf(double value, int fromIndex = 0)
        {
            if (fromIndex < 0) fromIndex = SysMath.Max(0, _array.Length + fromIndex);
            return SysArray.IndexOf(_array, value, fromIndex);
        }

        public bool includes(double value, int fromIndex = 0) => indexOf(value, fromIndex) >= 0;

        public string join(string separator = ",") => string.Join(separator, _array);

        public Float64Array reverse()
        {
            SysArray.Reverse(_array);
            return this;
        }

        public Float64Array sort(Comparison<double>? compareFn = null)
        {
            if (compareFn != null) SysArray.Sort(_array, compareFn);
            else SysArray.Sort(_array);
            return this;
        }

        public IEnumerator<double> GetEnumerator() => ((IEnumerable<double>)_array).GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => _array.GetEnumerator();
    }
}
