using System;
using System.Collections;
using System.Linq;

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
                Pair pair => pair.Count(),
                string str => str.Length,
                ICollection coll => coll.Count,
                IEnumerable enumerable => enumerable.Cast<object>().Count(),
                _ => throw new ArgumentException($"Not sure how to get the count of that")
            };
        }

        public static object? Get(object?[] args)
        {
            if (args.Length != 2)
            {
                throw new ArgumentException("get requires two arguments");
            }

            var index = Convert.ToInt32(args[1]);

            return args[0] switch
            {
                null => Nil.Value,
                Nil => Nil.Value,
                Pair pair => pair.ElementAt(index),
                string str => str[index],
                IList coll => coll[index],
                IEnumerable enumerable => enumerable.Cast<object>().ElementAt(index),
                _ => throw new ArgumentException($"Not sure how to get the index of that")
            };
        }
    }
}
