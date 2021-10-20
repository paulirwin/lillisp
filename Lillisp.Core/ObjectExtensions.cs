using System;
using System.Numerics;

namespace Lillisp.Core
{
    internal static class ObjectExtensions
    {
        public static bool IsTruthy(this object? value) => value is not false;

        public static bool IsNumber(this object? value) => value is Complex 
            or BigInteger
            or decimal
            or double
            or float
            or long
            or ulong
            or int
            or uint
            or short
            or ushort
            or byte
            or sbyte;

        // HACK: per Scheme docs: "In many implementations the complex? procedure will
        // be the same as number?, but unusual implementations may rep-
        // resent some irrational numbers exactly or may extend the num-
        // ber system to support some kind of non-complex numbers."
        public static bool IsComplex(this object? value) => IsNumber(value);

        public static bool IsRealNumber(this object? value) => value is BigInteger
            or decimal
            or double
            or float
            or long
            or ulong
            or int
            or uint
            or short
            or ushort
            or byte
            or sbyte;

        public static bool IsRationalNumber(this object? value) => value is BigInteger
            // TODO: or Rational
            or long
            or ulong
            or int
            or uint
            or short
            or ushort
            or byte
            or sbyte;

        /// <summary>
        /// Returns true if the <paramref name="value"/> is an integer type or
        /// safely convertible to exact integer without losing data. 
        /// </summary>
        /// <param name="value">The value to consider.</param>
        /// <returns>Boolean</returns>
        public static bool IsInteger(this object? value) => value is BigInteger
            or long
            or ulong
            or int
            or uint
            or short
            or ushort
            or byte
            or sbyte
        || (value is decimal de && Math.Truncate(de) == de);

        public static bool IsUnsignedNumber(this object? value) => value is ulong or uint or ushort or byte;
    }
}