using System;
using System.Collections.Generic;
using System.Linq;

namespace Lillisp.Core.Expressions
{
    public static class ListExpressions
    {
        public static object? Car(object?[] args)
        {
            if (args.Length == 0 || args[0] is not IList<object> objects)
            {
                throw new ArgumentException("car requires an argument");
            }

            if (objects.Count == 0)
            {
                throw new ArgumentException("Attempt to apply car on nil");
            }

            return objects[0];
        }

        public static object? Cdr(object?[] args)
        {
            if (args.Length == 0 || args[0] is not IList<object> objects)
            {
                throw new ArgumentException("cdr requires a list argument");
            }

            if (objects.Count == 0)
            {
                throw new ArgumentException("Attempt to apply cdr on nil");
            }

            return objects.Skip(1).ToArray();
        }

        public static object? Cons(object?[] args)
        {
            if (args.Length != 2 || args[1] is not IEnumerable<object> objects)
            {
                throw new ArgumentException("cons requires two arguments");
            }

            var first = args[0];

            return new[] { first }.Concat(objects).ToArray();
        }

        public static object? Append(object?[] args)
        {
            if (args.Length == 0)
            {
                return Nil.Value;
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

        public static object? Range(object?[] args)
        {
            if (args.Length > 3)
            {
                throw new ArgumentException("range takes at most 3 arguments");
            }

            // TODO: support IEnumerable ranges
            if (args.Length == 0)
            {
                return Array.Empty<object>();
            }

            double start = 0;
            double step = 1;
            double end = 0;

            if (args.Length == 1)
            {
                end = Convert.ToDouble(args[0]);
            }
            else if (args.Length >= 2)
            {
                start = Convert.ToDouble(args[0]);
                end = Convert.ToDouble(args[1]);
            }
            
            if (args.Length == 3)
            {
                step = Convert.ToDouble(args[2]);
            }

            return RangeGenerator(start, end, step).ToArray();
        }

        private static IEnumerable<object> RangeGenerator(double start, double end, double step)
        {
            for (double i = start; i < end; i += step)
            {
                yield return i;
            }
        }

        public static object? List(object?[] args)
        {
            if (args.Length == 0)
            {
                return Nil.Value;
            }

            return args.ToArray();
        }
    }
}