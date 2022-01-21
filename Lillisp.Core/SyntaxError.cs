using System;
using System.Collections.Generic;

namespace Lillisp.Core;

public class SyntaxError : Exception
{
    public SyntaxError(string message, IList<object?> args)
        : base(message)
    {
        Args = args;
    }

    public IList<object?> Args { get; set; }
}