using System.Collections.Generic;
using System.Linq;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests;

public class ValuesTests
{
    [Fact]
    public void BasicValuesTest()
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram("(values 1 2 3)") as IEnumerable<object>;

        Assert.NotNull(result);

        var list = result.ToList();

        Assert.Equal(1, list[0]);
        Assert.Equal(2, list[1]);
        Assert.Equal(3, list[2]);
    }
}