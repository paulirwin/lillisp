using System.Collections;
using System.Globalization;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis.CSharp;

namespace Lillisp.Core;

public static class OutputFormatter
{
    public static string? FormatPr(object? result) => Format(result, true, true);

    public static string? FormatRepl(object? result) => Format(result, true, true);

    public static string? FormatPrint(object? result) => Format(result, false, false);

    public static string? Format(object? result, bool quote, bool nullAsString)
    {
        return result switch
        {
            null => nullAsString ? "null" : null,
            Values values => string.Join(Environment.NewLine, values.Select(i => Format(i, quote, nullAsString))),
            Vector vector => $"[{string.Join(" ", vector.Select(i => Format(i, quote, nullAsString)))}]",
            Pair pair => pair.ToString(i => Format(i, quote, nullAsString)),
            ICollection objArray => $"({string.Join(" ", objArray.Cast<object>().Select(i => Format(i, quote, nullAsString)))})",
            Delegate expr => expr.Method.ToString(),
            string str => quote ? SymbolDisplay.FormatLiteral(str, true) : str,
            StringBuilder sb => quote ? SymbolDisplay.FormatLiteral(sb.ToString(), true) : sb.ToString(),
            char ch => quote ? SymbolDisplay.FormatLiteral(ch, true) : ch.ToString(),
            Complex complex => FormatComplex(complex),
            Regex regex => new RegexLiteral(regex).ToString(),
            double.PositiveInfinity or float.PositiveInfinity => "+inf.0",
            double.NegativeInfinity or float.NegativeInfinity => "-inf.0",
            double.NaN or float.NaN => "+nan.0",
            true => "#t",
            false => "#f",
            _ => result.ToString()
        };
    }

    private static string FormatComplex(Complex complex)
    {
        string real = complex.Real switch
        {
            double.PositiveInfinity => "+inf.0",
            double.NegativeInfinity => "-inf.0",
            double.NaN => "+nan.0",
            _ => complex.Real.ToString(CultureInfo.InvariantCulture)
        };

        string imaginary = complex.Imaginary switch
        {
            double.PositiveInfinity => "+inf.0i",
            double.NegativeInfinity => "-inf.0i",
            double.NaN => "+nan.0i",
            >= 0 => $"+{complex.Imaginary.ToString(CultureInfo.InvariantCulture)}i",
            _ => $"{complex.Imaginary.ToString(CultureInfo.InvariantCulture)}i"
        };

        return real + imaginary;
    }
}