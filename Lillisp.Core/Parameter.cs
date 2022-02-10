namespace Lillisp.Core;

public class Parameter : IInvokable
{
    public Parameter(object? init, Procedure? converter = null)
    {
        Value = init;
        Converter = converter;
    }

    public object? Value { get; }

    public Procedure? Converter { get; }

    public object? Invoke(LillispRuntime runtime, Scope scope, object?[] args) => Value;

    public override string ToString()
    {
        return $"Parameter{{{nameof(Value)}: {Value}, {nameof(Converter)}: {Converter?.ToString() ?? "null"}}}";
    }
}