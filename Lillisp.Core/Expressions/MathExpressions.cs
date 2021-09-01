using System;

namespace Lillisp.Core.Expressions
{
    public static class MathExpressions
    {
        public static object? Plus(Scope scope, object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("+ requires at least one argument");
            }

            var sum = Convert.ToDouble(args[0]);

            if (args.Length == 1)
            {
                return sum < 0 ? sum * -1 : sum;
            }

            for (int i = 1; i < args.Length; i++)
            {
                sum += Convert.ToDouble(args[i]);
            }

            return sum;
        }

        public static object? Minus(Scope scope, object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("- requires at least one argument");
            }

            var difference = Convert.ToDouble(args[0]);

            if (args.Length == 1)
            {
                return difference * -1;
            }

            for (int i = 1; i < args.Length; i++)
            {
                difference -= Convert.ToDouble(args[i]);
            }

            return difference;
        }

        public static object? Multiply(Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("* requires at least two arguments");
            }

            var result = Convert.ToDouble(args[0]);

            for (int i = 1; i < args.Length; i++)
            {
                result *= Convert.ToDouble(args[i]);
            }

            return result;
        }

        public static object? Divide(Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("/ requires at least two arguments");
            }

            var result = Convert.ToDouble(args[0]);

            for (int i = 1; i < args.Length; i++)
            {
                result /= Convert.ToDouble(args[i]);
            }

            return result;
        }

        public static object? Modulo(Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("% requires at least two arguments");
            }

            var result = Convert.ToDouble(args[0]);

            for (int i = 1; i < args.Length; i++)
            {
                result %= Convert.ToDouble(args[i]);
            }

            return result;
        }

        public static object? Power(Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("^ requires at least two arguments");
            }

            var result = Convert.ToDouble(args[0]);

            for (int i = 1; i < args.Length; i++)
            {
                result = Math.Pow(result, Convert.ToDouble(args[i]));
            }

            return result;
        }

        public static object? Abs(Scope scope, object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("abs requires one argument");
            }

            return Math.Abs(Convert.ToDouble(args[0]));
        }

        public static object? Max(Scope scope, object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("max requires at least one argument");
            }

            double max = Convert.ToDouble(args[0]);

            if (args.Length == 1)
            {
                return max;
            }

            for (int i = 1; i < args.Length; i++)
            {
                max = Math.Max(max, Convert.ToDouble(args[i]));
            }

            return max;
        }

        public static object? Min(Scope scope, object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("max requires at least one argument");
            }

            double min = Convert.ToDouble(args[0]);

            if (args.Length == 1)
            {
                return min;
            }

            for (int i = 1; i < args.Length; i++)
            {
                min = Math.Min(min, Convert.ToDouble(args[i]));
            }

            return min;
        }

        public static object? Sqrt(Scope scope, object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("sqrt requires one argument");
            }

            return Math.Sqrt(Convert.ToDouble(args[0]));
        }

        public static object? ShiftRight(Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException(">> requires at least two arguments");
            }

            var result = Convert.ToInt32(args[0]);

            for (int i = 1; i < args.Length; i++)
            {
                result >>= Convert.ToInt32(args[i]);
            }

            return result;
        }

        public static object? ShiftLeft(Scope scope, object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException(">> requires at least two arguments");
            }

            var result = Convert.ToInt32(args[0]);

            for (int i = 1; i < args.Length; i++)
            {
                result <<= Convert.ToInt32(args[i]);
            }

            return result;
        }

        public static object? Ln(Scope scope, object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("ln requires one argument");
            }

            return Math.Log(Convert.ToDouble(args[0]), Math.E);
        }

        public static object? Log(Scope scope, object?[] args)
        {
            if (args.Length is 0 or > 2)
            {
                throw new ArgumentException("log requires one or two arguments");
            }

            return Math.Log(Convert.ToDouble(args[0]), args.Length == 2 ? Convert.ToDouble(args[1]) : 10);
        }
    }
}
