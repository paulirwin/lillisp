using System;
using Lillisp.Core.Syntax;

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

            return args[0] is null;
        }

        public static object? IsNumber(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("number? requires one argument");
            }

            return args[0] is sbyte or byte or short or ushort or int or uint or long or ulong or float or double or decimal;
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

            return args[0] is Quote { Value: Pair };
        }

        public static object? IsProcedure(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("procedure? requires one argument");
            }

            return args[0] is Delegate; // TODO: this is most certainly not correct or exhaustive
        }

        public static object? IsSymbol(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("symbol? requires one argument");
            }

            // TODO: this does not currently work correctly
            return args[0] is Quote { Value: Atom { AtomType:AtomType.Symbol}};
        }
    }
}