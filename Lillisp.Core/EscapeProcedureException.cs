using System;

namespace Lillisp.Core;

public sealed class EscapeProcedureException : Exception
{
    public EscapeProcedureException(object? returnValue)
    {
        ReturnValue = returnValue;
    }

    public object? ReturnValue { get; }
}