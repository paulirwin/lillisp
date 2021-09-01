using System;
using System.Collections;
using System.Linq;
using Lillisp.Core.Syntax;

namespace Lillisp.Core.Expressions
{
    public static class DynamicExpressions
    {
        public static object? Count(object?[] args)
        {
            if (args.Length != 1)
            {
                throw new ArgumentException("count/length requires one argument");
            }

            return args[0] switch
            {
                null => 0,
                Nil => 0,
                List list => list.Children.Count,
                string str => str.Length,
                ICollection coll => coll.Count,
                IEnumerable enumerable => enumerable.Cast<object>().Count(),
                _ => throw new ArgumentException($"Not sure how to get the count of that")
            };
        }
    }
}
