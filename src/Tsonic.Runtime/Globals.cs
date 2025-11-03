/**
 * Global functions available at Tsonic.Runtime root level
 */

using System;
using System.Globalization;
using System.Text;

namespace Tsonic.Runtime
{
    /// <summary>
    /// Global functions (parseInt, parseFloat, encoding, etc.)
    /// </summary>
    public static class Globals
    {
        // Global constants
        public const double Infinity = double.PositiveInfinity;
        public const double NaN = double.NaN;
        public static readonly object? undefined = null;

        /// <summary>
        /// Parse string to integer with optional radix
        /// </summary>
        public static double parseInt(string str, int? radix = null)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return double.NaN;
            }

            str = str.Trim();
            int actualRadix = radix ?? 10;

            if (actualRadix < 2 || actualRadix > 36)
            {
                return double.NaN;
            }

            try
            {
                return Convert.ToInt32(str, actualRadix);
            }
            catch
            {
                return double.NaN;
            }
        }

        /// <summary>
        /// Parse string to floating point number
        /// </summary>
        public static double parseFloat(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return double.NaN;
            }

            str = str.Trim();

            if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
            {
                return result;
            }

            return double.NaN;
        }

        /// <summary>
        /// Check if value is NaN
        /// </summary>
        public static bool isNaN(double value)
        {
            return double.IsNaN(value);
        }

        /// <summary>
        /// Check if value is finite (not infinite or NaN)
        /// </summary>
        public static bool isFinite(double value)
        {
            return !double.IsInfinity(value) && !double.IsNaN(value);
        }

        /// <summary>
        /// Helper method to percent-encode a rune (Unicode scalar value)
        /// </summary>
        private static void AppendPercentEncoded(StringBuilder sb, Rune rune)
        {
            Span<byte> utf8Bytes = stackalloc byte[4];
            var bytesWritten = rune.EncodeToUtf8(utf8Bytes);
            for (var i = 0; i < bytesWritten; i++)
            {
                sb.Append('%');
                sb.Append(utf8Bytes[i].ToString("X2"));
            }
        }

        /// <summary>
        /// Check if character should NOT be encoded by encodeURI
        /// Per spec: A-Z a-z 0-9 ; , / ? : @ & = + $ - _ . ! ~ * ' ( ) #
        /// </summary>
        private static bool IsUriUnescaped(Rune rune)
        {
            var value = rune.Value;
            return (value >= 'A' && value <= 'Z') ||
                   (value >= 'a' && value <= 'z') ||
                   (value >= '0' && value <= '9') ||
                   value == ';' || value == ',' || value == '/' || value == '?' ||
                   value == ':' || value == '@' || value == '&' || value == '=' ||
                   value == '+' || value == '$' || value == '-' || value == '_' ||
                   value == '.' || value == '!' || value == '~' || value == '*' ||
                   value == '\'' || value == '(' || value == ')' || value == '#';
        }

        /// <summary>
        /// Check if character should NOT be encoded by encodeURIComponent
        /// Per spec: A-Z a-z 0-9 - _ . ! ~ * ' ( )
        /// </summary>
        private static bool IsComponentUnescaped(Rune rune)
        {
            var value = rune.Value;
            return (value >= 'A' && value <= 'Z') ||
                   (value >= 'a' && value <= 'z') ||
                   (value >= '0' && value <= '9') ||
                   value == '-' || value == '_' || value == '.' ||
                   value == '!' || value == '~' || value == '*' ||
                   value == '\'' || value == '(' || value == ')';
        }

        /// <summary>
        /// Encode URI component (encodes all except: A-Z a-z 0-9 - _ . ! ~ * ' ( ))
        /// </summary>
        public static string encodeURIComponent(string component)
        {
            var sb = new StringBuilder(component.Length);
            foreach (var rune in component.EnumerateRunes())
            {
                if (IsComponentUnescaped(rune))
                {
                    sb.Append(rune);
                }
                else
                {
                    AppendPercentEncoded(sb, rune);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Decode URI component
        /// </summary>
        public static string decodeURIComponent(string component)
        {
            return Uri.UnescapeDataString(component);
        }

        /// <summary>
        /// Encode URI (preserves URI reserved characters: ; , / ? : @ & = + $ #)
        /// </summary>
        public static string encodeURI(string uri)
        {
            var sb = new StringBuilder(uri.Length);
            foreach (var rune in uri.EnumerateRunes())
            {
                if (IsUriUnescaped(rune))
                {
                    sb.Append(rune);
                }
                else
                {
                    AppendPercentEncoded(sb, rune);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// Decode URI
        /// </summary>
        public static string decodeURI(string uri)
        {
            return Uri.UnescapeDataString(uri);
        }

        /// <summary>
        /// Convert value to number
        /// </summary>
        public static double Number(object? value)
        {
            if (value == null) return 0;

            if (value is double d) return d;
            if (value is int i) return i;
            if (value is long l) return l;
            if (value is float f) return f;
            if (value is decimal dec) return (double)dec;
            if (value is bool b) return b ? 1 : 0;

            if (value is string str)
            {
                if (string.IsNullOrWhiteSpace(str)) return 0;
                str = str.Trim();
                if (str == "Infinity") return double.PositiveInfinity;
                if (str == "-Infinity") return double.NegativeInfinity;
                if (double.TryParse(str, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
                {
                    return result;
                }
                return double.NaN;
            }

            return double.NaN;
        }

        /// <summary>
        /// Convert value to string
        /// </summary>
        public static string String(object? value)
        {
            if (value == null) return "undefined";
            if (value is string s) return s;
            if (value is bool b) return b ? "true" : "false";
            if (value is double d)
            {
                if (double.IsNaN(d)) return "NaN";
                if (double.IsPositiveInfinity(d)) return "Infinity";
                if (double.IsNegativeInfinity(d)) return "-Infinity";
            }
            return value.ToString() ?? "";
        }

        /// <summary>
        /// Convert value to boolean
        /// </summary>
        public static bool Boolean(object? value)
        {
            if (value == null) return false;

            if (value is bool b) return b;
            if (value is string s) return s.Length > 0;
            if (value is double d)
            {
                if (double.IsNaN(d)) return false;
                return d != 0;
            }
            if (value is int i) return i != 0;
            if (value is long l) return l != 0;
            if (value is float f) return f != 0;
            if (value is decimal dec) return dec != 0;

            return true; // Objects are truthy
        }
    }
}
