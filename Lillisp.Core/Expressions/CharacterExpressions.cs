using System;
using System.Linq;

namespace Lillisp.Core.Expressions
{
    public static class CharacterExpressions
    {
        public static object? Equals(object?[] args)
        {
            char? last = null;

            foreach (var arg in args.Cast<char>())
            {
                if (last != null && arg != last)
                    return false;

                last = arg;
            }

            return true;
        }

        public static object? LessThan(object?[] args)
        {
            char? last = null;

            foreach (var arg in args.Cast<char>())
            {
                if (last >= arg)
                    return false;

                last = arg;
            }

            return true;
        }

        public static object? GreaterThan(object?[] args)
        {
            char? last = null;

            foreach (var arg in args.Cast<char>())
            {
                if (last <= arg)
                    return false;

                last = arg;
            }

            return true;
        }

        public static object? LessThanOrEqualTo(object?[] args)
        {
            char? last = null;

            foreach (var arg in args.Cast<char>())
            {
                if (last > arg)
                    return false;

                last = arg;
            }

            return true;
        }

        public static object? GreaterThanOrEqualTo(object?[] args)
        {
            char? last = null;

            foreach (var arg in args.Cast<char>())
            {
                if (last < arg)
                    return false;

                last = arg;
            }

            return true;
        }

        public static object? CaseInsensitiveEquals(object?[] args)
        {
            char? last = null;

            foreach (var arg in args.Cast<char>().Select(char.ToUpperInvariant))
            {
                if (last != null && arg != last)
                    return false;

                last = arg;
            }

            return true;
        }

        public static object? CaseInsensitiveLessThan(object?[] args)
        {
            char? last = null;

            foreach (var arg in args.Cast<char>().Select(char.ToUpperInvariant))
            {
                if (last >= arg)
                    return false;

                last = arg;
            }

            return true;
        }

        public static object? CaseInsensitiveGreaterThan(object?[] args)
        {
            char? last = null;

            foreach (var arg in args.Cast<char>().Select(char.ToUpperInvariant))
            {
                if (last <= arg)
                    return false;

                last = arg;
            }

            return true;
        }

        public static object? CaseInsensitiveLessThanOrEqualTo(object?[] args)
        {
            char? last = null;

            foreach (var arg in args.Cast<char>().Select(char.ToUpperInvariant))
            {
                if (last > arg)
                    return false;

                last = arg;
            }

            return true;
        }

        public static object? CaseInsensitiveGreaterThanOrEqualTo(object?[] args)
        {
            char? last = null;

            foreach (var arg in args.Cast<char>().Select(char.ToUpperInvariant))
            {
                if (last < arg)
                    return false;

                last = arg;
            }

            return true;
        }

        public static object? IsAlphabetic(object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("A character is required.");
            }

            return args[0] is char c && char.IsLetter(c);
        }

        public static object? IsNumeric(object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("A character is required.");
            }

            return args[0] is char c && char.IsNumber(c);
        }

        public static object? IsWhitespace(object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("A character is required.");
            }

            return args[0] is char c && char.IsWhiteSpace(c);
        }

        public static object? IsUpperCase(object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("A character is required.");
            }

            return args[0] is char c && char.IsUpper(c);
        }

        public static object? IsLowerCase(object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("A character is required.");
            }

            return args[0] is char c && char.IsLower(c);
        }
    }
}
