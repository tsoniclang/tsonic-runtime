/**
 * JavaScript RegExp implementation
 * Wraps System.Text.RegularExpressions.Regex with JavaScript semantics
 */

using System;
using System.Text.RegularExpressions;

namespace Tsonic.JSRuntime
{
    /// <summary>
    /// JavaScript RegExp - regular expression handling
    /// </summary>
    public class RegExp
    {
        private readonly Regex _regex;
        private readonly string _pattern;
        private readonly string _flags;
        private int _lastIndex;

        // ==================== Constructors ====================

        /// <summary>
        /// Create RegExp from pattern string
        /// </summary>
        public RegExp(string pattern) : this(pattern, "")
        {
        }

        /// <summary>
        /// Create RegExp from pattern and flags
        /// Flags: g (global), i (ignoreCase), m (multiline), s (dotAll), u (unicode)
        /// </summary>
        public RegExp(string pattern, string flags)
        {
            _pattern = pattern;
            _flags = flags ?? "";
            _lastIndex = 0;

            var options = RegexOptions.None;

            if (_flags.Contains('i'))
                options |= RegexOptions.IgnoreCase;

            if (_flags.Contains('m'))
                options |= RegexOptions.Multiline;

            if (_flags.Contains('s'))
                options |= RegexOptions.Singleline; // Makes . match newlines

            // Note: 'g' (global) and 'y' (sticky) are handled in exec/test methods
            // Note: 'u' (unicode) is handled by default in .NET

            try
            {
                _regex = new Regex(pattern, options);
            }
            catch (ArgumentException)
            {
                // Invalid regex pattern - create a regex that never matches
                _regex = new Regex("(?!)");
            }
        }

        // ==================== Properties ====================

        /// <summary>
        /// The pattern string
        /// </summary>
        public string source => _pattern;

        /// <summary>
        /// The flags string
        /// </summary>
        public string flags => _flags;

        /// <summary>
        /// Whether global flag is set
        /// </summary>
        public bool global => _flags.Contains('g');

        /// <summary>
        /// Whether ignoreCase flag is set
        /// </summary>
        public bool ignoreCase => _flags.Contains('i');

        /// <summary>
        /// Whether multiline flag is set
        /// </summary>
        public bool multiline => _flags.Contains('m');

        /// <summary>
        /// Whether dotAll flag is set
        /// </summary>
        public bool dotAll => _flags.Contains('s');

        /// <summary>
        /// Whether unicode flag is set
        /// </summary>
        public bool unicode => _flags.Contains('u');

        /// <summary>
        /// Whether sticky flag is set
        /// </summary>
        public bool sticky => _flags.Contains('y');

        /// <summary>
        /// Index at which to start the next match (for global/sticky)
        /// </summary>
        public int lastIndex
        {
            get => _lastIndex;
            set => _lastIndex = value < 0 ? 0 : value;
        }

        // ==================== Methods ====================

        /// <summary>
        /// Execute a search for a match in a string
        /// Returns match result or null if no match
        /// </summary>
        public RegExpMatchResult? exec(string str)
        {
            if (str == null) return null;

            int startIndex = (global || sticky) ? _lastIndex : 0;

            if (startIndex < 0 || startIndex > str.Length)
            {
                _lastIndex = 0;
                return null;
            }

            Match match;
            if (sticky)
            {
                // Sticky: match must start exactly at lastIndex
                match = _regex.Match(str, startIndex);
                if (match.Success && match.Index != startIndex)
                {
                    _lastIndex = 0;
                    return null;
                }
            }
            else
            {
                match = _regex.Match(str, startIndex);
            }

            if (!match.Success)
            {
                if (global || sticky)
                    _lastIndex = 0;
                return null;
            }

            if (global || sticky)
                _lastIndex = match.Index + match.Length;

            // Build groups array
            var groups = new string?[match.Groups.Count];
            for (int i = 0; i < match.Groups.Count; i++)
            {
                groups[i] = match.Groups[i].Success ? match.Groups[i].Value : null;
            }

            return new RegExpMatchResult(
                match.Value,
                match.Index,
                str,
                groups
            );
        }

        /// <summary>
        /// Test whether regex matches string
        /// </summary>
        public bool test(string str)
        {
            if (str == null) return false;

            int startIndex = (global || sticky) ? _lastIndex : 0;

            if (startIndex < 0 || startIndex > str.Length)
            {
                _lastIndex = 0;
                return false;
            }

            Match match;
            if (sticky)
            {
                match = _regex.Match(str, startIndex);
                if (match.Success && match.Index != startIndex)
                {
                    _lastIndex = 0;
                    return false;
                }
            }
            else
            {
                match = _regex.Match(str, startIndex);
            }

            if (!match.Success)
            {
                if (global || sticky)
                    _lastIndex = 0;
                return false;
            }

            if (global || sticky)
                _lastIndex = match.Index + match.Length;

            return true;
        }

        /// <summary>
        /// Convert to string representation
        /// </summary>
        public override string ToString() => $"/{_pattern}/{_flags}";

        // ==================== Internal Access ====================

        /// <summary>
        /// Get underlying .NET Regex for use by String methods
        /// </summary>
        internal Regex GetInternalRegex() => _regex;
    }

    /// <summary>
    /// Result from RegExp.exec()
    /// </summary>
    public class RegExpMatchResult
    {
        private readonly string?[] _groups;

        public RegExpMatchResult(string value, int index, string input, string?[] groups)
        {
            this.value = value;
            this.index = index;
            this.input = input;
            _groups = groups;
        }

        /// <summary>
        /// The matched string (same as groups[0])
        /// </summary>
        public string value { get; }

        /// <summary>
        /// Index of the match in the input string
        /// </summary>
        public int index { get; }

        /// <summary>
        /// The original input string
        /// </summary>
        public string input { get; }

        /// <summary>
        /// Number of captured groups (including full match at index 0)
        /// </summary>
        public int length => _groups.Length;

        /// <summary>
        /// Access captured groups by index
        /// Index 0 is the full match, 1+ are capture groups
        /// </summary>
        public string? this[int groupIndex]
        {
            get
            {
                if (groupIndex < 0 || groupIndex >= _groups.Length)
                    return null;
                return _groups[groupIndex];
            }
        }

        /// <summary>
        /// Get all captured groups as array
        /// </summary>
        public string?[] groups => _groups;
    }
}
