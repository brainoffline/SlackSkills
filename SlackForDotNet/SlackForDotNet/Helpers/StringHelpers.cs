using System;

namespace SlackForDotNet
{
    public static class StringHelpers
    {
        public static bool IgnoreCaseEquals( this string? str, string? other ) =>
            string.Equals( str, other, StringComparison.OrdinalIgnoreCase );
    }
}
