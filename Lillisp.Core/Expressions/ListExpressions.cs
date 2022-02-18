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

        dynamic start = 0;
        dynamic step = 1;
        dynamic end = 0;

        if (args.Length == 1)
        {
            end = args[0] ?? throw new ArgumentException("end argument must not be null");
        }
        else if (args.Length >= 2)
        {
            start = args[0] ?? throw new ArgumentException("start argument must not be null");
            end = args[1] ?? throw new ArgumentException("end argument must not be null");
        }
            
        if (args.Length == 3)
        {
            step = args[2] ?? throw new ArgumentException("step argument must not be null");
        }

        var items = new List<dynamic>();

        for (var i = start; i < end; i += step)
        {
            items.Add(i);
        }

        return Core.List.FromNodes(items);
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

    public static object? Reverse(object?[] args)
    {
        if (args.Length != 1 || args[0] is not Pair { IsList: true } listPair)
        {
            throw new ArgumentException("reverse requires one list argument");
        }

        return Core.List.FromNodes(listPair.Reverse());
    }

    public static object? SetCar(object?[] args)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("set-car! requires two arguments");
        }

        if (args[0] is not Pair pair)
        {
            throw new ArgumentException("set-car!'s first argument must be a pair or list");
        }

        pair.Car = args[1];

        return Nil.Value;
    }

    public static object? SetCdr(object?[] args)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("set-cdr! requires two arguments");
        }

        if (args[0] is not Pair pair)
        {
            throw new ArgumentException("set-cdr!'s first argument must be a pair or list");
        }

        pair.Cdr = args[1];

        return Nil.Value;
    }

    public static object? ListSet(object?[] args)
    {
        if (args.Length != 3)
        {
            throw new ArgumentException("list-set! requires three arguments");
        }

        if (args[0] is not Pair { IsList: true } list)
        {
            throw new ArgumentException("list-set!'s first argument must be a list");
        }

        int k = Convert.ToInt32(args[1]);
        object? obj = args[2];

        object? next = list;
        int i = 0;

        while (next is Pair p)
        {
            if (i++ == k)
            {
                p.Car = obj;
                return Nil.Value;
            }

            next = p.Cdr;
        }

        throw new InvalidOperationException("Invalid list element or end of list encountered");
    }
}