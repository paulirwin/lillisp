using System.Text.RegularExpressions;

namespace Lillisp.Core.Expressions;

public static class RegularExpressions
{
    public static object? IsMatch(object?[] args)
    {
        if (args.Length != 2)
        {
            throw new ArgumentException("match? requires two arguments");
        }

        if (args[0] is not Regex rx)
        {
            throw new ArgumentException("match?'s first argument must be a regular expression");
        }
        
        var input = args[1]?.ToString() ?? throw new ArgumentException("match?'s second argument must not be null");

        return rx.IsMatch(input);
    }
}