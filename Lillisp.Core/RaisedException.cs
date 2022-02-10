using System;

namespace Lillisp.Core;

public sealed class RaisedException : Exception
{
    public RaisedException(object? expression)
    {
        Expression = expression;
    }

    public object? Expression { get; }
}