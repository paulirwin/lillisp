using System;
using System.Collections;
using System.Linq;

namespace Lillisp.Core
{
    public static class OutputFormatter
    {
        public static string? FormatRepl(object? result) => Format(result, true);

        public static string? FormatPrint(object? result) => Format(result, false);

        public static string? Format(object? result, bool quote)
        {
            if (result == null)
            {
                return "null";
            }

            if (result is ICollection objArray)
            {
                return $"({string.Join(" ", objArray.Cast<object>().Select(i => Format(i, quote)))})";
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
