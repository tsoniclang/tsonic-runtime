using System;
using Xunit;

namespace Tsonic.JSRuntime.Tests
{
    public class DateTests
    {
        // ==================== Constructor Tests ====================

        [Fact]
        public void Date_DefaultConstructor_CreatesCurrentTime()
        {
            var before = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var date = new Date();
            var after = DateTimeOffset.Now.ToUnixTimeMilliseconds();

            var time = date.getTime();
            // Allow small tolerance for timing differences
            Assert.True(time >= before - 10 && time <= after + 10);
        }

        [Fact]
        public void Date_MillisecondsConstructor_CreatesCorrectDate()
        {
            // January 1, 2000 00:00:00 UTC = 946684800000ms
            var date = new Date(946684800000);
            Assert.Equal(2000, date.getUTCFullYear());
            Assert.Equal(0, date.getUTCMonth()); // January
            Assert.Equal(1, date.getUTCDate());
        }

        [Fact]
        public void Date_StringConstructor_ParsesISOString()
        {
            var date = new Date("2023-06-15T12:30:45.123Z");
            Assert.Equal(2023, date.getUTCFullYear());
            Assert.Equal(5, date.getUTCMonth()); // June (0-indexed)
            Assert.Equal(15, date.getUTCDate());
            Assert.Equal(12, date.getUTCHours());
            Assert.Equal(30, date.getUTCMinutes());
            Assert.Equal(45, date.getUTCSeconds());
        }

        [Fact]
        public void Date_ComponentsConstructor_CreatesCorrectDate()
        {
            // Month is 0-indexed in JavaScript
            var date = new Date(2023, 5, 15, 10, 30, 45, 123); // June 15, 2023
            Assert.Equal(2023, date.getFullYear());
            Assert.Equal(5, date.getMonth()); // June
            Assert.Equal(15, date.getDate());
            Assert.Equal(10, date.getHours());
            Assert.Equal(30, date.getMinutes());
            Assert.Equal(45, date.getSeconds());
            Assert.Equal(123, date.getMilliseconds());
        }

        // ==================== Static Method Tests ====================

        [Fact]
        public void Date_now_ReturnsCurrentTimeInMilliseconds()
        {
            var before = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            var now = Date.now();
            var after = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

            // Allow small tolerance for timing differences
            Assert.True(now >= before - 10 && now <= after + 10);
        }

        [Fact]
        public void Date_parse_ParsesValidString()
        {
            var ms = Date.parse("2023-06-15T00:00:00Z");
            var date = new Date(ms);
            Assert.Equal(2023, date.getUTCFullYear());
            Assert.Equal(5, date.getUTCMonth());
            Assert.Equal(15, date.getUTCDate());
        }

        [Fact]
        public void Date_parse_ReturnsNaNForInvalidString()
        {
            var result = Date.parse("not a date");
            Assert.True(double.IsNaN(result));
        }

        [Fact]
        public void Date_UTC_ReturnsMilliseconds()
        {
            // January 1, 2000 00:00:00 UTC
            var ms = Date.UTC(2000, 0, 1);
            Assert.Equal(946684800000, ms);
        }

        [Fact]
        public void Date_UTC_WithAllComponents()
        {
            var ms = Date.UTC(2023, 5, 15, 12, 30, 45, 123);
            var date = new Date(ms);
            Assert.Equal(2023, date.getUTCFullYear());
            Assert.Equal(5, date.getUTCMonth());
            Assert.Equal(15, date.getUTCDate());
            Assert.Equal(12, date.getUTCHours());
            Assert.Equal(30, date.getUTCMinutes());
            Assert.Equal(45, date.getUTCSeconds());
            Assert.Equal(123, date.getUTCMilliseconds());
        }

        // ==================== UTC Getter Tests ====================

        [Fact]
        public void Date_getUTCFullYear_ReturnsYear()
        {
            var date = new Date(Date.UTC(2023, 11, 25)); // Dec 25, 2023
            Assert.Equal(2023, date.getUTCFullYear());
        }

        [Fact]
        public void Date_getUTCMonth_ReturnsZeroIndexedMonth()
        {
            var date = new Date(Date.UTC(2023, 11, 25)); // December
            Assert.Equal(11, date.getUTCMonth());
        }

        [Fact]
        public void Date_getUTCDate_ReturnsDayOfMonth()
        {
            var date = new Date(Date.UTC(2023, 11, 25));
            Assert.Equal(25, date.getUTCDate());
        }

        [Fact]
        public void Date_getUTCDay_ReturnsDayOfWeek()
        {
            // January 1, 2023 was a Sunday (0)
            var date = new Date(Date.UTC(2023, 0, 1));
            Assert.Equal(0, date.getUTCDay());

            // January 2, 2023 was a Monday (1)
            var date2 = new Date(Date.UTC(2023, 0, 2));
            Assert.Equal(1, date2.getUTCDay());
        }

        [Fact]
        public void Date_getUTCHours_ReturnsHours()
        {
            var date = new Date(Date.UTC(2023, 0, 1, 14));
            Assert.Equal(14, date.getUTCHours());
        }

        [Fact]
        public void Date_getUTCMinutes_ReturnsMinutes()
        {
            var date = new Date(Date.UTC(2023, 0, 1, 0, 30));
            Assert.Equal(30, date.getUTCMinutes());
        }

        [Fact]
        public void Date_getUTCSeconds_ReturnsSeconds()
        {
            var date = new Date(Date.UTC(2023, 0, 1, 0, 0, 45));
            Assert.Equal(45, date.getUTCSeconds());
        }

