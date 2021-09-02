using System;
using System.Linq;

namespace Lillisp.Core.Expressions
{
    public static class StringExpressions
    {
        public static object? Print(object?[] args)
        {
            string output = string.Join(' ', args.Select(OutputFormatter.FormatPrint));

            Console.Write(output);

            return Nil.Value;
        }

        public static object? PrintLn(object?[] args)
        {
            string output = string.Join(' ', args.Select(OutputFormatter.FormatPrint));

            Console.WriteLine(output);

            return Nil.Value;
        }

        public static object? Str(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("str needs exactly one argument");
            }

            return args[0]?.ToString() ?? "";
        }
    }
}
