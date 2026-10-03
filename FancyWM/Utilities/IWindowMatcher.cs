using System;
using System.Text.RegularExpressions;

using WinMan;

namespace FancyWM.Utilities
{
    internal interface IWindowMatcher
    {
        bool Matches(IWindow window);
    }

    internal class MatchHelpers
    {
        public static bool IsMatch(string input, string pattern)
        {
            if (string.Equals(input, pattern, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (pattern.Length == 0)
            {
                return false;
            }

            try
            {
                return Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase);
            }
            catch (ArgumentException)
            {
                return false;
            }
        }
    }

    internal class ByProcessNameMatcher(string processName) : IWindowMatcher
    {
        public string ProcessName { get; } = processName;

        public bool Matches(IWindow window)
        {
            return MatchHelpers.IsMatch(window.GetCachedProcessName(), ProcessName);
        }
    }

    internal class ByClassNameMatcher(string className) : IWindowMatcher
    {
        public string ClassName { get; } = className;

        public bool Matches(IWindow window)
        {
            return (window is WinMan.Windows.Win32Window w) && MatchHelpers.IsMatch(w.ClassName, ClassName);
        }
    }

    internal class ByWindowTitleMatcher(string pattern) : IWindowMatcher
    {
        public string Pattern { get; } = pattern;

        private readonly Regex? m_regex = CreateRegex(pattern);

        public bool Matches(IWindow window)
        {
            return m_regex != null && m_regex.IsMatch(window.Title);
        }

        /// <summary>
        /// Patterns wrapped in slashes (e.g. "/^Jupyter/") are treated as regular expressions.
        /// Any other pattern must match the whole title, where "*" matches any sequence of
        /// characters and "?" matches a single character.
        /// </summary>
        private static Regex? CreateRegex(string pattern)
        {
            if (pattern.Length == 0)
            {
                return null;
            }

            string expression;
            RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.CultureInvariant;
            if (pattern.Length > 2 && pattern.StartsWith('/') && pattern.EndsWith('/'))
            {
                expression = pattern[1..^1];
            }
            else
            {
                expression = "^" + Regex.Escape(pattern).Replace(@"\*", ".*").Replace(@"\?", ".") + "$";
                options |= RegexOptions.Singleline;
            }

            try
            {
                return new Regex(expression, options);
            }
            catch (ArgumentException)
            {
                return null;
            }
        }
    }
}
