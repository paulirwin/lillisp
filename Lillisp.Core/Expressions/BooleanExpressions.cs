using System;

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

        public static object? Equal(object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("= needs at least 2 arguments");
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
    }
}
