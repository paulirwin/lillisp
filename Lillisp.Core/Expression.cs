namespace Lillisp.Core
{
    public delegate object? Expression(Scope scope, object?[] args);

    public delegate object? MacroExpression(LillispRuntime runtime, Scope scope, object?[] args);
}
