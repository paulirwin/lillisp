using System.Text;

namespace Lillisp.Core.Expressions;

public static class StringExpressions
{
    public static object? Print(object?[] args)
    {
        string output = string.Join(' ', args.Select(OutputFormatter.FormatPrint));

        Console.Write(output);

        return Nil.Value;
    }

    public static object? PrintLn(object?[] args)
    {
        string output = string.Join(' ', args.Select(OutputFormatter.FormatPrint));

        Console.WriteLine(output);

        return Nil.Value;
    }

    public static object? Str(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("str needs exactly one argument");
        }

        return OutputFormatter.FormatPrint(args[0]);
    }

    public static object? Prn(object?[] args)
    {
        string output = string.Join(' ', args.Select(OutputFormatter.FormatPr));

        Console.WriteLine(output);

        return Nil.Value;
    }

    public static object? Pr(object?[] args)
    {
        string output = string.Join(' ', args.Select(OutputFormatter.FormatPr));

        Console.Write(output);

        return Nil.Value;
    }

    public static object? MakeString(object?[] args)
    {
        if (args.Length is 0 or > 2)
        {
            throw new ArgumentException("make-string requires one or two arguments");
        }

        int k = Convert.ToInt32(args[0]);
        var sb = new StringBuilder(k);

        if (args.Length == 2)
        {
            if (args[1] is not char c)
            {
                throw new ArgumentException("make-string's second argument must be a character");
            }

            sb.Append(c, k);
        }
        else
        {
            sb.Append(' ', k);
        }

        return sb;
    }

    public static object? String(object?[] args)
    {
        var sb = new StringBuilder();

        foreach (var c in args.Cast<char>())
        {
            sb.Append(c);
        }

        return sb.ToString();
    }

    public static object? StringLength(object?[] args)
    {
        if (args.Length != 1)
        {
            throw new ArgumentException("string-length requires one argument");
        }

        return args[0] switch
        {
            string s => s.Length,
            StringBuilder sb => sb.Length,
            _ => throw new ArgumentException("string-length's argument must be a string")
        };
    }

    public static object? StringRef(object?[] args)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("string-ref requires two arguments");
        }

        var k = Convert.ToInt32(args[1]);

        return args[0] switch
        {
            string s => s[k],
            StringBuilder sb => sb[k],
            _ => throw new ArgumentException("string-ref's first argument must be a string")
        };
    }

    public static object? StringSet(object?[] args)
    {
        if (args.Length != 3)
        {
            throw new ArgumentException("string-set requires three arguments");
        }

        if (args[0] is not StringBuilder sb)
        {
            throw new ArgumentException("string-set's first argument must be a mutable string or StringBuilder");
        }

        if (args[2] is not char c)
        {
            throw new ArgumentException("string-set's third argument must be a character");
        }

        var k = Convert.ToInt32(args[1]);
            
        sb[k] = c;

        return Nil.Value;
    }

    private static IEnumerable<string> ArgsToStringlikes(this IEnumerable<object?> args)
    {
        foreach (var arg in args)
        {
            yield return arg switch
            {
                string s => s,
                StringBuilder sb => sb.ToString(),
                _ => throw new ArgumentException("Object argument is not a string")
            };
        }
    }

    public static object? Equals(object?[] args)
    {
        string? last = null;

        foreach (var arg in args.ArgsToStringlikes())
        {
            if (last != null && !arg.Equals(last, StringComparison.InvariantCulture))
                return false;

            last = arg;
        }

        return true;
    }

    public static object? LessThan(object?[] args)
    {
        string? last = null;

        foreach (var arg in args.ArgsToStringlikes())
        {
            if (last != null)
            {
                var comp = string.Compare(last, arg, StringComparison.InvariantCulture);

                if (comp >= 0)
                    return false;
            }

            last = arg;
        }

        return true;
    }

    public static object? GreaterThan(object?[] args)
    {
        string? last = null;

        foreach (var arg in args.ArgsToStringlikes())
        {
            if (last != null)
            {
                var comp = string.Compare(last, arg, StringComparison.InvariantCulture);

                if (comp <= 0)
                    return false;
            }

            last = arg;
        }

        return true;
    }

    public static object? LessThanOrEqualTo(object?[] args)
    {
        string? last = null;

        foreach (var arg in args.ArgsToStringlikes())
        {
            if (last != null)
            {
                var comp = string.Compare(last, arg, StringComparison.InvariantCulture);

                if (comp > 0)
                    return false;
            }

            last = arg;
        }

        return true;
    }

    public static object? GreaterThanOrEqualTo(object?[] args)
    {
        string? last = null;

        foreach (var arg in args.ArgsToStringlikes())
        {
            if (last != null)
            {
                var comp = string.Compare(last, arg, StringComparison.InvariantCulture);

                if (comp < 0)
                    return false;
            }

            last = arg;
        }

        return true;
    }

    public static object? CaseInsensitiveEquals(object?[] args)
    {
        string? last = null;

        foreach (var arg in args.ArgsToStringlikes())
        {
            if (last != null && !arg.Equals(last, StringComparison.InvariantCultureIgnoreCase))
                return false;

            last = arg;
        }

        return true;
    }

    public static object? CaseInsensitiveLessThan(object?[] args)
    {
        string? last = null;

        foreach (var arg in args.ArgsToStringlikes())
        {
            if (last != null)
            {
                var comp = string.Compare(last, arg, StringComparison.InvariantCultureIgnoreCase);

                if (comp >= 0)
                    return false;
            }

            last = arg;
        }

        return true;
    }

    public static object? CaseInsensitiveGreaterThan(object?[] args)
    {
        string? last = null;

        foreach (var arg in args.ArgsToStringlikes())
        {
            if (last != null)
            {
                var comp = string.Compare(last, arg, StringComparison.InvariantCultureIgnoreCase);

                if (comp <= 0)
                    return false;
            }

            last = arg;
        }

        return true;
    }

    public static object? CaseInsensitiveLessThanOrEqualTo(object?[] args)
    {
        string? last = null;

        foreach (var arg in args.ArgsToStringlikes())
        {
            if (last != null)
            {
                var comp = string.Compare(last, arg, StringComparison.InvariantCultureIgnoreCase);

                if (comp > 0)
                    return false;
            }

            last = arg;
        }

        return true;
    }

    public static object? CaseInsensitiveGreaterThanOrEqualTo(object?[] args)
    {
        string? last = null;

        foreach (var arg in args.ArgsToStringlikes())
        {
            if (last != null)
            {
                var comp = string.Compare(last, arg, StringComparison.InvariantCultureIgnoreCase);

                if (comp < 0)
                    return false;
            }

            last = arg;
        }

        return true;
    }

    public static object? Upcase(object?[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("string-upcase takes one argument");
        }

        return args[0] switch
        {
            string s => s.ToUpper(),
            StringBuilder sb => sb.ToString().ToUpper(),
            _ => throw new ArgumentException("string-upcase's first argument must be a string")
        };
    }

    public static object? Downcase(object?[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("string-downcase takes one argument");
        }

        return args[0] switch
        {
            string s => s.ToLower(),
            StringBuilder sb => sb.ToString().ToLower(),
            _ => throw new ArgumentException("string-downcase's first argument must be a string")
        };
    }

    public static object? Foldcase(object?[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("string-foldcase takes one argument");
        }

        return args[0] switch
        {
            string s => s.ToLowerInvariant(),
            StringBuilder sb => sb.ToString().ToLowerInvariant(),
            _ => throw new ArgumentException("string-foldcase's first argument must be a string")
        };
    }

    public static object? Substring(object?[] args)
    {
        if (args.Length != 3)
        {
            throw new ArgumentException("substring requires three arguments");
        }

        int start = Convert.ToInt32(args[1]);
        int end = Convert.ToInt32(args[2]);

        return args[0] switch
        {
            string s => s[start..end],
            StringBuilder sb => sb.ToString()[start..end], // HACK: could this be improved?
            _ => throw new ArgumentException("substring's first argument must be a string")
        };
    }

    public static object? StringAppend(object?[] args)
    {
        var sb = new StringBuilder();

        foreach (var arg in args.ArgsToStringlikes())
        {
            sb.Append(arg);
        }

        return sb.ToString();
    }

    public static object? StringCopy(object?[] args)
    {
        if (args.Length is 0 or > 3)
        {
            throw new ArgumentException("string-copy requires one to three arguments");
        }

        if (args[0] is not string str)
        {
            if (args[0] is StringBuilder sb)
            {
                str = sb.ToString();
            }
            else
            {
                throw new ArgumentException("string-copy's first argument must be a string");
            }
        }

        int start = 0, end = str.Length;

        if (args.Length > 1)
        {
            start = Convert.ToInt32(args[1]);
        }

        if (args.Length == 3)
        {
            end = Convert.ToInt32(args[2]);
        }

        return new StringBuilder(str[start..end]);
    }

    public static object? StringCopyTo(object?[] args)
    {
        if (args.Length is < 3 or > 5)
        {
            throw new ArgumentException("string-copy! requires 3 to 5 arguments");
        }

        if (args[0] is not StringBuilder to)
        {
            throw new ArgumentException("string-copy!'s first argument must be a mutable string");
        }

        if (args[2] is not string from)
        {
            if (args[2] is StringBuilder sbfrom)
            {
                from = sbfrom.ToString(); // TODO: is there a more efficient way of doing this?
            }
            else
            {
                throw new ArgumentException("string-copy!'s third argument must be a string");
            }
        }

        var at = Convert.ToInt32(args[1]);
        int start = 0, end = from.Length;

        if (args.Length > 3)
        {
            start = Convert.ToInt32(args[3]);
        }

        if (args.Length == 5)
        {
            end = Convert.ToInt32(args[4]);
        }

        if ((to.Length - at) < (end - start))
        {
            throw new ArgumentException("(- (string-length to) at) must not be less than (- end start)");
        }

        for (int i = start; i < end; i++)
        {
            to[at++] = from[i];
        }

        return Nil.Value; // TODO: is this correct?
    }

    public static object? StringFill(object?[] args)
    {
        if (args.Length is < 2 or > 4)
        {
            throw new ArgumentException("string-fill! requires 2 to 4 arguments");
        }

        if (args[0] is not StringBuilder sb)
        {
            throw new ArgumentException("string-fill!'s first argument must be a mutable string");
        }

        if (args[1] is not char fill)
        {
            throw new ArgumentException("string-fill!'s second argument must be a character");
        }

        int start = 0, end = sb.Length;

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
            sb[i] = fill;
        }

        return Nil.Value; // TODO: is this correct?
    }
}