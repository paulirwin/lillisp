﻿using Rationals;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace Lillisp.Core.Expressions;

public static class TypeExpressions
{
    public static object? TypeOf(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("typeof requires one argument");
        }
        
        if (args[0] is null)
        {
            return null;
        }

        if (args[0] is Type type)
        {
            return type;
        }

        return args[0]?.GetType();
    }

    public static object? IsBoolean(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("boolean? requires one argument");
        }

        return args[0] is bool;
    }

    public static object? IsChar(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("char? requires one argument");
        }

        return args[0] is char;
    }

    public static object? IsNull(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("null? requires one argument");
        }

        return args[0] is Nil;
    }

    public static object? IsNumber(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("number? requires one argument");
        }

        return args[0].IsNumber();
    }

    public static object? IsString(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("string? requires one argument");
        }

        return args[0] is string;
    }

    public static object? IsPair(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("pair? requires one argument");
        }

        return args[0] is not Nil && args[0] is Pair or object?[]; // HACK: eventually remove "or object?[]"
    }

    public static object? IsProcedure(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("procedure? requires one argument");
        }

        return args[0] is Delegate or Procedure; // TODO: this is most certainly not correct or exhaustive
    }

    public static object? IsSymbol(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("symbol? requires one argument");
        }
            
        return args[0] is Symbol;
    }

    public static object? IsVector(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("vector? requires one argument");
        }

        return args[0] is Vector;
    }

    public static object? CharacterToInteger(object?[] args)
    {
        if (args.Length == 0 || args[0] is not char c)
        {
            throw new ArgumentException("char->integer requires one char argument");
        }

        return (int)c;
    }

    public static object? IntegerToCharacter(object?[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("integer->char requires one integer argument");
        }

        int value = Convert.ToInt32(args[0]);

        return (char)value;
    }

    public static object? StringToList(object?[] args)
    {
        if (args.Length is 0 or > 3)
        {
            throw new ArgumentException("string->list requires one to three arguments");
        }

        if (args[0] is not string str)
        {
            if (args[0] is StringBuilder sb)
            {
                str = sb.ToString();
            }
            else
            {
                throw new ArgumentException("string->list's first argument must be a string");
            }
        }

        int start = 0, end = str.Length;

        if (args.Length > 1)
        {
            start = Convert.ToInt32(args[1]);
        }

        if (args.Length == 3)
        {
            end = Convert.ToInt32(args[2]);
        }

        return str[start..end].Cast<object>().ToArray();
    }

    public static object? ListToString(object?[] args)
    {
        if (args.Length is 0 or > 1 || args[0] is not IEnumerable<object?> list)
        {
            throw new ArgumentException("list->string requires one list argument");
        }

        return new string(list.Cast<char>().ToArray());
    }

    public static object? IsPromise(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("promise? requires one argument");
        }

        return args[0] is Lazy<object?> or Task<object?>;
    }

    public static object? IsComplex(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("complex? requires one argument");
        }

        return args[0].IsComplex();
    }

    public static object? IsReal(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("real? requires one argument");
        }

        return args[0].IsRealNumber();
    }

    public static object? IsRational(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("rational? requires one argument");
        }

        return args[0].IsRationalNumber();
    }

    public static object? IsInteger(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("number? requires one argument");
        }

        return args[0].IsInteger();
    }

    public static object? IsBytevector(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("bytevector? requires one argument");
        }

        return args[0] is Bytevector; // TODO: should we support byte[] here too?
    }

    public static object? Utf8ToString(object?[] args)
    {
        if (args.Length is 0 or > 3)
        {
            throw new ArgumentException("utf8->string requires one to three arguments");
        }

        if (args[0] is not Bytevector bv)
        {
            throw new ArgumentException("utf8->string's first argument must be a bytevector");
        }

        int start = 0, end = bv.Count;

        if (args.Length > 1)
        {
            start = Convert.ToInt32(args[1]);
        }

        if (args.Length == 3)
        {
            end = Convert.ToInt32(args[2]);
        }

        var slice = bv[start..end];

        // TODO: reduce extra allocations here
        return Encoding.UTF8.GetString(slice.ToByteArray());
    }

    public static object? IsFinite(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("finite? requires one argument");
        }

        if (args[0].IsInteger() || args[0] is decimal or Rational)
        {
            return true;
        }

        return args[0] switch
        {
            Complex complex => complex.Real.IsFinite() && complex.Imaginary.IsFinite(),
            float f => f.IsFinite(),
            double d => d.IsFinite(),
            _ => throw new InvalidOperationException("Not sure how to determine if this is finite")
        };
    }

    public static object? IsExactInteger(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("exact? requires one argument");
        }

        return args[0] is int or uint or long or ulong or short or ushort or byte or sbyte or BigInteger;
    }

    public static object? IsInfinite(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("infinite? requires one argument");
        }

        if (args[0].IsInteger() || args[0] is decimal or Rational)
        {
            return false;
        }

        return args[0] switch
        {
            Complex complex => complex.Real.IsInfinite() || complex.Imaginary.IsInfinite(),
            float f => f.IsInfinite(),
            double d => d.IsInfinite(),
            _ => throw new InvalidOperationException("Not sure how to determine if this is infinite")
        };
    }

    internal static object? IsNaN(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("nan? requires one argument");
        }

        return args[0] switch
        {
            Complex c => double.IsNaN(c.Real) || double.IsNaN(c.Imaginary),
            double d => double.IsNaN(d),
            float f => float.IsNaN(f),
            _ => false
        };
    }

    public static object? IsExact(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("exact? requires one argument");
        }

        return args[0] is int or uint or long or ulong or short or ushort or byte or sbyte or decimal or BigInteger or Rational;
    }

    public static object? IsInexact(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("inexact? requires one argument");
        }

        return args[0] is float or double or Complex;
    }

    public static object? StringToUtf8(object?[] args)
    {
        if (args.Length is 0 or > 3)
        {
            throw new ArgumentException("string->utf8 requires one to three arguments");
        }

        if (args[0] is not string str)
        {
            if (args[0] is StringBuilder sb)
            {
                str = sb.ToString();
            }
            else
            {
                throw new ArgumentException("string->utf8's first argument must be a string");
            }
        }

        int start = 0, end = str.Length;

        if (args.Length > 1)
        {
            start = Convert.ToInt32(args[1]);
        }

        if (args.Length == 3)
        {
            end = Convert.ToInt32(args[2]);
        }

        return new Bytevector(Encoding.UTF8.GetBytes(str[start..end]));
    }

    public static object? IsPort(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("port? requires one argument");
        }

        return args[0] is Stream or TextReader or TextWriter;
    }

    public static object? StringToSymbol(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("string->symbol requires one string argument");
        }

        return args[0] switch
        {
            string s => new Symbol(s),
            StringBuilder sb => new Symbol(sb.ToString()),
            _ => throw new ArgumentException("string->symbol requires one string argument")
        };
    }

    public static object? SymbolToString(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("symbol->string requires one symbol argument");
        }

        return args[0] switch
        {
            Symbol s => s.ToString(),
            _ => throw new ArgumentException("symbol->string requires one symbol argument")
        };
    }

    public static object? NumberToString(object?[] args)
    {
        if (args.Length is < 1 or > 2)
        {
            throw new ArgumentException("number->string requires one or two arguments");
        }

        int radix = 10;

        if (args.Length == 2)
        {
            radix = Convert.ToInt32(args[1]);
        }

        if (radix != 2 && radix != 8 && radix != 10 && radix != 16)
        {
            throw new ArgumentException("Invalid radix. Must be 2, 8, 10, or 16.");
        }
            
        if (radix != 10 && args[0] is Rational or BigInteger or Complex or double or float or decimal or ulong)
        {
            throw new ArgumentException("Cannot use a radix other than 10 with the provided argument.");
        }

        return args[0] switch
        {
            Rational r => r.ToString(),
            BigInteger bi => bi.ToString(), 
            Complex c => c.ToString(),
            double.NaN or float.NaN => "+nan.0",
            double.PositiveInfinity or float.PositiveInfinity => "+inf.0",
            double.NegativeInfinity or float.NegativeInfinity => "-inf.0",
            double d => d.ToString(CultureInfo.InvariantCulture),
            float f => f.ToString(CultureInfo.InvariantCulture),
            decimal d => d.ToString(CultureInfo.InvariantCulture),
            ulong ul => ul.ToString(),
            long l => Convert.ToString(l, radix),
            uint ui => Convert.ToString(ui, radix),
            int i => Convert.ToString(i, radix),
            ushort us => Convert.ToString(us, radix),
            short s => Convert.ToString(s, radix),
            byte b => Convert.ToString(b, radix),
            sbyte sb => Convert.ToString(sb, radix),
            _ => throw new ArgumentException("Argument is not a number")
        };
    }

    public static object? StringToNumber(object?[] args)
    {
        if (args.Length is < 1 or > 2)
        {
            throw new ArgumentException("string->number requires one or two arguments");
        }

        int radix = 10;

        if (args.Length == 2)
        {
            radix = Convert.ToInt32(args[1]);
        }

        if (radix != 2 && radix != 8 && radix != 10 && radix != 16)
        {
            throw new ArgumentException("Invalid radix. Must be 2, 8, 10, or 16.");
        }

        string s = args[0] switch
        {
            string s2 => s2,
            StringBuilder sb => sb.ToString(),
            _ => throw new ArgumentException("string->number's first argument must be a string"),
        };

        if (string.IsNullOrEmpty(s))
        {
            return false;
        }

        try
        {
            if (radix == 10)
            {
                // HACK: make this parsing more specific to just numbers
                if (LillispRuntime.ParseProgramText(s) is not Program prog)
                {
                    return false;
                }

                if (prog.Children.Count != 1 || prog.Children[0] is not Atom { AtomType: AtomType.Number } atom)
                {
                    return false;
                }

                return atom.Value;
            }
            else
            {
                return Convert.ToInt32(s, radix);
            }
        }
        catch
        {
            return false;
        }
    }

    public static object? ConvertToExact(object?[] args)
    {
        if (args.Length != 1 || args[0] == null)
        {
            throw new ArgumentException("exact requires exactly one argument");
        }

        return args[0] switch
        {
            Complex => throw new InvalidOperationException("Exact complex types are not yet supported"),
            Rational or BigInteger or ulong or long or uint or int or ushort or short or sbyte or byte or decimal => args[0],
            float f => (decimal)f,
            double d => (decimal)d,
            _ => throw new InvalidOperationException("Cannot convert the provided value to an exact number"),
        };
    }

    public static object? ConvertToInexact(object?[] args)
    {
        if (args.Length != 1 || args[0] == null)
        {
            throw new ArgumentException("inexact requires exactly one argument");
        }

        return args[0] switch
        {
            Complex or float or double => args[0],
            Rational r => (double)r,
            BigInteger bi => (double)bi,
            decimal or ulong or long or uint or int or ushort or short or sbyte or byte => Convert.ToDouble(args[0]),
            _ => throw new InvalidOperationException("Cannot convert the provided value to an inexact number"),
        };
    }

    public static object? IsList(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("list? requires one argument");
        }

        return Nil.Value.Equals(args[0]) || args[0] is Pair { IsList: true };
    }
}