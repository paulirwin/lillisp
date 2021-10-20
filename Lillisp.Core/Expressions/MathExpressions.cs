using System;
using System.Numerics;

// ReSharper disable IntVariableOverflowInUncheckedContext - allow for runtime exceptions in these cases

namespace Lillisp.Core.Expressions
{
    public static class MathExpressions
    {
        public static dynamic? Add(dynamic?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("+ requires at least one argument");
            }

            dynamic sum = args[0] ?? throw new NullReferenceException("Cannot apply + to null");

            if (args.Length == 1)
            {
                // make positive as unary operator
                // NOTE: dynamic dispatch doesn't seem to work with pattern matching for "< 0", so checking all signed numeric types
                return sum switch
                {
                    Complex complex => complex.Real < 0 ? new Complex(-1 * complex.Real, complex.Imaginary) : complex, // TODO: I have no idea if this is correct
                    BigInteger bi => bi < 0 ? -1 * bi : bi,
                    sbyte sb => sb < 0 ? -1 * sb : sb,
                    short s => s < 0 ? -1 * s : s,
                    int i => i < 0 ? -1 * i : i,
                    long l => l < 0 ? -1 * l : l,
                    float f => f < 0 ? -1 * f : f,
                    double d => d < 0 ? -1 * d : d,
                    decimal d => d < 0 ? -1 * d : d,
                    _ => sum
                };
            }

            for (int i = 1; i < args.Length; i++)
            {
                sum += args[i];
            }

            return sum;
        }

        public static dynamic? Subtract(dynamic?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("- requires at least one argument");
            }

            var difference = args[0];

            if (args.Length == 1)
            {
                return difference * -1;
            }

            for (int i = 1; i < args.Length; i++)
            {
                difference -= args[i];
            }

            return difference;
        }

        public static dynamic? Multiply(dynamic?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("* requires at least two arguments");
            }

            var result = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                result *= args[i];
            }

            return result;
        }

        public static dynamic? Divide(dynamic?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("/ requires at least two arguments");
            }

            var result = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                result /= args[i];
            }

            return result;
        }

        public static dynamic? Modulo(dynamic?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("% requires at least two arguments");
            }

            var result = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                result %= args[i];
            }

            return result;
        }

        public static dynamic? Power(dynamic?[] args)
        {
            if (args.Length < 2)
            {
                throw new ArgumentException("^ requires at least two arguments");
            }

            var result = args[0];

            for (int i = 1; i < args.Length; i++)
            {
                result = (result, args[i]) switch
                {
                    (Complex a, Complex b) => Complex.Pow(a, b),
                    (Complex a, _) => Complex.Pow(a, Convert.ToDouble(args[i])),
                    _ => Math.Pow(result, Convert.ToDouble(args[i]))
                };
            }

            return result;
        }

        public static object? Abs(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("abs requires one argument");
            }

            if (args[0] == null || args[0].IsUnsignedNumber())
            {
                return args[0];
            }

            return args[0] switch
            {
                Complex complex => Complex.Abs(complex),
                BigInteger bi => BigInteger.Abs(bi),
                decimal d => Math.Abs(d),
                double d => Math.Abs(d),
                float f => Math.Abs(f),
                long l => Math.Abs(l),
                int i => Math.Abs(i),
                short s => Math.Abs(s),
                sbyte sb => Math.Abs(sb),
                _ => Math.Abs(Convert.ToDouble(args[0]))
            };
        }

        public static dynamic? Max(dynamic?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("max requires at least one argument");
            }

            if (args[0] is Complex)
            {
                throw new InvalidOperationException("Cannot compare complex numbers");
            }

            var max = args[0];

            if (args.Length == 1)
            {
                return max;
            }

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] is Complex)
                {
                    throw new InvalidOperationException("Cannot compare complex numbers");
                }

                max = Math.Max(max, args[i]);
            }

            return max;
        }

        public static dynamic? Min(dynamic?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("max requires at least one argument");
            }

            if (args[0] is Complex)
            {
                throw new InvalidOperationException("Cannot compare complex numbers");
            }

            double min = Convert.ToDouble(args[0]);

            if (args.Length == 1)
            {
                return min;
            }

            for (int i = 1; i < args.Length; i++)
            {
                if (args[i] is Complex)
                {
                    throw new InvalidOperationException("Cannot compare complex numbers");
                }

                min = Math.Min(min, args[i]);
            }

            return min;
        }

        public static object? Sqrt(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("sqrt requires one argument");
            }

            return args[0] switch
            {
                // HACK: without the cast to object in one of the cases below, the type of the switch expression is Complex
                double d => (object)Math.Sqrt(d),
                Complex complex => Complex.Sqrt(complex),
                _ => Math.Sqrt(Convert.ToDouble(args[0]))
            };
        }

        public static object? ShiftRight(object?[] args)
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

        public static object? ShiftLeft(object?[] args)
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

        public static object? Ln(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("ln requires one argument");
            }

            return args[0] switch
            {
                // HACK: without the cast to object, the type of the switch expression is Complex
                double d => (object)Math.Log(d),
                Complex complex => Complex.Log(complex),
                _ => Math.Log(Convert.ToDouble(args[0]))
            };
        }

        public static object? Log(object?[] args)
        {
            if (args.Length is 0 or > 2)
            {
                throw new ArgumentException("log requires one or two arguments");
            }

            if (args.Length == 1)
            {
                return args[0] switch
                {
                    // HACK: without the cast to object, the type of the switch expression is Complex
                    double d => (object)Math.Log10(d),
                    Complex complex => Complex.Log10(complex),
                    _ => Math.Log10(Convert.ToDouble(args[0]))
                };
            }

            return (args[0], args[1]) switch
            {
                // HACK: without the cast to object, the type of the switch expression is Complex
                (double d, double b) => (object)Math.Log(d, b),
                (Complex complex, double b) => Complex.Log(complex, b),
                (Complex complex, _) => Complex.Log(complex, Convert.ToDouble(args[1])),
                _ => Math.Log(Convert.ToDouble(args[0]), Convert.ToDouble(args[1]))
            };
        }

        public static dynamic? Decrement(dynamic?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("dec requires one argument");
            }

            return args[0] - 1;
        }

        public static dynamic? Increment(dynamic?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("inc requires one argument");
            }

            return args[0] + 1;
        }
    }
}
