﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Lillisp.Core.Expressions;

public static class ListExpressions
{
    public static object? Car(object?[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("car requires an argument");
        }

        if (args[0] is Nil)
        {
            throw new ArgumentException("Attempt to apply car on nil");
        }

        if (args[0] is not Pair pair)
        {
            throw new ArgumentException("car requires a pair argument");
        }

        return pair.Car;
    }

    public static object? Cdr(object?[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("cdr requires a list argument");
        }

        if (args[0] is Nil)
        {
            throw new ArgumentException("Attempt to apply cdr on nil");
        }

        if (args[0] is not Pair pair)
        {
            throw new ArgumentException("cdr requires a pair argument");
        }

        return pair.Cdr;
    }

    public static object? Cons(object?[] args)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("cons requires two arguments");
        }

        var first = args[0];
            
        return args[1] is IEnumerable<object> objects ? Core.List.FromNodes(new[] { first }.Concat(objects).ToArray()) : new Pair(first, args[1]);
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

        return Core.List.FromNodes(result.ToArray());
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

        return Core.List.FromNodes(RangeGenerator(start, end, step).ToArray());
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

        return Core.List.FromNodes(args.ToArray());
    }

    public static object? MakeList(object?[] args)
    {
        if (args.Length is 0 or > 2)
        {
            throw new ArgumentException("make-list requires at least one but no more than two arguments");
        }

        int count = Convert.ToInt32(args[0]);
        object? defaultValue = null;

        if (args.Length == 2)
        {
            defaultValue = args[1];
        }

        return Core.List.FromNodes(Enumerable.Repeat(defaultValue, count));
    }
}