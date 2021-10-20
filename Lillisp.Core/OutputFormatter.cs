using System;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;

namespace Lillisp.Core
{
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
                Vector vector => $"[{string.Join(" ", vector.Select(i => Format(i, quote, nullAsString)))}]",
                ICollection objArray => $"({string.Join(" ", objArray.Cast<object>().Select(i => Format(i, quote, nullAsString)))})",
                Delegate expr => expr.Method.ToString(),
                string str => quote ? SymbolDisplay.FormatLiteral(str, true) : str,
                StringBuilder sb => quote ? SymbolDisplay.FormatLiteral(sb.ToString(), true) : sb.ToString(),
                char ch => quote ? SymbolDisplay.FormatLiteral(ch, true) : ch.ToString(),
                Complex { Imaginary: 0d } complex => complex.Real.ToString(CultureInfo.InvariantCulture),
                Complex { Imaginary: < 0d } complex => $"{complex.Real.ToString(CultureInfo.InvariantCulture)}{complex.Imaginary.ToString(CultureInfo.InvariantCulture)}i",
                Complex { Imaginary: > 0d } complex => $"{complex.Real.ToString(CultureInfo.InvariantCulture)}+{complex.Imaginary.ToString(CultureInfo.InvariantCulture)}i",
                _ => result.ToString()
            };
        }
    }
}
