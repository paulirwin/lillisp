using System;
using System.Collections.Generic;
using System.Linq;

namespace Lillisp.Core.Expressions
{
    public static class BooleanExpressions
    {
        public static dynamic? LessThan(dynamic?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("< needs at least 2 arguments");
            }

            var prev = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                var next = args[i];

                if (prev >= next)
                    return false;

                prev = next;
            }

            return true;
        }

        public static dynamic? GreaterThan(dynamic?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("> needs at least 2 arguments");
            }

            var prev = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                var next = args[i];

                if (prev <= next)
                    return false;

                prev = next;
            }

            return true;
        }

        public static dynamic? LessThanOrEqual(dynamic?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("<= needs at least 2 arguments");
            }

            var prev = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                var next = args[i];

                if (prev > next)
                    return false;

                prev = next;
            }

            return true;
        }

        public static dynamic? GreaterThanOrEqual(dynamic?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException(">= needs at least 2 arguments");
            }

            var prev = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                var next = args[i];

                if (prev < next)
                    return false;

                prev = next;
            }

            return true;
        }

        public static object? Equivalent(object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("eqv? needs at least 2 arguments");
            }

            var first = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                if (!Equals(first, args[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static object? Not(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("not needs one argument");
            }

            return !args[0].IsTruthy() ? true : Nil.Value;
        }

        public static object? ReferencesEqual(object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("eq? needs at least 2 arguments");
            }

            var first = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                // HACK: Symbols are currently different objects, even though the value is interned. Should they be part of some global symbol cache?
                if (first is Symbol firstSym && args[i] is Symbol secondSym)
                {
                    if (!ReferenceEquals(firstSym.Value, secondSym.Value))
                    {
                        return false;
                    }
                }
                else if (!ReferenceEquals(first, args[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static object? NumericallyEqual(object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("= needs at least 2 arguments");
            }

            if (args[0] == null || !args[0].IsNumber())
            {
                throw new ArgumentException("At least one argument is not a number");
            }

            dynamic first = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                dynamic arg = args[i];

                if (arg == null || !args[i].IsNumber())
                {
                    throw new ArgumentException("At least one argument is not a number");
                }

                if (first != arg)
                {
                    return false;
                }
            }

            return true;
        }

        public static object? Equal(object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("eq? needs at least 2 arguments");
            }

            var first = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                if (!EqualEqualityComparer.Instance.Equals(first, args[i]))
                {
                    return false;
                }
            }

            return true;
        }

        private class EqualEqualityComparer : IEqualityComparer<object?>
        {
            public static readonly EqualEqualityComparer Instance = new();

            public new bool Equals(object? x, object? y)
            {
                return object.Equals(x, y) || (x is IEnumerable<object?> firstEnumerable && y is IEnumerable<object?> secondEnumerable && firstEnumerable.SequenceEqual(secondEnumerable, Instance));
            }

            public int GetHashCode(object? obj)
            {
                return obj?.GetHashCode() ?? 0;
            }
        }
    }
}
