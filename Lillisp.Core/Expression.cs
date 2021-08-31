namespace Lillisp.Core
{
    public delegate object? Expression(object?[] args);

    public delegate object? MacroExpression(LillispRuntime runtime, object?[] args);
}
