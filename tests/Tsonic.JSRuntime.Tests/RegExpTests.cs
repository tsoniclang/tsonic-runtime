using Xunit;

namespace Tsonic.JSRuntime.Tests
{
    public class RegExpTests
    {
        // ==================== Constructor Tests ====================

        [Fact]
        public void RegExp_Constructor_WithPatternOnly()
        {
            var regex = new RegExp("abc");
            Assert.Equal("abc", regex.source);
            Assert.Equal("", regex.flags);
        }

        [Fact]
        public void RegExp_Constructor_WithFlags()
        {
            var regex = new RegExp("abc", "gi");
            Assert.Equal("abc", regex.source);
            Assert.Equal("gi", regex.flags);
        }

        [Fact]
        public void RegExp_Constructor_NullFlags_TreatedAsEmpty()
        {
            var regex = new RegExp("abc", null!);
            Assert.Equal("", regex.flags);
        }

        // ==================== Property Tests ====================

        [Fact]
        public void RegExp_global_ReturnsCorrectValue()
        {
            Assert.True(new RegExp("abc", "g").global);
            Assert.True(new RegExp("abc", "gi").global);
            Assert.False(new RegExp("abc", "i").global);
            Assert.False(new RegExp("abc").global);
        }

        [Fact]
        public void RegExp_ignoreCase_ReturnsCorrectValue()
        {
            Assert.True(new RegExp("abc", "i").ignoreCase);
            Assert.True(new RegExp("abc", "gi").ignoreCase);
            Assert.False(new RegExp("abc", "g").ignoreCase);
            Assert.False(new RegExp("abc").ignoreCase);
        }

        [Fact]
        public void RegExp_multiline_ReturnsCorrectValue()
        {
            Assert.True(new RegExp("abc", "m").multiline);
            Assert.True(new RegExp("abc", "gim").multiline);
            Assert.False(new RegExp("abc", "gi").multiline);
        }

        [Fact]
        public void RegExp_dotAll_ReturnsCorrectValue()
        {
            Assert.True(new RegExp("abc", "s").dotAll);
            Assert.False(new RegExp("abc", "gi").dotAll);
        }

        [Fact]
        public void RegExp_unicode_ReturnsCorrectValue()
        {
            Assert.True(new RegExp("abc", "u").unicode);
            Assert.False(new RegExp("abc", "gi").unicode);
        }

        [Fact]
        public void RegExp_sticky_ReturnsCorrectValue()
        {
            Assert.True(new RegExp("abc", "y").sticky);
            Assert.False(new RegExp("abc", "gi").sticky);
        }

        [Fact]
        public void RegExp_lastIndex_DefaultsToZero()
        {
            var regex = new RegExp("abc", "g");
            Assert.Equal(0, regex.lastIndex);
        }

        [Fact]
        public void RegExp_lastIndex_CanBeSet()
        {
            var regex = new RegExp("abc", "g");
            regex.lastIndex = 5;
            Assert.Equal(5, regex.lastIndex);
        }

        [Fact]
        public void RegExp_lastIndex_NegativeBecomesZero()
        {
            var regex = new RegExp("abc", "g");
            regex.lastIndex = -10;
            Assert.Equal(0, regex.lastIndex);
        }

        // ==================== test() Method Tests ====================

        [Fact]
        public void RegExp_test_ReturnsTrue_WhenMatches()
        {
            var regex = new RegExp("abc");
            Assert.True(regex.test("abc"));
            Assert.True(regex.test("xyzabc123"));
        }

        [Fact]
        public void RegExp_test_ReturnsFalse_WhenNoMatch()
        {
            var regex = new RegExp("abc");
            Assert.False(regex.test("def"));
            Assert.False(regex.test("ABC")); // Case sensitive
        }

        [Fact]
        public void RegExp_test_IgnoreCase_Works()
        {
            var regex = new RegExp("abc", "i");
            Assert.True(regex.test("ABC"));
            Assert.True(regex.test("AbC"));
        }

