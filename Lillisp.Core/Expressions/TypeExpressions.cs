using System;

namespace Lillisp.Core.Expressions
{
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

            return args[0].GetType();
        }

        public static object? Cast(object?[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("cast requires two arguments");
            }

            if (args[0] is null)
            {
                return null;
            }

            if (args[1] is not Type type)
            {
                throw new ArgumentException("Second parameter must be a Type");
            }

            return Convert.ChangeType(args[0], type);
        }
    }
}