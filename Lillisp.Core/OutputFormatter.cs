using Lillisp.Core.Syntax;
using System;
using System.Collections;
using System.Linq;

namespace Lillisp.Core
{
    public static class OutputFormatter
    {
        public static string? FormatPr(object? result) => Format(result, true, true);

        public static string? FormatRepl(object? result) => Format(result, true, true);

        public static string? FormatPrint(object? result) => Format(result, false, false);

        public static string? Format(object? result, bool quote, bool nullAsString)
        {
            if (result == null)
            {
                return nullAsString ? "null" : null;
            }

            if (result is Vector vector)
            {
                return $"[{string.Join(" ", vector.Select(i => Format(i, quote, nullAsString)))}]";
            }

            if (result is ICollection objArray)
            {
                return $"({string.Join(" ", objArray.Cast<object>().Select(i => Format(i, quote, nullAsString)))})";
            }

            if (result is Delegate expr)
            {
                return expr.Method.ToString();
            }

            if (result is string str)
            {
                return quote ? $"\"{str}\"" : str;
            }

            if (result is char ch)
            {
                return quote ? $"\'{ch}\'" : ch.ToString();
            }

            return result.ToString();
        }
    }
}
