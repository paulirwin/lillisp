using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Rationals;

// ReSharper disable IntVariableOverflowInUncheckedContext - allow for runtime exceptions in these cases

namespace Lillisp.Core.Expressions
{
    public static class MathExpressions
    {
        public static dynamic? Add(dynamic?[] args)
        {
            if (args.Length == 0)
            {
                return 0;
            }

            dynamic sum = args[0] ?? throw new NullReferenceException("Cannot apply + to null");

            if (args.Length == 1)
            {
                return args[0];
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
            if (args.Length == 0)
            {
                return 1;
            }

            dynamic result = 1;

            for (int i = 0; i < args.Length; i++)
            {
                result *= args[i];
            }

            return result;
        }

        public static object? Divide(object?[] args)
        {
            if (args.Length < 1 || args[0] == null)
            {
                throw new ArgumentException("/ requires at least one argument");
            }

            if (args.Length == 1)
            {
                return args[0] switch
                {
                    Rational r => (1 / r).CanonicalForm,
                    Complex c => 1 / c,
                    BigInteger or ulong or long or uint or int or ushort or short or sbyte or byte => new Rational(1, args[0]!.ToBigInteger()).CanonicalForm,
                    decimal d => 1m / d,
                    _ => 1.0 / (dynamic)args[0]!
                };
            }

            if (args.All(a => a.IsRationalNumber()))
            {
                return args.Select(a => a.ToRational()).Aggregate((result, next) => result / next).CanonicalForm;
            }

            return args.Cast<dynamic>().Aggregate((result, next) => result / next);
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
                    (Rational a, _) => ObjectExtensions.IsInteger(args[i]) ? Rational.Pow(a, Convert.ToInt32(args[i])) : throw new NotImplementedException("Raising a rational to a non-integer is not yet supported"),
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
                Rational r => Rational.Abs(r),
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
                double d => Math.Sqrt(d),
                Complex complex => Complex.Sqrt(complex),
                Rational r => RationalSqrt(r),
                _ => Math.Sqrt(Convert.ToDouble(args[0]))
            };
        }

        private static Rational RationalSqrt(Rational rational)
        {
            if (rational.Numerator > long.MaxValue
                || rational.Numerator < long.MinValue
                || rational.Denominator > long.MaxValue
                || rational.Denominator < long.MinValue)
            {
                throw new ArgumentOutOfRangeException(nameof(rational), "Rational value has either a numerator or denominator that is too large to sqrt.");
            }

            var num = Math.Sqrt((double)rational.Numerator);
            var den = Math.Sqrt((double)rational.Denominator);
            
            var result = num / den;

            return Rational.Approximate(result);
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

        public static object? Floor(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("floor requires one argument");
            }

            return args[0] switch
            {
                BigInteger or ulong or long or uint or int or ushort or short or sbyte or byte => args[0],
                double d => Math.Floor(d),
                decimal d => Math.Floor(d),
                Rational r => (int)Math.Floor((decimal)r),
                _ => Math.Floor(Convert.ToDouble(args[0]))
            };
        }

        public static object? Ceiling(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("ceiling requires one argument");
            }

            return args[0] switch
            {
                BigInteger or ulong or long or uint or int or ushort or short or sbyte or byte => args[0],
                double d => Math.Ceiling(d),
                decimal d => Math.Ceiling(d),
                Rational r => (int)Math.Ceiling((decimal)r),
                _ => Math.Ceiling(Convert.ToDouble(args[0]))
            };
        }

        public static object? Truncate(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("truncate requires one argument");
            }

