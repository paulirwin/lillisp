using System;
using System.Linq;

namespace Lillisp.Core
{
    public static class ReplOutputFormatter
    {
        public static string? Format(object? result)
        {
            if (result == null)
            {
                return "null";
            }

            if (result is object[] objArray)
            {
                return $"({string.Join(" ", objArray.Select(Format))})";
            }

            if (result is Delegate expr)
            {
                return expr.Method.ToString();
            }

            return result.ToString();
        }
    }
}
