using Lillisp.Core;

namespace Lillisp.Tests;

public static class TestHelper
{
    public static void DefaultTest(string input, object? expected)
    {
        var runtime = new LillispRuntime();

        var result = runtime.EvaluateProgram(input);
        
        if (expected is IEnumerable<object?> objEnumerable)
        {
            var objArr = objEnumerable as object?[] ?? objEnumerable.ToArray();

            var enumerable = result as IEnumerable<object?>;

            Assert.NotNull(enumerable);

            var list = enumerable!.ToList();

            Assert.Equal(objArr.Length, list.Count);

            for (int i = 0; i < objArr.Length; i++)
            {
                Assert.Equal(objArr[i], list[i]);
            }
        }
        else if (expected == null)
        {
            Assert.Null(result);
        }
        else
        {
            Assert.Equal(expected, result);
        }
    }
}