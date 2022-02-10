using System;
using System.Linq;

namespace Lillisp.Core.Expressions;

public static class VectorExpressions
{
    public static object? MakeVector(object?[] args)
    {
        if (args.Length is 0 or > 2)
        {
            throw new ArgumentException("make-vector requires at least one but no more than two arguments");
        }

        int count = Convert.ToInt32(args[0]);
        object? defaultValue = null;

        if (args.Length == 2)
        {
            defaultValue = args[1];
        }

        return new Vector(Enumerable.Repeat(defaultValue, count));
    }

    public static object? Vector(object?[] args)
    {
        return new Vector(args);
    }

    public static object? VectorLength(object?[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("vector-length requires one vector argument");
        }

        if (args[0] is not Vector vector)
        {
            throw new ArgumentException("Argument to vector-length must be a vector");
        }

        return vector.Count;
    }

    public static object? VectorRef(object?[] args)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("vector-ref requires two arguments");
        }

        if (args[0] is not Vector vector)
        {
            throw new ArgumentException("vector-ref's first argument must be a vector");
        }

        var k = Convert.ToInt32(args[1]);

        return vector[k];
    }

    public static object? VectorSet(object?[] args)
    {
        if (args.Length != 3)
        {
            throw new ArgumentException("vector-set requires three arguments");
        }

        if (args[0] is not Vector vector)
        {
            throw new ArgumentException("vector-set's first argument must be a vector");
        }

        var k = Convert.ToInt32(args[1]);
        var obj = args[2];

        vector[k] = obj;

        return obj; // TODO: is this correct?
    }

    public static object? VectorCopy(object?[] args)
    {
        if (args.Length is 0 or > 3)
        {
            throw new ArgumentException("vector-copy requires 1 to 3 arguments");
        }

        if (args[0] is not Vector vector)
        {
            throw new ArgumentException("vector-copy's first argument must be a vector");
        }

        int start = 0, end = vector.Count;

        if (args.Length > 1)
        {
            start = Convert.ToInt32(args[1]);
        }

        if (args.Length == 3)
        {
            end = Convert.ToInt32(args[2]);
        }

        return new Vector(vector.Skip(start).Take(end - start));
    }

    public static object? VectorCopyTo(object?[] args)
    {
        if (args.Length is < 3 or > 5)
        {
            throw new ArgumentException("vector-copy! requires 3 to 5 arguments");
        }

        if (args[0] is not Vector to)
        {
            throw new ArgumentException("vector-copy!'s first argument must be a vector");
        }

        if (args[2] is not Vector from)
        {
            throw new ArgumentException("vector-copy!'s third argument must be a vector");
        }

        var at = Convert.ToInt32(args[1]);
        int start = 0, end = from.Count;

        if (args.Length > 3)
        {
            start = Convert.ToInt32(args[3]);
        }

        if (args.Length == 5)
        {
            end = Convert.ToInt32(args[4]);
        }

        if ((to.Count - at) < (end - start))
        {
            throw new ArgumentException("(- (vector-length to) at) must not be less than (- end start)");
        }

        for (int i = start; i < end; i++)
        {
            to[at++] = from[i];
        }

        return Nil.Value; // TODO: is this correct?
    }

    public static object? Append(object?[] args)
    {
        if (args.Length == 0)
        {
            return new Vector();
        }

        return new Vector(args.Cast<Vector>().SelectMany(i => i));
    }

    public static object? VectorFill(object?[] args)
    {
        if (args.Length is < 2 or > 4)
        {
            throw new ArgumentException("vector-fill! requires 2 to 4 arguments");
        }

        if (args[0] is not Vector vector)
        {
            throw new ArgumentException("vector-fill!'s first argument must be a vector");
        }

        var fill = args[1];
        int start = 0, end = vector.Count;

        if (args.Length > 2)
        {
            start = Convert.ToInt32(args[2]);
        }

        if (args.Length == 4)
        {
            end = Convert.ToInt32(args[3]);
        }

        for (int i = start; i < end; i++)
        {
            vector[i] = fill;
        }

        return Nil.Value; // TODO: is this correct?
    }
}