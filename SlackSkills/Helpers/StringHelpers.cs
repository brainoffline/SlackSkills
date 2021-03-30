using System;

namespace SlackSkills
{
    public static class StringHelpers
    {
        public static bool IgnoreCaseEquals( this string? str, string? other ) =>
            string.Equals( str, other, StringComparison.OrdinalIgnoreCase );
    }
}
