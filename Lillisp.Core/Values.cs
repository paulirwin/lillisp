using System.Collections;
using System.Collections.Generic;

namespace Lillisp.Core;

public class Values : IEnumerable<object?>
{
    private readonly IEnumerable<object?> _values;

    public Values(IEnumerable<object?> values)
    {
        _values = values;
    }

    public Values(params object?[] values)
    {
        _values = values;
    }

    public IEnumerator<object?> GetEnumerator() => _values.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}