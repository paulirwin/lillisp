using System;
using System.Numerics;
using Rationals;

namespace Lillisp.Core.Expressions
{
    public static class RationalExpressions
    {
        public static object? Rationalize(object?[] args)
        {
            if (args.Length != 2)
            {
                throw new InvalidOperationException("rationalize requires two arguments");
            }

            if (args[0] is null)
            {
                return null;
            }

            if (args[0] is Rational r)
            {
                return r;
            }

            if (args[0].IsInteger())
            {
                return new Rational((long)args[0]!);
            }

            var value = Convert.ToDouble(args[0]);

            var y = Convert.ToDouble(args[1]);

            return Rational.Approximate(value, y);
        }

        public static object? Numerator(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("numerator requires one argument");
            }

            return args[0] switch
            {
                null => null,
                Rational r => r.Numerator,
                BigInteger or ulong or long or uint or int or ushort or short or byte or sbyte => args[0],
                _ => $"Getting the numerator of a {args[0]!.GetType()} is not supported. For inexact numbers, try rationalizing it first."
            };
        }

        public static object? Denominator(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("denominator requires one argument");
            }

            return args[0] switch
            {
                null => null,
                Rational r => r.Denominator,
                BigInteger or ulong or long or uint or int or ushort or short or byte or sbyte => 1,
                _ => $"Getting the denominator of a {args[0]!.GetType()} is not supported. For inexact numbers, try rationalizing it first."
            };
        }

        public static object? Simplify(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("simplify requires one argument");
            }

            return args[0] switch
            {
                null => null,
                Rational r => SimplifyRational(r),
                BigInteger or ulong or long or uint or int or ushort or short or byte or sbyte => args[0],
                _ => $"Simplifying a {args[0]!.GetType()} is not supported. For inexact numbers, try rationalizing it first."
            };
        }

        private static Rational SimplifyRational(Rational rational)
        {
            var gcd = BigInteger.GreatestCommonDivisor(rational.Numerator, rational.Denominator);

            return gcd == 1 ? rational : new Rational(rational.Numerator / gcd, rational.Denominator / gcd);
        }
    }
}
