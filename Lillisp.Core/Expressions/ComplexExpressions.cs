using System;
using System.Numerics;

namespace Lillisp.Core.Expressions;

public static class ComplexExpressions
{
    public static object? Angle(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("angle requires one argument");
        }

        return args[0] switch
        {
            Complex complex => complex.Phase,
            BigInteger => BigInteger.Zero,
            decimal => decimal.Zero,
            double => 0d,
            float => 0f,
            long => 0L,
            ulong => 0UL,
            int => 0,
            uint => 0u,
            short => (short)0,
            (ushort) => (ushort)0,
            byte => (byte)0,
            sbyte => (sbyte)0,
            null => null,
            _ => throw new ArgumentException($"Unable to determine the angle of a {args[0]!.GetType()}")
        };
    }

    public static object? Magnitude(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("magnitude requires one argument");
        }

        if (args[0] == null || args[0].IsUnsignedNumber())
        {
            return args[0];
        }

        return args[0] switch
        {
            Complex complex => complex.Magnitude,
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

    public static object? RealPart(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("real-part requires one argument");
        }

        if (args[0] == null)
        {
            return null;
        }

        if (args[0] is Complex complex)
        {
            return complex.Real;
        }

        if (args[0].IsRealNumber())
        {
            return args[0];
        }

        throw new ArgumentException($"{args[0]!.GetType()} is not a number");
    }

    public static object? ImaginaryPart(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("imag-part requires one argument");
        }

        return args[0] switch
        {
            Complex complex => complex.Imaginary,
            BigInteger => BigInteger.Zero,
            decimal => decimal.Zero,
            double => 0d,
            float => 0f,
            long => 0L,
            ulong => 0UL,
            int => 0,
            uint => 0u,
            short => (short)0,
            (ushort) => (ushort)0,
            byte => (byte)0,
            sbyte => (sbyte)0,
            null => null,
            _ => throw new ArgumentException($"Unable to determine the imaginary part of a {args[0]!.GetType()}")
        };
    }

    public static object? MakeRectangular(object?[] args)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("make-rectangular requires two arguments");
        }

        var real = Convert.ToDouble(args[0]);
        var imaginary = Convert.ToDouble(args[1]);

        return new Complex(real, imaginary);
    }

    public static object? MakePolar(object?[] args)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("make-polar requires two arguments");
        }

        var magnitude = Convert.ToDouble(args[0]);
        var phase = Convert.ToDouble(args[1]);

        return Complex.FromPolarCoordinates(magnitude, phase);
    }
}