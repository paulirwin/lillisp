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
    }
}
