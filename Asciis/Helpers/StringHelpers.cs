using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;

using JetBrains.Annotations;

namespace Asciis
{
    public static class StringHelpers
    {
        public static bool IsNullOrWhiteSpace( this IEnumerable< char >? value )
        {
            if (value == null) return true;

            foreach (char c in value)
            {
                if (!char.IsWhiteSpace(c)) 
                    return false;
            }
            
            return true;
        }

        public static bool IsNullOrWhiteSpace([CanBeNull] this string value) => 
            string.IsNullOrWhiteSpace(value);
        public static bool IsNullOrEmpty([CanBeNull] this string value) => 
            string.IsNullOrEmpty(value);
        public static bool HasValue([CanBeNull] this string value) => 
            !string.IsNullOrWhiteSpace(value);

        /// <summary>
        /// Parse a string into multiple arguments (similar to command-line args[]).
        /// Can be a multi-line string
        /// </summary>
        public static List<string>? ParseArguments(this string lines)
        {
            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (lines == null)
                return default;
            
            var list = new List<string>();
            var span = lines.Trim().AsSpan();

            int start = -1;
            for (int i = 0; i < span.Length; i++)
            {
                char ch = span[i];

                if (ch == '"' || ch == '\'')
                {
                    if (start >= 0)
                        AddArg(span, ref start, i);

                    char end = ch;
                    start = i + 1;

                    for (i++; i < span.Length; i++)
                    {
                        ch = span[i];

                        if (ch == end)
                            break;
                    }

                    AddArg(span, ref start, i);
                }
                // Any word separating character
                else if (char.IsWhiteSpace(ch) || ch == '\r' || ch == '\n' || char.IsSeparator(ch))
                {
                    if (start >= 0)
                        AddArg(span, ref start, i);
                }
                // everything else is part of the word
                else
                {
                    if (start < 0)
                        start = i;
                }
            }

            if (start >= 0)
                AddArg(span, ref start, span.Length);

            return list;

            void AddArg(ReadOnlySpan<char> spanReadOnly, ref int argStart, int end)
            {
                var arg = spanReadOnly.Slice(argStart, end - argStart).ToString().Trim();
                if (arg.HasValue())
                    list.Add(arg);
                argStart = -1;
            }
        }

        public static IEnumerable<string> EnumerateLines(this TextReader reader)
        {
            string? line;

            while ((line = reader.ReadLine()) != null)
                yield return line;
        }
        public static IEnumerable<string> EnumerateLines(this string str)
        {
            using var reader = new StringReader( str );
            return EnumerateLines( reader );
        }

        public static char TryGet(this StringBuilder sb, int index)
        {
            return sb.Length > index
                       ? sb[index]
                       : '\0';
        }
        public static char TryGet(this string str, int index)
        {
            return str.Length > index
                       ? str[index]
                       : '\0';
        }
    }
}
