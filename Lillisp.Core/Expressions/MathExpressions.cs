using System;

namespace Lillisp.Core.Expressions
{
    public static class MathExpressions
    {
        public static object? Add(object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("+ requires at least two arguments");
            }

            var sum = Convert.ToDouble(args[0]);

            for (int i = 1; i < args.Length; i++)
            {
                sum += Convert.ToDouble(args[i]);
            }

            return sum;
        }

        public static object? Subtract(object?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("- requires at least two arguments");
            }

            var difference = Convert.ToDouble(args[0]);

            for (int i = 1; i < args.Length; i++)
            {
                difference -= Convert.ToDouble(args[i]);
            }

            return difference;
        }

        public static object? Multiply(object?[] args)
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

        public static object? Divide(object?[] args)
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

        public static object? Modulo(object?[] args)
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

        public static object? Power(object?[] args)
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

        public static object? Abs(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("abs requires one argument");
            }

            return Math.Abs(Convert.ToDouble(args[0]));
        }

        public static object? Max(object?[] args)
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

        public static object? Min(object?[] args)
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
    }
}
