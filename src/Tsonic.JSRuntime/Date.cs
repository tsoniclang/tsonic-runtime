/**
 * JavaScript Date implementation
 * Wraps System.DateTimeOffset with JavaScript Date semantics
 */

using System;
using System.Globalization;

namespace Tsonic.JSRuntime
{
    /// <summary>
    /// JavaScript Date - date and time handling
    /// </summary>
    public class Date
    {
        private DateTimeOffset _value;

        // Unix epoch: January 1, 1970 00:00:00 UTC
        private static readonly DateTimeOffset Epoch = new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero);

        // ==================== Constructors ====================

        /// <summary>
        /// Create Date with current time
        /// </summary>
        public Date()
        {
            _value = DateTimeOffset.Now;
        }

        /// <summary>
        /// Create Date from milliseconds since epoch
        /// </summary>
        public Date(double milliseconds)
        {
            if (double.IsNaN(milliseconds) || double.IsInfinity(milliseconds))
            {
                _value = DateTimeOffset.MinValue;
            }
            else
            {
                _value = Epoch.AddMilliseconds(milliseconds);
            }
        }

        /// <summary>
        /// Create Date from date string
        /// </summary>
        public Date(string dateString)
        {
            if (DateTimeOffset.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                _value = parsed;
            }
            else
            {
                _value = DateTimeOffset.MinValue;
            }
        }

        /// <summary>
        /// Create Date from year, month, day, etc.
        /// Month is 0-indexed (0 = January) per JavaScript convention
        /// </summary>
        public Date(int year, int month, int day = 1, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            try
            {
                // JavaScript months are 0-indexed, DateTimeOffset months are 1-indexed
                _value = new DateTimeOffset(year, month + 1, day, hours, minutes, seconds, milliseconds, TimeZoneInfo.Local.GetUtcOffset(DateTime.Now));
            }
            catch
            {
                _value = DateTimeOffset.MinValue;
            }
        }

        // ==================== Static Methods ====================

        /// <summary>
        /// Returns current time in milliseconds since epoch
        /// </summary>
        public static double now() => (DateTimeOffset.UtcNow - Epoch).TotalMilliseconds;

