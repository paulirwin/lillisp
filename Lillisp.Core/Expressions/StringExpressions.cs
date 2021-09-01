using System;
using System.Linq;

namespace Lillisp.Core.Expressions
{
    public static class StringExpressions
    {
        public static object? Print(object?[] args)
        {
            string output = string.Join(' ', args.Select(ReplOutputFormatter.Format));

            Console.Write(output);

            return Nil.Value;
        }

        public static object? PrintLn(object?[] args)
        {
            string output = string.Join(' ', args.Select(ReplOutputFormatter.Format));

            Console.WriteLine(output);

            return Nil.Value;
        }
    }
}
