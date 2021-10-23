using System;
using System.IO;

namespace Lillisp.Core.Expressions
{
    public static class PortExpressions
    {
        public static object? OpenOutputString(object?[] args)
        {
            return new StringWriter();
        }

        public static object? GetOutputString(object?[] args)
        {
            if (args.Length != 1 || args[0] is not StringWriter sw)
            {
                throw new ArgumentException("get-output-string requires one argument that must be created with open-output-string");
            }

            return sw.ToString();
        }
    }
}
