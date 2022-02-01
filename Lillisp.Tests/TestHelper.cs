using System.Collections.Generic;
using System.Linq;
using Lillisp.Core;
using Xunit;

namespace Lillisp.Tests
{
    public static class TestHelper
    {
        public static void DefaultTest(string input, object expected)
        {
            var runtime = new LillispRuntime();

            var result = runtime.EvaluateProgram(input);

            Assert.Equal(expected, result);

            if (expected is object[] objArr)
            {
                var enumerable = result as IEnumerable<object>;

                Assert.NotNull(enumerable);

                var list = enumerable.ToList();

                Assert.Equal(objArr.Length, list.Count);

                for (int i = 0; i < objArr.Length; i++)
                {
                    Assert.Equal(objArr[i], list[i]);
                }
            }
        }
    }
}