            return args[0] switch
            {
                BigInteger or ulong or long or uint or int or ushort or short or sbyte or byte => args[0],
                double d => Math.Truncate(d),
                decimal d => Math.Truncate(d),
                Rational r => (int)Math.Truncate((decimal)r),
                _ => Math.Truncate(Convert.ToDouble(args[0]))
            };
        }

        public static object? Round(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("round requires one argument");
            }

            return args[0] switch
            {
                BigInteger or ulong or long or uint or int or ushort or short or sbyte or byte => args[0],
                double d => Math.Round(d),
                decimal d => Math.Round(d),
                Rational r => (int)Math.Round((decimal)r),
                _ => Math.Round(Convert.ToDouble(args[0]))
            };
        }

        public static object? FloorDivide(object?[] args)
        {
            if (args.Length != 2 || args[0] is null)
            {
                throw new ArgumentException("floor/ requires two arguments");
            }

            if (args[1] is null or 0)
            {
                throw new ArgumentException("floor/'s second argument must not be null/zero");
            }

            return IntegerDivide(args, Math.Floor, Math.Floor);
        }

        public static object? Gcd(object?[] args)
        {
            if (args.Length == 0)
            {
                return 0;
            }

            static object? GcdAggregate(object? n1, object? n2) => (n1, n2) switch
            {
                (null, _) or (_, null) => null,
                (BigInteger, _) or (_, BigInteger) => BigInteger.GreatestCommonDivisor((BigInteger)n1, (BigInteger)n2),
                (long, _) or (_, long) => GcdInternal((long)n1, (long)n2),
                (ulong, _) or (_, ulong) => GcdInternal((ulong)n1, (ulong)n2),
                (uint, _) or (_, uint) => GcdInternal((uint)n1, (uint)n2),
                _ => GcdInternal((int)n1, (int)n2)
            };

            return args.Aggregate(GcdAggregate);
        }

        private static long GcdInternal(long x, long y) => y == 0L ? Math.Abs(x) : GcdInternal(y, x % y);
        private static ulong GcdInternal(ulong x, ulong y) => y == 0UL ? x : GcdInternal(y, x % y);
        private static int GcdInternal(int x, int y) => y == 0 ? Math.Abs(x) : GcdInternal(y, x % y);
        private static uint GcdInternal(uint x, uint y) => y == 0U ? x : GcdInternal(y, x % y);
        private static double GcdInternal(double x, double y) => y == 0.0 ? Math.Abs(x) : GcdInternal(y, x % y);
        private static float GcdInternal(float x, float y) => y == 0.0f ? Math.Abs(x) : GcdInternal(y, x % y);
        private static decimal GcdInternal(decimal x, decimal y) => y == 0.0m ? Math.Abs(x) : GcdInternal(y, x % y);

        private static readonly IList<Type> LcmTypePrecedence = new[]
        {
            typeof(double),
            typeof(float),
            typeof(decimal),
            typeof(BigInteger),
            typeof(ulong),
            typeof(long),
            typeof(uint),
            typeof(int),
        };

        private static readonly int DefaultLcmTypePrecedence = LcmTypePrecedence.IndexOf(typeof(int));

        public static object? Lcm(object?[] args)
        {
            if (args.Length == 0)
            {
                return 1;
            }

            var resultType = typeof(int);
            var resultTypePrecedence = DefaultLcmTypePrecedence;

            foreach (var arg in args)
            {
                if (arg == null)
                {
                    return null;
                }

                var precedence = LcmTypePrecedence.IndexOf(arg.GetType());

                if (precedence >= 0 && precedence < resultTypePrecedence)
                {
                    resultType = arg.GetType();
                    resultTypePrecedence = precedence;
                }
            }

            static object? LcmAggregate(object? n1, object? n2) => (n1, n2) switch
            {
                (null, _) or (_, null) => null,
                (double a, double b) => Math.Abs(a / GcdInternal(a, b) * b),
                (float a, float b) => Math.Abs(a / GcdInternal(a, b) * b),
                (decimal a, decimal b) => Math.Abs(a / GcdInternal(a, b) * b),
                (BigInteger a, BigInteger b) => a / BigInteger.GreatestCommonDivisor(a, b) * b,
                (ulong a, ulong b) => a / GcdInternal(a, b) * b,
                (long a, long b) => Math.Abs(a / GcdInternal(a, b) * b),
                (uint a, uint b) => a / GcdInternal(a, b) * b,
                (int a, int b) => Math.Abs(a / GcdInternal(a, b) * b),
                _ => throw new NotImplementedException()
            };

            return args.Select(i => Convert.ChangeType(i, resultType)).Aggregate(LcmAggregate);
        }

        public static object? FloorQuotient(object?[] args)
        {
            if (args.Length != 2 || args[0] is null)
            {
                throw new ArgumentException("floor-quotient requires two arguments");
            }

            if (args[1] is null or 0)
            {
                throw new ArgumentException("floor-quotient's second argument must not be null/zero");
            }

            var result = IntegerDivide(args, Math.Floor, Math.Floor);

            return result.Car;
        }

        public static object? FloorRemainder(object?[] args)
        {
            if (args.Length != 2 || args[0] is null)
            {
                throw new ArgumentException("floor-remainder requires two arguments");
            }

            if (args[1] is null or 0)
            {
                throw new ArgumentException("floor-remainder's second argument must not be null/zero");
            }

            var result = IntegerDivide(args, Math.Floor, Math.Floor);

            return result.Cdr;
        }

        public static object? TruncateDivide(object?[] args)
        {
            if (args.Length != 2 || args[0] is null)
            {
                throw new ArgumentException("truncate/ requires two arguments");
            }

            if (args[1] is null or 0)
            {
                throw new ArgumentException("truncate/'s second argument must not be null/zero");
            }

            return IntegerDivide(args, Math.Truncate, Math.Truncate);
        }

        private static Pair IntegerDivide(object?[] args, Func<decimal, decimal> decimalFunc, Func<double, double> doubleFunc)
        {
            if (args[0] is decimal d1 && args[1] is decimal d2)
            {
                var dResult = d1 / d2;
                var dQuotient = decimalFunc(dResult);
                var dRemainder = d1 - (d2 * dQuotient);

                return new Pair(dQuotient, dRemainder);
            }

            var numerator = Convert.ToDouble(args[0]);
            var denominator = Convert.ToDouble(args[1]);

            var result = numerator / denominator;

            var quotient = doubleFunc(result);
            var remainder = numerator - (denominator * quotient);

            return (args[0], args[1]) switch
            {
                (double, _) or (_, double) => new Pair(quotient, remainder),
                (float, _) or (_, float) => Pair.CastPair<float>(quotient, remainder),
                (BigInteger, _) or (_, BigInteger) => Pair.CastPair<BigInteger>(quotient, remainder),
                (long, _) or (_, long) => Pair.CastPair<long>(quotient, remainder),
                (ulong, _) or (_, ulong) => Pair.CastPair<ulong>(quotient, remainder),
                (int, _) or (_, int) => Pair.CastPair<int>(quotient, remainder),
                (uint, _) or (_, uint) => Pair.CastPair<uint>(quotient, remainder),
                (short, _) or (_, short) => Pair.CastPair<short>(quotient, remainder),
                (ushort, _) or (_, ushort) => Pair.CastPair<ushort>(quotient, remainder),
                (sbyte, _) or (_, sbyte) => Pair.CastPair<sbyte>(quotient, remainder),
                (byte, _) or (_, byte) => Pair.CastPair<byte>(quotient, remainder),
                _ => Pair.CastPair<int>(quotient, remainder)
            };
        }

        public static object? TruncateQuotient(object?[] args)
        {
            if (args.Length != 2 || args[0] is null)
            {
                throw new ArgumentException("truncate-quotient requires two arguments");
            }

            if (args[1] is null or 0)
            {
                throw new ArgumentException("truncate-quotient's second argument must not be null/zero");
            }

            var result = IntegerDivide(args, Math.Truncate, Math.Truncate);

            return result.Car;
        }

        public static object? TruncateRemainder(object?[] args)
        {
            if (args.Length != 2 || args[0] is null)
            {
                throw new ArgumentException("truncate-remainder requires two arguments");
            }

            if (args[1] is null or 0)
            {
                throw new ArgumentException("truncate-remainder's second argument must not be null/zero");
            }

            var result = IntegerDivide(args, Math.Truncate, Math.Truncate);

            return result.Cdr;
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
                Rational r => Rational.Log(r),
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
                    Rational r => Rational.Log10(r),
                    _ => Math.Log10(Convert.ToDouble(args[0]))
                };
            }

            return (args[0], args[1]) switch
            {
                // HACK: without the cast to object, the type of the switch expression is Complex
                (double d, double b) => (object)Math.Log(d, b),
                (Complex complex, double b) => Complex.Log(complex, b),
                (Complex complex, _) => Complex.Log(complex, Convert.ToDouble(args[1])),
                (Rational r, double b) => Rational.Log(r, b),
                (Rational r, _) => Rational.Log(r, Convert.ToDouble(args[1])),
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

        public static object? ExactIntegerSqrt(object?[] args)
        {
            if (args.Length != 1 || args[0] == null || !args[0]!.IsInteger())
            {
                throw new ArgumentException("exact-integer-sqrt requires one integer argument");
            }

            if (args[0] is < 0)
            {
                throw new ArgumentException("exact-integer-sqrt requires a positive number");
            }

            if (args[0] is BigInteger)
            {
                throw new NotImplementedException("exact-integer-sqrt support for BigInteger is not yet implemented");
            }

            // Adapted from CC-SA code at https://en.wikipedia.org/wiki/Integer_square_root
            dynamic k = args[0]!;
            dynamic l = k - k; // this types l (as zero) to the same type as k
            dynamic r = k + 1;

            while (l != r - 1)
            {
                dynamic m = (l + r) / 2;

                if (m * m <= k)
                {
                    l = m;
                }
                else
                {
                    r = m;
                }
            }

            return new Values(l, k - (l * l));
        }
    }
}
