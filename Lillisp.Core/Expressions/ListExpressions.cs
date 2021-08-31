using System;
using System.Collections.Generic;
using System.Linq;
using Lillisp.Core.Syntax;

namespace Lillisp.Core.Expressions
{
    public static class ListExpressions
    {
        public static object? Car(object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("car requires an argument");
            }

            return args[0] switch
            {
                List list => list.Children[0],
                Atom { Value: List atomList } => atomList.Children[0],
                object[] objArr => objArr[0],
                _ => throw new ArgumentException("Argument to car must be a list")
            };
        }

        public static object? Cdr(object?[] args)
        {
            if (args.Length == 0)
            {
                throw new ArgumentException("cdr requires an argument");
            }

            return args[0] switch
            {
                List list => list.Children.Skip(1).Cast<object>().ToArray(),
                Atom { Value: List atomList } => atomList.Children.Skip(1).Cast<object>().ToArray(),
                object[] objArr => objArr.Skip(1).ToArray(),
                _ => throw new ArgumentException("Argument to cdr must be a list")
            };
        }

        public static object? Cons(object?[] args)
        {
            if (args.Length != 2 || args[1] is not object[] objArray)
            {
                throw new ArgumentException("cons requires two arguments");
            }

            var first = args[0];

            return new[] { first }.Concat(objArray).ToArray();
        }

        public static object? Append(object?[] args)
        {
            if (args.Length == 0)
            {
                return Array.Empty<object>();
            }

            var result = new List<object?>();

            for (int i = 0; i < args.Length; i++)
            {
                object? arg = args[i];

                if (arg is object[] objArray)
                {
                    result.AddRange(objArray);
                }
                else if (i == args.Length - 1)
                {
                    result.Add(arg);
                }
                else
                {
                    throw new ArgumentException($"{arg ?? "null"} is not of type list");
                }
            }

            return result.ToArray();
        }
    }
}