        [Fact]
        public void RegExp_test_Global_UpdatesLastIndex()
        {
            var regex = new RegExp("a", "g");
            var str = "aaa";

            Assert.True(regex.test(str));
            Assert.Equal(1, regex.lastIndex);

            Assert.True(regex.test(str));
            Assert.Equal(2, regex.lastIndex);

            Assert.True(regex.test(str));
            Assert.Equal(3, regex.lastIndex);

            Assert.False(regex.test(str));
            Assert.Equal(0, regex.lastIndex);
        }

        [Fact]
        public void RegExp_test_NonGlobal_DoesNotUpdateLastIndex()
        {
            var regex = new RegExp("a");
            var str = "aaa";

            Assert.True(regex.test(str));
            Assert.Equal(0, regex.lastIndex);

            Assert.True(regex.test(str));
            Assert.Equal(0, regex.lastIndex);
        }

        [Fact]
        public void RegExp_test_NullString_ReturnsFalse()
        {
            var regex = new RegExp("abc");
            Assert.False(regex.test(null!));
        }

        // ==================== exec() Method Tests ====================

        [Fact]
        public void RegExp_exec_ReturnsMatch_WhenFound()
        {
            var regex = new RegExp("abc");
            var result = regex.exec("xyzabc123");

            Assert.NotNull(result);
            Assert.Equal("abc", result.value);
            Assert.Equal(3, result.index);
            Assert.Equal("xyzabc123", result.input);
        }

        [Fact]
        public void RegExp_exec_ReturnsNull_WhenNoMatch()
        {
            var regex = new RegExp("abc");
            var result = regex.exec("def");
            Assert.Null(result);
        }

        [Fact]
        public void RegExp_exec_CapturesGroups()
        {
            var regex = new RegExp(@"(\d+)-(\d+)");
            var result = regex.exec("Phone: 123-4567");

            Assert.NotNull(result);
            Assert.Equal("123-4567", result[0]); // Full match
            Assert.Equal("123", result[1]);      // Group 1
            Assert.Equal("4567", result[2]);     // Group 2
        }

        [Fact]
        public void RegExp_exec_Global_IteratesThroughMatches()
        {
            var regex = new RegExp(@"\d+", "g");
            var str = "a1b22c333";

            var result1 = regex.exec(str);
            Assert.NotNull(result1);
            Assert.Equal("1", result1.value);
            Assert.Equal(1, result1.index);

            var result2 = regex.exec(str);
            Assert.NotNull(result2);
            Assert.Equal("22", result2.value);
            Assert.Equal(3, result2.index);

            var result3 = regex.exec(str);
            Assert.NotNull(result3);
            Assert.Equal("333", result3.value);
            Assert.Equal(6, result3.index);

            var result4 = regex.exec(str);
            Assert.Null(result4);
            Assert.Equal(0, regex.lastIndex);
        }

        [Fact]
        public void RegExp_exec_Global_ResetsOnNoMatch()
        {
            var regex = new RegExp("abc", "g");
            regex.lastIndex = 100;

            var result = regex.exec("abc");
            Assert.Null(result);
            Assert.Equal(0, regex.lastIndex);
        }

        [Fact]
        public void RegExp_exec_NonGlobal_DoesNotUpdateLastIndex()
        {
            var regex = new RegExp(@"\d+");
            var str = "a1b22c333";

            var result1 = regex.exec(str);
            Assert.NotNull(result1);
            Assert.Equal("1", result1.value);
            Assert.Equal(0, regex.lastIndex);

            // Without global flag, always starts from beginning
            var result2 = regex.exec(str);
            Assert.NotNull(result2);
            Assert.Equal("1", result2.value);
        }

        [Fact]
        public void RegExp_exec_NullString_ReturnsNull()
        {
            var regex = new RegExp("abc");
            Assert.Null(regex.exec(null!));
        }

        // ==================== Sticky Flag Tests ====================

        [Fact]
        public void RegExp_Sticky_MustMatchAtLastIndex()
        {
            var regex = new RegExp("abc", "y");

            // Match at position 0
            Assert.NotNull(regex.exec("abcdef"));
            Assert.Equal(3, regex.lastIndex);

            // Must match at position 3, but "abc" is not there
            Assert.Null(regex.exec("abcdef"));
            Assert.Equal(0, regex.lastIndex);
        }

