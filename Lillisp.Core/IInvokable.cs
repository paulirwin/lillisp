namespace Lillisp.Core;

public interface IInvokable
{
    object? Invoke(LillispRuntime runtime, Scope scope, object?[] args);
}