        /// <summary>
        /// Parse date string and return milliseconds since epoch
        /// </summary>
        public static double parse(string dateString)
        {
            if (DateTimeOffset.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
            {
                return (parsed - Epoch).TotalMilliseconds;
            }
            return double.NaN;
        }

        /// <summary>
        /// Create UTC date and return milliseconds since epoch
        /// Month is 0-indexed per JavaScript convention
        /// </summary>
        public static double UTC(int year, int month, int day = 1, int hours = 0, int minutes = 0, int seconds = 0, int milliseconds = 0)
        {
            try
            {
                var date = new DateTimeOffset(year, month + 1, day, hours, minutes, seconds, milliseconds, TimeSpan.Zero);
                return (date - Epoch).TotalMilliseconds;
            }
            catch
            {
                return double.NaN;
            }
        }

        // ==================== Instance Methods - Getters (Local Time) ====================

        /// <summary>
        /// Get milliseconds since epoch
        /// </summary>
        public double getTime() => (_value - Epoch).TotalMilliseconds;

        /// <summary>
        /// Get full year (4 digits)
        /// </summary>
        public int getFullYear() => _value.LocalDateTime.Year;

        /// <summary>
        /// Get month (0-11, 0 = January)
        /// </summary>
        public int getMonth() => _value.LocalDateTime.Month - 1;

        /// <summary>
        /// Get day of month (1-31)
        /// </summary>
        public int getDate() => _value.LocalDateTime.Day;

        /// <summary>
        /// Get day of week (0-6, 0 = Sunday)
        /// </summary>
        public int getDay() => (int)_value.LocalDateTime.DayOfWeek;

        /// <summary>
        /// Get hours (0-23)
        /// </summary>
        public int getHours() => _value.LocalDateTime.Hour;

        /// <summary>
        /// Get minutes (0-59)
        /// </summary>
        public int getMinutes() => _value.LocalDateTime.Minute;

        /// <summary>
        /// Get seconds (0-59)
        /// </summary>
        public int getSeconds() => _value.LocalDateTime.Second;

        /// <summary>
        /// Get milliseconds (0-999)
        /// </summary>
        public int getMilliseconds() => _value.LocalDateTime.Millisecond;

        /// <summary>
        /// Get timezone offset in minutes
        /// </summary>
        public int getTimezoneOffset() => -(int)_value.Offset.TotalMinutes;

        // ==================== Instance Methods - Getters (UTC) ====================

        /// <summary>
        /// Get UTC full year
        /// </summary>
        public int getUTCFullYear() => _value.UtcDateTime.Year;

        /// <summary>
        /// Get UTC month (0-11)
        /// </summary>
        public int getUTCMonth() => _value.UtcDateTime.Month - 1;

        /// <summary>
        /// Get UTC day of month
        /// </summary>
        public int getUTCDate() => _value.UtcDateTime.Day;

        /// <summary>
        /// Get UTC day of week
        /// </summary>
        public int getUTCDay() => (int)_value.UtcDateTime.DayOfWeek;

        /// <summary>
        /// Get UTC hours
        /// </summary>
        public int getUTCHours() => _value.UtcDateTime.Hour;

        /// <summary>
        /// Get UTC minutes
        /// </summary>
        public int getUTCMinutes() => _value.UtcDateTime.Minute;

        /// <summary>
        /// Get UTC seconds
        /// </summary>
        public int getUTCSeconds() => _value.UtcDateTime.Second;

        /// <summary>
        /// Get UTC milliseconds
        /// </summary>
        public int getUTCMilliseconds() => _value.UtcDateTime.Millisecond;

        // ==================== Instance Methods - Setters (Local Time) ====================

        /// <summary>
        /// Set time in milliseconds since epoch
        /// </summary>
        public double setTime(double milliseconds)
        {
            _value = Epoch.AddMilliseconds(milliseconds);
            return getTime();
        }

        /// <summary>
        /// Set milliseconds
        /// </summary>
        public double setMilliseconds(int ms)
        {
            var local = _value.LocalDateTime;
            _value = new DateTimeOffset(local.Year, local.Month, local.Day, local.Hour, local.Minute, local.Second, ms, _value.Offset);
            return getTime();
        }

        /// <summary>
        /// Set seconds (and optionally milliseconds)
        /// </summary>
        public double setSeconds(int sec, int? ms = null)
        {
            var local = _value.LocalDateTime;
            var newMs = ms ?? local.Millisecond;
            _value = new DateTimeOffset(local.Year, local.Month, local.Day, local.Hour, local.Minute, sec, newMs, _value.Offset);
            return getTime();
        }

        /// <summary>
        /// Set minutes (and optionally seconds, milliseconds)
        /// </summary>
        public double setMinutes(int min, int? sec = null, int? ms = null)
        {
            var local = _value.LocalDateTime;
            var newSec = sec ?? local.Second;
            var newMs = ms ?? local.Millisecond;
            _value = new DateTimeOffset(local.Year, local.Month, local.Day, local.Hour, min, newSec, newMs, _value.Offset);
            return getTime();
        }

        /// <summary>
        /// Set hours (and optionally minutes, seconds, milliseconds)
        /// </summary>
        public double setHours(int hour, int? min = null, int? sec = null, int? ms = null)
        {
            var local = _value.LocalDateTime;
            var newMin = min ?? local.Minute;
            var newSec = sec ?? local.Second;
            var newMs = ms ?? local.Millisecond;
            _value = new DateTimeOffset(local.Year, local.Month, local.Day, hour, newMin, newSec, newMs, _value.Offset);
            return getTime();
        }

        /// <summary>
        /// Set day of month
        /// </summary>
        public double setDate(int day)
        {
            var local = _value.LocalDateTime;
            _value = new DateTimeOffset(local.Year, local.Month, day, local.Hour, local.Minute, local.Second, local.Millisecond, _value.Offset);
            return getTime();
        }

        /// <summary>
        /// Set month (0-11) and optionally day
        /// </summary>
        public double setMonth(int month, int? day = null)
        {
            var local = _value.LocalDateTime;
            var newDay = day ?? local.Day;
            // JavaScript months are 0-indexed
            _value = new DateTimeOffset(local.Year, month + 1, newDay, local.Hour, local.Minute, local.Second, local.Millisecond, _value.Offset);
            return getTime();
        }

        /// <summary>
        /// Set full year (and optionally month, day)
        /// </summary>
        public double setFullYear(int year, int? month = null, int? day = null)
        {
            var local = _value.LocalDateTime;
            // JavaScript months are 0-indexed, need to add 1
            var newMonth = month.HasValue ? month.Value + 1 : local.Month;
            var newDay = day ?? local.Day;
            _value = new DateTimeOffset(year, newMonth, newDay, local.Hour, local.Minute, local.Second, local.Millisecond, _value.Offset);
            return getTime();
        }

        // ==================== Instance Methods - Setters (UTC) ====================

        /// <summary>
        /// Set UTC milliseconds
        /// </summary>
        public double setUTCMilliseconds(int ms)
        {
            var utc = _value.UtcDateTime;
            _value = new DateTimeOffset(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, utc.Second, ms, TimeSpan.Zero);
            return getTime();
        }

        /// <summary>
        /// Set UTC seconds
        /// </summary>
        public double setUTCSeconds(int sec, int? ms = null)
        {
            var utc = _value.UtcDateTime;
            var newMs = ms ?? utc.Millisecond;
            _value = new DateTimeOffset(utc.Year, utc.Month, utc.Day, utc.Hour, utc.Minute, sec, newMs, TimeSpan.Zero);
            return getTime();
        }

        /// <summary>
        /// Set UTC minutes
        /// </summary>
        public double setUTCMinutes(int min, int? sec = null, int? ms = null)
        {
            var utc = _value.UtcDateTime;
            var newSec = sec ?? utc.Second;
            var newMs = ms ?? utc.Millisecond;
            _value = new DateTimeOffset(utc.Year, utc.Month, utc.Day, utc.Hour, min, newSec, newMs, TimeSpan.Zero);
            return getTime();
        }

        /// <summary>
        /// Set UTC hours
        /// </summary>
        public double setUTCHours(int hour, int? min = null, int? sec = null, int? ms = null)
        {
            var utc = _value.UtcDateTime;
            var newMin = min ?? utc.Minute;
            var newSec = sec ?? utc.Second;
            var newMs = ms ?? utc.Millisecond;
            _value = new DateTimeOffset(utc.Year, utc.Month, utc.Day, hour, newMin, newSec, newMs, TimeSpan.Zero);
            return getTime();
        }

        /// <summary>
        /// Set UTC day of month
        /// </summary>
        public double setUTCDate(int day)
        {
            var utc = _value.UtcDateTime;
            _value = new DateTimeOffset(utc.Year, utc.Month, day, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, TimeSpan.Zero);
            return getTime();
        }

        /// <summary>
        /// Set UTC month (0-11)
        /// </summary>
        public double setUTCMonth(int month, int? day = null)
        {
            var utc = _value.UtcDateTime;
            var newDay = day ?? utc.Day;
            _value = new DateTimeOffset(utc.Year, month + 1, newDay, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, TimeSpan.Zero);
            return getTime();
        }

        /// <summary>
        /// Set UTC full year
        /// </summary>
        public double setUTCFullYear(int year, int? month = null, int? day = null)
        {
            var utc = _value.UtcDateTime;
            var newMonth = month.HasValue ? month.Value + 1 : utc.Month;
            var newDay = day ?? utc.Day;
            _value = new DateTimeOffset(year, newMonth, newDay, utc.Hour, utc.Minute, utc.Second, utc.Millisecond, TimeSpan.Zero);
            return getTime();
        }

        // ==================== Instance Methods - String Conversion ====================

        /// <summary>
        /// Convert to string representation
        /// </summary>
        public override string ToString() => _value.LocalDateTime.ToString("ddd MMM dd yyyy HH:mm:ss 'GMT'zzz", CultureInfo.InvariantCulture);

        /// <summary>
        /// Convert to date string
        /// </summary>
        public string toDateString() => _value.LocalDateTime.ToString("ddd MMM dd yyyy", CultureInfo.InvariantCulture);

        /// <summary>
        /// Convert to time string
        /// </summary>
        public string toTimeString() => _value.LocalDateTime.ToString("HH:mm:ss 'GMT'zzz", CultureInfo.InvariantCulture);

        /// <summary>
        /// Convert to ISO 8601 string
        /// </summary>
        public string toISOString() => _value.UtcDateTime.ToString("yyyy-MM-ddTHH:mm:ss.fffZ", CultureInfo.InvariantCulture);

        /// <summary>
        /// Convert to UTC string
        /// </summary>
        public string toUTCString() => _value.UtcDateTime.ToString("ddd, dd MMM yyyy HH:mm:ss 'GMT'", CultureInfo.InvariantCulture);

        /// <summary>
        /// Convert to JSON (same as toISOString)
        /// </summary>
        public string toJSON() => toISOString();

        /// <summary>
        /// Convert to locale date string
        /// </summary>
        public string toLocaleDateString() => _value.LocalDateTime.ToShortDateString();

        /// <summary>
        /// Convert to locale time string
        /// </summary>
        public string toLocaleTimeString() => _value.LocalDateTime.ToShortTimeString();

        /// <summary>
        /// Convert to locale string
        /// </summary>
        public string toLocaleString() => _value.LocalDateTime.ToString();

        // ==================== Primitive Value ====================

        /// <summary>
        /// Get primitive value (milliseconds since epoch)
        /// </summary>
        public double valueOf() => getTime();
    }
}