        [Fact]
        public void RegExp_Sticky_MatchesAtCorrectPosition()
        {
            var regex = new RegExp("abc", "y");
            regex.lastIndex = 3;

            var result = regex.exec("xyzabcdef");
            Assert.NotNull(result);
            Assert.Equal("abc", result.value);
            Assert.Equal(3, result.index);
        }

        // ==================== Multiline Flag Tests ====================

        [Fact]
        public void RegExp_Multiline_AnchorMatchesLineStart()
        {
            var regexMultiline = new RegExp("^abc", "m");
            var regexNormal = new RegExp("^abc");

            var str = "xyz\nabc";

            Assert.True(regexMultiline.test(str));
            Assert.False(regexNormal.test(str));
        }

        // ==================== DotAll Flag Tests ====================

        [Fact]
        public void RegExp_DotAll_DotMatchesNewline()
        {
            var regexDotAll = new RegExp("a.b", "s");
            var regexNormal = new RegExp("a.b");

            var str = "a\nb";

            Assert.True(regexDotAll.test(str));
            Assert.False(regexNormal.test(str));
        }

        // ==================== ToString Tests ====================

        [Fact]
        public void RegExp_ToString_ReturnsLiteralFormat()
        {
            var regex = new RegExp("abc", "gi");
            Assert.Equal("/abc/gi", regex.ToString());
        }

        [Fact]
        public void RegExp_ToString_EmptyFlags()
        {
            var regex = new RegExp("abc");
            Assert.Equal("/abc/", regex.ToString());
        }

        // ==================== MatchResult Tests ====================

        [Fact]
        public void RegExpMatchResult_length_ReturnsGroupCount()
        {
            var regex = new RegExp(@"(\d+)-(\d+)");
            var result = regex.exec("123-456");

            Assert.NotNull(result);
            Assert.Equal(3, result.length); // Full match + 2 groups
        }

        [Fact]
        public void RegExpMatchResult_groups_ReturnsAllGroups()
        {
            var regex = new RegExp(@"(\d+)-(\d+)");
            var result = regex.exec("123-456");

            Assert.NotNull(result);
            var groups = result.groups;
            Assert.Equal("123-456", groups[0]);
            Assert.Equal("123", groups[1]);
            Assert.Equal("456", groups[2]);
        }

        [Fact]
        public void RegExpMatchResult_OutOfBoundsIndex_ReturnsNull()
        {
            var regex = new RegExp("abc");
            var result = regex.exec("abc");

            Assert.NotNull(result);
            Assert.Null(result[-1]);
            Assert.Null(result[100]);
        }

        // ==================== Complex Pattern Tests ====================

        [Fact]
        public void RegExp_ComplexPattern_EmailLike()
        {
            var regex = new RegExp(@"[\w.]+@[\w.]+\.\w+");
            Assert.True(regex.test("test@example.com"));
            Assert.True(regex.test("user.name@domain.org"));
            Assert.False(regex.test("invalid-email"));
        }

        [Fact]
        public void RegExp_ComplexPattern_PhoneNumber()
        {
            var regex = new RegExp(@"\(\d{3}\)\s*\d{3}-\d{4}");
            Assert.True(regex.test("(123) 456-7890"));
            Assert.True(regex.test("(123)456-7890"));
            Assert.False(regex.test("123-456-7890"));
        }

        // ==================== Edge Cases ====================

        [Fact]
        public void RegExp_EmptyPattern_MatchesAnything()
        {
            var regex = new RegExp("");
            Assert.True(regex.test("anything"));
            Assert.True(regex.test(""));
        }

        [Fact]
        public void RegExp_SpecialCharacters_Escaped()
        {
            var regex = new RegExp(@"\.");
            Assert.True(regex.test("a.b"));
            Assert.False(regex.test("axb"));
        }

        [Fact]
        public void RegExp_InvalidPattern_HandledGracefully()
        {
            // Invalid regex should not throw
            var regex = new RegExp("[invalid");
            Assert.False(regex.test("anything"));
        }

        [Fact]
        public void RegExp_OptionalGroup_NullWhenNotMatched()
        {
            var regex = new RegExp(@"(a)(b)?");
            var result = regex.exec("a");

            Assert.NotNull(result);
            Assert.Equal("a", result[0]);
            Assert.Equal("a", result[1]);
            Assert.Null(result[2]); // Group 2 didn't match
        }
    }
}
