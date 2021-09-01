using System;

namespace Lillisp.Core.Expressions
{
    public static class BooleanExpressions
    {
        public static object? LessThan(Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("< needs at least 2 arguments");
            }

            double prev = Convert.ToDouble(args[0]);

            for (int i = 1; i < args.Length; i++)
            {
                double next = Convert.ToDouble(args[i]);

                if (prev >= next)
                    return false;

                prev = next;
            }

            return true;
        }

        public static object? GreaterThan(Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("> needs at least 2 arguments");
            }

            double prev = Convert.ToDouble(args[0]);

            for (int i = 1; i < args.Length; i++)
            {
                double next = Convert.ToDouble(args[i]);

                if (prev <= next)
                    return false;

                prev = next;
            }

            return true;
        }

        public static object? LessThanOrEqual(Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("<= needs at least 2 arguments");
            }

            double prev = Convert.ToDouble(args[0]);

            for (int i = 1; i < args.Length; i++)
            {
                double next = Convert.ToDouble(args[i]);

                if (prev > next)
                    return false;

                prev = next;
            }

            return true;
        }

        public static object? GreaterThanOrEqual(Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException(">= needs at least 2 arguments");
            }

            double prev = Convert.ToDouble(args[0]);

            for (int i = 1; i < args.Length; i++)
            {
                double next = Convert.ToDouble(args[i]);

                if (prev < next)
                    return false;

                prev = next;
            }

            return true;
        }

        public static object? Equal(Scope scope, object?[] args)
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

        public static object? Not(Scope scope, object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("not needs one argument");
            }

            return !args[0].IsTruthy() ? true : Nil.Value;
        }
    }
}