        [Fact]
        public void Date_getUTCMilliseconds_ReturnsMilliseconds()
        {
            var date = new Date(Date.UTC(2023, 0, 1, 0, 0, 0, 123));
            Assert.Equal(123, date.getUTCMilliseconds());
        }

        // ==================== Setter Tests ====================

        [Fact]
        public void Date_setTime_SetsMilliseconds()
        {
            var date = new Date();
            var expected = 946684800000.0; // Jan 1, 2000
            date.setTime(expected);
            Assert.Equal(expected, date.getTime());
        }

        [Fact]
        public void Date_setUTCFullYear_SetsYear()
        {
            var date = new Date(Date.UTC(2000, 0, 1));
            date.setUTCFullYear(2025);
            Assert.Equal(2025, date.getUTCFullYear());
        }

        [Fact]
        public void Date_setUTCMonth_SetsMonth()
        {
            var date = new Date(Date.UTC(2023, 0, 15));
            date.setUTCMonth(6); // July
            Assert.Equal(6, date.getUTCMonth());
        }

        [Fact]
        public void Date_setUTCDate_SetsDay()
        {
            var date = new Date(Date.UTC(2023, 5, 1));
            date.setUTCDate(20);
            Assert.Equal(20, date.getUTCDate());
        }

        [Fact]
        public void Date_setUTCHours_SetsHours()
        {
            var date = new Date(Date.UTC(2023, 0, 1, 0));
            date.setUTCHours(15);
            Assert.Equal(15, date.getUTCHours());
        }

        [Fact]
        public void Date_setUTCMinutes_SetsMinutes()
        {
            var date = new Date(Date.UTC(2023, 0, 1, 0, 0));
            date.setUTCMinutes(45);
            Assert.Equal(45, date.getUTCMinutes());
        }

        [Fact]
        public void Date_setUTCSeconds_SetsSeconds()
        {
            var date = new Date(Date.UTC(2023, 0, 1, 0, 0, 0));
            date.setUTCSeconds(30);
            Assert.Equal(30, date.getUTCSeconds());
        }

        [Fact]
        public void Date_setUTCMilliseconds_SetsMilliseconds()
        {
            var date = new Date(Date.UTC(2023, 0, 1, 0, 0, 0, 0));
            date.setUTCMilliseconds(500);
            Assert.Equal(500, date.getUTCMilliseconds());
        }

        // ==================== String Conversion Tests ====================

        [Fact]
        public void Date_toISOString_ReturnsISO8601Format()
        {
            var date = new Date(Date.UTC(2023, 5, 15, 12, 30, 45, 123));
            var iso = date.toISOString();
            Assert.Equal("2023-06-15T12:30:45.123Z", iso);
        }

        [Fact]
        public void Date_toJSON_ReturnsISO8601Format()
        {
            var date = new Date(Date.UTC(2023, 5, 15, 12, 30, 45, 123));
            var json = date.toJSON();
            Assert.Equal("2023-06-15T12:30:45.123Z", json);
        }

        [Fact]
        public void Date_toUTCString_ReturnsUTCFormat()
        {
            var date = new Date(Date.UTC(2023, 5, 15, 12, 30, 45));
            var utcStr = date.toUTCString();
            Assert.Contains("Thu", utcStr);
            Assert.Contains("15", utcStr);
            Assert.Contains("Jun", utcStr);
            Assert.Contains("2023", utcStr);
        }

        [Fact]
        public void Date_toDateString_ReturnsDatePortion()
        {
            var date = new Date(2023, 5, 15);
            var dateStr = date.toDateString();
            Assert.Contains("Jun", dateStr);
            Assert.Contains("15", dateStr);
            Assert.Contains("2023", dateStr);
        }

        // ==================== valueOf Tests ====================

        [Fact]
        public void Date_valueOf_ReturnsMilliseconds()
        {
            var ms = 946684800000.0;
            var date = new Date(ms);
            Assert.Equal(ms, date.valueOf());
        }

        [Fact]
        public void Date_getTime_SameAsValueOf()
        {
            var date = new Date(Date.UTC(2023, 5, 15));
            Assert.Equal(date.valueOf(), date.getTime());
        }

        // ==================== Edge Cases ====================

        [Fact]
        public void Date_EpochTime_IsCorrect()
        {
            var date = new Date(0);
            Assert.Equal(1970, date.getUTCFullYear());
            Assert.Equal(0, date.getUTCMonth());
            Assert.Equal(1, date.getUTCDate());
            Assert.Equal(0, date.getUTCHours());
            Assert.Equal(0, date.getUTCMinutes());
            Assert.Equal(0, date.getUTCSeconds());
        }

        [Fact]
        public void Date_NegativeMilliseconds_WorksCorrectly()
        {
            // Before epoch
            var date = new Date(-86400000); // 1 day before epoch
            Assert.Equal(1969, date.getUTCFullYear());
            Assert.Equal(11, date.getUTCMonth()); // December
            Assert.Equal(31, date.getUTCDate());
        }

        [Fact]
        public void Date_LeapYear_Handled()
        {
            // Feb 29 in a leap year
            var date = new Date(Date.UTC(2024, 1, 29)); // Feb 29, 2024
            Assert.Equal(2024, date.getUTCFullYear());
            Assert.Equal(1, date.getUTCMonth()); // February
            Assert.Equal(29, date.getUTCDate());
        }

        [Fact]
        public void Date_Setters_ReturnNewTime()
        {
            var date = new Date(Date.UTC(2023, 0, 1));
            var result = date.setUTCFullYear(2025);
            Assert.Equal(date.getTime(), result);
        }
    }
}
