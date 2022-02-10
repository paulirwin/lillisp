namespace Lillisp.Core.Expressions;

public static class CharacterExpressions
{
    public static object? Equals(object?[] args)
    {
        char? last = null;

        foreach (var arg in args.Cast<char>())
        {
            if (last != null && arg != last)
                return false;

            last = arg;
        }

        return true;
    }

    public static object? LessThan(object?[] args)
    {
        char? last = null;

        foreach (var arg in args.Cast<char>())
        {
            if (last >= arg)
                return false;

            last = arg;
        }

        return true;
    }

    public static object? GreaterThan(object?[] args)
    {
        char? last = null;

        foreach (var arg in args.Cast<char>())
        {
            if (last <= arg)
                return false;

            last = arg;
        }

        return true;
    }

    public static object? LessThanOrEqualTo(object?[] args)
    {
        char? last = null;

        foreach (var arg in args.Cast<char>())
        {
            if (last > arg)
                return false;

            last = arg;
        }

        return true;
    }

    public static object? GreaterThanOrEqualTo(object?[] args)
    {
        char? last = null;

        foreach (var arg in args.Cast<char>())
        {
            if (last < arg)
                return false;

            last = arg;
        }

        return true;
    }

    public static object? CaseInsensitiveEquals(object?[] args)
    {
        char? last = null;

        foreach (var arg in args.Cast<char>().Select(char.ToUpperInvariant))
        {
            if (last != null && arg != last)
                return false;

            last = arg;
        }

        return true;
    }

    public static object? CaseInsensitiveLessThan(object?[] args)
    {
        char? last = null;

        foreach (var arg in args.Cast<char>().Select(char.ToUpperInvariant))
        {
            if (last >= arg)
                return false;

            last = arg;
        }

        return true;
    }

    public static object? CaseInsensitiveGreaterThan(object?[] args)
    {
        char? last = null;

        foreach (var arg in args.Cast<char>().Select(char.ToUpperInvariant))
        {
            if (last <= arg)
                return false;

            last = arg;
        }

        return true;
    }

    public static object? CaseInsensitiveLessThanOrEqualTo(object?[] args)
    {
        char? last = null;

        foreach (var arg in args.Cast<char>().Select(char.ToUpperInvariant))
        {
            if (last > arg)
                return false;

            last = arg;
        }

        return true;
    }

    public static object? CaseInsensitiveGreaterThanOrEqualTo(object?[] args)
    {
        char? last = null;

        foreach (var arg in args.Cast<char>().Select(char.ToUpperInvariant))
        {
            if (last < arg)
                return false;

            last = arg;
        }

        return true;
    }

    public static object? IsAlphabetic(object?[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("A character is required.");
        }

        return args[0] is char c && char.IsLetter(c);
    }

    public static object? IsNumeric(object?[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("A character is required.");
        }

        return args[0] is char c && char.IsNumber(c);
    }

    public static object? IsWhitespace(object?[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("A character is required.");
        }

        return args[0] is char c && char.IsWhiteSpace(c);
    }

    public static object? IsUpperCase(object?[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("A character is required.");
        }

        return args[0] is char c && char.IsUpper(c);
    }

    public static object? IsLowerCase(object?[] args)
    {
        if (args.Length == 0)
        {
            throw new ArgumentException("A character is required.");
        }

        return args[0] is char c && char.IsLower(c);
    }

    public static object? DigitValue(object?[] args)
    {
        if (args.Length == 0 || args[0] is not char c)
        {
            throw new ArgumentException("digit-value's first parameter must be a character");
        }

        return char.IsDigit(c) ? (int)char.GetNumericValue(c) : false;
    }

    public static object? Upcase(object?[] args)
    {
        if (args.Length == 0 || args[0] is not char c)
        {
            throw new ArgumentException("char-upcase's first parameter must be a character");
        }

        return char.ToUpper(c);
    }

    public static object? Downcase(object?[] args)
    {
        if (args.Length == 0 || args[0] is not char c)
        {
            throw new ArgumentException("char-downcase's first parameter must be a character");
        }

        return char.ToLower(c);
    }

    public static object? Foldcase(object?[] args)
    {
        if (args.Length == 0 || args[0] is not char c)
        {
            throw new ArgumentException("char-foldcase's first parameter must be a character");
        }

        return char.ToLowerInvariant(c);
    }
}