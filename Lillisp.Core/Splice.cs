using System.Collections.Generic;

namespace Lillisp.Core;

public class Splice
{
    public Splice(IEnumerable<object?> values)
    {
        Values = values;
    }

    public IEnumerable<object?> Values { get; }
}