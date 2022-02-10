namespace Lillisp.Core.Macros;

public static class MathMacros
{
    public static object? Increment(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length != 1 || args[0] is not Symbol symbol)
        {
            throw new ArgumentException("++! requires one symbol argument");
        }

        object? value = scope.Resolve(symbol.Value);

        double d = Convert.ToDouble(value) + 1;

        scope.Set(symbol.Value, d);

        return symbol;
    }

    public static object? Decrement(LillispRuntime runtime, Scope scope, object?[] args)
    {
        if (args.Length != 1 || args[0] is not Symbol symbol)
        {
            throw new ArgumentException("++! requires one symbol argument");
        }

        object? value = scope.Resolve(symbol.Value);

        double d = Convert.ToDouble(value) - 1;

        scope.Set(symbol.Value, d);

        return symbol;
    }
}